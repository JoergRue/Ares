/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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

namespace Ares.Editor.Dialogs
{
    partial class OnlineDbPage : UserControl
    {
        public OnlineDbPage()
        {
            InitializeComponent();
            var settings = Ares.Settings.Settings.Instance;
            mailAddressBox.Text = settings.OnlineDBUserId;
            downloadDialogBox.Checked = settings.ShowDialogAfterDownload;
            uploadDialogBox.Checked = settings.ShowDialogAfterUpload;
        }

        public void OnConfirm()
        {
            var settings = Ares.Settings.Settings.Instance;
            settings.OnlineDBUserId = mailAddressBox.Text;
            settings.ShowDialogAfterDownload = downloadDialogBox.Checked;
            settings.ShowDialogAfterUpload = uploadDialogBox.Checked;
        }
    }

    public class OnlineDbPageHost : Ares.CommonGUI.ISettingsDialogPage
    {
        private OnlineDbPage m_Page = new OnlineDbPage();

        public Control Page
        {
            get { return m_Page; }
        }

        public string PageTitle
        {
            get { return StringResources.OnlineDB; }
        }

        public void OnConfirm()
        {
            m_Page.OnConfirm();
        }
    }
}
