using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;

namespace EdiUITests.Utils.Editor
{
    public class EditorTabs
    {
        private readonly Window _mainWindow;
        private IEnumerable<TabItem> _tabItems;

        public EditorTabs(Window mainWindow, string specificTabItemToWaitFor)
        {
            _mainWindow = mainWindow;
            var tabFound = Retry.WhileNull(
                () =>
                {
                    var tabItems = FlauiHelpers.FindElements(mainWindow, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.TabItem), "tabItem").Select(e => e.AsTabItem());
                    return tabItems.FirstOrDefault(t => t.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))?.AsLabel().Text == specificTabItemToWaitFor);
                },
                TimeSpan.FromSeconds(10),
                TimeSpan.FromMilliseconds(250)).Result;
            Assert.That(tabFound, Is.Not.Null, $"Could not find the tab item with header '{specificTabItemToWaitFor}'");
            _tabItems = FlauiHelpers.FindElements(mainWindow, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.TabItem), "tabItem").Select(e => e.AsTabItem());
        }

        public string GetTabItemContentText(string tabItemHeader)
        {
            return GetEditor(tabItemHeader).Patterns.Value.Pattern.Value;
        }

        public bool IsTabItemContentEditable(string tabItemHeader)
        {
            return GetEditor(tabItemHeader).Patterns.Value.Pattern.IsReadOnly == false;
        }

        public bool IsLockSymbolPresentOnTabItem(string tabItemHeader)
        {
            var lockSymbol = Retry.WhileNull(
                () => GetTabItem(tabItemHeader)?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Image)),
                TimeSpan.FromSeconds(5)).Result;
            if (lockSymbol == null)
            {
                throw new InvalidOperationException("Could not find the everpresent image element on the tab item");
            }
            return !lockSymbol.IsOffscreen;
        }

        public bool IsTabItemSelected(string tabItemHeader)
        {
            var tabItem = GetTabItem(tabItemHeader);
            if (tabItem == null)
            {
                throw new InvalidOperationException($"Could not find the tab item with header '{tabItemHeader}'");
            }
            return !FlauiHelpers.FindElement(tabItem, cf => cf.ByAutomationId("DocumentCloseButton"), "x button on tab").AsButton().IsOffscreen;
        }

        private TabItem? GetTabItem(string tabItemHeader)
        {
            return _tabItems.FirstOrDefault(t => t.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))?.AsLabel().Text == tabItemHeader);
        }

        public AutomationElement? TryGetEditor(string tabItemHeader, int timeoutSeconds = 5)
        {
            return Retry.WhileNull(
                () => GetTabItem(tabItemHeader)?.FindAllDescendants(cf => cf.ByControlType(ControlType.Custom))
                 .FirstOrDefault(e => e.Patterns.Value.IsSupported),
                    TimeSpan.FromSeconds(timeoutSeconds)).Result;
        }

        private AutomationElement GetEditor(string tabItemHeader) =>
            TryGetEditor(tabItemHeader)
            ?? throw new InvalidOperationException($"No editor control found in tab '{tabItemHeader}'");

        public void WriteToEditor(string content, string tabItemHeader)
        {
            GetEditor(tabItemHeader).Click();
            Keyboard.Type(content);
            Wait.UntilInputIsProcessed();
        }

        public string? GetSelectedTabItemHeader()
        {
            var tabItem = _tabItems.FirstOrDefault(t => t.FindFirstDescendant(cf => cf.ByAutomationId("DocumentCloseButton"))?.AsButton().IsOffscreen == false);
            return tabItem?.FindFirstDescendant(cf => cf.ByControlType(ControlType.Text))?.AsLabel().Text;
        }
    }
}