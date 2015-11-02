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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor.Dialogs
{
    public partial class AddID3TagsDialog : Form
    {
        public AddID3TagsDialog()
        {
            InitializeComponent();
        }

        public bool Interpret { get; set; }
        public bool Album { get; set; }
        public bool Genre { get; set; }
        public bool Mood { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            Interpret = interpreterBox.Checked;
            Album = albumBox.Checked;
            Genre = genreBox.Checked;
            Mood = moodBox.Checked;
        }
    }
}
