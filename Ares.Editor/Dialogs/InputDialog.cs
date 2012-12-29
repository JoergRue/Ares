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

namespace Ares.Editor.Dialogs
{
    partial class InputDialog : Form
    {
        public InputDialog()
        {
            InitializeComponent();
        }

        public String Input
        {
            get
            {
                return inputBox.Text;
            }

            set
            {
                inputBox.Text = value;
            }
        }

        public String Prompt
        {
            set
            {
                promptLabel.Text = value;
            }
        }
    }

    public class InputDialogs
    {
        public static DialogResult ShowInputDialog(IWin32Window parent, String title, String prompt, out String result)
        {
            return ShowInputDialog(parent, title, prompt, String.Empty, out result);
        }

        public static DialogResult ShowInputDialog(IWin32Window parent, String title, String prompt, String initial, out String result)
        {
            InputDialog dialog = new InputDialog();
            dialog.Prompt = prompt;
            dialog.Text = title;
            dialog.Input = initial;
            DialogResult dialogResult = dialog.ShowDialog(parent);
            result = dialog.Input;
            return dialogResult;
        }
    }
}
