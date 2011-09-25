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

import java.awt.Rectangle;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.util.StringTokenizer;
import java.util.prefs.BackingStoreException;
import java.util.prefs.Preferences;

// import javax.swing.ImageIcon;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JRootPane;
import javax.swing.UIManager;

/*
import dsa.gui.util.Help;
import dsa.gui.util.HelpProvider;
*/
import ares.controller.util.Directories;
import ares.controller.util.Localization;

/**
 * 
 */
public abstract class SubFrame extends JFrame { 
    // implements HelpProvider, de.javasoft.plaf.synthetica.HelpButtonTitlePane.HelpCallback {

  private static boolean shallSave;

  private final class MyWindowListener extends WindowAdapter {

    public void windowActivated(WindowEvent e) {
      FrameManagement.getInstance().activateAllFrames(SubFrame.this);
    }
  }

  public static void setSaveLocations(boolean save) {
    shallSave = save;
  }

  private void storeBounds(Rectangle r) {
    PrintWriter out = null;
    try {
      File file = new File(Directories.getApplicationPath() + "daten" //$NON-NLS-1$
          + File.separator + "allframebounds_" //$NON-NLS-1$
          + ares.controllers.control.Version.getCurrentVersionString() + ".dat"); //$NON-NLS-1$
      if (file.exists()) {
        out = new PrintWriter(new BufferedWriter(new FileWriter(file, true)));
      }
      else {
        out = new PrintWriter(new BufferedWriter(new FileWriter(file)));
      }
      String line = getTitle() + ";" + r.x + ";" + r.y + ";" + r.width + ";" //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$ //$NON-NLS-4$
          + r.height;
      out.println(line);
      out.flush();
    }
    catch (IOException e) {
      javax.swing.JOptionPane.showMessageDialog(this,
          Localization.getString("SubFrame.WriteBoundsFailed") + e.getMessage()); //$NON-NLS-1$
    }
    finally {
      if (out != null) out.close();
    }
  }

  public static void loadAllBounds(boolean update) throws IOException {
    File file = new File(update ? Directories.getApplicationPath() + "daten" //$NON-NLS-1$
        + File.separator + "allframebounds_" //$NON-NLS-1$
        + ares.controllers.control.Version.getCurrentVersionString() + ".dat" : Directories //$NON-NLS-1$
        .getApplicationPath()
        + "daten" + File.separator + "allframebounds.dat"); //$NON-NLS-1$ //$NON-NLS-2$
    if (!file.exists()) return;
    Preferences prefs = Preferences.userNodeForPackage(SubFrame.class);
    BufferedReader in = new BufferedReader(new InputStreamReader(
        new FileInputStream(file), "ISO-8859-1")); //$NON-NLS-1$
    try {
      String line = in.readLine();
      while (line != null) {
        StringTokenizer tokenizer = new StringTokenizer(line, ";"); //$NON-NLS-1$
        if (tokenizer.countTokens() != 5) {
          throw new IOException(Localization.getString("SubFrame.WrongBoundsFormat")); //$NON-NLS-1$
        }
        String title = tokenizer.nextToken();
        int x = Integer.parseInt(tokenizer.nextToken());
        int y = Integer.parseInt(tokenizer.nextToken());
        int w = Integer.parseInt(tokenizer.nextToken());
        int h = Integer.parseInt(tokenizer.nextToken());
        prefs.putInt(title + "x", x); //$NON-NLS-1$
        prefs.putInt(title + "y", y); //$NON-NLS-1$
        prefs.putInt(title + "w", w); //$NON-NLS-1$
        prefs.putInt(title + "h", h); //$NON-NLS-1$
        line = in.readLine();
      }
    }
    catch (NumberFormatException e) {
      throw new IOException(e.getMessage());
    }
    finally {
      in.close();
    }
    if (!update) loadAllBounds(true);
    else {
      try {
        prefs.sync();
      }
      catch (BackingStoreException e) {
        throw new IOException(e);
      }
    }
  }

  public static void saveFrameBounds(String title, Rectangle r) {
    Preferences prefs = Preferences
      .userNodeForPackage(SubFrame.class);
    prefs.putInt(title + "x", r.x); //$NON-NLS-1$
    prefs.putInt(title + "y", r.y); //$NON-NLS-1$
    prefs.putInt(title + "w", r.width); //$NON-NLS-1$
    prefs.putInt(title + "h", r.height); //$NON-NLS-1$
  }
  
  public static Rectangle getSavedFrameBounds(String title) {
    Preferences prefs = Preferences
    .userNodeForPackage(SubFrame.class);
    int x = prefs.getInt(title + "x", 0); //$NON-NLS-1$
    int y = prefs.getInt(title + "y", 0); //$NON-NLS-1$
    int w = prefs.getInt(title + "w", 100); //$NON-NLS-1$
    int h = prefs.getInt(title + "h", 100); //$NON-NLS-1$
    return new Rectangle(x, y, w, h);
  }

  public SubFrame() {
    super();
  }

  public SubFrame(String title) {
	  this(title, new Rectangle(50, 50, 420, 100));
  }
  
  public SubFrame(String title, Rectangle defaultBounds) {
    super(title);
    this.addWindowListener(new MyWindowListener());
    setTitle(title);
    Preferences prefs = Preferences.userNodeForPackage(SubFrame.class);
    int x = prefs.getInt(title + "x", defaultBounds.x); //$NON-NLS-1$
    int y = prefs.getInt(title + "y", defaultBounds.y); //$NON-NLS-1$
    int w = prefs.getInt(title + "w", defaultBounds.width); //$NON-NLS-1$
    int h = prefs.getInt(title + "h", defaultBounds.height); //$NON-NLS-1$
    java.awt.Dimension screen = java.awt.Toolkit.getDefaultToolkit()
        .getScreenSize();
    if (w > screen.width) w = screen.width;
    if (h > screen.height) h = screen.height;
    if (x < 0) x = 0;
    if (y < 0) y = 0;
    if (x + w > screen.width) x = screen.width - w;
    if (y + h > screen.height) y = screen.height - h;
    this.setBounds(x, y, w, h);
    addWindowListener(new WindowAdapter() {
      private void saveBounds() {
        Rectangle r = getBounds();
        String title = getTitle();
        saveFrameBounds(title, r);
        if (shallSave) storeBounds(r);
      }

      public void windowClosing(WindowEvent e) {
        saveBounds();
      }

      public void windowClosed(WindowEvent e) {
        saveBounds();
      }
    });
    this.setIconImage(getIcon().getImage());
    FrameManagement.getInstance().registerFrame(this);
  }
  
  //public String getHelpPage() { return null; }
  
  public final java.awt.Component getHelpParent() { return this; }

  private static ImageIcon theIcon = null;
  
  private static ImageIcon getIcon() {
    if (theIcon == null) {
      theIcon = new ImageIcon(SubFrame.class.getResource("icon.png")); //$NON-NLS-1$
    }
    return theIcon;
  }
  

  protected JRootPane createRootPane() {
    String rootPaneClass = UIManager.getString("ares.controller.gui.rootPaneClass"); //$NON-NLS-1$
    if (rootPaneClass == null || rootPaneClass.equals("")) { //$NON-NLS-1$
      return super.createRootPane();
    }
    else
      try {
        return (JRootPane) Class.forName(rootPaneClass).newInstance();
      }
      catch (ClassNotFoundException e) {
        e.printStackTrace();
        return super.createRootPane();
      }
      catch (InstantiationException e) {
        e.printStackTrace();
        return super.createRootPane();
      }
      catch (IllegalAccessException e) {
        e.printStackTrace();
        return super.createRootPane();
      }
  }

  /*
  public void callHelp() {
    String page = getHelpPage();
    if (page != null) {
      Help.showPage(getHelpParent(), page);
    }    
  }
  */
  
}
