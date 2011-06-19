/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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

import java.awt.event.KeyEvent;

import javax.swing.JComponent;

import ares.controller.control.KeyAction;

public class ComponentKeys {

	  private static void addKeyAction(JComponent component, ares.controllers.data.KeyStroke key) {
		  	javax.swing.KeyStroke swingStroke = javax.swing.KeyStroke.getKeyStroke(key.getKeyCode(), 0);
		    component.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(swingStroke, swingStroke.toString());
		    component.getActionMap().put(key.toString(), new KeyAction(key));
		  }
		  
	  public static void addAlwaysAvailableKeys(JComponent component) {
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_UP));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_DOWN));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_LEFT));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_RIGHT));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_PAGE_UP));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_PAGE_DOWN));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_INSERT));
		    addKeyAction(component, getKeyStroke(KeyEvent.VK_DELETE));    
		    ares.controllers.data.KeyStroke escapeStroke = getKeyStroke(KeyEvent.VK_ESCAPE);
		    javax.swing.KeyStroke swingEscape = javax.swing.KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0);
		    component.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(swingEscape, escapeStroke.toString());
		    component.getActionMap().put(escapeStroke.toString(), new KeyAction(escapeStroke));
		  }
		  
	  private static ares.controllers.data.KeyStroke getKeyStroke(int keyCode) {
		    return ares.controllers.data.KeyStroke.getKeyStroke(keyCode, 0);
		  }
		  
}
