/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Editor.Actions;

namespace Ares.Editor
{
    partial class ProjectExplorer : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ProjectExplorer()
        {
            InitializeComponent();
            HideOnClose = true;
            RecreateTree();
        }

        private System.Action m_AfterEditAction;

        protected override String GetPersistString()
        {
            return "ProjectExplorer";
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
            RecreateTree();
        }

        public ContextMenuStrip EditContextMenu
        {
            get
            {
                ContextMenuStrip menu = m_SelectedNode != null ? m_SelectedNode.ContextMenuStrip : null;
                if (menu != null)
                    UpdateContextMenuDueToPlaying(menu);
                return menu;
            }
        }

        private void RecreateTree()
        {
            projectTree.BeginUpdate();
            projectTree.Nodes.Clear();
            if (m_Project != null)
            {
                TreeNode projectNode = new TreeNode(m_Project.Title);
                projectNode.Tag = m_Project;
                AddModeNodes(projectNode, m_Project);
                projectNode.ContextMenuStrip = projectContextMenu;
                projectTree.Nodes.Add(projectNode);
            }
            else
            {
                projectTree.Nodes.Add(StringResources.NoOpenedProject);
            }
            projectTree.ExpandAll();
            projectTree.EndUpdate();
        }

        private void AddModeNodes(TreeNode projectNode, Ares.Data.IProject project)
        {
            KeysConverter converter = new KeysConverter();
            foreach (Ares.Data.IMode mode in project.GetModes())
            {
                TreeNode modeNode = new TreeNode(mode.GetNodeTitle());
                projectNode.Nodes.Add(modeNode);
                modeNode.Tag = mode;
                modeNode.ContextMenuStrip = modeContextMenu;
                foreach (Ares.Data.IModeElement element in mode.GetElements())
                {
                    AddModeElement(modeNode, element);
                }
            }
        }

        private void AddModeElement(TreeNode modeNode, Ares.Data.IModeElement modeElement)
        {
            TreeNode node = CreateModeElementNode(modeElement);
            modeNode.Nodes.Add(node);
            Ares.Data.IElement startElement = modeElement.StartElement;
            if (startElement is Ares.Data.IGeneralElementContainer)
            {
                AddSubElements(node, (startElement as Ares.Data.IGeneralElementContainer).GetGeneralElements());
            }
            else if (startElement is Ares.Data.IBackgroundSounds)
            {
                AddSubElements(node, (startElement as Ares.Data.IBackgroundSounds).GetElements());
            }
        }

        private void AddSubElements(TreeNode parent, IList<Ares.Data.IContainerElement> subElements)
        {
            foreach (Ares.Data.IContainerElement subElement in subElements)
            {
                Ares.Data.IElement innerElement = subElement.InnerElement;
                if (innerElement is Ares.Data.IFileElement)
                {
                    continue;
                }
                TreeNode node = CreateElementNode(innerElement);
                parent.Nodes.Add(node);
                if (innerElement is Ares.Data.IGeneralElementContainer)
                {
                    AddSubElements(node, (innerElement as Ares.Data.IGeneralElementContainer).GetGeneralElements());
                }
            }
        }

        private void AddSubElements(TreeNode parent, IList<Ares.Data.IBackgroundSoundChoice> subElements)
        {
            foreach (Ares.Data.IBackgroundSoundChoice subElement in subElements)
            {
                TreeNode node = CreateElementNode(subElement);
                parent.Nodes.Add(node);
            }
        }

        private TreeNode CreateModeElementNode(Ares.Data.IModeElement modeElement)
        {
            TreeNode node = CreateElementNode(modeElement.StartElement);
            node.Text = modeElement.GetNodeTitle();
            node.Tag = modeElement;
            return node;
        }

        private TreeNode CreateElementNode(Ares.Data.IElement element)
        {
            TreeNode node = new TreeNode(element.Title);
            node.Tag = element;
            if (element is Ares.Data.IBackgroundSounds)
            {
                node.ContextMenuStrip = bgSoundsContextMenu;
            }
            else if (element is Ares.Data.IBackgroundSoundChoice)
            {
                node.ContextMenuStrip = elementContextMenu;
            }
            else if (element is Ares.Data.IRandomBackgroundMusicList)
            {
                node.ContextMenuStrip = elementContextMenu;
            }
            else if (element is Ares.Data.ISequentialBackgroundMusicList)
            {
                node.ContextMenuStrip = elementContextMenu;
            }
            else if (element is Ares.Data.IGeneralElementContainer)
            {
                node.ContextMenuStrip = containerContextMenu;
            }
            return node;
        }

        private static Ares.Data.IElement GetElement(TreeNode node)
        {
            if (node.Tag is Ares.Data.IModeElement)
            {
                return (node.Tag as Ares.Data.IModeElement).StartElement;
            }
            else
            {
                return node.Tag as Ares.Data.IElement;
            }
        }

        private Ares.Data.IProject m_Project;

        private TreeNode m_SelectedNode;

        private bool cancelExpand = false;

        private void projectTree_MouseDown(object sender, MouseEventArgs e)
        {
            m_SelectedNode = projectTree.GetNodeAt(e.X, e.Y);
            cancelExpand = e.Clicks > 1;
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameProject();
        }

        public void InitNewProject()
        {
            m_SelectedNode = projectTree.Nodes[0];
            AddMode(false);
            TreeNode modeNode = m_SelectedNode;
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    m_SelectedNode = modeNode;
                    RenameMode();
                };
            m_SelectedNode = projectTree.Nodes[0];
            RenameProject();
        }

        public void RenameProject()
        {
            TreeNode projectNode = projectTree.Nodes[0];
            projectTree.SelectedNode = projectNode;
            projectTree.LabelEdit = true;
            if (!projectNode.IsEditing)
                projectNode.BeginEdit();
        }

        private void projectTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            String text = e.Label;
            if (e.Node.Tag == m_Project)
            {
                if (e.Label == null)
                    return;
                Actions.Actions.Instance.AddNew(new RenameProjectAction(e.Node, text));
            }
            else if (e.Node.Tag is Ares.Data.IMode)
            {
                if (e.Label == null)
                {
                    e.Node.Text = (e.Node.Tag as Ares.Data.IMode).GetNodeTitle();
                    return;
                }
                Ares.Data.IMode mode = e.Node.Tag as Ares.Data.IMode;
                Actions.Actions.Instance.AddNew(new RenameModeAction(e.Node, text));
                e.CancelEdit = true; // the text is already changed by the action
                // TODO: check for empty or equal titles, output warning
            }
            else if (e.Node.Tag is Ares.Data.IModeElement)
            {
                if (e.Label == null)
                {
                    e.Node.Text = (e.Node.Tag as Ares.Data.IModeElement).GetNodeTitle();
                    return;
                }
                Ares.Data.IModeElement modeElement = e.Node.Tag as Ares.Data.IModeElement;
                Actions.Actions.Instance.AddNew(new RenameModeElementAction(e.Node, text));
                e.CancelEdit = true; // the text is already changed by the action
            }
            else
            {
                if (e.Label == null)
                {
                    return;
                }
                Ares.Data.IElement element = e.Node.Tag as Ares.Data.IElement;
                Actions.Actions.Instance.AddNew(new RenameElementAction(e.Node, text));
            }
            projectTree.LabelEdit = false;
            if (m_AfterEditAction != null)
                m_AfterEditAction();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddMode(true);
        }

        private void AddMode(bool immediateRename)
        {
            TreeNode modeNode;
            Actions.Actions.Instance.AddNew(new AddModeAction(m_SelectedNode, out modeNode));
            modeNode.ContextMenuStrip = modeContextMenu;
            m_SelectedNode = modeNode;
            projectTree.SelectedNode = modeNode;
            if (immediateRename)
            {
                RenameMode();
            }
        }

        private void RenameMode()
        {
            projectTree.SelectedNode = m_SelectedNode;
            projectTree.LabelEdit = true;
            if (!m_SelectedNode.IsEditing)
            {
                m_SelectedNode.Text = (m_SelectedNode.Tag as Ares.Data.IMode).Title;
                m_SelectedNode.BeginEdit();
            }
        }

        private void AddRandomPlaylist()
        {
            String name = StringResources.NewPlaylist;
            Ares.Data.IRandomBackgroundMusicList element = Ares.Data.DataModule.ElementFactory.CreateRandomBackgroundMusicList(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            RenameElement();
        }

        private void AddSequentialPlaylist()
        {
            String name = StringResources.NewPlaylist;
            Ares.Data.ISequentialBackgroundMusicList element = Ares.Data.DataModule.ElementFactory.CreateSequentialBackgroundMusicList(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            RenameElement();
        }

        private void AddBackgroundSounds()
        {
            String name = StringResources.NewBackgroundSounds;
            Ares.Data.IBackgroundSounds element = Ares.Data.DataModule.ElementFactory.CreateBackgroundSounds(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    AddSoundChoice(true);
                };
            RenameElement();
        }

        private void AddSoundChoice(bool renameImmediately)
        {
            String name = StringResources.NewSoundChoice;
            TreeNode node;
            Actions.Actions.Instance.AddNew(new Actions.AddSoundChoiceAction(m_SelectedNode, 
                GetElement(m_SelectedNode) as Ares.Data.IBackgroundSounds, 
                name, CreateElementNode, out node));
            m_SelectedNode.Expand();
            m_SelectedNode = node;
            if (renameImmediately)
                RenameElement();
        }

        private void AddParallelList()
        {
            String name = StringResources.NewParallelList;
            Ares.Data.IElementContainer<Ares.Data.IParallelElement> element = Ares.Data.DataModule.ElementFactory.CreateParallelContainer(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            RenameElement();
        }

        private void AddSequentialList()
        {
            String name = StringResources.NewSequentialList;
            Ares.Data.IElementContainer<Ares.Data.ISequentialElement> element = Ares.Data.DataModule.ElementFactory.CreateSequentialContainer(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            RenameElement();
        }

        private void AddRandomList()
        {
            String name = StringResources.NewRandomList;
            Ares.Data.IElementContainer<Ares.Data.IChoiceElement> element = Ares.Data.DataModule.ElementFactory.CreateChoiceContainer(name);
            if (m_SelectedNode.Tag is Ares.Data.IMode)
            {
                AddModeElement(element, name);
            }
            else
            {
                AddContainerElement(element);
            }
            RenameElement();
        }

        private void AddScenario()
        {
            String name = StringResources.NewScenario;
            Ares.Data.IElementContainer<Ares.Data.IParallelElement> element = Ares.Data.DataModule.ElementFactory.CreateParallelContainer(name);
            AddModeElement(element, name);
            TreeNode scenarioNode = m_SelectedNode;
            String name2 = StringResources.Music;
            Ares.Data.IRandomBackgroundMusicList element2 = Ares.Data.DataModule.ElementFactory.CreateRandomBackgroundMusicList(name2);
            AddContainerElement(element2);
            m_SelectedNode = scenarioNode;
            String name3 = StringResources.Sounds;
            Ares.Data.IBackgroundSounds element3 = Ares.Data.DataModule.ElementFactory.CreateBackgroundSounds(name3);
            AddContainerElement(element3);
            AddSoundChoice(false);
            TreeNode soundChoiceNode = m_SelectedNode;
            m_SelectedNode = scenarioNode;
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    m_SelectedNode = soundChoiceNode;
                    RenameElement();
                };
            RenameElement();
        }

        private void AddModeElement(Ares.Data.IElement startElement, String title)
        {
            Ares.Data.IModeElement modeElement = Ares.Data.DataModule.ElementFactory.CreateModeElement(title, startElement);
            TreeNode node = CreateModeElementNode(modeElement);
            Actions.Actions.Instance.AddNew(new AddModeElementAction(m_SelectedNode, modeElement, node));
            m_SelectedNode.Expand();
            m_SelectedNode = node;
        }

        private void AddContainerElement(Ares.Data.IElement element)
        {
            TreeNode node;
            Actions.Actions.Instance.AddNew(new AddElementAction(m_SelectedNode, 
                GetElement(m_SelectedNode) as Ares.Data.IGeneralElementContainer, element, CreateElementNode, out node));
            m_SelectedNode.Expand();
            m_SelectedNode = node;
        }

        private void RenameElement()
        {
            projectTree.SelectedNode = m_SelectedNode;
            projectTree.LabelEdit = true;
            if (!m_SelectedNode.IsEditing)
            {
                m_SelectedNode.Text = (m_SelectedNode.Tag as Ares.Data.IElement).Title;
                m_SelectedNode.BeginEdit();
            }
        }

        private void addScenarioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddScenario();
        }

        private void renameToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RenameMode();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMode();
        }

        private void DeleteMode()
        {
            Ares.Data.IMode mode = m_SelectedNode as Ares.Data.IMode;
            if (m_SelectedNode != null)
            {
                Actions.Actions.Instance.AddNew(new DeleteModeAction(m_SelectedNode));
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectModeKey();
        }

        private void SelectModeKey()
        {
            int keyCode = 0;
            DialogResult result = Dialogs.KeyDialog.Show(this, out keyCode);
            if (result != DialogResult.Cancel)
            {
                Actions.Actions.Instance.AddNew(new Actions.SetModeKeyAction(m_SelectedNode, keyCode));
            }
        }

        private void SelectModeElementKey()
        {
            int keyCode = 0;
            DialogResult result = Dialogs.KeyDialog.Show(this, out keyCode);
            if (result != DialogResult.Cancel)
            {
                Actions.Actions.Instance.AddNew(new Actions.SetModeElementKeyAction(m_SelectedNode, keyCode));
            }
        }

        private void containerContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            if (m_SelectedNode.Tag is Ares.Data.IModeElement)
            {
                selectKeyToolStripMenuItem.Visible = true;
            }
            else
            {
                selectKeyToolStripMenuItem.Visible = false;
            }
        }

        private void elementContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            if (m_SelectedNode.Tag is Ares.Data.IModeElement)
            {
                selectKeyToolStripMenuItem1.Visible = true;
            }
            else
            {
                selectKeyToolStripMenuItem1.Visible = false;
            }
        }

        private void addRandomPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRandomPlaylist();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteElement();
        }

        private void DeleteElement()
        {
            if (m_SelectedNode.Parent.Tag is Ares.Data.IMode)
            {
                Actions.Actions.Instance.AddNew(new DeleteModeElementAction(m_SelectedNode));
            }
            else if (m_SelectedNode.Parent.Tag is Ares.Data.IBackgroundSounds)
            {
                Actions.Actions.Instance.AddNew(new DeleteBackgroundSoundChoiceAction(m_SelectedNode));
            }
            else
            {
                Actions.Actions.Instance.AddNew(new DeleteElementAction(m_SelectedNode));
            }
        }

        private void EditElement(Ares.Data.IElement element)
        {
            ElementEditors.Editors.ShowEditor(element, DockPanel);
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DeleteElement();
        }

        private void renameToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            RenameElement();
        }

        private void renameToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            RenameElement();
        }

        private void selectKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private void selectKeyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private void addSequentialPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSequentialPlaylist();
        }

        private void addBackgroundSoundsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBackgroundSounds();
        }

        private void addParallelElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddParallelList();
        }

        private void addSequentialElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSequentialList();
        }

        private void addChoiceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRandomList();
        }

        private void addChoiceListToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddRandomList();
        }

        private void addSequentialElementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddSequentialList();
        }

        private void addParallelElementToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddParallelList();
        }

        private void addBackgroundSoundsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddBackgroundSounds();
        }

        private void addSequentialPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddSequentialPlaylist();
        }

        private void addRandomPlaylistToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddRandomPlaylist();
        }

        private void projectTree_DoubleClick(object sender, EventArgs e)
        {
            if (m_SelectedNode != null)
            {
                if (m_SelectedNode.Tag is Ares.Data.IMode)
                {
                    SelectModeKey();
                }
                else if (m_SelectedNode.Tag is Ares.Data.IModeElement && GetElement(m_SelectedNode) is Ares.Data.IBackgroundSounds)
                {
                    SelectModeElementKey();
                }
                else if (m_SelectedNode.Tag is Ares.Data.IElement)
                {
                    EditElement(GetElement(m_SelectedNode));
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditElement(GetElement(m_SelectedNode));
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditElement(GetElement(m_SelectedNode));
        }

        private void projectTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelExpand)
            {
                e.Cancel = true;
                cancelExpand = false;
            }
        }

        private void projectTree_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (cancelExpand)
            {
                e.Cancel = true;
                cancelExpand = false;
            }
        }

        private void renameToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            RenameElement();
        }

        private void deleteToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            DeleteElement();
        }

        private void addSoundChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSoundChoice(true);
        }

        private void bgSoundsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            if (m_SelectedNode.Tag is Ares.Data.IModeElement)
            {
                selectKeyToolStripMenuItem2.Visible = true;
            }
            else
            {
                selectKeyToolStripMenuItem2.Visible = false;
            }

        }

        private void selectKeyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private Ares.Data.IElement m_PlayedElement;

        private void playButton_Click(object sender, EventArgs e)
        {
            Ares.Data.IElement element = GetElement(m_SelectedNode);
            if (element != null)
            {
                m_PlayedElement = element;
                Actions.Playing.Instance.PlayElement(element, this, () =>
                     {
                         m_PlayedElement = null;
                         stopButton.Enabled = false;
                         playButton.Enabled = PlayingPossible;
                     });
                stopButton.Enabled = true;
                playButton.Enabled = false;
            }
        }

        private bool PlayingPossible
        {
            get
            {
                if (m_PlayedElement != null)
                    return false;
                if (m_SelectedNode == null)
                    return false;
                Ares.Data.IElement element = GetElement(m_SelectedNode);
                if (element == null)
                    return false;
                if (Actions.Playing.Instance.IsElementOrSubElementPlaying(element))
                    return false;
                return true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (m_PlayedElement != null)
            {
                Actions.Playing.Instance.StopElement(m_PlayedElement);
            }
        }

        private void projectTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            playButton.Enabled = PlayingPossible;
            AddChangeListener();
        }

        private void projectTree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            RemoveChangeListener();
        }

        private void AddChangeListener()
        {
            if (m_SelectedNode != null)
            {
                Ares.Data.IElement element = GetElement(m_SelectedNode);
                if (element != null)
                {
                    Actions.ElementChanges.Instance.AddListener(element.Id, UpdateAfterElementChange);
                }
            }
        }

        
        private void RemoveChangeListener()
        {
            if (m_SelectedNode != null)
            {
                Ares.Data.IElement element = GetElement(m_SelectedNode);
                if (element != null)
                {
                    Actions.ElementChanges.Instance.RemoveListener(element.Id, UpdateAfterElementChange);
                }
            }
        }

        private void UpdateAfterElementChange(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (m_SelectedNode != null)
            {
                Ares.Data.IElement element = GetElement(m_SelectedNode);
                if (element != null)
                {
                    if (changeType == ElementChanges.ChangeType.Stopped || changeType == ElementChanges.ChangeType.Played)
                    {
                        playButton.Enabled = PlayingPossible;
                    }
                }
            }
        }

        private void modeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
        }

        private static bool IsNodePlaying(TreeNode node)
        {
            Ares.Data.IElement element = GetElement(node);
            if (element != null && Actions.Playing.Instance.IsElementOrSubElementPlaying(element))
            {
                return true;
            }
            else if (node.Tag is Ares.Data.IMode)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    if (IsNodePlaying(subNode))
                        return true;
                }
            }
            return false;
        }

        private void UpdateContextMenuDueToPlaying(ContextMenuStrip contextMenu)
        {
            bool disable = (m_SelectedNode != null && IsNodePlaying(m_SelectedNode));
            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item is ToolStripMenuItem)
                    item.Enabled = !disable;
            }
        }

        private void projectTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                if (m_SelectedNode != null)
                {
                    if (m_SelectedNode == projectTree.Nodes[0])
                        RenameProject();
                    else if (m_SelectedNode.Tag is Ares.Data.IMode)
                        RenameMode();
                    else
                        RenameElement();
                }
            }
        }
    }
}
