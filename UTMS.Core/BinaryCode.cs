using System;
using System.Collections.Generic;

namespace UTMS.Core
{
    /// <summary>
    /// Zpracovává binárně zakódovaný program Turingova stroje.
    /// </summary>
    public class BinaryCode
    {
        private readonly string binaryCode;
        private List<char> alphabet;
        private List<char> tapeAlphabet;
        private char blankSymbol;

        /// <summary>
        /// Připraví binární kód pro dekódování.
        /// </summary>
        public BinaryCode(string binaryCode)
        {
            if (binaryCode == null)
                binaryCode = "";

            this.binaryCode = binaryCode.Trim();
            alphabet = new List<char>(new char[] { '0', '1' });
            tapeAlphabet = new List<char>(new char[] { '0', '1', Properties.Settings.Default.BlankSymbol });
            blankSymbol = Properties.Settings.Default.BlankSymbol;
            InputData = "";
        }

        /// <summary>
        /// Vstupní abeceda načtená z binárního programu.
        /// </summary>
        public IReadOnlyList<char> Alphabet
        {
            get { return alphabet; }
        }

        /// <summary>
        /// Pásková abeceda načtená z binárního programu.
        /// </summary>
        public IReadOnlyList<char> TapeAlphabet
        {
            get { return tapeAlphabet; }
        }

        /// <summary>
        /// Prázdný symbol načtený z binárního programu.
        /// </summary>
        public char BlankSymbol
        {
            get { return blankSymbol; }
        }

        /// <summary>
        /// Vstupní data uložená za binárním programem.
        /// </summary>
        public string InputData { get; private set; }

        /// <summary>
        /// Dekóduje binární instrukce do textových přechodových funkcí.
        /// </summary>
        public List<TransitionFunction> MakeTextInstructions(ref string errorMessage)
        {
            string instructionBlock;
            if (!ParseVersion2Code(ref errorMessage, out instructionBlock))
                return null;

            List<TransitionFunction> transitions = DecodeInstructionBlock(instructionBlock, tapeAlphabet, ref errorMessage);
            if (transitions == null)
                return null;

            DecodeStateNames(transitions);
            return transitions;
        }

        /// <summary>
        /// Rozdělí binární program na metadata, blok instrukcí a vstupní data.
        /// </summary>
        private bool ParseVersion2Code(ref string errorMessage, out string instructionBlock)
        {
            instructionBlock = "";
            if (!ValidateBinaryAlphabet(ref errorMessage))
                return false;

            if (!binaryCode.StartsWith("1111"))
            {
                errorMessage = "Binarni kod musi zacinat prefixem verze 2: 1111.";
                return false;
            }

            string withoutPrefix = binaryCode.Substring(4);
            int metadataEnd = withoutPrefix.IndexOf("1111", StringComparison.Ordinal);
            if (metadataEnd <= 0)
            {
                errorMessage = "Binarni kod verze 2 musi obsahovat oddelovac metadat 1111.";
                return false;
            }

            string metadataBlock = withoutPrefix.Substring(0, metadataEnd);
            string machineBlock = withoutPrefix.Substring(metadataEnd + 4);
            string[] metadataParts = Split(metadataBlock, "111");
            if (metadataParts.Length != 3)
            {
                errorMessage = "Metadata binarniho kodu musi obsahovat vstupni abecedu, paskovou abecedu a blank symbol.";
                return false;
            }

            alphabet = DecodeCharacterSet(metadataParts[0], ref errorMessage);
            if (alphabet == null)
                return false;

            tapeAlphabet = DecodeCharacterSet(metadataParts[1], ref errorMessage);
            if (tapeAlphabet == null)
                return false;

            char? decodedBlankSymbol = DecodeCharacter(metadataParts[2], ref errorMessage);
            if (!decodedBlankSymbol.HasValue)
                return false;

            blankSymbol = decodedBlankSymbol.Value;
            if (!ValidateDefinition(ref errorMessage))
                return false;

            int instructionEnd = machineBlock.IndexOf("111", StringComparison.Ordinal);
            if (instructionEnd < 0)
            {
                errorMessage = "Binarni kod verze 2 musi obsahovat oddelovac instrukci a vstupu 111.";
                return false;
            }

            instructionBlock = machineBlock.Substring(0, instructionEnd);
            string inputBlock = machineBlock.Substring(instructionEnd + 3);
            InputData = DecodeInputData(inputBlock, ref errorMessage);
            return InputData != null;
        }

        /// <summary>
        /// Dekóduje blok přechodových funkcí oddělených dvojicí jedniček.
        /// </summary>
        private List<TransitionFunction> DecodeInstructionBlock(string instructionBlock, IList<char> symbols, ref string errorMessage)
        {
            if (string.IsNullOrEmpty(instructionBlock))
            {
                errorMessage = "Blok instrukci nesmi byt prazdny.";
                return null;
            }

            List<TransitionFunction> transitions = new List<TransitionFunction>();
            string[] instructions = Split(instructionBlock, "11");
            for (int i = 0; i < instructions.Length; i++)
            {
                TransitionFunction transition = DecodeInstruction(instructions[i], symbols, ref errorMessage);
                if (transition == null)
                    return null;

                transitions.Add(transition);
            }

            return transitions;
        }

        /// <summary>
        /// Dekóduje jednu binární instrukci do přechodové funkce.
        /// </summary>
        private TransitionFunction DecodeInstruction(string instruction, IList<char> symbols, ref string errorMessage)
        {
            string[] parts = instruction.Split('1');
            if (parts.Length != 5)
            {
                errorMessage = "Instrukce " + instruction + " musi obsahovat 5 casti oddelenych znakem \"1\".";
                return null;
            }

            char? inputSymbol = DecodeSymbol(parts[1], symbols, ref errorMessage);
            if (!inputSymbol.HasValue)
                return null;

            char? outputSymbol = DecodeSymbol(parts[3], symbols, ref errorMessage);
            if (!outputSymbol.HasValue)
                return null;

            char headMove = DecodeHeadMove(parts[4]);
            if (headMove == '?')
            {
                errorMessage = string.Format("Format pro pohyb hlavy {0} dat je neplatny.", parts[4]);
                return null;
            }

            return new TransitionFunction(parts[0], inputSymbol.Value, parts[2], outputSymbol.Value, headMove);
        }

        /// <summary>
        /// Převede binární kód pohybu hlavy na symbol používaný simulátorem.
        /// </summary>
        private char DecodeHeadMove(string encodedMove)
        {
            if (encodedMove == "0")
                return Properties.Settings.Default.MoveLeft;
            if (encodedMove == "00")
                return Properties.Settings.Default.MoveRight;
            if (encodedMove == "000")
                return Properties.Settings.Default.Stop;

            return '?';
        }

        /// <summary>
        /// Dekóduje symbol podle jeho indexu v předané abecedě.
        /// </summary>
        private char? DecodeSymbol(string encodedSymbol, IList<char> symbols, ref string errorMessage)
        {
            if (encodedSymbol.Length == 0)
            {
                errorMessage = "Kod symbolu nesmi byt prazdny.";
                return null;
            }

            int index = encodedSymbol.Length - 1;
            if (index < 0 || index >= symbols.Count)
            {
                errorMessage = string.Format("Symbol s kodem {0} neni definovan v paskove abecede.", encodedSymbol);
                return null;
            }

            return symbols[index];
        }

        /// <summary>
        /// Převede dočasné binární názvy stavů na čitelné názvy q0, qN a koncový stav.
        /// </summary>
        private static void DecodeStateNames(IList<TransitionFunction> transitions)
        {
            int longestStateCode = 0;
            foreach (TransitionFunction transition in transitions)
            {
                if (transition.InputState.Length > longestStateCode)
                    longestStateCode = transition.InputState.Length;
                if (transition.OutputState.Length > longestStateCode)
                    longestStateCode = transition.OutputState.Length;
            }

            foreach (TransitionFunction transition in transitions)
            {
                transition.InputState = DecodeStateName(transition.InputState, longestStateCode);
                transition.OutputState = DecodeStateName(transition.OutputState, longestStateCode);
            }
        }

        /// <summary>
        /// Převede jeden zakódovaný stav na uživatelský název stavu.
        /// </summary>
        private static string DecodeStateName(string encodedState, int finalStateLength)
        {
            if (encodedState.Length == 1)
                return Properties.Settings.Default.InitialState;
            if (encodedState.Length == finalStateLength)
                return Properties.Settings.Default.EndState;

            return "q" + (encodedState.Length - 1).ToString();
        }

        /// <summary>
        /// Ověří, že binární program obsahuje pouze znaky 0 a 1.
        /// </summary>
        private bool ValidateBinaryAlphabet(ref string errorMessage)
        {
            for (int i = 0; i < binaryCode.Length; i++)
            {
                if (binaryCode[i] != '0' && binaryCode[i] != '1')
                {
                    errorMessage = "Binarni kod smi obsahovat jen znaky 0 a 1, ale na pozici " + (i + 1).ToString() + " je jiny znak.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Dekóduje znakovou množinu uloženou v metadatech binárního formátu.
        /// </summary>
        private List<char> DecodeCharacterSet(string encodedAlphabet, ref string errorMessage)
        {
            if (encodedAlphabet.Length == 0)
            {
                errorMessage = "Abeceda nesmi byt prazdna.";
                return null;
            }

            List<char> decoded = new List<char>();
            string[] encodedSymbols = Split(encodedAlphabet, "11");
            for (int i = 0; i < encodedSymbols.Length; i++)
            {
                char? symbol = DecodeCharacter(encodedSymbols[i], ref errorMessage);
                if (!symbol.HasValue)
                    return null;

                if (IndexOf(decoded, symbol.Value) >= 0)
                {
                    errorMessage = string.Format("Symbol \"{0}\" je v abecede uveden vicekrat.", symbol.Value);
                    return null;
                }

                decoded.Add(symbol.Value);
            }

            return decoded;
        }

        /// <summary>
        /// Dekóduje jeden znak uložený jako posloupnost nul.
        /// </summary>
        private char? DecodeCharacter(string encodedSymbol, ref string errorMessage)
        {
            if (encodedSymbol.Length == 0)
            {
                errorMessage = "Kod znaku nesmi byt prazdny.";
                return null;
            }

            int charCode = encodedSymbol.Length - 1;
            if (charCode > char.MaxValue)
            {
                errorMessage = "Kod znaku je mimo podporovany rozsah.";
                return null;
            }

            return (char)charCode;
        }

        /// <summary>
        /// Dekóduje vstupní slovo uložené za blokem instrukcí.
        /// </summary>
        private string DecodeInputData(string inputBlock, ref string errorMessage)
        {
            if (inputBlock.Length == 0)
                return "";

            string[] encodedSymbols = inputBlock.Split('1');
            char[] input = new char[encodedSymbols.Length];
            for (int i = 0; i < encodedSymbols.Length; i++)
            {
                char? symbol = DecodeSymbol(encodedSymbols[i], alphabet, ref errorMessage);
                if (!symbol.HasValue)
                    return null;

                input[i] = symbol.Value;
            }

            return new string(input);
        }

        /// <summary>
        /// Ověří vztah vstupní abecedy, páskové abecedy a blank symbolu načtených z metadat.
        /// </summary>
        private bool ValidateDefinition(ref string errorMessage)
        {
            if (IndexOf(tapeAlphabet, blankSymbol) < 0)
            {
                errorMessage = "Blank symbol musi byt soucasti paskove abecedy.";
                return false;
            }

            if (IndexOf(alphabet, blankSymbol) >= 0)
            {
                errorMessage = "Blank symbol nesmi byt soucasti vstupni abecedy.";
                return false;
            }

            for (int i = 0; i < alphabet.Count; i++)
            {
                if (IndexOf(tapeAlphabet, alphabet[i]) < 0)
                {
                    errorMessage = string.Format("Symbol \"{0}\" ze vstupni abecedy chybi v paskove abecede.", alphabet[i]);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Rozdělí text podle víceznakového oddělovače.
        /// </summary>
        private static string[] Split(string text, string delimiter)
        {
            return text.Split(new string[] { delimiter }, StringSplitOptions.None);
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
