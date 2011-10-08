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

import java.util.ArrayList;
import java.util.List;

import android.os.Handler;
import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.MusicElement;
import ares.controllers.network.INetworkClient;

public class PlayingState implements INetworkClient {

	private int overallVolume = 100;
	private int musicVolume = 100;
	private int soundVolume = 100;
	
	private List<MusicElement> musicList = null;
		
	private String mode = "";
	private String musicPlayed = "";
	private String shortMusicPlayed = "";
	
	private ArrayList<String> modeElements = new ArrayList<String>();
	
	private String playerProject = "";

	public int getOverallVolume() {
		return overallVolume;
	}

	public int getMusicVolume() {
		return musicVolume;
	}

	public int getSoundVolume() {
		return soundVolume;
	}

	public String getMode() {
		return mode;
	}

	public String getMusicPlayed() {
		return musicPlayed;
	}
	
	public String getShortMusicPlayed() {
		return shortMusicPlayed;
	}
	
	public String getPlayerProject() {
		return playerProject;
	}
	
	public List<MusicElement> getMusicList()
	{
		return musicList;
	}
	
	private static PlayingState sInstance = null;
	
	public static PlayingState getInstance() {
		if (sInstance == null) {
			sInstance = new PlayingState();
		}
		return sInstance;
	}
	
	private Handler handler = new Handler();
	
	private INetworkClient client = null;

	public INetworkClient getClient() {
		return client;
	}

	public void setClient(INetworkClient client) {
		this.client = client;
	}
	
	public void clearState() {
		musicPlayed = "";
		modeElements.clear();
	}

	public void modeChanged(String newMode) {
		final String m = newMode;
		handler.post(new Runnable(){
			public void run() {
				mode = m;
				if (getClient() == null)
					return;
				getClient().modeChanged(mode);
			}
		});
	}
	

	public String getElements() {
		String elements = "";
		for (int i = 0; i < modeElements.size(); ++i) {
			elements += modeElements.get(i);
			if (i != modeElements.size() - 1)
				elements += ", ";
		}
		return elements;
	}
	
	@Override
	public void modeElementStarted(int element) {
		final int el = element;
		handler.post(new Runnable(){
			public void run() {
				String title = null;
				Configuration config = Control.getInstance().getConfiguration();
				if (config != null) {
					CommandButtonMapping.getInstance().commandStateChanged(el, true);
					title = config.getCommandTitle(el);
				}
				if (title != null) {
					modeElements.add(title);
				}
				if (getClient() == null)
					return;
				getClient().modeElementStarted(el);
			}
		});
	}

	@Override
	public void modeElementStopped(int element) {
		final int el = element;
		handler.post(new Runnable(){
			public void run() {
				String title = null;
				Configuration config = Control.getInstance().getConfiguration();
				if (config != null) {
					CommandButtonMapping.getInstance().commandStateChanged(el, false);
					title = config.getCommandTitle(el);
				}
				if (title != null) {
					modeElements.remove(title);
				}
				if (getClient() == null)
					return;
				getClient().modeElementStopped(el);
			}
		});		
	}

	@Override
	public void allModeElementsStopped() {
		handler.post(new Runnable() {
			public void run() {
				Configuration config = Control.getInstance().getConfiguration();
				if (config != null) {
					CommandButtonMapping.getInstance().allCommandsInactive();
				}				
				modeElements.clear();
				if (getClient() != null) {
					getClient().allModeElementsStopped();
				}
			}
		});
	}

	@Override
	public void volumeChanged(int index, int value) {
		final int i = index;
		final int v = value;
		handler.post(new Runnable() {
			public void run() {
				switch (i) {
				case 2:
					overallVolume = v;
					break;
				case 1:
					musicVolume = v;
					break;
				case 0:
					soundVolume = v;
					break;
				default:
					break;
				}
				if (getClient() == null)
					return;
				getClient().volumeChanged(i, v);
			}
		});
	}

	@Override
	public void musicChanged(String newMusic, String shortTitle) {
		final String m = newMusic;
		final String s = shortTitle;
		handler.post(new Runnable() {
			public void run() {
				musicPlayed = m;
				shortMusicPlayed = s;
				if (getClient() == null)
					return;
				getClient().musicChanged(m, s);
			}
		});
	}

	@Override
	public void disconnect() {
		handler.post(new Runnable() {
			public void run() {
				clearState();
				Control.getInstance().disconnect(false);
				if (getClient() != null)
					getClient().disconnect();
			}
		});
	}

	@Override
	public void connectionFailed() {
		handler.post(new Runnable() {
			public void run() {
				clearState();
				Control.getInstance().disconnect(false);
				if (getClient() != null) 
					getClient().connectionFailed();
			}
		});
	}

	@Override
	public void projectChanged(String newTitle) {
		final String t = newTitle; 
		handler.post(new Runnable() {
			public void run() {
				playerProject = t;
				if (getClient() != null) {
					getClient().projectChanged(t);
				}
			}
		});
	}

	@Override
	public void musicListChanged(List<MusicElement> newList) {
		final List<MusicElement> l = newList;
		handler.post(new Runnable() {
			public void run() {
				musicList = l;
				if (getClient() != null) {
					getClient().musicListChanged(l);
				}
			}
		});
		
	}
}
