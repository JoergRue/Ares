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
import java.util.List;

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
  public static JPanel createPanel(List<Command> commands, IActionCreator actionCreator, JRootPane rootPane, int nrOfColumns) {
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
      for (Command command : commands) {
        AbstractButton button = new JToggleButton();
        AbstractAction action = actionCreator.createAction(command, button); 
        button.setAction(action);
        button.setText(command.getTitle());
        buttonPane.add(button);
        rootPane.getActionMap().put(command.getTitle(), new KeyCommandAction(button));
        javax.swing.KeyStroke swingStroke = javax.swing.KeyStroke.getKeyStroke(command.getKeyStroke().getKeyCode(), 0);
        rootPane.getInputMap(JComponent.WHEN_ANCESTOR_OF_FOCUSED_COMPONENT).put(swingStroke, command.getTitle());
      }
      panel.add(buttonPane, BorderLayout.CENTER);
      return panel;
  }
  
}
