/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ares.Editor.Controls;

namespace Ares.Editor.ElementEditors
{
    partial class MacroEditor : EditorBase
    {
        public MacroEditor()
            : base()
        {
            InitializeComponent();
            AttachDragDropEvents(this);
            AttachOtherEvents();
        }

        protected Ares.Editor.Controls.ContainerControl ContainerControl
        {
            get
            {
                return macroControl;
            }
        }

        public void SetContainer(Ares.Data.IMacro container, Ares.Data.IProject project)
        {
            ElementId = container.Id;
            m_Element = container;
            m_Project = project;
            macroControl.SetContainer(container, project);
            ElementSet();
        }

        protected void DisableControls(bool allowStop)
        {
            macroControl.Enabled = false;
            macroControl.EnableStopButton(allowStop);
        }

        protected void EnableControls()
        {
            macroControl.Enabled = true;
        }

        private void AttachDragDropEvents(Control control)
        {
            control.DragDrop += new System.Windows.Forms.DragEventHandler(this.Editor_DragDrop);
            control.DragEnter += new System.Windows.Forms.DragEventHandler(this.Editor_DragEnter);
            control.DragOver += new System.Windows.Forms.DragEventHandler(this.Editor_DragOver);
            control.DragLeave += new System.EventHandler(this.Editor_DragLeave);
        }

        protected void AttachOtherEvents()
        {
            ContainerControl.ElementDoubleClick += new System.EventHandler<Ares.Editor.Controls.ElementDoubleClickEventArgs>(this.ContainerControl_ElementDoubleClick);
            ContainerControl.ElementsImported += new EventHandler<ElementImportEventArgs>(ContainerControl_ElementsImported);
            macroControl.StopButtonClick += new System.EventHandler<EventArgs>(this.stopButton_Click);
            macroControl.PlayButtonClick += new System.EventHandler<EventArgs>(this.playButton_Click);
            macroControl.AddButtonClick += new EventHandler<EventArgs>(macroControl_AddButtonClick);
        }

        void macroControl_AddButtonClick(object sender, EventArgs e)
        {
            Dialogs.MacroCommandDialog dialog = new Dialogs.MacroCommandDialog();
            dialog.SetData(null, m_Project, m_Element as Data.IMacro);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Actions.Actions.Instance.AddNew(new Actions.AddMacroCommandAction((Ares.Data.IMacro)m_Element, dialog.MacroCommand), m_Project);
            }
        }

        private void ContainerControl_ElementsImported(object sender, ElementImportEventArgs e)
        {
            if (String.IsNullOrEmpty(e.SerializedData))
                return;
            Ares.ModelInfo.ModelChecks.Instance.CancelChecks();
            IList<Ares.Data.IXmlWritable> elements = Ares.Data.DataModule.ProjectManager.ImportElementsFromString(e.SerializedData);
            ContainerControl.AddImportedElements(elements, e.InsertionRow);
        }

        protected void ElementSet()
        {
            Update(m_Element.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
            if (Actions.Playing.Instance.IsElementPlaying(m_Element))
            {
                DisableControls(true);
            }
            else if (Actions.Playing.Instance.IsElementOrSubElementPlaying(m_Element))
            {
                DisableControls(false);
            }
        }

        protected void Update(int elementId, Actions.ElementChanges.ChangeType changeType)
        {
            if (!listen)
                return;
            if (elementId == m_Element.Id)
            {
                if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    this.Text = m_Element.Title;
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Removed)
                {
                    Close();
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Played)
                {
                    DisableControls(false);
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Stopped)
                {
                    EnableControls();
                }
            }
        }

        private void ContainerControl_ElementDoubleClick(object sender, Controls.ElementDoubleClickEventArgs e)
        {
            Dialogs.MacroCommandDialog dialog = new Dialogs.MacroCommandDialog();
            dialog.SetData((Ares.Data.IMacroCommand)e.Element.InnerElement, m_Project, m_Element as Data.IMacro);
            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                Actions.Actions.Instance.AddNew(new Actions.ReplaceMacroCommandAction((Ares.Data.IMacro)m_Element, e.Element.InnerElement.Id, dialog.MacroCommand), m_Project);
                Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
            }
        }

        protected bool listen = true;
        protected Ares.Data.IElement m_Element;
        protected Ares.Data.IProject m_Project;

        private void playButton_Click(object sender, EventArgs e)
        {
            if (m_Element != null)
            {
                listen = false;
                DisableControls(true);
                Actions.Playing.Instance.PlayElement(m_Element, Parent, () => { });
                listen = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(m_Element);
        }

        private bool m_AcceptDrop;

        private void Editor_DragEnter(object sender, DragEventArgs e)
        {
            if (ContainerControl.PerformDragEnter(e))
                return;
            m_AcceptDrop = false;
        }

        private void Editor_DragLeave(object sender, EventArgs e)
        {
            ContainerControl.PerformDragLeave(e);
            m_AcceptDrop = false;
        }

        private void Editor_DragOver(object sender, DragEventArgs e)
        {
            if (ContainerControl.PerformDragOver(e))
                return;
            if (m_AcceptDrop && (e.AllowedEffect & DragDropEffects.Copy) != 0)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Editor_DragDrop(object sender, DragEventArgs e)
        {
            int row = 0;
            if (ContainerControl.PerformDrop(e, out row))
            {
                return;
            }
        }
    }
}
