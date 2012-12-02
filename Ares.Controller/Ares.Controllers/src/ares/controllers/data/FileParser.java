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
package ares.controllers.data;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import ares.controllers.messages.Messages;
import ares.controllers.messages.Message.MessageType;
import ares.controllers.util.Localization;

public final class FileParser {

  public static Configuration parseFile(File file) {
	  FileInputStream stream = null;
	  try {
		  stream = new FileInputStream(file);
		  return parseStream(stream, file.getName());
	  }
	  catch (IOException e) {
        Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
        return null;
      }
	  finally {
		  if (stream != null) {
			  try {
				  stream.close();
			  }
			  catch (IOException e) {
			  }
		  }
	  }
  }
  
  public static Configuration parseBytes(byte[] contents, String name) {
	  ByteArrayInputStream stream = null;
	  try {
		  stream = new ByteArrayInputStream(contents);
		  return parseStream(stream, name);
	  }
	  finally {
		  if (stream != null) {
			  try {
				  stream.close();
			  }
			  catch (IOException e) {
			  }
		  }
	  }
  }
  
  private static Configuration parseStream(InputStream stream, String name) {
    Configuration config = new Configuration();
    
    try {
      DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
      DocumentBuilder db = dbf.newDocumentBuilder();
      Document dom = db.parse(stream);

      Element docEle = dom.getDocumentElement();
      
      String title = docEle.getAttribute("Title"); //$NON-NLS-1$
      if (title == null) {
        Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.TitleMissing")); //$NON-NLS-1$
        title = name;
      }      
      config.setTitle(title);
      
      NodeList nl1 = docEle.getElementsByTagName("Modes"); //$NON-NLS-1$
      if (nl1.getLength() == 0)
    	  return config;
      else if (nl1.getLength() > 1)
    	  Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.DuplicateModesElement")); //$NON-NLS-1$
      
      NodeList nl = ((Element)nl1.item(0)).getElementsByTagName("Mode"); //$NON-NLS-1$
      for(int i = 0 ; i < nl.getLength();i++) {
        Element el = (Element)nl.item(i);
        Mode mode = readMode(el);
        if (mode == null) continue;
        if (config.containsKeyStroke(mode.getKeyStroke())) {
          Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.DuplicateKey") + mode.getKeyStroke() + Localization.getString("FileParser.InConfiguration")); //$NON-NLS-1$ //$NON-NLS-2$
          Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.Mode") + mode.getTitle() + Localization.getString("FileParser.Ignored")); //$NON-NLS-1$ //$NON-NLS-2$
          continue;
        }
        config.addMode(mode);
      }
      return config;
    }
    catch(ParserConfigurationException pce) {
      Messages.addMessage(MessageType.Error, pce.getLocalizedMessage());
      return null;
    }
    catch(SAXException se) {
      Messages.addMessage(MessageType.Error, se.getLocalizedMessage());
      return null;
    }
    catch (IOException e) {
      Messages.addMessage(MessageType.Error, e.getLocalizedMessage());
      return null;
    }
  }
  
  private static Mode readMode(Element el) {
    String title = el.getAttribute("Title"); //$NON-NLS-1$
    if (title == null) {
      Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.TitleMissing")); //$NON-NLS-1$
      return null;
    }
    KeyStroke keyStroke = null;
    int keyNr = 0;
    try {
    	String attr = el.getAttribute("Key"); //$NON-NLS-1$
    	keyNr = attr != null ? Integer.parseInt(attr) : 0;
    	if (keyNr != 0)
    	{
    		keyStroke = KeyStroke.getKeyStroke(keyNr, 0);
    	}
    }
    catch (NumberFormatException e) {}
    /*
    if (keyStroke == null) {
      Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.KeyOfMode") + title + Localization.getString("FileParser.MissingModeIgnored")); //$NON-NLS-1$ //$NON-NLS-2$
      return null;
    }
    */
    
    Mode mode = new Mode(title, keyNr, keyStroke);
    
    NodeList nl1 = el.getElementsByTagName("Elements"); //$NON-NLS-1$
    if (nl1.getLength() == 0) {
    	Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.Mode") + title + Localization.getString("FileParser.NoCommands")); //$NON-NLS-1$ //$NON-NLS-2$
    	return mode;
    }
    else if (nl1.getLength() > 1){
    	Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.DuplicateElementsElement") + mode.getTitle()); //$NON-NLS-1$
    }
    
    NodeList nl = ((Element)nl1.item(0)).getElementsByTagName("ModeElement"); //$NON-NLS-1$
    
    if (nl.getLength() == 0) {
      Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.Mode") + title + Localization.getString("FileParser.NoCommands")); //$NON-NLS-1$ //$NON-NLS-2$
    }
    
    for(int i = 0 ; i < nl.getLength();i++) {
      Element ele = (Element)nl.item(i);
      Command command = readCommand(ele);
      if (command == null) continue;
      if (mode.containsKeyStroke(command.getKeyStroke())) {
        Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.DuplicateKey") + command.getKeyStroke() + Localization.getString("FileParser.InMode") + title + "."); //$NON-NLS-1$ //$NON-NLS-2$ //$NON-NLS-3$
        Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.Command") + command.getTitle() + Localization.getString("FileParser.Ignored")); //$NON-NLS-1$ //$NON-NLS-2$
        continue;
      }
      mode.addCommand(command);
    }
    
    return mode;
  }
  
  private static Command readCommand(Element el) {
    String title = el.getAttribute("Title"); //$NON-NLS-1$
    if (title == null) {
      Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.CommandTitleMissing")); //$NON-NLS-1$
      return null;
    }
    int id = 0;
    try {
    	id = Integer.parseInt(el.getAttribute("Id")); //$NON-NLS-1$
    }
    catch (NumberFormatException e) {
    	Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.MissingElementId") + title + Localization.getString("FileParser.ElementIgnored")); //$NON-NLS-1$ //$NON-NLS-2$
    	return null;
    }
    boolean visible = true;
    String visibleAttr = el.getAttribute("visibleInPlayer"); //$NON-NLS-1$
    if (!"".equals(visibleAttr) && !("true".equals(visibleAttr))) //$NON-NLS-1$ $NON-NLS-2$
    {
    	visible = false;
    }
    NodeList nl = el.getElementsByTagName("KeyTrigger"); //$NON-NLS-1$
    if (nl.getLength() == 0) {
    	// Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.NoKeyDefined") + title + Localization.getString("FileParser.ElementIgnored")); //$NON-NLS-1$ //$NON-NLS-2$
    	return new Command(title, id, null, visible);
    }
    else if (nl.getLength() > 1) {
    	Messages.addMessage(MessageType.Warning, Localization.getString("FileParser.DuplicateKeyTriggerElement") + title); //$NON-NLS-1$
    	return new Command(title, id, null, visible);
    }
    else {
	    Element trigger = (Element)nl.item(0);
	    KeyStroke keyStroke = null;
	    try {
	    	int keyNr = Integer.parseInt(trigger.getAttribute("Key")); //$NON-NLS-1$
	    	keyStroke = KeyStroke.getKeyStroke(keyNr, 0);
	    }
	    catch (NumberFormatException e) {}
	    /*
	    if (keyStroke == null) {
	      Messages.addMessage(MessageType.Error, Localization.getString("FileParser.KeyStrokeCommand") + title + Localization.getString("FileParser.MissingCommandIgnored")); //$NON-NLS-1$ //$NON-NLS-2$
	      return null;
	    }
	    */
	    return new Command(title, id, keyStroke, visible);
    }
    
  }
  
}
