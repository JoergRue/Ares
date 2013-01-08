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
import java.util.HashMap;
import java.util.List;

import android.app.ListActivity;
import android.os.Bundle;
import android.view.View;

import android.widget.ListView;
import android.widget.SimpleAdapter;

public class FileDialog extends ListActivity {

	private static final String ITEM_KEY = "key";
	private static final String ITEM_IMAGE = "image";

	public static final String POSSIBLE_FILES = "POSSIBLE_FILES";
	public static final String RESULT_PATH = "RESULT_PATH";

	private List<String> path = null;
	private ArrayList<HashMap<String, Object>> mList;

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setResult(RESULT_CANCELED, getIntent());

		setContentView(R.layout.file_dialog_main);
		List<String> files = getIntent().getStringArrayListExtra(POSSIBLE_FILES);

		path = new ArrayList<String>();
		mList = new ArrayList<HashMap<String, Object>>();
		path.addAll(files);

		SimpleAdapter fileList = new SimpleAdapter(this, mList,
				R.layout.file_dialog_row,
				new String[] { ITEM_KEY, ITEM_IMAGE }, new int[] {
						R.id.fdrowtext, R.id.fdrowimage });

		for (String file : files) {
			addItem(file, R.drawable.file);
		}

		fileList.notifyDataSetChanged();

		setListAdapter(fileList);
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
		selectItem(itemPath, position);
	}

	private void selectItem(String itemPath, int position) {
		getIntent().putExtra(RESULT_PATH, itemPath);
		setResult(RESULT_OK, getIntent());
		finish();
	}
	
}