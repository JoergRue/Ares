/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Player
{
    public partial class StreamingDialog : Form
    {
        public StreamingDialog()
        {
            InitializeComponent();
            SetData();
        }

        private bool listen = true;

        private void SetData()
        {
            listen = false;
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            streamingBox.Checked = settings.UseStreaming; ;
            serverAddressBox.Text = settings.StreamingServerAddress;
            serverPortUpDown.Value = settings.StreamingServerPort;
            passwordBox.Text = settings.StreamingPassword;
            encodingBox.SelectedIndex = (settings.StreamingEncoder == (int)Playing.StreamEncoding.Ogg) ? 0 : 1;
            UpdateUrl();
            listen = true;
        }

        private void UpdateUrl()
        {
            String urlText = "Client URL: http://" + serverAddressBox.Text + ":" + serverPortUpDown.Value + "/Ares";
            if (encodingBox.SelectedIndex == 0)
                urlText += ".ogg";
            urlLabel.Text = urlText;
        }

        private void SaveData()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            settings.UseStreaming = streamingBox.Checked;
            settings.StreamingServerAddress = serverAddressBox.Text;
            settings.StreamingServerPort = (int)serverPortUpDown.Value;
            settings.StreamingPassword = passwordBox.Text;
            if (encodingBox.SelectedIndex == 1)
                settings.StreamingEncoder = (int)Playing.StreamEncoding.Lame;
            else
                settings.StreamingEncoder = (int)Playing.StreamEncoding.Ogg;
        }

        private void encodingBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            if (encodingBox.SelectedIndex == 1)
            {
                MessageBox.Show(this, StringResources.GetLameYourself, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            UpdateUrl();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        private void serverAddressBox_TextChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateUrl();
        }

        private void serverPortUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateUrl();
        }
    }
}
