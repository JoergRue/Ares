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

import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.Mode;

public final class ModeFrames implements FrameStateChanger {

  private static ModeFrames sInstance = null;
  
  public static ModeFrames getInstance() {
    if (sInstance == null) {
      sInstance = new ModeFrames();
    }
    return sInstance;
  }
  
  private ModeFrames() {
    FrameManagement.getInstance().addObserver(this);
    frames = new HashMap<String, SubFrame>();
  }
  
  private Map<String, SubFrame> frames;

  public void closeFrame(SubFrame frame) {
    String title = frame.getTitle();
    closeFrame(title);
  }
  
  public void closeFrame(String title) {
    if (frames.containsKey(title)) {
      frames.get(title).dispose();
      frames.remove(title);
    }    
  }

  public void frameStateChanged(SubFrame frame, boolean isOpen) {
    String title = frame.getTitle();
    if (frames.containsKey(title) && !isOpen) {
      frames.remove(title);
    }
    else if (!frames.containsKey(title) && isOpen) {
      Configuration config = Control.getInstance().getConfiguration();
      if (config == null) return;
      for (Mode mode : config.getModes()) {
        if (mode.getTitle().equals(title)) {
          frames.put(title, frame);
        }
      }
    }
  }
  
  private SubFrame doOpenFrame(String name) {
    Configuration config = Control.getInstance().getConfiguration();
    if (config == null) return null;
    for (Mode mode : config.getModes()) {
      if (mode.getTitle().equals(name)) {
        if (frames.containsKey(name)) return null;
        ModeFrame frame = new ModeFrame(mode);
        frame.setVisible(true);
        frames.put(name, frame);
        return frame;
      }
    }
    return null;
  }
  
  public void openFrame(String name) {
    doOpenFrame(name);
  }

  public void openFrame(String name, Rectangle bounds) {
    SubFrame frame = doOpenFrame(name);
    if (frame != null) {
      frame.setBounds(bounds);      
    }
  }
  
  public boolean isFrameOpen(String name) {
    return frames.containsKey(name);
  }
}
