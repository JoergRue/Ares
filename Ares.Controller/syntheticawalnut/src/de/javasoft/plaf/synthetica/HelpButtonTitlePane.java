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

import de.javasoft.plaf.synthetica.painter.ImagePainter;
import de.javasoft.util.java2d.DropShadow;
import java.awt.*;
import java.awt.event.*;
import java.awt.image.BufferedImage;
import java.beans.PropertyChangeEvent;
import java.beans.PropertyChangeListener;
import javax.swing.*;
import javax.swing.plaf.basic.BasicRootPaneUI;
import javax.swing.plaf.synth.*;

// Referenced classes of package de.javasoft.plaf.synthetica:
//            SyntheticaLookAndFeel, SyntheticaRootPaneUI

public class HelpButtonTitlePane extends JPanel
{
  
    public interface HelpCallback {
      void callHelp();
    }
    
    private HelpCallback helpCallback;
    
    public HelpButtonTitlePane(JRootPane root, BasicRootPaneUI ui, HelpCallback hc)
    {
        selected = true;
        rootPane = root;
        rootPaneUI = ui;
        helpCallback = hc;
        java.awt.Container parent = rootPane.getParent();
        window = (parent instanceof Window) ? (Window)parent : SwingUtilities.getWindowAncestor(parent);
        if(window instanceof Frame)
            frame = (Frame)window;
        else
        if(window instanceof Dialog)
            dialog = (Dialog)window;
        if(SyntheticaWalnutLookAndFeel.get("Synthetica.rootPane.titlePane.opaque", window) != null)
          setOpaque(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.opaque", window));
        int xGap = 4;
        javax.swing.border.Border titleBorder = BorderFactory.createEmptyBorder(3, 0, 4, 0);
        setLayout(new BoxLayout(this, 2));
        add(Box.createHorizontalStrut(xGap));
        closeAction = new AbstractAction() {
            public void actionPerformed(ActionEvent e)
            {
                close();
            }
        };
        helpAction = new AbstractAction() {
            public void actionPerformed(ActionEvent e) 
            {
                help();
            }
        };
        int decorationStyle = rootPane.getWindowDecorationStyle();
        if(decorationStyle == 1)
        {
            iconifyAction = new AbstractAction() {
                public void actionPerformed(ActionEvent e)
                {
                    iconify();
                }
            };
            restoreAction = new AbstractAction() {
                public void actionPerformed(ActionEvent e)
                {
                    restore();
                }
            };
            maximizeAction = new AbstractAction() {
                public void actionPerformed(ActionEvent e)
                {
                    maximize();
                }
            };
            menuButton = createTitlePaneButton();
            menuButton.setIcon(getFrameIcon());
            window.addPropertyChangeListener(new PropertyChangeListener() {

                public void propertyChange(PropertyChangeEvent evt)
                {
                    if("iconImage".equals(evt.getPropertyName()))
                        menuButton.setIcon(getFrameIcon());
                }
            });
            systemMenu = new JPopupMenu();
            addMenuItems(systemMenu);
            menuButton.addActionListener(new ActionListener() {

                public void actionPerformed(ActionEvent e)
                {
                    systemMenu.show(HelpButtonTitlePane.this, 0, getHeight());
                }
            });
            add(menuButton);
            add(Box.createHorizontalStrut(xGap));
        }
        titleLabel = getTitle() == null ? new JLabel("") : new JLabel(" ");
        titleLabel.setBorder(titleBorder);
        titleLabel.setFont(titleLabel.getFont().deriveFont(1));
        add(titleLabel);
        add(Box.createHorizontalGlue());
        createButtons();
        add(helpButton);
        add(Box.createHorizontalStrut(xGap));
        if(decorationStyle == 1 && dialog == null)
        {
            add(Box.createHorizontalStrut(xGap));
            add(iconifyButton);
            add(toggleButton);
            add(Box.createHorizontalStrut(xGap));
        }
        add(closeButton);
        add(Box.createHorizontalStrut(xGap));
        installListeners();
        setComponentsActiveState(window.isActive());
        if(frame != null && frame.getIconImage() == null)
            frame.setIconImage(((ImageIcon)getFrameIcon()).getImage());
        updateState();
    }

    private void installListeners()
    {
        windowListener = new WindowAdapter() {

            public void windowOpened(WindowEvent e)
            {
                if(rootPane.getWindowDecorationStyle() == 1 && dialog == null)
                {
                    updateToggleButton();
                    updateState();
                }
            }

            public void windowStateChanged(WindowEvent e)
            {
                updateToggleButton();
                updateState();
            }

            public void windowActivated(WindowEvent ev)
            {
                setActive(true);
                selected = true;
            }

            public void windowDeactivated(WindowEvent ev)
            {
                setActive(false);
                selected = false;
            }
        };
        window.addWindowListener(windowListener);
        window.addWindowStateListener((WindowStateListener)windowListener);
        propertyChangeListener = new PropertyChangeListener() {

            public void propertyChange(PropertyChangeEvent pce)
            {
                String name = pce.getPropertyName();
                if("title".equals(name))
                    titleLabel.setText(getTitle());
                else
                if("resizable".equals(name))
                {
                    boolean resizable = ((Boolean)pce.getNewValue()).booleanValue();
                    toggleButton.setEnabled(resizable);
                    systemMenu.removeAll();
                    addMenuItems(systemMenu);
                }
            }
        };
        window.addPropertyChangeListener(propertyChangeListener);
    }

    public JRootPane getRootPane()
    {
        return rootPane;
    }

    private String getTitle()
    {
        if(frame != null)
            return frame.getTitle();
        if(dialog != null)
            return dialog.getTitle();
        else
            return null;
    }

    private Icon getFrameIcon()
    {
        Image image = frame == null ? null : frame.getIconImage();
        Icon icon = null;
        if(image != null)
        {
            if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.menuButton.useOriginalImageSize", window))
                icon = new ImageIcon(image);
            else
                icon = new ImageIcon(image.getScaledInstance(16, 16, 4));
        } else
        {
            SynthStyle ss = SynthLookAndFeel.getStyle(rootPane, Region.ROOT_PANE);
            SynthContext sc = new SynthContext(rootPane, Region.ROOT_PANE, ss, 1024);
            icon = ss.getIcon(sc, "RootPane.icon");
        }
        return icon;
    }

    private void setActive(boolean active)
    {
        setComponentsActiveState(active);
        getRootPane().repaint();
    }

    private void setComponentsActiveState(boolean active)
    {
        javax.swing.JComponent c = new JInternalFrame();
        SynthStyle ss = SynthLookAndFeel.getStyleFactory().getStyle(c, Region.INTERNAL_FRAME_TITLE_PANE);
        int state = 1024;
        if(active)
            state = 512;
        SynthContext sc = new SynthContext(c, Region.INTERNAL_FRAME_TITLE_PANE, ss, state);
        Font font = ss.getFont(sc);
        font = font.deriveFont(font.getStyle(), font.getSize());
        titleLabel.setFont(font);
        Color fg = ss.getColor(sc, ColorType.FOREGROUND);
        fg = new Color(fg.getRGB());
        titleLabel.setForeground(fg);
        closeButton.putClientProperty("paintActive", Boolean.valueOf(active));
        helpButton.putClientProperty("paintActive", Boolean.valueOf(active));
        if(rootPane.getWindowDecorationStyle() == 1 && dialog == null)
        {
            iconifyButton.putClientProperty("paintActive", Boolean.valueOf(active));
            toggleButton.putClientProperty("paintActive", Boolean.valueOf(active));
        }
    }

    private boolean isFrameResizable()
    {
        return frame != null && frame.isResizable();
    }

    private boolean isFrameMaximized()
    {
        return frame != null && (frame.getExtendedState() & 6) == 6;
    }

    private boolean isSelected()
    {
        return window != null ? window.isActive() : true;
    }

    private void addMenuItems(JPopupMenu menu)
    {
        JMenuItem mi = menu.add(restoreAction);
        mi.setText(SyntheticaWalnutLookAndFeel.getString("InternalFrameTitlePane.restoreButtonText", window));
        mi.setMnemonic('R');
        mi.setEnabled(isFrameResizable());
        mi = menu.add(iconifyAction);
        mi.setText(SyntheticaWalnutLookAndFeel.getString("InternalFrameTitlePane.minimizeButtonText", window));
        mi.setMnemonic('n');
        mi.setEnabled(frame != null);
        if(Toolkit.getDefaultToolkit().isFrameStateSupported(6))
        {
            mi = menu.add(maximizeAction);
            mi.setText(SyntheticaWalnutLookAndFeel.getString("InternalFrameTitlePane.maximizeButtonText", window));
            mi.setMnemonic('x');
            mi.setEnabled(isFrameResizable());
        }
        menu.addSeparator();
        mi = menu.add(closeAction);
        mi.setText(SyntheticaWalnutLookAndFeel.getString("InternalFrameTitlePane.closeButtonText", window));
        mi.setMnemonic('C');
    }

    private JButton createTitlePaneButton()
    {
        JButton button = new JButton();
        button.setFocusPainted(false);
        button.setFocusable(false);
        button.setOpaque(false);
        button.setBorder(BorderFactory.createEmptyBorder());
        return button;
    }
    
    private static class MyMouseListener extends MouseAdapter {
      public void mouseEntered(MouseEvent evt)
      {
          JButton b = (JButton)evt.getSource();
          String name = (new StringBuilder("Synthetica.")).append(b.getName()).append("Icon.hover").toString();
          b.setIcon((Icon)b.getClientProperty(name));
      }

      public void mouseExited(MouseEvent evt)
      {
          JButton b = (JButton)evt.getSource();
          String name = (new StringBuilder("Synthetica.")).append(b.getName()).append("Icon").toString();
          b.setIcon((Icon)b.getClientProperty(name));
      }      
    }

    private void createButtons()
    {
        MouseAdapter listener = new MyMouseListener();
        SynthStyle ss = SynthLookAndFeel.getStyle(rootPane, Region.ROOT_PANE);
        SynthContext sc = new SynthContext(rootPane, Region.ROOT_PANE, ss, 1024);
        Icon closeIcon = ss.getIcon(sc, "RootPane.closeIcon");
        Icon helpIcon = ss.getIcon(sc, "RootPane.helpIcon");
        Icon iconifyIcon = ss.getIcon(sc, "RootPane.iconifyIcon");
        Icon maximizeIcon = ss.getIcon(sc, "RootPane.maximizeIcon");
        Icon minimizeIcon = ss.getIcon(sc, "RootPane.minimizeIcon");
        sc = new SynthContext(rootPane, Region.ROOT_PANE, ss, 2);
        Icon closeIconHover = ss.getIcon(sc, "RootPane.closeIcon");
        Icon helpIconHover = ss.getIcon(sc, "RootPane.helpIcon");
        Icon iconifyIconHover = ss.getIcon(sc, "RootPane.iconifyIcon");
        Icon maximizeIconHover = ss.getIcon(sc, "RootPane.maximizeIcon");
        Icon minimizeIconHover = ss.getIcon(sc, "RootPane.minimizeIcon");
        closeButton = createTitlePaneButton();
        closeButton.setName("close");
        closeButton.putClientProperty("Synthetica.closeIcon", closeIcon);
        closeButton.putClientProperty("Synthetica.closeIcon.hover", closeIconHover);
        closeButton.setAction(closeAction);
        closeButton.getAccessibleContext().setAccessibleName("Close");
        closeButton.setIcon(closeIcon);
        closeButton.addMouseListener(listener);
        helpButton = createTitlePaneButton();
        helpButton.setName("help");
        helpButton.putClientProperty("Synthetica.helpIcon", helpIcon);
        helpButton.putClientProperty("Synthetica.helpIcon.hover", helpIconHover);
        helpButton.setAction(helpAction);
        helpButton.getAccessibleContext().setAccessibleName("Help");
        helpButton.setIcon(helpIcon);
        helpButton.addMouseListener(listener);
        if(rootPane.getWindowDecorationStyle() == 1 && dialog == null)
        {
            iconifyButton = createTitlePaneButton();
            iconifyButton.setName("iconify");
            iconifyButton.putClientProperty("Synthetica.iconifyIcon", iconifyIcon);
            iconifyButton.putClientProperty("Synthetica.iconifyIcon.hover", iconifyIconHover);
            iconifyButton.setAction(iconifyAction);
            iconifyButton.getAccessibleContext().setAccessibleName("Iconify");
            iconifyButton.setIcon(iconifyIcon);
            iconifyButton.addMouseListener(listener);
            toggleButton = createTitlePaneButton();
            toggleButton.putClientProperty("Synthetica.maximizeIcon", maximizeIcon);
            toggleButton.putClientProperty("Synthetica.maximizeIcon.hover", maximizeIconHover);
            toggleButton.putClientProperty("Synthetica.minimizeIcon", minimizeIcon);
            toggleButton.putClientProperty("Synthetica.minimizeIcon.hover", minimizeIconHover);
            updateToggleButton();
            toggleButton.addMouseListener(listener);
        }
    }

    private void updateToggleButton()
    {
        Icon icon = null;
        if(!isFrameMaximized())
        {
            toggleButton.setAction(maximizeAction);
            toggleButton.getAccessibleContext().setAccessibleName("Maximize");
            toggleButton.setName("maximize");
            icon = (Icon)toggleButton.getClientProperty("Synthetica.maximizeIcon");
        } else
        {
            toggleButton.setAction(restoreAction);
            toggleButton.getAccessibleContext().setAccessibleName("Restore");
            toggleButton.setName("minimize");
            icon = (Icon)toggleButton.getClientProperty("Synthetica.minimizeIcon");
        }
        toggleButton.setIcon(icon);
        toggleButton.setEnabled(isFrameResizable());
    }

    public void paintComponent(Graphics g)
    {
        super.paintComponent(g);
        String imagePath = "Synthetica.rootPane.titlePane.background";
        if(isSelected())
            imagePath = (new StringBuilder(String.valueOf(imagePath))).append(".selected").toString();
        imagePath = SyntheticaWalnutLookAndFeel.getString(imagePath, window);
        java.awt.Insets insets = SyntheticaWalnutLookAndFeel.getInsets("Synthetica.rootPane.titlePane.background.insets", window);
        int xPolicy = 0;
        if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.background.horizontalTiled", window))
            xPolicy = 1;
        int yPolicy = 0;
        if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.background.verticalTiled", window))
            yPolicy = 1;
        ImagePainter iPainter = null;
        if(imagePath != null)
        {
            iPainter = new ImagePainter(g, 0, 0, getWidth(), getHeight(), imagePath, insets, insets, xPolicy, yPolicy);
            iPainter.draw();
        }
        imagePath = "Synthetica.rootPane.titlePane.background.light";
        if(isSelected())
            imagePath = (new StringBuilder(String.valueOf(imagePath))).append(".selected").toString();
        imagePath = SyntheticaWalnutLookAndFeel.getString(imagePath, window);
        if(imagePath != null)
        {
            iPainter = new ImagePainter(g, 0, 0, getWidth(), getHeight(), imagePath, insets, insets, 0, yPolicy);
            iPainter.draw();
        }
        String title = getTitle();
        if(title == null || title.length() == 0)
            return;
        FontMetrics fm = getFontMetrics(titleLabel.getFont());
        int th = fm.getHeight();
        int tw = fm.stringWidth(title);
        int tx = menuButton == null ? 4 : menuButton.getWidth() + 8;
        if(title.startsWith("W"))
            tx++;
        int ty = (getSize().height - th) / 2;
        JInternalFrame iFrame = new JInternalFrame();
        SynthStyle ss = SynthLookAndFeel.getStyle(iFrame, Region.INTERNAL_FRAME_TITLE_PANE);
        int state = 1024;
        if(selected)
            state = 512;
        SynthContext sc = new SynthContext(iFrame, Region.INTERNAL_FRAME_TITLE_PANE, ss, state);
        if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.dropShadow", window) && selected)
        {
            BufferedImage image = new BufferedImage(tw, th, 2);
            Graphics g2 = image.createGraphics();
            g2.setFont(titleLabel.getFont());
            g2.drawString(title, 0, fm.getAscent());
            g2.dispose();
            DropShadow ds = new DropShadow(image);
            ds.setDistance(SyntheticaWalnutLookAndFeel.getInt("Synthetica.rootPane.titlePane.dropShadow.distance", window, -5));
            if(SyntheticaWalnutLookAndFeel.getColor("Synthetica.rootPane.titlePane.dropShadow.color", window) != null)
                ds.setShadowColor(SyntheticaWalnutLookAndFeel.getColor("Synthetica.rootPane.titlePane.dropShadow.color", window));
            if(SyntheticaWalnutLookAndFeel.get("Synthetica.rootPane.titlePane.dropShadow.highQuality", window) != null)
                ds.setQuality(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.dropShadow.highQuality", window));
            ds.paintShadow(g, tx, ty);
        }
        g.setColor(ss.getColor(sc, ColorType.FOREGROUND));
        g.setFont(titleLabel.getFont());
        ss.getGraphicsUtils(sc).paintText(sc, g, title, tx, ty, -1);
    }

    private void close()
    {
        window.dispatchEvent(new WindowEvent(window, 201));
    }
    
    private void help()
    {
        if (helpCallback != null) helpCallback.callHelp();
    }

    private void iconify()
    {
        int state = frame.getExtendedState();
        frame.setExtendedState(state | 1);
        updateState();
    }

    void maximize()
    {
        int state = frame.getExtendedState();
        if ((rootPaneUI instanceof SyntheticaRootPaneUI))
        {
          ((SyntheticaRootPaneUI)rootPaneUI).setMaximizedBounds(frame);
        }
        frame.setExtendedState(state | 6);
        updateState();
    }

    void restore()
    {
        int state = frame.getExtendedState();
        if((state & 1) == 1)
            frame.setExtendedState(state ^ 1);
        else
        if((state & 6) == 6)
            frame.setExtendedState(state ^ 6);
        updateState();
    }

    private void updateState()
    {
        if(frame == null)
            return;
        switch(frame.getExtendedState())
        {
        case 6: // '\006'
            restoreAction.setEnabled(true);
            maximizeAction.setEnabled(false);
            iconifyAction.setEnabled(true);
            break;

        case 1: // '\001'
            restoreAction.setEnabled(true);
            maximizeAction.setEnabled(true);
            iconifyAction.setEnabled(false);
            break;

        default:
            restoreAction.setEnabled(false);
            maximizeAction.setEnabled(true);
            iconifyAction.setEnabled(true);
            break;
        }
    }

    private static final long serialVersionUID = 0xaa74455e0b6e7e41L;
    private JRootPane rootPane;
    private BasicRootPaneUI rootPaneUI;
    private Window window;
    private Frame frame;
    private Dialog dialog;
    private JButton menuButton;
    private JLabel titleLabel;
    private JButton toggleButton;
    private JButton iconifyButton;
    private JButton closeButton;
    private JButton helpButton;
    private JPopupMenu systemMenu;
    private Action closeAction;
    private Action helpAction;
    private Action iconifyAction;
    private Action restoreAction;
    private Action maximizeAction;
    private WindowListener windowListener;
    private PropertyChangeListener propertyChangeListener;
    private boolean selected;















}
