using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.Core.WindowsAPI;

namespace EdiUITests.Utils.Dialogs.SaveDialog
{
    public class SaveFileDialog
    {
        private readonly FlaUI.Core.AutomationElements.Window _saveDialog;
        public SaveFileDialog(FlaUI.Core.AutomationElements.Window mainWindow)
        {
            _saveDialog = FlauiHelpers.FindElement(mainWindow, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window), "Save dialog").AsWindow();
        }

        public void SaveFile(string fullFilePath)
        {
            var fileNameComboBox = FlauiHelpers.FindElement(_saveDialog, cf => cf.ByAutomationId(SaveFileDialogConstants.FileNameComboBoxAutomationId), "Save dialog file name").AsComboBox();
            var fileNameTextBox = FlauiHelpers.FindElement(fileNameComboBox, cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit), "Save dialog file name editable").AsTextBox();
            fileNameTextBox.Focus();
            Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A);
            Keyboard.Type(fullFilePath);

            FlauiHelpers.FindElement(_saveDialog, cf => cf.ByAutomationId(SaveFileDialogConstants.SaveButtonAutomationId)
                                           .And(cf.ByControlType(ControlType.Button)), "Save button")
                .AsButton().Invoke();
            var saved = Retry.WhileFalse(() => File.Exists(fullFilePath), TimeSpan.FromSeconds(10)).Result;
            Assert.That(saved, Is.True, $"File was not saved to {fullFilePath}");
        }
    }
}