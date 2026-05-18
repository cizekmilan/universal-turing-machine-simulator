using System;
using System.Collections.Generic;
using System.IO;
using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy ukládání programu do textového a binárního formátu.
    /// </summary>
    public class TuringMachineProgramSerializerTest
    {
        [Fact]
        public void ToText_WritesProgramAndInputData()
        {
            List<TransitionFunction> program = new List<TransitionFunction>
            {
                new TransitionFunction("q0", '#', "qF", '1', 'S')
            };

            string text = TuringMachineProgramSerializer.ToText(program, "1011");

            Assert.Contains("alphabet = {0,1}", text);
            Assert.Contains("tapeAlphabet = {0,1,#}", text);
            Assert.Contains("blank = #", text);
            Assert.Contains("(q0, #) = (qF, 1, S)", text);
            Assert.Contains("w = 1011", text);
        }

        [Fact]
        public void ToBinary_WritesExpectedIncrementDemoCode()
        {
            List<TransitionFunction> program = CreateIncrementProgram();
            string expected = File.ReadAllText(Path.Combine(GetWorkspaceRoot(), "demos", "bin_increment.btm")).Trim();

            string actual = TuringMachineProgramSerializer.ToBinary(program, "1011");

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ToBinary_CanBeDecodedBackToTransitionsAndInputData()
        {
            List<TransitionFunction> program = new List<TransitionFunction>
            {
                new TransitionFunction("q0", '#', "qF", '1', 'S')
            };
            string encoded = TuringMachineProgramSerializer.ToBinary(program, "101");
            string errorMessage = "";
            BinaryCode binaryCode = new BinaryCode(encoded);

            List<TransitionFunction> decoded = binaryCode.MakeTextInstructions(ref errorMessage);

            Assert.NotNull(decoded);
            Assert.Single(decoded);
            Assert.Equal("101", binaryCode.InputData);
            Assert.Equal("q0", decoded[0].InputState);
            Assert.Equal('#', decoded[0].InputSymbol);
            Assert.Equal("qF", decoded[0].OutputState);
            Assert.Equal('1', decoded[0].OutputSymbol);
            Assert.Equal('S', decoded[0].HeadMove);
        }

        [Fact]
        public void SaveTextAndSaveBinary_WriteFiles()
        {
            List<TransitionFunction> program = new List<TransitionFunction>
            {
                new TransitionFunction("q0", '#', "qF", '1', 'S')
            };
            string textPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tm");
            string binaryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".btm");

            try
            {
                TuringMachineProgramSerializer.SaveText(textPath, program, "1");
                TuringMachineProgramSerializer.SaveBinary(binaryPath, program, "1");

                Assert.Contains("alphabet = {0,1}", File.ReadAllText(textPath));
                Assert.StartsWith("1111", File.ReadAllText(binaryPath));
            }
            finally
            {
                File.Delete(textPath);
                File.Delete(binaryPath);
            }
        }

        [Fact]
        public void ToBinary_RejectsNamedStatesThatCannotBeEncoded()
        {
            List<TransitionFunction> program = new List<TransitionFunction>
            {
                new TransitionFunction("copyRtoL", '0', "qF", '0', 'R')
            };

            Assert.Throws<ArgumentException>(() => TuringMachineProgramSerializer.ToBinary(program, ""));
        }

        [Fact]
        public void ToBinary_UsesDeclaredTapeAlphabetForHelperSymbols()
        {
            List<TransitionFunction> program = new List<TransitionFunction>
            {
                new TransitionFunction("q0", '0', "q1", 'o', 'R'),
                new TransitionFunction("q1", '#', "qF", '#', 'S')
            };
            string encoded = TuringMachineProgramSerializer.ToBinary(program, "0", new char[] { '0', '1' }, new char[] { '0', '1', '#', 'o' }, '#');
            string errorMessage = "";
            BinaryCode binaryCode = new BinaryCode(encoded);

            List<TransitionFunction> decoded = binaryCode.MakeTextInstructions(ref errorMessage);

            Assert.NotNull(decoded);
            Assert.Equal('o', decoded[0].OutputSymbol);
            Assert.Contains('o', binaryCode.TapeAlphabet);
        }

        [Fact]
        public void Serializer_WritesDefinitionToTextAndBinary()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#', 'x' },
                '#',
                "0",
                new TransitionFunction[] { new TransitionFunction("q0", '0', "qF", 'x', 'S') });

            string text = TuringMachineProgramSerializer.ToText(definition);
            string binary = TuringMachineProgramSerializer.ToBinary(definition);
            string errorMessage = "";
            BinaryCode binaryCode = new BinaryCode(binary);

            List<TransitionFunction> decoded = binaryCode.MakeTextInstructions(ref errorMessage);

            Assert.Contains("tapeAlphabet = {0,1,#,x}", text);
            Assert.NotNull(decoded);
            Assert.Equal('x', decoded[0].OutputSymbol);
        }

        private static List<TransitionFunction> CreateIncrementProgram()
        {
            return new List<TransitionFunction>
            {
                new TransitionFunction("q0", '1', "q0", '1', 'R'),
                new TransitionFunction("q0", '0', "q0", '0', 'R'),
                new TransitionFunction("q0", '#', "q1", '#', 'L'),
                new TransitionFunction("q1", '0', "q2", '1', 'L'),
                new TransitionFunction("q1", '1', "q3", '0', 'L'),
                new TransitionFunction("q2", '1', "q2", '1', 'L'),
                new TransitionFunction("q2", '0', "q2", '0', 'L'),
                new TransitionFunction("q2", '#', "qF", '#', 'R'),
                new TransitionFunction("q3", '1', "q3", '0', 'L'),
                new TransitionFunction("q3", '0', "q2", '1', 'L'),
                new TransitionFunction("q3", '#', "qF", '1', 'S')
            };
        }

        private static string GetWorkspaceRoot()
        {
            DirectoryInfo directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "UTMS.sln")))
            {
                directory = directory.Parent;
            }

            if (directory == null)
                throw new DirectoryNotFoundException("NepodaĹ™ilo se najĂ­t koĹ™en projektu.");

            return directory.FullName;
        }
    }
}
