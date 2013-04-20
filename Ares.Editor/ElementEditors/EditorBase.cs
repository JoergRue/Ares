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
using System.Linq;
using System.Text;
using Ares.Editor.Controls;
using System.Windows.Forms;

namespace Ares.Editor.ElementEditors
{
#if !MONO
    class EditorBase : WeifenLuo.WinFormsUI.Docking.DockContent
#else
    class EditorBase : System.Windows.Forms.Form
#endif
    {
        protected int ElementId 
        {
            get
            {
                return m_ElementId;
            }
            set
            {
                if (m_ElementId != -1)
                {
                    EditorRegistry.Instance.UnregisterEditor(m_ElementId);
                }
                m_ElementId = value;
                if (m_ElementId != -1)
                {
                    EditorRegistry.Instance.RegisterEditor(m_ElementId, this);
                }
            }
        }

        protected EditorBase() 
        { 
            ElementId = -1; 
        }

        protected override void Dispose(bool disposing)
        {
            if (ElementId != -1 && disposing)
            {
                EditorRegistry.Instance.UnregisterEditor(ElementId);
            }
            base.Dispose(disposing);
        }

        private int m_ElementId;
    }

    class ContainerEditorBase : EditorBase
    {
        protected virtual Ares.Editor.Controls.ContainerControl ContainerControl
        {
            get
            {
                return null;
            }
        }

        protected virtual Label BigLabel
        {
            get
            {
                return null;
            }
        }

        protected virtual void DisableControls(bool allowStop)
        {
        }

        protected virtual void EnableControls()
        {
        }

        protected Ares.Data.IProject Project { get; set; }

        protected ContainerEditorBase()
            : base()
        {
            AttachDragDropEvents(this);
        }

        private void AttachDragDropEvents(Control control)
        {
            control.DragDrop += new System.Windows.Forms.DragEventHandler(this.Editor_DragDrop);
            control.DragEnter += new System.Windows.Forms.DragEventHandler(this.Editor_DragEnter);
            control.DragOver += new System.Windows.Forms.DragEventHandler(this.Editor_DragOver);
            control.DragLeave += new System.EventHandler(this.Editor_DragLeave);
        }

        protected void AttachOtherEvents(Button playButton, Button stopButton)
        {
            ContainerControl.ElementDoubleClick += new System.EventHandler<Ares.Editor.Controls.ElementDoubleClickEventArgs>(this.ContainerControl_ElementDoubleClick);
            ContainerControl.ElementsImported += new EventHandler<ElementImportEventArgs>(ContainerControl_ElementsImported);
            stopButton.Click += new System.EventHandler(this.stopButton_Click);
            playButton.Click += new System.EventHandler(this.playButton_Click);
            if (BigLabel != null)
            {
                this.SizeChanged += new System.EventHandler(this.Editor_SizeChanged);
                AttachDragDropEvents(BigLabel);
            }
        }

        private void ContainerControl_ElementsImported(object sender, ElementImportEventArgs e)
        {
            if (String.IsNullOrEmpty(e.SerializedData))
                return;
            IList<Ares.Data.IXmlWritable> elements = Ares.Data.DataModule.ProjectManager.ImportElementsFromString(e.SerializedData);
            ContainerControl.AddImportedElements(elements, e.InsertionRow);
        }

        protected void ElementSet(Ares.Data.IProject project)
        {
            Project = project;
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

        private void Editor_SizeChanged(object sender, EventArgs e)
        {
            System.Drawing.Font font = BigLabel.Font;
            String text = BigLabel.Text;
            using (System.Drawing.Graphics g = BigLabel.CreateGraphics())
            {
                float textWidth = g.MeasureString(text, font).Width;
                if (textWidth == 0) return;
                float factor = BigLabel.Width / textWidth;
                if (factor == 0) return;
                BigLabel.Font = new System.Drawing.Font(font.Name, font.SizeInPoints * factor);
            }
        }

        private void ContainerControl_ElementDoubleClick(object sender, Controls.ElementDoubleClickEventArgs e)
        {
#if !MONO
            Editors.ShowEditor(e.Element.InnerElement, m_Element as Ares.Data.IGeneralElementContainer, Project, this.DockPanel);
#else
            Editors.ShowEditor(e.Element.InnerElement, m_Element as Ares.Data.IGeneralElementContainer, m_Project, this.MdiParent);
#endif
        }

        protected bool listen = true;
        protected Ares.Data.IElement m_Element;

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
            m_AcceptDrop = ContainerControl.Enabled && e.Data.GetDataPresent(typeof(List<DraggedItem>));
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
            List<DraggedItem> list = e.Data.GetData(typeof(List<DraggedItem>)) as List<DraggedItem>;
            if (list != null)
            {
                if (list.Count > 1 || list[0].NodeType == DraggedItemType.Directory)
                {
                    System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
                    TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.AddingFiles, tokenSource);
                    monitor.IncreaseProgress(0.1, StringResources.GettingTitles);
                    var task = DragAndDrop.GetElementsFromDroppedItemsAsync(list, tokenSource, monitor);
                    task.ContinueWith((t) =>
                    {
                        monitor.Close();
                        try
                        {
                            var result = task.Result;
                            if (result != null)
                                ContainerControl.AddElements(result, row);
                        }
                        catch (AggregateException)
                        {
                        }
                    }, tokenSource.Token, System.Threading.Tasks.TaskContinuationOptions.OnlyOnRanToCompletion, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                    task.ContinueWith((t) =>
                    {
                        monitor.Close();
                    }, tokenSource.Token, System.Threading.Tasks.TaskContinuationOptions.NotOnRanToCompletion, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    List<Ares.Data.IElement> elements = new List<Ares.Data.IElement>(DragAndDrop.GetElementsFromDroppedItems(list));
                    ContainerControl.AddElements(elements, row);
                }
            }
        }

    }
}
