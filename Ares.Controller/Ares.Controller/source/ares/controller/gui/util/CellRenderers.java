/*
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
package ares.controller.gui.util;

import java.awt.Color;
import java.awt.Component;

import javax.swing.ImageIcon;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JTable;
import javax.swing.table.DefaultTableCellRenderer;
import javax.swing.table.TableCellRenderer;

public final class CellRenderers {
  
  private CellRenderers() {}
  
  public interface ColourSelector {
    boolean shallBeOpaque(int column);
    boolean shallBeGray(int row, int column);
    Color getForeground(int row, int column);
  }
  
  private static final class GreyingCellRenderer extends DefaultTableCellRenderer {
    
    private ColourSelector mCallbacks;
    
    private static final Color BACKGROUND_GRAY = new Color(238, 238, 238);
    
    public GreyingCellRenderer(ColourSelector selector) {
      mCallbacks = selector;
    }
    
    public Component getTableCellRendererComponent(JTable table, Object value,
        boolean isSelected, boolean hasFocus, int row, int column) {
      Component comp = super.getTableCellRendererComponent(table, value,
          isSelected, hasFocus, row, column);
      comp.setBackground(mCallbacks.shallBeGray(row, column) ? BACKGROUND_GRAY
          : Color.WHITE);
      comp.setForeground(mCallbacks.getForeground(row, column));
      ((JComponent) comp).setOpaque(isSelected || mCallbacks.shallBeOpaque(column));
      return comp;
    }
  }

  private static final class NormalCellRenderer extends DefaultTableCellRenderer {
    public Component getTableCellRendererComponent(JTable table, Object value,
        boolean isSelected, boolean hasFocus, int row, int column) {
      Component comp = super.getTableCellRendererComponent(table, value,
          isSelected, hasFocus, row, column);
      ((JComponent) comp).setOpaque(isSelected);
      return comp;
    }
  }
  
  private static final class ImageCellRenderer extends DefaultTableCellRenderer {
    public Component getTableCellRendererComponent(JTable table, Object value, 
        boolean isSelected, boolean hasFocus, int row, int column) {
      Component comp = super.getTableCellRendererComponent(table, value,
          isSelected, hasFocus, row, column);
      ((JComponent) comp).setOpaque(isSelected);
      if (comp instanceof JLabel) {
        if (value instanceof ImageIcon) {
          ((JLabel)comp).setText(""); //$NON-NLS-1$
          ((JLabel)comp).setIcon((ImageIcon)value);
        }
        else if (value instanceof ImageAndText) {
          ((JLabel)comp).setText(""); //$NON-NLS-1$
          ((JLabel)comp).setIcon(((ImageAndText)value).getImage());
          ((JLabel)comp).setToolTipText(((ImageAndText)value).getText());
        }
      }
      return comp;
    }
  }
  
  public static class ImageAndText {
    private String text;
    private ImageIcon image;
    public ImageAndText(String text, ImageIcon image) {
      this.text = text;
      this.image = image;
    }
    public ImageIcon getImage() { return image; }
    public String getText() { return text; }
  }

  public static DefaultTableCellRenderer createGreyingCellRenderer(ColourSelector selector) {
    return new GreyingCellRenderer(selector);
  }
  
  public static TableCellRenderer createNormalCellRenderer() {
    return new NormalCellRenderer();
  }
  
  public static TableCellRenderer createImageCellRenderer() {
    return new ImageCellRenderer();
  }

}
