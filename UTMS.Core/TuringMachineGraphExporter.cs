using System;
using System.Collections.Generic;
using System.Text;

namespace UTMS.Core
{
    /// <summary>
    /// Exportuje definici Turingova stroje do textového formátu Graphviz DOT.
    /// </summary>
    public static class TuringMachineGraphExporter
    {
        /// <summary>
        /// Vytvoří DOT graf stavů a přechodů Turingova stroje.
        /// </summary>
        public static string ToDot(TuringMachineDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException(nameof(definition));

            List<string> states = CollectStates(definition.Transitions);
            string startNode = CreateHelperNodeName(states, "__utms_start");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("digraph TuringMachine {");
            sb.AppendLine("    rankdir=LR;");
            sb.AppendLine("    node [shape=circle];");
            sb.AppendLine();
            sb.AppendFormat("    {0} [shape=point, label=\"\"];", Quote(startNode));
            sb.AppendLine();
            sb.AppendFormat("    {0} -> {1};", Quote(startNode), Quote(TransitionFunction.InitialStateName));
            sb.AppendLine();
            sb.AppendLine();

            foreach (string state in states)
            {
                if (state == TransitionFunction.FinalStateName)
                    sb.AppendFormat("    {0} [shape=doublecircle];", Quote(state));
                else
                    sb.AppendFormat("    {0};", Quote(state));

                sb.AppendLine();
            }

            sb.AppendLine();
            foreach (TransitionFunction transition in definition.Transitions)
            {
                sb.AppendFormat(
                    "    {0} -> {1} [label={2}];",
                    Quote(transition.InputState),
                    Quote(transition.OutputState),
                    Quote(FormatTransitionLabel(transition)));
                sb.AppendLine();
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Posbírá všechny stavy použité v přechodech a doplní počáteční i koncový stav.
        /// </summary>
        private static List<string> CollectStates(IEnumerable<TransitionFunction> transitions)
        {
            List<string> states = new List<string>();
            AddDistinct(states, TransitionFunction.InitialStateName);
            AddDistinct(states, TransitionFunction.FinalStateName);

            foreach (TransitionFunction transition in transitions)
            {
                AddDistinct(states, transition.InputState);
                AddDistinct(states, transition.OutputState);
            }

            states.Sort(StringComparer.Ordinal);
            return states;
        }

        /// <summary>
        /// Vytvoří pomocný název uzlu tak, aby nekolidoval s uživatelskými stavy.
        /// </summary>
        private static string CreateHelperNodeName(IList<string> existingStates, string baseName)
        {
            string name = baseName;
            int suffix = 1;
            while (Contains(existingStates, name))
            {
                name = baseName + suffix;
                suffix++;
            }

            return name;
        }

        /// <summary>
        /// Vytvoří popisek hrany v kompaktním tvaru (čtený,zapsaný,pohyb).
        /// </summary>
        private static string FormatTransitionLabel(TransitionFunction transition)
        {
            return string.Format("({0},{1},{2})", transition.InputSymbol, transition.OutputSymbol, transition.HeadMove);
        }

        /// <summary>
        /// Zapíše DOT řetězec v uvozovkách a ošetří speciální znaky.
        /// </summary>
        private static string Quote(string value)
        {
            if (value == null)
                return "\"\"";

            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c == '\\' || c == '"')
                    sb.Append('\\');

                if (c == '\r')
                    sb.Append("\\r");
                else if (c == '\n')
                    sb.Append("\\n");
                else
                    sb.Append(c);
            }

            sb.Append('"');
            return sb.ToString();
        }

        /// <summary>
        /// Přidá hodnotu do seznamu jen v případě, že v něm ještě není.
        /// </summary>
        private static void AddDistinct(IList<string> values, string value)
        {
            if (!Contains(values, value))
                values.Add(value);
        }

        /// <summary>
        /// Ověří přítomnost hodnoty v seznamu bez závislosti na LINQ.
        /// </summary>
        private static bool Contains(IEnumerable<string> values, string value)
        {
            foreach (string existingValue in values)
            {
                if (existingValue == value)
                    return true;
            }

            return false;
        }
    }
}
