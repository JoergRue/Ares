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
    partial class ToolsPage : UserControl
    {
        public ToolsPage()
        {
            InitializeComponent();
            audioFileEditorBox.Text = Ares.Settings.Settings.Instance.SoundFileEditor;
            musicPlayerBox.Text = Ares.Settings.Settings.Instance.ExternalMusicPlayer;
        }

        public void OnConfirm()
        {
            Ares.Settings.Settings.Instance.SoundFileEditor = audioFileEditorBox.Text;
            Ares.Settings.Settings.Instance.ExternalMusicPlayer = musicPlayerBox.Text;
        }

        private void selectFileEditorButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                audioFileEditorBox.Text = openFileDialog1.FileName;
            }
        }

        private void selectMusicPlayerButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                musicPlayerBox.Text = openFileDialog1.FileName;
            }
        }
    }

    public class ToolsPageHost : Ares.CommonGUI.ISettingsDialogPage
    {
        private ToolsPage m_Page = new ToolsPage();

        public Control Page
        {
            get { return m_Page; }
        }

        public string PageTitle
        {
            get { return StringResources.Tools; }
        }

        public void OnConfirm()
        {
            m_Page.OnConfirm();
        }
    }
}
