# EdiUITests

UI test automation for the [Edi](https://github.com/Dirkster99/Edi) text editor, built with FlaUI and NUnit.

## Table of Contents

- [The Stack](#the-stack)
- [Building Edi](#building-edi)
- [The Runsettings](#the-runsettings)
- [Running the Tests](#running-the-tests)
- [The Solution Itself](#the-solution-itself)
  - [Features](#features)
  - [Code Considerations](#code-considerations)
- [The Tests](#the-tests)
- [Test Results](#test-results)
- [Known Issues](#known-issues)

## The Stack

The following NuGet packages / software were used for the project:

| Package / Tool | Version |
| --- | --- |
| [NUnit](https://nunit.org/) | 3.14 |
| [FlaUI](https://github.com/FlaUI/FlaUI) | 5.0.0 |
| [FlaUInspect](https://github.com/FlaUI/FlaUInspect) | 3.1.0 |

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

1. Open the `.sln` and build the project via **Build → Build Solution**.
2. Go to **Test → Configure Run Settings → Select Solution Wide runsettings File** and select the `edi.runsettings` included in the solution.
3. Run the tests from the **Test Explorer** tab.

## The Solution Itself

### Features

- **Clean state** — Edi settings are deleted before every run to ensure consistency between runs.
- **Isolated file operations** — every file operation uses temp files and temp folders, which are deleted after each test run.
- **Per-process logs** — every Edi process started during a test has its own logs saved.
- **Screenshots on failure** — a screenshot of the screen is saved whenever a test fails.
- **Crash monitoring** — unhandled exceptions in Edi processes are monitored.

### Code Considerations

The goal was to keep the actual test implementations as readable as possible, free of FlaUI lookup boilerplate. To achieve this, UI elements are separated into their own dedicated classes, so tests can interact with them directly and clearly.

Tests are split into classes by category. There is some overlap between them, which could be restructured in the future. The more important part is the `TestBase` class, which handles the core testing features listed above.

## The Tests

- **Naming** — test names follow the `Given_When_Then` convention. Because the paths used to organize test results can get very long, method names are heavily abbreviated. Each test also has a user-friendly `Description` attribute.
- **Categories** — since categories can overlap (e.g. a startup test that is also a file-open test), some tests carry multiple `Category` attributes.
- **File opening** — the provided example tests were implemented loosely. Test cases cover several file-open scenarios, both with the application already running and at startup. Rather than opening files through Explorer's "Open With" functionality, the tests start the Edi process with a file path argument, which mimics opening files from the command line.
- **New files** — creating, writing and saving new files is covered by multiple single-concern tests.

## Test Results

Individual test data is stored in the built DLL's folder, organized as:

```
\<TestRunTimeStamp>\<TestClass>\<TestName>\
```

Each test folder contains its logs and, for failed tests, a screenshot.

The overall test run result is saved to the user-defined `ResultsDirectory` in `.html` and `.trx` formats, together with an automatically copied set of the test data that the `.trx` file links to.

To examine the results in Visual Studio, right-click a test and choose **View Test Result Details** to see more information, including the files attached to the test run.

## Known Issues

- Edi's logger does not produce useful logs, even at its most verbose logging settings.
- `AppRun_CreateNewTextWrite_ShownStar` does not handle the "are you sure" popup on exit.
- When an otherwise successful test fails due to an unhandled exception, no screenshot is taken.
- Some helper classes violate clean code principles, as they occasionally contain assertions.
