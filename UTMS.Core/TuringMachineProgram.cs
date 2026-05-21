using System;
using System.Collections.Generic;

namespace UTMS.Core
{
    /// <summary>
    /// Jedna přechodová funkce programu Turingova stroje.
    /// </summary>
    public class TransitionFunction
    {
        /// <summary>
        /// Název počátečního stavu podle nastavení.
        /// </summary>
        public static string InitialStateName { get; private set; }

        /// <summary>
        /// Název koncového stavu podle nastavení.
        /// </summary>
        public static string FinalStateName { get; private set; }

        static TransitionFunction()
        {
            InitialStateName = Properties.Settings.Default.InitialState;
            FinalStateName = Properties.Settings.Default.EndState;
        }

        /// <summary>
        /// Vytvoří výchozí přechodovou funkci.
        /// </summary>
        public TransitionFunction()
            : this(InitialStateName, '0', InitialStateName, '0', '0')
        {
        }

        /// <summary>
        /// Vytvoří přechodovou funkci z plné pětice hodnot.
        /// </summary>
        public TransitionFunction(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove)
        {
            InputState = inputState;
            InputSymbol = inputSymbol;
            OutputState = outputState;
            OutputSymbol = outputSymbol;
            HeadMove = headMove;
        }

        /// <summary>
        /// Stav, ve kterém se přechod uplatní.
        /// </summary>
        public string InputState { get; set; }

        /// <summary>
        /// Symbol, který musí být pod hlavou.
        /// </summary>
        public char InputSymbol { get; set; }

        /// <summary>
        /// Stav stroje po provedení přechodu.
        /// </summary>
        public string OutputState { get; set; }

        /// <summary>
        /// Symbol zapsaný na pásku.
        /// </summary>
        public char OutputSymbol { get; set; }

        /// <summary>
        /// Směr pohybu hlavy po provedení přechodu.
        /// </summary>
        public char HeadMove { get; set; }

        /// <summary>
        /// Určuje, zda přechod odpovídá zadanému stavu a symbolu.
        /// </summary>
        public bool Matches(string inputState, char inputSymbol)
        {
            return inputState == InputState && inputSymbol == InputSymbol;
        }
    }

    /// <summary>
    /// Runtime program Turingova stroje tvořený pouze seznamem přechodových funkcí.
    /// </summary>
    public class TuringMachineProgram
    {
        private readonly List<TransitionFunction> transitions;

        /// <summary>
        /// Vytvoří prázdný program.
        /// </summary>
        public TuringMachineProgram()
        {
            transitions = new List<TransitionFunction>();
        }

        /// <summary>
        /// Vytvoří program z formální definice stroje.
        /// </summary>
        public TuringMachineProgram(TuringMachineDefinition definition)
            : this()
        {
            LoadDefinition(definition);
        }

        /// <summary>
        /// Nastane po vložení nové přechodové funkce.
        /// </summary>
        public event TransitionLoadedHandler TransitionLoaded;

        /// <summary>
        /// Načtené přechodové funkce v pořadí programu.
        /// </summary>
        public IReadOnlyList<TransitionFunction> Transitions
        {
            get { return transitions; }
        }

        /// <summary>
        /// Nahradí přechody programu předanou definicí stroje.
        /// </summary>
        public void LoadDefinition(TuringMachineDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            transitions.Clear();
            foreach (TransitionFunction transition in definition.Transitions)
                AddTransition(transition.InputState, transition.InputSymbol, transition.OutputState, transition.OutputSymbol, transition.HeadMove);
        }

        /// <summary>
        /// Přidá přechodovou funkci do programu.
        /// </summary>
        public void AddTransition(string inputState, char inputSymbol, string outputState, char outputSymbol, char headMove)
        {
            transitions.Add(new TransitionFunction(inputState, inputSymbol, outputState, outputSymbol, headMove));
            TransitionLoaded?.Invoke(inputState, inputSymbol, outputState, outputSymbol, headMove);
        }

        /// <summary>
        /// Vyhledá první přechod odpovídající aktuálnímu stavu a symbolu.
        /// </summary>
        public bool TryFindTransition(string inputState, char inputSymbol, out string outputState, out char outputSymbol, out char headMove, out int index)
        {
            outputState = "?";
            outputSymbol = '0';
            headMove = '0';
            index = -1;

            for (int i = 0; i < transitions.Count; i++)
            {
                TransitionFunction transition = transitions[i];
                if (!transition.Matches(inputState, inputSymbol))
                    continue;

                outputState = transition.OutputState;
                outputSymbol = transition.OutputSymbol;
                headMove = transition.HeadMove;
                index = i;
                return true;
            }

            return false;
        }
    }
}
