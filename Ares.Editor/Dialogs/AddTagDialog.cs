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
    public partial class AddTagDialog : Form
    {
        public AddTagDialog()
        {
            InitializeComponent();
        }

        private bool m_Listen = true;

        public void SetCategories(String[] names, String selectedName)
        {
            m_Listen = false;
            int selIndex = -1;
            for (int i = 0; i < names.Length; ++i)
            {
                categoryBox.Items.Add(names[i]);
                if (names[i] == selectedName)
                    selIndex = categoryBox.Items.Count - 1;
            }
            if (selIndex != -1)
            {
                categoryBox.SelectedValue = selectedName;
                categoryBox.Text = selectedName;
            }
            m_Listen = true;
        }

        private void categoryBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            CheckCategory();
        }

        private bool CheckCategory()
        {
            if (String.IsNullOrEmpty(categoryBox.Text))
            {
                errorProvider1.SetError(categoryBox, StringResources.TagMustHaveCategory);
                return false;
            }
            else
            {
                errorProvider1.SetError(categoryBox, String.Empty);
                m_Category = categoryBox.Text;
                return true;
            }
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            CheckName();
        }

        private bool CheckName()
        {
            if (nameBox.Text == String.Empty)
            {
                errorProvider1.SetError(nameBox, StringResources.TagMustHaveName);
                return false;
            }
            else
            {
                errorProvider1.SetError(nameBox, String.Empty);
                m_Name = nameBox.Text;
                return true;
            }
        }

        private String m_Category = String.Empty;
        private String m_Name = String.Empty;

        public String Category { get { return m_Category; } }
        public String TagName { get { return m_Name; } }

        private void okbutton_Click(object sender, EventArgs e)
        {
            bool res = CheckCategory();
            res = CheckName() && res;
            if (!res)
            {
                MessageBox.Show(this, StringResources.CorrectErrorsFirst, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void categoryBox_TextChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            CheckCategory();
        }
    }
}
