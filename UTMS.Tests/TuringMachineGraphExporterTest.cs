using UTMS.Core;
using Xunit;

namespace UTMS.Tests
{
    /// <summary>
    /// Testy exportu stavového grafu Turingova stroje do Graphviz DOT.
    /// </summary>
    public class TuringMachineGraphExporterTest
    {
        /// <summary>
        /// Ověřuje, že DOT export obsahuje orientovaný graf, počáteční šipku a popsaný přechod.
        /// </summary>
        [Fact]
        public void ToDot_WritesInitialNodeAndTransitionLabel()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0', '1' },
                new char[] { '0', '1', '#' },
                '#',
                "0",
                new TransitionFunction[]
                {
                    new TransitionFunction("q0", '0', "q1", '1', 'R')
                });

            string dot = TuringMachineGraphExporter.ToDot(definition);

            Assert.Contains("digraph TuringMachine", dot);
            Assert.Contains("rankdir=LR", dot);
            Assert.Contains("\"__utms_start\" -> \"q0\"", dot);
            Assert.Contains("\"q0\" -> \"q1\" [label=\"(0,1,R)\"]", dot);
        }

        /// <summary>
        /// Ověřuje, že koncový stav je vykreslen jako dvojitý kruh.
        /// </summary>
        [Fact]
        public void ToDot_WritesFinalStateAsDoubleCircle()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0' },
                new char[] { '0', '#' },
                '#',
                "0",
                new TransitionFunction[]
                {
                    new TransitionFunction("q0", '0', "qF", '0', 'S')
                });

            string dot = TuringMachineGraphExporter.ToDot(definition);

            Assert.Contains("\"qF\" [shape=doublecircle]", dot);
        }

        /// <summary>
        /// Ověřuje, že export ošetří uvozovky v uživatelském názvu stavu.
        /// </summary>
        [Fact]
        public void ToDot_EscapesStateNames()
        {
            TuringMachineDefinition definition = new TuringMachineDefinition(
                new char[] { '0' },
                new char[] { '0', '#' },
                '#',
                "0",
                new TransitionFunction[]
                {
                    new TransitionFunction("q0", '0', "q\"1", '0', 'R'),
                    new TransitionFunction("q\"1", '#', "qF", '#', 'S')
                });

            string dot = TuringMachineGraphExporter.ToDot(definition);

            Assert.Contains("\"q\\\"1\"", dot);
        }
    }
}
