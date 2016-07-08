/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.CompoundButton;
import android.widget.GridView;
import android.widget.ToggleButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.Spinner;
import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.TitledElement;
import ares.controllers.network.INetworkClient;

public class TagsFragment extends ModeLikeFragment implements INetworkClient {

    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    	View view = inflater.inflate(R.layout.tags, container, false);
    	return view;
    }

    public void onActivityCreated(Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        if (!isOnTablet()) {
        	registerGestures();
        }
    }
    
    public void preferencesChanged() {
    	onPrefsChanged();
    }
    
    private boolean inPreferences = false;
    
    protected boolean showMainActivityOnDisconnect() {
    	return !isOnTablet() && !inPreferences;
    }

	private void showPreferences() {
		if (Control.getInstance().isConnected())
		{
			SharedPreferences.Editor prefs = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext()).edit();
			switch (PlayingState.getInstance().getTagCategoryCombination())
			{
			case And:
				prefs.putString("tag_categories_op", "globalAnd");
				break;
			case CategoryAnd:
				prefs.putString("tag_categories_op", "and");
				break;
			case Or:
			default:
				prefs.putString("tag_categories_op", "or");
				break;
			}
			prefs.putString("tag_fading_time", "" + PlayingState.getInstance().getTagFadingTime());
			prefs.putBoolean("tag_fading_only_on_change", PlayingState.getInstance().getTagFadeOnlyOnChange());
			prefs.putBoolean("music_on_all_speakers", PlayingState.getInstance().getMusicOnAllSpeakers());
			switch (PlayingState.getInstance().getMusicFadingOption())
			{
			case 0:
				prefs.putString("music_fading_op", "noFading");
				break;
			case 1:
				prefs.putString("music_fading_op", "fading");
				break;
			case 2:
			default:
				prefs.putString("music_fading_op", "crossFading");
				break;
			}
			prefs.putString("music_fading_time", "" + PlayingState.getInstance().getMusicFadingTime());
			prefs.commit();
		}
		inPreferences = true;
		Intent intent = new Intent(getActivity().getBaseContext(), PrefsActivity.class);
		getActivity().startActivityForResult(intent, ControlFragment.REQUEST_PREFS);
	}

	public void onStart() {
    	super.onStart();
    	uninitializeViews();
    	initializeViews();
    	PlayingState.getInstance().addClient(this);
    	inPreferences = false;
    }
    
    public void onStop() {
    	PlayingState.getInstance().removeClient(this);
    	super.onStop();
    }

    public void onDestroyView() {
    	uninitializeViews();
    	super.onDestroyView();
    }
    
    private static final int REMOVE_ALL_TAGS = 8;
    private static final int PREFERENCES = 9;
    
    public void onCreateOptionsMenu(android.view.Menu menu, MenuInflater inflater) {
    	menu.add(Menu.NONE, REMOVE_ALL_TAGS, Menu.NONE, R.string.ClearTags);
    	if (!isOnTablet()) {
    		menu.add(Menu.NONE, PREFERENCES, Menu.NONE, R.string.preferences);
    	}
    	super.onCreateOptionsMenu(menu, inflater);
    }
    
    public boolean onOptionsItemSelected(android.view.MenuItem menuItem) {
    	switch (menuItem.getItemId())
    	{
    	case REMOVE_ALL_TAGS:
    		Control.getInstance().removeAllTags();
    		break;
    	case PREFERENCES:
    		showPreferences();
    	}
    	return super.onOptionsItemSelected(menuItem);
    }
    
    private void uninitializeViews() {
    	m_Listen = false;
    }
    
    private void initializeViews() {
        updateCategorySpinner();
        updateTagButtons();
        m_Listen = true;
    }

    private int m_LastCategoryId = -1;
    
    private void updateCategorySpinner() {
    	Spinner categorySpinner = (Spinner)getActivity().findViewById(R.id.categorySpinner);
    	categorySpinner.setOnItemSelectedListener(null);
    	List<TitledElement> categories = PlayingState.getInstance().getTagCategories();
    	String[] names = new String[categories.size()];
    	int selIndex = -1;
    	for (int i = 0; i < categories.size(); ++i) 
    	{
    		names[i] = categories.get(i).getTitle();
    		if (categories.get(i).getId() == m_LastCategoryId) {
    			selIndex = i;
    		}
    	}
    	if (selIndex == -1 && categories.size() > 0) {
    		selIndex = 0;
    		m_LastCategoryId = categories.get(0).getId();
    	}
    	ArrayAdapter<String> adapter = new ArrayAdapter<String>(getActivity(), android.R.layout.simple_spinner_item, names);
    	adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
    	categorySpinner.setAdapter(adapter);
    	if (selIndex != -1) {
    		categorySpinner.setSelection(selIndex, false);
    	}
    	categorySpinner.setOnItemSelectedListener(new OnItemSelectedListener() {
			@Override
			public void onItemSelected(AdapterView<?> arg0, View arg1,
					int position, long arg3) {
		    	List<TitledElement> categories = PlayingState.getInstance().getTagCategories();
				if (position >= 0 && position < categories.size()) {
					m_LastCategoryId = categories.get(position).getId();
					updateTagButtons();
				}
			}

			@Override
			public void onNothingSelected(AdapterView<?> arg0) {
				m_LastCategoryId = -1;
				updateTagButtons();
			}
    	});
    }
    
    private ButtonAdapter mAdapter;
    
    private void updateTagButtons() {
    	GridView buttonGrid = (GridView)getActivity().findViewById(R.id.tagButtonGrid);
    	mAdapter = new ButtonAdapter(getActivity(), m_LastCategoryId);
    	buttonGrid.setAdapter(mAdapter);
    }
    
    
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
	}

	@Override
	public void musicListChanged(List<TitledElement> newList) {
	}

	@Override
	public void tagsChanged(List<TitledElement> newCategories,
			Map<Integer, List<TitledElement>> newTagsPerCategory) {
		updateCategorySpinner();
		updateTagButtons();
	}

	@Override
	public void activeTagsChanged(List<Integer> newActiveTags) {
		if (mAdapter != null) {
			mAdapter.deactivateAllButtons();
			for (Integer tagId : newActiveTags) {
				mAdapter.changeButtonState(tagId, true);
			}
		}
	}

	@Override
	public void tagSwitched(int tagId, boolean isActive) {
		if (mAdapter != null) {
			mAdapter.changeButtonState(tagId, isActive);
		}
	}

	@Override
	public void tagCategoryCombinationChanged(INetworkClient.CategoryCombination combination) {
		// not handled directly
	}

	@Override
	public void disconnect() {
	}

	@Override
	public void connectionFailed() {
	}

	@Override
	public void musicRepeatChanged(boolean isRepeat) {
	}

	@Override
	public void projectLoaded() {
	}

	@Override
	public void importProgressChanged(int percent, String additionalInfo) {
	}
	
	private boolean m_Listen = true;

    private class CommandSender implements OnCheckedChangeListener {
		public void onCheckedChanged(CompoundButton button, boolean checked) {
			if (!m_Listen)
				return;
			Log.d("TagFragment", "Switching tag " + mId);
			Control.getInstance().switchTag(mCategoryId, mId, checked);
		}
		
		public CommandSender(int id, int categoryId) {
			mId = id;
			mCategoryId = categoryId;
		}

		private int mId;
		private int mCategoryId;
    }
    
    private class ButtonAdapter extends BaseAdapter {
    	
    	private Context mContext;
    	private List<TitledElement> mTags;
    	private int mCategoryId;
    	
    	public ButtonAdapter(Context c, int categoryId) {
    		mContext = c;
    		mCategoryId = categoryId;
    		Map<Integer, List<TitledElement>> tags = PlayingState.getInstance().getTags();
    		if (tags.containsKey(categoryId))
    			mTags = tags.get(categoryId);
    		else
    			mTags = new ArrayList<TitledElement>();
    	}
    	
		public int getCount() {
			return mTags.size();
		}

		public Object getItem(int position) {
			return null;
		}

		public long getItemId(int position) {
			return 0;
		}
		
		private HashMap<Integer, ToggleButton> mButtons = new HashMap<Integer, ToggleButton>(); 
		private HashMap<Integer, ToggleButton> mButtonsById = new HashMap<Integer, ToggleButton>();
		
		public void changeButtonState(int tagId, boolean active) {
			if (mButtonsById.containsKey(tagId)) {
				m_Listen = false;
				mButtonsById.get(tagId).setChecked(active);
				m_Listen = true;
			}
		}
		
		public void deactivateAllButtons() {
			m_Listen = false;
			for (ToggleButton button : mButtonsById.values()) {
				button.setChecked(false);
			}
			m_Listen = true;
		}

		public View getView(int position, View convertView, ViewGroup parent) {
			ToggleButton button;
			if (convertView == null) {
				if (mButtons.containsKey(position)) {
					return mButtons.get(position);
				}
				else {
					button = new ToggleButton(mContext);
					TitledElement tag = mTags.get(position);
					button.setPadding(5, 5, 5, 5);
					if (tag != null) {
						button.setText(tag.getTitle());
						button.setTextOn(tag.getTitle());
						button.setTextOff(tag.getTitle());
						if (PlayingState.getInstance().getActiveTags().contains(tag.getId())) {
							button.setChecked(true);
						}
						button.setOnCheckedChangeListener(new CommandSender(tag.getId(), mCategoryId));					
						mButtonsById.put(tag.getId(), button);
					}
					else {
						button.setText("<Error>");
					}
					mButtons.put(position, button);
					return button;
				}
			}
			else {
				return (ToggleButton)convertView;
			}
		}
    }

	@Override
	public void projectFilesRetrieved(List<String> files) {
	}

	@Override
	public void configurationChanged(Configuration newConfiguration,
			String fileName) {
		// tags change will be reported separately
	}

	@Override
	public void musicTagFadingChanged(int fadeTime, boolean fadeOnlyOnChange) {
		// not handled directly
	}

	@Override
	public void musicOnAllSpeakersChanged(boolean onAllSpeakers) {
	}
	
	@Override
	public void musicFadingChanged(int fadingOption, int fadingTime) {
	}
}
