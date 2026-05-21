# 🧠 Universal Turing Machine Simulator

> Educational desktop simulator for exploring how simple transition rules, machine states, and tape operations can express general computation.

## 🎯 Overview

UTMS is a Windows desktop study project for loading, editing, running, inspecting, and exporting deterministic single-tape Turing machine programs.

A Turing machine is a formal model of computation consisting of states, an input/output tape, a tape head, and transition rules. Despite its simplicity, it is computationally equivalent to modern general-purpose programming languages in terms of what can be computed.

The application focuses on a clear educational workflow: open a machine definition, inspect its transition table, change the input word, run the simulation automatically or step by step, and observe how the head, tape, states, and transitions evolve. The codebase is split into a reusable simulation core, a WinForms user interface, and xUnit tests for the non-UI logic.

## ✨ Features

Current functionality:

- deterministic single-tape Turing machine simulation
- two supported machine formats: readable `.tm` files for editing and encoded `.btm` files for formal binary representation
- input alphabet, tape alphabet, blank symbol, transition rules, and input word loaded from files
- validation of input data against the formal input alphabet
- automatic execution with discrete speed levels
- pause/continue workflow for switching from automatic execution to manual control
- step-by-step execution for inspecting individual transitions
- visual tape with current state, head position, read symbol, step count, status, and last transition
- mouse drag support for moving the visible tape area
- two tape view modes: moving head over tape or tape following the head
- editable input word and blank symbol before execution
- read-only display of the current input alphabet and tape alphabet
- GUI transition editor for creating and changing machine definitions
- Graphviz DOT export of state graphs
- demo machines for common binary tasks

## 🧵 Tape Visualization

The tape panel displays:

- tape cells and their indexes
- the current head position
- the current machine state under the head
- blank symbols in a lighter style
- the last written cell with a short highlight
- automatic viewport adjustment when the head approaches an edge
- manual horizontal tape movement by dragging with the mouse

The status row above the tape shows the current state, head index, read symbol, step count, simulation status, and last executed transition.

## 🧮 Transition Editor

The transition editor allows transition rules to be edited in a grid:

| Column | Meaning |
| --- | --- |
| `Current state` | State before the transition |
| `Read` | Symbol read from the tape |
| `Next state` | State after the transition |
| `Write` | Symbol written to the tape |
| `Move` | Head movement: `L`, `R`, or `S` |

The state and symbol columns use dropdowns. The editor also supports creating a new state or a new tape symbol from the dropdown. It validates incomplete rows, invalid symbols, invalid head movements, and duplicate transitions for the same `(state, read symbol)` pair before changes can be accepted.

The main form also opens this editor on double-click in the runtime transition list and selects the corresponding transition row.

## 🧩 Demo Machines

Demo files are stored in `demos/`.

| File | Description |
| --- | --- |
| `bin_increment.tm` / `bin_increment.btm` | Adds `1` to a binary number |
| `bin_decrement.tm` / `bin_decrement.btm` | Subtracts `1` from a binary number while preserving input width |
| `bin_bitwise_not.tm` / `bin_bitwise_not.btm` | Flips all bits in a binary number |
| `bin_mirroring.tm` / `bin_mirroring.btm` | Appends a reversed copy of the binary input |
| `binary_shift_left.tm` / `binary_shift_left.btm` | Appends a zero bit to the right side of a binary number |
| `palindrome_check.tm` / `palindrome_check.btm` | Checks whether the binary input is a palindrome and leaves `1` or `0` as the result |

## 📝 Text Machine Format

The `.tm` format is intended to be readable and easy to edit.

Example:

```text
// Task: add 1 to a binary number.

alphabet = {0,1}
tapeAlphabet = {0,1,#}
blank = #

(q0, 1) = (q0, 1, R)
(q0, 0) = (q0, 0, R)
(q0, #) = (q1, #, L)
(q1, 0) = (qF, 1, S)

w = 1011
```

Supported declarations:

| Declaration | Meaning |
| --- | --- |
| `alphabet = {0,1}` | Input alphabet: symbols allowed in the input word |
| `tapeAlphabet = {0,1,#,x}` | Full tape alphabet, including input symbols, blank, and helper symbols |
| `blank = #` | Blank tape symbol |
| `w = 1011` | Input word written to the tape before simulation |

Transition syntax:

```text
(inputState, inputSymbol) = (outputState, outputSymbol, headMove)
```

Supported head moves:

| Symbol | Meaning |
| --- | --- |
| `L` | Move left |
| `R` | Move right |
| `S` | Stay on the current cell |

Notes:

- Lines starting with `//` are treated as comments.
- Each alphabet symbol is a single character.
- The blank symbol must be part of the tape alphabet.
- The blank symbol must not be part of the input alphabet.
- Every input alphabet symbol must also be part of the tape alphabet.
- Helper symbols such as `x`, `y`, `o`, or `i` belong to the tape alphabet, not necessarily to the input alphabet.
- For deterministic machines, only one transition may exist for the same `(state, read symbol)` pair.

## 🔢 Binary Machine Format

The `.btm` format stores an encoded machine definition with:

- input alphabet
- tape alphabet
- blank symbol
- transition rules
- input data

The binary format starts with the current metadata-aware prefix and is intended mainly as a formal encoded representation. The `.tm` format remains the preferred human-editable format.

When a `.tm` file is exported to `.btm`, state names are encoded by position. After loading a `.btm` file, states are restored as canonical names such as `q0`, `q1`, and `qF`. This is intentional for the binary representation.

## 📈 Graph Export

`Tools -> Export graph...` writes the current machine as a Graphviz DOT file.

The exported graph contains:

- an initial point node leading to `q0`
- all states found in the transition table
- `qF` rendered as a double-circle state
- directed transition edges
- edge labels in the form `(read,write,move)`, for example `(0,1,R)`

The menu item is enabled only when a loaded machine contains at least one transition.

## 🗂️ Project Structure

```text
/
+-- demos/                     # Example .tm and .btm machine definitions
|
+-- UTMS.Core/                 # Simulation core and file formats
|   +-- BinaryCode.cs
|   +-- SyntaxChecker.cs
|   +-- TuringMachine.cs
|   +-- TuringMachineDefinition.cs
|   +-- TuringMachineDefinitionLoader.cs
|   +-- TuringMachineGraphExporter.cs
|   +-- TuringMachineProgram.cs
|   +-- TuringMachineProgramSerializer.cs
|   +-- TuringSimulator.cs
|   +-- Properties/
|
+-- UTMS.WinForms/             # Windows Forms user interface
|   +-- Assets/
|   |   +-- machine_stop.wav
|   |   +-- tape_tick.wav
|   +-- MainForm.cs
|   +-- MainForm.Designer.cs
|   +-- PromptDialog.cs
|   +-- PromptDialog.Designer.cs
|   +-- SimulationVisualState.cs
|   +-- SoundEffectPlayer.cs
|   +-- TapeRenderer.cs
|   +-- TransitionEditorForm.cs
|   +-- TransitionEditorForm.Designer.cs
|   +-- Program.cs
|   +-- Properties/
|
+-- UTMS.Tests/                # xUnit tests for non-UI logic
|   +-- BinaryCodeTest.cs
|   +-- TapeTest.cs
|   +-- TuringMachineDefinitionLoaderTest.cs
|   +-- TuringMachineGraphExporterTest.cs
|   +-- TuringMachineProgramSerializerTest.cs
|   +-- TuringMachineProgramTest.cs
|   +-- TuringMachineTaskTest.cs
|   +-- TuringSimulatorTest.cs
|
+-- UTMS.sln
+-- README.md
```

## 🔧 Requirements

- Windows
- .NET 10 SDK
- Visual Studio 2026 or newer with Windows Forms support

Test packages:

- `xunit`
- `Microsoft.NET.Test.Sdk`
- `xunit.runner.visualstudio`

## 🚀 Running the Application

From the repository root:

```powershell
dotnet restore
dotnet build UTMS.sln
dotnet run --project UTMS.WinForms\UTMS.WinForms.csproj
```

The application can also be opened and run directly from Visual Studio using `UTMS.sln`.

## 🧪 Testing

Run all tests:

```powershell
dotnet test UTMS.sln
```

The test suite covers:

- tape behavior
- transition lookup
- simulator execution
- text machine loading
- binary machine decoding
- machine definition validation
- duplicate transition validation
- text and binary serialization
- Graphviz DOT graph export
- concrete machine tasks defined directly in tests

## ⚠️ Current Limitations

Known limitations:

- the simulator currently supports a single tape
- graph export currently writes DOT text, not rendered image files
- the transition editor is functional, but still intentionally simple
- `.btm` is useful as a formal encoded format, but `.tm` is the preferred human-editable format
- the application UI and user-facing messages are in English, while code comments are currently Czech

## 🛣️ Roadmap

Planned direction:

- richer transition editor workflow
- stronger validation and editor feedback
- visual redesign of the WinForms interface
- optional rendered graph export through Graphviz
- visual representation of states and transitions inside the application
- broader demo set, including non-binary alphabets
- future exploration of a real universal Turing machine definition

## Status

Current status:

- ✅ core simulation logic is separated from the GUI
- ✅ project structure is prepared for a Git repository
- ✅ MSTest was replaced by xUnit
- ✅ demo programs are available in both supported formats
- ✅ build and test suite currently pass on .NET 10
- ✅ `.tm` and `.btm` can be opened and saved from the GUI
- ✅ DOT graph export is available from the GUI

## License

Educational project intended for study and demonstration purposes.
