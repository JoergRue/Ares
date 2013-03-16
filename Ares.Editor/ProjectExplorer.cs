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
using Ares.Data;

namespace Ares.Editor
{
    partial class ProjectExplorer : ToolWindow
    {
        private static void ensureDefaultImageIndex(TreeView tree)
        {
            if (tree.ImageList != null)
            {
                tree.ImageIndex = tree.ImageList.Images.Count;
                tree.SelectedImageIndex = tree.ImageList.Images.Count;
            }
        }

        private static ImageList sImageList = null;

        static ProjectExplorer()
        {
            sImageList = new ImageList();
            sImageList.Images.Add(ImageResources.random_music_list);
            sImageList.Images.Add(ImageResources.sequential_music_list);
            sImageList.Images.Add(ImageResources.randomsounds);
            sImageList.Images.Add(ImageResources.parallelsounds);
            sImageList.Images.Add(ImageResources.aressmall);
            sImageList.Images.Add(ImageResources.random);
            sImageList.Images.Add(ImageResources.sequential);
            sImageList.Images.Add(ImageResources.parallel);
            sImageList.Images.Add(ImageResources.vierge);
            sImageList.Images.Add(ImageResources.base_cog_32);
            sImageList.Images.Add(ImageResources.SeriousWarning);

            sImageList.Images.Add(ImageResources.random_music_list_ref);
            sImageList.Images.Add(ImageResources.sequential_music_list_ref);
            sImageList.Images.Add(ImageResources.randomsounds_ref);
            sImageList.Images.Add(ImageResources.parallelsounds_ref);
            sImageList.Images.Add(ImageResources.aressmall);
            sImageList.Images.Add(ImageResources.random_ref);
            sImageList.Images.Add(ImageResources.sequential_ref);
            sImageList.Images.Add(ImageResources.parallel_ref);
            sImageList.Images.Add(ImageResources.vierge);
            sImageList.Images.Add(ImageResources.base_cog_32_ref);
            sImageList.Images.Add(ImageResources.SeriousWarning_ref);
        }

        public ProjectExplorer()
        {
            InitializeComponent();
            projectTree.ImageList = sImageList;
            ensureDefaultImageIndex(projectTree);
            RecreateTree();
            ElementChanges.Instance.AddListener(-1, ElementChanged);
        }

        private int m_Id;

        private static int s_NextId = 1;

        private System.Action m_AfterEditAction;
        private bool listenForContainerChanges = true;

#if !MONO
        protected override String GetPersistString()
        {
            return "ProjectExplorer";
        }
#endif

        public void SetProject(IProject project)
        {
            m_Project = project;
            RecreateTree();
        }

        public ContextMenuStrip EditContextMenu
        {
            get
            {
                ContextMenuStrip menu = SelectedNode != null ? SelectedNode.ContextMenuStrip : null;
                if (menu != null)
                {
                    UpdateContextMenuDueToPlaying(menu);
                    UpdateContextMenuDueToSelection(menu);
                    UpdateVisiblityDueToModeElement(menu);
                }
                return menu;
            }
        }

        public void MoveToElement(Object element)
        {
            TreeNode result = DoMoveToElement(projectTree.Nodes[0], element);
            if (result == null)
            {
                result = DoMoveToContainerElement(projectTree.Nodes[0], element);
            }
            if (result != null)
            {
                SelectedNode = result;
                result.EnsureVisible();
            }
        }

        private TreeNode DoMoveToContainerElement(TreeNode node, Object element)
        {
            Object tag = node.Tag;
            if (tag is IModeElement)
            {
                tag = ((IModeElement)tag).StartElement;
            }
            if (tag is IGeneralElementContainer)
            {
                foreach (IContainerElement containerElement in ((tag as IGeneralElementContainer).GetGeneralElements()))
                {
                    if (containerElement == element || containerElement.InnerElement == element)
                    {
                        return node;
                    }
                }
            }
            foreach (TreeNode subNode in node.Nodes)
            {
                TreeNode result = DoMoveToContainerElement(subNode, element);
                if (result != null)
                {
                    node.Expand();
                    return result;
                }
            }
            return null;
        }

        private TreeNode DoMoveToElement(TreeNode node, Object element)
        {
            if (node.Tag == element)
            {
                return node;
            }
            else if (node.Tag is IModeElement && ((node.Tag as IModeElement).StartElement == element))
            {
                return node;
            }
            else
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    TreeNode result = DoMoveToElement(subNode, element);
                    if (result != null)
                    {
                        node.Expand();
                        return result;
                    }
                }
            }
            return null;
        }

        private void RecreateTree()
        {
            m_AfterEditAction = null;
            m_ExportItems = null;
            m_Id = ++s_NextId;
            projectTree.BeginUpdate();
            projectTree.Nodes.Clear();
            if (m_Project != null)
            {
                TreeNode projectNode = new TreeNode(m_Project.Title);
                projectNode.Tag = m_Project;
                projectNode.SelectedImageIndex = projectNode.ImageIndex = 4;
                AddModeNodes(projectNode, m_Project);
                projectNode.ContextMenuStrip = projectContextMenu;
                projectTree.Nodes.Add(projectNode);
                projectNode.Expand();
            }
            else
            {
                projectTree.Nodes.Add(StringResources.NoOpenedProject);
            }
            projectTree.EndUpdate();
        }

        private void AddModeNodes(TreeNode projectNode, IProject project)
        {
            KeysConverter converter = new KeysConverter();
            foreach (IMode mode in project.GetModes())
            {
                TreeNode modeNode = new TreeNode(mode.GetNodeTitle());
                modeNode.SelectedImageIndex = modeNode.ImageIndex = 8;
                projectNode.Nodes.Add(modeNode);
                modeNode.Tag = mode;
                modeNode.ContextMenuStrip = modeContextMenu;
                foreach (IModeElement element in mode.GetElements())
                {
                    AddModeElement(modeNode, element);
                }
                modeNode.Expand();
            }
        }

        private void AddModeElement(TreeNode modeNode, IModeElement modeElement)
        {
            TreeNode node = CreateModeElementNode(modeElement);
            modeNode.Nodes.Add(node);
            IElement startElement = modeElement.StartElement;
            AddSubElements(node, startElement);
            node.Collapse();
        }

        private void AddSubElements(TreeNode node, IElement element)
        {
            if (element is IMacro)
            {
                return;
            }
            else if (element is IGeneralElementContainer)
            {
                AddSubElements(node, (element as IGeneralElementContainer).GetGeneralElements());
            }
            else if (element is IBackgroundSounds)
            {
                AddSubElements(node, (element as IBackgroundSounds).GetElements());
            }
            // do not follow references (too confusing)
        }

        private void AddSubElements(TreeNode parent, IList<IContainerElement> subElements)
        {
            foreach (IContainerElement subElement in subElements)
            {
                IElement innerElement = subElement.InnerElement;
                if (innerElement is IFileElement)
                {
                    continue;
                }
                TreeNode node = CreateElementNode(innerElement);
                parent.Nodes.Add(node);
                if (innerElement is IGeneralElementContainer)
                {
                    AddSubElements(node, (innerElement as IGeneralElementContainer).GetGeneralElements());
                }
                node.Collapse();
            }
        }

        private void AddSubElements(TreeNode parent, IList<IBackgroundSoundChoice> subElements)
        {
            foreach (IBackgroundSoundChoice subElement in subElements)
            {
                TreeNode node = CreateElementNode(subElement);
                parent.Nodes.Add(node);
            }
        }

        private TreeNode CreateModeElementNode(IModeElement modeElement)
        {
            TreeNode node = CreateElementNode(modeElement.StartElement);
            node.Text = modeElement.GetNodeTitle();
            node.Tag = modeElement;
            return node;
        }

        private ContextMenuStrip GetNodeContextMenu(IElement element)
        {
            if (element is IBackgroundSounds)
            {
                return bgSoundsContextMenu;
            }
            else if (element is IBackgroundSoundChoice)
            {
                return elementContextMenu;
            }
            else if (element is IRandomBackgroundMusicList)
            {
                return elementContextMenu;
            }
            else if (element is ISequentialBackgroundMusicList)
            {
                return elementContextMenu;
            }
            else if (element is IMacro)
            {
                return elementContextMenu;
            }
            else if (element is IGeneralElementContainer)
            {
                return containerContextMenu;
            }
            else if (element is IReferenceElement)
            {
                return elementContextMenu;
            }
            else
                return null;
        }

        private int GetNodeImageIndex(IElement element)
        {
            if (element is IBackgroundSounds)
            {
                return 3;
            }
            else if (element is IBackgroundSoundChoice)
            {
                return 2;
            }
            else if (element is IRandomBackgroundMusicList)
            {
                return 0;
            }
            else if (element is ISequentialBackgroundMusicList)
            {
                return 1;
            }
            else if (element is IMacro)
            {
                return 9;
            }
            else if (element is IGeneralElementContainer)
            {
                if (element is IElementContainer<IChoiceElement>)
                {
                    return 5;
                }
                else if (element is IElementContainer<ISequentialElement>)
                {
                    return 6;
                }
                else
                {
                    return 7;
                }
            }
            else if (element is IReferenceElement)
            {
                IElement referencedElement = Data.DataModule.ElementRepository.GetElement((element as IReferenceElement).ReferencedId);
                if (referencedElement != null)
                    return GetNodeImageIndex(referencedElement) + 11;
                else
                    return 10;
            }
            else
                return 0;
        }

        private TreeNode CreateElementNode(IElement element)
        {
            TreeNode node = new TreeNode(element.Title);
            node.Tag = element;
            node.ContextMenuStrip = GetNodeContextMenu(element);
            node.ImageIndex = node.SelectedImageIndex = GetNodeImageIndex(element);
            return node;
        }

        private static IElement GetElement(TreeNode node)
        {
            if (node.Tag is IModeElement)
            {
                return (node.Tag as IModeElement).StartElement;
            }
            else
            {
                return node.Tag as IElement;
            }
        }

        private IProject m_Project;

        private TreeNode SelectedNode
        {
            get
            {
                return projectTree.SelectedNode;
            }

            set
            {
                projectTree.SelectedNode = value;
                setKeyButton.Enabled = (value != null) && ((value.Tag is IMode) || (value.Tag is IModeElement));
            }
        }


        private bool cancelExpand = false;

        private Rectangle m_DragStartRect;
        private bool m_InDrag;

        private void projectTree_MouseDown(object sender, MouseEventArgs e)
        {
            cancelExpand = e.Clicks > 1;
            setKeyButton.Enabled = (SelectedNode != null) && ((SelectedNode.Tag is IMode) || (SelectedNode.Tag is IModeElement));
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Size dragSize = SystemInformation.DragSize;
                m_DragStartRect = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
                m_InDrag = true;
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameProject();
        }

        public void InitNewProject()
        {
            SelectedNode = projectTree.Nodes[0];
            AddMode(false);
            TreeNode modeNode = SelectedNode;
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    SelectedNode = modeNode;
                    RenameMode();
                };
            SelectedNode = projectTree.Nodes[0];
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
            if (e.Node == null)
                return;
            if (e.Node.Tag == m_Project)
            {
                if (e.Label != null)
                {
                    Actions.Actions.Instance.AddNew(new RenameProjectAction(e.Node, text), m_Project);
                }
            }
            else if (e.Node.Tag is IMode)
            {
                if (e.Label == null)
                {
                    e.Node.Text = (e.Node.Tag as IMode).GetNodeTitle();
                }
                else
                {
                    IMode mode = e.Node.Tag as IMode;
                    Actions.Actions.Instance.AddNew(new RenameModeAction(e.Node, text), m_Project);
                    e.CancelEdit = true; // the text is already changed by the action
                }
                // TODO: check for empty or equal titles, output warning
            }
            else if (e.Node.Tag is IModeElement)
            {
                if (e.Label == null)
                {
                    e.Node.Text = (e.Node.Tag as IModeElement).GetNodeTitle();
                }
                else
                {
                    IModeElement modeElement = e.Node.Tag as IModeElement;
                    Actions.Actions.Instance.AddNew(new RenameModeElementAction(e.Node, text), m_Project);
                    e.CancelEdit = true; // the text is already changed by the action
                }
            }
            else
            {
                if (e.Label != null)
                {
                    IElement element = e.Node.Tag as IElement;
                    Actions.Actions.Instance.AddNew(new RenameElementAction(e.Node, text), m_Project);
                }
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
            Actions.Actions.Instance.AddNew(new AddModeAction(m_Project, SelectedNode, out modeNode), m_Project);
            modeNode.ContextMenuStrip = modeContextMenu;
            modeNode.ImageIndex = modeNode.SelectedImageIndex = 8;
            SelectedNode = modeNode;
            projectTree.SelectedNode = modeNode;
            if (immediateRename)
            {
                RenameMode();
            }
        }

        private void RenameMode()
        {
            if (!(SelectedNode.Tag is IMode))
                return;
            projectTree.SelectedNode = SelectedNode;
            projectTree.LabelEdit = true;
            if (!SelectedNode.IsEditing)
            {
                SelectedNode.Text = (SelectedNode.Tag as IMode).Title;
                SelectedNode.BeginEdit();
            }
        }

        private void AddRandomPlaylist()
        {
            String name = StringResources.NewPlaylist;
            IRandomBackgroundMusicList element = DataModule.ElementFactory.CreateRandomBackgroundMusicList(name);
            if (SelectedNode.Tag is IMode)
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
                EditElement(GetElement(SelectedNode));
            };
            RenameElement();
        }

        private void AddSequentialPlaylist()
        {
            String name = StringResources.NewPlaylist;
            ISequentialBackgroundMusicList element = DataModule.ElementFactory.CreateSequentialBackgroundMusicList(name);
            if (SelectedNode.Tag is IMode)
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
                EditElement(GetElement(SelectedNode));
            };
            RenameElement();
        }

        private void AddBackgroundSounds()
        {
            String name = StringResources.NewBackgroundSounds;
            IBackgroundSounds element = DataModule.ElementFactory.CreateBackgroundSounds(name);
            if (SelectedNode.Tag is IMode)
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

        private void AddMacro()
        {
            String name = StringResources.NewMacro;
            IMacro element = DataModule.ElementFactory.CreateMacro(name);
            AddModeElement(element, name);
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    EditElement(GetElement(SelectedNode));
                };
            RenameElement();
        }

        private void AddSoundChoice(bool renameImmediately)
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            String name = StringResources.NewSoundChoice;
            TreeNode node;
            Actions.Actions.Instance.AddNew(new Actions.AddSoundChoiceAction(SelectedNode, 
                GetElement(SelectedNode) as IBackgroundSounds,
                name, CreateElementNode, out node), m_Project);
            SelectedNode.Expand();
            SelectedNode = node;
            if (renameImmediately)
            {
                m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    EditElement(GetElement(SelectedNode));
                };
                RenameElement();
            }
            listenForContainerChanges = oldListen;
        }

        private void AddParallelList()
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            String name = StringResources.NewParallelList;
            IElementContainer<IParallelElement> element = DataModule.ElementFactory.CreateParallelContainer(name);
            if (SelectedNode.Tag is IMode)
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
                EditElement(GetElement(SelectedNode));
            };
            RenameElement();
            listenForContainerChanges = oldListen;
        }

        private void AddSequentialList()
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            String name = StringResources.NewSequentialList;
            ISequentialContainer element = DataModule.ElementFactory.CreateSequentialContainer(name);
            if (SelectedNode.Tag is IMode)
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
                EditElement(GetElement(SelectedNode));
            };
            RenameElement();
            listenForContainerChanges = oldListen;
        }

        private void AddRandomList()
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            String name = StringResources.NewRandomList;
            IElementContainer<IChoiceElement> element = DataModule.ElementFactory.CreateChoiceContainer(name);
            if (SelectedNode.Tag is IMode)
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
                EditElement(GetElement(SelectedNode));
            };
            RenameElement();
            listenForContainerChanges = oldListen;
        }

        private void AddScenario()
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            String name = StringResources.NewScenario;
            IElementContainer<IParallelElement> element = DataModule.ElementFactory.CreateParallelContainer(name);
            AddModeElement(element, name);
            TreeNode scenarioNode = SelectedNode;
            IModeElement modeElement = scenarioNode.Tag as IModeElement;
            modeElement.Trigger = DataModule.ElementFactory.CreateNoTrigger();
            modeElement.Trigger.StopSounds = true;
            String name2 = StringResources.Music;
            IRandomBackgroundMusicList element2 = DataModule.ElementFactory.CreateRandomBackgroundMusicList(name2);
            AddContainerElement(element2);
            SelectedNode = scenarioNode;
            String name3 = StringResources.Sounds;
            IBackgroundSounds element3 = DataModule.ElementFactory.CreateBackgroundSounds(name3);
            AddContainerElement(element3);
            AddSoundChoice(false);
            TreeNode soundChoiceNode = SelectedNode;
            SelectedNode = scenarioNode;
            m_AfterEditAction = () =>
                {
                    m_AfterEditAction = null;
                    SelectedNode = soundChoiceNode;
                    RenameElement();
                };
            RenameElement();
            listenForContainerChanges = oldListen;
        }

        private void AddModeElement(IElement startElement, String title)
        {
            IModeElement modeElement = DataModule.ElementFactory.CreateModeElement(title, startElement);
            TreeNode node = CreateModeElementNode(modeElement);
            Actions.Actions.Instance.AddNew(new AddModeElementAction(SelectedNode, modeElement, node), m_Project);
            SelectedNode.Expand();
            SelectedNode = node;
        }

        private void AddContainerElement(IElement element)
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            TreeNode node;
            Actions.Actions.Instance.AddNew(new AddElementAction(SelectedNode,
                GetElement(SelectedNode) as IGeneralElementContainer, element, CreateElementNode, out node), m_Project);
            SelectedNode.Expand();
            SelectedNode = node;
            listenForContainerChanges = oldListen;
        }

        private void RenameElement()
        {
            projectTree.SelectedNode = SelectedNode;
            projectTree.LabelEdit = true;
            if (!SelectedNode.IsEditing)
            {
                SelectedNode.Text = (SelectedNode.Tag as IElement).Title;
                SelectedNode.BeginEdit();
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
            DeleteElements();
        }

        private void WithSelectedRoots(Action<TreeNode> action)
        {
            Dictionary<TreeNode, object> rootElements = new Dictionary<TreeNode, object>();
            // first add all selected to a set for quick search
            Dictionary<TreeNode, object> selectedElements = new Dictionary<TreeNode, object>();
            foreach (TreeNode node in projectTree.SelectedNodes)
                selectedElements.Add(node, null);
            foreach (TreeNode node in projectTree.SelectedNodes)
            {
                // walk to the roots; find the topmost root which is selected
                TreeNode root = node;
                TreeNode selRoot = node;
                while (root != null && root.Parent != null && root.Parent.Parent != null) // do not include the topmost root node
                {
                    if (selectedElements.ContainsKey(root.Parent))
                        selRoot = root.Parent;
                    root = root.Parent;
                }
                // if it wasn't already added from another element, add it
                if (!rootElements.ContainsKey(selRoot) && root.Parent != null)
                    rootElements.Add(selRoot, null);
            }
            // now take the action on all selected roots
            foreach (TreeNode rootElement in rootElements.Keys)
            {
                action(rootElement);
            }
        }

        private void DeleteElements()
        {
            // find for each node the topmost root which is also selected
            // only those nodes will be deleted
            WithSelectedRoots((TreeNode rootElement) =>
            {
                if (rootElement.Tag is IMode)
                    DeleteMode(rootElement);
                else
                    DeleteElement(rootElement);
            });
            m_ExportItems = null;
        }

        private void DeleteMode(TreeNode node)
        {
            if (node != null)
            {
                Actions.Actions.Instance.AddNew(new DeleteModeAction(node), m_Project);
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
                if ((System.Windows.Forms.Keys)keyCode == System.Windows.Forms.Keys.Delete)
                    keyCode = 0;
                Actions.Actions.Instance.AddNew(new Actions.SetModeKeyAction(SelectedNode, keyCode), m_Project);
            }
        }

        private void SelectModeElementKey()
        {
            int keyCode = 0;
            DialogResult result = Dialogs.KeyDialog.Show(this, out keyCode);
            if (result != DialogResult.Cancel)
            {
                IModeElement modeElement = SelectedNode.Tag as IModeElement;
                ITrigger trigger = null;
                if ((System.Windows.Forms.Keys)keyCode == System.Windows.Forms.Keys.Delete)
                {
                    trigger = Data.DataModule.ElementFactory.CreateNoTrigger();
                }
                else
                {
                    IKeyTrigger keyTrigger = Data.DataModule.ElementFactory.CreateKeyTrigger();
                    keyTrigger.TargetElementId = modeElement.Id;
                    keyTrigger.KeyCode = keyCode;
                    trigger = keyTrigger;
                }
                if (modeElement.Trigger != null && modeElement.Trigger.TriggerType == TriggerType.Key)
                {
                    IKeyTrigger oldTrigger = modeElement.Trigger as IKeyTrigger;
                    trigger.StopSounds = oldTrigger.StopSounds;
                    trigger.StopMusic = oldTrigger.StopMusic;
                }
                else
                {
                    trigger.StopMusic = trigger.StopSounds = false;
                }
                Actions.Actions.Instance.AddNew(new Actions.SetModeElementTriggerAction(modeElement, trigger), m_Project);
            }
        }

        private void ElementChanged(int elementId, ElementChanges.ChangeType changeType)
        {
            if (changeType == ElementChanges.ChangeType.TriggerChanged)
            {
                foreach (TreeNode node in projectTree.Nodes[0].Nodes)
                {
                    if (node.Tag is IMode)
                    {
                        foreach (TreeNode node2 in node.Nodes)
                        {
                            IModeElement modeElement = node2.Tag as IModeElement;
                            if (modeElement != null && modeElement.Id == elementId)
                            {
                                node2.Text = modeElement.GetNodeTitle();
                                return;
                            }
                        }
                    }
                }
            }
            else if (changeType == ElementChanges.ChangeType.Changed && listenForContainerChanges)
            {
                TreeNode node = FindNodeForElement(elementId, projectTree.Nodes[0]);
                if (node != null && GetElement(node) is IGeneralElementContainer && !(GetElement(node) is IMacro))
                {
                    node.Nodes.Clear();
                    AddSubElements(node, (GetElement(node) as IGeneralElementContainer).GetGeneralElements());
                }
                else if (node != null && GetElement(node) is IBackgroundSounds)
                {
                    node.Nodes.Clear();
                    AddSubElements(node, (GetElement(node) as IBackgroundSounds).GetElements());
                }
            }
        }

        private TreeNode FindNodeForElement(int elementId, TreeNode node)
        {
            if (node.Tag is IElement && (node.Tag as IElement).Id == elementId)
                return node;
            if (GetElement(node) != null && GetElement(node).Id == elementId)
                return node;
            foreach (TreeNode subNode in node.Nodes)
            {
                TreeNode foundNode = FindNodeForElement(elementId, subNode);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        private void EditModeElementTrigger()
        {
            IModeElement element = SelectedNode.Tag as IModeElement;
#if !MONO
            ElementEditors.Editors.ShowTriggerEditor(element, m_Project, DockPanel);
#else
            ElementEditors.Editors.ShowTriggerEditor(element, m_Project, MdiParent);
#endif
        }

        private void containerContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            UpdateVisiblityDueToModeElement(sender as ContextMenuStrip);
        }

        private void elementContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            UpdateContextMenuDueToSelection(sender as ContextMenuStrip);
            UpdateVisiblityDueToModeElement(sender as ContextMenuStrip);
        }

        private void addRandomPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddRandomPlaylist();
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DeleteElements();
        }

        private void DeleteElement(TreeNode node)
        {
            bool oldListen = listenForContainerChanges;
            listenForContainerChanges = false;
            if (node.Parent.Tag is IMode)
            {
                Actions.Actions.Instance.AddNew(new DeleteModeElementAction(node), m_Project);
            }
            else if (node.Parent.Tag is IBackgroundSounds)
            {
                Actions.Actions.Instance.AddNew(new DeleteBackgroundSoundChoiceAction(node), m_Project);
            }
            else
            {
                Actions.Actions.Instance.AddNew(new DeleteElementAction(node), m_Project);
            }
            listenForContainerChanges = oldListen;
        }

        private void EditElement(IElement element)
        {
#if !MONO
            ElementEditors.Editors.ShowEditor(element, null, m_Project, DockPanel);
#else
            ElementEditors.Editors.ShowEditor(element, null, m_Project, MdiParent);
#endif
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DeleteElements();
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
            EditModeElementTrigger();
        }

        private void selectKeyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditModeElementTrigger();
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
            DefaultNodeAction();
        }

        private void DefaultNodeAction()
        {
            if (SelectedNode != null)
            {
                if (SelectedNode.Tag is IMode)
                {
                    SelectModeKey();
                }
                else if (SelectedNode.Tag is IModeElement && GetElement(SelectedNode) is IBackgroundSounds)
                {
                    SelectModeElementKey();
                }
                else if (SelectedNode.Tag is IElement)
                {
                    EditElement(GetElement(SelectedNode));
                }
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditElement(GetElement(SelectedNode));
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditElement(GetElement(SelectedNode));
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
            DeleteElements();
        }

        private void addSoundChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddSoundChoice(true);
        }

        private void bgSoundsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            UpdateContextMenuDueToPlaying(sender as ContextMenuStrip);
            UpdateContextMenuDueToSelection(sender as ContextMenuStrip);
            UpdateVisiblityDueToModeElement(sender as ContextMenuStrip);
        }

        private void selectKeyToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            EditModeElementTrigger();
        }

        private IElement m_PlayedElement;

        private void playButton_Click(object sender, EventArgs e)
        {
            IElement element = GetElement(SelectedNode);
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
                if (projectTree.SelectedNodes.Count != 1)
                    return false;
                if (SelectedNode == null)
                    return false;
                IElement element = GetElement(SelectedNode);
                if (element == null)
                    return false;
                if (Actions.Playing.Instance.IsElementOrSubElementPlaying(element))
                    return false;
                if (element is IMacro || ((element is IModeElement) && (element as IModeElement).StartElement is IMacro))
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
            if (SelectedNode != null)
            {
                IElement element = GetElement(SelectedNode);
                if (element != null)
                {
                    Actions.ElementChanges.Instance.AddListener(element.Id, UpdateAfterElementChange);
                }
            }
        }

        
        private void RemoveChangeListener()
        {
            if (SelectedNode != null)
            {
                IElement element = GetElement(SelectedNode);
                if (element != null)
                {
                    Actions.ElementChanges.Instance.RemoveListener(element.Id, UpdateAfterElementChange);
                }
            }
        }

        private void UpdateAfterElementChange(int id, Actions.ElementChanges.ChangeType changeType)
        {
            if (SelectedNode != null)
            {
                IElement element = GetElement(SelectedNode);
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
            UpdateContextMenuDueToSelection(sender as ContextMenuStrip);
        }

        private static bool IsNodePlaying(TreeNode node)
        {
            IElement element = GetElement(node);
            if (element != null && Actions.Playing.Instance.IsElementOrSubElementPlaying(element))
            {
                return true;
            }
            else if (node.Tag is IMode)
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
            bool disable = (SelectedNode != null && IsNodePlaying(SelectedNode));
            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item is ToolStripMenuItem)
                    item.Enabled = !disable || 
                        ((item.Tag != null) && item.Tag.ToString().Contains("DuringPlay"));
            }
        }

        private void UpdateContextMenuDueToSelection(ContextMenuStrip contextMenu)
        {
            bool disable = projectTree.SelectedNodes.Count > 1;
            bool disableAll = false;
            if (disable) foreach (TreeNode node in projectTree.SelectedNodes)
            {
                if (node.Parent == null) // project node
                {
                    disableAll = true;
                    break;
                }
            }
            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item is ToolStripMenuItem)
                    item.Enabled = !disableAll && (!disable ||
                        ((item.Tag != null) && item.Tag.ToString().Contains("MultipleNodes")));
                if (item.Tag != null && item.Tag.ToString().Contains("Paste"))
                {
                    item.Enabled = item.Enabled && Clipboard.ContainsData(DataFormats.GetFormat("AresProjectExplorerElements").Name);
                }
                if (item.Tag != null && item.Tag.ToString().Contains("PasteLink"))
                {
                    item.Enabled = item.Enabled && m_ExportItems != null;
                }
                if (item.Tag != null && item.Tag.ToString().Contains("OnlyMusicLists"))
                {
                    if (!(projectTree.SelectedNode.Tag is IMusicList) && (!(projectTree.SelectedNode.Tag is IModeElement) || !((projectTree.SelectedNode.Tag as IModeElement).StartElement is IMusicList)))
                    {
                        item.Enabled = false;
                        item.Visible = false;
                    }
                    else if (disable)
                    {
                        item.Enabled = false;
                        item.Visible = true;
                    }
                    else
                    {
                        item.Enabled = true;
                        item.Visible = true;
                    }
                }
            }
        }

        private void UpdateVisiblityDueToModeElement(ContextMenuStrip contextMenu)
        {
            bool visible = SelectedNode != null && SelectedNode.Tag is IModeElement;
            foreach (ToolStripItem item in contextMenu.Items)
            {
                if (item.Tag != null && item.Tag.ToString().Contains("OnlyMode"))
                {
                    item.Visible = visible;
                }
            }
        }

        private void projectTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.F2)
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode == projectTree.Nodes[0])
                        RenameProject();
                    else if (SelectedNode.Tag is IMode)
                        RenameMode();
                    else
                        RenameElement();
                }
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.F3)
            {
                if (SelectedNode != null)
                {
                    if (SelectedNode.Tag is IMode)
                    {
                        SelectModeKey();
                    }
                    else if (SelectedNode.Tag is IModeElement)
                    {
                        SelectModeElementKey();
                    }
                }
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Return)
            {
                DefaultNodeAction();
            }
            else if (e.KeyCode == System.Windows.Forms.Keys.Delete)
            {
                TreeNode parent = SelectedNode.Parent;
                DeleteElements();
                if (parent != null)
                {
                    SelectedNode = parent;
                }
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.C)
            {
                CopyElements();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.X)
            {
                CutElements();
            }
            else if (e.Control && e.KeyCode == System.Windows.Forms.Keys.V)
            {
                if (projectTree.SelectedNodes.Count == 1 && !(projectTree.SelectedNode.Tag is IBackgroundSoundChoice))
                {
                    PasteElements();
                }
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            SelectModeElementKey();
        }

        private void setKeyButton_Click(object sender, EventArgs e)
        {
            if (SelectedNode.Tag is IMode)
            {
                SelectModeKey();
            }
            else
            {
                SelectModeElementKey();
            }
        }

        private void ExportElements()
        {
            List<IXmlWritable> exportItems = new List<IXmlWritable>();
            // export only root nodes
            WithSelectedRoots((TreeNode rootElement) =>
            {
                if (rootElement.Tag is IXmlWritable)
                    exportItems.Add(rootElement.Tag as IXmlWritable);
            });
            if (exportItems.Count == 0)
                return;
            if (exportDialog.ShowDialog(Parent) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (exportDialog.FileName.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String tempFileName = System.IO.Path.GetTempFileName() + ".ares";
                        Data.DataModule.ProjectManager.ExportElements(exportItems, tempFileName);
                        Ares.CommonGUI.ProgressMonitor monitor = new Ares.CommonGUI.ProgressMonitor(this, StringResources.Exporting);
                        Ares.ModelInfo.Exporter.Export(monitor, exportItems, tempFileName, exportDialog.FileName, error =>
                        {
                            monitor.Close();
                            if (error != null)
                            {
                                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ExportError, error.Message), StringResources.Ares,
                                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                        });
                    }
                    else
                    {
                        Data.DataModule.ProjectManager.ExportElements(exportItems, exportDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.SaveError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CutElements()
        {
            CopyElements();
            DeleteElements();
        }

        [Serializable]
        private class ClipboardElements
        {
            public String SerializedForm { get; set; }
            public int SourceId { get; set; }
        }

        private void CopyElements()
        {
            List<IXmlWritable> exportItems = new List<IXmlWritable>();
            // export only root nodes
            WithSelectedRoots((TreeNode rootElement) =>
            {
                if (rootElement.Tag is IXmlWritable)
                    exportItems.Add(rootElement.Tag as IXmlWritable);
            });
            if (exportItems.Count == 0)
                return;
            StringBuilder serializedForm = new StringBuilder();
            Data.DataModule.ProjectManager.ExportElements(exportItems, serializedForm);
            ClipboardElements cpElements = new ClipboardElements() { SerializedForm = serializedForm.ToString(), SourceId = m_Id };
            Clipboard.SetData(DataFormats.GetFormat("AresProjectExplorerElements").Name, cpElements);
            m_ExportItems = exportItems;
        }

        public void PasteElements()
        {
            String format = DataFormats.GetFormat("AresProjectExplorerElements").Name;
            if (!Clipboard.ContainsData(format))
                return;
            ClipboardElements cpElements = (ClipboardElements)Clipboard.GetData(format);
            if (cpElements == null)
                return;
            String serializedForm = cpElements.SerializedForm;

            IList<IXmlWritable> elements = Data.DataModule.ProjectManager.ImportElementsFromString(serializedForm);
            TreeNode parentNode = SelectedNode;
            bool hasEnabledElements = false;
            foreach (IXmlWritable element in elements)
            {
                bool enabled = IsImportPossible(parentNode.Tag, element);
                if (enabled)
                {
                    hasEnabledElements = true;
                    AddImportedElement(parentNode, parentNode.Tag, element);
                }
            }
            if (!hasEnabledElements)
            {
                MessageBox.Show(this, StringResources.NoPasteableElements, StringResources.Ares, MessageBoxButtons.OK);
                return;
            }
            else
            {
                Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
            }
        }

        private void PasteElementsAsLink()
        {
            String format = DataFormats.GetFormat("AresProjectExplorerElements").Name;
            if (!Clipboard.ContainsData(format))
                return;
            ClipboardElements cpElements = (ClipboardElements)Clipboard.GetData(format);
            if (cpElements == null)
                return;
            if (m_ExportItems == null)
                return;
            if (cpElements.SourceId != this.m_Id)
            {
                return;
            }
            TreeNode parentNode = SelectedNode;
            List<IXmlWritable> roots = m_ExportItems;
            foreach (IXmlWritable element in roots)
            {
                bool enabled = IsImportPossible(parentNode.Tag, element);
                if (enabled)
                {
                    if (element is IReferenceElement)
                    {
                        IElement referencedElement = DataModule.ElementRepository.GetElement((element as IReferenceElement).ReferencedId);
                        if (referencedElement != null)
                        {
                            AddLink(parentNode, parentNode.Tag, referencedElement);
                        }
                    }
                    else
                    {
                        AddLink(parentNode, parentNode.Tag, element);
                    }
                }
            }
        }

        private void SelectTagsForElement()
        {
            IMusicList musicList = SelectedNode.Tag as IMusicList;
            if (musicList == null)
            {
                IModeElement modeElement = SelectedNode.Tag as IModeElement;
                if (modeElement != null)
                {
                    musicList = modeElement.StartElement as IMusicList;
                }
            }
            if (musicList != null)
            {
                var fileLists = new Ares.ModelInfo.FileLists(Ares.ModelInfo.DuplicateRemoval.PathBased);
                var tempList = new List<IXmlWritable>();
                tempList.Add(musicList as IXmlWritable);
                var files = fileLists.GetAllFiles(tempList);
                var pathList = new List<String>();
                foreach (IFileElement element in files)
                {
                    pathList.Add(element.FilePath);
                }

                int languageId = m_Project.TagLanguageId;
                if (languageId == -1)
                {
                    try
                    {
                        languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                    }
                    catch (Ares.Tags.TagsDbException ex)
                    {
                        MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                Dialogs.FileTagsDialog dialog = new Dialogs.FileTagsDialog();
                dialog.LanguageId = languageId;
                dialog.SetFiles(pathList);
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ChangeTagsForFiles(pathList, dialog.AddedTags, dialog.RemovedTags, languageId);
                    m_Project.TagLanguageId = dialog.LanguageId;
                }
            }
        }

        private void ChangeTagsForFiles(List<String> files, HashSet<int> addedTags, HashSet<int> removedTags, int languageId)
        {
            try
            {
                Actions.Actions.Instance.AddNew(new Actions.ChangeFileTagsAction(files, addedTags, removedTags, languageId), m_Project);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsImportPossible(object parentElement, IXmlWritable element)
        {
            if (element is IReferenceElement)
            {
                IElement referencedElement = Data.DataModule.ElementRepository.GetElement((element as IReferenceElement).ReferencedId);
                if (referencedElement != null)
                    return IsImportPossible(parentElement, referencedElement);
                else
                    return false;
            }
            if (parentElement is IProject)
            {
                return element is IMode;
            }
            else if (parentElement is IMode)
            {
                if (element is IModeElement || element is IGeneralElementContainer)
                {
                    // not individual sound choices
                    return !(element is IBackgroundSoundChoice);
                }
                else
                {
                    return false;
                }
            }
            else if (parentElement is IModeElement)
            {
                return IsImportPossible((parentElement as IModeElement).StartElement, element);
            }
            else if (parentElement is IBackgroundSounds)
            {
                return element is IBackgroundSoundChoice;
            }
            else if (parentElement is IGeneralElementContainer)
            {
                return !(element is IMode) && !(element is IBackgroundSoundChoice);
            }
            else
            {
                return false;
            }
        }

        private void AddLink(TreeNode parent, object parentElement, IXmlWritable element)
        {
            if (element == null)
                return;

            if (parentElement is IProject)
            {
                // should not happen
                return;
            }
            else if (parentElement is IMode)
            {
                if (element is IModeElement)
                {
                    AddLink(parent, parentElement, (element as IModeElement).StartElement);
                }
                else
                {
                    IElement newLink = null;
                    if (element is IReferenceElement)
                    {
                        newLink = DataModule.ElementFactory.CreateReferenceElement((element as IReferenceElement).ReferencedId);
                    }
                    else
                    {
                        newLink = DataModule.ElementFactory.CreateReferenceElement((element as IElement).Id);
                    }
                    IModeElement modeElement = DataModule.ElementFactory.CreateModeElement(newLink.Title, newLink);
                    TreeNode node = CreateModeElementNode(modeElement);
                    Actions.Actions.Instance.AddNew(new AddModeElementAction(parent, modeElement, node), m_Project);
                    AddSubElements(node, modeElement.StartElement); // though there shouldn't be any sub-elements in current design
                }
            }
            else if (parentElement is IModeElement)
            {
                AddLink(parent, (parentElement as IModeElement).StartElement, element);
            }
            else if (parentElement is IBackgroundSounds)
            {
                // not supported yet
                return;
            }
            else if (parentElement is IGeneralElementContainer)
            {
                IElement elem = element is IModeElement ? (element as IModeElement).StartElement : element as IElement;
                IElement newLink = null;
                if (elem is IReferenceElement)
                {
                    newLink = DataModule.ElementFactory.CreateReferenceElement((elem as IReferenceElement).ReferencedId);
                }
                else
                {
                    newLink = DataModule.ElementFactory.CreateReferenceElement((elem as IElement).Id);
                }
                bool oldListen = listenForContainerChanges;
                listenForContainerChanges = false;
                TreeNode newNode;
                Actions.Actions.Instance.AddNew(new AddElementAction(parent, parentElement as IGeneralElementContainer, newLink,
                    CreateElementNode, out newNode), m_Project);
                AddSubElements(newNode, newLink); // though there shouldn't be any sub-elements in current design
                listenForContainerChanges = oldListen;
            }
        }

        private void AddImportedElement(TreeNode parent, object parentElement, IXmlWritable element)
        {
            if (parentElement is IProject)
            {
                TreeNode modeNode;
                IMode mode = element as IMode;
                Actions.Actions.Instance.AddNew(new AddModeAction(SelectedNode, mode, out modeNode), m_Project);
                modeNode.ContextMenuStrip = modeContextMenu;
                modeNode.ImageIndex = modeNode.SelectedImageIndex = 8;
                foreach (IModeElement modeElement in mode.GetElements())
                {
                    AddModeElement(modeNode, modeElement);
                }
            }
            else if (parentElement is IMode)
            {
                if (element is IModeElement)
                {
                    IModeElement modeElement = element as IModeElement;
                    TreeNode newNode = CreateModeElementNode(modeElement);
                    Actions.Actions.Instance.AddNew(new AddModeElementAction(parent, modeElement, newNode), m_Project);
                    AddSubElements(newNode, modeElement.StartElement);
                }
                else
                {
                    IModeElement modeElement = DataModule.ElementFactory.CreateModeElement(element.Title, element as IElement);
                    TreeNode node = CreateModeElementNode(modeElement);
                    Actions.Actions.Instance.AddNew(new AddModeElementAction(parent, modeElement, node), m_Project);
                    AddSubElements(node, modeElement.StartElement);
                }
            }
            else if (parentElement is IModeElement)
            {
                AddImportedElement(parent, (parentElement as IModeElement).StartElement, element);
            }
            else if (parentElement is IBackgroundSounds)
            {
                bool oldListen = listenForContainerChanges;
                listenForContainerChanges = false;
                TreeNode newNode;
                Actions.Actions.Instance.AddNew(new AddSoundChoiceAction(parent, parentElement as IBackgroundSounds, element,
                    CreateElementNode, out newNode), m_Project);
                listenForContainerChanges = oldListen;
            }
            else if (parentElement is IGeneralElementContainer)
            {
                IElement elem = element is IModeElement ? (element as IModeElement).StartElement : element as IElement;
                bool oldListen = listenForContainerChanges;
                listenForContainerChanges = false;
                TreeNode newNode;
                Actions.Actions.Instance.AddNew(new AddElementAction(parent, parentElement as IGeneralElementContainer, elem,
                    CreateElementNode, out newNode), m_Project);
                AddSubElements(newNode, elem);
                listenForContainerChanges = oldListen;
            }
        }

        private void Import()
        {
            if (importDialog.ShowDialog(Parent) == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    String fileName = importDialog.FileName;
                    if (fileName.EndsWith(".apkg", StringComparison.InvariantCultureIgnoreCase))
                    {
                        String tempFileName = System.IO.Path.GetTempFileName() + ".ares";
                        Ares.CommonGUI.ProgressMonitor monitor = new Ares.CommonGUI.ProgressMonitor(this, StringResources.Importing);
                        Ares.ModelInfo.Importer.Import(monitor, fileName, tempFileName, (error, cancelled) =>
                        {
                            monitor.Close();
                            if (error != null)
                            {
                                System.Windows.Forms.MessageBox.Show(String.Format(StringResources.ImportError, error.Message), StringResources.Ares,
                                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                            else if (!cancelled)
                            {
                                DoImport(tempFileName);
                            }
                        });
                    }
                    else
                    {
                        DoImport(fileName);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DoImport(String fileName)
        {
            try 
            {
                IList<IXmlWritable> elements = Data.DataModule.ProjectManager.ImportElements(fileName);
                TreeNode parentNode = SelectedNode;
                List<Dialogs.ImportElement> importElements = new List<Dialogs.ImportElement>();
                bool hasEnabledElements = false;
                foreach (IXmlWritable element in elements)
                {
                    bool enabled = IsImportPossible(parentNode.Tag, element);
                    importElements.Add(new Dialogs.ImportElement(element, enabled));
                    if (enabled)
                    {
                        hasEnabledElements = true;
                    }
                }
                if (!hasEnabledElements)
                {
                    MessageBox.Show(this, StringResources.NoImportableElements, StringResources.Ares, MessageBoxButtons.OK);
                    return;
                }
                Dialogs.ImportDialog dialog = new Dialogs.ImportDialog();
                dialog.SetElements(importElements);
                if (dialog.ShowDialog(Parent) == System.Windows.Forms.DialogResult.OK)
                {
                    foreach (Dialogs.ImportElement importElement in importElements)
                    {
                        if (importElement.Selected)
                        {
                            AddImportedElement(parentNode, parentNode.Tag, importElement.Element);
                        }
                    }
                }
                Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
            }
            catch (Exception e)
            {
                MessageBox.Show(this, String.Format(StringResources.LoadError, e.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportElements();
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void exportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ExportElements();
        }

        private void importToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void exportToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ExportElements();
        }

        private void importToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void exportToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            ExportElements();
        }

        private void importToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Import();
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            CutElements();
        }

        private void toolStripMenuItem5_Click_1(object sender, EventArgs e)
        {
            CopyElements();
        }

        private void toolStripMenuItem3_Click_1(object sender, EventArgs e)
        {
            PasteElements();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            PasteElements();
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            CopyElements();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            CutElements();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            PasteElements();
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            CopyElements();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            CutElements();
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            PasteElements();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            CopyElements();
        }

        private void toolStripMenuItem11_Click_1(object sender, EventArgs e)
        {
            CutElements();
        }

        private void projectContextMenu_Opening(object sender, CancelEventArgs e)
        {
            ContextMenuStrip menu = SelectedNode != null ? SelectedNode.ContextMenuStrip : null;
            if (menu == null)
                return;

            foreach (ToolStripItem item in menu.Items)
            {
                if (item.Tag != null && item.Tag.ToString().Contains("Paste"))
                {
                    item.Enabled = item.Enabled && Clipboard.ContainsData(DataFormats.GetFormat("AresProjectExplorerElements").Name);
                }
                if (item.Tag != null && item.Tag.ToString().Contains("PasteLink"))
                {
                    item.Enabled = item.Enabled && m_ExportItems != null;
                }
            }

        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteElements();
        }

        private bool m_DragStartedHere = false;
        private List<IXmlWritable> m_ExportItems = null;

        private void projectTree_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_InDrag && m_DragStartRect != null && !m_DragStartRect.Contains(e.X, e.Y))
            {
                m_ExportItems = new List<IXmlWritable>();
                List<TreeNode> exportNodes = new List<TreeNode>();
                // export only root nodes
                WithSelectedRoots((TreeNode rootElement) =>
                {
                    if (rootElement.Tag is IXmlWritable)
                    {
                        m_ExportItems.Add(rootElement.Tag as IXmlWritable);
                        exportNodes.Add(rootElement);
                    }
                });
                if (m_ExportItems.Count == 0)
                    return;
                StringBuilder serializedForm = new StringBuilder();
                Data.DataModule.ProjectManager.ExportElements(m_ExportItems, serializedForm);
                ClipboardElements cpElements = new ClipboardElements() { SerializedForm = serializedForm.ToString() };
                m_DragStartedHere = true;
                DragDropEffects effects = DoDragDrop(cpElements, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
                m_DragStartedHere = false;
                if (effects == DragDropEffects.Move)
                {
                    foreach (TreeNode rootNode in exportNodes)
                    {
                        if (rootNode.Tag is IMode)
                            DeleteMode(rootNode);
                        else
                            DeleteElement(rootNode);
                    }
                }
                m_ExportItems = null;
                m_InDrag = false;
            }

        }

        private void projectTree_MouseUp(object sender, MouseEventArgs e)
        {
            m_InDrag = false;
        }

        private void projectTree_DragEnter(object sender, DragEventArgs e)
        {
            CheckAllowedDrop(e);
        }

        private void CheckAllowedDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ClipboardElements)))
            {
                TreeNode node = projectTree.GetNodeAt(projectTree.PointToClient(new Point(e.X, e.Y)));
                if (node == null || node.Tag is IBackgroundSoundChoice)
                {
                    e.Effect = DragDropEffects.None;
                }
                else if (((e.KeyState & 8) == 8) && ((e.KeyState & 4) == 4) && ((e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link) 
                    && !(node.Tag is IProject) && !(node.Tag is IBackgroundSounds) && !((node.Tag is IModeElement) && ((IModeElement)node.Tag).StartElement is IBackgroundSounds))
                {
                    e.Effect = DragDropEffects.Link;
                }
                else if (((e.KeyState & 8) == 8) && ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    e.Effect = DragDropEffects.Move;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void projectTree_DragOver(object sender, DragEventArgs e)
        {
            CheckAllowedDrop(e);
        }

        private void projectTree_DragDrop(object sender, DragEventArgs e)
        {
                TreeNode node = projectTree.GetNodeAt(projectTree.PointToClient(new Point(e.X, e.Y)));
                if (node == null || node.Tag is IBackgroundSoundChoice)
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                if (!e.Data.GetDataPresent(typeof(ClipboardElements)))
                {
                    e.Effect = DragDropEffects.None;
                    return;
                }

                ClipboardElements cpElements = (ClipboardElements)e.Data.GetData(typeof(ClipboardElements));
                if (cpElements == null)
                    return;
                String serializedForm = cpElements.SerializedForm;

                IList<IXmlWritable> elements = Data.DataModule.ProjectManager.ImportElementsFromString(serializedForm);
                TreeNode parentNode = node;
                bool hasEnabledElements = false;
                if (m_DragStartedHere && e.Effect == DragDropEffects.Link)
                {
                    foreach (IXmlWritable element in m_ExportItems)
                    {
                        bool enabled = IsImportPossible(parentNode.Tag, element);
                        if (enabled)
                        {
                            hasEnabledElements = true;
                            if (element is IReferenceElement)
                            {
                                IElement referencedElement = DataModule.ElementRepository.GetElement((element as IReferenceElement).ReferencedId);
                                if (referencedElement != null)
                                {
                                    AddLink(parentNode, parentNode.Tag, referencedElement);
                                }
                            }
                            else
                            {
                                AddLink(parentNode, parentNode.Tag, element);
                            }
                        }
                    }
                }
                else
                {
                    foreach (IXmlWritable element in elements)
                    {
                        bool enabled = IsImportPossible(parentNode.Tag, element);
                        if (enabled)
                        {
                            hasEnabledElements = true;
                            AddImportedElement(parentNode, parentNode.Tag, element);
                        }
                    }
                }
                if (!hasEnabledElements)
                {
                    MessageBox.Show(this, StringResources.NoDroppableElements, StringResources.Ares, MessageBoxButtons.OK);
                    e.Effect = DragDropEffects.None;
                    return;
                }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            AddMacro();
        }

        private void toolStripMenuItem15_Click(object sender, EventArgs e)
        {
            PasteElementsAsLink();
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            PasteElementsAsLink();
        }

        private void tagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectTagsForElement();
        }

    }
}
