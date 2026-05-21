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
                description = "Radek nesmi byt prazdny.";
                return false;
            }

            line = line.Trim().Replace(" ", "");
            if (NumToken(line, '=') != 2)
            {
                description = "Radek s instrukci musi obsahovat popis pocatecniho a koncoveho stavu oddelene rovnitkem.";
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
                description = "Mezi zavorkami musi byt dva udaje oddelene carkou.";
                return false;
            }

            if (GetToken(firstTuple, ',', 2).Length != 1)
            {
                description = "Druhy udaj v prvni zavorce musi byt jediny znak.";
                return false;
            }

            if (NumToken(secondTuple, ',') != 3)
            {
                description = "Mezi druhymi zavorkami musi byt tri udaje oddelene carkou.";
                return false;
            }

            if (GetToken(secondTuple, ',', 2).Length != 1)
            {
                description = "Druhy udaj v druhe zavorce musi byt jediny znak.";
                return false;
            }

            string headMove = GetToken(secondTuple, ',', 3);
            Properties.Settings settings = Properties.Settings.Default;
            if (headMove == settings.MoveLeft.ToString() || headMove == settings.MoveRight.ToString() || headMove == settings.Stop.ToString())
                return true;

            description = string.Format("Jako treti udaj v druhe zavorce musi byt {0} nebo {1} nebo {2}.", settings.MoveLeft, settings.MoveRight, settings.Stop);
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
                description = "Kazda cast instrukce musi byt uzavrena v zavorkach.";
                return false;
            }

            if (openParen != 0 || closeParen != expression.Length - 1)
            {
                description = "Zavorky musi obalovat celou cast instrukce.";
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
