using UTMS.Core;
using Xunit;

namespace UTMS.Tests
{
    /// <summary>
    /// Testy přechodových funkcí, programu a stavu Turingova stroje.
    /// </summary>
    public class TuringMachineProgramTest
    {
        /// <summary>
        /// Ověřuje, že přechodová funkce se porovnává pouze podle vstupního stavu a čteného symbolu.
        /// </summary>
        [Fact]
        public void TransitionFunction_MatchesOnlyStateAndInputSymbol()
        {
            TransitionFunction transition = new TransitionFunction("q0", '1', "q1", '0', 'R');

            Assert.True(transition.Matches("q0", '1'));
            Assert.False(transition.Matches("q0", '0'));
            Assert.False(transition.Matches("q1", '1'));
        }

        /// <summary>
        /// Ověřuje přidání přechodů, událost načtení a vyhledání odpovídající přechodové funkce.
        /// </summary>
        [Fact]
        public void TuringMachineProgram_InsertsRaisesEventAndFindsTransition()
        {
            TuringMachineProgram program = new TuringMachineProgram();
            int eventCount = 0;
            string lastEventState = null;
            char lastEventInput = '\0';
            program.TransitionLoaded += (inputState, inputSymbol, outputState, outputSymbol, headMove) =>
            {
                eventCount++;
                lastEventState = inputState;
                lastEventInput = inputSymbol;
            };

            program.AddTransition("q0", '1', "q1", '0', 'R');
            program.AddTransition("q1", '#', "qF", '#', 'S');

            Assert.Equal(2, eventCount);
            Assert.Equal("q1", lastEventState);
            Assert.Equal('#', lastEventInput);
            Assert.Equal(2, program.Transitions.Count);

            string outputState;
            char outputSymbol;
            char headMove;
            int index;
            bool found = program.TryFindTransition("q1", '#', out outputState, out outputSymbol, out headMove, out index);

            Assert.True(found);
            Assert.Equal("qF", outputState);
            Assert.Equal('#', outputSymbol);
            Assert.Equal('S', headMove);
            Assert.Equal(1, index);
        }

        /// <summary>
        /// Ověřuje výchozí hodnoty vracené při hledání neexistujícího přechodu.
        /// </summary>
        [Fact]
        public void TuringMachineProgram_ReturnsDefaultsWhenTransitionDoesNotExist()
        {
            TuringMachineProgram program = new TuringMachineProgram();

            string outputState;
            char outputSymbol;
            char headMove;
            int index;
            bool found = program.TryFindTransition("q9", '1', out outputState, out outputSymbol, out headMove, out index);

            Assert.False(found);
            Assert.Equal("?", outputState);
            Assert.Equal('0', outputSymbol);
            Assert.Equal('0', headMove);
            Assert.Equal(-1, index);
        }

        /// <summary>
        /// Ověřuje, že formální definice zachová vstup, páskovou abecedu a oddělí pomocné symboly od vstupní abecedy.
        /// </summary>
        [Fact]
        public void TuringMachineDefinition_ValidatesFormalDefinition()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#', 'x' },
                '#',
                "01",
                new TransitionFunction[] { new TransitionFunction("q0", '0', "qF", 'x', 'S') });

            Assert.Equal("01", definition.InputData);
            Assert.Contains('x', definition.TapeAlphabet);
            Assert.DoesNotContain('x', definition.Alphabet);
        }

        /// <summary>
        /// Ověřuje, že odvození páskové abecedy ponechá vstupní abecedu oddělenou od pomocných symbolů.
        /// </summary>
        [Fact]
        public void TuringMachineDefinition_InferTapeAlphabetAddsBlankAndHelperSymbols()
        {
            TransitionFunction[] transitions = new TransitionFunction[]
            {
                new TransitionFunction("q0", '0', "q1", 'x', 'R'),
                new TransitionFunction("q1", 'x', "qF", '#', 'S')
            };

            char[] tapeAlphabet = TuringMachineDefinition.InferTapeAlphabet(new char[] { '0', '1' }, '#', transitions);

            Assert.Contains('0', tapeAlphabet);
            Assert.Contains('1', tapeAlphabet);
            Assert.Contains('#', tapeAlphabet);
            Assert.Contains('x', tapeAlphabet);
            Assert.Equal(4, tapeAlphabet.Length);
        }

        /// <summary>
        /// Ověřuje, že definice odmítne vstupní slovo obsahující symbol mimo vstupní abecedu.
        /// </summary>
        [Fact]
        public void TuringMachineDefinition_RejectsInputOutsideAlphabet()
        {
            Assert.Throws<System.ArgumentException>(() => new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#', 'x' },
                '#',
                "0x",
                new TransitionFunction[] { new TransitionFunction("q0", '0', "qF", '0', 'S') }));
        }

        /// <summary>
        /// Ověřuje, že definice odmítne duplicitní přechody pro stejný stav a čtený symbol.
        /// </summary>
        [Fact]
        public void TuringMachineDefinition_RejectsDuplicateTransitions()
        {
            System.ArgumentException exception = Assert.Throws<System.ArgumentException>(() => new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#' },
                '#',
                "",
                new TransitionFunction[]
                {
                    new TransitionFunction("q0", '0', "q1", '1', 'R'),
                    new TransitionFunction("q0", '0', "qF", '0', 'S')
                }));

            Assert.Contains("defined more than once", exception.Message);
        }

        /// <summary>
        /// Ověřuje, že načtení nové definice nahradí původní obsah runtime programu.
        /// </summary>
        [Fact]
        public void TuringMachineProgram_LoadDefinitionReplacesProgramContents()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#' },
                '#',
                "1",
                new TransitionFunction[] { new TransitionFunction("q0", '1', "qF", '0', 'S') });
            TuringMachineProgram program = new TuringMachineProgram();

            program.AddTransition("q0", '#', "qF", '#', 'S');
            program.LoadDefinition(definition);

            Assert.Single(program.Transitions);
            Assert.Equal('1', program.Transitions[0].InputSymbol);
        }

        /// <summary>
        /// Ověřuje práci s aktuálním stavem stroje a rozpoznání koncového stavu.
        /// </summary>
        [Fact]
        public void TuringMachine_TracksCurrentAndFinalState()
        {
            TuringMachine machine = new TuringMachine();

            Assert.Equal("q0", machine.CurrentState());
            Assert.False(machine.IsInFinalState());

            machine.SetCurrentState("qF");

            Assert.Equal("qF", machine.CurrentState());
            Assert.True(machine.IsInFinalState());
        }
    }
}
