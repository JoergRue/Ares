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

import javax.swing.JPanel;
import java.awt.BorderLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;
import java.util.List;
import java.util.prefs.Preferences;

import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.ImageIcon;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.border.EtchedBorder;
import javax.swing.table.DefaultTableColumnModel;
import javax.swing.table.DefaultTableModel;
import javax.swing.table.TableColumn;

import ares.controller.gui.lf.BGTableCellRenderer;
import ares.controller.gui.lf.Colors;
import ares.controller.gui.util.CellRenderers;
import ares.controller.gui.util.TableSorter;
import ares.controller.messages.IMessageListener;
import ares.controller.messages.Message;
import ares.controller.messages.Messages;
import ares.controller.util.Localization;

class MessageFrame extends SubFrame implements IMessageListener {
	
	private static class MyTableModel extends DefaultTableModel
	{
		public boolean isCellEditable(int row, int column) {
			return false;
		}
	}

  private JPanel jContentPane = null;
  private JPanel upperPanel = null;
  private JScrollPane jScrollPane = null;
  private JButton clearButton = null;
  private JTable messagesTable = null;
  private JComboBox levelCombo = null;
  private MyTableModel tableModel = null;
  private TableSorter sorter = null;
  private int messageLevel;

  /**
   * This method initializes 
   * 
   */
  public MessageFrame() {
  	super(Localization.getString("MessageFrame.Messages")); //$NON-NLS-1$
    messageLevel = Preferences.userNodeForPackage(MessageFrame.class).getInt("MessageLevel", 1); //$NON-NLS-1$
  	initialize();
  	addExistingMessages();
  	Messages.getInstance().addObserver(this);
  }
  
  public void dispose() {
    Messages.getInstance().removeObserver(this);
    super.dispose();
  }

  private void addExistingMessages() {
    List<Message> messages = Messages.getInstance().getMessages();
    for (Message message : messages) {
      if (message.getType().ordinal() <= messageLevel)
      {
    	    tableModel.addRow(new Object[] { new CellRenderers.ImageAndText(getTypeText(message), getIcon(message)), message.getMessage() });
      }
    }
  }
  

  /**
   * This method initializes this
   * 
   */
  private void initialize() {
        this.setContentPane(getJContentPane());
        this.setTitle(Localization.getString("MessageFrame.Messages")); //$NON-NLS-1$
        this.setDefaultCloseOperation(javax.swing.JFrame.DISPOSE_ON_CLOSE);             		
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
      jContentPane.add(getJScrollPane(), BorderLayout.CENTER);
      jContentPane.add(getUpperPanel(), BorderLayout.NORTH);
    }
    return jContentPane;
  }
  
  private JPanel getUpperPanel() {
	  if (upperPanel == null) {
		  upperPanel = new JPanel();
		  BoxLayout layout = new BoxLayout(upperPanel, BoxLayout.LINE_AXIS);
		  upperPanel.setLayout(layout);
		  upperPanel.add(getClearButton());
		  upperPanel.add(Box.createHorizontalStrut(5));
		  upperPanel.add(new JLabel(Localization.getString("MessageFrame.MessageLevel"))); //$NON-NLS-1$
		  upperPanel.add(Box.createHorizontalStrut(5));
		  upperPanel.add(getLevelCombo());
		  upperPanel.add(Box.createHorizontalGlue());
		  upperPanel.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 0));
	  }
	  return upperPanel;
  }
  
  private JButton getClearButton() {
	  if (clearButton == null) {
		  clearButton = new JButton(Localization.getString("MessageFrame.Clear")); //$NON-NLS-1$
		  clearButton.addActionListener(new ActionListener() {
			  public void actionPerformed(ActionEvent e0) {
				  clearTable();
			  }
		  });
	  }
	  return clearButton;
  }
  
  private JComboBox getLevelCombo() {
	  if (levelCombo == null) {
		  levelCombo = new JComboBox();
		  levelCombo.addItem(Localization.getString("MessageFrame.Error")); //$NON-NLS-1$
		  levelCombo.addItem(Localization.getString("MessageFrame.Warning")); //$NON-NLS-1$
		  levelCombo.addItem(Localization.getString("MessageFrame.Information")); //$NON-NLS-1$
		  levelCombo.addItem(Localization.getString("MessageFrame.Debug")); //$NON-NLS-1$
		  levelCombo.setSelectedIndex(messageLevel);
		  levelCombo.addItemListener(new ItemListener() {
			public void itemStateChanged(ItemEvent arg0) {
				messageLevel = levelCombo.getSelectedIndex();
				clearTable();
				addExistingMessages();
			}
		  });
	  }
	  return levelCombo;
  }

  /**
   * This method initializes jScrollPane	
   * 	
   * @return javax.swing.JScrollPane	
   */
  private JScrollPane getJScrollPane() {
    if (jScrollPane == null) {
      jScrollPane = new JScrollPane(getTable());
      jScrollPane.setOpaque(false);
      jScrollPane.getViewport().setOpaque(false);
      jScrollPane.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
      //jScrollPane.setViewportView(getTextArea());
    }
    return jScrollPane;
  }
  
  private void clearTable()
  {
	  tableModel.setRowCount(0);
  }

  /**
   * This method initializes textArea	
   * 	
   * @return javax.swing.JTextArea	
   */
  private JTable getTable() {
    if (messagesTable == null) {
      tableModel = new MyTableModel();
      DefaultTableColumnModel tcm = new DefaultTableColumnModel();
      tableModel.addColumn(Localization.getString("MessageFrame.Type")); //$NON-NLS-1$
      tableModel.addColumn(Localization.getString("MessageFrame.Message")); //$NON-NLS-1$
      tcm.addColumn(new TableColumn(0, 45));
      tcm.addColumn(new TableColumn(1, 450));
      sorter = new TableSorter(tableModel);
      messagesTable = new JTable(sorter, tcm);
      messagesTable.setCellSelectionEnabled(false);
      sorter.setTableHeader(messagesTable.getTableHeader());
      for (int i = 0; i < messagesTable.getColumnCount(); ++i)
          tcm.getColumn(i).setHeaderValue(messagesTable.getColumnName(i));

      messagesTable.setColumnSelectionAllowed(false);
      messagesTable.setIntercellSpacing(new java.awt.Dimension(6, 6));
      messagesTable.setRowSelectionAllowed(true);
      messagesTable.setSelectionMode(javax.swing.ListSelectionModel.SINGLE_SELECTION);
      messagesTable.setRowHeight(22);
      if (Colors.hasCustomColors())
      {
    	  BGTableCellRenderer renderer = new BGTableCellRenderer();
    	  messagesTable.setDefaultRenderer(Object.class, renderer);
    	  messagesTable.setSelectionBackground(Colors.getSelectedBackground());
    	  messagesTable.setSelectionForeground(Colors.getSelectedForeground());
    	  messagesTable.setOpaque(false);
      }
      messagesTable.getColumn(Localization.getString("MessageFrame.Type")).setCellRenderer(CellRenderers.createImageCellRenderer()); //$NON-NLS-1$
      messagesTable.setBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED));
    }
    return messagesTable;
  }
  
  private ImageIcon errorIcon, warningIcon, infoIcon, debugIcon;
  
  private ImageIcon getErrorIcon() {
	  if (errorIcon == null) {
		  errorIcon = new ImageIcon(getClass().getResource("eventlogError.png")); //$NON-NLS-1$
	  }
	  return errorIcon;
  }
  
  private ImageIcon getWarningIcon() {
	  if (warningIcon == null) {
		  warningIcon = new ImageIcon(getClass().getResource("eventlogWarn.png")); //$NON-NLS-1$
	  }
	  return warningIcon;
  }
  
  private ImageIcon getInfoIcon() {
	  if (infoIcon == null) {
		  infoIcon = new ImageIcon(getClass().getResource("eventlogInfo.png")); //$NON-NLS-1$
	  }
	  return infoIcon;
  }
  
  private ImageIcon getDebugIcon() {
	  if (debugIcon == null) {
		  debugIcon = new ImageIcon(getClass().getResource("gear.png")); //$NON-NLS-1$
	  }
	  return debugIcon;
  }
  
  private ImageIcon getIcon(Message message) {
	  switch (message.getType()) {
	  case Debug:
		  return getDebugIcon();
	  case Info:
		  return getInfoIcon();
	  case Warning:
		  return getWarningIcon();
	  default:
		  return getErrorIcon();
	  }
  }
  
  private String getTypeText(Message message) {
	  switch (message.getType()) {
	  case Debug:
		  return Localization.getString("MessageFrame.Debug"); //$NON-NLS-1$
	  case Info:
		  return Localization.getString("MessageFrame.Information"); //$NON-NLS-1$
	  case Warning:
		  return Localization.getString("MessageFrame.Warning"); //$NON-NLS-1$
	  default:
		  return Localization.getString("MessageFrame.Error"); //$NON-NLS-1$
	  }
  }

  public void messageAdded(Message message) {
    int messageLevel = Preferences.userNodeForPackage(MessageFrame.class).getInt("MessageLevel", 1); //$NON-NLS-1$
    if (message.getType().ordinal() > messageLevel) return;
    tableModel.addRow(new Object[] { new CellRenderers.ImageAndText(getTypeText(message), getIcon(message)), message.getMessage() });
  }

}  //  @jve:decl-index=0:visual-constraint="10,10"
