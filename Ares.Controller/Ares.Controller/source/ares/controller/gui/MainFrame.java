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


import java.awt.Dimension;
import javax.swing.JPanel;
import javax.swing.BorderFactory;
import javax.swing.border.EtchedBorder;
import javax.swing.border.TitledBorder;
import javax.swing.event.ChangeEvent;
import javax.swing.event.ChangeListener;
import javax.swing.AbstractAction;
import javax.swing.AbstractButton;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.GroupLayout;
import javax.swing.ImageIcon;
import javax.swing.JCheckBoxMenuItem;
import javax.swing.JFileChooser;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;
import javax.swing.JSlider;
import javax.swing.JButton;
import javax.swing.JToggleButton;
import javax.swing.KeyStroke;
import javax.swing.SpringLayout;
import javax.swing.SwingUtilities;

import java.awt.BorderLayout;
import java.awt.FontMetrics;
import java.awt.Graphics;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.WindowAdapter;
import java.awt.event.WindowEvent;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.prefs.Preferences;

import javax.swing.JComboBox;

import ares.controllers.control.Control;
import ares.controllers.control.KeyAction;
import ares.controller.control.OnlineOperations;
import ares.controllers.control.Version;
import ares.controllers.data.Command;
import ares.controllers.data.Configuration;
import ares.controllers.data.Mode;
import ares.controller.gui.util.ExampleFileFilter;
import ares.controllers.messages.IMessageListener;
import ares.controllers.messages.Message;
import ares.controllers.messages.Messages;
import ares.controllers.messages.Message.MessageType;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.IServerListener;
import ares.controllers.network.ServerInfo;
import ares.controllers.network.ServerSearch;
import ares.controller.util.Directories;
import ares.controller.util.Localization;

public final class MainFrame extends FrameController implements IMessageListener, IServerListener, INetworkClient, CommandsPanelCreator.IActionCreator {

  private JPanel jContentPane = null;
  private JPanel buttonsPanel = null;
  private JLabel jLabel = null;
  private JButton fileButton = null;
  private JPanel networkPanel = null;
  private JLabel jLabel1 = null;
  private JComboBox serverBox = null;
  private JButton connectButton = null;
  private JToggleButton messagesButton = null;
  private JButton settingsButton = null;
  private JButton infoButton = null;
  
  private ServerSearch serverSearch = null;
  private JPanel statePanel;
  
  private final class MyWindowListener extends WindowAdapter {

    public void windowIconified(WindowEvent e) {
      FrameManagement.getInstance().iconifyAllFrames(MainFrame.this);
    }

    public void windowDeiconified(WindowEvent e) {
      FrameManagement.getInstance().deiconifyAllFrames(MainFrame.this);
    }

  }

  /**
   * This method initializes 
   * 
   */
  public MainFrame() {
  	super(Localization.getString("MainFrame.SoundController")); //$NON-NLS-1$
  	initialize();
    this.addWindowListener(new MyWindowListener());
  	Messages.getInstance().addObserver(this);
  	Messages.addMessage(MessageType.Info, Localization.getString("MainFrame.RSCVersion") + Version.getCurrentVersionString() + Localization.getString("MainFrame.started")); //$NON-NLS-1$ //$NON-NLS-2$
  	ModeFrames.getInstance();
  	addButton(Localization.getString("MainFrame.Messages"), getMessagesButton()); //$NON-NLS-1$
  	enableProjectSpecificControls(false);
  	serverSearch = new ServerSearch(this, Preferences.userNodeForPackage(MainFrame.class).getInt("UDPPort", 8009)); //$NON-NLS-1$
  	serverSearch.startSearch();
    Control.getInstance().addAlwaysAvailableKeys(getRootPane());
    boolean checkForNewVersion = Preferences.userNodeForPackage(ares.controller.gui.OptionsDialog.class).getBoolean("CheckForUpdate", true); //$NON-NLS-1$
    if (checkForNewVersion) {
      javax.swing.SwingUtilities.invokeLater(new Runnable() {
        public void run() {
          OnlineOperations.checkForUpdate(MainFrame.this, false);
        }
      });
    }
  }
  
  public void dispose() {
    storeLayout();
    Control.getInstance().disconnect(true);
    Messages.getInstance().removeObserver(this);
    serverSearch.dispose();
    FrameManagement.getInstance().setExiting();
    FrameManagement.getInstance().closeAllFrames(this);
    super.dispose();
  }

  /**
   * This method initializes this
   * 
   */
  private void initialize() {
        this.setSize(new Dimension(456, 215));
        this.setContentPane(getJContentPane());
        this.setTitle(Localization.getString("MainFrame.SoundController")); //$NON-NLS-1$
        this.setDefaultCloseOperation(javax.swing.JFrame.DISPOSE_ON_CLOSE);    
        this.setJMenuBar(createMenuBar());
        pack();
  }
  
  private JMenuBar menuBar;
  
  private JMenuBar createMenuBar() {
	  if (menuBar == null) {
		  menuBar = new JMenuBar();
		  menuBar.add(getFileMenu());
		  menuBar.add(getPlayMenu());
		  menuBar.add(getExtrasMenu());
		  menuBar.add(getHelpMenu());
	  }
	  return menuBar;
  }
  
  private JMenu recentMenu;
  
  private JMenu getFileMenu() {
	  JMenu fileMenu = new JMenu(Localization.getString("MainFrame.Project")); //$NON-NLS-1$
	  JMenuItem openItem = new JMenuItem(Localization.getString("MainFrame.OpenMenu")); //$NON-NLS-1$
	  openItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  selectFile();
		  }
	  });
	  recentMenu = new JMenu(Localization.getString("MainFrame.Recent")); //$NON-NLS-1$
	  JMenuItem exitItem = new JMenuItem(Localization.getString("MainFrame.Exit")); //$NON-NLS-1$
	  exitItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  exitProgram();
		  }
	  });
	  fileMenu.add(openItem);
	  fileMenu.add(recentMenu);
	  fileMenu.addSeparator();
	  fileMenu.add(exitItem);
	  return fileMenu;
  }
  
  public void exitProgram()
  {
	  dispose();
  }
  
  private JMenu getPlayMenu() {
	  JMenu playMenu = new JMenu(Localization.getString("MainFrame.Play")); //$NON-NLS-1$
	  JMenuItem stopItem = new JMenuItem(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0)));
	  stopItem.setText(Localization.getString("MainFrame.StopAll")); //$NON-NLS-1$
	  JMenuItem previousItem = new JMenuItem(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_LEFT, 0)));
	  previousItem.setText(Localization.getString("MainFrame.PreviousMusic")); //$NON-NLS-1$
	  JMenuItem nextItem = new JMenuItem(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT, 0)));
	  nextItem.setText(Localization.getString("MainFrame.NextMusic")); //$NON-NLS-1$
	  playMenu.add(stopItem);
	  playMenu.add(previousItem);
	  playMenu.add(nextItem);
	  return playMenu;
  }
  
  private JMenuItem messagesMenuItem;
  
  private JMenu getExtrasMenu() {
	  JMenu extrasMenu = new JMenu(Localization.getString("MainFrame.Extras")); //$NON-NLS-1$
	  messagesMenuItem = new JCheckBoxMenuItem(Localization.getString("MainFrame.MessagesMenu")); //$NON-NLS-1$
	  messagesMenuItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  getMessagesButton().doClick();
		  }
	  });
	  JMenuItem settingsItem = new JMenuItem(Localization.getString("MainFrame.SettingsMenu")); //$NON-NLS-1$
	  settingsItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  showSettingsDialog();
		  }
	  });
	  extrasMenu.add(messagesMenuItem);
	  extrasMenu.addSeparator();
	  extrasMenu.add(settingsItem);
	  return extrasMenu;
  }
  
  private JMenu getHelpMenu() {
	  JMenu helpMenu = new JMenu(Localization.getString("MainFrame.Help")); //$NON-NLS-1$
	  JMenuItem helpItem = new JMenuItem(Localization.getString("MainFrame.HelpMenuItem")); //$NON-NLS-1$
	  helpItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  showHelpPage();
		  }
	  });
	  JMenuItem updateItem = new JMenuItem(Localization.getString("MainFrame.CheckForUpdate")); //$NON-NLS-1$
	  updateItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			  OnlineOperations.checkForUpdate(MainFrame.this, true);
		  }
	  });
	  JMenuItem aboutItem = new JMenuItem(Localization.getString("MainFrame.AboutMenu")); //$NON-NLS-1$
	  aboutItem.addActionListener(new ActionListener() {
		  public void actionPerformed(ActionEvent e) {
			 showAboutDialog();
		  }
	  });
	  helpMenu.add(helpItem);
	  helpMenu.add(updateItem);
	  helpMenu.addSeparator();
	  helpMenu.add(aboutItem);
	  return helpMenu;
  }
  
  private void storeLayout() {
    if (Control.getInstance().getConfiguration() != null) {
      File layoutsFile = new File(Directories.getUserDataPath() + Control.getInstance().getFileName() + ".layout"); //$NON-NLS-1$
      try {
        FrameLayouts.getInstance().storeToFile(layoutsFile.getAbsolutePath());
      }
      catch (IOException e) {
        Messages.addMessage(MessageType.Warning, Localization.getString("MainFrame.LayoutWriteError")); //$NON-NLS-1$
      }
    }    
  }
  
  private JPanel modesPanel;
  private JPanel modeCommandsPanel;
  
  private JPanel getModesPanel() {
	  if (modesPanel == null) {
		  modesPanel = new JPanel(new BorderLayout());
		  modesPanel.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED), 
				  Localization.getString("ModesFrame.Modes"), //$NON-NLS-1$
				  TitledBorder.DEFAULT_JUSTIFICATION, TitledBorder.DEFAULT_POSITION, null, null));
	  }
	  return modesPanel;
  }
  
  private void enableProjectSpecificControls(boolean enabled) {
	  getStopAllButton().setEnabled(enabled);
	  getNextTrackButton().setEnabled(enabled);
	  getPreviousButton().setEnabled(enabled);
	  if (!enabled) {
		  getModesPanel().setVisible(false);
		  if (modeCommandsPanel != null) {
			  getModesPanel().remove(modeCommandsPanel);
			  modeCommandsPanel = null;
		  }
	  }
	  else {
          Configuration config = Control.getInstance().getConfiguration();
          if (config == null) return;
          List<Mode> modes = config.getModes();
          if (modes.size() == 0) return;
          List<Command> commands = new ArrayList<Command>();
          commands.addAll(modes);
          if (modeCommandsPanel != null)
		  {
        	  getModesPanel().remove(modeCommandsPanel);
		  }
          modeCommandsPanel = CommandsPanelCreator.createPanel(commands, this, getRootPane(), 3);
          getModesPanel().add(modeCommandsPanel, BorderLayout.CENTER);
          getModesPanel().setVisible(true);
	  }
	  refillContentPane(getJContentPane());
	  pack();
  }
  
  private static void updateLastProjects(String path, String name) {
	  java.util.prefs.Preferences prefs = java.util.prefs.Preferences
	  	.userNodeForPackage(MainFrame.class);
	  int nrOfLastProjects = prefs.getInt("LastUsedProjectsCount", 0); //$NON-NLS-1$
	  if (nrOfLastProjects < 4) nrOfLastProjects++;
	  int previousPos = nrOfLastProjects - 1;
	  for (int i = nrOfLastProjects - 2; i >= 0; --i) {
		  String oldPath = prefs.get("LastUsedProjectFile" + i, ""); //$NON-NLS-1$ //$NON-NLS-2$
		  if (oldPath.equals(path)) {
			  previousPos = i;
			  --nrOfLastProjects;
			  break;
		  }
	  }
	  for (int i = previousPos; i > 0; --i) {
		  String oldPath = prefs.get("LastUsedProjectFile" + (i - 1), ""); //$NON-NLS-1$ //$NON-NLS-2$
		  String oldName = prefs.get("LastUsedProjectName" + (i - 1), ""); //$NON-NLS-1$ //$NON-NLS-2$
		  prefs.put("LastUsedProjectFile" + i, oldPath); //$NON-NLS-1$
		  prefs.put("LastUsedProjectName" + i, oldName); //$NON-NLS-1$
	  }
	  prefs.put("LastUsedProjectFile" + 0, path); //$NON-NLS-1$
	  prefs.put("LastUsedProjectName" + 0, name); //$NON-NLS-1$
	  prefs.putInt("LastUsedProjectsCount", nrOfLastProjects); //$NON-NLS-1$
  }

  public void openFile(String fileName) {
    File file = new File(fileName);
    if (!file.exists()) return;
    
    storeLayout();

    List<SubFrame> openFrames = new ArrayList<SubFrame>();
    openFrames.add(this);
    if (messageFrame != null) openFrames.add(messageFrame);
    SubFrame[] frames = new SubFrame[openFrames.size()];
    openFrames.toArray(frames);
    FrameManagement.getInstance().closeAllFrames(frames);
      
    Control.getInstance().openFile(file);
    if (Control.getInstance().getConfiguration() != null) {
      enableProjectSpecificControls(true);
      projectLabel.setText(Control.getInstance().getConfiguration().getTitle());
      
      File layoutsFile = new File(Directories.getUserDataPath() + file.getName() + ".layout"); //$NON-NLS-1$
      if (layoutsFile.exists()) try {
        FrameLayouts.getInstance().readFromFile(layoutsFile.getAbsolutePath());
        FrameLayouts.getInstance().restoreLastLayout();
      }
      catch (IOException e) {
        Messages.addMessage(MessageType.Warning, Localization.getString("MainFrame.LayoutReadError")); //$NON-NLS-1$
      }
      
      Preferences.userNodeForPackage(MainFrame.class).put("LastConfiguration", fileName); //$NON-NLS-1$
      updateLastProjects(fileName, Control.getInstance().getConfiguration().getTitle());
      rebuildLastProjectsMenu();
    }
    else {
      enableProjectSpecificControls(false);
      projectLabel.setText("-"); //$NON-NLS-1$
    }
  }

  /**
   * This method initializes jContentPane	
   * 	
   * @return javax.swing.JPanel	
   */
  private JPanel getJContentPane() {
    if (jContentPane == null) {
      jContentPane = new JPanel();
      refillContentPane(jContentPane);
    }
    return jContentPane;
  }
  
  private void refillContentPane(JPanel contentPane) {
	  contentPane.removeAll();
      GroupLayout layout = new GroupLayout(contentPane);
      contentPane.setLayout(layout);
      contentPane.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
      layout.setHorizontalGroup(layout.createParallelGroup()
    		  .addComponent(getButtonsPanel())
    		  .addComponent(getVolumesPanel())
    	      .addComponent(getStatePanel())
    	      .addComponent(getNetworkPanel())
    	      .addComponent(getModesPanel())
    	      );
      layout.setVerticalGroup(layout.createSequentialGroup()
    		  .addComponent(getButtonsPanel())
    		  .addComponent(getVolumesPanel())
    	      .addComponent(getStatePanel())
    	      .addComponent(getNetworkPanel())
    	      .addComponent(getModesPanel())
    	      );
  }

  /**
   * This method initializes jPanel	
   * 	
   * @return javax.swing.JPanel	
   */
  private JPanel getButtonsPanel() {
    if (buttonsPanel == null) {
      jLabel = new JLabel();
      jLabel.setText(Localization.getString("MainFrame.File")); //$NON-NLS-1$
      buttonsPanel = new JPanel();
      buttonsPanel.setLayout(new BorderLayout(5, 5));
      JPanel buttonPanel = new JPanel();
      buttonPanel.setLayout(new GridLayout(1, 5, 5, 5));
      buttonPanel.add(getFileButton());
      buttonPanel.add(getMessagesButton());
      buttonPanel.add(getSettingsButton());
      buttonPanel.add(getInfoButton());
      JPanel northPanel = new JPanel();
      northPanel.setLayout(new BoxLayout(northPanel, BoxLayout.LINE_AXIS));
      northPanel.add(buttonPanel);
      northPanel.add(Box.createHorizontalStrut(15));
      JPanel buttonPanel2 = new JPanel();
      buttonPanel2.setLayout(new GridLayout(1, 4, 5, 5));
      buttonPanel2.add(getStopAllButton());
      buttonPanel2.add(getPreviousButton());
      buttonPanel2.add(getNextTrackButton());
      northPanel.add(buttonPanel2);
      JPanel inner = new JPanel();
      inner.setLayout(new BorderLayout());
      inner.add(northPanel, BorderLayout.NORTH);
      buttonsPanel.add(inner, BorderLayout.WEST);
      buttonsPanel.setBorder(BorderFactory.createEmptyBorder(5, 0, 5, 0));
    }
    return buttonsPanel;
  }
  
  private JPanel volumesPanel;
  private JSlider mainVolumeSlider, musicVolumeSlider, soundVolumeSlider;
  
  private JPanel getVolumesPanel() {
	  if (volumesPanel == null) {
		  JPanel inner = new JPanel(new SpringLayout());
		  inner.add(new JLabel(Localization.getString("MainFrame.Overall"))); //$NON-NLS-1$
		  inner.add(getMainVolumeSlider());
		  inner.add(new JLabel(Localization.getString("MainFrame.Music"))); //$NON-NLS-1$
		  inner.add(getMusicVolumeSlider());
		  inner.add(new JLabel(Localization.getString("MainFrame.Sounds"))); //$NON-NLS-1$
		  inner.add(getSoundVolumeSlider());
		  ares.controller.gui.util.SpringUtilities.makeCompactGrid(inner, 3, 2, 5, 5, 5, 5);
		  inner.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED), 
				  Localization.getString("MainControlsFrame.Volume"), //$NON-NLS-1$
				  TitledBorder.DEFAULT_JUSTIFICATION, TitledBorder.DEFAULT_POSITION, null, null));
		  volumesPanel = new JPanel(new BorderLayout(5, 5));
		  volumesPanel.add(inner, BorderLayout.NORTH);
	  }
	  return volumesPanel;
  }
  
  enum VolumeType { Sounds, Music, Both }
  
  private JSlider getMainVolumeSlider() {
	  if (mainVolumeSlider == null) {
		  mainVolumeSlider = getSlider(VolumeType.Both);
	  }
	  return mainVolumeSlider;
  }
  
  private JSlider getMusicVolumeSlider() {
	  if (musicVolumeSlider == null) {
		  musicVolumeSlider = getSlider(VolumeType.Music);
	  }
	  return musicVolumeSlider;
  }
  
  private JSlider getSoundVolumeSlider() {
	  if (soundVolumeSlider == null) {
		  soundVolumeSlider = getSlider(VolumeType.Sounds);
	  }
	  return soundVolumeSlider;
  }
  
  private boolean listenForVolume = true;
  
  private JSlider getSlider(final VolumeType volType) {
	  JSlider slider = new JSlider(JSlider.HORIZONTAL, 0, 100, 100);
	  slider.setMajorTickSpacing(10);
	  slider.setPaintLabels(false);
	  slider.setPaintTicks(false);
	  slider.addChangeListener(new ChangeListener() {

		public void stateChanged(ChangeEvent arg0) {
			if (!listenForVolume) return;
			Control.getInstance().setVolume(volType.ordinal(), ((JSlider)arg0.getSource()).getValue());
		}
	  });
	  return slider;
  }

  private JLabel projectLabel, modeLabel, elementsLabel, musicLabel, elementsDescLabel;
  
  private JPanel getStatePanel() {
	if (statePanel == null) {
	  JPanel inner = new JPanel(new SpringLayout());
	  modeLabel = new JLabel();
	  elementsLabel = new JLabel();
	  musicLabel = new JLabel();
	  projectLabel = new JLabel();
	  inner.add(new JLabel(Localization.getString("MainFrame.Configuration"))); //$NON-NLS-1$
	  inner.add(projectLabel);
	  inner.add(new JLabel(Localization.getString("MainFrame.Mode"))); //$NON-NLS-1$
	  inner.add(modeLabel);
	  elementsDescLabel = new JLabel(Localization.getString("MainFrame.Elements")); //$NON-NLS-1$ 
	  inner.add(elementsDescLabel);
	  inner.add(elementsLabel);
	  inner.add(new JLabel(Localization.getString("MainFrame.Music"))); //$NON-NLS-1$
	  inner.add(musicLabel);
	  ares.controller.gui.util.SpringUtilities.makeCompactGrid(inner, 4, 2, 5, 5, 5, 5);
	  inner.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED), Localization.getString("MainFrame.Status"), TitledBorder.DEFAULT_JUSTIFICATION, TitledBorder.DEFAULT_POSITION, null, null)); //$NON-NLS-1$
	  statePanel = new JPanel(new BorderLayout(5, 5));
	  statePanel.add(inner, BorderLayout.NORTH);
	}
	return statePanel;
  }

  /**
   * This method initializes fileButton	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getFileButton() {
    if (fileButton == null) {
      fileButton = new JButton();
      fileButton.setIcon(new ImageIcon(getClass().getResource("openHS.png"))); //$NON-NLS-1$
      fileButton.setToolTipText(Localization.getString("MainFrame.Select")); //$NON-NLS-1$
      fileButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          selectFile();
        }
      });
    }
    return fileButton;
  }
  
  private void selectFile() {
    JFileChooser chooser = new JFileChooser();
    chooser.setAcceptAllFileFilterUsed(true);
    chooser.setFileFilter(new ExampleFileFilter("ares", Localization.getString("MainFrame.ConfigFiles"))); //$NON-NLS-1$ //$NON-NLS-2$
    chooser.setCurrentDirectory(Directories.getLastUsedDirectory(this, "ConfigurationFiles")); //$NON-NLS-1$
    chooser.setMultiSelectionEnabled(false);
    if (chooser.showOpenDialog(this) == JFileChooser.APPROVE_OPTION) {
      File file = chooser.getSelectedFile();
      Directories.setLastUsedDirectory(this, "ConfigurationFiles", file); //$NON-NLS-1$
   	  openFile(file.getAbsolutePath());
    }
  }

  /**
   * This method initializes jPanel1	
   * 	
   * @return javax.swing.JPanel	
   */
  private JPanel getNetworkPanel() {
    if (networkPanel == null) {
      jLabel1 = new JLabel();
      jLabel1.setText(Localization.getString("MainFrame.Server")); //$NON-NLS-1$
      networkPanel = new JPanel();
      networkPanel.setLayout(new BorderLayout());
      JPanel innerPanel = new JPanel();
      innerPanel.setLayout(new BoxLayout(innerPanel, BoxLayout.LINE_AXIS));
      innerPanel.add(Box.createRigidArea(new Dimension(5, 0)));
      innerPanel.add(jLabel1);
      innerPanel.add(Box.createRigidArea(new Dimension(5, 0)));
      innerPanel.add(getServerBox());
      innerPanel.add(Box.createRigidArea(new Dimension(5, 0)));
      innerPanel.add(getConnectButton());
      networkPanel.add(innerPanel, BorderLayout.NORTH);
      networkPanel.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED), Localization.getString("MainFrame.Network"), TitledBorder.DEFAULT_JUSTIFICATION, TitledBorder.DEFAULT_POSITION, null, null)); //$NON-NLS-1$ //$NON-NLS-2$
    }
    return networkPanel;
  }

  /**
   * This method initializes serverBox	
   * 	
   * @return javax.swing.JComboBox	
   */
  private JComboBox getServerBox() {
    if (serverBox == null) {
      serverBox = new JComboBox();
    }
    return serverBox;
  }

  /**
   * This method initializes connectButton	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getConnectButton() {
    if (connectButton == null) {
      connectButton = new JButton();
      connectButton.setEnabled(false);
      connectButton.setText(Localization.getString("MainFrame.Connect")); //$NON-NLS-1$
      connectButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          connectOrDisconnect();
        }
      });
    }
    return connectButton;
  }
  
  private void connectOrDisconnect() {
    if (Control.getInstance().isConnected()) {
      Control.getInstance().disconnect(true);
    }
    else {
      String server = getServerBox().getSelectedItem().toString();
      ServerInfo serverInfo = servers.get(server);
      if (serverInfo != null) {
        Control.getInstance().connect(serverInfo, this);
      }
    }
    updateNetworkState();
  }
  
  private void updateNetworkState() {
    if (Control.getInstance().isConnected()) {
      serverSearch.stopSearch();
      getConnectButton().setText(Localization.getString("MainFrame.Disconnect")); //$NON-NLS-1$
    }
    else {
      servers.clear();
      getServerBox().removeAllItems();
      getConnectButton().setEnabled(false);
      getConnectButton().setText(Localization.getString("MainFrame.Connect")); //$NON-NLS-1$
      serverSearch.startSearch();
    }
  }

  /**
   * This method initializes messagesBox	
   * 	
   * @return javax.swing.JCheckBox	
   */
  private JToggleButton getMessagesButton() {
    if (messagesButton == null) {
      messagesButton = new JToggleButton(new ImageIcon(getClass().getResource("LegendHS.png"))); //$NON-NLS-1$
      messagesButton.setToolTipText(Localization.getString("MainFrame.Messages")); //$NON-NLS-1$
      messagesButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          if (messageFrame != null && messageFrame.isVisible()) {
            messageFrame.dispose();
            messageFrame = null;
            messagesMenuItem.setSelected(false);
          }
          else {
            messageFrame = new MessageFrame();
            messageFrame.addWindowListener(new WindowAdapter() {
              public void windowClosing(java.awt.event.WindowEvent e) {
                MainFrame.this.getMessagesButton().setSelected(false);
                messagesMenuItem.setSelected(false);
              }
            });
            messagesMenuItem.setSelected(true);
            messageFrame.setVisible(true);
          }
        }

      });
    }
    return messagesButton;
  }
  
  private MessageFrame messageFrame = null;

  /**
   * This method initializes jButton	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getSettingsButton() {
    if (settingsButton == null) {
      settingsButton = new JButton();
      settingsButton.setIcon(new ImageIcon(getClass().getResource("Properties.png"))); //$NON-NLS-1$
      settingsButton.setToolTipText(Localization.getString("MainFrame.Settings")); //$NON-NLS-1$
      settingsButton.addActionListener(new java.awt.event.ActionListener() {
        public void actionPerformed(java.awt.event.ActionEvent e) {
          showSettingsDialog();
        }
      });
    }
    return settingsButton;
  }
  
  private JButton getInfoButton() {
	  if (infoButton == null) {
		  infoButton = new JButton();
		  infoButton.setIcon(new ImageIcon(getClass().getResource("Information.gif"))); //$NON-NLS-1$
		  infoButton.setToolTipText(Localization.getString("MainFrame.AboutMenu")); //$NON-NLS-1$
		  infoButton.addActionListener(new java.awt.event.ActionListener() {
			  public void actionPerformed(java.awt.event.ActionEvent e) {
				  showAboutDialog();
			  }
		  });
	  }
	  return infoButton;
  }

  private void showAboutDialog() {
	  AboutDialog dialog = new AboutDialog(this);
	  dialog.setVisible(true);
  }
  
  private void showHelpPage() {
	  ares.controller.control.OnlineOperations.showHomepage(this);
  }

  private JButton stopAllButton = null;

  private JButton getStopAllButton() {
	if (stopAllButton == null) {
	    stopAllButton = new JButton();
	    stopAllButton.setAction(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0)));
	    stopAllButton.setIcon(new ImageIcon(getClass().getResource("StopSmall.png"))); //$NON-NLS-1$
	    stopAllButton.setToolTipText(Localization.getString("MainControlsFrame.StopAll")); //$NON-NLS-1$
	}
	return stopAllButton;
  }
  
  private JButton nextTrackButton = null;
  private JButton previousButton = null;

  private JButton getNextTrackButton() {
	  if (nextTrackButton == null) {
		  nextTrackButton = new JButton();
		  nextTrackButton.setAction(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT, 0)));
		  nextTrackButton.setIcon(new ImageIcon(getClass().getResource("forward.png"))); //$NON-NLS-1$
		  nextTrackButton.setToolTipText(Localization.getString("MainControlsFrame.NextTrack")); //$NON-NLS-1$
	  }
	  return nextTrackButton;
  }

  /**
   * This method initializes previousButton	
   * 	
   * @return javax.swing.JButton	
   */
  private JButton getPreviousButton() {
	  if (previousButton == null) {
		  previousButton = new JButton();
		  previousButton.setAction(new KeyAction(KeyStroke.getKeyStroke(KeyEvent.VK_LEFT, 0)));
		  previousButton.setIcon(new ImageIcon(getClass().getResource("back.png"))); //$NON-NLS-1$
		  previousButton.setToolTipText(Localization.getString("MainControlsFrame.PreviousTrack")); //$NON-NLS-1$
	  }
	  return previousButton;
  }
  
  

  /* (non-Javadoc)
   * @see ares.controller.messages.IMessageListener#messageAdded(ares.controller.messages.Message)
   */
  @Override
  public void messageAdded(Message message) {
    if (message.getType().ordinal() <= Message.MessageType.Warning.ordinal()) {
      int messageLevel = Preferences.userNodeForPackage(MainFrame.class).getInt("MessageLevel", Message.MessageType.Warning.ordinal()); //$NON-NLS-1$
      if (message.getType().ordinal() <= messageLevel) {
        if (!getMessagesButton().isSelected()) {
          getMessagesButton().doClick();
        }
      }
    }
  }

  /* (non-Javadoc)
   * @see ares.controller.network.IServerListener#serverFound(ares.controller.network.ServerInfo)
   */
  @Override
  public void serverFound(ServerInfo server) {
    if (servers.containsKey(server.getName())) return;
    Messages.addMessage(MessageType.Info, Localization.getString("MainFrame.ServerFound") + server.getName() + Localization.getString("MainFrame.33")); //$NON-NLS-1$ //$NON-NLS-2$
    getServerBox().addItem(server.getName());
    getConnectButton().setEnabled(true);
    servers.put(server.getName(), server);
  }
  
  private Map<String, ServerInfo> servers = new HashMap<String, ServerInfo>();
  
  private List<String> modeElements = new ArrayList<String>();
  
  private void updateModeElements()
  {
	  String text = new String(); 
	  for (int i = 0; i < modeElements.size(); ++i) {
		  text += modeElements.get(i);
		  if (i != modeElements.size() - 1) {
			  text += ", "; //$NON-NLS-1$
		  }
	  }
	  
	String fit = compact(text, elementsLabel, getStatePanel().getWidth() - 25 - elementsDescLabel.getWidth());
	elementsLabel.setText(fit);
	if (!fit.equals(text))
	{
		elementsLabel.setToolTipText(text);
	}
	else
	{
		elementsLabel.setToolTipText(""); //$NON-NLS-1$
	}
  }

	@Override
	public void modeChanged(final String newMode) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				modeLabel.setText(newMode);
				FrameManagement.getInstance().activateFrame(newMode);
			}
		});
	}
	
	@Override
	public void modeElementStarted(final int element) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				CommandButtonMapping.getInstance().commandStateChanged(element, true);
				String title = Control.getInstance().getConfiguration().getCommandTitle(element);
				if (title != null) {
					modeElements.add(title);
					updateModeElements();
				}
			}
		});
	}
	
	@Override
	public void modeElementStopped(final int element) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				CommandButtonMapping.getInstance().commandStateChanged(element, false);
				String title = Control.getInstance().getConfiguration().getCommandTitle(element);
				if (title != null) {
					modeElements.remove(title);
					updateModeElements();
				}
			}
		});
	}
	
	@Override
	public void volumeChanged(final int index, final int value) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				listenForVolume = false;
				switch (index) {
				case 2:
					getMainVolumeSlider().setValue(value);
					break;
				case 1:
					getMusicVolumeSlider().setValue(value);
					break;
				case 0:
					getSoundVolumeSlider().setValue(value);
					break;
				default:
					break;
				}
				listenForVolume = true;
			}
		});
	}
	
	private static String compact(String text, JLabel label, int labelMaxWidth)
	{
		Graphics g = label.getGraphics();
		FontMetrics fm = g.getFontMetrics();
		int width = fm.stringWidth(text);
		
		if (width <= labelMaxWidth)
			return text;

		int len = 0;
		int seg = text.length();
		String fit = ""; //$NON-NLS-1$
		
		final String ELLIPSIS = "..."; //$NON-NLS-1$

		// find the longest string that fits into
		// the control boundaries using bisection method 
		while (seg > 1)
		{
			seg -= seg / 2;

			int left = len + seg;
			int right = text.length();

			if (left > right)
				continue;

			right -= left;
			left = 0;

			// build and measure a candidate string with ellipsis
			String tst = text.substring(0, left) + 
				ELLIPSIS + text.substring(right);
			
		    int testWidth = fm.stringWidth(tst);
				
			// candidate string fits into control boundaries, 
			// try a longer string
			// stop when seg <= 1 
			if (testWidth <= labelMaxWidth)
			{
				len += seg;
				fit = tst;
			}
		}

		if (len == 0) // string can't fit into control
		{
			return ELLIPSIS;
		}
		return fit;
	}
	
	@Override
	public void musicChanged(final String newMusic) {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				String fit = compact(newMusic, musicLabel, getStatePanel().getWidth() - 25 - elementsDescLabel.getWidth());
				musicLabel.setText(fit);
				if (!fit.equals(newMusic))
				{
					musicLabel.setToolTipText(newMusic);
				}
				else
				{
					musicLabel.setToolTipText(""); //$NON-NLS-1$
				}
			}
		});
	}
	
	@Override
	public void disconnect() {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				Control.getInstance().disconnect(false);
				updateNetworkState();
			}
		});
	}
  
	
	public void connectionFailed() {
		SwingUtilities.invokeLater(new Runnable() {
			public void run() {
				Control.getInstance().disconnect(false);
				updateNetworkState();
			}
		});		
	}

	private static class SelectModeAction extends AbstractAction {

		private String title;

		public SelectModeAction(Command command) {
			this.title = command.getTitle();
		}

		public void actionPerformed(ActionEvent arg0) {
			if (ModeFrames.getInstance().isFrameOpen(title)) {
				ModeFrames.getInstance().closeFrame(title);
			}
			else {
				ModeFrames.getInstance().openFrame(title);
			}
		}
	}

	public AbstractAction createAction(Command command, AbstractButton button) {
		addButton(command.getTitle(), button);
		button.setSelected(ModeFrames.getInstance().isFrameOpen(command.getTitle()));
		return new SelectModeAction(command);
	}

	private void showSettingsDialog() {
		int oldPort = Preferences.userNodeForPackage(MainFrame.class).getInt("UDPPort", 8009);  //$NON-NLS-1$
		OptionsDialog dialog = new OptionsDialog(MainFrame.this);
		dialog.setLocationRelativeTo(this);
		dialog.setModal(true);
		dialog.setVisible(true);
		int newPort = Preferences.userNodeForPackage(MainFrame.class).getInt("UDPPort", 8009);  //$NON-NLS-1$
		if (serverSearch != null && oldPort != newPort) {
			boolean wasSearching = serverSearch.isSearching();
			if (wasSearching) {
				serverSearch.stopSearch();
			}
			serverSearch.dispose();
			serverSearch = new ServerSearch(MainFrame.this, newPort);
			if (wasSearching) {
				serverSearch.startSearch();
			}
		}
	}
	
	private void buildLastProjectsMenu() {
		java.util.prefs.Preferences prefs = java.util.prefs.Preferences
		.userNodeForPackage(getClass());
		int nrOfLastProjects = prefs.getInt("LastUsedProjectsCount", 0); //$NON-NLS-1$
		class LastProjectsActionListener implements java.awt.event.ActionListener {
			public LastProjectsActionListener(String fileName) {
				this.fileName = fileName;
			}

			public void actionPerformed(java.awt.event.ActionEvent e) {
				openFile(fileName);
			}

			private final String fileName;
		}
		for (int i = 0; i < nrOfLastProjects; ++i) {
			JMenuItem lastProjectItem = new JMenuItem();
			lastProjectItem.setText((i + 1) + " " //$NON-NLS-1$
					+ prefs.get("LastUsedProjectName" + i, "Project " + (i + 1))); //$NON-NLS-1$ //$NON-NLS-2$
			lastProjectItem.setMnemonic(java.awt.event.KeyEvent.VK_1 + i);
			lastProjectItem.addActionListener(new LastProjectsActionListener(prefs.get(
					"LastUsedProjectFile" + i, ""))); //$NON-NLS-1$ //$NON-NLS-2$
			recentMenu.add(lastProjectItem);
		}
	}

	private void rebuildLastProjectsMenu() {
		recentMenu.removeAll();
		buildLastProjectsMenu();
	}


}  //  @jve:decl-index=0:visual-constraint="10,10"
