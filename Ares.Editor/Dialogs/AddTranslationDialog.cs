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
    public partial class AddTranslationDialog : Form
    {
        public AddTranslationDialog()
        {
            InitializeComponent();
        }

        private Dictionary<int, int> langIndexToLangId;

        public void SetData(IList<Ares.Tags.LanguageForLanguage> languages, int currentLanguageId, String nameInCurrentLanguage, String defaultName, int selLangId)
        {
            langIndexToLangId = new Dictionary<int, int>();
            int selIndex = -1;
            foreach (var info in languages)
            {
                if (info.Id == currentLanguageId)
                {
                    translationGroupBox.Text = String.Format(StringResources.TranslationBoxTitle, nameInCurrentLanguage);
                }
                else
                {
                    languageBox.Items.Add(info.Name);
                    langIndexToLangId[languageBox.Items.Count - 1] = info.Id;
                    if (info.Id == selLangId)
                    {
                        selIndex = languageBox.Items.Count - 1;
                    }
                }
            }
            if (selIndex == -1)
                selIndex = 0;
            languageBox.SelectedIndex = selIndex;
            nameBox.Text = defaultName;
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            CheckName();
        }

        private bool CheckName()
        {
            if (nameBox.Text == String.Empty)
            {
                errorProvider1.SetError(nameBox, StringResources.TranslationMustHaveName);
                return false;
            }
            else
            {
                errorProvider1.SetError(nameBox, String.Empty);
                m_Name = nameBox.Text;
                return true;
            }
        }

        private int m_LanguageId;
        private String m_Name;

        public int LanguageId { get { return m_LanguageId; } }
        public String Translation { get { return m_Name; } }

        private void okbutton_Click(object sender, EventArgs e)
        {
            bool res = CheckName();
            if (!res)
            {
                MessageBox.Show(this, StringResources.CorrectErrorsFirst, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
            m_LanguageId = langIndexToLangId[languageBox.SelectedIndex];
        }


    }
}
