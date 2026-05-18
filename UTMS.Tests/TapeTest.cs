using System.Linq;
using TuringMachineSimulator;
using Xunit;

namespace UTMS.Test
{
    /// <summary>
    /// Testy práce s páskou a detekce jejího přetečení.
    /// </summary>
    public class TapeTest
    {
        [Fact]
        public void Constructor_CreatesBlankTapeWithHeadAtConfiguredStart()
        {
            Tape tape = new Tape();

            Assert.Equal(100, tape.Cells.Count);
            Assert.Equal(10, tape.HeadIndex());
            Assert.All(tape.Cells, symbol => Assert.Equal('#', symbol));
            Assert.False(tape.HasOverflowed);
        }

        [Fact]
        public void SetData_TrimsInputAndWritesItFromStartPosition()
        {
            Tape tape = new Tape();

            tape.SetData(" 101 ");

            Assert.Equal('1', tape.Cells[10]);
            Assert.Equal('0', tape.Cells[11]);
            Assert.Equal('1', tape.Cells[12]);
            Assert.True(tape.Cells.Take(10).All(symbol => symbol == '#'));
        }

        [Fact]
        public void Constructor_UsesConfiguredBlankSymbol()
        {
            Tape tape = new Tape('_');

            Assert.Equal('_', tape.BlankSymbol);
            Assert.All(tape.Cells, symbol => Assert.Equal('_', symbol));

            tape.SetData("1");

            Assert.Equal('1', tape.Cells[10]);
            Assert.Equal('_', tape.Cells[11]);
        }

        [Fact]
        public void SetData_ClearsPreviousTapeContents()
        {
            Tape tape = new Tape();

            tape.SetData("101");
            tape.MoveRight();
            tape.SetData("0");

            Assert.Equal('0', tape.Cells[10]);
            Assert.Equal('#', tape.Cells[11]);
            Assert.Equal('#', tape.Cells[12]);
            Assert.Equal(10, tape.HeadIndex());
            Assert.False(tape.HasOverflowed);
        }

        [Fact]
        public void SetData_MarksOverflowWhenInputDoesNotFitOnTape()
        {
            Tape tape = new Tape();
            string longInput = new string('1', 91);

            tape.SetData(longInput);

            Assert.True(tape.HasOverflowed);
            Assert.Equal('1', tape.Cells[99]);
            Assert.Equal(10, tape.HeadIndex());
        }

        [Fact]
        public void HeadMovement_UpdatesIndexAndDetectsTapeOverflow()
        {
            Tape tape = new Tape();

            tape.MoveRight();
            Assert.Equal(11, tape.HeadIndex());

            tape.MoveLeft();
            Assert.Equal(10, tape.HeadIndex());

            for (int i = 0; i < 11; i++)
            {
                tape.MoveLeft();
            }

            Assert.True(tape.HasOverflowed);
        }
    }
}
