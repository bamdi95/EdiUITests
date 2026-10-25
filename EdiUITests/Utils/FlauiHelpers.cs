using EdiUITests.Tests;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using System.Diagnostics;

namespace EdiUITests.Utils
{
    public static class FlauiHelpers
    {
        public static AutomationElement FindElement(
        AutomationElement parent,
        Func<ConditionFactory, ConditionBase> condition,
        string elementName, int timeoutSeconds = 10)
        {
            var result = Retry.WhileNull(
                () => parent.FindFirstDescendant(condition),
                TimeSpan.FromSeconds(timeoutSeconds));

            if (result.Result == null)
            {
                throw new InvalidOperationException($"Could not find {elementName} within {timeoutSeconds} seconds");
            }

            return result.Result;
        }

        public static IEnumerable<AutomationElement> FindElements(AutomationElement parent, Func<ConditionFactory, ConditionBase> condition, string elementName, int timeoutSeconds = 10)
        {
            var result = Retry.WhileNull(
                () => parent.FindAllDescendants(condition),
                TimeSpan.FromSeconds(timeoutSeconds));

            if (result.Result == null)
            {
                throw new InvalidOperationException($"Could not find {elementName} within {timeoutSeconds} seconds");
            }
            return result.Result;
        }

        public static Application AttachToRunningEdi()
        {
            var process = Retry.WhileNull(
                () => Process.GetProcessesByName(Path.GetFileNameWithoutExtension(SetupFixture.EdiPath)).FirstOrDefault(),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(250)).Result;
            var app = process != null ? Application.Attach(process) : null;
            Assert.That(app, Is.Not.Null, "Could not attach to the Edi application");
            return app;
        }

        public static FlaUI.Core.AutomationElements.Window GetLoadedMainWindow(FlaUI.Core.Application app, UIA3Automation automation)
        {
            app.WaitWhileMainHandleIsMissing(TimeSpan.FromSeconds(15));
            app.WaitWhileBusy(TimeSpan.FromSeconds(15));

            var desktop = automation.GetDesktop();

            var mainWindow = Retry.WhileNull(() =>
            {
                var window = desktop.FindFirstChild(cf =>
                    cf.ByProcessId(app.ProcessId).And(cf.ByControlType(ControlType.Window)))?.AsWindow();
                window?.Focus();

                return window?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Menu)) != null
                    ? window
                    : null;
            },
                TimeSpan.FromSeconds(15),
                TimeSpan.FromMilliseconds(250)).Result;

            Assert.That(mainWindow, Is.Not.Null, "Could not find Edi's loaded main window");
            return mainWindow;
        }
    }
}