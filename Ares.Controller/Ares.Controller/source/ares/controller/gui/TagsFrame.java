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
package ares.controller.gui;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.GridLayout;
import java.awt.Rectangle;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;

import javax.swing.AbstractAction;
import javax.swing.AbstractButton;
import javax.swing.BorderFactory;
import javax.swing.Box;
import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JRadioButton;
import javax.swing.JScrollPane;
import javax.swing.JToggleButton;

import ares.controller.control.ComponentKeys;
import ares.controller.util.Localization;
import ares.controllers.control.Control;
import ares.controllers.data.TitledElement;

public class TagsFrame extends SubFrame {

	public TagsFrame(String title) {
		super(title, new Rectangle(50, 50, 350, 400));
		ComponentKeys.addAlwaysAvailableKeys(getRootPane());
		initialize();
		listenForSelection = true;		
	}
	
	private List<TitledElement> m_Categories;
	private Map<Integer, List<TitledElement>> m_Tags;
	private int m_CurrentCategoryId = -1;
	
	public void setTags(List<TitledElement> categories, Map<Integer, List<TitledElement>> tags) {
		m_Categories = categories;
		m_Tags = tags;
		
		listenForSelection = false;
		categorySelectionBox.removeAllItems();
		int selIndex = -1;
		for (TitledElement category : categories) {
			categorySelectionBox.addItem(category.getTitle());
			if (category.getId() == m_CurrentCategoryId) {
				selIndex = categorySelectionBox.getItemCount() - 1;
			}
		}
		if (selIndex == -1 && categories.size() > 0) {
			selIndex = 0;
			m_CurrentCategoryId = categories.get(0).getId();
		}
		if (selIndex != -1) {
			categorySelectionBox.setSelectedIndex(selIndex);
		}
		updateTagSelectionPanel();
		m_ActiveTags.clear();
		updateActiveTagsLabel();
		
		listenForSelection = true;
	}
	
	private HashSet<Integer> m_ActiveTags = new HashSet<Integer>();
	
	public void setActiveTags(HashSet<Integer> tagIds) {
		m_ActiveTags = tagIds;
		listenForSelection = false;
		for (AbstractButton button : m_Buttons.values()) {
			button.setSelected(false);
		}
		for (Integer id : tagIds) {
			if (m_Buttons.containsKey(id)) {
				m_Buttons.get(id).setSelected(true);
			}
		}
		updateActiveTagsLabel();
		listenForSelection = true;
	}
	
	public void setTagActive(int tagId, boolean isActive) {
		if (isActive) {
			m_ActiveTags.add(tagId);
		}
		else {
			m_ActiveTags.remove(tagId);
		}
		listenForSelection = false;
		if (m_Buttons.containsKey(tagId)) {
			m_Buttons.get(tagId).setSelected(isActive);
		}
		updateActiveTagsLabel();
		listenForSelection = true;
	}
	
	public void setCategoryOperator(boolean isAndOperator) {
		listenForSelection = false;
		categoryOrButton.setSelected(!isAndOperator);
		categoryAndButton.setSelected(isAndOperator);
		listenForSelection = true;
	}
	
	private void updateTagSelectionPanel() {
		tagSelectionPanel.removeAll();
		if (m_CurrentCategoryId != -1 && m_Tags.containsKey(m_CurrentCategoryId))
		{
			List<TitledElement> currentTags = m_Tags.get(m_CurrentCategoryId);
			int count = currentTags.size();
			int nrOfColumns = 3;
			JPanel buttonPanel = new JPanel();
			buttonPanel.setLayout(new GridLayout((int)Math.ceil(count / (float)nrOfColumns), nrOfColumns, 10, 10));
			for (TitledElement tag : currentTags) {
		        AbstractButton button = new JToggleButton();
		        button.setSelected(m_ActiveTags.contains(tag.getId()));
		        AbstractAction action = createTagAction(tag.getId(), m_CurrentCategoryId, button); 
		        button.setAction(action);
		        buttonPanel.add(button);
		        String title = tag.getTitle();
		        button.setText(title);				
			}
			tagSelectionPanel.add(buttonPanel, BorderLayout.NORTH);
		}
	}
	
	private void updateActiveTagsLabel() {
		StringBuilder builder = new StringBuilder();
		builder.append("<html>"); //$NON-NLS-1$
		for (TitledElement category : m_Categories) {
			boolean hasTag = false;
			if (!m_Tags.containsKey(category.getId()))
				continue;
			for (TitledElement tag : m_Tags.get(category.getId())) {
				if (m_ActiveTags.contains(tag.getId())) {
					if (!hasTag) {
						if (builder.length() > 6)
							builder.append("; "); //$NON-NLS-1$
						builder.append(category.getTitle());
						builder.append(": "); //$NON-NLS-1$
						hasTag = true;
					}
					else {
						builder.append(", "); //$NON-NLS-1$
					}
					builder.append(tag.getTitle());
				}
			}
		}
		builder.append("</html>"); //$NON-NLS-1$
		activeTagsLabel.setText(builder.toString());
	}
	
	private class TagSelectionAction extends AbstractAction {
		public void actionPerformed(ActionEvent arg0) {
			if (!listenForSelection)
				return;
			boolean active = ((JToggleButton)arg0.getSource()).isSelected();
			Control.getInstance().switchTag(m_CategoryId, m_TagId, active);
		}
		
		public TagSelectionAction(int tagId, int categoryId) {
			m_TagId = tagId;
			m_CategoryId = categoryId;
		}
		
		private int m_TagId;
		private int m_CategoryId;
	}
	
	private Map<Integer, AbstractButton> m_Buttons = new HashMap<Integer, AbstractButton>();
	
	private AbstractAction createTagAction(int tagId, int categoryId, AbstractButton button) {
		m_Buttons.put(tagId, button);
		return new TagSelectionAction(tagId, categoryId);
	}
	
	private void initialize() {
		mainPanel = new JPanel(new BorderLayout());
		mainPanel.setBorder(BorderFactory.createEmptyBorder(5, 5, 5, 5));
		
		categoriesPanel = new JPanel();
		categoriesPanel.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(), Localization.getString("TagsFrame.Categories"))); //$NON-NLS-1$
		categoriesPanel.setLayout(new BoxLayout(categoriesPanel, BoxLayout.PAGE_AXIS));
		categorySelectionPanel = new JPanel();
		categorySelectionPanel.setLayout(new BoxLayout(categorySelectionPanel, BoxLayout.LINE_AXIS));
		categorySelectionPanel.add(new JLabel(Localization.getString("TagsFrame.SelectedCategory"))); //$NON-NLS-1$
		categorySelectionPanel.setAlignmentX(LEFT_ALIGNMENT);
		categorySelectionBox = new JComboBox();
		categorySelectionBox.setMinimumSize(new Dimension(50, 15));
		categorySelectionBox.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (!listenForSelection)
					return;
				int selIndex = categorySelectionBox.getSelectedIndex();
				if (selIndex >= 0 && selIndex < m_Categories.size()) {
					m_CurrentCategoryId = m_Categories.get(selIndex).getId();
					updateTagSelectionPanel();
					tagSelectionPanel.revalidate();
				}
			}
		});
		categorySelectionPanel.add(Box.createRigidArea(new Dimension(5, 0)));
		categorySelectionPanel.add(categorySelectionBox);
		categorySelectionPanel.add(Box.createHorizontalGlue());
		categoriesPanel.add(categorySelectionPanel);
		categoriesPanel.add(Box.createRigidArea(new Dimension(0, 5)));
		JLabel label = new JLabel(Localization.getString("TagsFrame.CategoryOperatorDesc")); //$NON-NLS-1$
		label.setAlignmentX(LEFT_ALIGNMENT);
		categoriesPanel.add(label);
		categoriesPanel.add(Box.createRigidArea(new Dimension(0, 5)));
		categoryOrButton = new JRadioButton(Localization.getString("TagsFrame.CategoryOrOperator")); //$NON-NLS-1$
		categoryOrButton.setAlignmentX(LEFT_ALIGNMENT);
		categoryOrButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (!listenForSelection)
					return;
				Control.getInstance().setTagCategoryOperator(!categoryOrButton.isSelected());
			}
		});
		categoriesPanel.add(categoryOrButton);
		categoriesPanel.add(Box.createRigidArea(new Dimension(0, 5)));
		categoryAndButton = new JRadioButton(Localization.getString("TagsFrame.CategoryAndOperator")); //$NON-NLS-1$
		categoryAndButton.setAlignmentX(LEFT_ALIGNMENT);
		categoryAndButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (!listenForSelection)
					return;
				Control.getInstance().setTagCategoryOperator(categoryAndButton.isSelected());
			}
		});
		categoriesPanel.add(categoryAndButton);
		mainPanel.add(categoriesPanel, BorderLayout.NORTH);
		
		tagsPanel = new JPanel();
		tagsPanel.setBorder(BorderFactory.createTitledBorder(BorderFactory.createEtchedBorder(), Localization.getString("TagsFrame.Tags"))); //$NON-NLS-1$
		tagsPanel.setLayout(new BorderLayout());
		tagSelectionPanel = new JPanel(new BorderLayout());
		JScrollPane scrollPane = new JScrollPane(tagSelectionPanel);
		tagsPanel.add(scrollPane, BorderLayout.CENTER);
		activeTagsPanel = new JPanel();
		activeTagsPanel.setLayout(new BorderLayout(5, 5));
		activeTagsPanel1 = new JPanel();
		activeTagsPanel1.setLayout(new BorderLayout());
		activeTagsPanel1.add(new JLabel(Localization.getString("TagsFrame.CurrentItems")), BorderLayout.WEST); //$NON-NLS-1$
		clearTagsButton = new JButton(Localization.getString("TagsFrame.ClearTags")); //$NON-NLS-1$
		clearTagsButton.addActionListener(new ActionListener() {
			public void actionPerformed(ActionEvent e) {
				if (!listenForSelection)
					return;
				Control.getInstance().removeAllTags();
			}
		});
		activeTagsPanel1.add(clearTagsButton, BorderLayout.EAST);
		activeTagsPanel.add(activeTagsPanel1, BorderLayout.NORTH);
		activeTagsLabel = new JLabel(""); //$NON-NLS-1$
		activeTagsLabel.setMinimumSize(new Dimension(50, 20));
		
		activeTagsPanel.add(activeTagsLabel, BorderLayout.CENTER);
		tagsPanel.add(activeTagsPanel, BorderLayout.SOUTH);
		mainPanel.add(tagsPanel, BorderLayout.CENTER);

		setContentPane(mainPanel);
	}
	
	private boolean listenForSelection;
	
	private JPanel mainPanel;
	private JPanel tagsPanel;
	private JPanel tagSelectionPanel;
	private JPanel categoriesPanel;
	private JPanel categorySelectionPanel;
	private JPanel activeTagsPanel;
	private JPanel activeTagsPanel1;
	
	private JComboBox categorySelectionBox;
	private JRadioButton categoryAndButton;
	private JRadioButton categoryOrButton;
	private JButton clearTagsButton;
	private JLabel activeTagsLabel;
}
