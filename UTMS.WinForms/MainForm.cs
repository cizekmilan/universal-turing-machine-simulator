using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using UTMS.Core;

namespace UTMS.WinForms
{
    /// <summary>
    /// Hlavní formulář aplikace.
    /// </summary>
    public partial class MainForm : Form
    {
        private const int NoSelectedTransitionIndex = -1;
        private static readonly int[] AutomaticStepDelayMilliseconds = new int[] { 50, 100, 250, 500, 1000, 2000 };

        private ErrorProvider validationErrors;
        private Color inputDataDefaultBackColor;
        private Color blankSymbolDefaultBackColor;
        private TapeRenderer tapeRenderer;
        private SoundEffectPlayer tapeMoveSound;
        private SoundEffectPlayer machineStopSound;
        private Timer writeHighlightTimer;
        private TuringSimulator simulator;
        private string currentProgramFileName = "";
        private string summary = "";
        private bool lockTransitionSelection;
        private bool updatingTransitionSelection;
        private bool simulationPaused;
        private bool formClosing;
        private bool hasUnsavedChanges;
        private bool suppressDirtyTracking;
        private int currentTransitionIndex = -1;
        private bool tapeFollowsHead;

        /// <summary>
        /// Inicializuje hlavní formulář a připraví kreslicí plochu pásky.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            tapeRenderer = new TapeRenderer(panelTapeCanvas);
            ConfigureSoundEffects();
            ApplyTapeViewMode(false);
            // Zvýraznění aktuálního přechodu musí zůstat viditelné i při ovládání simulace tlačítky.
            listProgramTransitions.HideSelection = false;
            ResizeProgramTransitionColumns();
            validationErrors = new ErrorProvider();
            validationErrors.ContainerControl = this;
            validationErrors.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            inputDataDefaultBackColor = txtInputData.BackColor;
            blankSymbolDefaultBackColor = txtBlankSymbol.BackColor;
            UpdateWindowTitle();
            UpdateGraphExportState();
            SetSimulationVisualState(SimulationVisualState.Ready);
        }

        /// <summary>
        /// Zvýrazní přechod, který simulátor právě provádí.
        /// </summary>
        private void Simulator_TransitionExecuting(int index)
        {
            SelectTransition(index);
        }

        /// <summary>
        /// Přidá načtenou přechodovou funkci do runtime seznamu programu.
        /// </summary>
        private void Simulator_TransitionLoaded(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove)
        {
            ListViewItem viewItem = new ListViewItem(new string[] { inputState, inputSymbol.ToString(), outputState, outputSymbol.ToString(), headMove.ToString() });
            listProgramTransitions.Items.Add(viewItem);
            ResizeProgramTransitionColumns();
        }

        /// <summary>
        /// Po dokončení kroku přehraje zvuk posunu a krátce zvýrazní zapsanou buňku.
        /// </summary>
        private void Simulator_TuringTransitionCompleted(object sender, EventArgs e)
        {
            if (formClosing)
                return;

            PlayTapeMoveSound();
            HighlightLastWrittenCell();
        }

        /// <summary>
        /// Překreslí pásku po změně obsahu nebo pozice hlavy.
        /// </summary>
        private void Simulator_TapeChanged(object sender, EventArgs e)
        {
            if (formClosing || IsDisposed || Disposing)
                return;

            tapeRenderer.ClearWriteHighlight();
            UpdateSimulationStatusPanel(tapeRenderer.VisualState);
            tapeRenderer.Draw(simulator);
            panelTapeCanvas.Invalidate();
        }

        /// <summary>
        /// Připraví krátký zvuk pro kroky, při kterých se pohne hlava stroje.
        /// </summary>
        private void ConfigureSoundEffects()
        {
            tapeMoveSound = new SoundEffectPlayer("tape_tick.wav");
            machineStopSound = new SoundEffectPlayer("machine_stop.wav");

            writeHighlightTimer = new Timer();
            writeHighlightTimer.Interval = 180;
            writeHighlightTimer.Tick += writeHighlightTimer_Tick;
        }

        /// <summary>
        /// Vrátí formulář i simulátor do počátečního stavu podle aktuálně vybraného souboru programu.
        /// </summary>
        private void Reset()
        {
            suppressDirtyTracking = true;
            try
            {
                simulator = CreateSimulator();
                currentTransitionIndex = -1;
                lockTransitionSelection = false;
                simulationPaused = false;
                simulationTimer.Stop();
                ResetTapeViewport();
                listProgramTransitions.Items.Clear();
                string errorMessage;
                if (simulator.LoadProgram(currentProgramFileName, out errorMessage))
                {
                    txtProgramFile.Text = currentProgramFileName;
                    btnRunMachine.Enabled = true;
                    btnStepMachine.Enabled = true;
                    SetPauseButton(false, false);
                    btnSetInputData.Enabled = true;
                    btnResetMachine.Enabled = false;
                    menuSaveProgram.Enabled = true;
                    menuSaveProgramAs.Enabled = true;
                    UpdateGraphExportState();
                    SetMachineDefinitionEnabled(true);
                    PopulateDefinitionFields(simulator.Definition);
                    lblInputData.Text = string.Format("Input data on tape (blank symbol is {0}):", simulator.Definition.BlankSymbol);
                    txtSummary.ResetText();
                    SetSimulationVisualState(SimulationVisualState.Ready);
                    panelTapeCanvas.Refresh();
                    UpdateDefinitionValidationIndicators();
                    SetDirty(false);
                }
                else
                {
                    menuSaveProgram.Enabled = false;
                    menuSaveProgramAs.Enabled = false;
                    UpdateGraphExportState();
                    UpdateWindowTitle();
                    MessageBox.Show("Error: " + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                suppressDirtyTracking = false;
            }
        }

        /// <summary>
        /// Obslouží položku menu pro otevření programu.
        /// </summary>
        private void menuOpenProgram_Click(object sender, EventArgs e)
        {
            OpenProgram();
        }

        /// <summary>
        /// Obslouží položku menu pro uložení programu do aktuálního souboru.
        /// </summary>
        private void menuSaveProgram_Click(object sender, EventArgs e)
        {
            SaveProgram();
        }

        /// <summary>
        /// Obslouží položku menu pro uložení programu do nového souboru.
        /// </summary>
        private void menuSaveProgramAs_Click(object sender, EventArgs e)
        {
            SaveProgramAs();
        }

        /// <summary>
        /// Otevře editor přechodových funkcí a po potvrzení synchronizuje definici stroje.
        /// </summary>
        private void menuEditTransitions_Click(object sender, EventArgs e)
        {
            OpenTransitionEditor(NoSelectedTransitionIndex);
        }

        /// <summary>
        /// Obslouží položku menu pro export stavového grafu do Graphviz DOT.
        /// </summary>
        private void menuExportGraph_Click(object sender, EventArgs e)
        {
            ExportGraph();
        }

        /// <summary>
        /// Otevře editor přechodových funkcí z dvojkliku v runtime seznamu programu.
        /// </summary>
        private void listProgramTransitions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem clickedItem = listProgramTransitions.GetItemAt(e.X, e.Y);
            if (clickedItem == null)
                return;

            OpenTransitionEditor(clickedItem.Index);
        }

        /// <summary>
        /// Zkontroluje stav simulace a otevře editor přechodových funkcí.
        /// </summary>
        private void OpenTransitionEditor(int selectedTransitionIndex)
        {
            if (simulator == null || simulator.Definition == null)
            {
                MessageBox.Show("No program is loaded.", "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!grpMachineDefinition.Enabled)
            {
                MessageBox.Show("Transitions cannot be edited while simulation is running or paused.", "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                TuringMachineDefinition currentDefinition = CreateEditorDefinitionFromForm();
                using (TransitionEditorForm form = new TransitionEditorForm(currentDefinition, selectedTransitionIndex))
                {
                    if (form.ShowDialog(this) == DialogResult.OK)
                        ApplyEditedTransitions(form.Transitions);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Uloží aktuální definici stroje jako stavový graf ve formátu Graphviz DOT.
        /// </summary>
        private void ExportGraph()
        {
            if (!CanExportGraph())
                return;

            saveGraphDialog.InitialDirectory = GetDialogInitialDirectory();
            saveGraphDialog.FileName = GetDefaultGraphFileName();
            if (saveGraphDialog.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                string dot = TuringMachineGraphExporter.ToDot(simulator.Definition);
                File.WriteAllText(saveGraphDialog.FileName, dot, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Export graph", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Navrhne název DOT souboru podle aktuálního programu.
        /// </summary>
        private string GetDefaultGraphFileName()
        {
            if (string.IsNullOrWhiteSpace(currentProgramFileName))
                return "machine.dot";

            return Path.GetFileNameWithoutExtension(currentProgramFileName) + ".dot";
        }

        /// <summary>
        /// Vybere soubor programu, načte jej a obnoví stav formuláře.
        /// </summary>
        private void OpenProgram()
        {
            if (!ConfirmSaveChanges())
                return;

            openProgramDialog.InitialDirectory = GetDialogInitialDirectory();
            openProgramDialog.FileName = "";
            openProgramDialog.FilterIndex = 1;
            if (openProgramDialog.ShowDialog() == DialogResult.OK)
            {
                currentProgramFileName = openProgramDialog.FileName;
                Reset();
            }
        }

        /// <summary>
        /// Uloží aktuálně načtený program do jeho současného souboru.
        /// </summary>
        private bool SaveProgram()
        {
            if (simulator == null || simulator.Definition == null)
            {
                MessageBox.Show("No program is loaded.", "Save program", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (string.IsNullOrWhiteSpace(currentProgramFileName))
            {
                return SaveProgramAs();
            }

            return SaveProgramToFile(currentProgramFileName);
        }

        /// <summary>
        /// Vyžádá si cílový soubor a uloží do něj aktuální program.
        /// </summary>
        private bool SaveProgramAs()
        {
            if (simulator == null || simulator.Definition == null)
            {
                MessageBox.Show("No program is loaded.", "Save program", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            saveProgramDialog.InitialDirectory = GetDialogInitialDirectory();
            saveProgramDialog.FileName = Path.GetFileNameWithoutExtension(currentProgramFileName);
            saveProgramDialog.FilterIndex = string.Equals(Path.GetExtension(currentProgramFileName), ".btm", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
            if (saveProgramDialog.ShowDialog() != DialogResult.OK)
                return false;

            return SaveProgramToFile(saveProgramDialog.FileName);
        }

        /// <summary>
        /// Serializuje aktuální definici z formuláře do textového nebo binárního formátu podle přípony.
        /// </summary>
        private bool SaveProgramToFile(string fileName)
        {
            try
            {
                TuringMachineDefinition definition = CreateDefinitionFromForm();

                string extension = Path.GetExtension(fileName);
                if (string.Equals(extension, ".btm", StringComparison.OrdinalIgnoreCase))
                    TuringMachineProgramSerializer.SaveBinary(fileName, definition);
                else
                    TuringMachineProgramSerializer.SaveText(fileName, definition);

                currentProgramFileName = fileName;
                txtProgramFile.Text = currentProgramFileName;
                SetDirty(false);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Save program", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Sestaví definici z formuláře, znovu vytvoří simulátor a načte do něj aktuální program.
        /// </summary>
        private bool ReloadSimulatorFromForm(string messageTitle = "Run program")
        {
            try
            {
                TuringMachineDefinition definition = CreateDefinitionFromForm();
                return LoadDefinitionIntoSimulator(definition, messageTitle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Načte předanou definici do nové instance simulátoru a promítne ji do formuláře.
        /// </summary>
        private bool LoadDefinitionIntoSimulator(TuringMachineDefinition definition, string messageTitle)
        {
            try
            {
                currentTransitionIndex = -1;
                lockTransitionSelection = false;
                simulationPaused = false;
                simulationTimer.Stop();
                ResetTapeViewport();
                listProgramTransitions.Items.Clear();
                simulator = CreateSimulator();
                string errorMessage;
                if (!simulator.LoadProgram(definition, out errorMessage))
                {
                    UpdateGraphExportState();
                    MessageBox.Show("Error: " + errorMessage, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                lblInputData.Text = string.Format("Input data on tape (blank symbol is {0}):", definition.BlankSymbol);
                PopulateDefinitionFields(definition);
                UpdateGraphExportState();
                SetSimulationVisualState(SimulationVisualState.Ready);
                panelTapeCanvas.Refresh();
                UpdateDefinitionValidationIndicators();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, messageTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Vytvoří novou instanci simulátoru a napojí události potřebné pro formulář.
        /// </summary>
        private TuringSimulator CreateSimulator()
        {
            TuringSimulator newSimulator = new TuringSimulator();
            newSimulator.TapeChanged += Simulator_TapeChanged;
            newSimulator.TransitionLoaded += Simulator_TransitionLoaded;
            newSimulator.TuringTransitionCompleted += Simulator_TuringTransitionCompleted;
            newSimulator.TransitionExecuting += Simulator_TransitionExecuting;
            newSimulator.SyntaxError += Simulator_SyntaxError;
            newSimulator.InputDataLoaded += Simulator_InputDataLoaded;
            return newSimulator;
        }

        /// <summary>
        /// Promítne abecedy a prázdný symbol z definice do readonly polí formuláře.
        /// </summary>
        private void PopulateDefinitionFields(TuringMachineDefinition definition)
        {
            bool previousSuppressDirtyTracking = suppressDirtyTracking;
            suppressDirtyTracking = true;
            try
            {
                txtInputAlphabet.Text = FormatAlphabet(definition.Alphabet);
                txtTapeAlphabet.Text = FormatAlphabet(definition.TapeAlphabet);
                txtBlankSymbol.Text = definition.BlankSymbol.ToString();
            }
            finally
            {
                suppressDirtyTracking = previousSuppressDirtyTracking;
            }
        }

        /// <summary>
        /// Vytvoří formální definici stroje z aktuálních hodnot formuláře a načtených přechodů.
        /// </summary>
        private TuringMachineDefinition CreateDefinitionFromForm()
        {
            return CreateDefinitionFromForm(simulator.Definition.Transitions);
        }

        /// <summary>
        /// Vytvoří formální definici stroje z hodnot formuláře a předaného seznamu přechodů.
        /// </summary>
        private TuringMachineDefinition CreateDefinitionFromForm(IEnumerable<TransitionFunction> transitions)
        {
            string blank = txtBlankSymbol.Text.Trim();
            if (blank.Length != 1)
                throw new ArgumentException("Blank symbol must be exactly one character.");

            char[] inputAlphabet = GetCurrentInputAlphabet();
            ValidateInputData(txtInputData.Text, inputAlphabet, blank[0]);
            char[] tapeAlphabet = InferTapeAlphabet(transitions, inputAlphabet, blank[0]);

            return new TuringMachineDefinition(
                inputAlphabet,
                tapeAlphabet,
                blank[0],
                txtInputData.Text,
                transitions);
        }

        /// <summary>
        /// Vytvoří definici pro editor přechodů i tehdy, když je aktuální vstupní slovo dočasně neplatné.
        /// </summary>
        private TuringMachineDefinition CreateEditorDefinitionFromForm()
        {
            string blank = txtBlankSymbol.Text.Trim();
            if (blank.Length != 1)
                throw new ArgumentException("Blank symbol must be exactly one character.");

            IEnumerable<TransitionFunction> transitions = simulator.Definition.Transitions;
            char[] inputAlphabet = GetCurrentInputAlphabet();
            char[] tapeAlphabet = InferTapeAlphabet(transitions, inputAlphabet, blank[0]);
            return new TuringMachineDefinition(
                inputAlphabet,
                tapeAlphabet,
                blank[0],
                "",
                transitions);
        }

        /// <summary>
        /// Převede abecedu na čárkami oddělený text pro zobrazení v GUI.
        /// </summary>
        private static string FormatAlphabet(IEnumerable<char> alphabet)
        {
            return string.Join(",", alphabet);
        }

        /// <summary>
        /// Vrátí vstupní abecedu aktuální definice bez odvozování z pracovních symbolů přechodů.
        /// </summary>
        private char[] GetCurrentInputAlphabet()
        {
            List<char> result = new List<char>();
            if (simulator != null && simulator.Definition != null)
                foreach (char symbol in simulator.Definition.Alphabet)
                    AddDistinct(result, symbol);

            if (result.Count == 0)
            {
                AddDistinct(result, '0');
                AddDistinct(result, '1');
            }

            return result.ToArray();
        }

        /// <summary>
        /// Ověří, že vstupní slovo obsahuje pouze symboly vstupní abecedy a neobsahuje blank symbol.
        /// </summary>
        private static void ValidateInputData(string inputData, IEnumerable<char> inputAlphabet, char blankSymbol)
        {
            char invalidSymbol;
            if (!TryFindInvalidInputSymbol(inputData, inputAlphabet, blankSymbol, out invalidSymbol))
                return;

            if (invalidSymbol == blankSymbol)
                throw new ArgumentException("Input data cannot contain the blank symbol.");

            throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the input alphabet.", invalidSymbol));
        }

        /// <summary>
        /// Najde první symbol vstupu, který nepatří do vstupní abecedy.
        /// </summary>
        private static bool TryFindInvalidInputSymbol(string inputData, IEnumerable<char> inputAlphabet, char blankSymbol, out char invalidSymbol)
        {
            List<char> alphabet = new List<char>(inputAlphabet);
            string data = inputData == null ? "" : inputData.Trim();
            foreach (char symbol in data)
            {
                if (symbol == blankSymbol || !alphabet.Contains(symbol))
                {
                    invalidSymbol = symbol;
                    return true;
                }
            }

            invalidSymbol = '\0';
            return false;
        }

        /// <summary>
        /// Odvodí páskovou abecedu ze vstupní abecedy, blank symbolu a symbolů použitých v přechodech.
        /// </summary>
        private static char[] InferTapeAlphabet(IEnumerable<TransitionFunction> transitions, IEnumerable<char> inputAlphabet, char blankSymbol)
        {
            List<char> result = new List<char>();
            foreach (char symbol in inputAlphabet)
                AddDistinct(result, symbol);

            AddDistinct(result, blankSymbol);

            foreach (TransitionFunction transition in transitions)
            {
                AddDistinct(result, transition.InputSymbol);
                AddDistinct(result, transition.OutputSymbol);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Přidá symbol do seznamu pouze v případě, že v něm ještě není.
        /// </summary>
        private static void AddDistinct(IList<char> values, char symbol)
        {
            if (!values.Contains(symbol))
                values.Add(symbol);
        }

        /// <summary>
        /// Vrátí počáteční složku pro dialogy podle aktuálního programu nebo umístění aplikace.
        /// </summary>
        private string GetDialogInitialDirectory()
        {
            if (!string.IsNullOrWhiteSpace(currentProgramFileName))
                return Path.GetDirectoryName(currentProgramFileName);

            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        /// <summary>
        /// Promítne vstupní data načtená loaderem do vstupního pole formuláře.
        /// </summary>
        private void Simulator_InputDataLoaded(string inputData)
        {
            bool previousSuppressDirtyTracking = suppressDirtyTracking;
            suppressDirtyTracking = true;
            try
            {
                txtInputData.Text = inputData;
            }
            finally
            {
                suppressDirtyTracking = previousSuppressDirtyTracking;
            }
        }

        /// <summary>
        /// Vykreslí připravený bitmapový obraz pásky na panel formuláře.
        /// </summary>
        private void panelTapeCanvas_Paint(object sender, PaintEventArgs e)
        {
            tapeRenderer.Paint(e.Graphics, simulator);
        }

        /// <summary>
        /// Zahájí ruční posun pohledu na pásku tažením myší.
        /// </summary>
        private void panelTapeCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            tapeRenderer.BeginDrag(e, simulator);
        }

        /// <summary>
        /// Během tažení přepočítá vodorovný posun pásky.
        /// </summary>
        private void panelTapeCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            tapeRenderer.Drag(e, simulator);
        }

        /// <summary>
        /// Ukončí ruční posun pásky.
        /// </summary>
        private void panelTapeCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            tapeRenderer.StopDragging();
        }

        /// <summary>
        /// Vrátí kurzor panelu, pokud myš opustí pásku mimo aktivní tažení.
        /// </summary>
        private void panelTapeCanvas_MouseLeave(object sender, EventArgs e)
        {
            tapeRenderer.HandleMouseLeave();
        }

        /// <summary>
        /// Při změně velikosti panelu znovu vytvoří bitmapu pásky.
        /// </summary>
        private void panelTapeCanvas_SizeChanged(object sender, EventArgs e)
        {
            tapeRenderer.Resize(simulator);
        }

        /// <summary>
        /// Přepne vizualizaci do režimu, kdy se hlava pohybuje po stojící pásce.
        /// </summary>
        private void menuHeadMovesOverTape_Click(object sender, EventArgs e)
        {
            ApplyTapeViewMode(false);
        }

        /// <summary>
        /// Přepne vizualizaci do režimu, kdy páska automaticky sleduje hlavu.
        /// </summary>
        private void menuTapeFollowsHead_Click(object sender, EventArgs e)
        {
            ApplyTapeViewMode(true);
        }

        /// <summary>
        /// Po krátkém zvýraznění vrátí naposledy zapsanou buňku do běžného vzhledu.
        /// </summary>
        private void writeHighlightTimer_Tick(object sender, EventArgs e)
        {
            writeHighlightTimer.Stop();
            if (formClosing || IsDisposed || Disposing)
                return;

            tapeRenderer.ClearWriteHighlight();
            tapeRenderer.Draw(simulator);
            panelTapeCanvas.Invalidate();
        }

        /// <summary>
        /// Spustí automatickou simulaci nebo pokračuje z pozastaveného stavu.
        /// </summary>
        private void btnRunMachine_Click(object sender, EventArgs e)
        {
            if (grpMachineDefinition.Enabled && !ReloadSimulatorFromForm())
                return;

            RunAutomaticSimulation();
        }

        /// <summary>
        /// Připraví formulář pro automatický běh stroje od aktuální pozice.
        /// </summary>
        private void RunAutomaticSimulation()
        {
            simulationPaused = false;
            lockTransitionSelection = true;
            btnStepMachine.Enabled = false;
            btnRunMachine.Enabled = false;
            SetPauseButton(true, false);
            btnResetMachine.Enabled = false;
            SetMachineDefinitionEnabled(false);
            txtSummary.ResetText();
            SetSimulationVisualState(SimulationVisualState.Running);
            simulationTimer.Interval = GetAutomaticStepDelayMilliseconds();
            simulationTimer.Start();
        }

        /// <summary>
        /// Provede jeden automatický krok simulace a znovu naplánuje další krok.
        /// </summary>
        private void simulationTimer_Tick(object sender, EventArgs e)
        {
            simulationTimer.Stop();
            if (formClosing)
                return;

            summary = simulator.Run(true);
            if (summary != "")
            {
                FinishSimulation(summary);
                return;
            }

            simulationTimer.Interval = GetAutomaticStepDelayMilliseconds();
            simulationTimer.Start();
        }

        /// <summary>
        /// Při změně rychlosti upraví interval dalšího automatického kroku.
        /// </summary>
        private void trackSimulationDelay_ValueChanged(object sender, EventArgs e)
        {
            simulationTimer.Interval = GetAutomaticStepDelayMilliseconds();
        }

        /// <summary>
        /// Převede skokovou hodnotu slideru rychlosti na prodlevu mezi automatickými kroky.
        /// </summary>
        private int GetAutomaticStepDelayMilliseconds()
        {
            int index = trackSimulationDelay.Value;
            if (index < 0)
                index = 0;
            if (index >= AutomaticStepDelayMilliseconds.Length)
                index = AutomaticStepDelayMilliseconds.Length - 1;

            return AutomaticStepDelayMilliseconds[index];
        }

        /// <summary>
        /// Pozastaví automatický běh nebo pokračuje z pozastaveného stavu.
        /// </summary>
        private void btnPauseMachine_Click(object sender, EventArgs e)
        {
            if (simulationPaused)
            {
                RunAutomaticSimulation();
                return;
            }

            PauseSimulation();
        }

        /// <summary>
        /// Zobrazí syntaktickou chybu nahlášenou loaderem programu.
        /// </summary>
        private void Simulator_SyntaxError(string description, string line)
        {
            MessageBox.Show("Line syntax is invalid: " + description + "\n\n" + line, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Provede jeden simulační krok a udržuje formulář v režimu krokování, dokud stroj neskončí.
        /// </summary>
        private void btnStepMachine_Click(object sender, EventArgs e)
        {
            if (grpMachineDefinition.Enabled && !ReloadSimulatorFromForm())
                return;

            lockTransitionSelection = true;
            simulationTimer.Stop();
            btnRunMachine.Enabled = false;
            SetPauseButton(false, false);
            btnResetMachine.Enabled = true;
            SetMachineDefinitionEnabled(false);
            txtSummary.ResetText();
            SetSimulationVisualState(SimulationVisualState.Running);
            summary = simulator.Run(true);
            if (summary != "")
            {
                FinishSimulation(summary);
            }
            else
            {
                PauseSimulation();
            }
        }

        /// <summary>
        /// Aplikuje vstupní data a blank symbol z formuláře bez spuštění simulace.
        /// </summary>
        private void btnSetInputData_Click(object sender, EventArgs e)
        {
            if (ReloadSimulatorFromForm("Input data"))
                btnSetInputData.Enabled = false;
        }

        /// <summary>
        /// Resetuje simulaci do stavu po načtení aktuálního programu.
        /// </summary>
        private void btnResetMachine_Click(object sender, EventArgs e)
        {
            lockTransitionSelection = false;
            simulationPaused = false;
            simulationTimer.Stop();
            currentTransitionIndex = -1;
            if (simulator != null && simulator.Definition != null && LoadDefinitionIntoSimulator(simulator.Definition, "Reset"))
            {
                btnRunMachine.Enabled = true;
                btnStepMachine.Enabled = true;
                SetPauseButton(false, false);
                btnResetMachine.Enabled = false;
                SetMachineDefinitionEnabled(true);
                txtSummary.ResetText();
            }
        }

        /// <summary>
        /// Zapne nebo vypne skupinu prvků, které mění definici stroje.
        /// </summary>
        private void SetMachineDefinitionEnabled(bool enabled)
        {
            grpMachineDefinition.Enabled = enabled;
        }

        /// <summary>
        /// Přijme přechody z editoru, vytvoří z nich novou definici a obnoví stav simulátoru.
        /// </summary>
        private void ApplyEditedTransitions(IEnumerable<TransitionFunction> transitions)
        {
            try
            {
                TuringMachineDefinition definition = CreateDefinitionFromForm(transitions);
                if (!LoadDefinitionIntoSimulator(definition, "Edit transitions"))
                    return;

                btnRunMachine.Enabled = true;
                btnStepMachine.Enabled = true;
                SetPauseButton(false, false);
                btnResetMachine.Enabled = false;
                btnSetInputData.Enabled = true;
                menuSaveProgram.Enabled = true;
                menuSaveProgramAs.Enabled = true;
                UpdateGraphExportState();
                SetMachineDefinitionEnabled(true);
                txtSummary.ResetText();
                SetDirty(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Nastaví ovládací prvky do stavu po dokončené simulaci.
        /// </summary>
        private void FinishSimulation(string simulationSummary)
        {
            txtSummary.ResetText();
            txtSummary.Text = simulationSummary;
            SetSimulationVisualState(GetFinishedVisualState());
            PlayMachineStopSound();
            btnStepMachine.Enabled = false;
            btnRunMachine.Enabled = false;
            SetPauseButton(false, false);
            btnResetMachine.Enabled = true;
            SetMachineDefinitionEnabled(true);
            lockTransitionSelection = false;
            simulationPaused = false;
            simulationTimer.Stop();
        }

        /// <summary>
        /// Nastaví ovládací prvky do stavu, kdy lze pokračovat krokováním nebo dalším automatickým během.
        /// </summary>
        private void PauseSimulation()
        {
            simulationPaused = true;
            simulationTimer.Stop();
            btnStepMachine.Enabled = true;
            btnRunMachine.Enabled = false;
            SetPauseButton(true, true);
            btnResetMachine.Enabled = true;
            SetMachineDefinitionEnabled(false);
            lockTransitionSelection = true;
            SetSimulationVisualState(SimulationVisualState.Paused);
        }

        /// <summary>
        /// Nastaví dostupnost a text tlačítka pro pozastavení nebo pokračování běhu.
        /// </summary>
        private void SetPauseButton(bool enabled, bool continueMode)
        {
            btnPauseMachine.Enabled = enabled;
            btnPauseMachine.Text = continueMode ? "Continue" : "Pause";
        }

        /// <summary>
        /// Vrátí informaci, zda je simulátor právě v běhu nebo v režimu krokování.
        /// </summary>
        private bool IsSimulationActive()
        {
            return simulator != null && simulator.Definition != null && !grpMachineDefinition.Enabled;
        }

        /// <summary>
        /// Určí, zda aktuální stroj obsahuje definici vhodnou pro export grafu.
        /// </summary>
        private bool CanExportGraph()
        {
            return simulator != null && simulator.Definition != null && simulator.Definition.Transitions.Count > 0;
        }

        /// <summary>
        /// Zapne nebo vypne položku exportu grafu podle aktuálního stavu stroje.
        /// </summary>
        private void UpdateGraphExportState()
        {
            if (menuExportGraph != null)
                menuExportGraph.Enabled = CanExportGraph();
        }

        /// <summary>
        /// Nastaví vizuální stav simulace a překreslí pásku.
        /// </summary>
        private void SetSimulationVisualState(SimulationVisualState state)
        {
            if (formClosing || IsDisposed || Disposing)
                return;

            tapeRenderer.VisualState = state;
            UpdateSimulationStatusPanel(state);
            tapeRenderer.Draw(simulator);
            panelTapeCanvas.Invalidate();
        }

        /// <summary>
        /// Promítne aktuální stav simulátoru do běžných prvků formuláře nad páskou.
        /// </summary>
        private void UpdateSimulationStatusPanel(SimulationVisualState state)
        {
            TuringMachine machine = simulator != null ? simulator.Machine : null;
            string currentState = machine != null ? machine.CurrentState() : "-";
            string headIndex = machine != null ? machine.HeadIndex().ToString() : "-";
            string readSymbol = machine != null ? machine.ReadSymbol().ToString() : "-";
            string stepCount = simulator != null ? simulator.StepCount.ToString() : "0";
            string statusText;
            Color statusBackColor;
            Color statusTextColor;
            Color transitionTextColor;

            GetVisualStateAppearance(state, out statusText, out statusBackColor, out statusTextColor);
            SetStatusBadge(lblMachineStateStatus, "State", currentState, Color.FromArgb(230, 241, 255), Color.FromArgb(9, 105, 218));
            SetStatusBadge(lblHeadStatus, "Head", headIndex, Color.FromArgb(255, 248, 197), Color.FromArgb(154, 103, 0));
            SetStatusBadge(lblReadStatus, "Read", readSymbol, Color.FromArgb(246, 248, 250), Color.FromArgb(87, 96, 106));
            SetStatusBadge(lblStepStatus, "Step", stepCount, Color.FromArgb(246, 248, 250), Color.FromArgb(87, 96, 106));
            SetStatusBadge(lblRunStatus, "Status", statusText, statusBackColor, statusTextColor);

            lblLastTransitionStatus.Text = GetLastTransitionText(out transitionTextColor);
            lblLastTransitionStatus.ForeColor = transitionTextColor;
        }

        /// <summary>
        /// Nastaví text a barvy jednoho stavového pole nad páskou.
        /// </summary>
        private static void SetStatusBadge(Label label, string title, string value, Color backColor, Color textColor)
        {
            label.BackColor = backColor;
            label.ForeColor = textColor;
            label.Text = string.Format("{0}   {1}", title, value);
        }

        /// <summary>
        /// Sestaví text posledního provedeného přechodu pro stavový řádek.
        /// </summary>
        private string GetLastTransitionText(out Color textColor)
        {
            SimulationStep step = simulator != null ? simulator.LastStep : null;
            if (step == null)
            {
                textColor = Color.FromArgb(70, 78, 92);
                return "Ready";
            }

            if (!step.TransitionFound)
            {
                textColor = Color.FromArgb(178, 94, 0);
                return string.Format("No transition for ({0}, {1})", step.InputState, step.InputSymbol);
            }

            textColor = Color.FromArgb(36, 97, 170);
            return string.Format("Last: ({0}, {1}) ➞ ({2}, {3}, {4})", step.InputState, step.InputSymbol, step.OutputState, step.OutputSymbol, step.HeadMove);
        }

        /// <summary>
        /// Vrátí text a barvy pro aktuální stav simulace.
        /// </summary>
        private static void GetVisualStateAppearance(SimulationVisualState state, out string text, out Color backColor, out Color textColor)
        {
            switch (state)
            {
                case SimulationVisualState.Running:
                    text = "Running";
                    backColor = Color.FromArgb(230, 241, 255);
                    textColor = Color.FromArgb(9, 105, 218);
                    break;
                case SimulationVisualState.Paused:
                    text = "Paused";
                    backColor = Color.FromArgb(255, 248, 197);
                    textColor = Color.FromArgb(154, 103, 0);
                    break;
                case SimulationVisualState.Finished:
                    text = "Finished";
                    backColor = Color.FromArgb(218, 251, 225);
                    textColor = Color.FromArgb(26, 127, 55);
                    break;
                case SimulationVisualState.NoTransition:
                    text = "No transition";
                    backColor = Color.FromArgb(255, 241, 230);
                    textColor = Color.FromArgb(188, 76, 0);
                    break;
                case SimulationVisualState.Overflow:
                    text = "Overflow";
                    backColor = Color.FromArgb(255, 235, 233);
                    textColor = Color.FromArgb(207, 34, 46);
                    break;
                case SimulationVisualState.StepLimit:
                    text = "Step limit";
                    backColor = Color.FromArgb(255, 241, 230);
                    textColor = Color.FromArgb(188, 76, 0);
                    break;
                default:
                    text = "Ready";
                    backColor = Color.FromArgb(246, 248, 250);
                    textColor = Color.FromArgb(87, 96, 106);
                    break;
            }
        }

        /// <summary>
        /// Určí finální vizuální stav podle důvodu zastavení simulátoru.
        /// </summary>
        private SimulationVisualState GetFinishedVisualState()
        {
            if (simulator == null || simulator.Machine == null)
                return SimulationVisualState.Finished;

            if (simulator.Machine.HasOverflowed)
                return SimulationVisualState.Overflow;

            if (simulator.StepCount >= TuringMachine.MaxSteps)
                return SimulationVisualState.StepLimit;

            if (simulator.LastStep != null && !simulator.LastStep.TransitionFound)
                return SimulationVisualState.NoTransition;

            return SimulationVisualState.Finished;
        }

        /// <summary>
        /// Při běhu vrací ručně změněný výběr zpět na aktuální přechod simulátoru.
        /// </summary>
        private void listProgramTransitions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (updatingTransitionSelection || !lockTransitionSelection || currentTransitionIndex < 0)
                return;

            SelectTransition(currentTransitionIndex);
        }

        /// <summary>
        /// Přepočítá šířky sloupců runtime seznamu při změně velikosti formuláře.
        /// </summary>
        private void listProgramTransitions_SizeChanged(object sender, EventArgs e)
        {
            ResizeProgramTransitionColumns();
        }

        /// <summary>
        /// Rozdělí dostupnou šířku seznamu mezi sloupce přechodové funkce podle jejich obsahu.
        /// </summary>
        private void ResizeProgramTransitionColumns()
        {
            if (listProgramTransitions.Columns.Count == 0)
                return;

            int availableWidth = listProgramTransitions.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 4;
            if (availableWidth <= 0)
                return;

            colInputState.Width = availableWidth * 27 / 100;
            colInputSymbol.Width = availableWidth * 12 / 100;
            colOutputState.Width = availableWidth * 27 / 100;
            colOutputSymbol.Width = availableWidth * 12 / 100;
            colHeadMove.Width = availableWidth - colInputState.Width - colInputSymbol.Width - colOutputState.Width - colOutputSymbol.Width;
        }

        /// <summary>
        /// Označí přechod v runtime seznamu a posune jej do viditelné části seznamu.
        /// </summary>
        private void SelectTransition(int index)
        {
            if (index < 0 || index >= listProgramTransitions.Items.Count)
                return;

            try
            {
                currentTransitionIndex = index;
                updatingTransitionSelection = true;
                foreach (ListViewItem item in listProgramTransitions.Items)
                    item.Selected = false;

                ListViewItem selectedItem = listProgramTransitions.Items[index];
                selectedItem.Selected = true;
                selectedItem.Focused = true;
                listProgramTransitions.EnsureVisible(index);
            }
            finally
            {
                updatingTransitionSelection = false;
            }
        }

        /// <summary>
        /// Uvolní bitmapu pásky při zavření formuláře.
        /// </summary>
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            formClosing = true;
            tapeRenderer?.Dispose();
            validationErrors?.Dispose();
            writeHighlightTimer?.Dispose();
            tapeMoveSound?.Dispose();
            machineStopSound?.Dispose();
            base.OnFormClosed(e);
        }

        /// <summary>
        /// Nabídne uložení změn před akcí, která by mohla zahodit aktuální definici.
        /// </summary>
        private bool ConfirmSaveChanges()
        {
            if (!hasUnsavedChanges)
                return true;

            DialogResult result = MessageBox.Show(
                "Current machine has unsaved changes. Do you want to save them?",
                "Unsaved changes",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.Cancel)
                return false;

            if (result == DialogResult.No)
                return true;

            return SaveProgram();
        }

        /// <summary>
        /// Před zavřením aplikace ohlídá neuložené změny.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            bool wasSimulationActive = IsSimulationActive();
            bool wasTimerRunning = simulationTimer.Enabled;
            bool previousSimulationPaused = simulationPaused;
            bool previousPauseEnabled = btnPauseMachine.Enabled;
            string previousPauseText = btnPauseMachine.Text;
            if (wasSimulationActive)
            {
                // Při zavírání nejdřív zastavíme automatický časovač, aby dialog pro uložení
                // nemohl vyvolat další změny nad aktivně krokovaným simulátorem.
                simulationTimer.Stop();
                SetPauseButton(false, false);
            }

            if (!ConfirmSaveChanges())
            {
                e.Cancel = true;
                formClosing = false;
                simulationPaused = previousSimulationPaused;
                btnPauseMachine.Enabled = previousPauseEnabled;
                btnPauseMachine.Text = previousPauseText;
                if (wasTimerRunning)
                    simulationTimer.Start();
                return;
            }

            formClosing = true;
            simulationTimer.Stop();
            simulationPaused = false;
            writeHighlightTimer?.Stop();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Označí dokument jako změněný nebo uložený a obnoví titulek okna.
        /// </summary>
        private void SetDirty(bool dirty)
        {
            hasUnsavedChanges = dirty;
            UpdateWindowTitle();
        }

        /// <summary>
        /// Promítne název aktuálního souboru a příznak změn do titulku okna.
        /// </summary>
        private void UpdateWindowTitle()
        {
            string title = "UTMS";
            if (!string.IsNullOrWhiteSpace(currentProgramFileName))
                title += " - " + Path.GetFileName(currentProgramFileName);

            if (hasUnsavedChanges)
                title += " *";

            Text = title;
        }

        /// <summary>
        /// Označí definici jako změněnou po ruční úpravě vstupu nebo blank symbolu.
        /// </summary>
        private void MachineDefinitionField_TextChanged(object sender, EventArgs e)
        {
            if (suppressDirtyTracking || simulator == null || simulator.Definition == null)
                return;

            btnSetInputData.Enabled = true;
            UpdateDefinitionValidationIndicators();
            SetDirty(true);
        }

        /// <summary>
        /// Zobrazí průběžné chyby pro blank symbol a vstupní slovo.
        /// </summary>
        private void UpdateDefinitionValidationIndicators()
        {
            if (validationErrors == null)
                return;

            validationErrors.SetError(txtBlankSymbol, "");
            validationErrors.SetError(txtInputData, "");
            txtBlankSymbol.BackColor = blankSymbolDefaultBackColor;
            txtInputData.BackColor = inputDataDefaultBackColor;

            if (simulator == null || simulator.Definition == null)
                return;

            string blank = txtBlankSymbol.Text.Trim();
            if (blank.Length != 1)
            {
                validationErrors.SetError(txtBlankSymbol, "Blank symbol must be exactly one character.");
                txtBlankSymbol.BackColor = Color.MistyRose;
                return;
            }

            char[] inputAlphabet = GetCurrentInputAlphabet();
            char invalidSymbol;
            if (!TryFindInvalidInputSymbol(txtInputData.Text, inputAlphabet, blank[0], out invalidSymbol))
                return;

            string errorMessage = invalidSymbol == blank[0]
                ? "Input data cannot contain the blank symbol."
                : string.Format("Input symbol \"{0}\" is not defined in the input alphabet.", invalidSymbol);
            validationErrors.SetError(txtInputData, errorMessage);
            txtInputData.BackColor = Color.MistyRose;
        }

        /// <summary>
        /// Krátce zvýrazní buňku, do které poslední krok zapisoval.
        /// </summary>
        private void HighlightLastWrittenCell()
        {
            if (formClosing || IsDisposed || Disposing)
                return;

            if (simulator == null || simulator.Machine == null || simulator.LastStep == null || !simulator.LastStep.TransitionFound)
                return;

            int writtenCellIndex = GetLastWrittenCellIndex();
            if (writtenCellIndex < 0 || writtenCellIndex >= simulator.Machine.Cells.Count)
                return;

            tapeRenderer.HighlightWriteCell(simulator, writtenCellIndex, simulator.LastStep.OutputSymbol);
            panelTapeCanvas.Update();
            writeHighlightTimer.Stop();
            writeHighlightTimer.Start();
        }

        /// <summary>
        /// Dopočítá pozici zapsané buňky z aktuální pozice hlavy a posledního pohybu.
        /// </summary>
        private int GetLastWrittenCellIndex()
        {
            int headIndex = simulator.Machine.HeadIndex();
            if (simulator.LastStep.HeadMove == TuringMachine.MoveLeftSymbol)
                return headIndex + 1;
            if (simulator.LastStep.HeadMove == TuringMachine.MoveRightSymbol)
                return headIndex - 1;

            return headIndex;
        }

        /// <summary>
        /// Přehraje jemné tiknutí u kroků, při kterých se hlava skutečně pohnula.
        /// </summary>
        private void PlayTapeMoveSound()
        {
            if (!menuSoundEffects.Checked || tapeMoveSound == null || simulator == null || simulator.LastStep == null || !simulator.LastStep.TransitionFound)
                return;

            char move = simulator.LastStep.HeadMove;
            if (move != TuringMachine.MoveLeftSymbol && move != TuringMachine.MoveRightSymbol)
                return;

            tapeMoveSound.Play();
        }

        /// <summary>
        /// Přehraje krátký zvuk při dokončení nebo zastavení simulace.
        /// </summary>
        private void PlayMachineStopSound()
        {
            if (!menuSoundEffects.Checked)
                return;

            machineStopSound?.Play();
        }

        /// <summary>
        /// Vrátí pohled pásky zpět do režimu automatického centrování na hlavu.
        /// </summary>
        private void ResetTapeViewport()
        {
            tapeRenderer.ResetViewport();
            writeHighlightTimer?.Stop();
        }

        /// <summary>
        /// Nastaví režim pohledu na pásku a obnoví její aktuální vykreslení.
        /// </summary>
        private void ApplyTapeViewMode(bool followsHead)
        {
            tapeFollowsHead = followsHead;

            if (menuHeadMovesOverTape != null)
                menuHeadMovesOverTape.Checked = !tapeFollowsHead;

            if (menuTapeFollowsHead != null)
                menuTapeFollowsHead.Checked = tapeFollowsHead;

            tapeRenderer.ApplyViewMode(tapeFollowsHead, simulator);
        }
    }
}
