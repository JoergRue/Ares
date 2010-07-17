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

import java.awt.Rectangle;
import java.util.HashMap;
import java.util.Map;

import javax.swing.AbstractButton;

public abstract class FrameController extends SubFrame implements
    FrameStateChanger {
  
  protected FrameController(String name) {
    super(name);
    FrameManagement.getInstance().addObserver(this);
    this.setDefaultCloseOperation(javax.swing.JFrame.DISPOSE_ON_CLOSE);           
  }
  
  public void dispose() {
    FrameManagement.getInstance().removeObserver(this);
    super.dispose();
  }

  public void openFrame(String name, Rectangle bounds) {
    if (name.equals(getTitle())) {
      setBounds(bounds);
      return;
    }
    AbstractButton button = frameButtons.get(name);
    if (button != null) {
      SubFrame.saveFrameBounds(name, bounds);
      button.doClick();
    }
  }
  
  public void closeFrame(SubFrame frame) {
    String title = frame.getTitle();
    frame.dispose();
    if (frameButtons.containsKey(title)) {
      frameButtons.get(title).setSelected(false);
    }
  }
  
  public void frameStateChanged(SubFrame frame, boolean isOpen) {
    if (frameButtons.containsKey(frame.getTitle())) {
      frameButtons.get(frame.getTitle()).setSelected(isOpen);
    }
  }
  
  protected void addButton(String title, AbstractButton button) {
    frameButtons.put(title, button);
  }
  
  private Map<String, AbstractButton> frameButtons = new HashMap<String, AbstractButton>();

}
