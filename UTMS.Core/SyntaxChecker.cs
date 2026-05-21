namespace UTMS.Core
{
    /// <summary>
    /// Kontroluje syntaxi textových instrukcí ve tvaru (q0, 1) = (q1, 0, L).
    /// </summary>
    class SyntaxChecker
    {
        /// <summary>
        /// Ověří syntaxi všech předaných řádků programu.
        /// </summary>
        public bool IsSyntaxValid(string[] lines, ref string description)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                if (!IsLineSyntaxValid(lines[i], ref description))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Ověří syntaxi jednoho řádku programu.
        /// </summary>
        public bool IsLineSyntaxValid(string line, ref string description)
        {
            if (line == null)
            {
                description = "Line cannot be empty.";
                return false;
            }

            line = line.Trim().Replace(" ", "");
            if (NumToken(line, '=') != 2)
            {
                description = "Instruction line must contain input and output parts separated by an equals sign.";
                return false;
            }

            if (line.StartsWith("w="))
                return true;

            string firstTupleExpression = GetToken(line, '=', 1);
            string secondTupleExpression = GetToken(line, '=', 2);
            string firstTuple;
            string secondTuple;

            if (!TryGetTupleContent(firstTupleExpression, ref description, out firstTuple))
                return false;

            if (!TryGetTupleContent(secondTupleExpression, ref description, out secondTuple))
                return false;

            if (NumToken(firstTuple, ',') != 2)
            {
                description = "The first tuple must contain two values separated by a comma.";
                return false;
            }

            if (GetToken(firstTuple, ',', 2).Length != 1)
            {
                description = "The second value in the first tuple must be a single character.";
                return false;
            }

            if (NumToken(secondTuple, ',') != 3)
            {
                description = "The second tuple must contain three values separated by commas.";
                return false;
            }

            if (GetToken(secondTuple, ',', 2).Length != 1)
            {
                description = "The second value in the second tuple must be a single character.";
                return false;
            }

            string headMove = GetToken(secondTuple, ',', 3);
            Properties.Settings settings = Properties.Settings.Default;
            if (headMove == settings.MoveLeft.ToString() || headMove == settings.MoveRight.ToString() || headMove == settings.Stop.ToString())
                return true;

            description = string.Format("The third value in the second tuple must be {0}, {1} or {2}.", settings.MoveLeft, settings.MoveRight, settings.Stop);
            return false;
        }

        /// <summary>
        /// Vyjme obsah závorek a ověří, že závorky obalují celou část instrukce.
        /// </summary>
        private static bool TryGetTupleContent(string expression, ref string description, out string content)
        {
            content = "";
            int openParen = expression.IndexOf('(');
            int closeParen = expression.IndexOf(')');

            if (openParen < 0 || closeParen < 0 || closeParen <= openParen + 1)
            {
                description = "Each instruction part must be enclosed in parentheses.";
                return false;
            }

            if (openParen != 0 || closeParen != expression.Length - 1)
            {
                description = "Parentheses must enclose the whole instruction part.";
                return false;
            }

            content = expression.Substring(openParen + 1, closeParen - openParen - 1);
            return true;
        }

        /// <summary>
        /// Vrátí počet částí oddělených zadaným znakem.
        /// </summary>
        public static int NumToken(string text, char delimiter)
        {
            string[] tokens = text.Split(delimiter);
            return tokens.Length;
        }

        /// <summary>
        /// Vrátí zvolenou část textu odděleného zadaným znakem.
        /// </summary>
        public static string GetToken(string text, char delimiter, int tokenNumber)
        {
            string[] tokens = text.Split(delimiter);
            if (tokenNumber <= 0 || tokenNumber > tokens.Length)
                return "";

            return tokens[tokenNumber - 1];
        }
    }
}
