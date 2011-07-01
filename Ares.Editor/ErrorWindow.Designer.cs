/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
namespace Ares.Editor
{
    partial class ErrorWindow
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
            if (disposing)
            {
                Ares.ModelInfo.ModelChecks.Instance.ErrorsUpdated -= new System.EventHandler<System.EventArgs>(ErrorsUpdated);
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorWindow));
            this.messagesGrid = new System.Windows.Forms.DataGridView();
            this.iconColumn = new System.Windows.Forms.DataGridViewImageColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.messagesGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // messagesGrid
            // 
            this.messagesGrid.AllowUserToAddRows = false;
            this.messagesGrid.AllowUserToDeleteRows = false;
            this.messagesGrid.AllowUserToResizeColumns = false;
            this.messagesGrid.AllowUserToResizeRows = false;
            this.messagesGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.messagesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.messagesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iconColumn,
            this.Message});
            resources.ApplyResources(this.messagesGrid, "messagesGrid");
            this.messagesGrid.Name = "messagesGrid";
            this.messagesGrid.ReadOnly = true;
            this.messagesGrid.RowHeadersVisible = false;
            this.messagesGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.messagesGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.messagesGrid_CellDoubleClick);
            // 
            // iconColumn
            // 
            this.iconColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            resources.ApplyResources(this.iconColumn, "iconColumn");
            this.iconColumn.Name = "iconColumn";
            this.iconColumn.ReadOnly = true;
            // 
            // Message
            // 
            this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.Message, "Message");
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            this.Message.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ErrorWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.messagesGrid);
            this.Name = "ErrorWindow";
            this.Load += new System.EventHandler(this.ErrorWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.messagesGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView messagesGrid;
        private System.Windows.Forms.DataGridViewImageColumn iconColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
    }
}