namespace UTMS.WinForms
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            txtSummary = new System.Windows.Forms.RichTextBox();
            lblTape = new System.Windows.Forms.Label();
            txtProgramFile = new System.Windows.Forms.TextBox();
            lblProgramFile = new System.Windows.Forms.Label();
            txtInputAlphabet = new System.Windows.Forms.TextBox();
            lblInputAlphabet = new System.Windows.Forms.Label();
            txtTapeAlphabet = new System.Windows.Forms.TextBox();
            lblTapeAlphabet = new System.Windows.Forms.Label();
            txtBlankSymbol = new System.Windows.Forms.TextBox();
            lblBlankSymbol = new System.Windows.Forms.Label();
            grpMachineDefinition = new System.Windows.Forms.GroupBox();
            txtInputData = new System.Windows.Forms.TextBox();
            lblInputData = new System.Windows.Forms.Label();
            btnSetInputData = new System.Windows.Forms.Button();
            btnRunMachine = new System.Windows.Forms.Button();
            panelSimulationStatus = new System.Windows.Forms.Panel();
            lblMachineStateStatus = new System.Windows.Forms.Label();
            lblHeadStatus = new System.Windows.Forms.Label();
            lblReadStatus = new System.Windows.Forms.Label();
            lblStepStatus = new System.Windows.Forms.Label();
            lblRunStatus = new System.Windows.Forms.Label();
            lblLastTransitionStatus = new System.Windows.Forms.Label();
            panelTapeCanvas = new System.Windows.Forms.Panel();
            lblSummary = new System.Windows.Forms.Label();
            openProgramDialog = new System.Windows.Forms.OpenFileDialog();
            saveProgramDialog = new System.Windows.Forms.SaveFileDialog();
            saveGraphDialog = new System.Windows.Forms.SaveFileDialog();
            lblSimulationSpeed = new System.Windows.Forms.Label();
            trackSimulationDelay = new System.Windows.Forms.TrackBar();
            lblSlower = new System.Windows.Forms.Label();
            lblFaster = new System.Windows.Forms.Label();
            listProgramTransitions = new System.Windows.Forms.ListView();
            colInputState = new System.Windows.Forms.ColumnHeader();
            colInputSymbol = new System.Windows.Forms.ColumnHeader();
            colOutputState = new System.Windows.Forms.ColumnHeader();
            colOutputSymbol = new System.Windows.Forms.ColumnHeader();
            colHeadMove = new System.Windows.Forms.ColumnHeader();
            btnStepMachine = new System.Windows.Forms.Button();
            btnPauseMachine = new System.Windows.Forms.Button();
            btnResetMachine = new System.Windows.Forms.Button();
            mainMenu = new System.Windows.Forms.MenuStrip();
            menuFile = new System.Windows.Forms.ToolStripMenuItem();
            menuOpenProgram = new System.Windows.Forms.ToolStripMenuItem();
            menuSaveProgram = new System.Windows.Forms.ToolStripMenuItem();
            menuSaveProgramAs = new System.Windows.Forms.ToolStripMenuItem();
            menuFileSeparatorBeforeExit = new System.Windows.Forms.ToolStripSeparator();
            menuExitApplication = new System.Windows.Forms.ToolStripMenuItem();
            menuSettings = new System.Windows.Forms.ToolStripMenuItem();
            menuHeadMovesOverTape = new System.Windows.Forms.ToolStripMenuItem();
            menuTapeFollowsHead = new System.Windows.Forms.ToolStripMenuItem();
            menuSettingsSeparatorBeforeSound = new System.Windows.Forms.ToolStripSeparator();
            menuSoundEffects = new System.Windows.Forms.ToolStripMenuItem();
            menuTools = new System.Windows.Forms.ToolStripMenuItem();
            menuEditTransitions = new System.Windows.Forms.ToolStripMenuItem();
            menuExportGraph = new System.Windows.Forms.ToolStripMenuItem();
            simulationTimer = new System.Windows.Forms.Timer(components);
            validationErrors = new System.Windows.Forms.ErrorProvider(components);
            grpMachineDefinition.SuspendLayout();
            panelSimulationStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackSimulationDelay).BeginInit();
            ((System.ComponentModel.ISupportInitialize)validationErrors).BeginInit();
            mainMenu.SuspendLayout();
            SuspendLayout();
            // 
            // txtSummary
            // 
            txtSummary.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            txtSummary.BackColor = System.Drawing.SystemColors.InactiveBorder;
            txtSummary.Location = new System.Drawing.Point(14, 621);
            txtSummary.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtSummary.Name = "txtSummary";
            txtSummary.ReadOnly = true;
            txtSummary.Size = new System.Drawing.Size(826, 103);
            txtSummary.TabIndex = 9;
            txtSummary.TabStop = false;
            txtSummary.Text = "";
            // 
            // lblTape
            // 
            lblTape.AutoSize = true;
            lblTape.Location = new System.Drawing.Point(14, 322);
            lblTape.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTape.Name = "lblTape";
            lblTape.Size = new System.Drawing.Size(82, 15);
            lblTape.TabIndex = 3;
            lblTape.Text = "Machine tape:";
            // 
            // txtProgramFile
            // 
            txtProgramFile.Location = new System.Drawing.Point(111, 93);
            txtProgramFile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtProgramFile.Name = "txtProgramFile";
            txtProgramFile.ReadOnly = true;
            txtProgramFile.Size = new System.Drawing.Size(298, 23);
            txtProgramFile.TabIndex = 8;
            txtProgramFile.TabStop = false;
            // 
            // lblProgramFile
            // 
            lblProgramFile.AutoSize = true;
            lblProgramFile.Location = new System.Drawing.Point(14, 96);
            lblProgramFile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblProgramFile.Name = "lblProgramFile";
            lblProgramFile.Size = new System.Drawing.Size(75, 15);
            lblProgramFile.TabIndex = 5;
            lblProgramFile.Text = "Program file:";
            // 
            // txtInputAlphabet
            // 
            txtInputAlphabet.Location = new System.Drawing.Point(97, 25);
            txtInputAlphabet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtInputAlphabet.Name = "txtInputAlphabet";
            txtInputAlphabet.ReadOnly = true;
            txtInputAlphabet.Size = new System.Drawing.Size(100, 23);
            txtInputAlphabet.TabIndex = 3;
            txtInputAlphabet.TabStop = false;
            // 
            // lblInputAlphabet
            // 
            lblInputAlphabet.AutoSize = true;
            lblInputAlphabet.Location = new System.Drawing.Point(10, 29);
            lblInputAlphabet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInputAlphabet.Name = "lblInputAlphabet";
            lblInputAlphabet.Size = new System.Drawing.Size(87, 15);
            lblInputAlphabet.TabIndex = 22;
            lblInputAlphabet.Text = "Input alphabet:";
            // 
            // txtTapeAlphabet
            // 
            txtTapeAlphabet.Location = new System.Drawing.Point(307, 25);
            txtTapeAlphabet.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtTapeAlphabet.Name = "txtTapeAlphabet";
            txtTapeAlphabet.ReadOnly = true;
            txtTapeAlphabet.Size = new System.Drawing.Size(137, 23);
            txtTapeAlphabet.TabIndex = 4;
            txtTapeAlphabet.TabStop = false;
            // 
            // lblTapeAlphabet
            // 
            lblTapeAlphabet.AutoSize = true;
            lblTapeAlphabet.Location = new System.Drawing.Point(204, 29);
            lblTapeAlphabet.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblTapeAlphabet.Name = "lblTapeAlphabet";
            lblTapeAlphabet.Size = new System.Drawing.Size(83, 15);
            lblTapeAlphabet.TabIndex = 24;
            lblTapeAlphabet.Text = "Tape alphabet:";
            // 
            // txtBlankSymbol
            // 
            txtBlankSymbol.Location = new System.Drawing.Point(97, 54);
            txtBlankSymbol.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBlankSymbol.MaxLength = 1;
            txtBlankSymbol.Name = "txtBlankSymbol";
            txtBlankSymbol.Size = new System.Drawing.Size(46, 23);
            txtBlankSymbol.TabIndex = 0;
            txtBlankSymbol.TextChanged += MachineDefinitionField_TextChanged;
            // 
            // lblBlankSymbol
            // 
            lblBlankSymbol.AutoSize = true;
            lblBlankSymbol.Location = new System.Drawing.Point(10, 57);
            lblBlankSymbol.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblBlankSymbol.Name = "lblBlankSymbol";
            lblBlankSymbol.Size = new System.Drawing.Size(81, 15);
            lblBlankSymbol.TabIndex = 26;
            lblBlankSymbol.Text = "Blank symbol:";
            // 
            // grpMachineDefinition
            // 
            grpMachineDefinition.Controls.Add(txtInputData);
            grpMachineDefinition.Controls.Add(lblInputData);
            grpMachineDefinition.Controls.Add(txtInputAlphabet);
            grpMachineDefinition.Controls.Add(lblInputAlphabet);
            grpMachineDefinition.Controls.Add(txtTapeAlphabet);
            grpMachineDefinition.Controls.Add(lblTapeAlphabet);
            grpMachineDefinition.Controls.Add(txtBlankSymbol);
            grpMachineDefinition.Controls.Add(lblBlankSymbol);
            grpMachineDefinition.Controls.Add(btnSetInputData);
            grpMachineDefinition.Location = new System.Drawing.Point(14, 122);
            grpMachineDefinition.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMachineDefinition.Name = "grpMachineDefinition";
            grpMachineDefinition.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            grpMachineDefinition.Size = new System.Drawing.Size(453, 147);
            grpMachineDefinition.TabIndex = 1;
            grpMachineDefinition.TabStop = false;
            grpMachineDefinition.Text = "Machine definition";
            // 
            // txtInputData
            // 
            txtInputData.Location = new System.Drawing.Point(8, 109);
            txtInputData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtInputData.Name = "txtInputData";
            txtInputData.Size = new System.Drawing.Size(346, 23);
            txtInputData.TabIndex = 1;
            txtInputData.TextChanged += MachineDefinitionField_TextChanged;
            // 
            // lblInputData
            // 
            lblInputData.AutoSize = true;
            lblInputData.Location = new System.Drawing.Point(8, 91);
            lblInputData.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblInputData.Name = "lblInputData";
            lblInputData.Size = new System.Drawing.Size(107, 15);
            lblInputData.TabIndex = 8;
            lblInputData.Text = "Input data on tape:";
            // 
            // btnSetInputData
            // 
            btnSetInputData.Enabled = false;
            btnSetInputData.Location = new System.Drawing.Point(358, 109);
            btnSetInputData.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnSetInputData.Name = "btnSetInputData";
            btnSetInputData.Size = new System.Drawing.Size(88, 24);
            btnSetInputData.TabIndex = 2;
            btnSetInputData.Text = "Set";
            btnSetInputData.UseVisualStyleBackColor = true;
            btnSetInputData.Click += btnSetInputData_Click;
            // 
            // btnRunMachine
            // 
            btnRunMachine.BackColor = System.Drawing.SystemColors.Control;
            btnRunMachine.Enabled = false;
            btnRunMachine.Location = new System.Drawing.Point(188, 276);
            btnRunMachine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnRunMachine.Name = "btnRunMachine";
            btnRunMachine.Size = new System.Drawing.Size(175, 45);
            btnRunMachine.TabIndex = 2;
            btnRunMachine.Text = "Run Turing machine";
            btnRunMachine.UseVisualStyleBackColor = false;
            btnRunMachine.Click += btnRunMachine_Click;
            // 
            // panelSimulationStatus
            // 
            panelSimulationStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelSimulationStatus.BackColor = System.Drawing.Color.FromArgb(246, 248, 250);
            panelSimulationStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelSimulationStatus.Controls.Add(lblMachineStateStatus);
            panelSimulationStatus.Controls.Add(lblHeadStatus);
            panelSimulationStatus.Controls.Add(lblReadStatus);
            panelSimulationStatus.Controls.Add(lblStepStatus);
            panelSimulationStatus.Controls.Add(lblRunStatus);
            panelSimulationStatus.Controls.Add(lblLastTransitionStatus);
            panelSimulationStatus.Location = new System.Drawing.Point(14, 340);
            panelSimulationStatus.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelSimulationStatus.Name = "panelSimulationStatus";
            panelSimulationStatus.Size = new System.Drawing.Size(826, 58);
            panelSimulationStatus.TabIndex = 29;
            // 
            // lblMachineStateStatus
            // 
            lblMachineStateStatus.AutoEllipsis = true;
            lblMachineStateStatus.BackColor = System.Drawing.Color.FromArgb(230, 241, 255);
            lblMachineStateStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblMachineStateStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblMachineStateStatus.ForeColor = System.Drawing.Color.FromArgb(9, 105, 218);
            lblMachineStateStatus.Location = new System.Drawing.Point(18, 14);
            lblMachineStateStatus.Name = "lblMachineStateStatus";
            lblMachineStateStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            lblMachineStateStatus.Size = new System.Drawing.Size(126, 30);
            lblMachineStateStatus.TabIndex = 0;
            lblMachineStateStatus.Text = "State";
            lblMachineStateStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHeadStatus
            // 
            lblHeadStatus.AutoEllipsis = true;
            lblHeadStatus.BackColor = System.Drawing.Color.FromArgb(255, 248, 197);
            lblHeadStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblHeadStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblHeadStatus.ForeColor = System.Drawing.Color.FromArgb(154, 103, 0);
            lblHeadStatus.Location = new System.Drawing.Point(154, 14);
            lblHeadStatus.Name = "lblHeadStatus";
            lblHeadStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            lblHeadStatus.Size = new System.Drawing.Size(102, 30);
            lblHeadStatus.TabIndex = 1;
            lblHeadStatus.Text = "Head";
            lblHeadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblReadStatus
            // 
            lblReadStatus.AutoEllipsis = true;
            lblReadStatus.BackColor = System.Drawing.Color.FromArgb(246, 248, 250);
            lblReadStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblReadStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblReadStatus.ForeColor = System.Drawing.Color.FromArgb(87, 96, 106);
            lblReadStatus.Location = new System.Drawing.Point(266, 14);
            lblReadStatus.Name = "lblReadStatus";
            lblReadStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            lblReadStatus.Size = new System.Drawing.Size(92, 30);
            lblReadStatus.TabIndex = 2;
            lblReadStatus.Text = "Read";
            lblReadStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStepStatus
            // 
            lblStepStatus.AutoEllipsis = true;
            lblStepStatus.BackColor = System.Drawing.Color.FromArgb(246, 248, 250);
            lblStepStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblStepStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblStepStatus.ForeColor = System.Drawing.Color.FromArgb(87, 96, 106);
            lblStepStatus.Location = new System.Drawing.Point(368, 14);
            lblStepStatus.Name = "lblStepStatus";
            lblStepStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            lblStepStatus.Size = new System.Drawing.Size(104, 30);
            lblStepStatus.TabIndex = 3;
            lblStepStatus.Text = "Step";
            lblStepStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblRunStatus
            // 
            lblRunStatus.AutoEllipsis = true;
            lblRunStatus.BackColor = System.Drawing.Color.FromArgb(246, 248, 250);
            lblRunStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblRunStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblRunStatus.ForeColor = System.Drawing.Color.FromArgb(87, 96, 106);
            lblRunStatus.Location = new System.Drawing.Point(482, 14);
            lblRunStatus.Name = "lblRunStatus";
            lblRunStatus.Padding = new System.Windows.Forms.Padding(8, 0, 0, 1);
            lblRunStatus.Size = new System.Drawing.Size(130, 30);
            lblRunStatus.TabIndex = 4;
            lblRunStatus.Text = "Status";
            lblRunStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLastTransitionStatus
            // 
            lblLastTransitionStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblLastTransitionStatus.AutoEllipsis = true;
            lblLastTransitionStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            lblLastTransitionStatus.ForeColor = System.Drawing.Color.FromArgb(70, 78, 92);
            lblLastTransitionStatus.Location = new System.Drawing.Point(628, 14);
            lblLastTransitionStatus.Name = "lblLastTransitionStatus";
            lblLastTransitionStatus.Size = new System.Drawing.Size(179, 30);
            lblLastTransitionStatus.TabIndex = 5;
            lblLastTransitionStatus.Text = "Ready";
            lblLastTransitionStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelTapeCanvas
            // 
            panelTapeCanvas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelTapeCanvas.BackColor = System.Drawing.SystemColors.ControlLight;
            panelTapeCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            panelTapeCanvas.Location = new System.Drawing.Point(14, 397);
            panelTapeCanvas.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelTapeCanvas.Name = "panelTapeCanvas";
            panelTapeCanvas.Size = new System.Drawing.Size(826, 183);
            panelTapeCanvas.TabIndex = 9;
            panelTapeCanvas.Paint += panelTapeCanvas_Paint;
            panelTapeCanvas.MouseDown += panelTapeCanvas_MouseDown;
            panelTapeCanvas.MouseLeave += panelTapeCanvas_MouseLeave;
            panelTapeCanvas.MouseMove += panelTapeCanvas_MouseMove;
            panelTapeCanvas.MouseUp += panelTapeCanvas_MouseUp;
            panelTapeCanvas.SizeChanged += panelTapeCanvas_SizeChanged;
            // 
            // lblSummary
            // 
            lblSummary.AutoSize = true;
            lblSummary.Location = new System.Drawing.Point(14, 602);
            lblSummary.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSummary.Name = "lblSummary";
            lblSummary.Size = new System.Drawing.Size(61, 15);
            lblSummary.TabIndex = 10;
            lblSummary.Text = "Summary:";
            // 
            // openProgramDialog
            // 
            openProgramDialog.FileName = "openProgramDialog";
            openProgramDialog.Filter = "Turing machine programs (*.tm;*.btm)|*.tm;*.btm|Text machine program (*.tm)|*.tm|Binary machine program (*.btm)|*.btm";
            // 
            // saveProgramDialog
            // 
            saveProgramDialog.Filter = "Text machine program (*.tm)|*.tm|Binary machine program (*.btm)|*.btm";
            // 
            // saveGraphDialog
            // 
            saveGraphDialog.DefaultExt = "dot";
            saveGraphDialog.Filter = "Graphviz DOT graph (*.dot)|*.dot|All files (*.*)|*.*";
            // 
            // lblSimulationSpeed
            // 
            lblSimulationSpeed.AutoSize = true;
            lblSimulationSpeed.Location = new System.Drawing.Point(14, 50);
            lblSimulationSpeed.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSimulationSpeed.Name = "lblSimulationSpeed";
            lblSimulationSpeed.Size = new System.Drawing.Size(101, 15);
            lblSimulationSpeed.TabIndex = 12;
            lblSimulationSpeed.Text = "Simulation speed:";
            // 
            // trackSimulationDelay
            // 
            trackSimulationDelay.Location = new System.Drawing.Point(225, 42);
            trackSimulationDelay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            trackSimulationDelay.LargeChange = 1;
            trackSimulationDelay.Maximum = 5;
            trackSimulationDelay.Minimum = 0;
            trackSimulationDelay.Name = "trackSimulationDelay";
            trackSimulationDelay.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            trackSimulationDelay.Size = new System.Drawing.Size(173, 45);
            trackSimulationDelay.SmallChange = 1;
            trackSimulationDelay.TabIndex = 0;
            trackSimulationDelay.TickFrequency = 1;
            trackSimulationDelay.Value = 4;
            trackSimulationDelay.ValueChanged += trackSimulationDelay_ValueChanged;
            // 
            // lblSlower
            // 
            lblSlower.AutoSize = true;
            lblSlower.Location = new System.Drawing.Point(158, 50);
            lblSlower.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblSlower.Name = "lblSlower";
            lblSlower.Size = new System.Drawing.Size(41, 15);
            lblSlower.TabIndex = 14;
            lblSlower.Text = "slower";
            // 
            // lblFaster
            // 
            lblFaster.AutoSize = true;
            lblFaster.Location = new System.Drawing.Point(405, 50);
            lblFaster.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFaster.Name = "lblFaster";
            lblFaster.Size = new System.Drawing.Size(36, 15);
            lblFaster.TabIndex = 15;
            lblFaster.Text = "faster";
            // 
            // listProgramTransitions
            // 
            listProgramTransitions.AutoArrange = false;
            listProgramTransitions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { colInputState, colInputSymbol, colOutputState, colOutputSymbol, colHeadMove });
            listProgramTransitions.FullRowSelect = true;
            listProgramTransitions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            listProgramTransitions.Location = new System.Drawing.Point(481, 37);
            listProgramTransitions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            listProgramTransitions.MultiSelect = false;
            listProgramTransitions.Name = "listProgramTransitions";
            listProgramTransitions.Size = new System.Drawing.Size(359, 218);
            listProgramTransitions.TabIndex = 6;
            listProgramTransitions.UseCompatibleStateImageBehavior = false;
            listProgramTransitions.View = System.Windows.Forms.View.Details;
            listProgramTransitions.ItemSelectionChanged += listProgramTransitions_ItemSelectionChanged;
            listProgramTransitions.MouseDoubleClick += listProgramTransitions_MouseDoubleClick;
            listProgramTransitions.SizeChanged += listProgramTransitions_SizeChanged;
            // 
            // colInputState
            // 
            colInputState.Text = "Input state";
            // 
            // colInputSymbol
            // 
            colInputSymbol.Text = "Read";
            colInputSymbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colOutputState
            // 
            colOutputState.Text = "Output state";
            colOutputState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colOutputSymbol
            // 
            colOutputSymbol.Text = "Write";
            colOutputSymbol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // colHeadMove
            // 
            colHeadMove.Text = "Head";
            colHeadMove.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStepMachine
            // 
            btnStepMachine.BackColor = System.Drawing.SystemColors.Control;
            btnStepMachine.Enabled = false;
            btnStepMachine.Location = new System.Drawing.Point(481, 276);
            btnStepMachine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnStepMachine.Name = "btnStepMachine";
            btnStepMachine.Size = new System.Drawing.Size(92, 45);
            btnStepMachine.TabIndex = 4;
            btnStepMachine.Text = "Step";
            btnStepMachine.UseVisualStyleBackColor = false;
            btnStepMachine.Click += btnStepMachine_Click;
            // 
            // btnPauseMachine
            // 
            btnPauseMachine.BackColor = System.Drawing.SystemColors.Control;
            btnPauseMachine.Enabled = false;
            btnPauseMachine.Location = new System.Drawing.Point(374, 276);
            btnPauseMachine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnPauseMachine.Name = "btnPauseMachine";
            btnPauseMachine.Size = new System.Drawing.Size(92, 45);
            btnPauseMachine.TabIndex = 3;
            btnPauseMachine.Text = "Pause";
            btnPauseMachine.UseVisualStyleBackColor = false;
            btnPauseMachine.Click += btnPauseMachine_Click;
            // 
            // btnResetMachine
            // 
            btnResetMachine.BackColor = System.Drawing.SystemColors.Control;
            btnResetMachine.Enabled = false;
            btnResetMachine.Location = new System.Drawing.Point(584, 276);
            btnResetMachine.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnResetMachine.Name = "btnResetMachine";
            btnResetMachine.Size = new System.Drawing.Size(92, 45);
            btnResetMachine.TabIndex = 5;
            btnResetMachine.Text = "Reset";
            btnResetMachine.UseVisualStyleBackColor = false;
            btnResetMachine.Click += btnResetMachine_Click;
            // 
            // mainMenu
            // 
            mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { menuFile, menuSettings, menuTools });
            mainMenu.Location = new System.Drawing.Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            mainMenu.Size = new System.Drawing.Size(854, 24);
            mainMenu.TabIndex = 20;
            mainMenu.Text = "mainMenu";
            // 
            // menuFile
            // 
            menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuOpenProgram, menuSaveProgram, menuSaveProgramAs, menuFileSeparatorBeforeExit, menuExitApplication });
            menuFile.Name = "menuFile";
            menuFile.Size = new System.Drawing.Size(37, 20);
            menuFile.Text = "File";
            // 
            // menuOpenProgram
            // 
            menuOpenProgram.Name = "menuOpenProgram";
            menuOpenProgram.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O;
            menuOpenProgram.Size = new System.Drawing.Size(155, 22);
            menuOpenProgram.Text = "Open...";
            menuOpenProgram.Click += menuOpenProgram_Click;
            // 
            // menuSaveProgram
            // 
            menuSaveProgram.Enabled = false;
            menuSaveProgram.Name = "menuSaveProgram";
            menuSaveProgram.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S;
            menuSaveProgram.Size = new System.Drawing.Size(155, 22);
            menuSaveProgram.Text = "Save";
            menuSaveProgram.Click += menuSaveProgram_Click;
            // 
            // menuSaveProgramAs
            // 
            menuSaveProgramAs.Enabled = false;
            menuSaveProgramAs.Name = "menuSaveProgramAs";
            menuSaveProgramAs.Size = new System.Drawing.Size(155, 22);
            menuSaveProgramAs.Text = "Save As...";
            menuSaveProgramAs.Click += menuSaveProgramAs_Click;
            // 
            // menuFileSeparatorBeforeExit
            // 
            menuFileSeparatorBeforeExit.Name = "menuFileSeparatorBeforeExit";
            menuFileSeparatorBeforeExit.Size = new System.Drawing.Size(152, 6);
            // 
            // menuExitApplication
            // 
            menuExitApplication.Name = "menuExitApplication";
            menuExitApplication.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;
            menuExitApplication.Size = new System.Drawing.Size(155, 22);
            menuExitApplication.Text = "Exit";
            menuExitApplication.Click += menuExitApplication_Click;
            // 
            // menuSettings
            // 
            menuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuHeadMovesOverTape, menuTapeFollowsHead, menuSettingsSeparatorBeforeSound, menuSoundEffects });
            menuSettings.Name = "menuSettings";
            menuSettings.Size = new System.Drawing.Size(61, 20);
            menuSettings.Text = "Settings";
            // 
            // menuHeadMovesOverTape
            // 
            menuHeadMovesOverTape.CheckOnClick = true;
            menuHeadMovesOverTape.Name = "menuHeadMovesOverTape";
            menuHeadMovesOverTape.Size = new System.Drawing.Size(196, 22);
            menuHeadMovesOverTape.Text = "Head moves over tape";
            menuHeadMovesOverTape.Click += menuHeadMovesOverTape_Click;
            // 
            // menuTapeFollowsHead
            // 
            menuTapeFollowsHead.CheckOnClick = true;
            menuTapeFollowsHead.Name = "menuTapeFollowsHead";
            menuTapeFollowsHead.Size = new System.Drawing.Size(196, 22);
            menuTapeFollowsHead.Text = "Tape follows head";
            menuTapeFollowsHead.Click += menuTapeFollowsHead_Click;
            // 
            // menuSettingsSeparatorBeforeSound
            // 
            menuSettingsSeparatorBeforeSound.Name = "menuSettingsSeparatorBeforeSound";
            menuSettingsSeparatorBeforeSound.Size = new System.Drawing.Size(193, 6);
            // 
            // menuSoundEffects
            // 
            menuSoundEffects.Checked = true;
            menuSoundEffects.CheckOnClick = true;
            menuSoundEffects.CheckState = System.Windows.Forms.CheckState.Checked;
            menuSoundEffects.Name = "menuSoundEffects";
            menuSoundEffects.Size = new System.Drawing.Size(196, 22);
            menuSoundEffects.Text = "Sound effects";
            // 
            // menuTools
            // 
            menuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { menuEditTransitions, menuExportGraph });
            menuTools.Name = "menuTools";
            menuTools.Size = new System.Drawing.Size(46, 20);
            menuTools.Text = "Tools";
            // 
            // menuEditTransitions
            // 
            menuEditTransitions.Name = "menuEditTransitions";
            menuEditTransitions.Size = new System.Drawing.Size(161, 22);
            menuEditTransitions.Text = "Edit transitions...";
            menuEditTransitions.Click += menuEditTransitions_Click;
            // 
            // menuExportGraph
            // 
            menuExportGraph.Enabled = false;
            menuExportGraph.Name = "menuExportGraph";
            menuExportGraph.Size = new System.Drawing.Size(161, 22);
            menuExportGraph.Text = "Export graph...";
            menuExportGraph.Click += menuExportGraph_Click;
            // 
            // simulationTimer
            // 
            simulationTimer.Tick += simulationTimer_Tick;
            // 
            // validationErrors
            // 
            validationErrors.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(854, 741);
            Controls.Add(btnResetMachine);
            Controls.Add(btnPauseMachine);
            Controls.Add(panelSimulationStatus);
            Controls.Add(grpMachineDefinition);
            Controls.Add(listProgramTransitions);
            Controls.Add(btnStepMachine);
            Controls.Add(lblFaster);
            Controls.Add(lblSlower);
            Controls.Add(trackSimulationDelay);
            Controls.Add(lblSimulationSpeed);
            Controls.Add(lblSummary);
            Controls.Add(panelTapeCanvas);
            Controls.Add(btnRunMachine);
            Controls.Add(lblProgramFile);
            Controls.Add(txtProgramFile);
            Controls.Add(lblTape);
            Controls.Add(txtSummary);
            Controls.Add(mainMenu);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenu;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "UTMS - Universal Turing Machine Simulator";
            panelSimulationStatus.ResumeLayout(false);
            grpMachineDefinition.ResumeLayout(false);
            grpMachineDefinition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackSimulationDelay).EndInit();
            ((System.ComponentModel.ISupportInitialize)validationErrors).EndInit();
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox txtSummary;
        private System.Windows.Forms.Label lblTape;
        private System.Windows.Forms.TextBox txtProgramFile;
        private System.Windows.Forms.Label lblProgramFile;
        private System.Windows.Forms.TextBox txtInputAlphabet;
        private System.Windows.Forms.Label lblInputAlphabet;
        private System.Windows.Forms.TextBox txtTapeAlphabet;
        private System.Windows.Forms.Label lblTapeAlphabet;
        private System.Windows.Forms.TextBox txtBlankSymbol;
        private System.Windows.Forms.Label lblBlankSymbol;
        private System.Windows.Forms.GroupBox grpMachineDefinition;
        private System.Windows.Forms.Button btnRunMachine;
        private System.Windows.Forms.TextBox txtInputData;
        private System.Windows.Forms.Label lblInputData;
        private System.Windows.Forms.Panel panelTapeCanvas;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.OpenFileDialog openProgramDialog;
        private System.Windows.Forms.SaveFileDialog saveProgramDialog;
        private System.Windows.Forms.SaveFileDialog saveGraphDialog;
        private System.Windows.Forms.Label lblSimulationSpeed;
        private System.Windows.Forms.TrackBar trackSimulationDelay;
        private System.Windows.Forms.Label lblSlower;
        private System.Windows.Forms.Label lblFaster;
        private System.Windows.Forms.Button btnStepMachine;
        private System.Windows.Forms.Button btnPauseMachine;
        private System.Windows.Forms.ListView listProgramTransitions;
        private System.Windows.Forms.ColumnHeader colInputState;
        private System.Windows.Forms.ColumnHeader colInputSymbol;
        private System.Windows.Forms.ColumnHeader colOutputState;
        private System.Windows.Forms.ColumnHeader colOutputSymbol;
        private System.Windows.Forms.ColumnHeader colHeadMove;
        private System.Windows.Forms.Button btnSetInputData;
        private System.Windows.Forms.Button btnResetMachine;
        private System.Windows.Forms.Panel panelSimulationStatus;
        private System.Windows.Forms.Label lblMachineStateStatus;
        private System.Windows.Forms.Label lblHeadStatus;
        private System.Windows.Forms.Label lblReadStatus;
        private System.Windows.Forms.Label lblStepStatus;
        private System.Windows.Forms.Label lblRunStatus;
        private System.Windows.Forms.Label lblLastTransitionStatus;
        private System.Windows.Forms.MenuStrip mainMenu;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuOpenProgram;
        private System.Windows.Forms.ToolStripMenuItem menuSaveProgram;
        private System.Windows.Forms.ToolStripMenuItem menuSaveProgramAs;
        private System.Windows.Forms.ToolStripSeparator menuFileSeparatorBeforeExit;
        private System.Windows.Forms.ToolStripMenuItem menuExitApplication;
        private System.Windows.Forms.ToolStripMenuItem menuSettings;
        private System.Windows.Forms.ToolStripMenuItem menuHeadMovesOverTape;
        private System.Windows.Forms.ToolStripMenuItem menuTapeFollowsHead;
        private System.Windows.Forms.ToolStripSeparator menuSettingsSeparatorBeforeSound;
        private System.Windows.Forms.ToolStripMenuItem menuSoundEffects;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuEditTransitions;
        private System.Windows.Forms.ToolStripMenuItem menuExportGraph;
        private System.Windows.Forms.Timer simulationTimer;
        private System.Windows.Forms.ErrorProvider validationErrors;
    }
}

