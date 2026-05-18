using System;
using System.IO;
using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy čisté simulační logiky bez závislosti na GUI.
    /// </summary>
    public class TuringSimulatorTest
    {
        [Fact]
        public void Step_AppliesMatchingTransitionWithoutGraphics()
        {
            string path = WriteTempFile("(q0, #) = (qF, 1, S)");
            TuringSimulator simulator = new TuringSimulator();
            int executedIndex = -1;
            int redrawCount = 0;
            simulator.TransitionExecuting += index => executedIndex = index;
            simulator.TapeChanged += (sender, args) => redrawCount++;

            try
            {
                string errorMessage;
                Assert.True(simulator.LoadProgram(path, out errorMessage));

                SimulationStep step = simulator.Step();

                Assert.NotNull(step);
                Assert.True(step.TransitionFound);
                Assert.Equal(0, executedIndex);
                Assert.True(redrawCount >= 2);
                Assert.Equal("q0", step.InputState);
                Assert.Equal('#', step.InputSymbol);
                Assert.Equal("qF", step.OutputState);
                Assert.Equal('1', step.OutputSymbol);
                Assert.Equal('S', step.HeadMove);
                Assert.Equal("qF", simulator.Machine.CurrentState());
                Assert.Equal('1', simulator.Machine.ReadSymbol());
                Assert.Equal(10, simulator.Machine.HeadIndex());
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Run_ExecutesLoadedProgramUntilFinalState()
        {
            string path = WriteTempFile("(q0, #) = (qF, 1, S)");
            TuringSimulator simulator = new TuringSimulator();
            int completedSteps = 0;
            simulator.TuringTransitionCompleted += (sender, args) => completedSteps++;

            try
            {
                string errorMessage;
                Assert.True(simulator.LoadProgram(path, out errorMessage));

                string summary = simulator.Run(false);

                Assert.Equal(1, completedSteps);
                Assert.Equal(1, simulator.StepCount);
                Assert.Contains("Celkem provedeno kroku: 1", summary);
                Assert.Contains("Posledni stav je: qF", summary);
                Assert.Contains("Koncovy stav.", summary);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void LoadProgram_WritesInputDataToTapeWithoutExternalSubscriber()
        {
            string path = WriteTempFile(
                "(q0, 1) = (qF, 0, S)",
                "w = 1");
            TuringSimulator simulator = new TuringSimulator();

            try
            {
                string errorMessage;
                Assert.True(simulator.LoadProgram(path, out errorMessage));

                Assert.Equal('1', simulator.Machine.ReadSymbol());
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void LoadProgram_RaisesInputDataLoadedWhenFileContainsInput()
        {
            string path = WriteTempFile(
                "(q0, 1) = (qF, 0, S)",
                "w = 1");
            TuringSimulator simulator = new TuringSimulator();
            string loadedInputData = null;
            simulator.InputDataLoaded += data => loadedInputData = data;

            try
            {
                string errorMessage;
                Assert.True(simulator.LoadProgram(path, out errorMessage));

                Assert.Equal("1", loadedInputData);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void LoadProgram_LoadsDefinitionWithoutFile()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#' },
                '#',
                "1",
                new TransitionFunction[] { new TransitionFunction("q0", '1', "qF", '0', 'S') });
            TuringSimulator simulator = new TuringSimulator();

            string errorMessage;
            Assert.True(simulator.LoadProgram(definition, out errorMessage), errorMessage);

            string summary = simulator.Run(false);

            Assert.Equal("qF", simulator.Machine.CurrentState());
            Assert.Equal('0', simulator.Machine.ReadSymbol());
            Assert.Contains("Koncovy stav.", summary);
        }

        [Fact]
        public void LoadProgram_UsesDefinitionBlankSymbol()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '_' },
                '_',
                "",
                new TransitionFunction[] { new TransitionFunction("q0", '_', "qF", '1', 'S') });
            TuringSimulator simulator = new TuringSimulator();

            string errorMessage;
            Assert.True(simulator.LoadProgram(definition, out errorMessage), errorMessage);

            SimulationStep step = simulator.Step();

            Assert.NotNull(step);
            Assert.Equal('_', simulator.Machine.BlankSymbol);
            Assert.Equal('_', step.InputSymbol);
            Assert.Equal('1', simulator.Machine.ReadSymbol());
            Assert.Equal("qF", simulator.Machine.CurrentState());
        }

        [Fact]
        public void Run_ReturnsUnknownStateSummaryWhenTransitionIsMissing()
        {
            string path = WriteTempFile("(q1, #) = (qF, 1, S)");
            TuringSimulator simulator = new TuringSimulator();

            try
            {
                string errorMessage;
                Assert.True(simulator.LoadProgram(path, out errorMessage));

                string summary = simulator.Run(false);

                Assert.Equal(1, simulator.StepCount);
                Assert.True(simulator.LastStep.UnknownState);
                Assert.Contains("V prechodovych funkcich nebyl nalezen stav \"q0\".", summary);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static string WriteTempFile(params string[] lines)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tm");
            File.WriteAllLines(path, lines);
            return path;
        }
    }
}
