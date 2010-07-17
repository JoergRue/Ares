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

import javax.swing.UIManager;

public class Colors {

  public static boolean hasCustomColors() {
    init();
    return sForeground != null;
  }

  public static Color getSelectedForeground() {
    init();
    return sForeground;
  }

  public static Color getSelectedBackground() {
    init();
    return sBackground;
  }
  
  public static Color getFocusRectColor() {
    init();
    return sFocus;
  }

  private static Color sForeground = null;

  private static Color sBackground = null;
  
  private static Color sFocus = null;

  private static boolean bInit = false;

  public static void init() {
    if (!bInit) {
      bInit = true;
      Object o = UIManager.get("ares.controller.gui.lf.focusedTextColor"); //$NON-NLS-1$
      if (o != null) {
        sForeground = (Color) o;
        sBackground = (Color) UIManager
            .get("ares.controller.gui.lf.focusedBackgroundColor"); //$NON-NLS-1$
        sFocus = (Color) UIManager.getColor("ares.controller.gui.lf.focusRectColor"); //$NON-NLS-1$
      }
    }
  }
  
  private Colors() {}

}
