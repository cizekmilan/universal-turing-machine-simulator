using System;
using System.IO;
using System.Text;
using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy ukázkových programů uložených ve složce demos.
    /// </summary>
    public class DemoProgramTest
    {
        [Theory]
        [InlineData("bin_decrement.tm", "0111")]
        [InlineData("bin_decrement.btm", "0111")]
        [InlineData("bin_bitwise_not.tm", "010011")]
        [InlineData("bin_bitwise_not.btm", "010011")]
        [InlineData("bin_mirroring.tm", "11001011010011")]
        [InlineData("bin_mirroring.btm", "11001011010011")]
        public void DemoProgram_RunsToExpectedOutput(string fileName, string expectedOutput)
        {
            TuringSimulator simulator = new TuringSimulator();

            string errorMessage;
            Assert.True(simulator.LoadProgram(Path.Combine(GetWorkspaceRoot(), "demos", fileName), out errorMessage), errorMessage);

            simulator.Run(false);

            Assert.Equal("qF", simulator.Machine.CurrentState());
            Assert.Equal(expectedOutput, ReadOutput(simulator.Machine, expectedOutput.Length));
        }

        private static string ReadOutput(Tape tape, int length)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < length; i++)
                output.Append(tape.Cells[10 + i]);

            return output.ToString();
        }

        private static string GetWorkspaceRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "UTMS.sln")))
            {
                directory = directory.Parent;
            }

            if (directory == null)
                throw new DirectoryNotFoundException("Nepodařilo se najít kořen projektu.");

            return directory.FullName;
        }
    }
}
