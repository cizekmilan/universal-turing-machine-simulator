using System;
using System.IO;
using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy načítání souborů do formální definice Turingova stroje.
    /// </summary>
    public class TuringMachineDefinitionLoaderTest
    {
        [Fact]
        public void Load_ReturnsDefinitionFromTextFile()
        {
            string path = WriteTempFile(
                "alphabet = {0,1}",
                "tapeAlphabet = {0,1,#,x}",
                "blank = #",
                "(q0, 0) = (qF, x, S)",
                "w = 0");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.NotNull(definition);
                Assert.Equal("", errorMessage);
                Assert.Equal("0", definition.InputData);
                Assert.Contains('x', definition.TapeAlphabet);
                Assert.Single(definition.Transitions);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_AcceptsCustomBlankSymbolFromTextFile()
        {
            string path = WriteTempFile(
                "alphabet = {0,1}",
                "tapeAlphabet = {0,1,_}",
                "blank = _",
                "(q0, _) = (qF, 1, S)");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.NotNull(definition);
                Assert.Equal("", errorMessage);
                Assert.Equal('_', definition.BlankSymbol);
                Assert.Contains('_', definition.TapeAlphabet);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_ReturnsNullAndErrorForInvalidFile()
        {
            string path = WriteTempFile(
                "alphabet = {0,1}",
                "tapeAlphabet = {0,1,#}",
                "blank = #",
                "(q0, 0) = (qF, x, S)",
                "w = 0");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.Null(definition);
                Assert.Equal("Prechodove funkce obsahuji symbol mimo paskovou abecedu.", errorMessage);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_ThrowsWhenUsingExceptionBasedApi()
        {
            string path = WriteTempFile("(q0, #) = (qF, 1, X)");

            try
            {
                Assert.Throws<InvalidOperationException>(() => TuringMachineDefinitionLoader.Load(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_ReturnsErrorForEmptyProgram()
        {
            string path = WriteTempFile("// comment only", "");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.Null(definition);
                Assert.Equal("Program neobsahuje zadne prikazy.", errorMessage);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_ReturnsErrorForMissingFileName()
        {
            string errorMessage;

            TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(null, out errorMessage);

            Assert.Null(definition);
            Assert.Equal("Nazev souboru je prazdny.", errorMessage);
        }

        [Fact]
        public void Load_ReturnsErrorForSyntaxProblem()
        {
            string path = WriteTempFile("(q0, #) = (qF, 1, X)");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.Null(definition);
                Assert.Equal("Objevila se syntakticka chyba. Program nemuze pokracovat.", errorMessage);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void Load_ReturnsErrorForInputOutsideInputAlphabet()
        {
            string path = WriteTempFile(
                "alphabet = {0,1}",
                "tapeAlphabet = {0,1,#,x}",
                "blank = #",
                "(q0, x) = (qF, x, S)",
                "w = x");

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.Null(definition);
                Assert.Contains("neni ve vstupni abecede", errorMessage);
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
