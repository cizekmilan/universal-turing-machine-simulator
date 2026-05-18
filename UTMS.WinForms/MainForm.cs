using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Hlavní formulář aplikace.
    /// </summary>
    public partial class MainForm : Form
    {
        private Bitmap tapeBitmap;
        private TuringSimulator simulator;
        private string summary = "";

        public MainForm()
        {
            InitializeComponent();
            tapeBitmap = new Bitmap(panelTapeCanvas.Width, panelTapeCanvas.Height);
        }

        private void Simulator_TransitionExecuting(int index)
        {
            listProgramTransitions.Items[index].Selected = true;
            listProgramTransitions.EnsureVisible(index);
        }

        private void Simulator_TransitionLoaded(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove)
        {
            ListViewItem viewItem = new ListViewItem(new string[] { inputState, inputSymbol.ToString(), outputState, outputSymbol.ToString(), headMove.ToString() });
            listProgramTransitions.Items.Add(viewItem);
        }

        private void Simulator_TuringTransitionCompleted(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now.AddMilliseconds(trackSimulationDelay.Value);
            while (dt > DateTime.Now)
            {
                Application.DoEvents();
            }
        }

        private void Simulator_TapeChanged(object sender, EventArgs e)
        {
            DrawTape();
            panelTapeCanvas.Invalidate();
        }

        private void Reset()
        {
            txtProgramFile.Text = openProgramDialog.FileName;
            simulator = new TuringSimulator();
            simulator.TapeChanged += Simulator_TapeChanged;
            simulator.TransitionLoaded += Simulator_TransitionLoaded;
            simulator.TuringTransitionCompleted += Simulator_TuringTransitionCompleted;
            simulator.TransitionExecuting += Simulator_TransitionExecuting;
            simulator.SyntaxError += Simulator_SyntaxError;
            simulator.InputDataLoaded += Simulator_InputDataLoaded;
            listProgramTransitions.Items.Clear();
            if (tapeBitmap != null)
                tapeBitmap.Dispose();
            tapeBitmap = new Bitmap(panelTapeCanvas.Width, panelTapeCanvas.Height);
            string errorMessage;
            if (simulator.LoadProgram(txtProgramFile.Text.Trim(), out errorMessage))
            {
                btnRunMachine.Enabled = true;
                btnStepMachine.Enabled = true;
                btnSetInputData.Enabled = true;
                btnResetMachine.Enabled = false;
                txtSummary.ResetText();
                panelTapeCanvas.Refresh();
            }
            else
            {
                MessageBox.Show("Error: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBrowseProgram_Click(object sender, EventArgs e)
        {
            openProgramDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            openProgramDialog.FileName = "";
            openProgramDialog.FilterIndex = 1;
            if (openProgramDialog.ShowDialog() == DialogResult.OK)
            {
                Reset();
            }
        }

        private void Simulator_InputDataLoaded(string inputData)
        {
            txtInputData.Text = inputData;
        }

        private void panelTapeCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (tapeBitmap != null)
                e.Graphics.DrawImage(tapeBitmap, new PointF(0, 0));
        }

        private void btnRunMachine_Click(object sender, EventArgs e)
        {
            btnStepMachine.Enabled = false;
            btnRunMachine.Enabled = false;
            txtSummary.ResetText();
            summary = simulator.Run(false);
            txtSummary.Text = summary;
            btnStepMachine.Enabled = false;
            btnRunMachine.Enabled = false;
            btnResetMachine.Enabled = true;
        }

        private void Simulator_SyntaxError(string description, string line)
        {
            MessageBox.Show("Line syntax is invalid: " + description + "\n\n" + line, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnStepMachine_Click(object sender, EventArgs e)
        {
            btnRunMachine.Enabled = false;
            btnResetMachine.Enabled = true;
            txtSummary.ResetText();
            summary = simulator.Run(true);
            if (summary != "")
            {
                txtSummary.ResetText();
                txtSummary.Text = summary;
                btnStepMachine.Enabled = false;
                btnRunMachine.Enabled = false;
            }
        }

        private void btnSetInputData_Click(object sender, EventArgs e)
        {
            simulator.SetTapeData(txtInputData.Text);
            btnSetInputData.Enabled = false;
        }

        private void btnResetMachine_Click(object sender, EventArgs e)
        {
            simulator = null;
            Reset();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            tapeBitmap?.Dispose();
            base.OnFormClosed(e);
        }

        private void DrawTape()
        {
            if (simulator == null || simulator.Machine == null || tapeBitmap == null)
                return;

            TuringMachine machine = simulator.Machine;
            SimulationStep step = simulator.LastStep;
            string previousState = step == null ? machine.CurrentState() : step.InputState;
            char input = step == null ? machine.ReadSymbol() : step.InputSymbol;
            string nextState = step == null ? "?" : step.OutputState;
            char output = step == null ? '0' : step.OutputSymbol;
            char move = step == null || !step.TransitionFound ? '0' : step.HeadMove;

            using (Graphics surface = Graphics.FromImage(tapeBitmap))
            using (Font drawFont = new Font("Arial", 10))
            using (SolidBrush solidbr = new SolidBrush(Color.Black))
            {
                int index = 0;
                surface.Clear(Color.Khaki);
                foreach (char ch in machine.Cells)
                {
                    if (index == machine.HeadIndex())
                    {
                        solidbr.Color = Color.Red;
                        using (Pen pen = new Pen(solidbr))
                        {
                            surface.DrawLine(pen, new Point(10 * index, 20), new Point(10 * index, 120));
                        }

                        StringFormat strformat = new StringFormat();
                        strformat.FormatFlags = StringFormatFlags.DirectionVertical;
                        Rectangle r = new Rectangle(new Point(10 * (index - 2), 40), new Size(20, 100));
                        using (SolidBrush blueBrush = new SolidBrush(Color.Blue))
                        using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                        {
                            surface.FillRectangle(blueBrush, r);
                            surface.DrawString("Turing machine", drawFont, whiteBrush, new Point(10 * (index - 2), 45), strformat);
                        }
                        surface.DrawString(string.Format("Current state: {0}", previousState), drawFont, solidbr, new Point(10 * (index + 1), 55));
                        surface.DrawString(string.Format("Read: {0}", input), drawFont, solidbr, new Point(10 * (index + 1), 70));
                        if (move != '0')
                        {
                            surface.DrawString(string.Format("({0},{1},{2},{3},{4})", previousState, input, nextState, output, move), drawFont, solidbr, new Point(10 * (index + 1), 40));
                            surface.DrawString(string.Format("New state: {0}", nextState), drawFont, solidbr, new Point(10 * (index + 1), 85));
                            surface.DrawString(string.Format("Write: {0}", output), drawFont, solidbr, new Point(10 * (index + 1), 100));
                            surface.DrawString(string.Format("Head: {0}", move), drawFont, solidbr, new Point(10 * (index + 1), 115));
                        }

                        using (SolidBrush navyBrush = new SolidBrush(Color.Navy))
                        using (Pen navyPen = new Pen(navyBrush))
                        {
                            surface.DrawRectangle(navyPen, new Rectangle(10 * (index - 2), 40, 200, 100));
                        }
                    }
                    else if (ch == '1')
                        solidbr.Color = Color.Black;
                    else if (ch != Tape.BlankSymbol)
                        solidbr.Color = Color.Blue;
                    else
                        solidbr.Color = Color.White;

                    surface.DrawString(ch.ToString(), drawFont, solidbr, 10 * index, 10);
                    index++;
                }
            }
        }
    }
}
