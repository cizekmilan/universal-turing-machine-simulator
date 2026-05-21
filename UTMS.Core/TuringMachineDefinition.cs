using System;
using System.Collections.Generic;

namespace UTMS.Core
{
    /// <summary>
    /// Formální definice Turingova stroje včetně vstupu pro simulaci.
    /// </summary>
    public sealed class TuringMachineDefinition
    {
        private static readonly char[] DefaultAlphabet = new char[] { '0', '1' };

        private readonly List<char> alphabet;
        private readonly List<char> tapeAlphabet;
        private readonly List<TransitionFunction> transitions;

        /// <summary>
        /// Vytvoří definici stroje a ověří konzistenci abeced, vstupu a přechodů.
        /// </summary>
        public TuringMachineDefinition(IEnumerable<char> alphabet, IEnumerable<char> tapeAlphabet, char blankSymbol, string inputData, IEnumerable<TransitionFunction> transitions)
        {
            this.alphabet = NormalizeAlphabet(alphabet, "alphabet");
            this.tapeAlphabet = NormalizeAlphabet(tapeAlphabet, "tapeAlphabet");
            BlankSymbol = blankSymbol;
            InputData = inputData ?? "";
            this.transitions = CopyTransitions(transitions);

            Validate();
        }

        /// <summary>
        /// Vstupní abeceda stroje.
        /// </summary>
        public IReadOnlyList<char> Alphabet
        {
            get { return alphabet; }
        }

        /// <summary>
        /// Pásková abeceda stroje.
        /// </summary>
        public IReadOnlyList<char> TapeAlphabet
        {
            get { return tapeAlphabet; }
        }

        /// <summary>
        /// Prázdný symbol pásky.
        /// </summary>
        public char BlankSymbol { get; private set; }

        /// <summary>
        /// Vstupní data stroje.
        /// </summary>
        public string InputData { get; private set; }

        /// <summary>
        /// Přechodové funkce stroje.
        /// </summary>
        public IReadOnlyList<TransitionFunction> Transitions
        {
            get { return transitions; }
        }

        /// <summary>
        /// Vytvoří definici se standardní binární vstupní abecedou a odvozenou páskovou abecedou.
        /// </summary>
        public static TuringMachineDefinition Infer(IEnumerable<TransitionFunction> transitions, string inputData, char blankSymbol)
        {
            List<TransitionFunction> transitionList = CopyTransitions(transitions);
            List<char> inferredAlphabet = new List<char>(DefaultAlphabet);
            if (inputData != null)
                AddDistinct(inferredAlphabet, inputData);

            List<char> inferredTapeAlphabet = new List<char>();
            AddDistinct(inferredTapeAlphabet, inferredAlphabet);
            AddDistinct(inferredTapeAlphabet, blankSymbol);
            foreach (TransitionFunction transition in transitionList)
            {
                AddDistinct(inferredTapeAlphabet, transition.InputSymbol);
                AddDistinct(inferredTapeAlphabet, transition.OutputSymbol);
            }

            return new TuringMachineDefinition(inferredAlphabet, inferredTapeAlphabet, blankSymbol, inputData, transitionList);
        }

        /// <summary>
        /// Dopočítá páskovou abecedu ze vstupní abecedy, blank symbolu a symbolů použitých v přechodech.
        /// </summary>
        public static char[] InferTapeAlphabet(IEnumerable<char> alphabet, char blankSymbol, IEnumerable<TransitionFunction> transitions)
        {
            if (alphabet == null)
                throw new ArgumentNullException(nameof(alphabet));
            if (transitions == null)
                throw new ArgumentNullException(nameof(transitions));

            List<char> inferredTapeAlphabet = new List<char>();
            AddDistinct(inferredTapeAlphabet, alphabet);
            AddDistinct(inferredTapeAlphabet, blankSymbol);

            foreach (TransitionFunction transition in transitions)
            {
                AddDistinct(inferredTapeAlphabet, transition.InputSymbol);
                AddDistinct(inferredTapeAlphabet, transition.OutputSymbol);
            }

            return inferredTapeAlphabet.ToArray();
        }

        /// <summary>
        /// Ověří vztahy mezi abecedami, vstupem, blank symbolem a determinističností přechodů.
        /// </summary>
        private void Validate()
        {
            if (IndexOf(tapeAlphabet, BlankSymbol) < 0)
                throw new ArgumentException("Blank symbol must be part of the tape alphabet.");

            if (IndexOf(alphabet, BlankSymbol) >= 0)
                throw new ArgumentException("Blank symbol cannot be part of the input alphabet.");

            for (int i = 0; i < alphabet.Count; i++)
            {
                if (IndexOf(tapeAlphabet, alphabet[i]) < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" must also be part of the tape alphabet.", alphabet[i]));
            }

            for (int i = 0; i < InputData.Length; i++)
            {
                if (IndexOf(alphabet, InputData[i]) < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the input alphabet.", InputData[i]));
            }

            foreach (TransitionFunction transition in transitions)
            {
                if (IndexOf(tapeAlphabet, transition.InputSymbol) < 0)
                    throw new ArgumentException(string.Format("Input symbol \"{0}\" is not defined in the tape alphabet.", transition.InputSymbol));
                if (IndexOf(tapeAlphabet, transition.OutputSymbol) < 0)
                    throw new ArgumentException(string.Format("Output symbol \"{0}\" is not defined in the tape alphabet.", transition.OutputSymbol));
            }

            // Deterministický stroj smí mít pro dvojici stav + čtený symbol pouze jeden přechod.
            for (int i = 0; i < transitions.Count; i++)
            {
                for (int j = i + 1; j < transitions.Count; j++)
                {
                    if (transitions[i].InputState == transitions[j].InputState && transitions[i].InputSymbol == transitions[j].InputSymbol)
                    {
                        throw new ArgumentException(string.Format(
                            "Transition for state \"{0}\" and input symbol \"{1}\" is defined more than once.",
                            transitions[i].InputState,
                            transitions[i].InputSymbol));
                    }
                }
            }
        }

        /// <summary>
        /// Vytvoří neprázdnou abecedu bez duplicit.
        /// </summary>
        private static List<char> NormalizeAlphabet(IEnumerable<char> values, string argumentName)
        {
            if (values == null)
                throw new ArgumentNullException(argumentName);

            List<char> result = new List<char>();
            AddDistinct(result, values);
            if (result.Count == 0)
                throw new ArgumentException(argumentName + " cannot be empty.");

            return result;
        }

        /// <summary>
        /// Vytvoří oddělenou kopii přechodů, aby definice nebyla navázaná na původní kolekci.
        /// </summary>
        private static List<TransitionFunction> CopyTransitions(IEnumerable<TransitionFunction> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            List<TransitionFunction> result = new List<TransitionFunction>();
            foreach (TransitionFunction transition in values)
            {
                result.Add(new TransitionFunction(transition.InputState, transition.InputSymbol, transition.OutputState, transition.OutputSymbol, transition.HeadMove));
            }

            return result;
        }

        /// <summary>
        /// Přidá do cílového seznamu pouze nové znaky z předané kolekce.
        /// </summary>
        private static void AddDistinct(IList<char> target, IEnumerable<char> values)
        {
            foreach (char value in values)
                AddDistinct(target, value);
        }

        /// <summary>
        /// Přidá znak do seznamu pouze v případě, že v něm ještě není.
        /// </summary>
        private static void AddDistinct(IList<char> target, char value)
        {
            if (IndexOf(target, value) < 0)
                target.Add(value);
        }

        /// <summary>
        /// Najde index znaku v seznamu bez závislosti na LINQ.
        /// </summary>
        private static int IndexOf(IList<char> values, char value)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i] == value)
                    return i;
            }

            return -1;
        }
    }
}
