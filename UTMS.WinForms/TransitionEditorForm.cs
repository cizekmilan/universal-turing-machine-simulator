using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UTMS.Core;

namespace UTMS.WinForms
{
    /// <summary>
    /// Editor přechodových funkcí Turingova stroje.
    /// </summary>
    public sealed partial class TransitionEditorForm : Form
    {
        private const int NoSelectedTransitionIndex = -1;
        private const string NewStateOption = "<new state>";
        private const string NewSymbolOption = "<new symbol>";

        private static readonly Color InvalidCellBackColor = Color.MistyRose;

        private readonly char blankSymbol;
        private readonly List<char> inputAlphabet;
        private readonly List<TransitionFunction> transitions;
        private bool isUpdatingCell;

        /// <summary>
        /// Vytvoří editor a naplní jej aktuální definicí stroje.
        /// </summary>
        public TransitionEditorForm(TuringMachineDefinition definition)
            : this(definition, NoSelectedTransitionIndex)
        {
        }

        /// <summary>
        /// Vytvoří editor, naplní jej aktuální definicí stroje a případně nastaví zvolený řádek.
        /// </summary>
        public TransitionEditorForm(TuringMachineDefinition definition, int selectedTransitionIndex)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            blankSymbol = definition.BlankSymbol;
            inputAlphabet = new List<char>(definition.Alphabet);
            transitions = new List<TransitionFunction>();
            InitializeComponent();

            LoadDefinitionRows(definition);
            RefreshDerivedInformation();
            SelectTransitionRow(selectedTransitionIndex);
        }

        /// <summary>
        /// Přechody potvrzené uživatelem v editoru.
        /// </summary>
        public IReadOnlyList<TransitionFunction> Transitions
        {
            get { return transitions; }
        }

        /// <summary>
        /// Naplní tabulku přechody z aktuální definice a doplní potřebné položky do comboboxů.
        /// </summary>
        private void LoadDefinitionRows(TuringMachineDefinition definition)
        {
            foreach (char symbol in definition.TapeAlphabet)
                AddSymbolOption(symbol);

            foreach (TransitionFunction transition in definition.Transitions)
            {
                AddStateOption(transition.InputState);
                AddStateOption(transition.OutputState);
                AddSymbolOption(transition.InputSymbol);
                AddSymbolOption(transition.OutputSymbol);
                gridTransitions.Rows.Add(
                    transition.InputState,
                    transition.InputSymbol.ToString(),
                    transition.OutputState,
                    transition.OutputSymbol.ToString(),
                    transition.HeadMove.ToString());
            }
        }

        /// <summary>
        /// Nastaví aktivní buňku v řádku, který odpovídá přechodu vybranému v hlavním formuláři.
        /// </summary>
        private void SelectTransitionRow(int transitionIndex)
        {
            if (transitionIndex < 0 || transitionIndex >= gridTransitions.Rows.Count)
                return;

            DataGridViewRow row = gridTransitions.Rows[transitionIndex];
            if (row.IsNewRow)
                return;

            gridTransitions.ClearSelection();
            gridTransitions.CurrentCell = row.Cells["CurrentState"];
            row.Cells["CurrentState"].Selected = true;
            gridTransitions.FirstDisplayedScrollingRowIndex = transitionIndex;
        }

        /// <summary>
        /// Potvrdí dialog pouze tehdy, když lze řádky převést na platné přechody.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            List<TransitionFunction> parsedTransitions;
            string errorMessage;
            if (!TryReadTransitions(out parsedTransitions, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.None;
                return;
            }

            transitions.Clear();
            transitions.AddRange(parsedTransitions);
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// Ihned potvrzuje výběr v comboboxu, aby se změna buňky zpracovala bez odchodu z buňky.
        /// </summary>
        private void gridTransitions_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (gridTransitions.IsCurrentCellDirty)
                gridTransitions.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>
        /// Reaguje na změnu buňky a obsluhuje speciální volby pro nový stav nebo symbol.
        /// </summary>
        private void gridTransitions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUpdatingCell || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewCell cell = gridTransitions.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string value = Convert.ToString(cell.Value);

            if (value == NewStateOption)
            {
                AddStateFromCell(cell);
                return;
            }

            if (value == NewSymbolOption)
            {
                AddSymbolFromCell(cell);
                return;
            }

            RefreshDerivedInformation();
        }

        /// <summary>
        /// Po smazání řádku přepočítá odvozené stavy, abecedy a validaci.
        /// </summary>
        private void gridTransitions_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            RefreshDerivedInformation();
        }

        /// <summary>
        /// Po přidání řádku přepočítá stav editoru.
        /// </summary>
        private void gridTransitions_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RefreshDerivedInformation();
        }

        /// <summary>
        /// Potlačí interní chybu DataGridView při dočasné hodnotě mimo položky comboboxu.
        /// </summary>
        private void gridTransitions_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        /// <summary>
        /// Založí nový stav z aktuální buňky a přidá jej do obou stavových sloupců.
        /// </summary>
        private void AddStateFromCell(DataGridViewCell cell)
        {
            string stateName = PromptDialog.Show(this, "New state", "State name:", GetNextStateName());
            if (string.IsNullOrWhiteSpace(stateName))
            {
                SetCellValue(cell, "q0");
                RefreshDerivedInformation();
                return;
            }

            string trimmedStateName = stateName.Trim();
            string errorMessage;
            if (!TuringMachineTextFormatRules.TryValidateStateName(trimmedStateName, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetCellValue(cell, "q0");
                RefreshDerivedInformation();
                return;
            }

            AddStateOption(trimmedStateName);
            SetCellValue(cell, trimmedStateName);
            RefreshDerivedInformation();
        }

        /// <summary>
        /// Založí nový páskový symbol z aktuální buňky a přidá jej do symbolových sloupců.
        /// </summary>
        private void AddSymbolFromCell(DataGridViewCell cell)
        {
            string symbol = PromptDialog.Show(this, "New tape symbol", "Tape symbol:", "X");
            if (string.IsNullOrWhiteSpace(symbol) || symbol.Trim().Length != 1)
            {
                MessageBox.Show("Tape symbol must be exactly one character.", "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetCellValue(cell, blankSymbol.ToString());
                RefreshDerivedInformation();
                return;
            }

            char trimmedSymbol = symbol.Trim()[0];
            string errorMessage;
            if (!TuringMachineTextFormatRules.TryValidateTapeSymbol(trimmedSymbol, out errorMessage))
            {
                MessageBox.Show(errorMessage, "Edit transitions", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetCellValue(cell, blankSymbol.ToString());
                RefreshDerivedInformation();
                return;
            }

            AddSymbolOption(trimmedSymbol);
            SetCellValue(cell, trimmedSymbol.ToString());
            RefreshDerivedInformation();
        }

        /// <summary>
        /// Nastaví hodnotu buňky bez rekurzivního spuštění obsluhy změny a obnoví aktivní combobox.
        /// </summary>
        private void SetCellValue(DataGridViewCell cell, string value)
        {
            try
            {
                isUpdatingCell = true;
                cell.Value = value;
                UpdateCurrentEditingControl(value);
                gridTransitions.EndEdit();
                gridTransitions.InvalidateCell(cell);
            }
            finally
            {
                isUpdatingCell = false;
            }
        }

        /// <summary>
        /// Synchronizuje právě otevřený combobox s hodnotou, která byla programově vložena do buňky.
        /// </summary>
        private void UpdateCurrentEditingControl(string value)
        {
            ComboBox comboBox = gridTransitions.EditingControl as ComboBox;
            if (comboBox == null)
                return;

            if (!comboBox.Items.Contains(value))
                comboBox.Items.Add(value);

            comboBox.SelectedItem = value;
            comboBox.Text = value;
        }

        /// <summary>
        /// Přidá nový stav do sloupců počátečního i cílového stavu.
        /// </summary>
        private void AddStateOption(string stateName)
        {
            AddComboBoxOption("CurrentState", stateName, NewStateOption);
            AddComboBoxOption("NextState", stateName, NewStateOption);
        }

        /// <summary>
        /// Přidá nový páskový symbol do sloupců čteného i zapisovaného symbolu.
        /// </summary>
        private void AddSymbolOption(char symbol)
        {
            AddComboBoxOption("ReadSymbol", symbol.ToString(), NewSymbolOption);
            AddComboBoxOption("WriteSymbol", symbol.ToString(), NewSymbolOption);
        }

        /// <summary>
        /// Přidá položku do konkrétního combobox sloupce před speciální volbu pro novou hodnotu.
        /// </summary>
        private void AddComboBoxOption(string columnName, string value, string newOption)
        {
            DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)gridTransitions.Columns[columnName];
            if (column.Items.Contains(value))
                return;

            int insertIndex = column.Items.IndexOf(newOption);
            if (insertIndex < 0)
                column.Items.Add(value);
            else
                column.Items.Insert(insertIndex, value);
        }

        /// <summary>
        /// Navrhne další volný název stavu ve tvaru qN.
        /// </summary>
        private string GetNextStateName()
        {
            HashSet<string> states = CollectStates();
            int index = 1;
            while (states.Contains("q" + index))
                index++;

            return "q" + index;
        }

        /// <summary>
        /// Přepočítá informační panel podle aktuálních řádků tabulky.
        /// </summary>
        private void RefreshDerivedInformation()
        {
            HashSet<string> states = CollectStates();
            HashSet<char> tapeAlphabet = new HashSet<char>();

            foreach (char symbol in inputAlphabet)
                tapeAlphabet.Add(symbol);

            tapeAlphabet.Add(blankSymbol);
            foreach (DataGridViewRow row in gridTransitions.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string currentState = GetCellText(row, "CurrentState");
                string readSymbol = GetCellText(row, "ReadSymbol");
                string writeSymbol = GetCellText(row, "WriteSymbol");

                if (readSymbol.Length == 1)
                    tapeAlphabet.Add(readSymbol[0]);

                if (writeSymbol.Length == 1)
                    tapeAlphabet.Add(writeSymbol[0]);
            }

            List<TransitionFunction> parsedTransitions;
            string validationMessage;
            bool isValid = ValidateTransitionRows(out parsedTransitions, out validationMessage, true);
            lblStates.Text = "States: " + FormatValues(states);
            lblInputAlphabet.Text = "Input alphabet: " + FormatValues(inputAlphabet);
            lblTapeAlphabet.Text = "Tape alphabet: " + FormatValues(tapeAlphabet);
            lblValidation.Text = isValid ? "Validation: no errors" : "Validation: " + validationMessage;
            lblValidation.ForeColor = isValid ? Color.DarkGreen : Color.Firebrick;
            btnOk.Enabled = isValid;
        }

        /// <summary>
        /// Přečte tabulku a převede ji na přechodové funkce.
        /// </summary>
        private bool TryReadTransitions(out List<TransitionFunction> parsedTransitions, out string errorMessage)
        {
            return ValidateTransitionRows(out parsedTransitions, out errorMessage, true);
        }

        /// <summary>
        /// Ověří řádky editoru, případně zvýrazní chybné buňky, a vrátí načtené přechody.
        /// </summary>
        private bool ValidateTransitionRows(out List<TransitionFunction> parsedTransitions, out string errorMessage, bool markInvalidCells)
        {
            parsedTransitions = new List<TransitionFunction>();
            errorMessage = "";
            Dictionary<string, DataGridViewRow> transitionKeys = new Dictionary<string, DataGridViewRow>();
            List<string> validationMessages = new List<string>();
            bool isValid = true;

            if (markInvalidCells)
                ClearValidationStyles();

            foreach (DataGridViewRow row in gridTransitions.Rows)
            {
                if (row.IsNewRow)
                    continue;

                string currentState = GetCellText(row, "CurrentState").Trim();
                string readSymbol = GetCellText(row, "ReadSymbol").Trim();
                string nextState = GetCellText(row, "NextState").Trim();
                string writeSymbol = GetCellText(row, "WriteSymbol").Trim();
                string headMove = GetCellText(row, "HeadMove").Trim();
                bool emptyRow = currentState == "" && readSymbol == "" && nextState == "" && writeSymbol == "" && headMove == "";
                if (emptyRow)
                    continue;

                if (currentState == "" || readSymbol == "" || nextState == "" || writeSymbol == "" || headMove == "")
                {
                    AddValidationMessage(validationMessages, "All transition cells must be filled.");
                    MarkEmptyCells(row, markInvalidCells);
                    isValid = false;
                    continue;
                }

                if (currentState == NewStateOption || nextState == NewStateOption || readSymbol == NewSymbolOption || writeSymbol == NewSymbolOption)
                {
                    AddValidationMessage(validationMessages, "Special values for creating states or symbols cannot be saved as transitions.");
                    MarkSpecialValueCells(row, markInvalidCells);
                    isValid = false;
                    continue;
                }

                if (readSymbol.Length != 1 || writeSymbol.Length != 1 || headMove.Length != 1)
                {
                    AddValidationMessage(validationMessages, "Read, write and move values must contain exactly one character.");
                    if (readSymbol.Length != 1)
                        MarkInvalidCell(row, "ReadSymbol", markInvalidCells);
                    if (writeSymbol.Length != 1)
                        MarkInvalidCell(row, "WriteSymbol", markInvalidCells);
                    if (headMove.Length != 1)
                        MarkInvalidCell(row, "HeadMove", markInvalidCells);
                    isValid = false;
                    continue;
                }

                if (!ValidateTextFormatCells(row, currentState, readSymbol[0], nextState, writeSymbol[0], validationMessages, markInvalidCells))
                {
                    isValid = false;
                    continue;
                }

                if (!IsMoveSupported(headMove[0]))
                {
                    AddValidationMessage(validationMessages, string.Format("Head move must be {0}, {1} or {2}.", TuringMachine.MoveLeftSymbol, TuringMachine.MoveRightSymbol, TuringMachine.StopSymbol));
                    MarkInvalidCell(row, "HeadMove", markInvalidCells);
                    isValid = false;
                    continue;
                }

                string key = currentState + "|" + readSymbol;
                if (transitionKeys.ContainsKey(key))
                {
                    AddValidationMessage(validationMessages, string.Format("Transition for state \"{0}\" and input symbol \"{1}\" is defined more than once.", currentState, readSymbol));
                    MarkInvalidCell(transitionKeys[key], "CurrentState", markInvalidCells);
                    MarkInvalidCell(transitionKeys[key], "ReadSymbol", markInvalidCells);
                    MarkInvalidCell(row, "CurrentState", markInvalidCells);
                    MarkInvalidCell(row, "ReadSymbol", markInvalidCells);
                    isValid = false;
                    continue;
                }

                transitionKeys.Add(key, row);
                parsedTransitions.Add(new TransitionFunction(currentState, readSymbol[0], nextState, writeSymbol[0], headMove[0]));
            }

            if (parsedTransitions.Count == 0)
            {
                AddValidationMessage(validationMessages, "Transition table must contain at least one transition.");
                errorMessage = FormatValidationMessages(validationMessages);
                return false;
            }

            errorMessage = FormatValidationMessages(validationMessages);
            return isValid;
        }

        /// <summary>
        /// Ověří stavy a symboly proti znakům, které by nešly bezpečně zapsat do textového formátu.
        /// </summary>
        private static bool ValidateTextFormatCells(DataGridViewRow row, string currentState, char readSymbol, string nextState, char writeSymbol, IList<string> validationMessages, bool markInvalidCells)
        {
            bool isValid = true;
            string message;

            if (!TuringMachineTextFormatRules.TryValidateStateName(currentState, out message))
            {
                AddValidationMessage(validationMessages, message);
                MarkInvalidCell(row, "CurrentState", markInvalidCells);
                isValid = false;
            }

            if (!TuringMachineTextFormatRules.TryValidateStateName(nextState, out message))
            {
                AddValidationMessage(validationMessages, message);
                MarkInvalidCell(row, "NextState", markInvalidCells);
                isValid = false;
            }

            if (!TuringMachineTextFormatRules.TryValidateTapeSymbol(readSymbol, out message))
            {
                AddValidationMessage(validationMessages, message);
                MarkInvalidCell(row, "ReadSymbol", markInvalidCells);
                isValid = false;
            }

            if (!TuringMachineTextFormatRules.TryValidateTapeSymbol(writeSymbol, out message))
            {
                AddValidationMessage(validationMessages, message);
                MarkInvalidCell(row, "WriteSymbol", markInvalidCells);
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Přidá validační zprávu jen jednou, aby panel neopakoval stejnou chybu pro více řádků.
        /// </summary>
        private static void AddValidationMessage(IList<string> validationMessages, string message)
        {
            if (!validationMessages.Contains(message))
                validationMessages.Add(message);
        }

        /// <summary>
        /// Sloučí validační zprávy do krátkého textu pro stavový panel editoru.
        /// </summary>
        private static string FormatValidationMessages(IList<string> validationMessages)
        {
            if (validationMessages.Count == 0)
                return "";

            if (validationMessages.Count == 1)
                return validationMessages[0];

            return validationMessages.Count + " errors: " + string.Join("; ", validationMessages);
        }

        /// <summary>
        /// Odstraní zvýraznění chyb ze všech buněk editoru.
        /// </summary>
        private void ClearValidationStyles()
        {
            foreach (DataGridViewRow row in gridTransitions.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Style.BackColor = Color.Empty;
            }
        }

        /// <summary>
        /// Zvýrazní prázdné povinné buňky řádku.
        /// </summary>
        private void MarkEmptyCells(DataGridViewRow row, bool markInvalidCells)
        {
            MarkInvalidCellIfEmpty(row, "CurrentState", markInvalidCells);
            MarkInvalidCellIfEmpty(row, "ReadSymbol", markInvalidCells);
            MarkInvalidCellIfEmpty(row, "NextState", markInvalidCells);
            MarkInvalidCellIfEmpty(row, "WriteSymbol", markInvalidCells);
            MarkInvalidCellIfEmpty(row, "HeadMove", markInvalidCells);
        }

        /// <summary>
        /// Zvýrazní buňky, které stále obsahují speciální volbu pro vytvoření nové hodnoty.
        /// </summary>
        private void MarkSpecialValueCells(DataGridViewRow row, bool markInvalidCells)
        {
            if (GetCellText(row, "CurrentState") == NewStateOption)
                MarkInvalidCell(row, "CurrentState", markInvalidCells);
            if (GetCellText(row, "NextState") == NewStateOption)
                MarkInvalidCell(row, "NextState", markInvalidCells);
            if (GetCellText(row, "ReadSymbol") == NewSymbolOption)
                MarkInvalidCell(row, "ReadSymbol", markInvalidCells);
            if (GetCellText(row, "WriteSymbol") == NewSymbolOption)
                MarkInvalidCell(row, "WriteSymbol", markInvalidCells);
        }

        /// <summary>
        /// Zvýrazní buňku, pokud je prázdná.
        /// </summary>
        private void MarkInvalidCellIfEmpty(DataGridViewRow row, string columnName, bool markInvalidCells)
        {
            if (GetCellText(row, columnName).Trim() == "")
                MarkInvalidCell(row, columnName, markInvalidCells);
        }

        /// <summary>
        /// Zvýrazní jednu buňku jako neplatnou.
        /// </summary>
        private static void MarkInvalidCell(DataGridViewRow row, string columnName, bool markInvalidCells)
        {
            if (markInvalidCells)
                row.Cells[columnName].Style.BackColor = InvalidCellBackColor;
        }

        /// <summary>
        /// Ověří, že pohyb hlavy patří mezi podporované symboly simulátoru.
        /// </summary>
        private static bool IsMoveSupported(char headMove)
        {
            return headMove == TuringMachine.MoveLeftSymbol || headMove == TuringMachine.MoveRightSymbol || headMove == TuringMachine.StopSymbol;
        }

        /// <summary>
        /// Posbírá všechny stavy použité jako vstupní nebo cílové stavy přechodů.
        /// </summary>
        private HashSet<string> CollectStates()
        {
            HashSet<string> states = new HashSet<string>();
            foreach (DataGridViewRow row in gridTransitions.Rows)
            {
                if (row.IsNewRow)
                    continue;

                AddState(states, GetCellText(row, "CurrentState"));
                AddState(states, GetCellText(row, "NextState"));
            }

            return states;
        }

        /// <summary>
        /// Přidá neprázdný stav do množiny, pokud nejde o speciální položku pro založení nového stavu.
        /// </summary>
        private static void AddState(ISet<string> states, string stateName)
        {
            if (!string.IsNullOrWhiteSpace(stateName) && stateName != NewStateOption)
                states.Add(stateName);
        }

        /// <summary>
        /// Bezpečně načte textovou hodnotu buňky z určeného sloupce.
        /// </summary>
        private static string GetCellText(DataGridViewRow row, string columnName)
        {
            object value = row.Cells[columnName].Value;
            return value == null ? "" : value.ToString();
        }

        /// <summary>
        /// Seřadí textové hodnoty a převede je na čitelný seznam.
        /// </summary>
        private static string FormatValues(IEnumerable<string> values)
        {
            List<string> list = new List<string>(values);
            list.Sort(StringComparer.Ordinal);
            return list.Count == 0 ? "-" : string.Join(", ", list);
        }

        /// <summary>
        /// Seřadí znakové hodnoty a převede je na čitelný seznam.
        /// </summary>
        private static string FormatValues(IEnumerable<char> values)
        {
            List<char> list = new List<char>(values);
            list.Sort();
            return list.Count == 0 ? "-" : string.Join(", ", list);
        }
    }

}
