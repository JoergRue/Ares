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
using System.Text;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    partial class MessagesForm : Form
    {
        public MessagesForm()
        {
            InitializeComponent();
            Ares.Controllers.Messages.Instance.MessageAdded += new EventHandler<Controllers.MessageEventArgs>(MessageReceived);
            filterBox.SelectedIndex = 3 - Ares.MGPlugin.Settings.Default.MessageLevel;
            FilterLevel = (Ares.Controllers.MessageType)Ares.MGPlugin.Settings.Default.MessageLevel;
        }

        private void RefillGrid()
        {
            messagesGrid.SuspendLayout();
            messagesGrid.Rows.Clear();
            foreach (Ares.Controllers.Message m in Ares.Controllers.Messages.Instance.GetMessages())
            {
                AddMessage(m);
            }
            messagesGrid.ResumeLayout();
        }

        private void MessageReceived(Object sender, Ares.Controllers.MessageEventArgs m)
        {
            if (IsDisposed)
                return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => AddMessage(m.Message)));
            }
            else
            {
                AddMessage(m.Message);
            }
        }

        private void AddMessage(Ares.Controllers.Message m)
        {
            if (m.Type <= FilterLevel)
            {
                messagesGrid.Rows.Add(GetIcon(m.Type), m.Text);
            }
        }

        private static object GetIcon(Ares.Controllers.MessageType type)
        {
            switch (type)
            {
                case Ares.Controllers.MessageType.Debug:
                    return Properties.Resources.gear;
                case Ares.Controllers.MessageType.Info:
                    return Properties.Resources.eventlogInfo;
                case Ares.Controllers.MessageType.Warning:
                    return Properties.Resources.eventlogWarn;
                case Ares.Controllers.MessageType.Error:
                    return Properties.Resources.eventlogError;
                default:
                    return null;
            }
        }

        public Ares.Controllers.MessageType FilterLevel { get; set; }

        private void filterBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLevel = 3 - (Ares.Controllers.MessageType)filterBox.SelectedIndex;
            Settings.Default.MessageLevel = 3 - filterBox.SelectedIndex;
            Settings.Default.Save();
            RefillGrid();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            messagesGrid.Rows.Clear();
            Ares.Controllers.Messages.Instance.Clear();
        }
    }
}
