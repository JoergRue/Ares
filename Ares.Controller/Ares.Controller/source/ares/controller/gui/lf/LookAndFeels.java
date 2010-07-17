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

import java.util.ArrayList;
import java.util.prefs.Preferences;

import javax.swing.UIManager;

import com.l2fprod.gui.plaf.skin.Skin;
import com.l2fprod.gui.plaf.skin.SkinLookAndFeel;

import de.javasoft.plaf.synthetica.SyntheticaLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaStandardLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaGreenDreamLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaWalnutLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaBlueIceLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaBlueSteelLookAndFeel;
import de.javasoft.plaf.synthetica.SyntheticaSilverMoonLookAndFeel;

public class LookAndFeels {
  
  private LookAndFeels() {}

  public static String[] getLookAndFeels() {
    ArrayList<String> names = new ArrayList<String>();
    names.add("Default (Walnut)"); //$NON-NLS-1$
    for (UIManager.LookAndFeelInfo info : UIManager.getInstalledLookAndFeels()) {
      names.add(info.getName());
    }
    names.add("Synthetica (Standard)"); //$NON-NLS-1$
    names.add("Synthetica (Green Dream)"); //$NON-NLS-1$
    names.add("Synthetica (Blue Ice)"); //$NON-NLS-1$
    names.add("Synthetica (Silver Moon)"); //$NON-NLS-1$
    names.add("Synthetica (Blue Steel)"); //$NON-NLS-1$
    names.add("Skin ..."); //$NON-NLS-1$
    String[] array = new String[names.size()];
    return names.toArray(array);
  }

  public static String getCurrentLookAndFeel() {
    Preferences prefs = Preferences.userNodeForPackage(LookAndFeels.class);
    return prefs.get("lastLookAndFeel", "Default (Walnut)"); //$NON-NLS-1$ //$NON-NLS-2$
  }

  public static String getLastThemePack() {
    Preferences prefs = Preferences.userNodeForPackage(LookAndFeels.class);
    return prefs.get("themePack", "themepack.zip"); //$NON-NLS-1$ //$NON-NLS-2$
  }

  public static void setLookAndFeel(String choice) {
    if (!choice.equals(getCurrentLookAndFeel())) {
      // internalSetLookAndFeel(choice);
      Preferences prefs = Preferences.userNodeForPackage(LookAndFeels.class);
      prefs.put("lastLookAndFeel", choice); //$NON-NLS-1$
    }
  }

  public static void setLookAndFeel(String choice, String themePack) {
    Preferences prefs = Preferences.userNodeForPackage(LookAndFeels.class);
    prefs.put("lastLookAndFeel", choice); //$NON-NLS-1$
    prefs.put("themePack", themePack); //$NON-NLS-1$
  }

  private static void internalSetLookAndFeel(String choice) throws Exception {
    Preferences prefs = Preferences.userNodeForPackage(ares.controller.gui.OptionsDialog.class);
    if (prefs.getBoolean("FontSet", false)) { //$NON-NLS-1$
      String fontName = prefs.get("FontName", "Dialog"); //$NON-NLS-1$ //$NON-NLS-2$
      int fontSize = prefs.getInt("FontSize", 12); //$NON-NLS-1$
      SyntheticaLookAndFeel.setFont(fontName, fontSize);
    }
    if (choice.equals("Synthetica (Standard)")) { //$NON-NLS-1$
      UIManager.setLookAndFeel(new SyntheticaStandardLookAndFeel());
      return;
    }
    else if (choice.equals("Synthetica (Green Dream)")) { //$NON-NLS-1$
      UIManager.setLookAndFeel(new SyntheticaGreenDreamLookAndFeel());
      return;
    }
    else if (choice.equals("Synthetica (Blue Ice)")) { //$NON-NLS-1$
      UIManager.setLookAndFeel(new SyntheticaBlueIceLookAndFeel());
      return;
    }
    else if (choice.equals("Synthetica (Blue Steel)")) { //$NON-NLS-1$
      UIManager.setLookAndFeel(new SyntheticaBlueSteelLookAndFeel());
      return;
    }
    else if (choice.equals("Synthetica (Silver Moon)")) { //$NON-NLS-1$
      UIManager.setLookAndFeel(new SyntheticaSilverMoonLookAndFeel());
      return;
    }
    else if (choice.equals("Skin ...")) { //$NON-NLS-1$
      Skin theSkinToUse = SkinLookAndFeel.loadThemePack(getLastThemePack());
      SkinLookAndFeel.setSkin(theSkinToUse);
      UIManager.setLookAndFeel(new SkinLookAndFeel());
      return;
    }
    else {
      for (UIManager.LookAndFeelInfo info : UIManager
          .getInstalledLookAndFeels()) {
        if (info.getName().equals(choice)) {
          UIManager.setLookAndFeel(info.getClassName());
          return;
        }
      }
    }
    UIManager.setLookAndFeel(new SyntheticaWalnutLookAndFeel());
  }

  public static void setLastLookAndFeel() throws Exception {
    try {
      internalSetLookAndFeel(getCurrentLookAndFeel());
      de.javasoft.plaf.synthetica.SyntheticaLookAndFeel.setExtendedFileChooserEnabled(false);
    }
    catch (Exception e) {
      e.printStackTrace();
      UIManager
          .setLookAndFeel(UIManager.getCrossPlatformLookAndFeelClassName());
    }
  }

}
