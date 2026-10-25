using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;

namespace EdiUITests.Utils.Dialogs.OpenDialog
{
    public class OpenFileDialog
    {
        private readonly FlaUI.Core.AutomationElements.Window _openDialog;
        public OpenFileDialog(FlaUI.Core.AutomationElements.Window mainWindow)
        {
            _openDialog = FlauiHelpers.FindElement(mainWindow, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window), "Open dialog").AsWindow();
        }

        public void OpenFile(string fullFilePath)
        {
            var fileNameComboBox = FlauiHelpers.FindElement(_openDialog, cf => cf.ByAutomationId(OpenFileDialogConstants.FileNameComboBoxAutomationId), "Open dialog file name").AsComboBox();
            var fileNameTextBox = FlauiHelpers.FindElement(fileNameComboBox, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit), "Open dialog file name editable").AsTextBox();
            fileNameTextBox.Text = fullFilePath;
            Keyboard.Press(FlaUI.Core.WindowsAPI.VirtualKeyShort.ENTER);
        }
    }
}