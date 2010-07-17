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
package ares.controller.gui.lf;

import java.awt.Dialog;
import java.awt.Frame;
import java.awt.GraphicsConfiguration;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;

import javax.swing.JButton;
import javax.swing.JComponent;
import javax.swing.JDialog;
import javax.swing.JRootPane;
import javax.swing.KeyStroke;
import javax.swing.UIManager;

public abstract class BGDialog extends JDialog {
  // implements HelpProvider, de.javasoft.plaf.synthetica.HelpButtonTitlePane.HelpCallback {

  public BGDialog() {
    super();
  }

  public BGDialog(Frame owner) {
    super(owner);
  }

  public BGDialog(Frame owner, boolean modal)  {
    super(owner, modal);
  }

  public BGDialog(Frame owner, String title)  {
    super(owner, title);
  }

  public BGDialog(Frame owner, String title, boolean modal)
       {
    super(owner, title, modal);
  }

  public BGDialog(Frame owner, String title, boolean modal,
      GraphicsConfiguration gc) {
    super(owner, title, modal, gc);
  }

  public BGDialog(Dialog owner)  {
    super(owner);
  }

  public BGDialog(Dialog owner, boolean modal)  {
    super(owner, modal);
  }

  public BGDialog(Dialog owner, String title)  {
    super(owner, title);
  }

  public BGDialog(Dialog owner, String title, boolean modal)
       {
    super(owner, title, modal);
  }

  public BGDialog(Dialog owner, String title, boolean modal,
      GraphicsConfiguration gc)  {
    super(owner, title, modal, gc);
  }
  
  //public String getHelpPage() {
    //return null;
  //}

  /*
  public void callHelp() {
    String page = getHelpPage();
    if (page != null) {
      Help.showPage(getHelpParent(), page);
    }    
  }
  */
  public final java.awt.Component getHelpParent() {
    return this;
  }

  protected JRootPane createRootPane() {
    JRootPane pane = null;
    String rootPaneClass = UIManager.getString("ares.controller.gui.rootPaneClass"); //$NON-NLS-1$
    if (rootPaneClass == null || rootPaneClass.equals("")) { //$NON-NLS-1$
      pane = super.createRootPane();
    }
    else
      try {
        pane = (JRootPane) Class.forName(rootPaneClass).newInstance();
      }
      catch (ClassNotFoundException e) {
        e.printStackTrace();
        pane = super.createRootPane();
      }
      catch (InstantiationException e) {
        e.printStackTrace();
        pane = super.createRootPane();
      }
      catch (IllegalAccessException e) {
        e.printStackTrace();
        pane = super.createRootPane();
      }
     if (pane != null) {
       registerEscape(pane);
     }
     return pane;
  }
  
  private JButton escapeButton;
  
  protected void setEscapeButton(JButton button) {
    escapeButton = button;
  }
  
  private void registerEscape(JRootPane pane) {
    KeyStroke stroke = KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0);
    pane.registerKeyboardAction(new ActionListener() {
      public void actionPerformed(ActionEvent e) {
        if (escapeButton != null) escapeButton.doClick();
      }
    }, stroke, JComponent.WHEN_IN_FOCUSED_WINDOW);
  }

}
