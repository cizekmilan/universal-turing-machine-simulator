using System;
using System.Text;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Událost dokončení jednoho kroku simulace.
    /// </summary>
    public delegate void TuringTransitionCompletedHandler(object sender, EventArgs e);

    /// <summary>
    /// Událost změny pásky nebo pozice hlavy.
    /// </summary>
    public delegate void TapeChangedHandler(object sender, EventArgs e);

    /// <summary>
    /// Událost vložení přechodové funkce do programu.
    /// </summary>
    public delegate void TransitionLoadedHandler(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove);

    /// <summary>
    /// Událost provádění přechodové funkce.
    /// </summary>
    public delegate void TransitionExecutingHandler(int index);

    /// <summary>
    /// Událost syntaktické chyby v programu.
    /// </summary>
    public delegate void SyntaxErrorHandler(string description, string line);

    /// <summary>
    /// Událost načtení vstupních dat pro pásku.
    /// </summary>
    public delegate void InputDataLoadedHandler(string inputData);

    /// <summary>
    /// Popis jednoho dokončeného nebo nenalezeného kroku simulace.
    /// </summary>
    public class SimulationStep
    {
        /// <summary>
        /// Vytvoří popis výsledku jednoho simulačního kroku.
        /// </summary>
        public SimulationStep(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove, int transitionIndex, bool transitionFound)
        {
            InputState = inputState;
            InputSymbol = inputSymbol;
            OutputState = outputState;
            OutputSymbol = outputSymbol;
            HeadMove = headMove;
            TransitionIndex = transitionIndex;
            TransitionFound = transitionFound;
        }

        /// <summary>
        /// Stav stroje před provedením kroku.
        /// </summary>
        public string InputState { get; private set; }

        /// <summary>
        /// Symbol přečtený z pásky.
        /// </summary>
        public char InputSymbol { get; private set; }

        /// <summary>
        /// Stav stroje po provedení kroku.
        /// </summary>
        public string OutputState { get; private set; }

        /// <summary>
        /// Symbol zapsaný na pásku.
        /// </summary>
        public char OutputSymbol { get; private set; }

        /// <summary>
        /// Pohyb hlavy po provedení kroku.
        /// </summary>
        public char HeadMove { get; private set; }

        /// <summary>
        /// Index prováděné přechodové funkce.
        /// </summary>
        public int TransitionIndex { get; private set; }

        /// <summary>
        /// Určuje, zda byla nalezena odpovídající přechodová funkce.
        /// </summary>
        public bool TransitionFound { get; private set; }

        /// <summary>
        /// Určuje, zda se simulace zastavila kvůli neznámému stavu.
        /// </summary>
        public bool UnknownState
        {
            get { return !TransitionFound; }
        }
    }

    /// <summary>
    /// Čistá výpočetní logika simulátoru bez vazby na GUI.
    /// </summary>
    public class TuringSimulator
    {
        private TuringMachineProgram program;
        private TuringMachine machine;
        private int stepCount;
        private bool unknownState;

        /// <summary>
        /// Nastane po změně pásky nebo pozice hlavy.
        /// </summary>
        public event TapeChangedHandler TapeChanged;

        /// <summary>
        /// Nastane při načtení přechodové funkce.
        /// </summary>
        public event TransitionLoadedHandler TransitionLoaded;

        /// <summary>
        /// Nastane těsně před provedením přechodové funkce.
        /// </summary>
        public event TransitionExecutingHandler TransitionExecuting;

        /// <summary>
        /// Nastane při syntaktické chybě programu.
        /// </summary>
        public event SyntaxErrorHandler SyntaxError;

        /// <summary>
        /// Nastane při načtení vstupních dat pro pásku.
        /// </summary>
        public event InputDataLoadedHandler InputDataLoaded;

        /// <summary>
        /// Nastane po dokončení simulačního kroku.
        /// </summary>
        public event TuringTransitionCompletedHandler TuringTransitionCompleted;

        /// <summary>
        /// Aktuální instance simulovaného stroje.
        /// </summary>
        public TuringMachine Machine
        {
            get { return machine; }
        }

        /// <summary>
        /// Aktuálně načtený program stroje.
        /// </summary>
        public TuringMachineProgram Program
        {
            get { return program; }
        }

        /// <summary>
        /// Počet provedených kroků.
        /// </summary>
        public int StepCount
        {
            get { return stepCount; }
        }

        /// <summary>
        /// Poslední provedený nebo neúspěšně vyhledaný krok.
        /// </summary>
        public SimulationStep LastStep { get; private set; }

        /// <summary>
        /// Poslední chyba vrácená loaderem nebo simulátorem.
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Nastaví vstupní data na pásku aktuálního stroje.
        /// </summary>
        public void SetTapeData(string input)
        {
            machine?.SetData(input);
            TapeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Načte program ze souboru bez předání chybové zprávy volajícímu.
        /// </summary>
        public bool LoadProgram(string fileName)
        {
            string errorMessage;
            return LoadProgram(fileName, out errorMessage);
        }

        /// <summary>
        /// Načte program ze souboru a vrátí případnou chybovou zprávu.
        /// </summary>
        public bool LoadProgram(string fileName, out string errorMessage)
        {
            stepCount = 0;
            unknownState = false;
            LastStep = null;
            LastError = "";
            machine = new TuringMachine();
            program = CreateProgram();

            errorMessage = "";
            TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(fileName, out errorMessage, SyntaxError, InputDataLoaded);
            if (definition == null)
            {
                LastError = errorMessage;
                TapeChanged?.Invoke(this, EventArgs.Empty);
                return false;
            }

            return LoadProgramCore(definition, out errorMessage);
        }

        /// <summary>
        /// Načte program přímo z formální definice stroje.
        /// </summary>
        public bool LoadProgram(TuringMachineDefinition definition)
        {
            string errorMessage;
            return LoadProgram(definition, out errorMessage);
        }

        /// <summary>
        /// Načte program přímo z formální definice stroje a vrátí případnou chybovou zprávu.
        /// </summary>
        public bool LoadProgram(TuringMachineDefinition definition, out string errorMessage)
        {
            stepCount = 0;
            unknownState = false;
            LastStep = null;
            LastError = "";
            machine = new TuringMachine();
            program = CreateProgram();

            if (definition == null)
            {
                errorMessage = "Definice stroje neni zadana.";
                LastError = errorMessage;
                TapeChanged?.Invoke(this, EventArgs.Empty);
                return false;
            }

            if (definition.BlankSymbol != Tape.BlankSymbol)
            {
                errorMessage = "Aktualni simulator zatim podporuje pouze blank symbol \"" + Tape.BlankSymbol + "\".";
                LastError = errorMessage;
                TapeChanged?.Invoke(this, EventArgs.Empty);
                return false;
            }

            return LoadProgramCore(definition, out errorMessage);
        }

        private TuringMachineProgram CreateProgram()
        {
            TuringMachineProgram newProgram = new TuringMachineProgram();
            newProgram.TransitionLoaded += TransitionLoaded;
            return newProgram;
        }

        private bool LoadProgramCore(TuringMachineDefinition definition, out string errorMessage)
        {
            program.LoadDefinition(definition);
            machine.SetData(definition.InputData);
            errorMessage = "";
            LastError = "";
            TapeChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Provede jeden krok simulace.
        /// </summary>
        public SimulationStep Step()
        {
            if (program == null || machine == null)
            {
                LastError = "Program neni nacten.";
                return null;
            }

            string outputState;
            char outputSymbol;
            char headMove;
            int index;
            char inputSymbol = machine.ReadSymbol();
            string inputState = machine.CurrentState();

            bool found = program.TryFindTransition(inputState, inputSymbol, out outputState, out outputSymbol, out headMove, out index);
            if (found)
            {
                TransitionExecuting?.Invoke(index);
                machine.SetCurrentState(outputState);
                machine.WriteSymbol(outputSymbol);
                if (headMove == TuringMachine.MoveLeftSymbol)
                    machine.MoveLeft();
                else if (headMove == TuringMachine.MoveRightSymbol)
                    machine.MoveRight();
                else if (headMove == TuringMachine.StopSymbol)
                    machine.Stay();
            }
            else
            {
                unknownState = true;
            }

            LastStep = new SimulationStep(inputState, inputSymbol, outputState, outputSymbol, headMove, index, found);
            TapeChanged?.Invoke(this, EventArgs.Empty);
            return LastStep;
        }

        /// <summary>
        /// Spustí simulaci buď celou, nebo po jednom kroku.
        /// </summary>
        public string Run(bool stepByStep)
        {
            if (program == null || machine == null)
            {
                LastError = "Program neni nacten.";
                return "Chyba: " + LastError;
            }

            try
            {
                TapeChanged?.Invoke(this, EventArgs.Empty);
                if (stepByStep)
                {
                    if (!CanContinue())
                        return BuildSummary();

                    Step();
                    stepCount++;
                    TuringTransitionCompleted?.Invoke(this, EventArgs.Empty);
                    if (CanContinue())
                        return "";

                    return BuildSummary();
                }

                while (CanContinue())
                {
                    Step();
                    stepCount++;
                    TuringTransitionCompleted?.Invoke(this, EventArgs.Empty);
                }

                return BuildSummary();
            }
            catch (Exception e)
            {
                LastError = e.Message;
                return "Chyba: " + e.Message;
            }
        }

        private bool CanContinue()
        {
            return stepCount < TuringMachine.MaxSteps && !machine.IsInFinalState() && !machine.HasOverflowed && !unknownState;
        }

        private string BuildSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("********* STATISTIKA **********");
            sb.AppendLine("Celkem provedeno kroku: " + stepCount);
            sb.AppendLine("Posledni stav je: " + machine.CurrentState());

            if (stepCount >= TuringMachine.MaxSteps)
                sb.AppendLine("Prilis velky pocet kroku.");
            if (machine.IsInFinalState())
                sb.AppendLine("Koncovy stav.");
            if (machine.HasOverflowed)
                sb.AppendLine("Pristup mimo pasku.");
            if (unknownState)
                sb.AppendLine("V prechodovych funkcich nebyl nalezen stav \"" + machine.CurrentState() + "\".");

            return sb.ToString();
        }
    }
}
