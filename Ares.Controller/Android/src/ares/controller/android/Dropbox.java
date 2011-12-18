package ares.controller.android;

import java.io.ByteArrayOutputStream;
import java.util.TreeMap;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences.Editor;
import android.preference.PreferenceManager;
import android.widget.Toast;
import ares.controllers.control.Control;

import com.dropbox.client2.DropboxAPI;
import com.dropbox.client2.android.AndroidAuthSession;
import com.dropbox.client2.exception.DropboxException;
import com.dropbox.client2.exception.DropboxServerException;
import com.dropbox.client2.session.AccessTokenPair;
import com.dropbox.client2.session.AppKeyPair;
import com.dropbox.client2.session.Session.AccessType;

class Dropbox {

	public static Dropbox getInstance() {
		if (sInstance == null) {
			sInstance = new Dropbox();
		}
		return sInstance;
	}
	
	public boolean connectToDropbox(Activity activity) {
		if (mDBApi != null) {
			return true;
		}
		AppKeyPair appKeys = new AppKeyPair(APP_KEY, APP_SECRET);
		AccessTokenPair pair = getDBAccessTokens(activity);
		if (pair == null)
		{
			AndroidAuthSession session = new AndroidAuthSession(appKeys, ACCESS_TYPE);
			mDBApi = new DropboxAPI<AndroidAuthSession>(session);
			mDBApi.getSession().startAuthentication(activity);
			mWaitForAuthentication = true;
			return false;
		}
		else {
			try {
				AndroidAuthSession session = new AndroidAuthSession(appKeys, ACCESS_TYPE, pair);
				mDBApi = new DropboxAPI<AndroidAuthSession>(session);
				return true;
			}
			catch (Exception e) {
				AndroidAuthSession session = new AndroidAuthSession(appKeys, ACCESS_TYPE);
				mDBApi = new DropboxAPI<AndroidAuthSession>(session);
				mDBApi.getSession().startAuthentication(activity);
				mWaitForAuthentication = true;
				return false;				
			}
		}
	}
	
	public boolean isWaiting() {
		return mWaitForAuthentication;
	}
	
	public boolean finishConnection(Activity activity) {
		if (mWaitForAuthentication) {
			mWaitForAuthentication = false;
			if (mDBApi.getSession().authenticationSuccessful()) {
				try {
					mDBApi.getSession().finishAuthentication();
					AccessTokenPair tokens = mDBApi.getSession().getAccessTokenPair();
					storeDBAccessTokens(activity, tokens);
					return true;
				}
				catch (IllegalStateException e) {
					mDBApi = null;
					return false;
				}
			}
			else {
				mDBApi = null;
				return false;
			}
		}
		else 
			return false;
	}
	
	public DropboxAPI.Entry getDBEntry(Context context, String dbPath) {
		String realPath = dbPath.substring(Control.DB_ROOT_ID.length());
		DropboxAPI.Entry cachedEntry = null;
		if (DBCache().containsKey(realPath)) {
			cachedEntry = DBCache().get(realPath);
		}
		if (mDBApi == null) {
			return null;
		}
		try {
			DropboxAPI.Entry result = mDBApi.metadata(realPath, 0, cachedEntry != null ? cachedEntry.hash : null, true, null);
			return result;
		}
		catch (DropboxServerException e) {
			if (e.error == 304) {
				return cachedEntry;
			}
			else {
				Toast.makeText(context, e.getLocalizedMessage(), Toast.LENGTH_SHORT).show();
			}
		}
		catch (Exception e) {
			Toast.makeText(context, e.getLocalizedMessage(), Toast.LENGTH_SHORT).show();			
		}
		return null;
	}
	
	public byte[] getEntryContent(Context context, String dbPath) {
		if (mDBApi == null)
			return null;
		try {
			ByteArrayOutputStream stream = new ByteArrayOutputStream();
			mDBApi.getFile(dbPath.substring(Control.DB_ROOT_ID.length()), null, stream, null);
			return stream.toByteArray();
		}
		catch (DropboxException e) {
			Toast.makeText(context, e.getLocalizedMessage(), Toast.LENGTH_SHORT).show();
			return null;
		}
	}
	
	private AccessTokenPair getDBAccessTokens(Activity activity) {
		String key = PreferenceManager.getDefaultSharedPreferences(activity.getBaseContext()).getString("dropboxKey", "");
    	String secret = PreferenceManager.getDefaultSharedPreferences(activity.getBaseContext()).getString("dropboxSecret", "");
    	if (key.length() > 0 && secret.length() > 0) {
    		return new AccessTokenPair(key, secret);
    	}
		return null;
	}
	
	private void storeDBAccessTokens(Activity activity, AccessTokenPair pair) {
		Editor editor = PreferenceManager.getDefaultSharedPreferences(activity.getBaseContext()).edit();
		editor.putString("dropboxKey", pair.key);
		editor.putString("dropboxSecret", pair.secret);
		editor.commit();
	}
	
	private TreeMap<String, DropboxAPI.Entry> DBCache() {
		return dbDirectoriesCache;
	}
	
	
	private static Dropbox sInstance;
	final static private String APP_KEY = "5g36qiqft5s3fk4";
	final static private String APP_SECRET = "kwkraklaao1r7bt";
	final static private AccessType ACCESS_TYPE = AccessType.APP_FOLDER;
	private DropboxAPI<AndroidAuthSession> mDBApi;
	private TreeMap<String, DropboxAPI.Entry> dbDirectoriesCache = new TreeMap<String, DropboxAPI.Entry> ();
	private boolean mWaitForAuthentication = false;
	
}
