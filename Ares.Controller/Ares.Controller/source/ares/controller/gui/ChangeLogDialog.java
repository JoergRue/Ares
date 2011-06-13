/*
    Copyright (c) 2011 [Joerg Ruedenauer]
  
    This file is part of Ares.

    Ares is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    ARes is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with ARes; if not, write to the Free Software
    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
package ares.controller.gui;

import ares.controller.control.OnlineOperations;
import ares.controllers.control.Version;
import ares.controller.gui.lf.BGDialog;
import ares.controller.util.Localization;

import java.awt.Dimension;
import javax.swing.JPanel;
import java.awt.BorderLayout;
import javax.swing.JButton;

import javax.swing.JFrame;
import javax.swing.JScrollPane;
import javax.swing.JTextPane;
import java.awt.FlowLayout;

public final class ChangeLogDialog extends BGDialog {

  private JPanel jContentPane = null;
  private JPanel jPanel = null;
  private JButton downloadButton = null;
  private JButton cancelButton = null;
  private JScrollPane jScrollPane = null;
  private JTextPane textPane = null;
  
  private Version version;
  
  /**
   * This method initializes 
   * 
   */
  public ChangeLogDialog(JFrame parent, String content, Version version) {
  	super(parent, true);
    this.version = version;
  	initialize();
    setLocationRelativeTo(parent);
    textPane.setText(content);
    if (content.length() > 0) textPane.setCaretPosition(0);
  }

  /**
   * This method initializes this
   * 
   */
  private void initialize() {
    this.setSize(new Dimension(489, 287));
    this.setContentPane(getJContentPane());
    this.setTitle(Localization.getString("ChangeLogDialog.History")); //$NON-NLS-1$
    setEscapeButton(cancelButton);
    getRootPane().setDefaultButton(downloadButton);
  }

  public String getHelpPage() {
    return "ChangeLog"; //$NON-NLS-1$
  }

  /**
   * This method initializes jContentPane	
   * 	
   * @return javax.swing.JPanel	
   */
  private JPanel getJContentPane() {
    if (jContentPane == null) {
      jContentPane = new JPanel();
      jContentPane.setLayout(new BorderLayout());
      jContentPane.add(getJPanel(), BorderLayout.SOUTH);
      jContentPane.add(getJScrollPane(), BorderLayout.CENTER);
    }
    return jContentPane;
  }

  /**
   * This method initializes jPanel	
   * 	
   * @return javax.swing.JPanel	
   */
  private JPanel getJPanel() {
    if (jPanel == null) {
      jPanel = new JPanel();
      jPanel.setLayout(new FlowLayout(FlowLayout.CENTER, 10, 5));
      jPanel.setPreferredSize(new Dimension(200, 40));
      jPanel.add(getDownloadButton(), null);
      jPanel.add(getCancelButton(), null);
    }
    return jPanel;
  }

  /**
   * This method initializes downloadButton	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getDownloadButton() {
    if (downloadButton == null) {
      downloadButton = new JButton();
      downloadButton.setText(Localization.getString("ChangeLogDialog.Download")); //$NON-NLS-1$
      downloadButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          dispose();
          java.awt.Container parent = ChangeLogDialog.this.getParent();
          if (parent instanceof JFrame) {
            OnlineOperations.downloadSetup((JFrame)parent, version);
          }
        }
      });
    }
    return downloadButton;
  }

  /**
   * This method initializes jButton1	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getCancelButton() {
    if (cancelButton == null) {
      cancelButton = new JButton();
      cancelButton.setText(Localization.getString("ChangeLogDialog.Cancel")); //$NON-NLS-1$
      cancelButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          dispose();
        }
      });
    }
    return cancelButton;
  }

  /**
   * This method initializes jScrollPane	
   * 	
   * @return javax.swing.JScrollPane	
   */
  private JScrollPane getJScrollPane() {
    if (jScrollPane == null) {
      jScrollPane = new JScrollPane();
      jScrollPane.setViewportView(getTextPane());
    }
    return jScrollPane;
  }

  /**
   * This method initializes textPane	
   * 	
   * @return javax.swing.JTextPane	
   */
  private JTextPane getTextPane() {
    if (textPane == null) {
      textPane = new JTextPane();
      textPane.setEditable(false);
    }
    return textPane;
  }

}  //  @jve:decl-index=0:visual-constraint="10,10"
