using System;
using System.Collections.Generic;
using System.IO;

namespace UTMS.Core
{
    /// <summary>
    /// Načítá soubory Turingova stroje do čisté formální definice.
    /// </summary>
    public static class TuringMachineDefinitionLoader
    {
        /// <summary>
        /// Načte definici stroje ze souboru a vyhodí výjimku při chybě.
        /// </summary>
        public static TuringMachineDefinition Load(string fileName)
        {
            string errorMessage;
            TuringMachineDefinition definition = Load(fileName, out errorMessage);
            if (definition == null)
                throw new InvalidOperationException(errorMessage);

            return definition;
        }

        /// <summary>
        /// Načte definici stroje ze souboru a vrátí případnou chybovou zprávu.
        /// </summary>
        public static TuringMachineDefinition Load(string fileName, out string errorMessage)
        {
            return Load(fileName, out errorMessage, null, null);
        }

        /// <summary>
        /// Načte definici stroje ze souboru a předá události simulátoru.
        /// </summary>
        internal static TuringMachineDefinition Load(string fileName, out string errorMessage, SyntaxErrorHandler syntaxError, InputDataLoadedHandler inputDataLoaded)
        {
            errorMessage = "";
            if (string.IsNullOrWhiteSpace(fileName))
            {
                errorMessage = "Nazev souboru je prazdny.";
                return null;
            }

            int lineNumber = 0;
            List<TransitionFunction> transitions = new List<TransitionFunction>();
            List<char> parsedAlphabet = null;
            List<char> parsedTapeAlphabet = null;
            char? parsedBlankSymbol = null;
            string inputData = "";

            try
            {
                SyntaxChecker syntaxChecker = new SyntaxChecker();
                string syntaxErrorDescription = "";
                using (StreamReader reader = File.OpenText(fileName))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        lineNumber++;
                        string trimmedLine = line.Trim();

                        if (trimmedLine == "" || trimmedLine.StartsWith("//"))
                            continue;

                        if (TryParseDefinitionLine(trimmedLine, ref parsedAlphabet, ref parsedTapeAlphabet, ref parsedBlankSymbol, ref errorMessage))
                        {
                            if (errorMessage != "")
                                return null;

                            continue;
                        }

                        if (trimmedLine.StartsWith("111"))
                        {
                            if (!TryParseBinaryLine(trimmedLine, transitions, ref parsedAlphabet, ref parsedTapeAlphabet, ref parsedBlankSymbol, ref inputData, ref errorMessage))
                                return null;

                            if (inputData != "")
                                inputDataLoaded?.Invoke(inputData);

                            continue;
                        }

                        if (!syntaxChecker.IsLineSyntaxValid(trimmedLine, ref syntaxErrorDescription))
                        {
                            syntaxError?.Invoke(syntaxErrorDescription, line);
                            errorMessage = "Objevila se syntakticka chyba. Program nemuze pokracovat.";
                            return null;
                        }

                        TransitionFunction transition;
                        string parsedInputData;
                        ParseLine(trimmedLine, out transition, out parsedInputData);
                        if (transition != null)
                            transitions.Add(transition);
                        else
                        {
                            inputData = parsedInputData;
                            inputDataLoaded?.Invoke(inputData);
                        }
                    }
                }

                if (transitions.Count == 0)
                {
                    errorMessage = "Program neobsahuje zadne prikazy.";
                    return null;
                }

                return CreateDefinition(parsedAlphabet, parsedTapeAlphabet, parsedBlankSymbol, inputData, transitions, ref errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = string.Format("Chyba na radku {0} ve vstupnim souboru {1}. Popis chyby: {2}. Vnitrni vyjimka: {3}.", lineNumber, fileName, ex.Message, ex.InnerException);
                return null;
            }
        }

        /// <summary>
        /// Načte jeden binárně zakódovaný řádek včetně metadat a přidá jeho přechody do výsledku.
        /// </summary>
        private static bool TryParseBinaryLine(string line, List<TransitionFunction> transitions, ref List<char> parsedAlphabet, ref List<char> parsedTapeAlphabet, ref char? parsedBlankSymbol, ref string inputData, ref string errorMessage)
        {
            BinaryCode binaryCode = new BinaryCode(line);
            List<TransitionFunction> binaryTransitions = binaryCode.MakeTextInstructions(ref errorMessage);
            if (binaryTransitions == null)
                return false;

            parsedAlphabet = new List<char>(binaryCode.Alphabet);
            parsedTapeAlphabet = new List<char>(binaryCode.TapeAlphabet);
            parsedBlankSymbol = binaryCode.BlankSymbol;
            inputData = binaryCode.InputData;
            transitions.AddRange(binaryTransitions);
            return true;
        }

        /// <summary>
        /// Rozpozná řádek se vstupním slovem nebo řádek s přechodovou funkcí.
        /// </summary>
        private static void ParseLine(string line, out TransitionFunction transition, out string inputData)
        {
            string normalized = line.Trim().Replace(" ", "");
            if (normalized.StartsWith("w="))
            {
                inputData = SyntaxChecker.GetToken(normalized, '=', 2);
                transition = null;
                return;
            }

            inputData = "";
            string firstTupleExpression = SyntaxChecker.GetToken(normalized, '=', 1);
            string secondTupleExpression = SyntaxChecker.GetToken(normalized, '=', 2);
            string firstTuple = GetTupleContent(firstTupleExpression);
            string secondTuple = GetTupleContent(secondTupleExpression);

            transition = new TransitionFunction(
                SyntaxChecker.GetToken(firstTuple, ',', 1),
                SyntaxChecker.GetToken(firstTuple, ',', 2)[0],
                SyntaxChecker.GetToken(secondTuple, ',', 1),
                SyntaxChecker.GetToken(secondTuple, ',', 2)[0],
                SyntaxChecker.GetToken(secondTuple, ',', 3)[0]);
        }

        /// <summary>
        /// Rozpozná metadata textového formátu: vstupní abecedu, páskovou abecedu a blank symbol.
        /// </summary>
        private static bool TryParseDefinitionLine(string line, ref List<char> parsedAlphabet, ref List<char> parsedTapeAlphabet, ref char? parsedBlankSymbol, ref string errorMessage)
        {
            string normalized = line.Trim().Replace(" ", "");
            if (normalized.StartsWith("alphabet="))
            {
                parsedAlphabet = ParseAlphabetValue(SyntaxChecker.GetToken(normalized, '=', 2), "vstupni abeceda", ref errorMessage);
                return true;
            }

            if (normalized.StartsWith("tapeAlphabet="))
            {
                parsedTapeAlphabet = ParseAlphabetValue(SyntaxChecker.GetToken(normalized, '=', 2), "paskova abeceda", ref errorMessage);
                return true;
            }

            if (normalized.StartsWith("blank="))
            {
                string value = SyntaxChecker.GetToken(normalized, '=', 2);
                if (value.Length != 1)
                {
                    errorMessage = "Blank symbol musi byt zapsan jako jediny znak.";
                    return true;
                }

                parsedBlankSymbol = value[0];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sestaví formální definici stroje z načtených metadat a přechodů.
        /// </summary>
        private static TuringMachineDefinition CreateDefinition(List<char> parsedAlphabet, List<char> parsedTapeAlphabet, char? parsedBlankSymbol, string inputData, List<TransitionFunction> transitions, ref string errorMessage)
        {
            char blankSymbol = parsedBlankSymbol.HasValue ? parsedBlankSymbol.Value : Tape.DefaultBlankSymbol;

            List<char> inputAlphabet = parsedAlphabet ?? new List<char>(new char[] { '0', '1' });
            List<char> fullTapeAlphabet = parsedTapeAlphabet ?? InferTapeAlphabet(inputAlphabet, blankSymbol, transitions);

            if (!fullTapeAlphabet.Contains(blankSymbol))
            {
                errorMessage = "Blank symbol musi byt soucasti paskove abecedy.";
                return null;
            }

            if (inputAlphabet.Contains(blankSymbol))
            {
                errorMessage = "Blank symbol nesmi byt soucasti vstupni abecedy.";
                return null;
            }

            for (int i = 0; i < inputAlphabet.Count; i++)
            {
                if (!fullTapeAlphabet.Contains(inputAlphabet[i]))
                {
                    errorMessage = "Symbol \"" + inputAlphabet[i] + "\" ze vstupni abecedy chybi v paskove abecede.";
                    return null;
                }
            }

            for (int i = 0; i < inputData.Length; i++)
            {
                if (!inputAlphabet.Contains(inputData[i]))
                {
                    errorMessage = "Vstupni data obsahuji symbol \"" + inputData[i] + "\", ktery neni ve vstupni abecede.";
                    return null;
                }
            }

            foreach (TransitionFunction transition in transitions)
            {
                if (!fullTapeAlphabet.Contains(transition.InputSymbol) || !fullTapeAlphabet.Contains(transition.OutputSymbol))
                {
                    errorMessage = "Prechodove funkce obsahuji symbol mimo paskovou abecedu.";
                    return null;
                }
            }

            return new TuringMachineDefinition(inputAlphabet, fullTapeAlphabet, blankSymbol, inputData, transitions);
        }

        /// <summary>
        /// Převede hodnotu ve tvaru {a,b,c} na seznam znaků a ověří duplicity.
        /// </summary>
        private static List<char> ParseAlphabetValue(string value, string name, ref string errorMessage)
        {
            if (!value.StartsWith("{") || !value.EndsWith("}"))
            {
                errorMessage = "Definice " + name + " musi byt ve tvaru {a,b,c}.";
                return null;
            }

            string content = value.Substring(1, value.Length - 2);
            if (content.Length == 0)
            {
                errorMessage = "Definice " + name + " nesmi byt prazdna.";
                return null;
            }

            List<char> alphabet = new List<char>();
            string[] symbols = content.Split(',');
            for (int i = 0; i < symbols.Length; i++)
            {
                if (symbols[i].Length != 1)
                {
                    errorMessage = "Kazdy symbol v definici " + name + " musi byt jediny znak.";
                    return null;
                }

                if (alphabet.Contains(symbols[i][0]))
                {
                    errorMessage = "Symbol \"" + symbols[i][0] + "\" je v definici " + name + " uveden vicekrat.";
                    return null;
                }

                alphabet.Add(symbols[i][0]);
            }

            return alphabet;
        }

        /// <summary>
        /// Dopočítá páskovou abecedu z input abecedy, blank symbolu a symbolů použitých v přechodech.
        /// </summary>
        private static List<char> InferTapeAlphabet(IEnumerable<char> inputAlphabet, char blankSymbol, IEnumerable<TransitionFunction> transitions)
        {
            List<char> fullTapeAlphabet = new List<char>();
            AddDistinct(fullTapeAlphabet, inputAlphabet);
            AddDistinct(fullTapeAlphabet, blankSymbol);

            foreach (TransitionFunction transition in transitions)
            {
                AddDistinct(fullTapeAlphabet, transition.InputSymbol);
                AddDistinct(fullTapeAlphabet, transition.OutputSymbol);
            }

            return fullTapeAlphabet;
        }

        /// <summary>
        /// Vrátí obsah první dvojice závorek v přechodové části řádku.
        /// </summary>
        private static string GetTupleContent(string expression)
        {
            int openParen = expression.IndexOf('(');
            int closeParen = expression.IndexOf(')');
            return expression.Substring(openParen + 1, closeParen - openParen - 1);
        }

        /// <summary>
        /// Přidá do cílového seznamu pouze znaky, které v něm dosud nejsou.
        /// </summary>
        private static void AddDistinct(IList<char> target, IEnumerable<char> values)
        {
            foreach (char value in values)
                AddDistinct(target, value);
        }

        /// <summary>
        /// Přidá znak do cílového seznamu bez vytvoření duplicity.
        /// </summary>
        private static void AddDistinct(IList<char> target, char value)
        {
            if (!target.Contains(value))
                target.Add(value);
        }
    }
}
