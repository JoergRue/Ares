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

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Add(m_Node);
            (m_Parent.Tag as IMode).AddElement(m_Element);
        }

        public override void Undo(Ares.Data.IProject project)
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

        public static void NotifyUndo(IModeElement element)
        {
            NotifyUndo(element.StartElement);
            DoNotifyUndo(element);
        }

        public static void NotifyRemoval(IElement element)
        {
            if (element == null)
                return;
            if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement subElement in (element as IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyRemoval(subElement.InnerElement);
                }
            }
            Notify(element);
        }

        public static void NotifyUndo(IElement element)
        {
            if (element == null)
                return;
            if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement subElement in (element as IGeneralElementContainer).GetGeneralElements())
                {
                    NotifyUndo(subElement.InnerElement);
                }
            }
            DoNotifyUndo(element);
        }

        private static void Notify(IElement element)
        {
            ElementChanges.Instance.ElementRemoved(element.Id);
        }

        private static void DoNotifyUndo(IElement element)
        {
            ElementChanges.Instance.ElementChanged(element.Id);
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

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Remove(m_Node);
            IModeElement element = (m_Node.Tag as IModeElement);
            (m_Parent.Tag as IMode).RemoveElement(element);
            Data.DataModule.ElementRepository.DeleteElement(element.Id);
            ElementRemoval.NotifyRemoval(element);
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Insert(m_Index, m_Node);
            Data.DataModule.ElementRepository.AddElement(m_Node.Tag as IModeElement);
            (m_Parent.Tag as IMode).InsertElement(m_Index, (m_Node.Tag as IModeElement));
            ElementRemoval.NotifyUndo(m_Node.Tag as IElement);
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private int m_Index;
    }

    public class DeleteBackgroundSoundChoiceAction : Action
    {
        public DeleteBackgroundSoundChoiceAction(TreeNode node)
        {
            m_Parent = node.Parent;
            m_Node = node;
            m_Index = node.Parent.Nodes.IndexOf(node);
        }

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Remove(m_Node);
            IBackgroundSoundChoice soundChoice = (m_Node.Tag as IBackgroundSoundChoice);
            IBackgroundSounds bgSounds = m_Parent.Tag as IBackgroundSounds;
            bgSounds.RemoveElement(soundChoice.Id);
            Data.DataModule.ElementRepository.DeleteElement(soundChoice.Id);
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
            ElementRemoval.NotifyRemoval(soundChoice);
            ElementChanges.Instance.ElementChanged(bgSounds.Id);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Insert(m_Index, m_Node);
            IBackgroundSounds bgSounds = m_Parent.Tag as IBackgroundSounds;
            bgSounds.InsertElement(m_Index, (m_Node.Tag as IBackgroundSoundChoice));
            Data.DataModule.ElementRepository.AddElement((m_Node.Tag as IBackgroundSoundChoice));
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
            ElementRemoval.NotifyUndo(m_Node.Tag as IBackgroundSoundChoice);
            ElementChanges.Instance.ElementChanged(bgSounds.Id);
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

        public override void Do(Ares.Data.IProject project)
        {
            IModeElement modeElement = m_Node.Tag as IModeElement;
            modeElement.Title = m_NewName;
            modeElement.StartElement.Title = m_NewName;
            m_Node.Text = modeElement.GetNodeTitle();
            ElementChanges.Instance.ElementRenamed(modeElement.Id);
            ElementChanges.Instance.ElementRenamed(modeElement.StartElement.Id);
        }

        public override void Undo(Ares.Data.IProject project)
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


    public class RenameElementAction : Action
    {
        public RenameElementAction(TreeNode node, String newName)
        {
            m_Node = node;
            m_OldName = (node.Tag as IElement).Title;
            m_NewName = newName;
        }

        public override void Do(Ares.Data.IProject project)
        {
            IElement element = m_Node.Tag as IElement;
            element.Title = m_NewName;
            ElementChanges.Instance.ElementRenamed(element.Id);
        }

        public override void Undo(Ares.Data.IProject project)
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

        public AddElementAction(TreeNode parent, IGeneralElementContainer container, IElement element, NodeCreator nodeCreator, out TreeNode node)
        {
            m_Parent = parent;
            m_Element = (IContainerElement)container.AddGeneralElement(element);
            m_Node = nodeCreator(m_Element.InnerElement);
            m_NodeIndex = m_Parent.Nodes.Count;
            m_ElementIndex = container.GetGeneralElements().Count - 1;
            container.RemoveElement(m_Element.Id);
            m_Container = container;
            node = m_Node;
        }

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Add(m_Node);
            m_Container.InsertGeneralElement(m_ElementIndex, m_Element);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Remove(m_Node);
            m_Container.RemoveElement(m_Element.Id);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private IContainerElement m_Element;
        private int m_NodeIndex;
        private int m_ElementIndex;
        private IGeneralElementContainer m_Container;
    }

    public class AddSoundChoiceAction : Action
    {
        public delegate TreeNode NodeCreator(IElement element);

        public AddSoundChoiceAction(TreeNode parent, IBackgroundSounds bgSounds, String name, NodeCreator nodeCreator, out TreeNode node)
        {
            m_Parent = parent;
            m_Element = bgSounds.AddElement(name);
            m_Node = nodeCreator(m_Element);
            m_Index = m_Parent.Nodes.Count;
            bgSounds.RemoveElement(m_Element.Id);
            node = m_Node;
            m_BGSounds = bgSounds;
        }

        public AddSoundChoiceAction(TreeNode parent, IBackgroundSounds bgSounds, IXmlWritable importedElement, NodeCreator nodeCreator, out TreeNode node)
        {
            m_Parent = parent;
            m_Element = bgSounds.AddImportedElement(importedElement);
            m_Node = nodeCreator(m_Element);
            m_Index = m_Parent.Nodes.Count;
            bgSounds.RemoveElement(m_Element.Id);
            node = m_Node;
            m_BGSounds = bgSounds;
        }

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Add(m_Node);
            m_BGSounds.InsertElement(m_Index, m_Element);
            ElementChanges.Instance.ElementChanged(m_BGSounds.Id);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Remove(m_Node);
            m_BGSounds.RemoveElement(m_Element.Id);
            ElementChanges.Instance.ElementChanged(m_BGSounds.Id);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private IBackgroundSoundChoice m_Element;
        private IBackgroundSounds m_BGSounds;
        private int m_Index;
    }

    public class DeleteElementAction : Action
    {
        public DeleteElementAction(TreeNode node)
        {
            m_Parent = node.Parent;
            m_Node = node;
            m_Index = node.Parent.Nodes.IndexOf(node);
            IElement parentElement = m_Parent.Tag is IModeElement ? (m_Parent.Tag as IModeElement).StartElement : m_Parent.Tag as IElement;
            m_Container = parentElement as IGeneralElementContainer;
            m_Element = m_Container.GetGeneralElements()[m_Index];
        }

        public override void Do(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Remove(m_Node);
            m_Container.RemoveElement((m_Node.Tag as IElement).Id);
            Data.DataModule.ElementRepository.DeleteElement((m_Node.Tag as IElement).Id);
            ElementRemoval.NotifyRemoval(m_Node.Tag as IElement);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            m_Parent.Nodes.Insert(m_Index, m_Node);
            m_Container.InsertGeneralElement(m_Index, m_Element);
            Data.DataModule.ElementRepository.AddElement(m_Element);
            ElementRemoval.NotifyUndo(m_Element);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
            Ares.ModelInfo.ModelChecks.Instance.CheckAll(project);
        }

        private TreeNode m_Parent;
        private TreeNode m_Node;
        private IGeneralElementContainer m_Container;
        private IElement m_Element;
        private int m_Index;
    }

    public class ReorderElementsAction<T> : Action where T : IContainerElement
    {
        public ReorderElementsAction(IReorderableContainer<T> container, List<int> indices, int offset)
        {
            m_Container = container;
            indices.Sort();
            m_Indices = indices;
            m_offset = offset;
            if (offset < 0)
            {
                while (indices.Count > 0 && indices[0] < -offset)
                {
                    indices.RemoveAt(0);
                }
            }
            else if (offset > 0)
            {
                while (indices.Count > 0 && indices[indices.Count - 1] >= container.GetElements().Count - offset)
                {
                    indices.RemoveAt(indices.Count - 1);
                }
            }
        }

        public override void Do(Ares.Data.IProject project)
        {
            if (m_offset > 0)
                MoveDown(m_offset);
            else
                MoveUp(-m_offset);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            if (m_offset > 0)
                MoveUp(m_offset);
            else
                MoveDown(-m_offset);
            ElementChanges.Instance.ElementChanged(m_Container.Id);
        }

        private void MoveUp(int offset)
        {
            for (int i = 0; i < m_Indices.Count; ++i)
            {
                m_Container.MoveElements(m_Indices[i], m_Indices[i], -offset);
                m_Indices[i] -= offset;
            }
        }

        private void MoveDown(int offset)
        {
            for (int i = m_Indices.Count - 1; i >= 0; --i)
            {
                m_Container.MoveElements(m_Indices[i], m_Indices[i], offset);
                m_Indices[i] += offset;
            }
        }


        private IReorderableContainer<T> m_Container;
        private List<int> m_Indices;
        private int m_offset;
    }
}
