using System;
using Ares.Data;
using System.Windows.Forms;

namespace Ares.Editor.Actions
{
    public static class KeyDisplayExtensions
    {
        public static string GetNodeTitle(this IMode mode)
        {
            KeysConverter converter = new KeysConverter();
            return mode.Title + " (" + (mode.KeyCode != 0 ? converter.ConvertToString((Keys)mode.KeyCode) : StringResources.NoKey) + ")";
        }

        public static string GetNodeTitle(this IModeElement modeElement)
        {
            ITrigger trigger = modeElement.Trigger;
            if (trigger != null && trigger.TriggerType == TriggerType.Key)
            {
                KeysConverter converter = new KeysConverter();
                int keyCode = (trigger as IKeyTrigger).KeyCode;
                return modeElement.Title + " (" + (keyCode != 0 ? converter.ConvertToString((Keys)keyCode) : StringResources.NoKey) + ")";
            }
            else
            {
                // at first, as long as there are no other triggers
                return modeElement.Title + " (" + StringResources.NoKey + ")";
            }
        }
    }

    public class RenameProjectAction : Action
    {
        public RenameProjectAction(TreeNode node, String newName)
        {
            m_Node = node;
            m_OldName = (node.Tag as IProject).Title;
            m_NewName = newName;
        }

        public override void Do()
        {
            m_Node.Text = m_NewName;
            (m_Node.Tag as IProject).Title = m_NewName;
        }

        public override void Undo()
        {
            m_Node.Text = m_OldName;
            (m_Node.Tag as IProject).Title = m_OldName;
        }

        private TreeNode m_Node;
        private String m_OldName;
        private String m_NewName;
    }

    public class RenameModeAction : Action
    {
        public RenameModeAction(TreeNode node, String newName)
        {
            m_Node = node;
            m_OldName = (node.Tag as IMode).Title;
            m_NewName = newName;
        }

        public override void Do()
        {
            IMode mode = m_Node.Tag as IMode;
            mode.Title = m_NewName;
            m_Node.Text = mode.GetNodeTitle();
        }

        public override void Undo()
        {
            IMode mode = m_Node.Tag as IMode;
            mode.Title = m_OldName;
            m_Node.Text = mode.GetNodeTitle();
        }

        private TreeNode m_Node;
        private String m_OldName;
        private String m_NewName;
    }

    public class AddModeAction : Action
    {
        public AddModeAction(TreeNode projectNode, out TreeNode modeNode)
        {
            String name = StringResources.NewMode;
            m_ProjectNode = projectNode;
            IMode mode = (m_ProjectNode.Tag as IProject).AddMode(name);
            m_ModeNode = new TreeNode(name);
            m_ModeNode.Tag = mode;
            m_ProjectNode.Nodes.Add(m_ModeNode);
            m_Index = m_ProjectNode.Nodes.Count - 1;
            modeNode = m_ModeNode;
            Undo();
        }

        public override void Do()
        {
            (m_ProjectNode.Tag as IProject).InsertMode(m_Index, m_ModeNode.Tag as IMode);
            m_ProjectNode.Nodes.Insert(m_Index, m_ModeNode);
            m_ProjectNode.Expand();
        }

        public override void Undo()
        {
            (m_ProjectNode.Tag as IProject).RemoveMode(m_ModeNode.Tag as IMode);
            m_ProjectNode.Nodes.Remove(m_ModeNode);
        }

        private TreeNode m_ProjectNode;
        private TreeNode m_ModeNode;
        private int m_Index;
    }

    public class DeleteModeAction : Action
    {
        public DeleteModeAction(TreeNode modeNode)
        {
            m_ProjectNode = modeNode.Parent;
            m_ModeNode = modeNode;
            m_Mode = modeNode.Tag as IMode;
            m_Index = m_ProjectNode.Nodes.IndexOf(modeNode);
        }

        public override void Do()
        {
            (m_ProjectNode.Tag as IProject).RemoveMode(m_Mode);
            m_ProjectNode.Nodes.Remove(m_ModeNode);
        }

        public override void Undo()
        {
            (m_ProjectNode.Tag as IProject).InsertMode(m_Index, m_Mode);
            m_ProjectNode.Nodes.Insert(m_Index, m_ModeNode);
        }

        private IMode m_Mode;
        private TreeNode m_ProjectNode, m_ModeNode;
        private int m_Index;
    }

    public class SetModeKeyAction : Action
    {
        public SetModeKeyAction(TreeNode modeNode, int newKey)
        {
            m_Node = modeNode;
            m_OldKey = (m_Node.Tag as IMode).KeyCode;
            m_NewKey = newKey;
        }

        public override void Do()
        {
            IMode mode = m_Node.Tag as IMode;
            mode.KeyCode = m_NewKey;
            m_Node.Text = mode.GetNodeTitle();
        }

        public override void Undo()
        {
            IMode mode = m_Node.Tag as IMode;
            mode.KeyCode = m_OldKey;
            m_Node.Text = mode.GetNodeTitle();
        }

        private TreeNode m_Node;
        private int m_OldKey;
        private int m_NewKey;
    }
}
