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
    public partial class AddLanguageDialog : Form
    {
        public AddLanguageDialog()
        {
            InitializeComponent();
        }

        public void SetCurrentLanguageName(String name)
        {
            nameLabel.Text = String.Format(StringResources.NameInLanguage, name);
        }

        private String m_NameInCurrentLanguage;
        private String m_NameInNewLanguage;
        private String m_LangCode;

        public String NameInCurrentLanguage { get { return m_NameInCurrentLanguage; } }
        public String NameInNewLanguage { get { return m_NameInNewLanguage; } }
        public String LanguageCode { get { return m_LangCode; } }

        private bool CheckName()
        {
            if (nameBox.Text == String.Empty)
            {
                errorProvider1.SetError(nameBox, StringResources.LanguageMustHaveName);
                return false;
            }
            else
            {
                errorProvider1.SetError(nameBox, String.Empty);
                m_NameInCurrentLanguage = nameBox.Text;
                return true;
            }
        }

        private bool CheckName2()
        {
            if (nameBox2.Text == String.Empty)
            {
                errorProvider1.SetError(nameBox2, StringResources.LanguageMustHaveTranslation);
                return false;
            }
            else
            {
                errorProvider1.SetError(nameBox2, String.Empty);
                m_NameInNewLanguage = nameBox2.Text;
                return true;
            }
        }

        private bool CheckLangCode()
        {
            if (langCodeBox.Text.Length != 2)
            {
                errorProvider1.SetError(langCodeBox, StringResources.LangCodeWrong);
                return false;
            }
            else
            {
                errorProvider1.SetError(langCodeBox, String.Empty);
                m_LangCode = langCodeBox.Text;
                return true;
            }
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            CheckName();
        }

        private void nameBox2_TextChanged(object sender, EventArgs e)
        {
            CheckName2();
        }

        private void okbutton_Click(object sender, EventArgs e)
        {
            bool res = CheckName();
            res = CheckName2() && res;
            res = CheckLangCode() && res;
            if (!res)
            {
                MessageBox.Show(this, StringResources.CorrectErrorsFirst, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void langCodeBox_Leave(object sender, EventArgs e)
        {
            CheckLangCode();
        }
    }
}
