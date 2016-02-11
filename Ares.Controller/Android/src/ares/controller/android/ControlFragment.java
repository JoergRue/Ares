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
import java.util.List;
import java.util.Map;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.DialogInterface.OnDismissListener;
import android.gesture.Gesture;
import android.gesture.GestureLibraries;
import android.gesture.GestureLibrary;
import android.gesture.GestureOverlayView;
import android.gesture.Prediction;
import android.gesture.GestureOverlayView.OnGesturePerformedListener;
import android.graphics.Bitmap;
import android.graphics.Color;
import android.graphics.drawable.BitmapDrawable;
import android.net.Uri;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
import android.view.LayoutInflater;
import android.view.MenuInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.SeekBar;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.LinearLayout.LayoutParams;
import ares.controllers.control.Control;
import ares.controllers.data.Configuration;
import ares.controllers.data.TitledElement;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.ServerInfo;


public class ControlFragment extends ConnectedFragment implements INetworkClient {

	private class VolumeChanger implements SeekBar.OnSeekBarChangeListener {
		
		public void onProgressChanged(SeekBar seekBar, int progress,
				boolean fromUser) {
			if (!fromUser)
				return;
			Control.getInstance().setVolume(volumeIndex, progress);
		}
		public void onStartTrackingTouch(SeekBar seekBar) {
		}
		public void onStopTrackingTouch(SeekBar seekBar) {
		}
    	
		public VolumeChanger(int index) {
			volumeIndex = index;
		}
		
		private int volumeIndex;
	}
	
	private final String LAST_PROJECT = "LastProject";
	
	private ImageButton openButton;
	private ImageButton stopButton;
	private ImageButton forwardButton;
	private ImageButton backButton;
	private Button modesButton;
	private ImageButton repeatButton;
	
	private int VK_ESCAPE = 27;
	private int VK_LEFT = 37;
	private int VK_RIGHT = 39;
	
	private void registerGestures()	{
		final GestureLibrary gesturelib = GestureLibraries.fromRawResource(getActivity(), R.raw.gestures);
		gesturelib.load();
		LinearLayout layout = (LinearLayout) getActivity().findViewById(R.id.rootLayout);
		GestureOverlayView gestureOverlay = new GestureOverlayView(getActivity());
		gestureOverlay.setUncertainGestureColor(Color.TRANSPARENT);
		gestureOverlay.setGestureColor(Color.TRANSPARENT);
		layout.addView(gestureOverlay, new LinearLayout.LayoutParams(LayoutParams.FILL_PARENT, LayoutParams.FILL_PARENT, 1));
		ViewGroup mainViewGroup = (ViewGroup) getActivity().findViewById(R.id.mainLayout);
		layout.removeView(mainViewGroup);
		gestureOverlay.addView(mainViewGroup);
		gestureOverlay.addOnGesturePerformedListener(new OnGesturePerformedListener() {			
			@Override
			public void onGesturePerformed(GestureOverlayView overlay, Gesture gesture) {
				ArrayList<Prediction> predictions = gesturelib.recognize(gesture);
				for (Prediction prediction : predictions) {
					if (prediction.score > 1.0) {
						if (prediction.name.equals("point_up")) {
							showModes(false);
						}
						else if (prediction.name.equals("point_down")) {
							showModes(true);
						}
						else if (prediction.name.equals("swipe_left")) {
							showFirstMode();
						}
						else if (prediction.name.equals("swipe_right")) {
							showLastMode();
						}
						break;
					}
				}
			}
		});
	}
	
	Bitmap normalRepeatBmp, selectedRepeatBmp;
	
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setHasOptionsMenu(true);
	}
    
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
    	View view = inflater.inflate(R.layout.control, container);
    	return view;
    }
    
    protected boolean isControlFragment() {
    	return true;
    }
    
    public void onActivityCreated(Bundle savedInstanceState) {
    	super.onActivityCreated(savedInstanceState);
    	
    	if (!isOnTablet())
        {
    		registerGestures();
        }
        
        SeekBar bar1 = (SeekBar)getActivity().findViewById(R.id.overallVolumeBar);
        bar1.setOnSeekBarChangeListener(new VolumeChanger(2));
        SeekBar bar2 = (SeekBar)getActivity().findViewById(R.id.musicVolumeBar);
        bar2.setOnSeekBarChangeListener(new VolumeChanger(1));
        SeekBar bar3 = (SeekBar)getActivity().findViewById(R.id.soundsVolumeBar);
        bar3.setOnSeekBarChangeListener(new VolumeChanger(0));
        boolean connected = Control.getInstance().isConnected();
        if ((Control.getInstance().getConfiguration() == null) && getActivity().getPreferences(Activity.MODE_PRIVATE).contains(LAST_PROJECT)) {
        	this.fileToOpenDelayed = getActivity().getPreferences(Activity.MODE_PRIVATE).getString(LAST_PROJECT, ""); 
        }
        
        if (!isOnTablet()) {
	        modesButton = (Button)getActivity().findViewById(R.id.modesButton);
	        if (modesButton != null) {
		        modesButton.setEnabled(Control.getInstance().getConfiguration() != null);
		        modesButton.setOnClickListener(new OnClickListener() {
		        	public void onClick(View v) {
		        		showModesActivity(ControllerActivity.ANIM_MOVE_DOWN);
		        		getActivity().overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
		        	}
		        });
	        }
        }
        openButton = (ImageButton)getActivity().findViewById(R.id.openProjectButton);
        openButton.setOnClickListener(new OnClickListener() {
        	public void onClick(View v) {
        		openProject();
        	}
        });
        openButton.setEnabled(connected);
        stopButton = (ImageButton)getActivity().findViewById(R.id.stopButton);
        forwardButton = (ImageButton)getActivity().findViewById(R.id.forwardButton);
        backButton = (ImageButton)getActivity().findViewById(R.id.backButton);
        stopButton.setEnabled(connected);
        forwardButton.setEnabled(connected);
        backButton.setEnabled(connected);
        stopButton.setOnClickListener(new OnClickListener() {
			public void onClick(View v) {
				Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke(VK_ESCAPE, 0));
			}
        });
        forwardButton.setOnClickListener(new OnClickListener() {
        	public void onClick(View v) {
        		Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke(VK_RIGHT, 0));
        	}
        });
        backButton.setOnClickListener(new OnClickListener() {
        	public void onClick(View v) {
        		Control.getInstance().sendKey(ares.controllers.data.KeyStroke.getKeyStroke(VK_LEFT, 0));
        	}
        });
        repeatButton = (ImageButton)getActivity().findViewById(R.id.repeatButton);
        repeatButton.setEnabled(connected);
        repeatButton.setOnClickListener(new OnClickListener() {
        	public void onClick(View v) {
        		Control.getInstance().setMusicRepeat(!PlayingState.getInstance().isMusicRepeat());
        	}
        });
        normalRepeatBmp = ((BitmapDrawable)getResources().getDrawable(R.drawable.repeat)).getBitmap();
        selectedRepeatBmp = ((BitmapDrawable)getResources().getDrawable(R.drawable.repeatred)).getBitmap();
        
        if (isOnTablet()) {
        	FragmentManager fragmentManager = getFragmentManager();
        	if (fragmentManager.findFragmentById(R.id.modeFragmentContainer) == null) {
	        	ModeFragment fragment = new ModeFragment();
	        	fragment.setMode(0);
	        	FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
	        	fragmentTransaction.add(R.id.modeFragmentContainer, fragment);
	        	fragmentTransaction.commit();
        	}
        }
    }
    
    public void onResume() {
    	super.onResume();
    	
    	if (fileToOpenDelayed != null) {
    		String path = fileToOpenDelayed;
    		fileToOpenDelayed = null;
			openProject(path, getActivity());
    	}
    }
    
    public void onStart() {
    	super.onStart();
    	PlayingState.getInstance().addClient(this);
    	updateAll();
    }
    
    public void onStop() {
    	PlayingState.getInstance().removeClient(this);
    	super.onStop();
    }
    
    public void onDestroy() {
    	super.onDestroy();
    }
    
    public static final int CONNECT = 3;
    public static final int ABOUT = 4;
	public static final int REQUEST_OPEN = 5;
	public static final int REQUEST_PREFS = 6;
    
	public void onCreateOptionsMenu(android.view.Menu menu, MenuInflater inflater) {
		inflater.inflate(R.menu.controlmenu, menu);
    	super.onCreateOptionsMenu(menu, inflater);
    }
    
    public void onPrepareOptionsMenu(android.view.Menu menu) {
    	super.onPrepareOptionsMenu(menu);
    	if (Control.getInstance().isConnected()) {
    		menu.getItem(0).setVisible(false);
    		menu.getItem(0).setEnabled(false);
    		menu.getItem(1).setVisible(true);
    		menu.getItem(1).setEnabled(true);
			menu.getItem(2).setVisible(true);
			menu.getItem(2).setEnabled(Control.getInstance().getCurrentServer().hasWebServer());
    	}
    	else {
    		menu.getItem(0).setVisible(true);
    		menu.getItem(0).setEnabled(true);
    		menu.getItem(1).setVisible(false);
    		menu.getItem(1).setEnabled(false);
			menu.getItem(2).setVisible(false);
			menu.getItem(2).setEnabled(false);
    	}
    }
    
    public boolean onOptionsItemSelected(android.view.MenuItem menuItem) {
    	switch (menuItem.getItemId())
    	{
    	case R.id.connectmenuitem:
    		connectWithPlayer();
    		break;
    	case R.id.disconnectmenuitem:
    		doDisconnect(true, true);
    		break;
    	case R.id.aboutMenuItem:
    		getActivity().showDialog(ABOUT);
    		break;
    	case R.id.preferencesmenuitem:
    		showPreferences();
    		break;
		case R.id.webcontrollermenuitem:
			showWebController();
			break;
    	}
    	return super.onOptionsItemSelected(menuItem);
    }

	private void showWebController() {
		ServerInfo info = Control.getInstance().getCurrentServer();
		if (info != null) {
			String url = "http://" + info.getAddress() + ":" + info.getWebPort() + "/";
			try {
				Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse(url));
				startActivity(intent);
			}
			catch (Exception e) {
				Toast.makeText(this.getActivity(), "Error opening browser: " + e.getMessage(), Toast.LENGTH_LONG).show();
			}
		}
	}
    
    public void preferencesChanged() {
    	onPrefsChanged();
    }
    
    private String fileToOpenDelayed = null;
    
    private void projectOpened(Configuration config, String fileName) {
    	if (Control.getInstance().getFilePath().startsWith(fileName)) {
			getActivity().getPreferences(Activity.MODE_PRIVATE).edit().putString(LAST_PROJECT, Control.getInstance().getFilePath()).commit();
    	}
    	Control.getInstance().setConfiguration(config, fileName);
		updateProjectTitle();
		if (modesButton != null) {
			modesButton.setEnabled(Control.getInstance().getConfiguration() != null);
		}
		if (isOnTablet()) {
			Fragment fragment = getFragmentManager().findFragmentById(R.id.modeFragmentContainer);
			ModeLikeFragment modeFragment = (ModeLikeFragment)fragment;
			if (modeFragment != null) {
				modeFragment.projectLoaded();
			}
			fragment = getFragmentManager().findFragmentById(R.id.modesFragmentContainer);
			ModesFragment modesFragment = (ModesFragment)fragment;
			if (modesFragment != null) {
				modesFragment.projectLoaded();
			}
		}
    }
    
    public void openProject(String path, Activity activity) {
		if (Control.getInstance().isConnected()) {
			Control.getInstance().openFile(path);
		}
		else {
			fileToOpenDelayed = path;
		}
    }
    
    private void updateProjectTitle() {
		TextView projectView = (TextView)getActivity().findViewById(R.id.projectTextView);
		if (Control.getInstance().getConfiguration() != null) {
			String title = Control.getInstance().getConfiguration().getTitle();
			projectView.setText(title);
		}
		else {
			projectView.setText(R.string.connectFirst);
		}
    }
    
	private void openProject() {
		Control.getInstance().requestProjectFiles();
	}
	
	private void showModes(boolean moveUp) {
		if (Control.getInstance().getConfiguration() == null)
			return;
		if (!isOnTablet())
		{
			showModesActivity(moveUp ? ControllerActivity.ANIM_MOVE_UP : ControllerActivity.ANIM_MOVE_DOWN);
			if (moveUp) {
				getActivity().overridePendingTransition(R.anim.slide_from_top, R.anim.slide_to_bottom);
			}
			else {
				getActivity().overridePendingTransition(R.anim.slide_from_bottom, R.anim.slide_to_top);
			}
		}
	}
	
	private void showLastMode() {
		if (isOnTablet())
			return;
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(Control.getInstance().getConfiguration().getModes().size() - 1, ControllerActivity.ANIM_MOVE_LEFT);
		getActivity().overridePendingTransition(R.anim.slide_from_left, R.anim.slide_to_right);
	}
	
	private void showFirstMode() {
		if (isOnTablet())
			return;
		if (Control.getInstance().getConfiguration() == null)
			return;
		showMode(0, ControllerActivity.ANIM_MOVE_RIGHT);
		getActivity().overridePendingTransition(R.anim.slide_from_right, R.anim.slide_to_left);
	}
	
	private void showMode(int index, int animation) {
		if (isOnTablet())
			return;
		Intent intent = new Intent(getActivity().getBaseContext(), ModeActivity.class);
		intent.putExtra(ModeLikeActivity.MODE_INDEX, index);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, animation);
		startActivity(intent);		
	}

	private void showModesActivity(int animation) {
		if (isOnTablet())
			return;
		Intent intent = new Intent(getActivity().getBaseContext(), ModesActivity.class);
		intent.putExtra(ControllerActivity.ANIMATION_TYPE, animation);
		startActivity(intent);
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
		
		Intent intent = new Intent(getActivity().getBaseContext(), PrefsActivity.class);
		getActivity().startActivityForResult(intent, REQUEST_PREFS);
	}

	private void connectWithPlayer() {
    	String connectMode = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext()).getString("player_connection", "auto");
    	if (connectMode.equals("auto")) {
			if (servers.size() == 0) {
				Toast.makeText(getActivity(), R.string.no_player, Toast.LENGTH_SHORT).show();
			}
			else if (servers.size() == 1){
				doConnect(servers.get(serverNames.get(0)));
			}
			else {
				getActivity().showDialog(CONNECT);
			}
    	}
    	else {
    		tryConnect();
    	}
	}

	public Dialog showAboutDialog() {
		AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
		builder.setMessage(String.format(getString(R.string.aboutInfo), ares.controllers.control.Version.getCurrentVersionString()))
				.setCancelable(true)
				.setNeutralButton("OK", new DialogInterface.OnClickListener() {
					public void onClick(DialogInterface dialog, int which) {
						dialog.cancel();
					}
				});
		return builder.create();
	}
	
	public Dialog showPlayerSelection() {
		AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
		builder.setTitle(getString(R.string.select_player));
		builder.setCancelable(true);
		CharSequence[] items = new CharSequence[servers.size()];
		for (int i = 0; i < servers.size(); ++i) {
			items[i] = serverNames.get(i); 
		}
		builder.setItems(items, new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int which) {
				String name = serverNames.get(which);
				ServerInfo info = servers.get(name);
				doConnect(info);
			}
		});
		Dialog dialog = builder.create();
		dialog.setOnDismissListener(new OnDismissListener() {
			public void onDismiss(DialogInterface dialog) {
				getActivity().removeDialog(CONNECT);
			}
		});
		return dialog;
	}
	
	private void setText(int id, String text) {
		TextView textView = (TextView)getActivity().findViewById(id);
		textView.setText(text);
	}
	
	private void setVolume(int id, int percent) {
		SeekBar bar = (SeekBar)getActivity().findViewById(id);
		bar.setProgress(percent);
	}
	
	private void updateAll() {
		setVolume(R.id.overallVolumeBar, PlayingState.getInstance().getOverallVolume());
		setVolume(R.id.musicVolumeBar, PlayingState.getInstance().getMusicVolume());
		setVolume(R.id.soundsVolumeBar, PlayingState.getInstance().getSoundVolume());
		setText(R.id.modeTextView, PlayingState.getInstance().getMode());
		setText(R.id.elementsTextView, PlayingState.getInstance().getElements());		
		setText(R.id.musicTextView, PlayingState.getInstance().getMusicPlayed());
		if (Control.getInstance().isConnected()) {
			setText(R.id.networkTextView, String.format(getString(R.string.connected_with), Control.getInstance().getServerName()));
		}
		else {
			setText(R.id.networkTextView, getString(R.string.disconnected));
		}
		if (Control.getInstance().getConfiguration() != null) {
			setText(R.id.projectTextView, Control.getInstance().getConfiguration().getTitle());
		}
		if (repeatButton != null) {
			boolean repeat = PlayingState.getInstance().isMusicRepeat();
			repeatButton.setSelected(repeat);
			repeatButton.setImageBitmap(!repeat ? normalRepeatBmp : selectedRepeatBmp);
		}
		updateProjectTitle();
	}

	
	protected void onConnect(ServerInfo info) {
		super.onConnect(info);
		setText(R.id.networkTextView, String.format(getString(R.string.connected_with), info.getName()));
		stopButton.setEnabled(true);
		backButton.setEnabled(true);
		forwardButton.setEnabled(true);
		repeatButton.setEnabled(true);
		openButton.setEnabled(true);
		if (fileToOpenDelayed != null) {
			String path = fileToOpenDelayed;
			fileToOpenDelayed = null;
			openProject(path, getActivity());
		}
	}
	
	protected void onDisconnect(boolean startServerSearch) {
		super.onDisconnect(startServerSearch);
		setText(R.id.networkTextView, getString(R.string.disconnected));
		stopButton.setEnabled(false);
		backButton.setEnabled(false);
		forwardButton.setEnabled(false);
		repeatButton.setEnabled(false);
		openButton.setEnabled(false);
		updateAll();
	}

	@Override
	public void disconnect() {
		onDisconnect(true);
	}

	@Override
	public void connectionFailed() {
		onDisconnect(true);
	}
	
	@Override
	public void modeChanged(String newMode) {
		setText(R.id.modeTextView, newMode);		
	}

	@Override
	public void modeElementStarted(int element) {
		setText(R.id.elementsTextView, PlayingState.getInstance().getElements());		
	}

	@Override
	public void modeElementStopped(int element) {
		setText(R.id.elementsTextView, PlayingState.getInstance().getElements());				
	}

	@Override
	public void volumeChanged(int index, int value) {
		switch (index) {
		case 2:
			setVolume(R.id.overallVolumeBar, value);
			break;
		case 1:
			setVolume(R.id.musicVolumeBar, value);
			break;
		case 0:
			setVolume(R.id.soundsVolumeBar, value);
			break;
		default:
			break;
		}
	}

	@Override
	public void musicChanged(String newMusic, String shortTitle) {
		setText(R.id.musicTextView, newMusic);		
	}

	@Override
	public void allModeElementsStopped() {
		setText(R.id.elementsTextView, PlayingState.getInstance().getElements());
	}

	@Override
	public void musicListChanged(java.util.List<ares.controllers.data.TitledElement> newList) {
		// nothing here
	}
	
	@Override
	public void musicRepeatChanged(boolean repeat) {
		if (repeatButton != null) {
			repeatButton.setSelected(repeat);
			repeatButton.setImageBitmap(!repeat ? normalRepeatBmp : selectedRepeatBmp);
		}
	}

	@Override
	public void tagsChanged(List<TitledElement> newCategories,
			Map<Integer, List<TitledElement>> newTagsPerCategory) {
		// nothing here
	}

	@Override
	public void activeTagsChanged(List<Integer> newActiveTags) {
		// nothing here
	}

	@Override
	public void tagSwitched(int tagId, boolean isActive) {
		// nothing here
	}

	@Override
	public void tagCategoryCombinationChanged(INetworkClient.CategoryCombination combination) {
		// nothing here
	}

	@Override
	public void projectFilesRetrieved(List<String> files) {
		if (files.size() > 0) {
			Intent intent = new Intent(getActivity().getBaseContext(), FileDialog.class);
			ArrayList<String> list = new ArrayList<String>(files);
			intent.putStringArrayListExtra(FileDialog.POSSIBLE_FILES, list);
			getActivity().startActivityForResult(intent, REQUEST_OPEN);
		}
		else {
			Toast.makeText(getActivity(), R.string.NoProjects, Toast.LENGTH_LONG).show();
		}
			
	}

	@Override
	public void configurationChanged(Configuration newConfiguration,
			String fileName) {
		projectOpened(newConfiguration, fileName);
	}

	@Override
	public void musicTagFadingChanged(int fadeTime, boolean fadeOnlyOnChange) {
		// nothing here
	}

	@Override
	public void musicOnAllSpeakersChanged(boolean onAllSpeakers) {
		// nothing here
	}
	
	@Override
	public void musicFadingChanged(int fadingOption, int fadingTime) {
		// nothing here
	}
}
