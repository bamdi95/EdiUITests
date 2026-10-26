using EdiUITests.Utils;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using System.Diagnostics;
using System.Text;

namespace EdiUITests.Tests
{
    public abstract class TestBase
    {
        protected UIA3Automation Automation = null!;
        protected FlaUI.Core.Application? App;
        protected WorkSpace WorkSpace = null!;

        public static readonly string RunDir =
        Path.Combine(TestContext.CurrentContext.TestDirectory, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));

        private List<EdiProcess> _ediProcesses = new List<EdiProcess>();

        [SetUp]
        public void BaseSetup()
        {
            EdiSettingsHandler.Reset();
            Automation = new UIA3Automation();
            WorkSpace = new WorkSpace();
        }

        [TearDown]
        public void BaseTearDown()
        {
            string ediOutput;
            try
            {
                ScreenShotOnFailure();
                CloseApp();
            }
            finally
            {
                ediOutput = ShutDownAllEdiProcesses();
                Automation.Dispose();
                WorkSpace.Dispose();
            }

            if (ediOutput.Length > 0)
            {
                TestContext.Out.WriteLine("Edi output:\n" + ediOutput);
            }

            Assert.That(ediOutput, Does.Not.Contain(EdiLogConstants.UnhandledException),
                "Edi caught an unhandled exception during this test");
        }

        private string ShutDownAllEdiProcesses()
        {
            var output = new StringBuilder();
            for (int i = 0; i < _ediProcesses.Count; i++)
            {
                var edi = _ediProcesses[i];
                edi.Dispose();
                if (edi.Output.Length > 0)
                {
                    output.AppendLine($"--- launch #{i} ---").Append(edi.Output);
                }
                if (File.Exists(edi.LogFile))
                {
                    TestContext.AddTestAttachment(edi.LogFile, $"Edi log (launch #{i})");
                }
            }
            _ediProcesses.Clear();
            return output.ToString();
        }

        private static void ScreenShotOnFailure()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                try
                {
                    var path = Path.Combine(CurrentTestDir(), "screenshot.png");
                    FlaUI.Core.Capturing.Capture.Screen().ToFile(path);
                    TestContext.AddTestAttachment(path, "Screen at failure");
                }
                catch (Exception ex)
                {
                    TestContext.WriteLine($"Screenshot failed: {ex.Message}");
                }
            }
        }

        private EdiProcess StartEdiProcess(params string[] args)
        {
            var logPath = Path.Combine(CurrentTestDir(), $"launch_{_ediProcesses.Count}");
            var process = new EdiProcess(SetupFixture.EdiPath, logPath, args);
            _ediProcesses.Add(process);

            return process;
        }

        protected Window LaunchEdi(params string[] args)
        {
            var edi = StartEdiProcess(args);
            App = Application.Attach(edi.Process);
            return FlauiHelpers.GetLoadedMainWindow(App, Automation);
        }

        protected void OpenFileViaCommandLine(string filePath, int timeoutSeconds = 10)
        {
            Assert.That(App, Is.Not.Null, "Edi must already be running");

            var edi = StartEdiProcess(filePath);
            Assert.That(edi.Process.WaitForExit(timeoutSeconds * 1000), Is.True,
                "The second Edi process did not exit — it may have opened its own window");
        }

        private void CloseApp()
        {
            App?.Close();
            App?.Dispose();
            App = null;
        }

        protected void CloseAppGracefully(Window mainWindow, int timeoutSeconds = 10)
        {
            Assert.That(App, Is.Not.Null, "No running app to close");
            var process = Process.GetProcessById(App!.ProcessId);

            mainWindow.Close();

            Assert.That(process.WaitForExit(timeoutSeconds * 1000), Is.True,
                "Edi did not exit in time");

            App.Dispose();
            App = null;
        }

        private static string CurrentTestDir()
        {
            var test = TestContext.CurrentContext.Test;
            var className = test.ClassName?.Split('.').Last() ?? "Unknown";
            var dir = Path.Combine(RunDir, className, test.MethodName ?? test.Name);
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}