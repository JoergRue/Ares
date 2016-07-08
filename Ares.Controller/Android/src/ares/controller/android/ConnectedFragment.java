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

import java.lang.reflect.Field;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.os.Handler;
import android.preference.PreferenceManager;
import android.support.v4.app.Fragment;
import android.util.Log;
import android.widget.Toast;
import ares.controllers.control.Control;
import ares.controllers.network.INetworkClient;
import ares.controllers.network.IServerListener;
import ares.controllers.network.ServerInfo;
import ares.controllers.network.ServerSearch;

public abstract class ConnectedFragment extends Fragment implements IServerListener {

	private ServerSearch serverSearch = null;
	
	private static Boolean sOnTablet = null;
	
	protected boolean isOnTablet() 
	{
		if (sOnTablet == null) {
		     Configuration conf = getResources().getConfiguration();
		     int screenLayout = 1; // application default behavior
		     try {
		         Field field = conf.getClass().getDeclaredField("screenLayout");
		         screenLayout = field.getInt(conf);
			     // Configuration.SCREENLAYOUT_SIZE_MASK == 15
			     int screenType = screenLayout & 15;
			     // Configuration.SCREENLAYOUT_SIZE_XLARGE == 4
			     // Configuration.SCREENLAYOUT_SIZE_LARGE == 3
		         sOnTablet = (screenType == 4) || (screenType == 3); 
		     } 
		     catch (Exception e) {
		         // NoSuchFieldException or related stuff
		    	 sOnTablet = false;
		     }
		}
		return sOnTablet;
	}
	
	protected boolean isControlFragment() {
		return false;
	}

	public void onStart() {
		super.onStart();

		if (mScheduler == null) {
			mScheduler = Executors.newSingleThreadScheduledExecutor();
		}

		if (serverSearch != null) {
			serverSearch.dispose();
		}
		serverSearch = new ServerSearch(this, getServerSearchPort());
        
		ConnectionManager.getInstance().addClient();
		
		if (Control.getInstance().getConfiguration() == null && !isOnTablet() && !isControlFragment()) {
			// no configuration loaded, not in control fragment, not in main activity
        	// switch to main activity so that control fragment is displayed
        	// and project can be opened
			Log.d("ConnectedFragment", "Switch to main activity because no project opened");
        	Intent intent = new Intent(getActivity().getBaseContext(), MainActivity.class);
        	startActivity(intent);    	
        	return;
		}
		
		boolean connected = Control.getInstance().isConnected();
        if (connected) {
        	// everything ok
        	// Log.d("ConnectedFragment", "Already connected");
        	connectWithFirstServer = false;
        }
        else if (isControlFragment() || !isOnTablet()){
        	// not connected, in own activity --> search for servers
        	servers.clear();
        	serverNames.clear();
        	connectWithFirstServer = true;
        	tryConnect();
        }
	}
	
	public void onStop() {
    	if (!Control.getInstance().isConnected()) {
    		Log.d("ConnectedFragment", "Stopping server search");
    		serverSearch.stopSearch();
    	}
		
		if (ConnectionManager.getInstance().removeClient() == 0) {
	    	if (Control.getInstance().isConnected()) {
				doDisconnect(false, true);
	    	}
		}

		if (mScheduler != null)
		{
			if (mFuture != null)
			{
				mFuture.cancel(false);
			}
			mScheduler.shutdown();
			mScheduler = null;
		}

		super.onStop();
	}
	
	private boolean setPlayerPreferencesOnConnect = false;
	
	protected void onPrefsChanged() {
		if (!Control.getInstance().isConnected())
		{
    		Log.d("ConnectedFragment", "Stopping server search");
			serverSearch.stopSearch();
		}
		servers.clear();
		serverNames.clear();
		if (serverSearch != null) {
			serverSearch.dispose();
		}
		serverSearch = new ServerSearch(this, getServerSearchPort());
		if (!Control.getInstance().isConnected()) 
		{
			if (isControlFragment() || !isOnTablet()) {
				tryConnect();
			}
		}    			
		if (Control.getInstance().isConnected()) {
			setPlayerPreferences();
		}
		else {
			setPlayerPreferencesOnConnect = true;
		}
	}
	
	private void setPlayerPreferences() {
		SharedPreferences prefs = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext());
		boolean onlyOnChange = prefs.getBoolean("tag_fading_only_on_change", false);
		String fadeTimeString = prefs.getString("tag_fading_time", "0");
		boolean musicOnAllSpeakers = prefs.getBoolean("music_on_all_speakers", false);
		int fadeTime = 0;
		try {
			fadeTime = Integer.parseInt(fadeTimeString);
		}
		catch (NumberFormatException ex) {
		}
		Control.getInstance().setMusicTagsFading(fadeTime, onlyOnChange);
		String combination = prefs.getString("tag_categories_op", "and");
		if (combination.equals("and"))
		{
			Control.getInstance().setTagCategoryCombination(INetworkClient.CategoryCombination.CategoryAnd);	
		}
		else if (combination.equals("globalAnd"))
		{
			Control.getInstance().setTagCategoryCombination(INetworkClient.CategoryCombination.And);
		}
		else 
		{
			Control.getInstance().setTagCategoryCombination(INetworkClient.CategoryCombination.Or);
		}
		Control.getInstance().setMusicOnAllSpeakers(musicOnAllSpeakers);
		int musicFadeTime = 0;
		try {
			musicFadeTime = Integer.parseInt(prefs.getString("music_fading_time", "0"));
		}
		catch (NumberFormatException ex) {
		}
		String musicFadeOp = prefs.getString("music_fading_op", "noFading");
		if (musicFadeOp.equals("crossFading")) {
			Control.getInstance().setMusicFading(2, musicFadeTime);
		}
		else if (musicFadeOp.equals("fading")) {
			Control.getInstance().setMusicFading(1, musicFadeTime);
		}
		else {
			Control.getInstance().setMusicFading(0, musicFadeTime);
		}
	}
	
	protected void onConnect(ServerInfo info) {
		if (setPlayerPreferencesOnConnect) {
			setPlayerPreferences();
			setPlayerPreferencesOnConnect = false;
		}
	}
	
	protected void onDisconnect(boolean startServerSearch) {
		if (startServerSearch && (isControlFragment() || !isOnTablet())) {
			if (!serverSearch.isSearching()) {
	    		Log.d("ConnectedFragment", "Starting server search");
				serverSearch.startSearch();
			}
		}
	}
	
	protected void doDisconnect(boolean startServerSearch, boolean informServer) {
		Log.d("ConnectedFragment", "Disconnecting (" + startServerSearch + ", " + informServer + ")");
		Control.getInstance().disconnect(informServer);
		PlayingState.getInstance().clearState();
		onDisconnect(startServerSearch);
	}
	
	protected void doConnect(ServerInfo info) {
		Log.d("ConnectedFragment", "Stopping server search");
		serverSearch.stopSearch();
		Log.d("ConnectedFragment", "Connecting with server");
		Control.getInstance().connect(info, PlayingState.getInstance(), false);		
		onConnect(info);
	}

	private ScheduledExecutorService mScheduler;
	private ScheduledFuture<?> mFuture;

	private boolean IsAppInstalled(String pkg)
	{
		PackageManager pm = getActivity().getPackageManager();
		boolean installed = false;
		try
		{
			pm.getPackageInfo(pkg, PackageManager.GET_ACTIVITIES);
			installed = true;
		}
		catch (PackageManager.NameNotFoundException ex)
		{
			installed = false;
		}
		return installed;
	}

	private void StartLocalPlayer()
	{
		boolean startLocalPlayer = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext()).getBoolean("start_local_player", true);
		if (!startLocalPlayer)
			return;
		Log.d("ConnectedFragment", "Timeout: starting local player if installed.");
		Handler handler = new Handler(getActivity().getMainLooper());
		handler.post(new Runnable()
		{
			public void run()
			{
				if (servers.isEmpty()) {
					if (IsAppInstalled("de.joerg_ruedenauer.ares_player"))
					{
						Log.d("ConnectedFragment", "Starting local player service");
						Intent intent = new Intent("de.joerg_ruedenauer.ares.PlayerService");
						intent.putExtra("UDPPort", getServerSearchPort());
						getActivity().startService(intent);
					}
				}
			}
		});
	}
	
    protected void tryConnect() {
    	String connectMode = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext()).getString("player_connection", "auto");
    	if (connectMode.equals("auto")) {
    		if (!serverSearch.isSearching()) {
	    		Log.d("ConnectedFragment", "Starting server search");
	    		serverSearch.startSearch();
				if (mScheduler == null) {
					mScheduler = Executors.newSingleThreadScheduledExecutor();
				}
				mFuture = mScheduler.schedule(new Runnable() { public void run() { StartLocalPlayer(); mFuture = null; } }, 1500, TimeUnit.MILLISECONDS);
    		}
    	}
    	else {
    		try {
    			ServerInfo info = ServerSearch.getServerInfo(connectMode, ",");
    			if (info == null) {
    				Toast.makeText(getActivity().getApplicationContext(), getString(R.string.invalid_player_connection_format), Toast.LENGTH_LONG).show();
    			}
    			else {
    				doConnect(info);
    			}
    		}
    		catch (UnknownHostException e) {
    			Toast.makeText(getActivity().getApplicationContext(), getString(R.string.invalid_player_connection_format), Toast.LENGTH_LONG).show();
    		}
    		catch (IllegalArgumentException e) {
    			Toast.makeText(getActivity().getApplicationContext(), getString(R.string.invalid_player_connection_format), Toast.LENGTH_LONG).show();
    		}
    	}
    }
    
	protected static HashMap<String, ServerInfo> servers = new HashMap<String, ServerInfo>();
	protected static ArrayList<String> serverNames = new ArrayList<String>();
	private boolean connectWithFirstServer = true;

	@Override
	public void serverFound(ServerInfo server) {
		if (mFuture != null)
		{
			mFuture.cancel(false);
		}
		if (!servers.containsKey(server.getName())) {
			Log.d("ConnectedFragment", "New server " + server.getName() + " found");
			servers.put(server.getName(), server);
			serverNames.add(server.getName());
			if (connectWithFirstServer && servers.size() == 1 && server.hasTcpServer()) {
				connectWithFirstServer = false;
				doConnect(server);
			}
			else {
				Log.d("ConnectedFragment", "Not connecting with found server");
			}
		}
	}

	private int getServerSearchPort() {
        String portString = PreferenceManager.getDefaultSharedPreferences(getActivity().getBaseContext()).getString("udp_port", "8009");
        int port = 8009;
        try {
        	port = Integer.parseInt(portString);
        }
        catch (NumberFormatException e) {
        	port = 8009;
        }
        return port;
	}
	
	private static class ConnectionManager {
		
		private static ConnectionManager sInstance;
		
		public static ConnectionManager getInstance() {
			if (sInstance == null)
				sInstance = new ConnectionManager();
			return sInstance;
		}
		
		private int mClients = 0;
		
		public int addClient() {
			Log.d("ConnectedFragment", "Now " + (mClients + 1) + " clients");
			return ++mClients;
		}
		
		public int removeClient() {
			Log.d("ConnectedFragment", "Now " + (mClients - 1) + " clients");
			return --mClients;
		}
	}
}
