// Decompiled by Jad v1.5.8e. Copyright 2001 Pavel Kouznetsov.
// Jad home page: http://www.geocities.com/kpdus/jad.html
// Decompiler options: packimports(3) 
// Source File Name:   HelpButtonRootPaneUI.java

package de.javasoft.plaf.synthetica;

import de.javasoft.plaf.synthetica.painter.ImagePainter;
import java.awt.*;
import java.awt.event.MouseEvent;
import java.beans.PropertyChangeEvent;
import java.security.*;
import javax.swing.*;
import javax.swing.border.Border;
import javax.swing.event.MouseInputListener;
import javax.swing.plaf.ComponentUI;
import javax.swing.plaf.basic.BasicRootPaneUI;

// Referenced classes of package de.javasoft.plaf.synthetica:
//            SyntheticaTitlePane, SyntheticaLookAndFeel

public class HelpButtonRootPaneUI extends BasicRootPaneUI
{
    private static class SyntheticaRootLayout
        implements LayoutManager2
    {

        public void addLayoutComponent(String s, Component component)
        {
        }

        public void removeLayoutComponent(Component component)
        {
        }

        public void addLayoutComponent(Component component, Object obj)
        {
        }

        public float getLayoutAlignmentX(Container target)
        {
            return 0.0F;
        }

        public float getLayoutAlignmentY(Container target)
        {
            return 0.0F;
        }

        public void invalidateLayout(Container container)
        {
        }

        public Dimension preferredLayoutSize(Container parent)
        {
            Insets insets = parent.getInsets();
            JRootPane root = (JRootPane)parent;
            JComponent titlePane = ((HelpButtonRootPaneUI)root.getUI()).titlePane;
            Dimension dimC = new Dimension(0, 0);
            if(root.getContentPane() != null)
                dimC = root.getContentPane().getPreferredSize();
            else
                dimC = root.getSize();
            dimC = dimC != null ? dimC : new Dimension(0, 0);
            Dimension dimM = new Dimension(0, 0);
            if(root.getJMenuBar() != null)
                dimM = root.getJMenuBar().getPreferredSize();
            dimM = dimM != null ? dimM : new Dimension(0, 0);
            Dimension dimT = titlePane.getPreferredSize();
            dimT = dimT != null ? dimT : new Dimension(0, 0);
            int width = Math.max(dimC.width, Math.max(dimM.width, dimT.width)) + insets.left + insets.right;
            int height = dimC.height + dimM.height + dimT.height + insets.top + insets.bottom;
            return new Dimension(width, height);
        }

        public Dimension minimumLayoutSize(Container parent)
        {
            Insets insets = parent.getInsets();
            JRootPane root = (JRootPane)parent;
            JComponent titlePane = ((HelpButtonRootPaneUI)root.getUI()).titlePane;
            Dimension dimC = new Dimension(0, 0);
            if(root.getContentPane() != null)
                dimC = root.getContentPane().getMinimumSize();
            else
                dimC = root.getSize();
            dimC = dimC != null ? dimC : new Dimension(0, 0);
            Dimension dimM = new Dimension(0, 0);
            if(root.getJMenuBar() != null)
                dimM = root.getJMenuBar().getMinimumSize();
            dimM = dimM != null ? dimM : new Dimension(0, 0);
            Dimension dimT = titlePane.getMinimumSize();
            dimT = dimT != null ? dimT : new Dimension(0, 0);
            int width = Math.max(dimC.width, Math.max(dimM.width, dimT.width)) + insets.left + insets.right;
            int height = dimC.height + dimM.height + dimT.height + insets.top + insets.bottom;
            return new Dimension(width, height);
        }

        public Dimension maximumLayoutSize(Container target)
        {
            Insets insets = target.getInsets();
            JRootPane root = (JRootPane)target;
            JComponent titlePane = ((HelpButtonRootPaneUI)root.getUI()).titlePane;
            Dimension dimC = new Dimension(0, 0);
            if(root.getContentPane() != null)
                dimC = root.getContentPane().getMaximumSize();
            else
                dimC = root.getSize();
            dimC = dimC != null ? dimC : new Dimension(0x7fffffff, 0x7fffffff);
            Dimension dimM = new Dimension(0, 0);
            if(root.getJMenuBar() != null)
                dimM = root.getJMenuBar().getMaximumSize();
            dimM = dimM != null ? dimM : new Dimension(0x7fffffff, 0x7fffffff);
            Dimension dimT = titlePane.getMaximumSize();
            dimT = dimT != null ? dimT : new Dimension(0x7fffffff, 0x7fffffff);
            int width = Math.max(dimC.width, Math.max(dimM.width, dimT.width));
            if(width != 0x7fffffff)
                width += insets.left + insets.right;
            int height = Math.max(dimC.height, Math.max(dimM.height, dimT.height));
            if(height != 0x7fffffff)
                height += insets.top + insets.bottom;
            return new Dimension(width, height);
        }

        public void layoutContainer(Container parent)
        {
            JRootPane rootPane = (JRootPane)parent;
            Rectangle bounds = rootPane.getBounds();
            Insets insets = rootPane.getInsets() == null ? new Insets(0, 0, 0, 0) : rootPane.getInsets();
            int width = bounds.width - insets.right - insets.left;
            int height = bounds.height - insets.top - insets.bottom;
            int nextY = 0;
            if(rootPane.getLayeredPane() != null)
                rootPane.getLayeredPane().setBounds(insets.left, insets.top, width, height);
            if(rootPane.getGlassPane() != null)
                rootPane.getGlassPane().setBounds(insets.left, insets.top, width, height);
            JComponent titlePane = ((HelpButtonRootPaneUI)rootPane.getUI()).titlePane;
            if(titlePane.isEnabled())
            {
                Dimension dimT = titlePane.getPreferredSize();
                if(dimT != null)
                {
                    titlePane.setBounds(0, 0, width, dimT.height);
                    nextY += dimT.height;
                }
            }
            JMenuBar mBar = rootPane.getJMenuBar();
            if(mBar != null)
            {
                Dimension dimM = mBar.getPreferredSize();
                mBar.setBounds(0, nextY, width, dimM.height);
                nextY += dimM.height;
            }
            Container cPane = rootPane.getContentPane();
            if(cPane != null)
                cPane.setBounds(0, nextY, width, height >= nextY ? height - nextY : 0);
        }

        SyntheticaRootLayout()
        {
        }
    }

    private class MouseInputHandler
        implements MouseInputListener
    {

        public void mousePressed(MouseEvent evt)
        {
            if(rootPane.getWindowDecorationStyle() == 0)
                return;
            window.toFront();
            Point windowPoint = evt.getPoint();
            Point titlePanePoint = SwingUtilities.convertPoint(window, windowPoint, titlePane);
            int cursor = position2Cursor(window, evt.getX(), evt.getY());
            if(cursor == 0 && titlePane != null && titlePane.contains(titlePanePoint) && (dialog != null || frame != null && frame.getExtendedState() != 6))
            {
                windowAction = 1;
                dragXOffset = windowPoint.x;
                dragYOffset = windowPoint.y;
            } else
            if(isWindowResizable())
            {
                windowAction = 2;
                dragXOffset = windowPoint.x;
                dragYOffset = windowPoint.y;
                dragDimension = new Dimension(window.getWidth(), window.getHeight());
                resizeType = position2Cursor(window, windowPoint.x, windowPoint.y);
            }
        }

        public void mouseReleased(MouseEvent evt)
        {
            if(windowAction == 2 && !window.isValid())
            {
                window.validate();
                rootPane.repaint();
            }
            windowAction = -1;
            window.setCursor(Cursor.getDefaultCursor());
        }

        public void mouseMoved(MouseEvent evt)
        {
            if(rootPane.getWindowDecorationStyle() == 0)
                return;
            int cursor = position2Cursor(window, evt.getX(), evt.getY());
            if(cursor != 0 && isWindowResizable())
                window.setCursor(Cursor.getPredefinedCursor(cursor));
            else
                window.setCursor(Cursor.getDefaultCursor());
        }

        public void mouseEntered(MouseEvent evt)
        {
            mouseMoved(evt);
        }

        public void mouseExited(MouseEvent evt)
        {
            window.setCursor(Cursor.getDefaultCursor());
        }

        public void mouseDragged(MouseEvent evt)
        {
            if(windowAction == 1)
                try
                {
                    Point windowPt = (Point)(Point)AccessController.doPrivileged(getLocationAction);
                    windowPt.x -= dragXOffset;
                    windowPt.y -= dragYOffset;
                    window.setLocation(windowPt);
                }
                catch(PrivilegedActionException privilegedactionexception) { }
            else
            if(windowAction == 2)
            {
                Point pt = evt.getPoint();
                Dimension min = window.getMinimumSize();
                Rectangle bounds = window.getBounds();
                Rectangle startBounds = new Rectangle(bounds);
                if(resizeType == 11 || resizeType == 7 || resizeType == 5)
                    bounds.width = Math.max(min.width, (dragDimension.width + pt.x) - dragXOffset);
                if(resizeType == 9 || resizeType == 4 || resizeType == 5)
                    bounds.height = Math.max(min.height, (dragDimension.height + pt.y) - dragYOffset);
                if(resizeType == 8 || resizeType == 6 || resizeType == 7)
                {
                    bounds.height = Math.max(min.height, (bounds.height - pt.y) + dragYOffset);
                    if(bounds.height != min.height)
                        bounds.y += pt.y - dragYOffset;
                }
                if(resizeType == 10 || resizeType == 6 || resizeType == 4)
                {
                    bounds.width = Math.max(min.width, (bounds.width - pt.x) + dragXOffset);
                    if(bounds.width != min.width)
                        bounds.x += pt.x - dragXOffset;
                }
                if(!bounds.equals(startBounds))
                    window.setBounds(bounds);
            }
        }

        public void mouseClicked(MouseEvent evt)
        {
            if(frame == null)
                return;
            Point convertedPoint = SwingUtilities.convertPoint(window, evt.getPoint(), titlePane);
            if(titlePane != null && titlePane.contains(convertedPoint) && evt.getClickCount() == 2 && (evt.getModifiers() & 0x10) == 16)
                if(frame.isResizable() && isFrameResizable())
                    ((SyntheticaTitlePane)titlePane).maximize();
                else
                if(frame.isResizable() && !isFrameResizable())
                    ((SyntheticaTitlePane)titlePane).restore();
        }

        private int position2Cursor(Window w, int x, int y)
        {
            Insets insets = rootPane.getBorder().getBorderInsets(rootPane);
            int ww = w.getWidth();
            int wh = w.getHeight();
            int nwCornerSize = insets.top + insets.left;
            int neCornerSize = insets.top + insets.right;
            int swCornerSize = insets.bottom + insets.left;
            int seCornerSize = insets.bottom + insets.right;
            if(x < nwCornerSize && y < nwCornerSize)
                return 6;
            if(x > ww - neCornerSize && y < neCornerSize)
                return 7;
            if(x < swCornerSize && y > wh - swCornerSize)
                return 4;
            if(x > ww - seCornerSize && y > wh - seCornerSize)
                return 5;
            if(x < insets.top)
                return 10;
            if(x > ww - insets.bottom)
                return 11;
            if(y < insets.left)
                return 8;
            return y <= wh - insets.right ? 0 : 9;
        }

        private boolean isFrameResizable()
        {
            return frame != null && frame.isResizable() && (frame.getExtendedState() & 6) == 0;
        }

        private boolean isDialogResizable()
        {
            return dialog != null && dialog.isResizable();
        }

        private boolean isWindowResizable()
        {
            return isFrameResizable() || isDialogResizable();
        }

        private int windowAction;
        private int dragXOffset;
        private int dragYOffset;
        private Dimension dragDimension;
        private int resizeType;
        private Frame frame;
        private Dialog dialog;
        private final PrivilegedExceptionAction<Point> getLocationAction = new PrivilegedExceptionAction<Point>() {

            public Point run()
                throws HeadlessException
            {
                return MouseInfo.getPointerInfo().getLocation();
            }

        };
        MouseInputHandler()
        {
            frame = null;
            dialog = null;
            if(window instanceof Frame)
                frame = (Frame)window;
            else
            if(window instanceof Dialog)
                dialog = (Dialog)window;
        }
    }


    public HelpButtonRootPaneUI()
    {
    }

    public static ComponentUI createUI(JComponent c)
    {
        return new HelpButtonRootPaneUI();
    }

    public void installUI(JComponent c)
    {
        super.installUI(c);
        rootPane = (JRootPane)c;
        if(rootPane.getWindowDecorationStyle() != 0)
            installClientDecorations(rootPane);
    }

    public void uninstallUI(JComponent c)
    {
        super.uninstallUI(c);
        uninstallClientDecorations(rootPane);
        rootPane = null;
    }

    private void installClientDecorations(JRootPane root)
    {
        JComponent titlePane = null;
        Container parent = root.getParent();
        if  (parent instanceof HelpButtonTitlePane.HelpCallback) {
          titlePane = new HelpButtonTitlePane(root, this, 
              (HelpButtonTitlePane.HelpCallback) parent);
        }
        else {
          titlePane = new SyntheticaTitlePane(root, this);
        }
        String key = (new StringBuilder(String.valueOf(root.getParent().getClass().getSimpleName()))).append(".titlePane.enabled").toString();
        if(UIManager.get(key) != null && !UIManager.getBoolean(key))
            titlePane.setEnabled(false);
        setTitlePane(root, titlePane);
        installBorder(root);
        installWindowListeners(root, root.getParent());
        installLayout(root);
    }

    private void uninstallClientDecorations(JRootPane root)
    {
        setTitlePane(root, null);
        uninstallBorder(root);
        uninstallWindowListeners(root);
        uninstallLayout(root);
    }

    void installBorder(JRootPane root)
    {
        int style = root.getWindowDecorationStyle();
        if(style != 0)
            root.setBorder(new Border() {

                public void paintBorder(Component c, Graphics g, int x, int y, int w, int h)
                {
                    boolean maximized = (window instanceof Frame) && (((Frame)window).getExtendedState() & 6) == 6;
                    boolean opaque = SyntheticaWalnutLookAndFeel.get("Synthetica.rootPane.titlePane.opaque", window) != null ? SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.opaque", window) : true;
                    if(maximized && opaque)
                        return;
                    String imagePath = "Synthetica.rootPane.border";
                    if(window.isActive())
                        imagePath = (new StringBuilder(String.valueOf(imagePath))).append(".selected").toString();
                    imagePath = SyntheticaWalnutLookAndFeel.getString(imagePath, window);
                    Insets sInsets = SyntheticaWalnutLookAndFeel.getInsets("Synthetica.rootPane.border.insets", window);
                    Insets dInsets = sInsets;
                    if(maximized)
                    {
                        h = sInsets.top;
                        sInsets = SyntheticaWalnutLookAndFeel.getInsets("Synthetica.rootPane.border.size", window);
                        dInsets = new Insets(0, 0, 0, 0);
                    }
                    int xPolicy = 0;
                    if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.background.horizontalTiled", window))
                        xPolicy = 1;
                    int yPolicy = 0;
                    if(SyntheticaWalnutLookAndFeel.getBoolean("Synthetica.rootPane.titlePane.background.verticalTiled", window))
                        yPolicy = 1;
                    ImagePainter iPainter = new ImagePainter(g, x, y, w, h, imagePath, sInsets, dInsets, xPolicy, yPolicy);
                    if(maximized)
                        iPainter.drawCenter();
                    else
                        iPainter.drawBorder();
                    imagePath = "Synthetica.rootPane.border.light";
                    if(window.isActive())
                        imagePath = (new StringBuilder(String.valueOf(imagePath))).append(".selected").toString();
                    imagePath = SyntheticaWalnutLookAndFeel.getString(imagePath, window);
                    if(imagePath != null)
                    {
                        iPainter = new ImagePainter(g, x, y, w, h, imagePath, sInsets, dInsets, 0, 0);
                        if(maximized)
                            iPainter.drawCenter();
                        else
                            iPainter.drawBorder();
                    }
                }

                public Insets getBorderInsets(Component c)
                {
                    if((window instanceof Frame) && (((Frame)window).getExtendedState() & 6) == 6)
                        return new Insets(0, 0, 0, 0);
                    Insets insets = SyntheticaWalnutLookAndFeel.getInsets("Synthetica.rootPane.border.size", window);
                    if(insets == null)
                        insets = SyntheticaWalnutLookAndFeel.getInsets("Synthetica.rootPane.border.insets", window);
                    return insets;
                }

                public boolean isBorderOpaque()
                {
                    return false;
                }
            });
    }

    private void uninstallBorder(JRootPane root)
    {
        root.setBorder(null);
    }

    private void installWindowListeners(JRootPane root, Component parent)
    {
        window = (parent instanceof Window) ? (Window)parent : SwingUtilities.getWindowAncestor(parent);
        if(window != null)
        {
            if(mouseInputListener == null)
                mouseInputListener = new MouseInputHandler();
            window.addMouseListener(mouseInputListener);
            window.addMouseMotionListener(mouseInputListener);
        }
    }

    private void uninstallWindowListeners(JRootPane root)
    {
        if(window != null)
        {
            window.removeMouseListener(mouseInputListener);
            window.removeMouseMotionListener(mouseInputListener);
        }
        mouseInputListener = null;
        window = null;
    }

    private void installLayout(JRootPane root)
    {
        if(layoutManager == null)
            layoutManager = new SyntheticaRootLayout();
        oldLayoutManager = root.getLayout();
        root.setLayout(layoutManager);
    }

    private void uninstallLayout(JRootPane root)
    {
        if(oldLayoutManager != null)
            root.setLayout(oldLayoutManager);
        oldLayoutManager = null;
        layoutManager = null;
    }

    private void setTitlePane(JRootPane root, JComponent titlePane)
    {
        JLayeredPane layeredPane = root.getLayeredPane();
        if(this.titlePane != null)
        {
            this.titlePane.setVisible(false);
            layeredPane.remove(this.titlePane);
        }
        if(titlePane != null)
        {
            layeredPane.add(titlePane, JLayeredPane.FRAME_CONTENT_LAYER);
            titlePane.setVisible(true);
        }
        this.titlePane = titlePane;
    }

    public void propertyChange(PropertyChangeEvent e)
    {
        super.propertyChange(e);
        String propertyName = e.getPropertyName();
        if(propertyName == null)
            return;
        if(propertyName.equals("windowDecorationStyle"))
        {
            uninstallClientDecorations(rootPane);
            if(rootPane.getWindowDecorationStyle() != 0)
                installClientDecorations(rootPane);
        } else
        if(propertyName.equals("ancestor"))
        {
            uninstallWindowListeners(rootPane);
            if(rootPane.getWindowDecorationStyle() != 0)
                installWindowListeners(rootPane, rootPane.getParent());
        }
    }

    public void setMaximizedBounds(Frame frame)
    {
        if(System.getProperty("synthetica.frame.fullscreen") != null)
            return;
        GraphicsConfiguration gc = frame.getGraphicsConfiguration();
        Rectangle screenBounds = gc.getBounds();
        if(System.getProperty("synthetica.frame.disablePosCorrection") == null)
            screenBounds.x = 0;
        Insets screenInsets = Toolkit.getDefaultToolkit().getScreenInsets(gc);
        if(System.getProperty("synthetica.frame.disableAutoHideTaskBarCorrection") == null && screenInsets.bottom == 0)
            screenInsets.bottom++;
        Rectangle maxBounds = new Rectangle(screenBounds.x + screenInsets.left, screenBounds.y + screenInsets.top, screenBounds.width - (screenInsets.left + screenInsets.right), screenBounds.height - (screenInsets.top + screenInsets.bottom));
        frame.setMaximizedBounds(maxBounds);
    }

    private Window window;
    private JRootPane rootPane;
    private LayoutManager layoutManager;
    private LayoutManager oldLayoutManager;
    private MouseInputListener mouseInputListener;
    private JComponent titlePane;



}
