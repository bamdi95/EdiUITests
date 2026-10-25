using EdiUITests.Utils.Dialogs.OpenDialog;
using EdiUITests.Utils.Dialogs.SaveDialog;
using FlaUI.Core.AutomationElements;

namespace EdiUITests.Utils.Menu
{
    public class MenuBar
    {
        private readonly FlaUI.Core.AutomationElements.Window _mainWindow;
        private readonly FlaUI.Core.AutomationElements.Menu _menu;

        public MenuBar(FlaUI.Core.AutomationElements.Window mainWindow)
        {
            _mainWindow = mainWindow;
            _menu = FlauiHelpers.FindElement(_mainWindow, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Menu), "Menu").AsMenu();
        }

        public void OpenFile(string filePath)
        {
            _menu.Items[MenuBarConstants.FileMenu].Click();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.OpenMenu].Expand();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.OpenMenu].Items[MenuBarConstants.TextFilesMenu].Click();

            new OpenFileDialog(_mainWindow).OpenFile(filePath);
        }

        public void CreateNewTextFile()
        {
            _menu.Items[MenuBarConstants.FileMenu].Click();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.NewMenu].Expand();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.NewMenu].Items[MenuBarConstants.TextDocumentMenu].Click();
        }

        public void SaveFileAs(string filePath)
        {
            _menu.Items[MenuBarConstants.FileMenu].Click();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.SaveAsMenu].Click();
            new SaveFileDialog(_mainWindow).SaveFile(filePath);
        }

        public void SaveNewFile(string filePath)
        {
            _menu.Items[MenuBarConstants.FileMenu].Click();
            _menu.Items[MenuBarConstants.FileMenu].Items[MenuBarConstants.SaveMenu].Click();
            new SaveFileDialog(_mainWindow).SaveFile(filePath);
        }
    }
}