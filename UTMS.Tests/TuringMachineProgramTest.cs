using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy přechodových funkcí, programu a stavu Turingova stroje.
    /// </summary>
    public class TuringMachineProgramTest
    {
        [Fact]
        public void TransitionFunction_MatchesOnlyStateAndInputSymbol()
        {
            TransitionFunction transition = new TransitionFunction("q0", '1', "q1", '0', 'R');

            Assert.True(transition.Matches("q0", '1'));
            Assert.False(transition.Matches("q0", '0'));
            Assert.False(transition.Matches("q1", '1'));
        }

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
