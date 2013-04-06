/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.event.MouseAdapter;
import java.awt.event.MouseEvent;

import javax.swing.BorderFactory;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.ListSelectionModel;

import ares.controller.gui.lf.BGDialog;
import ares.controller.util.Localization;

public class FileSelectionDialog extends BGDialog {

	  private JPanel jContentPane = null;
	  private JPanel jPanel = null;
	  private JPanel listPanel = null;
	  private JButton okButton = null;
	  private JButton cancelButton = null;
	  private JList filesList = null;
	  
	  public FileSelectionDialog(JFrame parent, java.util.List<String> possibleFiles) {
		  super(parent, true);
		  files = possibleFiles;
  	  	  initialize();
	     setLocationRelativeTo(parent);
	  }
	  
	  private void initialize() {
		    this.setContentPane(getJContentPane());
		    this.setTitle(Localization.getString("FileSelectionDialog.SelectProject")); //$NON-NLS-1$
		    setEscapeButton(cancelButton);
		    getRootPane().setDefaultButton(okButton);
		    pack();
		  }
	  
	      public String getSelectedFile() {
	    	  return selectedFile;
	      }

		  public String getHelpPage() {
		    return "Project_Selection"; //$NON-NLS-1$
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
		      jContentPane.add(getListPanel(), BorderLayout.CENTER);
		    }
		    return jContentPane;
		  }
		  
		  private JPanel getListPanel() {
			  if (listPanel == null) {
				  listPanel = new JPanel(new BorderLayout(5, 5));
				  listPanel.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(), Localization.getString("FileSelectionDialog.Project"))); //$NON-NLS-1$
				  JScrollPane scrollPane = new JScrollPane(getFilesList());
				  listPanel.add(scrollPane, BorderLayout.CENTER);
				  JLabel label = new JLabel(Localization.getString("FileSelectionDialog.CurrentDirHint")); //$NON-NLS-1$
				  listPanel.add(label, BorderLayout.SOUTH);
			  }
			  return listPanel;
		  }
		  
		  private JList getFilesList() {
			  if (filesList == null) {
				  filesList = new JList(files.toArray());
				  filesList.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
				  filesList.addMouseListener(new MouseAdapter() {
					  public void mouseClicked(MouseEvent evt) {
						  if (evt.getClickCount() == 2 && filesList.getSelectedIndex() != -1) {
							  selectedFile = files.get(filesList.getSelectedIndex());
							  dispose();
						  }
					  }
				  });
			  }
			  return filesList;
		  }

		  /**
		   * This method initializes jPanel	
		   * 	
		   * @return javax.swing.JPanel	
		   */
		  private JPanel getJPanel() {
		    if (jPanel == null) {
		      jPanel = new JPanel();
		      jPanel.setLayout(new FlowLayout(FlowLayout.TRAILING, 10, 5));
		      jPanel.setPreferredSize(new Dimension(200, 40));
		      jPanel.add(getOkButton(), null);
		      jPanel.add(getCancelButton(), null);
		    }
		    return jPanel;
		  }
		  
		  private String selectedFile = null;
		  private java.util.List<String> files;

		  /**
		   * This method initializes okButton	
		   * 	
		   * @return javax.swing.JButton	
		   */
		  private JButton getOkButton() {
		    if (okButton == null) {
		    	okButton = new JButton();
		    	okButton.setText(Localization.getString("FileSelectionDialog.OK")); //$NON-NLS-1$
		    	okButton.addActionListener(new java.awt.event.ActionListener() {
		        public void actionPerformed(java.awt.event.ActionEvent e) {
		          selectedFile = filesList.getSelectedIndex() != -1 ? files.get(filesList.getSelectedIndex()) : null; 
		          dispose();
		        }
		      });
		    }
		    return okButton;
		  }

		  /**
		   * This method initializes cancelButton	
		   * 	
		   * @return javax.swing.JButton	
		   */
		  private JButton getCancelButton() {
		    if (cancelButton == null) {
		      cancelButton = new JButton();
		      cancelButton.setText(Localization.getString("FileSelectionDialog.Cancel")); //$NON-NLS-1$
		      cancelButton.addActionListener(new java.awt.event.ActionListener() {
		        public void actionPerformed(java.awt.event.ActionEvent e) {
		          selectedFile = null;
		          dispose();
		        }
		      });
		    }
		    return cancelButton;
		  }
}
