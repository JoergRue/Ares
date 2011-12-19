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
package ares.controller.control;

import java.util.prefs.Preferences;

import javax.swing.SwingUtilities;
import javax.swing.UIManager;

import ares.controller.gui.MainFrame;
import ares.controller.gui.SubFrame;
import ares.controller.gui.lf.LookAndFeels;

/**
 * 
 */
public class Main {
  
  private Main() {}

  private static java.util.List<String> startFiles = null;
  
  public static void main(String[] args) {
	ares.controllers.util.UIThreadDispatcher.setDispatcher(new ares.controllers.util.IUIThreadDispatcher() {
		public void dispatchToUIThread(Runnable runnable) {
			SwingUtilities.invokeLater(runnable);
		}
	});
    SubFrame.setSaveLocations(false);
    if (args.length > 0) {
      startFiles = java.util.Arrays.asList(args);
    }
    else {
      String lastFile = Preferences.userNodeForPackage(MainFrame.class).get("LastConfiguration", ""); //$NON-NLS-1$ //$NON-NLS-2$
      if (lastFile != null && lastFile.length() != 0) {
        startFiles = new java.util.ArrayList<String>();
        startFiles.add(lastFile);
      }
    }

    SwingUtilities.invokeLater(new Runnable() {
      public void run() {
        createAndShowGUI();
      }
    });
  }
  
  public static void exit(int retCode)
  {
    System.exit(retCode);
  }

  private static void createAndShowGUI() {
    try {
      // Synthetica licence
	  String[] li = {"Licensee=Jörg Rüdenauer", "LicenseRegistrationNumber=NCJR110913", "Product=Synthetica", "LicenseType=Non Commercial", "ExpireDate=--.--.----", "MaxVersion=2.999.999"};
	  UIManager.put("Synthetica.license.info", li);
	  UIManager.put("Synthetica.license.key", "6B3713D8-767BE153-5CA73FFD-BB277655-EDA39A83");
      
	  LookAndFeels.setLastLookAndFeel();
      MainFrame mainFrame = new MainFrame();
      mainFrame.setVisible(true);
      if (startFiles != null) {
        mainFrame.openFile(startFiles.get(0));
      }
    }
    catch (Exception e) {
      e.printStackTrace();
    }
  }
}
