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

import java.util.List;
import java.util.Map;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.ViewSwitcher;

import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.TitledElement;
import ares.controllers.network.INetworkClient;

public class MusicListFragment extends ModeLikeFragment implements INetworkClient {

    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    	View view = inflater.inflate(R.layout.music_list, container, false);
    	return view;
    }

    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        if (!isOnTablet()) {
        	registerGestures();
        }
        
        mListShown = true;
        mElements = PlayingState.getInstance().getMusicList();
        
		ListView listView = (ListView)getActivity().findViewById(R.id.musicList);
		listView.setOnItemClickListener(new OnItemClickListener() {
			public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
				if (position < 0 || position >= mElements.size())
					return;
				Control.getInstance().setMusicTitle(mElements.get(position).getId());
			}
		});
		listView.setTextFilterEnabled(true);
	}
    
    public void onResume() {
    	super.onResume();
    	refreshView();
    }
    
    public void onPause() {
    	mViewSwitcher.reset();
    	super.onPause();
    }
    
    public void projectLoaded() {
    	
    }
    
    private void refreshView() {
        mViewSwitcher = (ViewSwitcher)getActivity().findViewById(R.id.musicListSwitcher);

        if ((mElements == null || mElements.size() == 0) && mListShown) {
        	mViewSwitcher.showNext();
        	mListShown = false;
        }
        else if (mElements != null && mElements.size() > 0 && !mListShown){
        	mViewSwitcher.showPrevious();
        	mListShown = true;
        	updateList();
        }    	
        else if (mElements != null && mElements.size() > 0) {
        	updateList();
        }
    }
	
    public void onStart() {
    	super.onStart();
    	PlayingState.getInstance().addClient(this);
    }
    
    public void onStop() {
    	PlayingState.getInstance().removeClient(this);
    	super.onStop();
    }

	private void updateList() {
		if (mElements != null && mElements.size() > 0) {
			int curPos = -1;
			String curTitle = PlayingState.getInstance().getShortMusicPlayed();
			String[] entries = new String[mElements.size()];
			for (int i = 0; i < mElements.size(); ++i) {
				entries[i] = mElements.get(i).getTitle();
				if (entries[i].equals(curTitle)) {
					curPos = i;
				}
			}
			ListView listView = (ListView)getActivity().findViewById(R.id.musicList);
			listView.setAdapter(new ArrayAdapter<String>(getActivity(), R.layout.music_list_item, entries));
			if (curPos != -1)
			{
				listView.setSelection(curPos);
			}
		}
	}
	
	private ViewSwitcher mViewSwitcher;
	private List<TitledElement> mElements;
	private boolean mListShown;

	@Override
	public void modeChanged(String newMode) {
	}

	@Override
	public void modeElementStarted(int element) {
	}

	@Override
	public void modeElementStopped(int element) {
	}

	@Override
	public void allModeElementsStopped() {
	}

	@Override
	public void volumeChanged(int index, int value) {
	}

	@Override
	public void musicChanged(String newMusic, String shortTitle) {
		if (!mListShown)
			return;
		for (int i = 0; i < mElements.size(); ++i) {
			if (mElements.get(i).getTitle().equals(shortTitle)) {
				ListView listView = (ListView)getActivity().findViewById(R.id.musicList);
				listView.setSelection(i);
				break;
			}
		}
	}

	@Override
	public void musicListChanged(List<TitledElement> newList) {
		mElements = newList;
		updateList();
		if (mElements == null || mElements.size() == 0) {
			if (mListShown) {
				mViewSwitcher.showNext();
				mListShown = false;
			}
		}
		else {
			if (!mListShown) {
				mViewSwitcher.showPrevious();
				mListShown = true;
			}
		}
	}
	
	@Override
	public void musicRepeatChanged(boolean repeat) {
	}

	@Override
	public void disconnect() {
		mElements = null;
		updateList();
		if (mListShown) {
			mViewSwitcher.showNext();
			mListShown = false;
		}
	}

	@Override
	public void connectionFailed() {
	}

	@Override
	public void tagsChanged(List<TitledElement> newCategories,
			Map<Integer, List<TitledElement>> newTagsPerCategory) {
	}

	@Override
	public void activeTagsChanged(List<Integer> newActiveTags) {
	}

	@Override
	public void tagSwitched(int tagId, boolean isActive) {
	}

	@Override
	public void tagCategoryOperatorChanged(boolean operatorIsAnd) {
	}

	@Override
	public void projectFilesRetrieved(List<String> files) {
	}

	@Override
	public void configurationChanged(Configuration newConfiguration,
			String fileName) {
		// music list change is reported separately
	}

	@Override
	public void musicTagFadingChanged(int fadeTime, boolean fadeOnlyOnChange) {
	}

	@Override
	public void musicOnAllSpeakersChanged(boolean onAllSpeakers) {
	}
}
