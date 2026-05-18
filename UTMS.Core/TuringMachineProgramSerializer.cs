using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace TuringMachineSimulator
{
    /// <summary>
    /// Ukládá programy Turingova stroje do textového a binárního formátu.
    /// </summary>
    public static class TuringMachineProgramSerializer
    {
        private static readonly Regex NumberedState = new Regex("^q([0-9]+)$", RegexOptions.Compiled);
        private static readonly char[] DefaultAlphabet = new char[] { '0', '1' };
        private static readonly char[] DefaultTapeAlphabet = new char[] { '0', '1', '#' };

        /// <summary>
        /// Vytvoří textovou reprezentaci programu ve formátu .tm.
        /// </summary>
        public static string ToText(TuringMachineDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return ToText(definition.Transitions, definition.InputData, definition.Alphabet, definition.TapeAlphabet, definition.BlankSymbol);
        }

        /// <summary>
        /// Vytvoří textovou reprezentaci programu ve formátu .tm.
        /// </summary>
        public static string ToText(IEnumerable<TransitionFunction> program, string inputData)
        {
            return ToText(program, inputData, DefaultAlphabet, DefaultTapeAlphabet, Tape.DefaultBlankSymbol);
        }

        /// <summary>
        /// Vytvoří textovou reprezentaci programu včetně definice abeced.
        /// </summary>
        public static string ToText(IEnumerable<TransitionFunction> program, string inputData, IEnumerable<char> alphabet, IEnumerable<char> tapeAlphabet, char blankSymbol)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            List<TransitionFunction> transitions = new List<TransitionFunction>(program);
            List<char> alphabetList = NormalizeAlphabet(alphabet, "alphabet");
            List<char> tapeAlphabetList = NormalizeAlphabet(tapeAlphabet, "tapeAlphabet");
            ValidateDefinition(alphabetList, tapeAlphabetList, blankSymbol, inputData);

            StringBuilder sb = new StringBuilder();
            sb.Append("alphabet = ");
            AppendAlphabet(sb, alphabetList);
            sb.AppendLine();
            sb.Append("tapeAlphabet = ");
            AppendAlphabet(sb, tapeAlphabetList);
            sb.AppendLine();
            sb.Append("blank = ");
            sb.Append(blankSymbol);
            sb.AppendLine();
            sb.AppendLine();

            foreach (TransitionFunction transition in transitions)
            {
                ValidateTransitionSymbols(transition, tapeAlphabetList);
                sb.AppendFormat("({0}, {1}) = ({2}, {3}, {4})", transition.InputState, transition.InputSymbol, transition.OutputState, transition.OutputSymbol, transition.HeadMove);
                sb.AppendLine();
            }

            if (inputData != null)
            {
                sb.Append("w = ");
                sb.Append(inputData);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Uloží textovou reprezentaci programu do souboru.
        /// </summary>
        public static void SaveText(string fileName, TuringMachineDefinition definition)
        {
            File.WriteAllText(fileName, ToText(definition));
        }

        /// <summary>
        /// Uloží textovou reprezentaci programu do souboru.
        /// </summary>
        public static void SaveText(string fileName, IEnumerable<TransitionFunction> program, string inputData)
        {
            File.WriteAllText(fileName, ToText(program, inputData));
        }

        /// <summary>
        /// Uloží textovou reprezentaci programu včetně definice abeced.
        /// </summary>
        public static void SaveText(string fileName, IEnumerable<TransitionFunction> program, string inputData, IEnumerable<char> alphabet, IEnumerable<char> tapeAlphabet, char blankSymbol)
        {
            File.WriteAllText(fileName, ToText(program, inputData, alphabet, tapeAlphabet, blankSymbol));
        }

        /// <summary>
        /// Vytvoří binární reprezentaci programu ve formátu .btm verze 2.
        /// </summary>
        public static string ToBinary(TuringMachineDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            return ToBinary(definition.Transitions, definition.InputData, definition.Alphabet, definition.TapeAlphabet, definition.BlankSymbol);
        }

        /// <summary>
        /// Vytvoří binární reprezentaci programu ve formátu .btm verze 2.
        /// </summary>
        public static string ToBinary(IEnumerable<TransitionFunction> program, string inputData)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            List<TransitionFunction> transitions = new List<TransitionFunction>(program);
            List<char> alphabet = BuildAlphabet(inputData);
            List<char> tapeAlphabet = BuildTapeAlphabet(transitions, alphabet, Tape.DefaultBlankSymbol);
            return ToBinary(transitions, inputData, alphabet, tapeAlphabet, Tape.DefaultBlankSymbol);
        }

        /// <summary>
        /// Vytvoří binární reprezentaci programu se zadanou definicí abeced.
        /// </summary>
        public static string ToBinary(IEnumerable<TransitionFunction> program, string inputData, IEnumerable<char> alphabet, IEnumerable<char> tapeAlphabet, char blankSymbol)
        {
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            List<TransitionFunction> transitions = new List<TransitionFunction>(program);
            List<char> alphabetList = NormalizeAlphabet(alphabet, "alphabet");
            List<char> tapeAlphabetList = NormalizeAlphabet(tapeAlphabet, "tapeAlphabet");
            ValidateDefinition(alphabetList, tapeAlphabetList, blankSymbol, inputData);

            int finalStateLength = GetFinalStateLength(transitions);
            List<string> encodedTransitions = new List<string>();

            foreach (TransitionFunction transition in transitions)
            {
                ValidateTransitionSymbols(transition, tapeAlphabetList);
                encodedTransitions.Add(
                    EncodeState(transition.InputState, finalStateLength) + "1" +
                    EncodeSymbol(transition.InputSymbol, tapeAlphabetList) + "1" +
                    EncodeState(transition.OutputState, finalStateLength) + "1" +
                    EncodeSymbol(transition.OutputSymbol, tapeAlphabetList) + "1" +
                    EncodeMove(transition.HeadMove));
            }

            string data = inputData ?? "";
            return "1111" +
                EncodeCharacterSet(alphabetList) + "111" +
                EncodeCharacterSet(tapeAlphabetList) + "111" +
                EncodeCharacter(blankSymbol) + "1111" +
                string.Join("11", encodedTransitions) + "111" +
                EncodeInputData(data, alphabetList);
        }

        /// <summary>
        /// Uloží binární reprezentaci programu do souboru.
        /// </summary>
        public static void SaveBinary(string fileName, TuringMachineDefinition definition)
        {
            File.WriteAllText(fileName, ToBinary(definition));
        }

        /// <summary>
        /// Uloží binární reprezentaci programu do souboru.
        /// </summary>
        public static void SaveBinary(string fileName, IEnumerable<TransitionFunction> program, string inputData)
        {
            File.WriteAllText(fileName, ToBinary(program, inputData));
        }

        /// <summary>
        /// Uloží binární reprezentaci programu se zadanou definicí abeced.
        /// </summary>
        public static void SaveBinary(string fileName, IEnumerable<TransitionFunction> program, string inputData, IEnumerable<char> alphabet, IEnumerable<char> tapeAlphabet, char blankSymbol)
        {
            File.WriteAllText(fileName, ToBinary(program, inputData, alphabet, tapeAlphabet, blankSymbol));
        }

        private static void AppendAlphabet(StringBuilder sb, IEnumerable<char> alphabet)
        {
            sb.Append("{");
            bool first = true;
            foreach (char symbol in alphabet)
            {
                if (!first)
                    sb.Append(",");

                sb.Append(symbol);
                first = false;
            }

            sb.Append("}");
        }

        private static int GetFinalStateLength(IEnumerable<TransitionFunction> transitions)
        {
            int maxStateIndex = 0;
            foreach (TransitionFunction transition in transitions)
            {
                maxStateIndex = Math.Max(maxStateIndex, GetNumberedStateIndex(transition.InputState));
                maxStateIndex = Math.Max(maxStateIndex, GetNumberedStateIndex(transition.OutputState));
            }

            return maxStateIndex + 2;
        }

        private static int GetNumberedStateIndex(string state)
        {
            if (state == TransitionFunction.FinalStateName)
                return 0;

            Match match = NumberedState.Match(state);
            if (!match.Success)
                throw new ArgumentException(string.Format("State \"{0}\" cannot be written to the binary format. Supported states are q0, q1, ... and {1}.", state, TransitionFunction.FinalStateName));

            return int.Parse(match.Groups[1].Value);
        }

        private static string EncodeState(string state, int finalStateLength)
        {
            if (state == TransitionFunction.FinalStateName)
                return new string('0', finalStateLength);

            int stateIndex = GetNumberedStateIndex(state);
            return new string('0', stateIndex + 1);
        }

        private static string EncodeSymbol(char symbol, IList<char> tapeAlphabet)
        {
            int index = IndexOf(tapeAlphabet, symbol);
            if (index >= 0)
                return new string('0', index + 1);

            throw new ArgumentException(string.Format("Symbol \"{0}\" is not defined in the tape alphabet.", symbol));
        }

        private static string EncodeMove(char move)
        {
            if (move == TuringMachine.MoveLeftSymbol)
                return "0";
            if (move == TuringMachine.MoveRightSymbol)
                return "00";
            if (move == TuringMachine.StopSymbol)
                return "000";

            throw new ArgumentException(string.Format("Head move \"{0}\" cannot be written to the binary format. Supported moves are {1}, {2} and {3}.", move, TuringMachine.MoveLeftSymbol, TuringMachine.MoveRightSymbol, TuringMachine.StopSymbol));
        }

        private static string EncodeCharacterSet(IEnumerable<char> alphabet)
        {
            List<string> encodedSymbols = new List<string>();
            foreach (char symbol in alphabet)
                encodedSymbols.Add(EncodeCharacter(symbol));

            return string.Join("11", encodedSymbols);
        }

        private static string EncodeCharacter(char symbol)
        {
            return new string('0', symbol + 1);
        }

        private static string EncodeInputData(string inputData, IList<char> alphabet)
        {
            if (string.IsNullOrEmpty(inputData))
                return "";

            List<string> encodedSymbols = new List<string>();
            for (int i = 0; i < inputData.Length; i++)
            {
                int index = IndexOf(alphabet, inputData[i]);
                if (index < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the input alphabet.", inputData[i]));

                encodedSymbols.Add(new string('0', index + 1));
            }

            return string.Join("1", encodedSymbols);
        }

        private static List<char> BuildAlphabet(string inputData)
        {
            List<char> alphabet = new List<char>(DefaultAlphabet);
            if (inputData != null)
                AddDistinct(alphabet, inputData);

            return alphabet;
        }

        private static List<char> BuildTapeAlphabet(IEnumerable<TransitionFunction> transitions, IEnumerable<char> alphabet, char blankSymbol)
        {
            List<char> tapeAlphabet = new List<char>();
            AddDistinct(tapeAlphabet, alphabet);
            AddDistinct(tapeAlphabet, blankSymbol);

            foreach (TransitionFunction transition in transitions)
            {
                AddDistinct(tapeAlphabet, transition.InputSymbol);
                AddDistinct(tapeAlphabet, transition.OutputSymbol);
            }

            return tapeAlphabet;
        }

        private static List<char> NormalizeAlphabet(IEnumerable<char> alphabet, string argumentName)
        {
            if (alphabet == null)
                throw new ArgumentNullException(argumentName);

            List<char> normalized = new List<char>();
            AddDistinct(normalized, alphabet);
            if (normalized.Count == 0)
                throw new ArgumentException(argumentName + " cannot be empty.");

            return normalized;
        }

        private static void ValidateDefinition(IList<char> alphabet, IList<char> tapeAlphabet, char blankSymbol, string inputData)
        {
            if (IndexOf(tapeAlphabet, blankSymbol) < 0)
                throw new ArgumentException("Blank symbol must be part of the tape alphabet.");

            if (IndexOf(alphabet, blankSymbol) >= 0)
                throw new ArgumentException("Blank symbol cannot be part of the input alphabet.");

            for (int i = 0; i < alphabet.Count; i++)
            {
                if (IndexOf(tapeAlphabet, alphabet[i]) < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" must also be part of the tape alphabet.", alphabet[i]));
            }

            if (inputData == null)
                return;

            for (int i = 0; i < inputData.Length; i++)
            {
                if (IndexOf(alphabet, inputData[i]) < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the input alphabet.", inputData[i]));
            }
        }

        private static void ValidateTransitionSymbols(TransitionFunction transition, IList<char> tapeAlphabet)
        {
            if (IndexOf(tapeAlphabet, transition.InputSymbol) < 0)
                throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the tape alphabet.", transition.InputSymbol));
            if (IndexOf(tapeAlphabet, transition.OutputSymbol) < 0)
                throw new ArgumentException(string.Format("Output symbol \"{0}\" is not defined in the tape alphabet.", transition.OutputSymbol));
        }

        private static int IndexOf(IList<char> values, char value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == value)
                    return i;
            }

            return -1;
        }

        private static void AddDistinct(IList<char> target, IEnumerable<char> values)
        {
            foreach (char value in values)
                AddDistinct(target, value);
        }

        private static void AddDistinct(IList<char> target, char value)
        {
            if (IndexOf(target, value) < 0)
                target.Add(value);
        }
    }
}
