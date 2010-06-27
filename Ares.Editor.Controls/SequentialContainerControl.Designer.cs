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
                foreach (int key in m_ElementsToRows.Keys)
                {
                    Actions.ElementChanges.Instance.RemoveListener(key, Update);
                }
                if (m_Container != null)
                    Actions.ElementChanges.Instance.RemoveListener(m_Container.Id, Update);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequentialContainerControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.elementsGrid = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewNumericUpDownColumn1 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.dataGridViewNumericUpDownColumn2 = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.nameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fixedDelayCol = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.randomDelayCol = new DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.elementsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.elementsGrid);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // elementsGrid
            // 
            this.elementsGrid.AllowUserToAddRows = false;
            this.elementsGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.elementsGrid.BackgroundColor = System.Drawing.SystemColors.Control;
            this.elementsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.elementsGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.elementsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.elementsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.elementsGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameCol,
            this.fixedDelayCol,
            this.randomDelayCol});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.elementsGrid.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.elementsGrid, "elementsGrid");
            this.elementsGrid.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.elementsGrid.Name = "elementsGrid";
            this.elementsGrid.RowHeadersVisible = false;
            this.elementsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.elementsGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.elementsGrid_CellEndEdit);
            this.elementsGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.elementsGrid_RowsRemoved);
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
            // nameCol
            // 
            this.nameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.nameCol, "nameCol");
            this.nameCol.Name = "nameCol";
            this.nameCol.ReadOnly = true;
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
            this.fixedDelayCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            this.randomDelayCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SequentialContainerControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SequentialContainerControl";
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
        private System.Windows.Forms.DataGridViewTextBoxColumn nameCol;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn fixedDelayCol;
        private DataGridViewNumericUpDownElements.DataGridViewNumericUpDownColumn randomDelayCol;
    }
}
