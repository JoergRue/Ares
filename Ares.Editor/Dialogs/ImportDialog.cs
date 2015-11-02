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
    public partial class ImportDialog : Form
    {
        public ImportDialog()
        {
            InitializeComponent();
            elementsBox.DrawItem += new DrawItemEventHandler(elementsBox_DrawItem);
        }

        private const int CHECKBOX_WIDTH = 15;

        void elementsBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.Window)), e.Bounds);
            Rectangle contentRect = e.Bounds;
            contentRect.X = CHECKBOX_WIDTH + 1;
            Color c = m_Elements[e.Index].Enabled ? Color.FromKnownColor(KnownColor.WindowText) : Color.FromKnownColor(KnownColor.GrayText);
            e.Graphics.DrawString(elementsBox.Items[e.Index].ToString(),
                e.Font,
                new SolidBrush(c),
                contentRect);
        }

        private IList<ImportElement> m_Elements;

        public void SetElements(IList<ImportElement> elements)
        {
            m_Elements = elements;
            for (int i = 0; i < elements.Count; ++i)
            {
                CheckBox cb = new CheckBox();
                cb.Text = String.Empty;
                cb.Checked = elements[i].Selected;
                cb.Enabled = elements[i].Enabled;
                cb.Height = elementsBox.ItemHeight;
                cb.Width = CHECKBOX_WIDTH;
                cb.Location = new Point(0, elementsBox.ItemHeight * i);
                cb.Tag = (Int32)i;
                cb.CheckedChanged += new EventHandler((object sender, EventArgs e) =>
                {
                    CheckBox scb = (CheckBox)sender;
                    m_Elements[(Int32)scb.Tag].Selected = scb.Checked;
                });
                elementsBox.Controls.Add(cb);
                elementsBox.Items.Add(elements[i].Element.Title);
            }
        }

    }

    public class ImportElement
    {
        public Ares.Data.IXmlWritable Element { get; set; }
        public bool Enabled { get; set; }
        public bool Selected { get; set; }

        public ImportElement(Ares.Data.IXmlWritable element, bool enabled)
        {
            Element = element;
            Enabled = enabled;
            Selected = enabled;
        }
    }

}
