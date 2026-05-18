namespace TuringMachineSimulator
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.txtSummary = new System.Windows.Forms.RichTextBox();
            this.lblTape = new System.Windows.Forms.Label();
            this.txtProgramFile = new System.Windows.Forms.TextBox();
            this.lblProgramFile = new System.Windows.Forms.Label();
            this.btnRunMachine = new System.Windows.Forms.Button();
            this.txtInputData = new System.Windows.Forms.TextBox();
            this.lblInputData = new System.Windows.Forms.Label();
            this.panelTapeCanvas = new System.Windows.Forms.Panel();
            this.lblSummary = new System.Windows.Forms.Label();
            this.openProgramDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowseProgram = new System.Windows.Forms.Button();
            this.lblSimulationSpeed = new System.Windows.Forms.Label();
            this.trackSimulationDelay = new System.Windows.Forms.TrackBar();
            this.lblSlower = new System.Windows.Forms.Label();
            this.lblFaster = new System.Windows.Forms.Label();
            this.listProgramTransitions = new System.Windows.Forms.ListView();
            this.colInputState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInputSymbol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOutputState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOutputSymbol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeadMove = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStepMachine = new System.Windows.Forms.Button();
            this.btnSetInputData = new System.Windows.Forms.Button();
            this.btnResetMachine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackSimulationDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSummary
            // 
            this.txtSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSummary.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtSummary.Location = new System.Drawing.Point(12, 490);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(697, 90);
            this.txtSummary.TabIndex = 2;
            this.txtSummary.Text = "";
            // 
            // lblTape
            // 
            this.lblTape.AutoSize = true;
            this.lblTape.Location = new System.Drawing.Point(12, 231);
            this.lblTape.Name = "lblTape";
            this.lblTape.Size = new System.Drawing.Size(75, 13);
            this.lblTape.TabIndex = 3;
            this.lblTape.Text = "Machine tape:";
            // 
            // txtProgramFile
            // 
            this.txtProgramFile.Location = new System.Drawing.Point(95, 71);
            this.txtProgramFile.Name = "txtProgramFile";
            this.txtProgramFile.Size = new System.Drawing.Size(256, 20);
            this.txtProgramFile.TabIndex = 4;
            // 
            // lblProgramFile
            // 
            this.lblProgramFile.AutoSize = true;
            this.lblProgramFile.Location = new System.Drawing.Point(12, 74);
            this.lblProgramFile.Name = "lblProgramFile";
            this.lblProgramFile.Size = new System.Drawing.Size(71, 13);
            this.lblProgramFile.TabIndex = 5;
            this.lblProgramFile.Text = "Program file:";
            // 
            // btnRunMachine
            // 
            this.btnRunMachine.BackColor = System.Drawing.SystemColors.Control;
            this.btnRunMachine.Enabled = false;
            this.btnRunMachine.Location = new System.Drawing.Point(161, 191);
            this.btnRunMachine.Name = "btnRunMachine";
            this.btnRunMachine.Size = new System.Drawing.Size(239, 39);
            this.btnRunMachine.TabIndex = 6;
            this.btnRunMachine.Text = "Run Turing machine";
            this.btnRunMachine.UseVisualStyleBackColor = false;
            this.btnRunMachine.Click += new System.EventHandler(this.btnRunMachine_Click);
            // 
            // txtInputData
            // 
            this.txtInputData.Location = new System.Drawing.Point(15, 127);
            this.txtInputData.Name = "txtInputData";
            this.txtInputData.Size = new System.Drawing.Size(297, 20);
            this.txtInputData.TabIndex = 7;
            // 
            // lblInputData
            // 
            this.lblInputData.AutoSize = true;
            this.lblInputData.Location = new System.Drawing.Point(12, 111);
            this.lblInputData.Name = "lblInputData";
            this.lblInputData.Size = new System.Drawing.Size(204, 13);
            this.lblInputData.TabIndex = 8;
            this.lblInputData.Text = "Input data on tape (blank symbol is #):";
            // 
            // panelTapeCanvas
            // 
            this.panelTapeCanvas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTapeCanvas.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelTapeCanvas.Location = new System.Drawing.Point(12, 247);
            this.panelTapeCanvas.Name = "panelTapeCanvas";
            this.panelTapeCanvas.Size = new System.Drawing.Size(697, 209);
            this.panelTapeCanvas.TabIndex = 9;
            this.panelTapeCanvas.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTapeCanvas_Paint);
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(12, 474);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(53, 13);
            this.lblSummary.TabIndex = 10;
            this.lblSummary.Text = "Summary:";
            // 
            // openProgramDialog
            // 
            this.openProgramDialog.FileName = "openProgramDialog";
            this.openProgramDialog.Filter = "Turing machine program|*.TM|Binary machine program|*.BTM";
            // 
            // btnBrowseProgram
            // 
            this.btnBrowseProgram.Location = new System.Drawing.Point(357, 71);
            this.btnBrowseProgram.Name = "btnBrowseProgram";
            this.btnBrowseProgram.Size = new System.Drawing.Size(36, 21);
            this.btnBrowseProgram.TabIndex = 11;
            this.btnBrowseProgram.Text = "....";
            this.btnBrowseProgram.UseVisualStyleBackColor = true;
            this.btnBrowseProgram.Click += new System.EventHandler(this.btnBrowseProgram_Click);
            // 
            // lblSimulationSpeed
            // 
            this.lblSimulationSpeed.AutoSize = true;
            this.lblSimulationSpeed.Location = new System.Drawing.Point(12, 19);
            this.lblSimulationSpeed.Name = "lblSimulationSpeed";
            this.lblSimulationSpeed.Size = new System.Drawing.Size(91, 13);
            this.lblSimulationSpeed.TabIndex = 12;
            this.lblSimulationSpeed.Text = "Simulation speed:";
            // 
            // trackSimulationDelay
            // 
            this.trackSimulationDelay.Location = new System.Drawing.Point(193, 12);
            this.trackSimulationDelay.Maximum = 3000;
            this.trackSimulationDelay.Minimum = 100;
            this.trackSimulationDelay.Name = "trackSimulationDelay";
            this.trackSimulationDelay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackSimulationDelay.Size = new System.Drawing.Size(148, 45);
            this.trackSimulationDelay.SmallChange = 100;
            this.trackSimulationDelay.TabIndex = 13;
            this.trackSimulationDelay.TickFrequency = 500;
            this.trackSimulationDelay.Value = 1000;
            // 
            // lblSlower
            // 
            this.lblSlower.AutoSize = true;
            this.lblSlower.Location = new System.Drawing.Point(135, 19);
            this.lblSlower.Name = "lblSlower";
            this.lblSlower.Size = new System.Drawing.Size(38, 13);
            this.lblSlower.TabIndex = 14;
            this.lblSlower.Text = "slower";
            // 
            // lblFaster
            // 
            this.lblFaster.AutoSize = true;
            this.lblFaster.Location = new System.Drawing.Point(347, 19);
            this.lblFaster.Name = "lblFaster";
            this.lblFaster.Size = new System.Drawing.Size(34, 13);
            this.lblFaster.TabIndex = 15;
            this.lblFaster.Text = "faster";
            // 
            // listProgramTransitions
            // 
            this.listProgramTransitions.AutoArrange = false;
            this.listProgramTransitions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colInputState,
            this.colInputSymbol,
            this.colOutputState,
            this.colOutputSymbol,
            this.colHeadMove});
            this.listProgramTransitions.FullRowSelect = true;
            this.listProgramTransitions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listProgramTransitions.HideSelection = false;
            this.listProgramTransitions.Location = new System.Drawing.Point(412, 8);
            this.listProgramTransitions.MultiSelect = false;
            this.listProgramTransitions.Name = "listProgramTransitions";
            this.listProgramTransitions.Size = new System.Drawing.Size(308, 166);
            this.listProgramTransitions.TabIndex = 17;
            this.listProgramTransitions.UseCompatibleStateImageBehavior = false;
            this.listProgramTransitions.View = System.Windows.Forms.View.Details;
            // 
            // colInputState
            // 
            this.colInputState.Text = "Input state";
            // 
            // colInputSymbol
            // 
            this.colInputSymbol.Text = "Read";
            this.colInputSymbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colOutputState
            // 
            this.colOutputState.Text = "Output state";
            this.colOutputState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colOutputSymbol
            // 
            this.colOutputSymbol.Text = "Write";
            this.colOutputSymbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colHeadMove
            // 
            this.colHeadMove.Text = "Head";
            this.colHeadMove.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStepMachine
            // 
            this.btnStepMachine.BackColor = System.Drawing.SystemColors.Control;
            this.btnStepMachine.Enabled = false;
            this.btnStepMachine.Location = new System.Drawing.Point(412, 191);
            this.btnStepMachine.Name = "btnStepMachine";
            this.btnStepMachine.Size = new System.Drawing.Size(79, 39);
            this.btnStepMachine.TabIndex = 16;
            this.btnStepMachine.Text = "Step";
            this.btnStepMachine.UseVisualStyleBackColor = false;
            this.btnStepMachine.Click += new System.EventHandler(this.btnStepMachine_Click);
            // 
            // btnSetInputData
            // 
            this.btnSetInputData.Enabled = false;
            this.btnSetInputData.Location = new System.Drawing.Point(318, 127);
            this.btnSetInputData.Name = "btnSetInputData";
            this.btnSetInputData.Size = new System.Drawing.Size(75, 21);
            this.btnSetInputData.TabIndex = 18;
            this.btnSetInputData.Text = "Set";
            this.btnSetInputData.UseVisualStyleBackColor = true;
            this.btnSetInputData.Click += new System.EventHandler(this.btnSetInputData_Click);
            // 
            // btnResetMachine
            // 
            this.btnResetMachine.BackColor = System.Drawing.SystemColors.Control;
            this.btnResetMachine.Enabled = false;
            this.btnResetMachine.Location = new System.Drawing.Point(501, 191);
            this.btnResetMachine.Name = "btnResetMachine";
            this.btnResetMachine.Size = new System.Drawing.Size(79, 39);
            this.btnResetMachine.TabIndex = 19;
            this.btnResetMachine.Text = "Reset";
            this.btnResetMachine.UseVisualStyleBackColor = false;
            this.btnResetMachine.Click += new System.EventHandler(this.btnResetMachine_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(732, 594);
            this.Controls.Add(this.btnResetMachine);
            this.Controls.Add(this.btnSetInputData);
            this.Controls.Add(this.listProgramTransitions);
            this.Controls.Add(this.btnStepMachine);
            this.Controls.Add(this.lblFaster);
            this.Controls.Add(this.lblSlower);
            this.Controls.Add(this.trackSimulationDelay);
            this.Controls.Add(this.lblSimulationSpeed);
            this.Controls.Add(this.btnBrowseProgram);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.panelTapeCanvas);
            this.Controls.Add(this.lblInputData);
            this.Controls.Add(this.txtInputData);
            this.Controls.Add(this.btnRunMachine);
            this.Controls.Add(this.lblProgramFile);
            this.Controls.Add(this.txtProgramFile);
            this.Controls.Add(this.lblTape);
            this.Controls.Add(this.txtSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Turing Machine Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.trackSimulationDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox txtSummary;
        private System.Windows.Forms.Label lblTape;
        private System.Windows.Forms.TextBox txtProgramFile;
        private System.Windows.Forms.Label lblProgramFile;
        private System.Windows.Forms.Button btnRunMachine;
        private System.Windows.Forms.TextBox txtInputData;
        private System.Windows.Forms.Label lblInputData;
        private System.Windows.Forms.Panel panelTapeCanvas;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.OpenFileDialog openProgramDialog;
        private System.Windows.Forms.Button btnBrowseProgram;
        private System.Windows.Forms.Label lblSimulationSpeed;
        private System.Windows.Forms.TrackBar trackSimulationDelay;
        private System.Windows.Forms.Label lblSlower;
        private System.Windows.Forms.Label lblFaster;
        private System.Windows.Forms.Button btnStepMachine;
        private System.Windows.Forms.ListView listProgramTransitions;
        private System.Windows.Forms.ColumnHeader colInputState;
        private System.Windows.Forms.ColumnHeader colInputSymbol;
        private System.Windows.Forms.ColumnHeader colOutputState;
        private System.Windows.Forms.ColumnHeader colOutputSymbol;
        private System.Windows.Forms.ColumnHeader colHeadMove;
        private System.Windows.Forms.Button btnSetInputData;
        private System.Windows.Forms.Button btnResetMachine;
    }
}

