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
package ares.controller.android;

import android.content.Context;
import android.widget.Toast;
import ares.controllers.messages.IMessageListener;
import ares.controllers.messages.Message;
import ares.controllers.messages.Messages;

public class MessageHandler implements IMessageListener {

	@Override
	public void messageAdded(Message message) {
		if (mFilterLevel.ordinal() >= message.getType().ordinal()) {
			Toast.makeText(mContext, message.getMessage(), 
					message.getType() == Message.MessageType.Error ? Toast.LENGTH_LONG : Toast.LENGTH_SHORT)
					.show();
		}
	}
	
	public void register() {
		Messages.getInstance().addObserver(this);
	}
	
	public void unregister() {
		Messages.getInstance().removeObserver(this);
	}
	
	public void setFilter(String filter) {
		if (filter.equals("Error")) {
			mFilterLevel = Message.MessageType.Error;
		}
		else if (filter.equals("Warning")) {
			mFilterLevel = Message.MessageType.Warning;
		}
		else if (filter.equals("Info")) {
			mFilterLevel = Message.MessageType.Info;
		}
		else if (filter.equals("Debug")) {
			mFilterLevel = Message.MessageType.Debug;
		}
	}
	
	public void setContext(Context context) {
		mContext = context;
	}
	
	private Context mContext;
	private Message.MessageType mFilterLevel = Message.MessageType.Error;
	
	private static MessageHandler sInstance = null;
	
	public static MessageHandler getInstance() {
		if (sInstance == null) {
			sInstance = new MessageHandler();
		}
		return sInstance;
	}

}
