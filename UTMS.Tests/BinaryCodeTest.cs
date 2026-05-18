using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy dekódování binárního zápisu Turingova stroje.
    /// </summary>
    public class BinaryCodeTest
    {
        private const string ValidIncrementProgram = "1110100101001001101010101001101000100100010110010100010010110010010000101011000100100010010110001010001010110001000100000100010011000010010000101011000010100010010110000100010000010010001111011";

        [Fact]
        public void MakeTextInstructions_DecodesValidBinaryProgram()
        {
            string errorMessage = "";
            BinaryCode code = new BinaryCode(ValidIncrementProgram);

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

        [Theory]
        [InlineData("11101001010010011010101010110000000011101000100100010110010100010010110010010000101011000100100100101100010100010101100010001000001000100110000100100001010110000100100010010110000100010000010010001111011")]
        [InlineData("11101001010010011010101010011010001001000101100101000100101100100100001010110001001001001011000101000101011000100010000010001001100001001000010101100001001000101010110000100010000010010001111011")]
        [InlineData("1110100000101001001101010101001101000100100010110010100010010110010010000101011000100100100101100010100010101100010001000001000100110000100100001010110000100100010010110000100010000010010001111011")]
        [InlineData("111010010100100110101010100110100010010001O110010100010010110010010000101011000100100100101100010100010101100010001000001000100110000100100001010110000100100010010110000100010000010010001111011")]
        [InlineData("01010111")]
        [InlineData("11101010")]
        public void MakeTextInstructions_RejectsInvalidBinaryProgram(string invalidProgram)
        {
            string errorMessage = "";
            BinaryCode code = new BinaryCode(invalidProgram);

            Assert.Null(code.MakeTextInstructions(ref errorMessage));
            Assert.NotEqual("", errorMessage);
        }

        [Fact]
        public void MakeTextInstructions_RejectsNullBinaryProgramWithoutThrowing()
        {
            string errorMessage = "";
            BinaryCode code = new BinaryCode(null);

            Assert.Null(code.MakeTextInstructions(ref errorMessage));
            Assert.NotEqual("", errorMessage);
        }
    }
}
