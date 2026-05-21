using System.Linq;
using UTMS.Core;
using Xunit;

namespace UTMS.Tests
{
    /// <summary>
    /// Testy práce s páskou a detekce jejího přetečení.
    /// </summary>
    public class TapeTest
    {
        /// <summary>
        /// Ověřuje, že nová páska má nastavenou délku, výchozí pozici hlavy a blank symboly.
        /// </summary>
        [Fact]
        public void Constructor_CreatesBlankTapeWithHeadAtConfiguredStart()
        {
            Tape tape = new Tape();

            Assert.Equal(100, tape.Cells.Count);
            Assert.Equal(10, tape.HeadIndex());
            Assert.All(tape.Cells, symbol => Assert.Equal('#', symbol));
            Assert.False(tape.HasOverflowed);
        }

        /// <summary>
        /// Ověřuje, že vstupní slovo se ořízne a zapíše od konfigurované startovní pozice.
        /// </summary>
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

        /// <summary>
        /// Ověřuje, že páska respektuje vlastní blank symbol i po zápisu vstupních dat.
        /// </summary>
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

        /// <summary>
        /// Ověřuje, že opakované nastavení vstupu vyčistí starý obsah pásky a vrátí hlavu na začátek.
        /// </summary>
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

        /// <summary>
        /// Ověřuje, že příliš dlouhé vstupní slovo nastaví příznak přetečení pásky.
        /// </summary>
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

        /// <summary>
        /// Ověřuje posun hlavy doleva a doprava včetně detekce pokusu opustit pásku.
        /// </summary>
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
