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
    partial class SequentialContainerControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequentialContainerControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.elementsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewNumericUpDownColumn1 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.dataGridViewNumericUpDownColumn2 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.columnMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.artistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.albumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fixedDelayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomDelayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.artistColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.albumColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fixedDelayCol = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.randomDelayCol = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elementsGrid)).BeginInit();
            this.columnMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.downButton);
            this.groupBox1.Controls.Add(this.upButton);
            this.groupBox1.Controls.Add(this.elementsGrid);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
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
            this.nameCol,
            this.artistColumn,
            this.albumColumn,
            this.fixedDelayCol,
            this.randomDelayCol});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.elementsGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.elementsGrid.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.elementsGrid.Name = "elementsGrid";
            this.elementsGrid.RowHeadersVisible = false;
            this.elementsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.elementsGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.elementsGrid_CellDoubleClick);
            this.elementsGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.elementsGrid_CellEndEdit);
            this.elementsGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.elementsGrid_ColumnHeaderMouseClick);
            this.elementsGrid.SelectionChanged += new System.EventHandler(this.elementsGrid_SelectionChanged);
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
            // columnMenu
            // 
            this.columnMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.columnMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nameToolStripMenuItem,
            this.artistToolStripMenuItem,
            this.albumToolStripMenuItem,
            this.fixedDelayToolStripMenuItem,
            this.randomDelayToolStripMenuItem});
            this.columnMenu.Name = "columnMenu";
            resources.ApplyResources(this.columnMenu, "columnMenu");
            // 
            // nameToolStripMenuItem
            // 
            this.nameToolStripMenuItem.Checked = true;
            this.nameToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.nameToolStripMenuItem, "nameToolStripMenuItem");
            this.nameToolStripMenuItem.Name = "nameToolStripMenuItem";
            // 
            // artistToolStripMenuItem
            // 
            this.artistToolStripMenuItem.CheckOnClick = true;
            this.artistToolStripMenuItem.Name = "artistToolStripMenuItem";
            resources.ApplyResources(this.artistToolStripMenuItem, "artistToolStripMenuItem");
            this.artistToolStripMenuItem.Click += new System.EventHandler(this.artistToolStripMenuItem_Click);
            // 
            // albumToolStripMenuItem
            // 
            this.albumToolStripMenuItem.CheckOnClick = true;
            this.albumToolStripMenuItem.Name = "albumToolStripMenuItem";
            resources.ApplyResources(this.albumToolStripMenuItem, "albumToolStripMenuItem");
            this.albumToolStripMenuItem.Click += new System.EventHandler(this.albumToolStripMenuItem_Click);
            // 
            // fixedDelayToolStripMenuItem
            // 
            this.fixedDelayToolStripMenuItem.Checked = true;
            this.fixedDelayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.fixedDelayToolStripMenuItem, "fixedDelayToolStripMenuItem");
            this.fixedDelayToolStripMenuItem.Name = "fixedDelayToolStripMenuItem";
            // 
            // randomDelayToolStripMenuItem
            // 
            this.randomDelayToolStripMenuItem.Checked = true;
            this.randomDelayToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.randomDelayToolStripMenuItem, "randomDelayToolStripMenuItem");
            this.randomDelayToolStripMenuItem.Name = "randomDelayToolStripMenuItem";
            // 
            // nameCol
            // 
            this.nameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.nameCol, "nameCol");
            this.nameCol.Name = "nameCol";
            this.nameCol.ReadOnly = true;
            this.nameCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // artistColumn
            // 
            this.artistColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.artistColumn, "artistColumn");
            this.artistColumn.Name = "artistColumn";
            this.artistColumn.ReadOnly = true;
            // 
            // albumColumn
            // 
            this.albumColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            resources.ApplyResources(this.albumColumn, "albumColumn");
            this.albumColumn.Name = "albumColumn";
            this.albumColumn.ReadOnly = true;
            // 
            // fixedDelayCol
            // 
            this.fixedDelayCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.fixedDelayCol, "fixedDelayCol");
            this.fixedDelayCol.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.fixedDelayCol.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.fixedDelayCol.Name = "fixedDelayCol";
            // 
            // randomDelayCol
            // 
            this.randomDelayCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.randomDelayCol, "randomDelayCol");
            this.randomDelayCol.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.randomDelayCol.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.randomDelayCol.Name = "randomDelayCol";
            // 
            // SequentialContainerControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SequentialContainerControl";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.elementsGrid)).EndInit();
            this.columnMenu.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip columnMenu;
        private System.Windows.Forms.ToolStripMenuItem nameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem artistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem albumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fixedDelayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomDelayToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn artistColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn albumColumn;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn fixedDelayCol;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn randomDelayCol;
    }
}
