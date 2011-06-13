/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
package ares.controller.gui;

import java.awt.Component;
import java.awt.Rectangle;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ComponentAdapter;
import java.awt.event.ComponentEvent;
import java.awt.event.ComponentListener;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.prefs.Preferences;

import javax.swing.Timer;

import ares.controllers.util.AbstractObservable;

/*
import dsa.gui.util.Help;
import dsa.gui.util.HelpProvider;
*/

/**
 * 
 */
public class FrameManagement extends AbstractObservable<FrameStateChanger> {
  
  private final class WindowSnapper extends ComponentAdapter {

    boolean ud, lr;

    boolean doSnap = true;
    
    private Timer timer;
    
    private Component component;
    
    public WindowSnapper() {
      final int DELAY = 300;
      timer = new Timer(DELAY, new ActionListener() {
        public void actionPerformed(ActionEvent e) {
          if (doSnap) {
            Rectangle oldPos = positions.get(component);
            Rectangle newPos = component.getBounds();
            if (newPos.equals(oldPos)) return;
            snapComponent(component);
            positions.put(component, component.getBounds());
          }          
        }
      });
      timer.setRepeats(false);
    }

    private void snap(Component frame, Component other) {
      if (frame == other) return;
      int threshold = 10;
      Rectangle coords = frame.getBounds();
      Rectangle otherCoords = other.getBounds();
      // check whether right/left might be snapped
      if ((coords.y >= otherCoords.y && coords.y <= otherCoords.y
          + otherCoords.height)
          || (coords.y + coords.height >= otherCoords.y && coords.y <= otherCoords.y)) {
        // snap right border
        if (Math.abs(coords.x + coords.width - otherCoords.x) < threshold) {
          if (!lr) coords.x = otherCoords.x - coords.width;
          lr = true;
        }
        // snap left border
        else if (Math.abs(coords.x - (otherCoords.x + otherCoords.width)) < threshold) {
          if (!lr) coords.x = otherCoords.x + otherCoords.width;
          lr = true;
        }
      }
      // check whether top/bottom might be snapped
      if ((coords.x >= otherCoords.x && coords.x <= otherCoords.x
          + otherCoords.width)
          || (coords.x <= otherCoords.x && coords.x + coords.width >= otherCoords.x)) {
        // snap upper border
        if (Math.abs(coords.y - (otherCoords.y + otherCoords.height)) < threshold) {
          if (!ud) coords.y = otherCoords.y + otherCoords.height;
          ud = true;
        }
        // snap lower border
        else if (Math.abs(coords.y + coords.height - otherCoords.y) < threshold) {
          if (!ud) coords.y = otherCoords.y - coords.height;
          ud = true;
        }
      }
      frame.setBounds(coords);
    }

    private void snapComponent(Component c) {
      lr = false;
      ud = false;
      doSnap = false;
      for (SubFrame other : frames) {
        snap(c, other);
      }
      snap(c, mainFrame);
      doSnap = true;
    }

    public void componentMoved(ComponentEvent e) {
      component = e.getComponent();
      if (timer.isRunning()) {
        timer.restart();
      }
      else {
        timer.start();
      }
    }

    public void componentResized(ComponentEvent e) {
      component = e.getComponent();
      if (timer.isRunning()) {
        timer.restart();
      }
      else {
        timer.start();
      }
    }
  }

  private static final class ApplicationCloser implements Runnable {
    public void run() {
      ares.controller.control.Main.exit(0);
    }
  }

  private static FrameManagement instance = new FrameManagement();

  public static FrameManagement getInstance() {
    return instance;
  }
  
  private SubFrame mainFrame;
  
  /*
  private static class HelpListener extends KeyAdapter implements PropertyChangeListener {
    
    private Component focusOwner = null;

    public void propertyChange(PropertyChangeEvent evt) {
      Object o = evt.getNewValue();
      if (o instanceof Component) {
        if (focusOwner != null) focusOwner.removeKeyListener(this);
        focusOwner = (Component) o;
        if (focusOwner != null) focusOwner.addKeyListener(this);
      }
    }
    
    public void keyPressed(KeyEvent e) {
      if (focusOwner == null) return;
      if (e.getKeyCode() == KeyEvent.VK_F1) {
        Component c = focusOwner;
        while (c != null && (!(c instanceof HelpProvider))) {
          c = c.getParent();
        }
        if (c != null) {
          showHelp((HelpProvider)c);
        }
      }
    }
    
    private void showHelp(HelpProvider provider)
    {
      String page = provider.getHelpPage();
      if (page != null) {
        Help.showPage(provider.getHelpParent(), page);
      }
    }
  }
  */

  private FrameManagement() {
    frames = new java.util.LinkedList<SubFrame>();
    positions = new HashMap<Component, Rectangle>();
    windowSnapper = new WindowSnapper();
    // KeyboardFocusManager manager = KeyboardFocusManager.getCurrentKeyboardFocusManager();
    //manager.addPropertyChangeListener("permanentFocusOwner", new HelpListener());
  }

  private final java.util.LinkedList<SubFrame> frames;
  private HashMap<Component, Rectangle> positions;
  
  public Map<Component, Rectangle> getPositions() {
    return positions;
  }

  private final ComponentListener windowSnapper;

  private boolean isExiting = false;

  public void setExiting() {
    isExiting = true;
  }

  public java.util.List<SubFrame> getOpenFrames() {
    return frames;
  }

  public void registerFrame(SubFrame frame) {
    frame.addComponentListener(windowSnapper);
    positions.put(frame, frame.getBounds());
    if (frame instanceof MainFrame) {
      mainFrame = frame;
      return;
    }
    frames.add(frame);
    frame.addWindowListener(new WindowAdapter() {
      private void frameClosed(WindowEvent e) {
        frames.remove(e.getSource());
        positions.remove(e.getComponent());
        if (frames.size() == 0 && isExiting) {
          javax.swing.SwingUtilities.invokeLater(new ApplicationCloser());
        }
        FrameLayouts.getInstance().findCurrentLayout();
        for (FrameStateChanger frameStateChanger : observers) {
          frameStateChanger.frameStateChanged((SubFrame)e.getSource(), false);
        }
      }

      public void windowClosing(WindowEvent e) {
        frameClosed(e);
      }

      public void windowClosed(WindowEvent e) {
        frameClosed(e);
      }
    });
    FrameLayouts.getInstance().findCurrentLayout();
    List<FrameStateChanger> copy = new ArrayList<FrameStateChanger>();
    copy.addAll(observers);
    for (FrameStateChanger frameStateChanger : copy) {
      frameStateChanger.frameStateChanged(frame, true);
    }
  }

  public void closeAllFrames(SubFrame caller) {
    List<FrameStateChanger> copy = new ArrayList<FrameStateChanger>();
    copy.addAll(observers);
    for (SubFrame frame : frames)
      if (frame != null && frame.isVisible() && frame != caller) {
        for (FrameStateChanger frameStateChanger : copy) {
          frameStateChanger.closeFrame(frame);
        }
      }
  }
  
  private boolean contains(SubFrame[] frames, SubFrame frame) {
    for (SubFrame entry : frames) {
      if (entry == frame)
        return true;
    }
    return false;
  }
  
  public void closeAllFrames(SubFrame[] exceptions) {
    List<FrameStateChanger> copy = new ArrayList<FrameStateChanger>();
    copy.addAll(observers);
    for (SubFrame frame : frames) 
      if (frame != null && frame.isVisible() && !contains(exceptions, frame)) {
        for (FrameStateChanger frameStateChanger : copy) {
          frameStateChanger.closeFrame(frame);
        }
      }
  }
  
  public void closeFrame(SubFrame frame) {
    if (frame != null) {
      List<FrameStateChanger> copy = new ArrayList<FrameStateChanger>();
      copy.addAll(observers);
      for (FrameStateChanger frameStateChanger : copy) {
        frameStateChanger.closeFrame(frame);
      }
    }
  }

  public void iconifyAllFrames(SubFrame caller) {
    for (SubFrame frame : frames)
      if (frame != null && frame.isVisible() && frame != caller) {
        frame.setState(java.awt.Frame.ICONIFIED);
      }
  }
  
  public void activateFrame(String title) {
      Preferences prefs = Preferences.userNodeForPackage(FrameManagement.class);
      if (prefs.getBoolean("BringWindowsToTop", true)) { //$NON-NLS-1$
    	    for (SubFrame frame : frames) {
    	        if (frame.getTitle().equals(title)) {
    	        	frame.toFront();
    	        }
    	    }
      }	  
  }
  
  public void openFrame(String title, Rectangle bounds) {
    java.awt.Dimension screen = java.awt.Toolkit.getDefaultToolkit()
      .getScreenSize();
    if (bounds.width > screen.width) bounds.width = screen.width;
    if (bounds.height > screen.height) bounds.height = screen.height;
    if (bounds.x < 0) bounds.x = 0;
    if (bounds.y < 0) bounds.y = 0;
    if (bounds.x + bounds.width > screen.width) bounds.x = screen.width - bounds.width;
    if (bounds.y + bounds.height > screen.height) bounds.y = screen.height - bounds.height;
    for (SubFrame frame : frames) {
      if (frame.getTitle().equals(title)) {
        Preferences prefs = Preferences.userNodeForPackage(FrameManagement.class);
        if (prefs.getBoolean("BringWindowsToTop", true)) { //$NON-NLS-1$
          frame.toFront();
        }
        frame.setBounds(bounds);
        return;
      }
    }
    List<FrameStateChanger> copy = new ArrayList<FrameStateChanger>();
    copy.addAll(observers);
    for (FrameStateChanger frameStateChanger : copy) {
      frameStateChanger.openFrame(title, bounds);
    }
  }

  public SubFrame getFrame(String title) {
    for (SubFrame frame : frames)
      if (frame.getTitle().equals(title)) return frame;
    return null;
  }

  public void deiconifyAllFrames(SubFrame caller) {
    for (SubFrame frame : frames)
      if (frame != null && frame != caller)
        frame.setState(java.awt.Frame.NORMAL);
  }
  
  private static int activationCount = 0;

  public void activateAllFrames(SubFrame caller) {
    if (activationCount != 0) {
      --activationCount;
      return;
    }
    Preferences prefs = Preferences.userNodeForPackage(FrameManagement.class);    
    if (prefs.getBoolean("BringWindowsToTop", true)) { //$NON-NLS-1$
      for (SubFrame frame : frames)
        if (frame != null && frame.isVisible() && frame != caller) {
          ++activationCount;
          frame.toFront();
        }
      if (caller != null) {
        ++activationCount;
        caller.toFront();
      }
    }
  }
}
