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

import java.awt.Graphics;
import java.awt.Image;
import java.awt.Rectangle;

import javax.swing.JRootPane;

public class BGRootPane extends JRootPane {

  public BGRootPane() {
    super();
    setOpaque(true);
  }

  public BGRootPane(boolean forDialog) {
    super();
    setOpaque(true);
  }

  private static Image bgImage = createBGImage();
  
  private static Image createBGImage() {
    javax.swing.ImageIcon icon = new javax.swing.ImageIcon(
        BGRootPane.class
            .getResource("/de/javasoft/plaf/synthetica/walnut/images/backarea.png")); //$NON-NLS-1$
    return icon.getImage();    
  }

  private static Image getBGImage() {
    return bgImage;
  }

  private static Image gripper = createGripper();
  
  private static Image createGripper() {
    javax.swing.ImageIcon icon = new javax.swing.ImageIcon(
        BGRootPane.class
            .getResource("/de/javasoft/plaf/synthetica/walnut/images/resizer.png")); //$NON-NLS-1$
    return icon.getImage();
  }

  private static Image getGripper() {
    return gripper;
  }

  public void paintComponent(Graphics g) {
    int x, y;
    int width, height;
    Rectangle clip = g.getClipBounds();
    Image img = getBGImage();
    width = img.getHeight(this);
    height = img.getWidth(this);
    if (width > 0 && height > 0) {
      for (x = clip.x; x < (clip.x + clip.width); x += width) {
        for (y = clip.y; y < (clip.y + clip.height); y += height) {
          g.drawImage(img, x, y, this);
        }
      }
    }
    width = getGripper().getWidth(this);
    height = getGripper().getHeight(this);
    if (width > 0 && height > 0) {
      Rectangle bounds = this.getBounds();
      g.drawImage(getGripper(), bounds.width - width - 5, bounds.height
          - height - 5, this);
    }
  }

}
