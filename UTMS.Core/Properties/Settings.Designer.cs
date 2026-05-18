namespace TuringMachineSimulator.Properties
{
    internal sealed partial class Settings
    {
        private static readonly Settings defaultInstance = new Settings();

        public static Settings Default
        {
            get { return defaultInstance; }
        }

        public char MoveLeft
        {
            get { return 'L'; }
        }

        public char MoveRight
        {
            get { return 'R'; }
        }

        public char Stop
        {
            get { return 'S'; }
        }

        public string InitialState
        {
            get { return "q0"; }
        }

        public string EndState
        {
            get { return "qF"; }
        }

        public char BlankSymbol
        {
            get { return '#'; }
        }

        public int StartIndex
        {
            get { return 10; }
        }

        public int TapeLength
        {
            get { return 100; }
        }

        public int MaxSteps
        {
            get { return 1000; }
        }
    }
}
