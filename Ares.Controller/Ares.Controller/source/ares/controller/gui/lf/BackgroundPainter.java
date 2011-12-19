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

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
import java.awt.Rectangle;
import java.awt.Shape;
import java.awt.geom.Area;
import java.awt.geom.GeneralPath;
import java.awt.image.ImageObserver;
import java.awt.image.VolatileImage;

import javax.swing.ImageIcon;
//import javax.swing.JComboBox;
import javax.swing.AbstractButton;
import javax.swing.JScrollBar;
import javax.swing.JSpinner;
import javax.swing.JTabbedPane;
import javax.swing.JToggleButton;
import javax.swing.SwingConstants;
import javax.swing.UIManager;
import javax.swing.plaf.synth.SynthConstants;
import javax.swing.plaf.synth.SynthContext;

import de.javasoft.plaf.synthetica.painter.SyntheticaPainter;

public class BackgroundPainter extends SyntheticaPainter {
  
  private static Image loadImage(String id) {
    String img = UIManager.getString("ares.controller.gui.lf." + id); //$NON-NLS-1$
    ImageIcon icon = new ImageIcon(BackgroundPainter.class.getResource(img));
    return icon.getImage();
  }

  private static Image[] createImages() {
    Image[] images = new Image[IMAGE_COUNT];
    images[0]  = loadImage("bg"); //$NON-NLS-1$
    images[1]  = loadImage("hout-hover"); //$NON-NLS-1$
    images[2]  = loadImage("hout"); //$NON-NLS-1$
    images[3]  = loadImage("hout-vert-active"); //$NON-NLS-1$
    images[4]  = loadImage("hout-vert-hover"); //$NON-NLS-1$
    images[5]  = loadImage("hout-vert"); //$NON-NLS-1$
    images[6]  = loadImage("hout-active"); //$NON-NLS-1$
    images[7]  = loadImage("hout-hover"); //$NON-NLS-1$
    images[8]  = loadImage("hout"); //$NON-NLS-1$
    images[9]  = loadImage("arrowButtonLeft"); //$NON-NLS-1$
    images[10] = loadImage("arrowButtonRight"); //$NON-NLS-1$
    images[11] = loadImage("arrowButtonUp"); //$NON-NLS-1$
    images[12] = loadImage("arrowButtonDown"); //$NON-NLS-1$
    images[13] = loadImage("border"); //$NON-NLS-1$
    return images;
  }
  
  private static VolatileImage[] createVolatiles() {
    VolatileImage[] images = new VolatileImage[IMAGE_COUNT];
    return images;
  }
  
  private static final int IMAGE_COUNT = 14; 
  
  private static class ImageHolder {
  
    private static Image[] bufferedImages = createImages();
    private static VolatileImage[] volatileImages = createVolatiles();
    
  }
  
  private static VolatileImage drawVolatileImage(Graphics2D g, VolatileImage img, 
      int x, int y, Image orig, ImageObserver observer) {
    final int MAX_TRIES = 100;
    for (int i=0; i<MAX_TRIES; i++) {
        boolean copyImage = false; 
        if (img != null) {
            // Draw the volatile image
            g.drawImage(img, x, y, observer);

            // Check if it is still valid
            if (!img.contentsLost()) {
                return img;
            }
        } else {
            // Create the volatile image
            img = g.getDeviceConfiguration().createCompatibleVolatileImage(
                orig.getWidth(null), orig.getHeight(null));
        }

        // Determine how to fix the volatile image
        switch (img.validate(g.getDeviceConfiguration())) {
        case VolatileImage.IMAGE_OK:
            // This should not happen
            break;
        case VolatileImage.IMAGE_INCOMPATIBLE:
            // Create a new volatile image object;
            // this could happen if the component was moved to another device
            img.flush();
            img = g.getDeviceConfiguration().createCompatibleVolatileImage(
                orig.getWidth(null), orig.getHeight(null));
            copyImage = true;
            break;
        case VolatileImage.IMAGE_RESTORED:
            copyImage = true;
            break;
        default:
            break;
        }
        if (copyImage) {
          // Copy the original image to accelerated image memory
          Graphics2D gc = (Graphics2D)img.createGraphics();
          gc.drawImage(orig, 0, 0, null);
          gc.dispose();          
        }
    }

    // The image failed to be drawn after MAX_TRIES;
    // draw with the non-accelerated image
    g.drawImage(orig, x, y, observer);
    return img;
  }
  
  private static void drawImage(Graphics2D g, int x, int y, int index, ImageObserver observer) {
    VolatileImage volImg = drawVolatileImage(g, ImageHolder.volatileImages[index], 
        x, y, ImageHolder.bufferedImages[index], observer);
    ImageHolder.volatileImages[index] = volImg;
  }
  
  public void paintScrollBarTrackBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBackground(context, g, x, y, w, h, 0);
  }

  public void paintTableHeaderBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBackground(context, g, x, y, w, h, 2);
  }
  
  // public void paintButtonBackground(SynthContext context, Graphics g, int x,
  // int y, int w, int h) {
  // paintBackground(context, g, x, y, w, h, GetBGImage());
  // }

  public void paintScrollBarThumbBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h, int orientation) {
    JScrollBar bar = (JScrollBar) context.getComponent();
    int imgIndex = -1;
    if (orientation == JScrollBar.VERTICAL) {
      if (bar.getValueIsAdjusting()) {
        imgIndex = 3;
      }
      else if ((context.getComponentState() & SynthConstants.MOUSE_OVER) != 0) {
        imgIndex = 4;
      }
      else {
        imgIndex = 5;
      }
    }
    else {
      if (bar.getValueIsAdjusting()) {
        imgIndex = 6;
      }
      else if ((context.getComponentState() & SynthConstants.MOUSE_OVER) != 0) {
        imgIndex = 7;
      }
      else {
        imgIndex = 8;
      }
    }
    paintBackground(context, g, x, y, w, h, imgIndex);
  }

  public void paintArrowButtonBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h) {
    java.awt.Container c = context.getComponent().getParent();
    if ((c instanceof JSpinner)
    /* (c instanceof JComboBox) */) {
      super.paintArrowButtonBackground(context, g, x, y, w, h);
    }
    else {
      paintBtnBackground(context, g, x, y, w, h);
    }
  }

  private static int getArrowBtnImage(boolean horizontal, boolean leftOrUp) {
    final int baseIndex = 9;
    if (horizontal) {
      if (leftOrUp) {
        return baseIndex + 0;
      }
      else {
        return baseIndex + 1;
      }
    }
    else {
      if (leftOrUp) {
        return baseIndex + 2;
      }
      else {
        return baseIndex + 3;
      }
    }
  }

  public void paintArrowButtonForeground(SynthContext context, Graphics g,
      int x, int y, int w, int h, int direction) {
    java.awt.Container c = context.getComponent().getParent();
    if ((c instanceof JSpinner)
    /* (c instanceof JComboBox) */) {
      super.paintArrowButtonForeground(context, g, x, y, w, h, direction);
    }
    else {
      int image = -1;
      switch (direction) {
      case SwingConstants.NORTH:
        image = getArrowBtnImage(false, true);
        break;
      case SwingConstants.SOUTH:
        image = getArrowBtnImage(false, false);
        break;
      case SwingConstants.EAST:
        image = getArrowBtnImage(true, false);
        break;
      default:
        image = getArrowBtnImage(true, true);
        break;
      }
      int w2 = ImageHolder.bufferedImages[image].getWidth(context.getComponent());
      int h2 = ImageHolder.bufferedImages[image].getHeight(context.getComponent());
      int x2 = x + (w - w2) / 2;
      int y2 = y + (h - h2) / 2;
      if (w2 > w || h2 > h) {
        w2 = w2 > w ? w : w2;
        h2 = h2 > h ? h : h2;
        x2 = x;
        y2 = y;
        //ImageHolder.bufferedImages[image] = ImageHolder.bufferedImages[image].getScaledInstance(w2, h2, Image.SCALE_DEFAULT);
        ImageHolder.volatileImages[image] = null;
      }
      //drawImage((Graphics2D)g, image, x2, y2, context.getComponent());
      g.drawImage(ImageHolder.bufferedImages[image], x2, y2, w2, h2, context.getComponent());
    }
  }

  public void paintCheckBoxBorder(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
  }

  public void paintCheckBoxBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    if ((context.getComponentState() & SynthConstants.FOCUSED) != 0) {
      Color c = g.getColor();
      g.setColor(Colors.getFocusRectColor());
      g.drawRect(x, y, w-1, h-1);
      g.setColor(c);
    }
  }
  
  public void paintComboBoxBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
	paintComboBoxBorder(context, g, x, y, w, h);
    paintBackground(context, g, x+1, y+1, w-2, h-2, 2);
  }

  /*
  public void paintSeparatorForeground(SynthContext synthcontext, Graphics g, int i, int j, int k, int l, int i1) {
	  Color c = g.getColor();
	  g.setColor(Color.GRAY);
	  g.drawLine(i, j, k, l);
	  g.setColor(c);
  }
  */
  
  public void paintMenuBarBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBackground(context, g, x, y, w, h, 2);
  }

  public void paintMenuItemBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    int imgIndex = ((context.getComponentState() & SynthConstants.MOUSE_OVER) != 0) ? 1 : 2;
    paintBackground(context, g, x, y, w, h, imgIndex);
  }

  public void paintMenuBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    int imgIndex = ((context.getComponentState() & 
        (SynthConstants.SELECTED | SynthConstants.FOCUSED | SynthConstants.MOUSE_OVER)) != 0) ? 1 : 2;
    paintBackground(context, g, x, y, w, h, imgIndex);
  }
  
  
  public void paintPopupMenuBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    super.paintPopupMenuBackground(context, g, x, y, w, h);
  }
  

  public void paintOptionPaneBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h) {
    paintBackground(context, g, x, y, w, h, 0);
  }

  public void paintFileChooserBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h) {
    paintBackground(context, g, x, y, w, h, 0);
  }
  
  public void paintMenuBarBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    /*Color c = g.getColor();
    g.setColor(Colors.getFocusRectColor());
    g.drawRect(x+1, y+1, w-2, h-2);
    g.setColor(c);*/
  }

  public void paintMenuItemBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    /*Color c = g.getColor();
    g.setColor(Colors.getFocusRectColor());
    g.drawRect(x+1, y+1, w-2, h-2);
    g.setColor(c);*/
  }

  public void paintPopupMenuBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    Color c = g.getColor();
    g.setColor(Colors.getFocusRectColor());
    g.drawRoundRect(x, y, w-1, h-1, 3, 3);
    g.setColor(c);
  }

  public void paintMenuBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    Color c = g.getColor();
    g.setColor(Colors.getFocusRectColor());
    g.drawRoundRect(x, y, w, h, 3, 3);
    g.setColor(c);
  }

  public void paintButtonBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    paintBtnBorder(context, g, x, y, w, h);
  }

  public void paintToggleButtonBorder(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBtnBorder(context, g, x, y, w, h);
  }

  public void paintToggleButtonBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h) {
    paintBtnBackground(context, g, x, y, w, h);
  }

  public void paintButtonBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBtnBackground(context, g, x, y, w, h);
  }

  private void paintBtnBorder(SynthContext context, Graphics g, int x, int y,
      int w, int h) {
    if (((context.getComponentState() & SynthConstants.PRESSED) != 0)
        || (context.getComponent() instanceof AbstractButton && ((AbstractButton) context
            .getComponent()).isSelected())) {
      draw3DRect(g, x, y, w - 1, h - 1, false);
    }
    else {
      draw3DRect(g, x, y, w - 1, h - 1, true);
    }
  }
  
  public void paintComboBoxBorder(SynthContext context, Graphics g, int x, int y, int w, int h) {
	Color c = g.getColor();
	g.setColor(Color.GRAY);
	g.drawRect(x, y, w - 1, h - 1);
	g.setColor(c);
  }

  private void draw3DRect(Graphics g, int x, int y, int width, int height,
      boolean raised) {
    Color c = g.getColor();
    Color brighter = Color.WHITE;
    Color darker = raised ? Color.GRAY : Color.DARK_GRAY;

    g.setColor(raised ? brighter : darker);
    g.drawLine(x, y, x, y + height);
    g.drawLine(x, y, x + width, y);
    g.setColor(raised ? darker : brighter);
    g.drawLine(x, y + height, x + width, y + height);
    g.drawLine(x + width, y, x + width, y + height);
    g.setColor(c);
  }

  private void paintBtnBackground(SynthContext context, Graphics g, int x,
      int y, int w, int h) {
    paintBtnBorder(context, g, x, y, w, h);
    int imgIndex = -1;
    if ((context.getComponentState() & SynthConstants.PRESSED) != 0) {
      imgIndex = 6;
    }
    else if ((context.getComponentState() & SynthConstants.DISABLED) != 0) {
      imgIndex = -1;
    }
    else if (context.getComponent() instanceof JToggleButton
        && ((JToggleButton) context.getComponent()).isSelected()) {
      imgIndex = 7;
    }
    else if ((context.getComponentState() & SynthConstants.MOUSE_OVER) != 0) {
      imgIndex = 7;
    }
    else {
      imgIndex = 8;
    }
    if (imgIndex != -1) {
      paintBackground(context, g, x + 1, y + 1, w - 3, h - 3, imgIndex);
    }
    if ((context.getComponentState() & SynthConstants.FOCUSED) != 0) {
      Color c = g.getColor();
      g.setColor(Colors.getFocusRectColor());
      if (((context.getComponentState() & SynthConstants.PRESSED) != 0)
          || (context.getComponent() instanceof AbstractButton && ((AbstractButton) context
              .getComponent()).isSelected())) {
        g.drawRect(x + 2, y + 2, w - 7, h - 7);
      }
      else {
        g.drawRect(x + 3, y + 3, w - 6, h - 6);
      }
      g.setColor(c);
    }
  }
  
  private void drawTabShape(Graphics g, Rectangle r, boolean top, boolean active) {
    if (top) {
      g.drawLine(r.x, r.y + r.height, r.x, r.y + 5);
      g.drawLine(r.x, r.y + 5, r.x + 5, r.y);
      g.drawLine(r.x + 5, r.y, r.x + r.width, r.y);
      g.drawLine(r.x + r.width, r.y, r.x + r.width, r.y + r.height);
      if (!active) {
        g.drawLine(r.x + r.width, r.y + r.height, r.x, r.y + r.height);
      }
    }
    else {
      g.drawLine(r.x, r.y, r.x, r.y + r.height - 5);
      g.drawLine(r.x, r.y + r.height - 5, r.x + 5, r.y + r.height);
      g.drawLine(r.x + 5, r.y + r.height, r.x + r.width, r.y + r.height);
      g.drawLine(r.x + r.width, r.y + r.height, r.x + r.width, r.y);
      if (!active) {
        g.drawLine(r.x + r.width, r.y, r.x, r.y);
      }
    }
  }

  public void paintTabbedPaneTabBackground(SynthContext context, Graphics g,
      int x, int y, int w, int h, int tabIndex) {
    context.getComponent().putClientProperty("Synthetica.tabbedPane.tabIndex", Integer.valueOf(tabIndex));
    int imgIndex = -1;
    boolean active = false;
    if ((context.getComponentState() & SynthConstants.SELECTED) != 0) {
      imgIndex = 0;
      active = true;
    }
    else if ((context.getComponentState() & SynthConstants.MOUSE_OVER) != 0) {
      imgIndex = 1;
    }
    else {
      imgIndex = 2;
    }
    GeneralPath path = new GeneralPath();
    Rectangle r = new Rectangle(x, y, w, h);
    if (((JTabbedPane) context.getComponent()).getTabPlacement() == JTabbedPane.TOP) {
      path.moveTo(r.x + 5, r.y);
      path.lineTo(r.x, r.y + 5);
      path.lineTo(r.x, r.y + r.height);
      path.lineTo(r.x + r.width, r.y + r.height);
      path.lineTo(r.x + r.width, r.y);
    }
    else {
      path.moveTo(r.x, r.y);
      path.lineTo(r.x, r.y + r.height - 5);
      path.lineTo(r.x + 5, r.y + r.height);
      path.lineTo(r.x + r.width, r.y + r.height);
      path.lineTo(r.x + r.width, r.y);
    }
    path.closePath();
    Shape clip = g.getClip();
    g.setClip(path);
    drawImage((Graphics2D)g, r.x, r.y, imgIndex, context.getComponent());
    g.setClip(clip);
    boolean top =  (((JTabbedPane) context.getComponent()).getTabPlacement() == JTabbedPane.TOP);
    drawTabShape(g, r, top, active);
    if ((context.getComponentState() & SynthConstants.FOCUSED) != 0) {
      Color c = g.getColor();
      g.setColor(Colors.getFocusRectColor());
      Rectangle r2 = new Rectangle(r.x + 2, r.y + 2, r.width - 4, r.height - 4);
      drawTabShape(g, r2, top, active);
      g.drawLine(r2.x, r2.y, r2.x + r2.width, r2.y);
      g.setColor(c);
    }
  }

  private void paintBackground(SynthContext context, Graphics g, int x, int y,
      int w, int h, int index) {
    int i, j;
    int width, height;
    width = ImageHolder.bufferedImages[index].getHeight(context.getComponent());
    height = ImageHolder.bufferedImages[index].getWidth(context.getComponent());
    Shape clip = g.getClip();
    Rectangle r = new Rectangle(x, y, w, h);
    Area newClip = new Area(clip);
    newClip.intersect(new Area(r));
    g.setClip(newClip);
    if (width > 0 && height > 0) {
      for (i = x; i < (x + w); i += width) {
        for (j = y; j < (y + h); j += height) {
          drawImage((Graphics2D)g, i, j, index, context.getComponent());
        }
      }
    }
    g.setClip(clip);
  }
}
