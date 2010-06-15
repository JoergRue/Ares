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
                return m_SelectedNode != null ? m_SelectedNode.ContextMenuStrip : null;
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
        }

        private void AddSubElements(TreeNode parent, IList<Ares.Data.IElement> subElements)
        {
            foreach (Ares.Data.IElement subElement in subElements)
            {
                Ares.Data.IElement innerElement = (subElement as Ares.Data.IContainerElement).InnerElement;
                if (innerElement is Ares.Data.IBackgroundSoundChoice)
                {
                    continue;
                }
                else if (innerElement is Ares.Data.IFileElement)
                {
                    continue;
                }
                TreeNode node = CreateElementNode(innerElement);
                parent.Nodes.Add(node);
                if (innerElement is Ares.Data.IGeneralElementContainer)
                {
                    AddSubElements(node, (subElement as Ares.Data.IGeneralElementContainer).GetGeneralElements());
                }
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

        private Ares.Data.IElement GetElement(TreeNode node)
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

        private void projectTree_MouseDown(object sender, MouseEventArgs e)
        {
            m_SelectedNode = projectTree.GetNodeAt(e.X, e.Y);
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddMode();
        }

        private void AddMode()
        {
            TreeNode modeNode;
            Actions.Actions.Instance.AddNew(new AddModeAction(m_SelectedNode, out modeNode));
            modeNode.ContextMenuStrip = modeContextMenu;
            m_SelectedNode = modeNode;
            projectTree.SelectedNode = modeNode;
            RenameMode();
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
        }

        private void AddModeElement(Ares.Data.IElement startElement, String title)
        {
            Ares.Data.IModeElement modeElement = Ares.Data.DataModule.ElementFactory.CreateModeElement(title, startElement);
            TreeNode node = CreateModeElementNode(modeElement);
            Actions.Actions.Instance.AddNew(new AddModeElementAction(m_SelectedNode, modeElement, node));
            m_SelectedNode.Expand();
            m_SelectedNode = node;
            RenameElement();
        }

        private void AddContainerElement(Ares.Data.IElement element)
        {
            TreeNode node;
            Actions.Actions.Instance.AddNew(new AddElementAction(m_SelectedNode, element, CreateElementNode, out node));
            m_SelectedNode.Expand();
            m_SelectedNode = node;
            RenameElement();
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


    }
}
