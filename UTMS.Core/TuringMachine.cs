using System.Collections.Generic;

namespace UTMS.Core
{
    /// <summary>
    /// Konečná páska používaná simulátorem.
    /// </summary>
    public class Tape
    {
        /// <summary>
        /// Délka pásky podle nastavení simulátoru.
        /// </summary>
        public int TapeLength { get; private set; }

        private readonly List<char> cells;
        private int headIndex;
        private int inputPosition;
        private bool hasOverflowed;

        /// <summary>
        /// Vytvoří prázdnou pásku s hlavou na výchozí pozici.
        /// </summary>
        public Tape()
            : this(Properties.Settings.Default.BlankSymbol)
        {
        }

        /// <summary>
        /// Vytvoří prázdnou pásku s určeným prázdným symbolem.
        /// </summary>
        public Tape(char blankSymbol)
        {
            TapeLength = Properties.Settings.Default.TapeLength;
            BlankSymbol = blankSymbol;
            cells = new List<char>(TapeLength + 10);
            for (int i = 0; i < TapeLength; i++)
                cells.Add(BlankSymbol);

            Reset();
        }

        /// <summary>
        /// Výchozí prázdný symbol používaný při ručním vytvoření pásky.
        /// </summary>
        public static char DefaultBlankSymbol
        {
            get { return Properties.Settings.Default.BlankSymbol; }
        }

        /// <summary>
        /// Symbol prázdného políčka této pásky.
        /// </summary>
        public char BlankSymbol { get; private set; }

        /// <summary>
        /// Aktuální obsah pásky.
        /// </summary>
        public IReadOnlyList<char> Cells
        {
            get { return cells; }
        }

        /// <summary>
        /// Určuje, zda se hlava nebo vstupní data pokusily překročit hranici pásky.
        /// </summary>
        public bool HasOverflowed
        {
            get { return hasOverflowed; }
            set { hasOverflowed = value; }
        }

        /// <summary>
        /// Zapíše další vstupní symbol na pásku od výchozí vstupní pozice.
        /// </summary>
        public void WriteInputSymbol(char symbol)
        {
            if (inputPosition < 0 || inputPosition >= TapeLength)
            {
                HasOverflowed = true;
                return;
            }

            cells[inputPosition] = symbol;
            inputPosition++;
        }

        /// <summary>
        /// Vymaže pásku a zapíše na ni vstupní řetězec.
        /// </summary>
        public void SetData(string input)
        {
            Reset();

            string data = input == null ? "" : input.Trim();
            foreach (char symbol in data.ToCharArray())
                WriteInputSymbol(symbol);
        }

        /// <summary>
        /// Vrátí symbol pod hlavou.
        /// </summary>
        public char ReadSymbol()
        {
            if (headIndex < 0 || headIndex >= TapeLength)
            {
                HasOverflowed = true;
                return BlankSymbol;
            }

            return cells[headIndex];
        }

        /// <summary>
        /// Zapíše symbol na aktuální pozici hlavy.
        /// </summary>
        public void WriteSymbol(char symbol)
        {
            if (headIndex < 0 || headIndex >= TapeLength)
            {
                HasOverflowed = true;
                return;
            }

            cells[headIndex] = symbol;
        }

        /// <summary>
        /// Posune hlavu o jedno políčko doleva.
        /// </summary>
        public void MoveLeft()
        {
            if (headIndex <= 0)
            {
                HasOverflowed = true;
                return;
            }

            headIndex--;
        }

        /// <summary>
        /// Nechá hlavu na aktuální pozici.
        /// </summary>
        public void Stay()
        {
        }

        /// <summary>
        /// Posune hlavu o jedno políčko doprava.
        /// </summary>
        public void MoveRight()
        {
            if (headIndex >= TapeLength - 1)
            {
                HasOverflowed = true;
                return;
            }

            headIndex++;
        }

        /// <summary>
        /// Vrátí aktuální index hlavy.
        /// </summary>
        public int HeadIndex()
        {
            return headIndex;
        }

        /// <summary>
        /// Vyčistí obsah pásky, vrátí hlavu na výchozí pozici a zruší příznak přetečení.
        /// </summary>
        private void Reset()
        {
            for (int i = 0; i < TapeLength; i++)
                cells[i] = BlankSymbol;

            headIndex = Properties.Settings.Default.StartIndex;
            inputPosition = headIndex;
            HasOverflowed = false;
        }
    }

    /// <summary>
    /// Turingův stroj s aktuálním stavem a páskou.
    /// </summary>
    public class TuringMachine : Tape
    {
        /// <summary>
        /// Znak přechodu pro pohyb hlavy doleva.
        /// </summary>
        public static char MoveLeftSymbol { get; private set; }

        /// <summary>
        /// Znak přechodu pro pohyb hlavy doprava.
        /// </summary>
        public static char MoveRightSymbol { get; private set; }

        /// <summary>
        /// Znak přechodu pro zastavení hlavy na místě.
        /// </summary>
        public static char StopSymbol { get; private set; }

        /// <summary>
        /// Maximální počet kroků simulace.
        /// </summary>
        public static int MaxSteps { get; private set; }

        private string currentState;

        /// <summary>
        /// Načte symboly pohybů a limit kroků z nastavení aplikace.
        /// </summary>
        static TuringMachine()
        {
            MoveLeftSymbol = Properties.Settings.Default.MoveLeft;
            MoveRightSymbol = Properties.Settings.Default.MoveRight;
            StopSymbol = Properties.Settings.Default.Stop;
            MaxSteps = Properties.Settings.Default.MaxSteps;
        }

        /// <summary>
        /// Vytvoří stroj ve výchozím počátečním stavu.
        /// </summary>
        public TuringMachine()
            : this(Tape.DefaultBlankSymbol)
        {
        }

        /// <summary>
        /// Vytvoří stroj s určeným prázdným symbolem pásky.
        /// </summary>
        public TuringMachine(char blankSymbol)
            : base(blankSymbol)
        {
            currentState = TransitionFunction.InitialStateName;
        }

        /// <summary>
        /// Vrátí aktuální stav stroje.
        /// </summary>
        public string CurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Nastaví aktuální stav stroje.
        /// </summary>
        public void SetCurrentState(string state)
        {
            currentState = state;
        }

        /// <summary>
        /// Určuje, zda je stroj v koncovém stavu.
        /// </summary>
        public bool IsInFinalState()
        {
            return currentState == TransitionFunction.FinalStateName;
        }
    }
}
