using System;

namespace UTMS.Core
{
    /// <summary>
    /// Pravidla pro hodnoty, které lze bezpečně zapsat do čitelného textového formátu stroje.
    /// </summary>
    public static class TuringMachineTextFormatRules
    {
        private static readonly char[] StateReservedCharacters = new char[] { '(', ')', ',', '=' };
        private static readonly char[] SymbolReservedCharacters = new char[] { '(', ')', ',', '=', '{', '}' };

        /// <summary>
        /// Ověří název stavu proti znakům, které by rozbily zápis přechodové funkce.
        /// </summary>
        public static bool TryValidateStateName(string stateName, out string errorMessage)
        {
            errorMessage = "";
            if (string.IsNullOrWhiteSpace(stateName))
            {
                errorMessage = "State name cannot be empty.";
                return false;
            }

            for (int i = 0; i < stateName.Length; i++)
            {
                if (char.IsWhiteSpace(stateName[i]))
                {
                    errorMessage = "State name cannot contain whitespace.";
                    return false;
                }
            }

            if (ContainsAny(stateName, StateReservedCharacters))
            {
                errorMessage = "State name contains a character reserved by the text format.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ověří páskový symbol proti znakům, které nelze jednoznačně uložit do textového formátu.
        /// </summary>
        public static bool TryValidateTapeSymbol(char symbol, out string errorMessage)
        {
            errorMessage = "";
            if (char.IsWhiteSpace(symbol))
            {
                errorMessage = "Tape symbol cannot be whitespace.";
                return false;
            }

            for (int i = 0; i < SymbolReservedCharacters.Length; i++)
            {
                if (SymbolReservedCharacters[i] == symbol)
                {
                    errorMessage = "Tape symbol is reserved by the text format.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Zjistí, zda text obsahuje některý z vyhrazených znaků.
        /// </summary>
        private static bool ContainsAny(string value, char[] reservedCharacters)
        {
            for (int i = 0; i < value.Length; i++)
            {
                for (int j = 0; j < reservedCharacters.Length; j++)
                {
                    if (value[i] == reservedCharacters[j])
                        return true;
                }
            }

            return false;
        }
    }
}
