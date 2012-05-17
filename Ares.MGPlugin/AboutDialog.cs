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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();
            versionLabel.Text = "Version " + assemblyVersion;
        }

        private void homePageLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Online.OnlineOperations.ShowHomepage(this);
        }

        private void updateVersionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Online.OnlineOperations.CheckForUpdateMGPlugin(this);
        }
    }
}
