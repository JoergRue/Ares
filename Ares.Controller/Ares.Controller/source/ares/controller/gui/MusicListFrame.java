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
package ares.controller.gui;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.Rectangle;
import java.util.ArrayList;
import java.util.List;

import javax.swing.BorderFactory;
import javax.swing.DefaultListModel;
import javax.swing.JComponent;
import javax.swing.JLabel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.ListSelectionModel;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;

import ares.controller.control.ComponentKeys;
import ares.controller.util.Localization;
import ares.controllers.control.Control;
import ares.controllers.data.TitledElement;

public class MusicListFrame extends SubFrame {

	public MusicListFrame(String title) {
		super(title, new Rectangle(50, 50, 200, 250));
		ComponentKeys.addAlwaysAvailableKeys(getRootPane());
		initialize();
		listenForSelection = true;
	}
	
	private void initialize() {
		mainPanel = new JPanel(new BorderLayout());
		mainPanel.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
		noTitlesLabel = new JLabel();
		noTitlesLabel.setText(Localization.getString("MusicListFrame.NoMusicListPlaying")); //$NON-NLS-1$
		noTitlesLabel.setForeground(Color.DARK_GRAY);
		mainPanel.add(noTitlesLabel, BorderLayout.CENTER);
		setContentPane(mainPanel);
		listShown = false;
		model = new DefaultListModel();
	}
	
	public void setTitles(List<TitledElement> titles) {
		List<TitledElement> copy = titles != null ? new ArrayList<TitledElement>(titles) : null;
		musicTitles = titles;
		listenForSelection = false;
		model.clear();
		if (titles != null && titles.size() > 0) {
			for (TitledElement element : copy) {
				model.addElement(element.getTitle());
			}
			if (!listShown) {
				mainPanel.remove(noTitlesLabel);
				mainPanel.add(getTitlesList(), BorderLayout.CENTER);
				mainPanel.invalidate();
				mainPanel.validate();
				mainPanel.repaint();
				listShown = true;
				if (!"".equals(activeTitle)) //$NON-NLS-1$
					setActiveTitle(activeTitle);
			}
		}
		else if (listShown) {
			mainPanel.remove(getTitlesList());
			mainPanel.add(noTitlesLabel, BorderLayout.CENTER);
			mainPanel.invalidate();
			mainPanel.validate();
			mainPanel.repaint();
			listShown = false;
		}
		listenForSelection = true;
	}
	
	private String activeTitle = ""; //$NON-NLS-1$
	
	public void setActiveTitle(String title) {
		activeTitle = title;
		if (!listShown)
			return;
		if (musicTitles == null)
			return;
		for (int i = 0; i < musicTitles.size(); ++i) {
			if (musicTitles.get(i).getTitle().equals(title)) {
				listenForSelection = false;
				titlesList.setSelectedIndex(i);
				listenForSelection = true;
				titlesList.scrollRectToVisible(titlesList.getCellBounds(i, i));
				break;
			}
		}
	}
	
	private JComponent getTitlesList() {
		if (titlesList == null) {
			titlesList = new JList(model);
			titlesList.setSelectionMode(ListSelectionModel.SINGLE_SELECTION);
			titlesList.addListSelectionListener(new ListSelectionListener() {
				public void valueChanged(ListSelectionEvent arg0) {
					if (!listenForSelection)
						return;
					int index = titlesList.getSelectedIndex();
					if (index >= 0 && index < musicTitles.size()) {
						TitledElement selected = musicTitles.get(index);
						Control.getInstance().setMusicTitle(selected.getId());
					}
				}
			});
			titlesPane = new JScrollPane(titlesList);
			titlesPane.setOpaque(false);
			titlesPane.getViewport().setOpaque(false);
		}
		return titlesPane;
	}
	
	private boolean listenForSelection;
	private boolean listShown;
	private JList titlesList;
	private JScrollPane titlesPane;
	private JLabel noTitlesLabel;
	private JPanel mainPanel;
	private List<TitledElement> musicTitles;
	private DefaultListModel model;
}
