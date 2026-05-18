# 🧠 Universal Turing Machine Simulator

> Educational simulator for single-tape Turing machines with text and binary machine definitions.

## 🎯 Overview

UTMS is a desktop study project for loading, running, inspecting, and exporting Turing machine programs.

The current implementation focuses on a deterministic single-tape simulator. It supports a readable text format (`.tm`) and a binary encoded format (`.btm`) for machine definitions. The codebase is intentionally split into a reusable simulation core, a WinForms user interface, and xUnit tests for the non-UI logic.

The long-term direction is to grow the project into a richer Universal Turing Machine Simulator environment with an editable transition table, better visualization, and graph export.

## ✨ Features

Current functionality:

- single-tape Turing machine simulation
- step-by-step and continuous execution
- configurable input data loaded from machine files
- text machine definition format (`.tm`)
- binary encoded machine definition format (`.btm`)
- input alphabet and tape alphabet declarations in `.tm` files
- custom blank symbols loaded from machine definitions
- `.btm` v2 format preserving alphabet metadata
- demo machines for common binary operations
- xUnit tests covering core logic, parsers, serializers, and demos

## 🧪 Demo Machines

Demo files are stored in `demos/`.

| File | Description |
| --- | --- |
| `bin_increment.tm` / `bin_increment.btm` | Adds `1` to a binary number |
| `bin_decrement.tm` / `bin_decrement.btm` | Subtracts `1` from a binary number while preserving input width |
| `bin_bitwise_not.tm` / `bin_bitwise_not.btm` | Flips all bits in a binary number |
| `bin_mirroring.tm` / `bin_mirroring.btm` | Appends a reversed copy of the binary input |

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
| `alphabet = {0,1}` | input alphabet |
| `tapeAlphabet = {0,1,#,x}` | full tape alphabet, including blank and helper symbols |
| `blank = #` | blank tape symbol |
| `w = 1011` | input written to the tape before simulation |

Transition syntax:

```text
(inputState, inputSymbol) = (outputState, outputSymbol, headMove)
```

Supported head moves:

| Symbol | Meaning |
| --- | --- |
| `L` | move left |
| `R` | move right |
| `S` | stay / stop move |

## 🔢 Binary Machine Format

The `.btm` format stores an encoded machine definition. The current binary format starts with the version prefix `1111` and stores input alphabet, tape alphabet, blank symbol, transitions, and input data.

When a `.tm` file is exported to `.btm`, user-defined state names are encoded by position and are restored as canonical names such as `q0`, `q1`, and `qF` after loading. This is intentional: the binary format is primarily a formal encoded representation, while `.tm` remains the user-friendly editable format.

## 🗂️ Project Structure

```text
/
+-- demos/                 # Example .tm and .btm machine definitions
|
+-- UTMS.Core/             # Simulation core and file formats
|   +-- BinaryCode.cs
|   +-- SyntaxChecker.cs
|   +-- TuringMachine.cs
|   +-- TuringMachineDefinition.cs
|   +-- TuringMachineDefinitionLoader.cs
|   +-- TuringMachineProgram.cs
|   +-- TuringMachineProgramSerializer.cs
|   +-- TuringMachineSimulator.cs
|   +-- Properties/
|
+-- UTMS.WinForms/         # Windows Forms user interface
|   +-- MainForm.cs
|   +-- MainForm.Designer.cs
|   +-- Program.cs
|   +-- Properties/
|
+-- UTMS.Tests/            # xUnit tests for non-UI logic
|   +-- BinaryCodeTest.cs
|   +-- DemoProgramTest.cs
|   +-- TapeTest.cs
|   +-- TuringMachineDefinitionLoaderTest.cs
|   +-- TuringMachineProgramSerializerTest.cs
|   +-- TuringMachineProgramTest.cs
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

## ✅ Testing

Run all tests:

```powershell
dotnet test UTMS.sln
```

The test suite currently covers:

- tape behavior
- transition lookup
- simulator execution
- text machine loading
- binary machine decoding
- machine definition validation
- text and binary serialization
- demo machine loading and execution

## ⚠️ Current Limitations

Known limitations:

- the simulator currently supports a single tape
- GUI editing of transition tables is not implemented yet
- graph visualization/export is not implemented yet
- `.btm` is useful as a formal encoded format, but `.tm` is the preferred human-editable format

## 🛣️ Roadmap

Planned direction:

- transition table editor in the GUI
- export current GUI machine state to `.tm` and `.btm`
- stronger validation and editor feedback
- visual representation of states and transitions
- optional transition graph export, for example to DOT/Graphviz or PNG
- visual redesign of the WinForms interface

## 📌 Status

Current status:

- ✅ core simulation logic is separated from the GUI
- ✅ project structure is prepared for a Git repository
- ✅ MSTest was replaced by xUnit
- ✅ demo programs are available in both supported formats
- ✅ build and test suite currently pass on .NET 10

## 📄 License

Educational project intended for study and demonstration purposes.
