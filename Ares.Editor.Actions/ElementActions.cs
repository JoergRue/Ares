using System;
using System.Windows.Forms;

using Ares.Data;

namespace Ares.Editor.Actions
{
    public class AddModeElementAction : Action
    {
        public AddModeElementAction(TreeNode parent, IModeElement element, TreeNode elementNode)
        {
            m_Parent = parent;
            m_Node = elementNode;
            m_Element = element;
        }

        public override void Do()
        {
            m_Parent.Nodes.Add(m_Node);
            (m_Parent.Tag as IMode).AddElement(m_Element);
        }

        public override void Undo()
        {
            (m_Parent.Tag as IMode).RemoveElement(m_Element);
            m_Parent.Nodes.Remove(m_Node);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private IModeElement m_Element;
    }

    static class ElementRemoval
    {
        public static void NotifyRemoval(IModeElement element)
        {
            NotifyRemoval(element.StartElement);
            Notify(element);
        }

        public static void NotifyRemoval(IElement element)
        {
            if (element == null)
                return;
            if (element is IGeneralElementContainer)
            {
                foreach (IElement subElement in (element as IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyRemoval(subElement);
                }
            }
            Notify(element);
        }

        private static void Notify(IElement element)
        {
            ElementChanges.Instance.ElementRemoved(element.Id);
        }
    }

    public class DeleteModeElementAction : Action
    {
        public DeleteModeElementAction(TreeNode node)
        {
            m_Parent = node.Parent;
            m_Node = node;
            m_Index = node.Parent.Nodes.IndexOf(node);
        }

        public override void Do()
        {
            m_Parent.Nodes.Remove(m_Node);
            IModeElement element = (m_Node.Tag as IModeElement);
            (m_Parent.Tag as IMode).RemoveElement(element);
            ElementRemoval.NotifyRemoval(element);
        }

        public override void Undo()
        {
            m_Parent.Nodes.Insert(m_Index, m_Node);
            (m_Parent.Tag as IMode).InsertElement(m_Index, (m_Node.Tag as IModeElement));
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private int m_Index;
    }

    public class RenameModeElementAction : Action
    {
        public RenameModeElementAction(TreeNode node, String newName)
        {
            m_Node = node;
            m_OldName = (node.Tag as IModeElement).Title;
            m_NewName = newName;
        }

        public override void Do()
        {
            IModeElement modeElement = m_Node.Tag as IModeElement;
            modeElement.Title = m_NewName;
            modeElement.StartElement.Title = m_NewName;
            m_Node.Text = modeElement.GetNodeTitle();
            ElementChanges.Instance.ElementRenamed(modeElement.Id);
            ElementChanges.Instance.ElementRenamed(modeElement.StartElement.Id);
        }

        public override void Undo()
        {
            IModeElement modeElement = m_Node.Tag as IModeElement;
            modeElement.Title = m_OldName;
            modeElement.StartElement.Title = m_OldName;
            m_Node.Text = modeElement.GetNodeTitle();
            ElementChanges.Instance.ElementRenamed(modeElement.Id);
            ElementChanges.Instance.ElementRenamed(modeElement.StartElement.Id);
        }

        private TreeNode m_Node;
        private String m_OldName;
        private String m_NewName;
    }


    public class SetModeElementKeyAction : Action
    {
        public SetModeElementKeyAction(TreeNode modeElementNode, int newKey)
        {
            m_Node = modeElementNode;
            IModeElement modeElement = (m_Node.Tag as IModeElement);
            m_OldTrigger = modeElement.Trigger;
            IKeyTrigger newTrigger = DataModule.ElementFactory.CreateKeyTrigger();
            newTrigger.TargetElementId = modeElement.Id;
            newTrigger.KeyCode = newKey;
            m_NewTrigger = newTrigger;
        }

        public override void Do()
        {
            IModeElement modeElement = (m_Node.Tag as IModeElement);
            modeElement.Trigger = m_NewTrigger;
            m_Node.Text = modeElement.GetNodeTitle();
        }

        public override void Undo()
        {
            IModeElement modeElement = (m_Node.Tag as IModeElement);
            modeElement.Trigger = m_OldTrigger;
            m_Node.Text = modeElement.GetNodeTitle();
        }

        private TreeNode m_Node;
        private ITrigger m_OldTrigger;
        private ITrigger m_NewTrigger;
    }

    public class RenameElementAction : Action
    {
        public RenameElementAction(TreeNode node, String newName)
        {
            m_Node = node;
            m_OldName = (node.Tag as IElement).Title;
            m_NewName = newName;
        }

        public override void Do()
        {
            IElement element = m_Node.Tag as IElement;
            element.Title = m_NewName;
            ElementChanges.Instance.ElementRenamed(element.Id);
        }

        public override void Undo()
        {
            IElement element = m_Node.Tag as IElement;
            element.Title = m_OldName;
            m_Node.Text = m_OldName;
            ElementChanges.Instance.ElementRenamed(element.Id);
        }

        private TreeNode m_Node;
        private String m_OldName;
        private String m_NewName;
    }

    public class AddElementAction : Action
    {
        public delegate TreeNode NodeCreator(IElement element);

        public AddElementAction(TreeNode parent, IElement element, NodeCreator nodeCreator, out TreeNode node)
        {
            m_Parent = parent;
            m_Element = (m_Parent.Tag as IGeneralElementContainer).AddGeneralElement(element);
            m_Node = nodeCreator(m_Element);
            m_Index = m_Parent.Nodes.Count;
            (m_Parent.Tag as IGeneralElementContainer).RemoveElement(m_Element.Id);
            node = m_Node;
        }

        public override void Do()
        {
            m_Parent.Nodes.Add(m_Node);
            (m_Parent.Tag as IGeneralElementContainer).InsertGeneralElement(m_Index, m_Element);
        }

        public override void Undo()
        {
            m_Parent.Nodes.Remove(m_Node);
            (m_Parent.Tag as IGeneralElementContainer).RemoveElement(m_Element.Id);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private IElement m_Element;
        private int m_Index;
    }

    public class DeleteElementAction : Action
    {
        public DeleteElementAction(TreeNode node)
        {
            m_Parent = node.Parent;
            m_Node = node;
            m_Index = node.Parent.Nodes.IndexOf(node);
        }

        public override void Do()
        {
            m_Parent.Nodes.Remove(m_Node);
            (m_Parent.Tag as IGeneralElementContainer).RemoveElement((m_Node.Tag as IElement).Id);
            ElementRemoval.NotifyRemoval(m_Node.Tag as IElement);
        }

        public override void Undo()
        {
            m_Parent.Nodes.Insert(m_Index, m_Node);
            (m_Parent.Tag as IGeneralElementContainer).InsertGeneralElement(m_Index, (m_Node.Tag as IElement));
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private int m_Index;
    }
}
