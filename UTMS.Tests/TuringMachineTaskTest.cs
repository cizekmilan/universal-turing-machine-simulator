using System;
using System.IO;
using System.Text;
using UTMS.Core;
using Xunit;

namespace UTMS.Tests
{
    /// <summary>
    /// Behaviorální testy konkrétních úloh Turingova stroje definovaných přímo v testech.
    /// </summary>
    public class TuringMachineTaskTest
    {
        /// <summary>
        /// Ověřuje, že textová definice binárního inkrementu zvýší vstupní číslo o 1.
        /// </summary>
        [Fact]
        public void TextProgram_RunsBinaryIncrement()
        {
            AssertTextProgramProduces(BinaryIncrementProgram, "1100");
        }

        /// <summary>
        /// Ověřuje, že textová definice binárního dekrementu sníží vstupní číslo o 1.
        /// </summary>
        [Fact]
        public void TextProgram_RunsBinaryDecrement()
        {
            AssertTextProgramProduces(BinaryDecrementProgram, "0111");
        }

        /// <summary>
        /// Ověřuje, že textová definice bitové negace převrátí všechny bity vstupu.
        /// </summary>
        [Fact]
        public void TextProgram_RunsBitwiseNot()
        {
            AssertTextProgramProduces(BinaryBitwiseNotProgram, "010011");
        }

        /// <summary>
        /// Ověřuje, že textová definice zrcadlení připojí za vstup jeho obrácenou kopii.
        /// </summary>
        [Fact]
        public void TextProgram_RunsBinaryMirroring()
        {
            AssertTextProgramProduces(BinaryMirroringProgram, "11001011010011");
        }

        /// <summary>
        /// Ověřuje, že textová definice binárního posunu doleva připíše na konec čísla nulu.
        /// </summary>
        [Fact]
        public void TextProgram_RunsBinaryShiftLeft()
        {
            AssertTextProgramProduces(BinaryShiftLeftProgram, "10110");
        }

        /// <summary>
        /// Ověřuje, že kontrola palindromu vrátí 1 pro palindromický vstup.
        /// </summary>
        [Fact]
        public void TextProgram_RunsPalindromeCheckForPalindrome()
        {
            AssertTextProgramProduces(PalindromeCheckProgram, "1");
        }

        /// <summary>
        /// Ověřuje, že kontrola palindromu vrátí 0 pro nepalindromický vstup.
        /// </summary>
        [Fact]
        public void TextProgram_RunsPalindromeCheckForNonPalindrome()
        {
            AssertTextProgramProduces(PalindromeCheckProgram.Replace("w = 10101", "w = 10110"), "0");
        }

        /// <summary>
        /// Načte textovou definici stroje, spustí ji a porovná začátek pásky s očekávaným výstupem.
        /// </summary>
        private static void AssertTextProgramProduces(string programText, string expectedOutput)
        {
            TuringMachineDefinition definition = LoadDefinitionFromText(programText);
            TuringSimulator simulator = new TuringSimulator();

            string errorMessage;
            Assert.True(simulator.LoadProgram(definition, out errorMessage), errorMessage);

            simulator.Run(false);

            Assert.Equal("qF", simulator.Machine.CurrentState());
            Assert.Equal(expectedOutput, ReadOutput(simulator.Machine, expectedOutput.Length));
        }

        /// <summary>
        /// Zapíše text programu do dočasného souboru a načte jej běžným loaderem textového formátu.
        /// </summary>
        private static TuringMachineDefinition LoadDefinitionFromText(string programText)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".tm");
            File.WriteAllText(path, programText);

            try
            {
                string errorMessage;
                TuringMachineDefinition definition = TuringMachineDefinitionLoader.Load(path, out errorMessage);

                Assert.NotNull(definition);
                Assert.Equal("", errorMessage);
                return definition;
            }
            finally
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Přečte z pásky souvislý výstup od pozice, na kterou simulátor standardně vkládá vstupní slovo.
        /// </summary>
        private static string ReadOutput(Tape tape, int length)
        {
            StringBuilder output = new StringBuilder();
            for (int i = 0; i < length; i++)
                output.Append(tape.Cells[10 + i]);

            return output.ToString();
        }

        private const string BinaryIncrementProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#}
            blank = #

            (q0, 1) = (q0, 1, R)
            (q0, 0) = (q0, 0, R)
            (q0, #) = (q1, #, L)
            (q1, 0) = (q2, 1, L)
            (q1, 1) = (q3, 0, L)
            (q2, 1) = (q2, 1, L)
            (q2, 0) = (q2, 0, L)
            (q2, #) = (qF, #, R)
            (q3, 1) = (q3, 0, L)
            (q3, 0) = (q2, 1, L)
            (q3, #) = (qF, 1, S)
            w = 1011
            """;

        private const string BinaryDecrementProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#}
            blank = #

            (q0, 0) = (q0, 0, R)
            (q0, 1) = (q0, 1, R)
            (q0, #) = (q1, #, L)
            (q1, 0) = (q1, 1, L)
            (q1, 1) = (q2, 0, L)
            (q1, #) = (qF, #, R)
            (q2, 0) = (q2, 0, L)
            (q2, 1) = (q2, 1, L)
            (q2, #) = (qF, #, R)
            w = 1000
            """;

        private const string BinaryBitwiseNotProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#}
            blank = #

            (q0, 0) = (q0, 1, R)
            (q0, 1) = (q0, 0, R)
            (q0, #) = (qF, #, S)

            w = 101100
            """;

        private const string BinaryMirroringProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#,o,i}
            blank = #

            (q0,0) = (q0,0,R)
            (q0,1) = (q0,1,R)
            (q0,o) = (q0,0,R)
            (q0,i) = (q0,1,R)
            (q0, #)=(q1, #,L)
            (q1,0) = (q2,o, R)
            (q1,1) = (q3,i, R)
            (q1,o) = (q1,o,L)
            (q1,i) = (q1,i,L)
            (q2, #) = (q1,o,L)
            (q3, #) = (q1,i,L)
            (q2,0) = (q2,0, R)
            (q2,1) = (q2,1, R)
            (q2,o) = (q2,o, R)
            (q2,i) = (q2,i, R)
            (q3,0) = (q3,0, R)
            (q3,1) = (q3,1, R)
            (q3,o) = (q3,o, R)
            (q3,i) = (q3,i, R)
            (q1, #) = (q4, #, R)
            (q4,o) = (q4,0, R)
            (q4,i) = (q4,1, R)
            (q4,0) = (q4,0,L)
            (q4,1) = (q4,1,L)
            (q4, #) = (qF, #, R)

            w = 1100101
            """;

        private const string BinaryShiftLeftProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#}
            blank = #

            (q0, 0) = (q0, 0, R)
            (q0, 1) = (q0, 1, R)
            (q0, #) = (qF, 0, S)

            w = 1011
            """;

        private const string PalindromeCheckProgram =
            """
            alphabet = {0,1}
            tapeAlphabet = {0,1,#,x,y}
            blank = #

            (q0, x) = (q0, x, R)
            (q0, y) = (q0, y, R)
            (q0, 0) = (q1, x, R)
            (q0, 1) = (q2, y, R)
            (q0, #) = (q6, #, L)

            (q1, 0) = (q1, 0, R)
            (q1, 1) = (q1, 1, R)
            (q1, x) = (q1, x, R)
            (q1, y) = (q1, y, R)
            (q1, #) = (q3, #, L)

            (q2, 0) = (q2, 0, R)
            (q2, 1) = (q2, 1, R)
            (q2, x) = (q2, x, R)
            (q2, y) = (q2, y, R)
            (q2, #) = (q4, #, L)

            (q3, x) = (q3, x, L)
            (q3, y) = (q3, y, L)
            (q3, 0) = (q5, x, L)
            (q3, 1) = (q9, 1, L)
            (q3, #) = (q6, #, R)

            (q4, x) = (q4, x, L)
            (q4, y) = (q4, y, L)
            (q4, 1) = (q5, y, L)
            (q4, 0) = (q9, 0, L)
            (q4, #) = (q6, #, R)

            (q5, 0) = (q5, 0, L)
            (q5, 1) = (q5, 1, L)
            (q5, x) = (q5, x, L)
            (q5, y) = (q5, y, L)
            (q5, #) = (q0, #, R)

            (q6, 0) = (q6, 0, L)
            (q6, 1) = (q6, 1, L)
            (q6, x) = (q6, x, L)
            (q6, y) = (q6, y, L)
            (q6, #) = (q7, #, R)

            (q7, 0) = (q8, 1, R)
            (q7, 1) = (q8, 1, R)
            (q7, x) = (q8, 1, R)
            (q7, y) = (q8, 1, R)
            (q7, #) = (qF, 1, S)

            (q8, 0) = (q8, #, R)
            (q8, 1) = (q8, #, R)
            (q8, x) = (q8, #, R)
            (q8, y) = (q8, #, R)
            (q8, #) = (q12, #, L)

            (q12, #) = (q12, #, L)
            (q12, 1) = (qF, 1, S)

            (q9, 0) = (q9, 0, L)
            (q9, 1) = (q9, 1, L)
            (q9, x) = (q9, x, L)
            (q9, y) = (q9, y, L)
            (q9, #) = (q10, #, R)

            (q10, 0) = (q11, 0, R)
            (q10, 1) = (q11, 0, R)
            (q10, x) = (q11, 0, R)
            (q10, y) = (q11, 0, R)
            (q10, #) = (qF, 0, S)

            (q11, 0) = (q11, #, R)
            (q11, 1) = (q11, #, R)
            (q11, x) = (q11, #, R)
            (q11, y) = (q11, #, R)
            (q11, #) = (q13, #, L)

            (q13, #) = (q13, #, L)
            (q13, 0) = (qF, 0, S)

            w = 10101
            """;
    }
}
