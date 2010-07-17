/*
 Copyright (c) 2006-2009 [Joerg Ruedenauer]
 
 This file is part of DSA4-Heldenverwaltung.

 DSA4-Heldenverwaltung is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 DSA4-Heldenverwaltung is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with DSA4-Heldenverwaltung; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
package de.javasoft.plaf.synthetica;

import de.javasoft.plaf.synthetica.painter.MenuPainter;
import de.javasoft.plaf.synthetica.painter.TabbedPanePainter;
import de.javasoft.plaf.synthetica.painter.TreePainter;
import de.javasoft.util.IVersion;
import java.awt.*;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import java.text.ParseException;
import java.util.*;
import javax.swing.*;
import javax.swing.plaf.BorderUIResource;
import javax.swing.plaf.ColorUIResource;
import javax.swing.plaf.synth.*;
import sun.swing.DefaultLookup;
import sun.swing.plaf.synth.DefaultSynthStyle;

public class SyntheticaWalnutLookAndFeel extends SynthLookAndFeel {

  private static class Version implements IVersion {

    public int getMajor() {
      return major;
    }

    public int getMinor() {
      return minor;
    }

    public int getRevision() {
      return revision;
    }

    public int getBuild() {
      return build;
    }

    public String toString() {
      return (new StringBuilder(String.valueOf(major))).append(".").append(
          minor).append(".").append(revision).append(" Build ").append(build)
          .toString();
    }

    private int major;

    private int minor;

    private int revision;

    private int build;

    public Version() {
      super();
      ResourceBundle rb = getResourceBundle("SyntheticaStandardLookAndFeelVersion");
      major = Integer.parseInt(rb.getString("major"));
      minor = Integer.parseInt(rb.getString("minor"));
      revision = Integer.parseInt(rb.getString("revision"));
      build = Integer.parseInt(rb.getString("build"));
    }
  }

  public SyntheticaWalnutLookAndFeel() throws ParseException {
    String fileName = "walnut/synth.xml";
    Class<?> clazz = de.javasoft.plaf.synthetica.SyntheticaWalnutLookAndFeel.class;
    load(clazz.getResourceAsStream(fileName), clazz);
    try {
      String syntheticaFileName = "Synthetica.xml";
      load(clazz.getResourceAsStream((new StringBuilder("/")).append(
          syntheticaFileName).toString()), clazz);
    }
    catch (IllegalArgumentException illegalargumentexception) {
    }
    catch (Exception e) {
      e.printStackTrace();
    }
    String className = getClass().getName();
    try {
      String syntheticaFileName = (new StringBuilder(String.valueOf(className
          .substring(className.lastIndexOf(".") + 1)))).append(".xml")
          .toString();
      load(clazz.getResourceAsStream((new StringBuilder("/")).append(
          syntheticaFileName).toString()), clazz);
    }
    catch (IllegalArgumentException illegalargumentexception1) {
    }
    catch (Exception e) {
      e.printStackTrace();
    }
  }

  public String getDescription() {
    return "Synthetica - the extended Synth Look and Feel.";
  }

  private static class Version2 implements IVersion {

    public Version2(String id) {
      rb = getResourceBundle((new StringBuilder(String.valueOf(id))).append(
          "Version").toString());
      major = Integer.parseInt(rb.getString("major"));
      minor = Integer.parseInt(rb.getString("minor"));
      revision = Integer.parseInt(rb.getString("revision"));
      build = Integer.parseInt(rb.getString("build"));
    }

    ResourceBundle rb;

    final int major;

    final int minor;

    final int revision;

    final int build;

    public int getMajor() {
      return major;
    }

    public int getMinor() {
      return minor;
    }

    public int getRevision() {
      return revision;
    }

    public int getBuild() {
      return build;
    }

    public String toString() {
      return (new StringBuilder(String.valueOf(major))).append(".").append(
          minor).append(".").append(revision).append(" Build ").append(build)
          .toString();
    }

  };

  public IVersion getVersion() {
    return new Version2(getID());
  }

  public boolean getSupportsWindowDecorations() {
    return true;
  }

  public UIDefaults getDefaults() {
    UIDefaults defaults = super.getDefaults();
    return defaults;
  }

  private void addResourceBundleToDefaults(String name, UIDefaults defaults) {
    ResourceBundle resBundle = getResourceBundle(name);
    String key;
    String value;
    for (Enumeration<String> enumeration = resBundle.getKeys(); enumeration
        .hasMoreElements(); defaults.put(key, value)) {
      key = enumeration.nextElement();
      value = resBundle.getString(key);
    }

  }

  public void initialize() {
    super.initialize();
    orgDefaults = (UIDefaults) UIManager.getDefaults().clone();
    DefaultLookup.setDefaultLookup(new SyntheticaDefaultLookup());
    StyleFactory styleFactory = new FastTableStyleFactory(getStyleFactory());
    SynthLookAndFeel.setStyleFactory(styleFactory);
    PopupFactory.install();
    lafChangeListener = new PropertyChangeListener() {

      public void propertyChange(PropertyChangeEvent evt) {
        reinit();
        installSyntheticaDefaults();
        if (SyntheticaWalnutLookAndFeel.defaultsCompatibilityMode)
          installCompatibilityDefaults();
      }

    };
    PropertyChangeListener listeners[] = UIManager.getPropertyChangeListeners();
    PropertyChangeListener apropertychangelistener[] = listeners;
    int i = 0;
    for (int j = apropertychangelistener.length; i < j; i++) {
      PropertyChangeListener l = apropertychangelistener[i];
      UIManager.removePropertyChangeListener(l);
    }

    UIManager.addPropertyChangeListener(lafChangeListener);
    apropertychangelistener = listeners;
    i = 0;
    for (int k = apropertychangelistener.length; i < k; i++) {
      PropertyChangeListener l = apropertychangelistener[i];
      UIManager.addPropertyChangeListener(l);
    }

  }

  private void reinit() {
    ((MenuPainter) MenuPainter.getInstance()).reinitialize();
    ((TreePainter) TreePainter.getInstance()).reinitialize();
    ((TabbedPanePainter) TabbedPanePainter.getInstance()).reinitialize();
  }

  private void installSyntheticaDefaults() {
    UIDefaults defaults = UIManager.getDefaults();
    defaults.put("HyperlinkUI", "de.javasoft.plaf.synthetica.HyperlinkUI");
    defaults.put("StatusBarUI", "de.javasoft.plaf.synthetica.StatusBarUI");
    defaults.put("LoginPanelUI", "de.javasoft.plaf.synthetica.LoginPanelUI");
    defaults.put("MonthViewUI", "de.javasoft.plaf.synthetica.MonthViewUI");
    defaults.put("swingx/TaskPaneUI", "de.javasoft.plaf.synthetica.TaskPaneUI");
    defaults.put("swingx/TaskPaneContainerUI",
        "de.javasoft.plaf.synthetica.TaskPaneContainerUI");
    defaults.put("swingx/TipOfTheDayUI",
        "de.javasoft.plaf.synthetica.TipOfTheDayUI");
    defaults
        .put("Flexdock.view", "de.javasoft.plaf.synthetica.flexdock.ViewUI");
    defaults.put("Flexdock.titlebar",
        "de.javasoft.plaf.synthetica.flexdock.TitlebarUI");
    defaults.put("Flexdock.titlebar.button",
        "de.javasoft.plaf.synthetica.flexdock.ButtonUI");
    addResourceBundleToDefaults("synthetica", defaults);
    if (UIManager.getBoolean("Synthetica.window.decoration"))
      defaults.put("RootPaneUI",
          "de.javasoft.plaf.synthetica.HelpButtonRootPaneUI");
    else
      decorated = false;
    JFrame.setDefaultLookAndFeelDecorated(decorated);
    JDialog.setDefaultLookAndFeelDecorated(decorated);
    extendedFileChooserEnabled = UIManager
        .getBoolean("Synthetica.extendedFileChooser.enabled");
    setExtendedFileChooserEnabled(extendedFileChooserEnabled);
    rememberFileChooserPreferences = UIManager
        .getBoolean("Synthetica.extendedFileChooser.rememberPreferences");
    useSystemFileIcons = UIManager
        .getBoolean("Synthetica.extendedFileChooser.useSystemFileIcons");
    UIDefaults lafDefaults = UIManager.getLookAndFeelDefaults();
    for (Iterator<java.util.Map.Entry<Object, Object>> iterator = lafDefaults.entrySet().iterator(); iterator
        .hasNext();) {
      java.util.Map.Entry<Object, Object> entry = iterator.next();
      if (!defaults.containsKey(entry.getKey()))
        defaults.put(entry.getKey(), entry.getValue());
    }

  }

  private void installCompatibilityDefaults() {
    UIDefaults defaults = UIManager.getDefaults();
    initSystemColorDefaults(UIManager.getDefaults());
    Object uiDefaults[] = {
        "FileChooser.ancestorInputMap",
        new javax.swing.UIDefaults.LazyInputMap(new Object[] { "ESCAPE",
            "cancelSelection", "F5", "refresh", "ENTER", "approveSelection" }),
        "List.selectionForeground",
        new ColorUIResource(Color.white),
        "SplitPane.dividerSize",
        Integer.valueOf(8),
        "List.focusCellHighlightBorder",
        new javax.swing.plaf.BorderUIResource.LineBorderUIResource(defaults
            .getColor("Synthetica.list.focusCellHighlightBorder.color")),
        "Table.focusCellHighlightBorder",
        new javax.swing.plaf.BorderUIResource.LineBorderUIResource(defaults
            .getColor("Synthetica.table.focusCellHighlightBorder.color")),
        "Table.scrollPaneBorder",
        new javax.swing.plaf.BorderUIResource.LineBorderUIResource(defaults
            .getColor("Synthetica.table.scrollPane.border.color")),
        "TitledBorder.border",
        new BorderUIResource(new SyntheticaTitledBorder()),
        "RootPane.defaultButtonWindowKeyBindings",
        new Object[] { "ENTER", "press", "released ENTER", "release",
            "ctrl ENTER", "press", "ctrl released ENTER", "release" },
        "Table.ancestorInputMap",
        new javax.swing.UIDefaults.LazyInputMap(new Object[] { "ctrl C",
            "copy", "ctrl V", "paste", "ctrl X", "cut", "COPY", "copy",
            "PASTE", "paste", "CUT", "cut", "RIGHT", "selectNextColumn",
            "KP_RIGHT", "selectNextColumn", "shift RIGHT",
            "selectNextColumnExtendSelection", "shift KP_RIGHT",
            "selectNextColumnExtendSelection", "ctrl shift RIGHT",
            "selectNextColumnExtendSelection", "ctrl shift KP_RIGHT",
            "selectNextColumnExtendSelection", "ctrl RIGHT",
            "selectNextColumnChangeLead", "ctrl KP_RIGHT",
            "selectNextColumnChangeLead", "LEFT", "selectPreviousColumn",
            "KP_LEFT", "selectPreviousColumn", "shift LEFT",
            "selectPreviousColumnExtendSelection", "shift KP_LEFT",
            "selectPreviousColumnExtendSelection", "ctrl shift LEFT",
            "selectPreviousColumnExtendSelection", "ctrl shift KP_LEFT",
            "selectPreviousColumnExtendSelection", "ctrl LEFT",
            "selectPreviousColumnChangeLead", "ctrl KP_LEFT",
            "selectPreviousColumnChangeLead", "DOWN", "selectNextRow",
            "KP_DOWN", "selectNextRow", "shift DOWN",
            "selectNextRowExtendSelection", "shift KP_DOWN",
            "selectNextRowExtendSelection", "ctrl shift DOWN",
            "selectNextRowExtendSelection", "ctrl shift KP_DOWN",
            "selectNextRowExtendSelection", "ctrl DOWN",
            "selectNextRowChangeLead", "ctrl KP_DOWN",
            "selectNextRowChangeLead", "UP", "selectPreviousRow", "KP_UP",
            "selectPreviousRow", "shift UP",
            "selectPreviousRowExtendSelection", "shift KP_UP",
            "selectPreviousRowExtendSelection", "ctrl shift UP",
            "selectPreviousRowExtendSelection", "ctrl shift KP_UP",
            "selectPreviousRowExtendSelection", "ctrl UP",
            "selectPreviousRowChangeLead", "ctrl KP_UP",
            "selectPreviousRowChangeLead", "HOME", "selectFirstColumn",
            "shift HOME", "selectFirstColumnExtendSelection",
            "ctrl shift HOME", "selectFirstRowExtendSelection", "ctrl HOME",
            "selectFirstRow", "END", "selectLastColumn", "shift END",
            "selectLastColumnExtendSelection", "ctrl shift END",
            "selectLastRowExtendSelection", "ctrl END", "selectLastRow",
            "PAGE_UP", "scrollUpChangeSelection", "shift PAGE_UP",
            "scrollUpExtendSelection", "ctrl shift PAGE_UP",
            "scrollLeftExtendSelection", "ctrl PAGE_UP",
            "scrollLeftChangeSelection", "PAGE_DOWN",
            "scrollDownChangeSelection", "shift PAGE_DOWN",
            "scrollDownExtendSelection", "ctrl shift PAGE_DOWN",
            "scrollRightExtendSelection", "ctrl PAGE_DOWN",
            "scrollRightChangeSelection", "TAB", "selectNextColumnCell",
            "shift TAB", "selectPreviousColumnCell", "ENTER",
            "selectNextRowCell", "shift ENTER", "selectPreviousRowCell",
            "ctrl A", "selectAll", "ctrl SLASH", "selectAll",
            "ctrl BACK_SLASH", "clearSelection", "ESCAPE", "cancel", "F2",
            "startEditing", "SPACE", "addToSelection", "ctrl SPACE",
            "toggleAndAnchor", "shift SPACE", "extendTo", "ctrl shift SPACE",
            "moveSelectionTo" }),
        "Table.ancestorInputMap.RightToLeft",
        new javax.swing.UIDefaults.LazyInputMap(new Object[] { "RIGHT",
            "selectPreviousColumn", "KP_RIGHT", "selectPreviousColumn",
            "shift RIGHT", "selectPreviousColumnExtendSelection",
            "shift KP_RIGHT", "selectPreviousColumnExtendSelection",
            "ctrl shift RIGHT", "selectPreviousColumnExtendSelection",
            "ctrl shift KP_RIGHT", "selectPreviousColumnExtendSelection",
            "shift RIGHT", "selectPreviousColumnChangeLead", "shift KP_RIGHT",
            "selectPreviousColumnChangeLead", "LEFT", "selectNextColumn",
            "KP_LEFT", "selectNextColumn", "shift LEFT",
            "selectNextColumnExtendSelection", "shift KP_LEFT",
            "selectNextColumnExtendSelection", "ctrl shift LEFT",
            "selectNextColumnExtendSelection", "ctrl shift KP_LEFT",
            "selectNextColumnExtendSelection", "ctrl LEFT",
            "selectNextColumnChangeLead", "ctrl KP_LEFT",
            "selectNextColumnChangeLead", "ctrl PAGE_UP",
            "scrollRightChangeSelection", "ctrl PAGE_DOWN",
            "scrollLeftChangeSelection", "ctrl shift PAGE_UP",
            "scrollRightExtendSelection", "ctrl shift PAGE_DOWN",
            "scrollLeftExtendSelection" }), "controlLtHighlight",
        new ColorUIResource(Color.WHITE), "controlHighlight",
        new ColorUIResource(Color.LIGHT_GRAY), "controlShadow",
        new ColorUIResource(Color.DARK_GRAY), "controlDkShadow",
        new ColorUIResource(Color.BLACK) };
    defaults.putDefaults(uiDefaults);
    SynthStyle ss = null;
    SynthContext sc = null;
    SynthStyleFactory ssf = getStyleFactory();
    String inKeys[] = (String[]) null;
    String keys[] = (String[]) null;
    java.awt.Font font = null;
    JButton button = new JButton();
    ss = ssf.getStyle(button, Region.BUTTON);
    sc = new SynthContext(button, Region.BUTTON, ss, 1);
    font = ss.getFont(sc);
    defaults.put("Button.font", font);
    JComboBox cb = new JComboBox();
    ss = ssf.getStyle(cb, Region.COMBO_BOX);
    sc = new SynthContext(cb, Region.COMBO_BOX, ss, 1024);
    Color cbBackground = ss.getColor(sc, ColorType.BACKGROUND);
    defaults.put("ComboBox.background", cbBackground);
    Color cbForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("ComboBox.foreground", cbForeground);
    defaults.put("ComboBox.font", ss.getFont(sc));
    JLabel label = new JLabel();
    ss = ssf.getStyle(label, Region.LABEL);
    sc = new SynthContext(label, Region.LABEL, ss, 1024);
    font = ss.getFont(sc);
    defaults.put("Label.font", font);
    defaults.put("JXMonthView.font", font);
    Color labelForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("Label.foreground", labelForeground);
    JPanel panel = new JPanel();
    ss = ssf.getStyle(panel, Region.PANEL);
    sc = new SynthContext(panel, Region.PANEL, ss, 1024);
    Color panelBackground = ss.getColor(sc, ColorType.BACKGROUND);
    defaults.put("Panel.background", panelBackground);
    defaults.put("SplitPane.background", panelBackground);
    defaults.put("Label.background", panelBackground);
    defaults.put("ColorChooser.swatchesDefaultRecentColor", panelBackground);
    defaults.put("control", panelBackground);
    Color panelForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("Panel.foreground", panelForeground);
    font = ss.getFont(sc);
    defaults.put("Panel.font", font);
    defaults.put("TitledBorder.font", font);
    JList list = new JList();
    ss = ssf.getStyle(list, Region.LIST);
    sc = new SynthContext(list, Region.LIST, ss, 1024);
    Color listBackground = ss.getColor(sc, ColorType.TEXT_BACKGROUND);
    defaults.put("List.background", listBackground);
    Color listForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("List.foreground", listForeground);
    sc = new SynthContext(list, Region.LIST, ss, 512);
    listBackground = ss.getColor(sc, ColorType.TEXT_BACKGROUND);
    defaults.put("List.selectionBackground", listBackground);
    listForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("List.selectionForeground", listForeground);
    JTable table = new JTable();
    ss = ssf.getStyle(table, Region.TABLE_HEADER);
    sc = new SynthContext(table, Region.TABLE_HEADER, ss, 1024);
    Color tableHeaderBackground = ss.getColor(sc, ColorType.BACKGROUND);
    defaults.put("TableHeader.background", tableHeaderBackground);
    Color tableHeaderForeground = ss.getColor(sc, ColorType.FOREGROUND);
    defaults.put("TableHeader.foreground", tableHeaderForeground);
    ss = ssf.getStyle(table, Region.TABLE);
    sc = new SynthContext(table, Region.TABLE, ss, 1024);
    defaults.put("Table.gridColor", ss.get(sc, "Table.gridColor"));
    Color tableBackground = ss.getColor(sc, ColorType.BACKGROUND);
    defaults.put("Table.background", tableBackground);
    Color tableForeground = ss.getColor(sc, ColorType.FOREGROUND);
    defaults.put("Table.foreground", tableForeground);
    sc = new SynthContext(table, Region.TABLE, ss, 512);
    tableBackground = ss.getColor(sc, ColorType.TEXT_BACKGROUND);
    defaults.put("Table.selectionBackground", tableBackground);
    tableForeground = ss.getColor(sc, ColorType.TEXT_FOREGROUND);
    defaults.put("Table.selectionForeground", tableForeground);
    JTree tree = new JTree();
    ss = ssf.getStyle(tree, Region.TREE);
    font = ((DefaultSynthStyle) ss).getFont(tree, Region.TREE, 1);
    defaults.put("Tree.font", font);
    sc = new SynthContext(tree, Region.TREE, ss, 1024);
    keys = (new String[] { "Tree.expandedIcon", "Tree.collapsedIcon" });
    putIcons2Defaults(defaults, keys, keys, ss, sc);
    defaults.put("Tree.rowHeight", ss.get(sc, "Tree.rowHeight"));
    defaults.put("Tree.leftChildIndent", ss.get(sc, "Tree.leftChildIndent"));
    defaults.put("Tree.rightChildIndent", ss.get(sc, "Tree.rightChildIndent"));
    ss = ssf.getStyle(tree, Region.TREE_CELL);
    sc = new SynthContext(tree, Region.TREE_CELL, ss, 1024);
    defaults.put("Tree.textForeground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("Tree.textBackground", ss.getColor(sc,
        ColorType.TEXT_BACKGROUND));
    sc = new SynthContext(tree, Region.TREE_CELL, ss, 512);
    defaults.put("Tree.selectionForeground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("Tree.selectionBackground", ss.getColor(sc,
        ColorType.TEXT_BACKGROUND));
    defaults.put("Tree.hash", defaults
        .get("Synthetica.tree.line.color.vertical"));
    JInternalFrame iFrame = new JInternalFrame();
    ss = ssf.getStyle(iFrame, Region.INTERNAL_FRAME_TITLE_PANE);
    Color iFrameForeground = ((DefaultSynthStyle) ss).getColor(iFrame,
        Region.INTERNAL_FRAME_TITLE_PANE, 512, ColorType.FOREGROUND);
    defaults.put("InternalFrame.activeTitleForeground", iFrameForeground);
    iFrameForeground = ((DefaultSynthStyle) ss).getColor(iFrame,
        Region.INTERNAL_FRAME_TITLE_PANE, 1024, ColorType.FOREGROUND);
    defaults.put("InternalFrame.inactiveTitleForeground", iFrameForeground);
    Color iFrameBackground = ((DefaultSynthStyle) ss).getColor(iFrame,
        Region.INTERNAL_FRAME_TITLE_PANE, 512, ColorType.BACKGROUND);
    defaults.put("InternalFrame.activeTitleBackground", iFrameBackground);
    defaults.put("activeCaption", iFrameBackground);
    iFrameBackground = ((DefaultSynthStyle) ss).getColor(iFrame,
        Region.INTERNAL_FRAME_TITLE_PANE, 1024, ColorType.BACKGROUND);
    defaults.put("InternalFrame.inactiveTitleBackground", iFrameBackground);
    defaults.put("inactiveCaption", iFrameBackground);
    sc = new SynthContext(iFrame, Region.INTERNAL_FRAME_TITLE_PANE, ss, 1024);
    inKeys = (new String[] { "InternalFrameTitlePane.closeIcon",
        "InternalFrameTitlePane.maximizeIcon",
        "InternalFrameTitlePane.minimizeIcon",
        "InternalFrameTitlePane.iconifyIcon" });
    keys = (new String[] { "InternalFrame.closeIcon",
        "InternalFrame.maximizeIcon", "InternalFrame.minimizeIcon",
        "InternalFrame.iconifyIcon" });
    putIcons2Defaults(defaults, inKeys, keys, ss, sc);
    ss = ssf.getStyle(iFrame, Region.INTERNAL_FRAME);
    sc = new SynthContext(iFrame, Region.INTERNAL_FRAME, ss, 1024);
    keys = (new String[] { "InternalFrame.icon" });
    putIcons2Defaults(defaults, keys, keys, ss, sc);
    JMenu menu = new JMenu();
    ss = ssf.getStyle(menu, Region.MENU);
    sc = new SynthContext(menu, Region.MENU, ss, 1024);
    defaults.put("MenuItem.background", ss.getColor(sc, ColorType.BACKGROUND));
    JOptionPane oPane = new JOptionPane();
    ss = ssf.getStyle(oPane, Region.OPTION_PANE);
    sc = new SynthContext(oPane, Region.OPTION_PANE, ss, 1024);
    keys = (new String[] { "OptionPane.informationIcon",
        "OptionPane.questionIcon", "OptionPane.warningIcon",
        "OptionPane.errorIcon" });
    putIcons2Defaults(defaults, keys, keys, ss, sc);
    JCheckBox checkBox = new JCheckBox();
    ss = ssf.getStyle(checkBox, Region.CHECK_BOX);
    sc = new SynthContext(checkBox, Region.CHECK_BOX, ss, 1024);
    keys = (new String[] { "CheckBox.icon" });
    putIcons2Defaults(defaults, keys, keys, ss, sc);
    JRadioButton radioButton = new JRadioButton();
    ss = ssf.getStyle(radioButton, Region.RADIO_BUTTON);
    sc = new SynthContext(radioButton, Region.RADIO_BUTTON, ss, 1024);
    keys = (new String[] { "RadioButton.icon" });
    putIcons2Defaults(defaults, keys, keys, ss, sc);
    JTabbedPane tPane = new JTabbedPane();
    ss = ssf.getStyle(tPane, Region.TABBED_PANE_TAB_AREA);
    sc = new SynthContext(tPane, Region.TABBED_PANE_TAB_AREA, ss, 1024);
    defaults.put("TabbedPane.tabAreaInsets", ss.getInsets(sc, null));
    ss = ssf.getStyle(tPane, Region.TABBED_PANE_TAB);
    sc = new SynthContext(tPane, Region.TABBED_PANE_TAB, ss, 1024);
    defaults.put("TabbedPane.tabInsets", ss.getInsets(sc, null));
    ss = ssf.getStyle(tPane, Region.TABBED_PANE_TAB);
    sc = new SynthContext(tPane, Region.TABBED_PANE_TAB, ss, 512);
    defaults.put("TabbedPane.selectedTabPadInsets", ss.getInsets(sc, null));
    ss = ssf.getStyle(tPane, Region.TABBED_PANE_CONTENT);
    sc = new SynthContext(tPane, Region.TABBED_PANE_CONTENT, ss, 1024);
    defaults.put("TabbedPane.contentBorderInsets", ss.getInsets(sc, null));
    defaults.put("TabbedPane.shadow", Color.GRAY);
    JTextField tf = new JTextField();
    defaults.put("TextField.border", tf.getBorder());
    ss = ssf.getStyle(tf, Region.TEXT_FIELD);
    sc = new SynthContext(tf, Region.TEXT_FIELD, ss, 1024);
    defaults.put("TextField.foreground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("TextField.background", ss.getColor(sc,
        ColorType.TEXT_BACKGROUND));
    sc = new SynthContext(tf, Region.TEXT_FIELD, ss, 8);
    defaults.put("TextField.inactiveForeground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("TextField.inactiveBackground", ss.getColor(sc,
        ColorType.TEXT_BACKGROUND));
    sc = new SynthContext(tf, Region.TEXT_FIELD, ss, 512);
    defaults.put("TextField.selectionForeground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("TextField.selectionBackground", ss.getColor(sc,
        ColorType.TEXT_BACKGROUND));
    defaults.put("textHighlight", ss.getColor(sc, ColorType.TEXT_BACKGROUND));
    JToolTip tt = new JToolTip();
    sc = new SynthContext(tt, Region.TOOL_TIP, ss, 1024);
    ss = ssf.getStyle(tt, Region.TOOL_TIP);
    defaults.put("ToolTip.font", ss.getFont(sc));
    defaults.put("ToolTip.foreground", ss.getColor(sc,
        ColorType.TEXT_FOREGROUND));
    defaults.put("ToolTip.background", ss.getColor(sc, ColorType.BACKGROUND));
    defaults.put("ColumnHeaderRenderer.upIcon", makeIcon(getClass(), defaults
        .getString("Synthetica.arrow.up")));
    defaults.put("ColumnHeaderRenderer.downIcon", makeIcon(getClass(), defaults
        .getString("Synthetica.arrow.down")));
  }

  private void putIcons2Defaults(UIDefaults defaults, String inKeys[],
      String keys[], SynthStyle ss, SynthContext sc) {
    for (int i = 0; i < inKeys.length; i++) {
      javax.swing.Icon icon = ss.getIcon(sc, inKeys[i]);
      defaults.put(keys[i], icon);
    }

  }

  public void uninitialize() {
    ((StyleFactory) SynthLookAndFeel.getStyleFactory()).uninitialize();
    UIDefaults defaults = UIManager.getDefaults();
    defaults.clear();
    java.util.Map.Entry<Object, Object> es;
    for (Iterator<java.util.Map.Entry<Object, Object>> iterator = orgDefaults.entrySet().iterator(); iterator
        .hasNext(); defaults.put(es.getKey(), es.getValue()))
      es = iterator.next();

    UIManager.removePropertyChangeListener(lafChangeListener);
    super.uninitialize();
  }

  public static void setFont(String name, int size) {
    fontName = name;
    fontSize = size;
  }

  public static String getFontName() {
    return fontName;
  }

  public static int getFontSize() {
    return fontSize;
  }

  public static boolean getAntiAliasEnabled() {
    return antiAliasEnabled;
  }

  public static void setAntiAliasEnabled(boolean value) {
    antiAliasEnabled = value;
  }

  public static void setWindowsDecorated(boolean decorated) {
    SyntheticaWalnutLookAndFeel.decorated = decorated;
  }

  public static boolean getExtendedFileChooserEnabled() {
    return extendedFileChooserEnabled;
  }

  public static void setExtendedFileChooserEnabled(boolean value) {
    extendedFileChooserEnabled = value;
    if (extendedFileChooserEnabled)
      UIManager.getDefaults().put("FileChooserUI",
          "de.javasoft.plaf.synthetica.filechooser.SyntheticaFileChooserUI");
    else
      UIManager.getDefaults().put("FileChooserUI",
          "javax.swing.plaf.metal.MetalFileChooserUI");
  }

  public static boolean getRememberFileChooserPreferences() {
    return rememberFileChooserPreferences;
  }

  public static void setRememberFileChooserPreferences(boolean value) {
    rememberFileChooserPreferences = value;
  }

  public static boolean getUseSystemFileIcons() {
    return useSystemFileIcons;
  }

  public static void setUseSystemFileIcons(boolean value) {
    useSystemFileIcons = value;
  }

  public static void setDefaultsCompatibilityMode(boolean value) {
    defaultsCompatibilityMode = value;
  }

  public static boolean getDefaultsCompatibilityMode() {
    return defaultsCompatibilityMode;
  }

  public static void setToolbarSeparatorDimension(Dimension dim) {
    toolbarSeparatorDimension = dim;
  }

  public static Dimension getToolbarSeparatorDimension() {
    return toolbarSeparatorDimension;
  }

  private static ResourceBundle getResourceBundle(String name) {
    return ResourceBundle.getBundle((new StringBuilder(
        "de/javasoft/plaf/synthetica/resourceBundles/")).append(name)
        .toString());
  }

  public IVersion getSyntheticaVersion() {
    return new Version();
  }

  public static Object get(String propertyKey, String propertyName,
      String componentName, boolean fallback) {
    String pKey = propertyKey;
    char delimiter = '.';
    for (int i = propertyKey.length(); i > -1;) {
      StringBuilder key = new StringBuilder("Synthetica.");
      propertyKey = propertyKey.substring(0, i);
      key.append(propertyKey);
      if (propertyName != null) {
        key.append(delimiter);
        key.append(propertyName);
      }
      if (componentName != null) {
        key.append(delimiter);
        key.append(componentName);
      }
      if (UIManager.get(key.toString()) != null || !fallback)
        return UIManager.get(key.toString());
      i = propertyKey.lastIndexOf(delimiter);
      if (i == -1 && componentName != null) {
        componentName = null;
        propertyKey = pKey;
        i = propertyKey.length();
      }
    }

    return null;
  }

  public static String getString(String propertyKey, String propertyName,
      String componentName, boolean fallback) {
    return (String) get(propertyKey, propertyName, componentName, fallback);
  }

  public static Insets getInsets(String propertyKey, String propertyName,
      String componentName, boolean fallback) {
    return (Insets) get(propertyKey, propertyName, componentName, fallback);
  }

  public static int getInt(String propertyKey, String propertyName,
      String componentName, boolean fallback, int defaultValue) {
    Object o = get(propertyKey, propertyName, componentName, fallback);
    return o == null ? defaultValue : ((Integer) o).intValue();
  }

  public static Object get(String key, Component c) {
    String name = c.getName();
    if (name == null) return UIManager.get(key);
    Object o = UIManager.get((new StringBuilder(String.valueOf(key))).append(
        ".").append(name).toString());
    if (o != null)
      return o;
    else
      return UIManager.get(key);
  }

  public static boolean getBoolean(String key, Component c) {
    Object o = get(key, c);
    return o == null ? false : ((Boolean) o).booleanValue();
  }

  public static int getInt(String key, Component c) {
    return getInt(key, c, 0);
  }

  public static int getInt(String key, Component c, int defaultValue) {
    Object o = get(key, c);
    return o == null ? defaultValue : ((Integer) o).intValue();
  }

  public static Insets getInsets(String key, Component c) {
    return (Insets) get(key, c);
  }

  public static String getString(String key, Component c) {
    return (String) get(key, c);
  }

  public static Color getColor(String key, Component c) {
    return (Color) get(key, c);
  }

  public static boolean isOpaque(JComponent c) {
    boolean opaque = c.getClientProperty("Synthetica.opaque") != null ? ((Boolean) c
        .getClientProperty("Synthetica.opaque")).booleanValue()
        : true;
    if (getBoolean("Synthetica.textComponents.useSwingOpaqueness", c))
      opaque = c.isOpaque();
    return opaque;
  }

  public static void setChildrenOpaque(Container container, boolean opaque) {
    Component acomponent[] = container.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if (c instanceof JComponent) {
        JComponent jc = (JComponent) c;
        jc.setOpaque(opaque);
        jc.putClientProperty("Synthetica.opaque", Boolean.valueOf(opaque));
        setChildrenOpaque(((Container) (jc)), opaque);
      }
    }

  }

  public static void setChildrenName(Container container, String oldName,
      String newName) {
    Component acomponent[] = container.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if (oldName.equals(c.getName())) c.setName(newName);
      if (c instanceof Container)
        setChildrenName((Container) c, oldName, newName);
    }

  }

  public static Component findComponent(String name, Container container) {
    Component acomponent[] = container.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if (name.equals(c.getName())) return c;
      if (c instanceof Container) {
        Component cc = findComponent(name, (Container) c);
        if (cc != null) return cc;
      }
    }

    return null;
  }

  public static boolean popupHasIcons(JPopupMenu popup) {
    Component acomponent[] = popup.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if ((c instanceof JMenuItem) && ((JMenuItem) c).getIcon() != null)
        return true;
    }

    return false;
  }

  public static boolean popupHasCheckRadio(JPopupMenu popup) {
    Component acomponent[] = popup.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if ((c instanceof JCheckBoxMenuItem)
          || (c instanceof JRadioButtonMenuItem)) return true;
    }

    return false;
  }

  public static boolean popupHasCheckRadioWithIcon(JPopupMenu popup) {
    Component acomponent[] = popup.getComponents();
    int i = 0;
    for (int j = acomponent.length; i < j; i++) {
      Component c = acomponent[i];
      if ((c instanceof JCheckBoxMenuItem)
          || (c instanceof JRadioButtonMenuItem)) {
        JMenuItem mi = (JMenuItem) c;
        if (mi.getIcon() != null) return true;
      }
    }

    return false;
  }

  private static String fontName;

  private static int fontSize;

  private static boolean antiAliasEnabled;

  private static Dimension toolbarSeparatorDimension;

  private static boolean decorated = true;

  private static boolean extendedFileChooserEnabled = true;

  private static boolean rememberFileChooserPreferences = true;

  private static boolean useSystemFileIcons = true;

  private static boolean defaultsCompatibilityMode = true;

  private PropertyChangeListener lafChangeListener;

  private UIDefaults orgDefaults;

  public String getID() {
    return "SyntheticaWalnutLookAndFeel";
  }

  public String getName() {
    return "Synthetica Walnut Look and Feel";
  }

}
