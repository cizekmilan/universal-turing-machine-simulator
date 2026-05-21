using System;
using UTMS.Core;
using Xunit;

namespace UTMS.Tests
{
    /// <summary>
    /// Testy dekódování binárního zápisu Turingova stroje.
    /// </summary>
    public class BinaryCodeTest
    {
        /// <summary>
        /// Ověřuje, že platný binární program lze dekódovat na přechody a vstupní slovo.
        /// </summary>
        [Fact]
        public void MakeTextInstructions_DecodesValidBinaryProgram()
        {
            string errorMessage = "";
            string validIncrementProgram = TuringMachineProgramSerializer.ToBinary(CreateIncrementProgram(), "1011");
            BinaryCode code = new BinaryCode(validIncrementProgram);

            var instructions = code.MakeTextInstructions(ref errorMessage);

            Assert.NotNull(instructions);
            Assert.Equal(11, instructions.Count);
            Assert.Equal("1011", code.InputData);

            Assert.Equal("q0", instructions[0].InputState);
            Assert.Equal('1', instructions[0].InputSymbol);
            Assert.Equal("q0", instructions[0].OutputState);
            Assert.Equal('1', instructions[0].OutputSymbol);
            Assert.Equal('R', instructions[0].HeadMove);

            Assert.Equal("q2", instructions[5].InputState);
            Assert.Equal('1', instructions[5].InputSymbol);
            Assert.Equal("q2", instructions[5].OutputState);
            Assert.Equal('1', instructions[5].OutputSymbol);
            Assert.Equal('L', instructions[5].HeadMove);

            Assert.Equal("q3", instructions[10].InputState);
            Assert.Equal('#', instructions[10].InputSymbol);
            Assert.Equal("qF", instructions[10].OutputState);
            Assert.Equal('1', instructions[10].OutputSymbol);
            Assert.Equal('S', instructions[10].HeadMove);
        }

        /// <summary>
        /// Ověřuje, že poškozený nebo neúplný binární zápis skončí chybou místo částečného dekódování.
        /// </summary>
        [Theory]
        [InlineData("11101010")]
        [InlineData("11110100101001001101010101001101000100100010110010100010010110010010000101011000100100100101100010100010101100010001000001000100110000100100001010110000100100010010110000100010000010010001111011")]
        [InlineData("1111010010100100110101010100110100010010001O110010100010010110010010000101011000100100100101100010100010101100010001000001000100110000100100001010110000100100010010110000100010000010010001111011")]
        [InlineData("01010111")]
        [InlineData("1111")]
        public void MakeTextInstructions_RejectsInvalidBinaryProgram(string invalidProgram)
        {
            string errorMessage = "";
            BinaryCode code = new BinaryCode(invalidProgram);

            Assert.Null(code.MakeTextInstructions(ref errorMessage));
            Assert.NotEqual("", errorMessage);
        }

        /// <summary>
        /// Ověřuje, že null hodnota binárního programu vrátí chybu a nevyhodí výjimku.
        /// </summary>
        [Fact]
        public void MakeTextInstructions_RejectsNullBinaryProgramWithoutThrowing()
        {
            string errorMessage = "";
            BinaryCode code = new BinaryCode(null);

            Assert.Null(code.MakeTextInstructions(ref errorMessage));
            Assert.NotEqual("", errorMessage);
        }

        /// <summary>
        /// Vytvoří malý program binárního inkrementu používaný jako platný vstup pro binární enkodér.
        /// </summary>
        private static TransitionFunction[] CreateIncrementProgram()
        {
            return new TransitionFunction[]
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
    }
}
