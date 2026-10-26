# EdiUITests

UI test automation for the [Edi](https://github.com/Dirkster99/Edi) text editor, built with FlaUI and NUnit as a technical QA assignment.

| # | Acceptance criterion | Where it's covered |
|---|---|---|
| 1 | Build Edi locally | [Building Edi](#building-edi) |
| 2 | Custom UI automation for core features | FlaUI + NUnit, see [The Solution Itself](#the-solution-itself) |
| 3 | At least 6 test cases | 10 tests, see [Test Inventory](#test-inventory) |
| 4 | Automatic report of actual vs. expected results | HTML + TRX reports showing expected vs. actual values for each failure, with per-test logs and screenshots, see [Test Results](#test-results) |
| 5 | Documentation | This README |

**Tested against:** Edi commit `2f82afb` + manual nuget version fixes · Windows 10 v22HZ

## Table of Contents

- [The Stack](#the-stack)
- [Building Edi](#building-edi)
- [The Runsettings](#the-runsettings)
- [Running the Tests](#running-the-tests)
  - [Using Visual Studio](#using-visual-studio)
  - [Using Command Line](#using-command-line)
- [The Solution Itself](#the-solution-itself)
  - [Features](#features)
  - [Code Considerations](#code-considerations)
- [The Tests](#the-tests)
- [Test Inventory](#test-inventory)
- [Test Results](#test-results)
- [Current Results](#current-results)
- [Issues Found](#issues-found)
  - [BUG-1: Unhandled exception in the launching process](#bug-1-unhandled-nullreferenceexception-in-the-launching-edi-process-when-opening-a-file-via-command-line-argument)
  - [BUG-2: Opened file is not the active tab](#bug-2-opened-file-is-not-the-active-tab-after-startup)
- [Known Issues](#known-issues)

## The Stack

The following NuGet packages were used for the project:

| Package | Version |
| --- | --- |
| [NUnit](https://nunit.org/) | 3.14 |
| [FlaUI](https://github.com/FlaUI/FlaUI) | 5.0.0 |

The test project was built in **Visual Studio 2026** targeting **.NET 8**.

## Building Edi

1. Download **Visual Studio 2017**, as it comes with the required **.NET Framework 4.5.2** built in.
2. Update the broken `Dirkster.MWindowLib` references in the affected projects, as outlined in [this PR](https://github.com/Dirkster99/Edi/pull/66).
3. Build the solution.

## The Runsettings

To execute the tests, the following `.runsettings` file is required (included in the repository):

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
	<!-- Configurations that affect the Test Framework -->
	<RunConfiguration>
		<NumberOfTestWorkers>1</NumberOfTestWorkers>
		<!-- Path relative to directory that contains .runsettings file-->
		<ResultsDirectory>.\TestResults</ResultsDirectory>
	</RunConfiguration>

	<!-- Parameters used by tests at run time -->
	<TestRunParameters>
		<Parameter name="EdiPath" value="Path\To\Edi.exe" />
	</TestRunParameters>
	<LoggerRunSettings>
		<Loggers>
			<Logger friendlyName="trx">
				<Configuration>
					<LogFileName>EdiUITests.trx</LogFileName>
				</Configuration>
			</Logger>
			<Logger friendlyName="html">
				<Configuration>
					<LogFileName>EdiUITests.html</LogFileName>
				</Configuration>
			</Logger>
		</Loggers>
	</LoggerRunSettings>
</RunSettings>
```

| Setting | Description |
| --- | --- |
| `NumberOfTestWorkers` | Set to `1` so that several Edi instances don't run simultaneously and interfere with each other. |
| `ResultsDirectory` | The directory where `EdiUITests.trx` / `EdiUITests.html` are saved. User's choice. |
| `EdiPath` | Path to `Edi.exe`. The tests need it to launch Edi during execution. **Update this before running.** |
| `Loggers` | Output formats of the test results. `.html` is included for ease of use — any browser can open it. `.trx` gives an overall better experience, as it links directly to logs and screenshots, but requires Visual Studio to open. |

## Running the Tests

### Using Visual Studio

1. Open the `.slnx` and build the project via **Build → Build Solution**.
2. Go to **Test → Configure Run Settings → Select Solution Wide runsettings File** and select the `edi.runsettings` included in the solution.
3. Run the tests from the **Test Explorer** tab.

### Using Command Line
From the repository root:
```
dotnet test --settings edi.runsettings
```

## The Solution Itself

### Features

- **Clean state** — Edi settings are deleted before every run to ensure consistency between runs.
- **Isolated file operations** — every file operation uses temp files and temp folders, which are deleted after each test run.
- **Per-process logs** — every Edi process started during a test has its own logs saved.
- **Screenshots on failure** — a screenshot of the screen is saved whenever a test fails. See [Known Issues](#known-issues) for one exception.
- **Crash monitoring** — unhandled exceptions in Edi processes are monitored.

### Code Considerations

The goal was to keep the actual test implementations as readable as possible, free of FlaUI lookup boilerplate. To achieve this, UI elements are separated into their own dedicated classes, so tests can interact with them directly and clearly.

Tests are split into classes by category. There is some overlap between them, which could be restructured in the future. The more important part is the `TestBase` class, which handles the core testing features listed above.

## The Tests

- **Naming** — test names follow the `Given_When_Then` convention. Because the paths used to organize test results can get very long, method names are heavily abbreviated. Each test also has a user-friendly `Description` attribute.
- **Categories** — since categories can overlap (e.g. a startup test that is also a file-open test), some tests carry multiple `Category` attributes.
- **File opening** — the provided example tests were used as a starting point. Test cases cover several file-open scenarios, both with the application already running and at startup. Rather than opening files through Explorer's "Open With" functionality, the tests start the Edi process with a file path argument, which mimics opening files from the command line.
- **New files** — creating, writing and saving new files is covered by multiple single-concern tests.

## Test Inventory

| Test | Tests.xml ref | Scenario |
|---|---|---|
| `AppNotRun_StartApp_AppRunWindowLoaded` | (smoke) | Edi starts and its main window loads |
| `AppRun_OpenReadOnlyTextViaMenu_ShownReadOnly` | 001 | File > Open on a read-only file: content shown, not editable, lock icon |
| `AppNotRun_OpenReadOnlyTextViaArg_ShownReadOnly` | 002 (Edi not started) | Open file via argument at startup |
| `AppRun_OpenReadOnlyTextViaArg_ShownReadOnly` | 002 (Edi already started) | Open file via argument while running |
| `AppNotRunRecentFile_OpenReadOnlyTextViaArg_ShownReadOnly` | 002 (not started) + documented bug | Opened file is the active tab, not the last active one |
| `AppRunRecentFile_OpenReadOnlyTextViaArg_ShownReadOnly` | 002 (started) | Same, with Edi already running |
| `AppRun_CreateNewText_ShownEditable` | 003 | File > New creates an editable "Untitled.txt" tab |
| `AppRun_CreateNewTextSave_Saved` | 003 | File > New + Save As creates the file |
| `AppRun_CreateNewTextWrite_ShownStar` | 004 | Typing marks the tab dirty (`*`) |
| `AppRun_CreateNewTextWriteSave_Saved` | 004 | Typed content is saved with File > Save |

### Not covered

| Tests.xml ref | Scenario | Reason |
|---|---|---|
| 002 | Real Explorer "Open with" | Replaced with a command-line argument, which is the same entry point without automating Explorer |
| 005 | Drag & drop from Explorer | Time/Unreliable |
| 006, 007 | Recent Files / Explorer tool windows | Time |
| 008, 009 | Re-open previous session on restart | Time |

## Test Results

Individual test data is stored in the built DLL's folder, organized as:

```
\<TestRunTimeStamp>\<TestClass>\<TestName>\
```

Each test folder contains its logs and, for failed tests, a screenshot.

The overall test run result is saved to the user-defined `ResultsDirectory` in `.html` and `.trx` formats, together with an automatically copied set of the test data that the `.trx` file links to.

To examine the results in Visual Studio, right-click a test and choose **View Test Result Details** to see more information, including the files attached to the test run.

## Current Results

10 tests · 6 passed · 4 failed · ~51 s.
All 4 failures are caused by defects in Edi.

## Issues Found

### BUG-1: Unhandled NullReferenceException in the launching Edi process when opening a file via command-line argument
**Severity:** Low (the application defers execution to the already running Edi process and opens the file, the user does not notice the error in the second Edi process)
**Found by:** `AppRun_OpenReadOnlyTextViaArg_ShownReadOnly`, `AppRunRecentFile_OpenReadOnlyTextViaArg_ShownReadOnly`

**Steps**
1. Start Edi.
2. Launch `Edi.exe <path-to-text-file>`.

**Expected:** The file opens in the running instance, and the launching process hands off and exits without errors.
**Actual:** The file opens in a new tab but an unhandled `System.NullReferenceException` is thrown in the Edi launched with arguments in `ApplicationViewModel.IntegrateDocumentVm` (ApplicationViewModel.cs:531), called from `App.ProcessCmdLine`.
**Evidence:** Stack trace in the test log.

### BUG-2: Opened file is not the active tab after startup
**Severity:** Medium (wrong document focused)
**Found by:** `AppNotRunRecentFile_OpenReadOnlyTextViaArg_ShownReadOnly`, `AppNotRun_OpenReadOnlyTextViaArg_ShownReadOnly`
**Reference:** Known issue in Edi's Tests.xml, test 002

**Steps**
1. Open `notEmpty.txt` in Edi and exit.
2. Launch `Edi.exe other.txt`.

**Expected:** `other.txt` is the selected tab.
**Actual:** `notEmpty.txt` (the last active document) is selected.
With no previous session, the Start Page stays selected instead of the opened file.

## Known Issues

- Edi's logger does not produce useful logs, even at its most verbose logging settings. It has not yet been investigated whether additional logging configuration is needed.
- `AppRun_CreateNewTextWrite_ShownStar` does not handle the "unsaved changes" popup on exit.
- When an otherwise successful test fails due to an unhandled exception it is detected in `TearDown`, after the opportunity to take a screenshot so no screenshot is taken.
- Some helper classes violate clean code principles, as they occasionally contain assertions.

## Future Improvements

- Fix [Known Issues](#known-issues)
- Implement remaining tests. See [Test Inventory](#test-inventory)
