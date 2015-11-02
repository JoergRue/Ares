/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
namespace Ares.Editor.Controls
{
    partial class MacroControl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacroControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.elementsGrid = new System.Windows.Forms.DataGridView();
            this.commandCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.conditionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewNumericUpDownColumn1 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.dataGridViewNumericUpDownColumn2 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elementsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.editButton);
            this.groupBox1.Controls.Add(this.deleteButton);
            this.groupBox1.Controls.Add(this.addButton);
            this.groupBox1.Controls.Add(this.downButton);
            this.groupBox1.Controls.Add(this.upButton);
            this.groupBox1.Controls.Add(this.elementsGrid);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // elementsGrid
            // 
            this.elementsGrid.AllowUserToAddRows = false;
            resources.ApplyResources(this.elementsGrid, "elementsGrid");
            this.elementsGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.elementsGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.elementsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.elementsGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.elementsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.elementsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.elementsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.commandCol,
            this.conditionColumn});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.elementsGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.elementsGrid.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.elementsGrid.Name = "elementsGrid";
            this.elementsGrid.RowHeadersVisible = false;
            this.elementsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.elementsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.elementsGrid_CellDoubleClick);
            this.elementsGrid.SelectionChanged += new System.EventHandler(this.elementsGrid_SelectionChanged);
            // 
            // commandCol
            // 
            this.commandCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.commandCol.FillWeight = 1F;
            resources.ApplyResources(this.commandCol, "commandCol");
            this.commandCol.Name = "commandCol";
            this.commandCol.ReadOnly = true;
            this.commandCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // conditionColumn
            // 
            this.conditionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.conditionColumn.FillWeight = 1F;
            resources.ApplyResources(this.conditionColumn, "conditionColumn");
            this.conditionColumn.Name = "conditionColumn";
            this.conditionColumn.ReadOnly = true;
            this.conditionColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewNumericUpDownColumn1
            // 
            this.dataGridViewNumericUpDownColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.dataGridViewNumericUpDownColumn1, "dataGridViewNumericUpDownColumn1");
            this.dataGridViewNumericUpDownColumn1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.dataGridViewNumericUpDownColumn1.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.dataGridViewNumericUpDownColumn1.Name = "dataGridViewNumericUpDownColumn1";
            this.dataGridViewNumericUpDownColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewNumericUpDownColumn1.ThousandsSeparator = true;
            // 
            // dataGridViewNumericUpDownColumn2
            // 
            this.dataGridViewNumericUpDownColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.dataGridViewNumericUpDownColumn2, "dataGridViewNumericUpDownColumn2");
            this.dataGridViewNumericUpDownColumn2.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.dataGridViewNumericUpDownColumn2.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.dataGridViewNumericUpDownColumn2.Name = "dataGridViewNumericUpDownColumn2";
            this.dataGridViewNumericUpDownColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dataGridViewNumericUpDownColumn2.ThousandsSeparator = true;
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Image = global::Ares.Editor.Controls.Properties.Resources.edit;
            this.editButton.Name = "editButton";
            this.editButton.UseCompatibleTextRendering = true;
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Image = global::Ares.Editor.Controls.Properties.Resources.decrease_enabled;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.UseCompatibleTextRendering = true;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Image = global::Ares.Editor.Controls.Properties.Resources.increase;
            this.addButton.Name = "addButton";
            this.addButton.UseCompatibleTextRendering = true;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // downButton
            // 
            resources.ApplyResources(this.downButton, "downButton");
            this.downButton.Image = global::Ares.Editor.Controls.Properties.Resources.MoveDown;
            this.downButton.Name = "downButton";
            this.downButton.UseCompatibleTextRendering = true;
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            resources.ApplyResources(this.upButton, "upButton");
            this.upButton.Image = global::Ares.Editor.Controls.Properties.Resources.MoveUp;
            this.upButton.Name = "upButton";
            this.upButton.UseCompatibleTextRendering = true;
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // MacroControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "MacroControl";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.elementsGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView elementsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn dataGridViewNumericUpDownColumn1;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn dataGridViewNumericUpDownColumn2;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn commandCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn conditionColumn;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button editButton;
    }
}
