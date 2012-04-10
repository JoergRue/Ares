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

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.TreeMap;

import com.dropbox.client2.DropboxAPI;

import android.app.AlertDialog;
import android.app.ListActivity;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;

import android.widget.ListView;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.RadioGroup.OnCheckedChangeListener;
import android.widget.SimpleAdapter;
import android.widget.TextView;
import ares.controllers.control.Control;

public class FileDialog extends ListActivity {

	private static final String ITEM_KEY = "key";
	private static final String ITEM_IMAGE = "image";

	public static final String START_PATH = "START_PATH";
	public static final String RESULT_PATH = "RESULT_PATH";

	private List<String> path = null;
	private String root = "/";
	private String dbRoot = Control.DB_ROOT_ID + "/";
	private TextView myPath;
	private ArrayList<HashMap<String, Object>> mList;

	private String parentPath;
	private String currentPath = root;

	private HashMap<String, Integer> lastPositions = new HashMap<String, Integer>();

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setResult(RESULT_CANCELED, getIntent());

		setContentView(R.layout.file_dialog_main);
		myPath = (TextView) findViewById(R.id.path);
		String startPath = getIntent().getStringExtra(START_PATH);
		if (startPath != null) {
			String sep = startPath.startsWith(Control.DB_ROOT_ID) ? "/" : File.separator;
			if (!startPath.endsWith(sep)) {
				int lastSep = startPath.lastIndexOf(sep);
				if (lastSep != -1) {
					startPath = startPath.substring(0, lastSep);
				}
			}
			if (startPath.equals(Control.DB_ROOT_ID)) {
				startPath = dbRoot;
			}
			if (startPath.startsWith(Control.DB_ROOT_ID)) {
				((RadioButton)findViewById(R.id.rbDropbox)).setChecked(true);
			}
		}
		RadioGroup group = (RadioGroup) findViewById(R.id.fileSourceRadioGroup);
		group.setOnCheckedChangeListener(new OnCheckedChangeListener() {
			public void onCheckedChanged(RadioGroup group, int checkedId) {
				if (checkedId == R.id.rbDropbox && !currentPath.startsWith(dbRoot)) {
					if (Dropbox.getInstance().connectToDropbox(FileDialog.this)) {
						getDir(dbRoot);
					}
					// result == false: delayed, must be authenticated first (other activity started)
				}
				else if (checkedId == R.id.rbFileSystem && currentPath.startsWith(dbRoot)){
					getDir(root);
				}
			}
		});

		if (startPath != null && !startPath.equals("")) {
			getDir(startPath);
		} else {
			getDir(root);
		}
	}
	
	private void getDir(String dirPath) {

		boolean useAutoSelection = dirPath != null && currentPath != null && dirPath.length() < currentPath.length();

		Integer position = lastPositions.get(parentPath);

		getDirImpl(dirPath != null ? dirPath : "");

		if (position != null && useAutoSelection) {
			getListView().setSelection(position);
		}

	}

	private void getDirImpl(String dirPath) {
		
		myPath.setText(getText(R.string.location) + ": " + dirPath);
		currentPath = dirPath;

		path = new ArrayList<String>();
		mList = new ArrayList<HashMap<String, Object>>();

		TreeMap<String, String> dirsMap = new TreeMap<String, String>();
		TreeMap<String, String> dirsPathMap = new TreeMap<String, String>();
		TreeMap<String, String> filesMap = new TreeMap<String, String>();
		TreeMap<String, String> filesPathMap = new TreeMap<String, String>();

		if (dirPath.startsWith(dbRoot)) {
			readDBDirectory(dirPath, dirsMap, dirsPathMap, filesMap,
					filesPathMap);
		}
		else {
			readLocalDirectory(dirPath, dirsMap, dirsPathMap, filesMap,
					filesPathMap);
		}
		path.addAll(dirsPathMap.tailMap("").values());
		path.addAll(filesPathMap.tailMap("").values());

		SimpleAdapter fileList = new SimpleAdapter(this, mList,
				R.layout.file_dialog_row,
				new String[] { ITEM_KEY, ITEM_IMAGE }, new int[] {
						R.id.fdrowtext, R.id.fdrowimage });

		for (String dir : dirsMap.tailMap("").values()) {
			addItem(dir, R.drawable.folder);
		}

		for (String file : filesMap.tailMap("").values()) {
			addItem(file, R.drawable.file);
		}

		fileList.notifyDataSetChanged();

		setListAdapter(fileList);

	}

	private void readLocalDirectory(String dirPath,
			TreeMap<String, String> dirsMap,
			TreeMap<String, String> dirsPathMap,
			TreeMap<String, String> filesMap,
			TreeMap<String, String> filesPathMap) {
		File f = new File(dirPath);
		File[] files = f.listFiles();

		if (!dirPath.equals(root)) {

			addItem(root, R.drawable.folder);
			path.add(root);

			addItem("../", R.drawable.folder);
			path.add(f.getParent());
			parentPath = f.getParent();

		}

		if (files == null)
			return;
		for (File file : files) {
			if (file.isDirectory()) {
				String dirName = file.getName();
				dirsMap.put(dirName, dirName);
				dirsPathMap.put(dirName, file.getPath());
			} else {
				filesMap.put(file.getName(), file.getName());
				filesPathMap.put(file.getName(), file.getPath());
			}
		}
	}

	private void addItem(String fileName, int imageId) {
		HashMap<String, Object> item = new HashMap<String, Object>();
		item.put(ITEM_KEY, fileName);
		item.put(ITEM_IMAGE, imageId);
		mList.add(item);
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {

		String itemPath = path.get(position);
		if (itemPath == null)
			return;
		if (itemPath.startsWith(Control.DB_ROOT_ID)) {
			selectDBItem(itemPath, position);
		}
		else {
			selectLocalItem(itemPath, position);
		}
	}

	private void selectLocalItem(String itemPath, int position) {
		File file = new File(itemPath);
		if (file.isDirectory()) {
			if (file.canRead()) {
				lastPositions.put(currentPath, position);
				getDir(itemPath);
			} else {
				new AlertDialog.Builder(this).setIcon(R.drawable.icon)
						.setTitle(
								"[" + file.getName() + "] "
										+ getText(R.string.cant_read_folder))
						.setPositiveButton("OK",
								new DialogInterface.OnClickListener() {

									@Override
									public void onClick(DialogInterface dialog,
											int which) {

									}
								}).show();
			}
		} else {
			getIntent().putExtra(RESULT_PATH, file.getPath());
			setResult(RESULT_OK, getIntent());
			finish();
		}
	}

	private void selectDBItem(String itemPath, int position) {
		DropboxAPI.Entry entry = Dropbox.getInstance().getDBEntry(this, itemPath);
		if (entry == null)
			return;
		if (entry.isDir) {
			lastPositions.put(currentPath, position);
			getDir(itemPath);
		}
		else {
			getIntent().putExtra(RESULT_PATH, itemPath);
			setResult(RESULT_OK, getIntent());
			finish();
		}
	}

	@Override
	public boolean onKeyDown(int keyCode, KeyEvent event) {
		if ((keyCode == KeyEvent.KEYCODE_BACK)) {

			if (!currentPath.equals(root)) {
				getDir(parentPath);
			} else {
				return super.onKeyDown(keyCode, event);
			}

			return true;
		} else {
			return super.onKeyDown(keyCode, event);
		}
	}

	protected void onResume() {
		super.onResume();
		
		if (((RadioButton)findViewById(R.id.rbDropbox)).isChecked() && Dropbox.getInstance().isWaiting()) {
			if (Dropbox.getInstance().finishConnection(this)) {
				getDir(dbRoot);
			}
			else {
				((RadioButton)findViewById(R.id.rbFileSystem)).setChecked(true);
			}
		}
	}
	
	private void readDBDirectory(String dirPath,
			TreeMap<String, String> dirsMap,
			TreeMap<String, String> dirsPathMap,
			TreeMap<String, String> filesMap,
			TreeMap<String, String> filesPathMap) {
		
		DropboxAPI.Entry entry = Dropbox.getInstance().getDBEntry(this, dirPath);
		if (entry == null) {
			return;
		}
		
		if (!dirPath.equals(dbRoot)) {

			addItem(entry.root, R.drawable.folder);
			path.add(dbRoot);

			addItem("../", R.drawable.folder);
			path.add(Control.DB_ROOT_ID + entry.parentPath());
			parentPath = Control.DB_ROOT_ID + entry.parentPath();
		}

		for (DropboxAPI.Entry file : entry.contents) {
			String name = file.fileName();
			String path = dirPath.equals(dbRoot) ? dbRoot + name : dirPath + "/" + name;
			if (file.isDir) {
				dirsMap.put(name, name);
				dirsPathMap.put(name, path);
			} else {
				filesMap.put(name, name);
				filesPathMap.put(name, path);
			}
		}
	}
	
}