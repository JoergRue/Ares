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
    public partial class StreamingPage : UserControl
    {
        public StreamingPage()
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
            encodingBox.SelectedIndex = (settings.StreamingEncoder == (int)Playing.StreamEncoding.Ogg) ? 0 : 
                ((settings.StreamingEncoder == (int)Playing.StreamEncoding.Opus) ? 2 : 1);
            switch (settings.StreamingBitrate)
            {
                case 32:
                    bitrateBox.SelectedIndex = 0;
                    break;
                case 48:
                    bitrateBox.SelectedIndex = 1;
                    break;
                case 64:
                    bitrateBox.SelectedIndex = 2;
                    break;
                case 96:
                    bitrateBox.SelectedIndex = 3;
                    break;
                case 128:
                    bitrateBox.SelectedIndex = 4;
                    break;
                case 144:
                    bitrateBox.SelectedIndex = 5;
                    break;
                case 192:
                    bitrateBox.SelectedIndex = 6;
                    break;
                default:
                    bitrateBox.SelectedIndex = 4;
                    break;
            }
            streamNameBox.Text = settings.StreamingStreamName;
            userNameBox.Text = settings.StreamingUserName;
            UpdateUrl();
            listen = true;
        }

        private void UpdateUrl()
        {
            String urlText = "Client URL: http://" + serverAddressBox.Text + ":" + serverPortUpDown.Value + "/" + streamNameBox.Text;
            if (encodingBox.SelectedIndex == 0)
                urlText += ".ogg";
            else if (encodingBox.SelectedIndex == 2)
                urlText += ".opus";
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
            else if (encodingBox.SelectedIndex == 2)
                settings.StreamingEncoder = (int)Playing.StreamEncoding.Opus;
            else
                settings.StreamingEncoder = (int)Playing.StreamEncoding.Ogg;
            switch (bitrateBox.SelectedIndex)
            {
                case 0:
                    settings.StreamingBitrate = 32;
                    break;
                case 1:
                    settings.StreamingBitrate = 48;
                    break;
                case 2:
                    settings.StreamingBitrate = 64;
                    break;
                case 3:
                    settings.StreamingBitrate = 96;
                    break;
                case 4:
                    settings.StreamingBitrate = 128;
                    break;
                case 5:
                    settings.StreamingBitrate = 144;
                    break;
                case 6:
                    settings.StreamingBitrate = 192;
                    break;
                default:
                    settings.StreamingBitrate = 128;
                    break;
            }
            settings.StreamingStreamName = String.IsNullOrEmpty(streamNameBox.Text) ? "Ares" : streamNameBox.Text;
            settings.StreamingUserName = userNameBox.Text;
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

        public void OnConfirm()
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

        private void streamNameBox_TextChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            UpdateUrl();
        }
    }

    class StreamingPageHost : Ares.CommonGUI.ISettingsDialogPage
    {
        private StreamingPage m_Page = new StreamingPage();

        public Control Page
        {
            get { return m_Page; }
        }

        public string PageTitle
        {
            get { return StringResources.Streaming; }
        }

        public void OnConfirm()
        {
            m_Page.OnConfirm();
        }
    }
}
