namespace UTMS.WinForms
{
    partial class TransitionEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gridTransitions = new System.Windows.Forms.DataGridView();
            colCurrentState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            colReadSymbol = new System.Windows.Forms.DataGridViewComboBoxColumn();
            colNextState = new System.Windows.Forms.DataGridViewComboBoxColumn();
            colWriteSymbol = new System.Windows.Forms.DataGridViewComboBoxColumn();
            colHeadMove = new System.Windows.Forms.DataGridViewComboBoxColumn();
            bottomPanel = new System.Windows.Forms.Panel();
            statusPanel = new System.Windows.Forms.FlowLayoutPanel();
            lblStates = new System.Windows.Forms.Label();
            lblInputAlphabet = new System.Windows.Forms.Label();
            lblTapeAlphabet = new System.Windows.Forms.Label();
            lblValidation = new System.Windows.Forms.Label();
            buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            btnOk = new System.Windows.Forms.Button();
            btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)gridTransitions).BeginInit();
            bottomPanel.SuspendLayout();
            statusPanel.SuspendLayout();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // gridTransitions
            // 
            gridTransitions.AllowUserToAddRows = true;
            gridTransitions.AllowUserToDeleteRows = true;
            gridTransitions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            gridTransitions.BackgroundColor = System.Drawing.SystemColors.Window;
            gridTransitions.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            gridTransitions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colCurrentState, colReadSymbol, colNextState, colWriteSymbol, colHeadMove });
            gridTransitions.Dock = System.Windows.Forms.DockStyle.Fill;
            gridTransitions.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            gridTransitions.Location = new System.Drawing.Point(0, 0);
            gridTransitions.Name = "gridTransitions";
            gridTransitions.RowHeadersWidth = 28;
            gridTransitions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            gridTransitions.Size = new System.Drawing.Size(744, 291);
            gridTransitions.TabIndex = 0;
            gridTransitions.CellValueChanged += gridTransitions_CellValueChanged;
            gridTransitions.CurrentCellDirtyStateChanged += gridTransitions_CurrentCellDirtyStateChanged;
            gridTransitions.DataError += gridTransitions_DataError;
            gridTransitions.RowsAdded += gridTransitions_RowsAdded;
            gridTransitions.RowsRemoved += gridTransitions_RowsRemoved;
            // 
            // colCurrentState
            // 
            colCurrentState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            colCurrentState.HeaderText = "Current state";
            colCurrentState.Items.AddRange(new object[] { "q0", "q1", "q2", "qF", "<new state>" });
            colCurrentState.Name = "CurrentState";
            // 
            // colReadSymbol
            // 
            colReadSymbol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            colReadSymbol.HeaderText = "Read";
            colReadSymbol.Items.AddRange(new object[] { "0", "1", "<new symbol>" });
            colReadSymbol.Name = "ReadSymbol";
            // 
            // colNextState
            // 
            colNextState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            colNextState.HeaderText = "Next state";
            colNextState.Items.AddRange(new object[] { "q0", "q1", "q2", "qF", "<new state>" });
            colNextState.Name = "NextState";
            // 
            // colWriteSymbol
            // 
            colWriteSymbol.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            colWriteSymbol.HeaderText = "Write";
            colWriteSymbol.Items.AddRange(new object[] { "0", "1", "<new symbol>" });
            colWriteSymbol.Name = "WriteSymbol";
            // 
            // colHeadMove
            // 
            colHeadMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            colHeadMove.HeaderText = "Move";
            colHeadMove.Items.AddRange(new object[] { "L", "R", "S" });
            colHeadMove.Name = "HeadMove";
            // 
            // bottomPanel
            // 
            bottomPanel.Controls.Add(statusPanel);
            bottomPanel.Controls.Add(buttonPanel);
            bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            bottomPanel.Location = new System.Drawing.Point(0, 291);
            bottomPanel.Name = "bottomPanel";
            bottomPanel.Size = new System.Drawing.Size(744, 100);
            bottomPanel.TabIndex = 1;
            // 
            // statusPanel
            // 
            statusPanel.AutoSize = false;
            statusPanel.Controls.Add(lblStates);
            statusPanel.Controls.Add(lblInputAlphabet);
            statusPanel.Controls.Add(lblTapeAlphabet);
            statusPanel.Controls.Add(lblValidation);
            statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            statusPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            statusPanel.Location = new System.Drawing.Point(0, 0);
            statusPanel.Name = "statusPanel";
            statusPanel.Padding = new System.Windows.Forms.Padding(10, 6, 10, 8);
            statusPanel.Size = new System.Drawing.Size(566, 100);
            statusPanel.TabIndex = 0;
            statusPanel.WrapContents = false;
            // 
            // lblStates
            // 
            lblStates.AutoSize = true;
            lblStates.Location = new System.Drawing.Point(10, 8);
            lblStates.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            lblStates.Name = "lblStates";
            lblStates.Size = new System.Drawing.Size(42, 15);
            lblStates.TabIndex = 0;
            lblStates.Text = "States:";
            // 
            // lblInputAlphabet
            // 
            lblInputAlphabet.AutoSize = true;
            lblInputAlphabet.Location = new System.Drawing.Point(10, 27);
            lblInputAlphabet.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            lblInputAlphabet.Name = "lblInputAlphabet";
            lblInputAlphabet.Size = new System.Drawing.Size(88, 15);
            lblInputAlphabet.TabIndex = 1;
            lblInputAlphabet.Text = "Input alphabet:";
            // 
            // lblTapeAlphabet
            // 
            lblTapeAlphabet.AutoSize = true;
            lblTapeAlphabet.Location = new System.Drawing.Point(10, 46);
            lblTapeAlphabet.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            lblTapeAlphabet.Name = "lblTapeAlphabet";
            lblTapeAlphabet.Size = new System.Drawing.Size(86, 15);
            lblTapeAlphabet.TabIndex = 2;
            lblTapeAlphabet.Text = "Tape alphabet:";
            // 
            // lblValidation
            // 
            lblValidation.AutoSize = true;
            lblValidation.Location = new System.Drawing.Point(10, 65);
            lblValidation.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            lblValidation.Name = "lblValidation";
            lblValidation.Size = new System.Drawing.Size(62, 15);
            lblValidation.TabIndex = 3;
            lblValidation.Text = "Validation:";
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(btnOk);
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Dock = System.Windows.Forms.DockStyle.Right;
            buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            buttonPanel.Location = new System.Drawing.Point(566, 0);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            buttonPanel.Size = new System.Drawing.Size(178, 100);
            buttonPanel.TabIndex = 1;
            // 
            // btnOk
            // 
            btnOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnOk.Location = new System.Drawing.Point(3, 4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(82, 27);
            btnOk.TabIndex = 0;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(91, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(82, 27);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // TransitionEditorForm
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(744, 391);
            Controls.Add(gridTransitions);
            Controls.Add(bottomPanel);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TransitionEditorForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Edit transitions";
            ((System.ComponentModel.ISupportInitialize)gridTransitions).EndInit();
            bottomPanel.ResumeLayout(false);
            statusPanel.ResumeLayout(false);
            statusPanel.PerformLayout();
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView gridTransitions;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCurrentState;
        private System.Windows.Forms.DataGridViewComboBoxColumn colReadSymbol;
        private System.Windows.Forms.DataGridViewComboBoxColumn colNextState;
        private System.Windows.Forms.DataGridViewComboBoxColumn colWriteSymbol;
        private System.Windows.Forms.DataGridViewComboBoxColumn colHeadMove;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.FlowLayoutPanel statusPanel;
        private System.Windows.Forms.Label lblStates;
        private System.Windows.Forms.Label lblInputAlphabet;
        private System.Windows.Forms.Label lblTapeAlphabet;
        private System.Windows.Forms.Label lblValidation;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
