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
package ares.controller.gui;

import java.awt.BorderLayout;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.KeyEvent;
import java.util.List;
import java.util.prefs.Preferences;

import javax.swing.AbstractAction;
import javax.swing.AbstractButton;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRootPane;
import javax.swing.JToggleButton;

import ares.controllers.data.Command;

class CommandsPanelCreator {
  
  private CommandsPanelCreator() {
  }
  
  private static class KeyCommandAction extends AbstractAction {
    
    private AbstractButton button;
    
    public KeyCommandAction(AbstractButton button) {
      this.button = button;
    }

    public void actionPerformed(ActionEvent arg0) {
      button.doClick();
    }
    
  }
  
  public interface IActionCreator
  {
	  AbstractAction createAction(Command command, AbstractButton button);  
  }
  
  /**
   * This method initializes jContentPane 
   *  
   * @return javax.swing.JPanel 
   */
  public static JPanel createPanel(List<Command> commands, IActionCreator actionCreator, JRootPane rootPane, int nrOfColumns, JToggleButton musicListButton) {
      JPanel panel = new JPanel();
      panel.setLayout(new BorderLayout());
      JLabel label1 = new JLabel();
      label1.setPreferredSize(new java.awt.Dimension(5, 5));
      panel.add(label1, BorderLayout.NORTH);
      JLabel label2 = new JLabel();
      label2.setPreferredSize(new java.awt.Dimension(5, 5));
      panel.add(label2, BorderLayout.EAST);
      JLabel label3 = new JLabel();
      label3.setPreferredSize(new java.awt.Dimension(5, 5));
      panel.add(label3, BorderLayout.SOUTH);
      JLabel label4 = new JLabel();
      label4.setPreferredSize(new java.awt.Dimension(5, 5));
      panel.add(label4, BorderLayout.WEST);
      JPanel buttonPane = new JPanel();
      buttonPane.setLayout(new GridLayout((int)Math.ceil(commands.size() / (float)nrOfColumns), nrOfColumns, 10, 10));
      Preferences prefs = Preferences.userNodeForPackage(OptionsDialog.class);
      boolean showKeys = prefs.getBoolean("ShowKeys", false); //$NON-NLS-1$
      if (musicListButton != null) {
    	  buttonPane.add(musicListButton);
      }
      for (Command command : commands) {
        AbstractButton button = new JToggleButton();
        AbstractAction action = actionCreator.createAction(command, button); 
        button.setAction(action);
        buttonPane.add(button);
        String title = command.getTitle();
        if (command.getKeyStroke() != null) {
        	javax.swing.KeyStroke swingStroke = javax.swing.KeyStroke.getKeyStroke(command.getKeyStroke().getKeyCode(), 0);
        	rootPane.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(swingStroke, command.getTitle());
        	if (showKeys) {
        		title += " (" + getKeyText(swingStroke.getKeyCode()) + ")";
        	}
        }
        button.setText(title);
        rootPane.getActionMap().put(title, new KeyCommandAction(button));
      }
      panel.add(buttonPane, BorderLayout.CENTER);
      return panel;
  }
  
  private static String getKeyText(int keyCode) {
      if (keyCode >= KeyEvent.VK_0 && keyCode <= KeyEvent.VK_9 ||
          keyCode >= KeyEvent.VK_A && keyCode <= KeyEvent.VK_Z) {
          return String.valueOf((char)keyCode);
      }

      switch(keyCode) {
        case KeyEvent.VK_COMMA: return ",";
        case KeyEvent.VK_PERIOD: return ".";
        case KeyEvent.VK_SLASH: return "/";
        case KeyEvent.VK_SEMICOLON: return ";";
        case KeyEvent.VK_EQUALS: return "=";
        case KeyEvent.VK_OPEN_BRACKET: return "(";
        case KeyEvent.VK_BACK_SLASH: return "\\";
        case KeyEvent.VK_CLOSE_BRACKET: return ")";

        case KeyEvent.VK_ENTER: return "Enter";
        case KeyEvent.VK_BACK_SPACE: return "Backspace";
        case KeyEvent.VK_TAB: return "Tab";
        case KeyEvent.VK_PAUSE: return "Pause";
        case KeyEvent.VK_SPACE: return "Space";

        // numpad numeric keys handled below
        case KeyEvent.VK_MULTIPLY: return "*";
        case KeyEvent.VK_ADD: return "+";
        case KeyEvent.VK_SUBTRACT: return "-";
        case KeyEvent.VK_DIVIDE: return "/";
        case KeyEvent.VK_DELETE: return "Del";

        case KeyEvent.VK_F1: return "F1";
        case KeyEvent.VK_F2: return "F2";
        case KeyEvent.VK_F3: return "F3";
        case KeyEvent.VK_F4: return "F4";
        case KeyEvent.VK_F5: return "F5";
        case KeyEvent.VK_F6: return "F6";
        case KeyEvent.VK_F7: return "F7";
        case KeyEvent.VK_F8: return "F8";
        case KeyEvent.VK_F9: return "F9";
        case KeyEvent.VK_F10: return "F10";
        case KeyEvent.VK_F11: return "F11";
        case KeyEvent.VK_F12: return "F12";
        case KeyEvent.VK_F13: return "F13";
        case KeyEvent.VK_F14: return "F14";
        case KeyEvent.VK_F15: return "F15";
        case KeyEvent.VK_F16: return "F16";
        case KeyEvent.VK_F17: return "F17";
        case KeyEvent.VK_F18: return "F18";
        case KeyEvent.VK_F19: return "F19";
        case KeyEvent.VK_F20: return "F20";
        case KeyEvent.VK_F21: return "F21";
        case KeyEvent.VK_F22: return "F22";
        case KeyEvent.VK_F23: return "F23";
        case KeyEvent.VK_F24: return "F24";

        case KeyEvent.VK_BACK_QUOTE: return "`";
        case KeyEvent.VK_QUOTE: return "'";

        case KeyEvent.VK_AMPERSAND: return "&";
        case KeyEvent.VK_QUOTEDBL: return "\"";
        case KeyEvent.VK_LESS: return "<";
        case KeyEvent.VK_GREATER: return ">";
        case KeyEvent.VK_BRACELEFT: return "[";
        case KeyEvent.VK_BRACERIGHT: return "]";
        case KeyEvent.VK_AT: return "@";
        case KeyEvent.VK_COLON: return ":";
        case KeyEvent.VK_CIRCUMFLEX: return "^";
        case KeyEvent.VK_DOLLAR: return "$";
        case KeyEvent.VK_EURO_SIGN: return "€"; 
      }

      if (keyCode >= KeyEvent.VK_NUMPAD0 && keyCode <= KeyEvent.VK_NUMPAD9) {
          char c = (char)(keyCode - KeyEvent.VK_NUMPAD0 + '0');
          return "" + c;
      }

      return "0x" + Integer.toString(keyCode, 16);
  }
  
}
