using EdiUITests.Resources;
using EdiUITests.Utils;
using EdiUITests.Utils.Editor;
using EdiUITests.Utils.Menu;
using FlaUI.Core.AutomationElements;

namespace EdiUITests.Tests
{
    public class StartApplicationTests : TestBase
    {
        [Test]
        [Category("Start Application")]
        [Description("Given app is not running, when starting the app, then the main window is loaded")]
        public void AppNotRun_StartApp_AppRunWindowLoaded()
        {
            App = FlaUI.Core.Application.Launch(SetupFixture.EdiPath);
            FlauiHelpers.GetLoadedMainWindow(App, Automation);
        }

        [Test]
        [Category("File Open")]
        [Category("Start Application")]
        [Description("Given app is not running, when opening a read-only text file through the command line argument, then the content is displayed and not editable")]
        public void AppNotRun_OpenReadOnlyTextViaArg_ShownReadOnly()
        {
            var filePath = WorkSpace.CopyFileToWorkspace(ResourceFilePaths.NotEmptyTxt, true);
            var fileName = Path.GetFileName(filePath);

            Window mainWindow = LaunchEdi(filePath);

            var editorTabs = new EditorTabs(mainWindow, fileName);
            Assertions.AssertTabIsSelected(editorTabs, fileName);

            Assert.Multiple(() =>
            {
                Assertions.AssertContentOfTabIsDisplayed(editorTabs, fileName, File.ReadAllText(filePath));
                Assertions.AssertTabIsReadonly(editorTabs, fileName);
            });
        }

        [Test]
        [Category("File Open")]
        [Category("Start Application")]
        [Description("Given app is not running and recent files contains a file, when opening a read-only text file through the command line argument, then the content is displayed and not editable")]
        public void AppNotRunRecentFile_OpenReadOnlyTextViaArg_ShownReadOnly()
        {
            var filePath = WorkSpace.CopyFileToWorkspace(ResourceFilePaths.NotEmptyTxt, true);
            var otherFilePath = WorkSpace.CopyFileToWorkspace(ResourceFilePaths.NotEmptyTxt, true, "other.txt");
            var otherFileName = Path.GetFileName(otherFilePath);

            var window = LaunchEdi();
            new MenuBar(window).OpenFile(filePath);

            CloseAppGracefully(window);

            Window mainWindow = LaunchEdi(otherFilePath);

            var editorTabs = new EditorTabs(mainWindow, otherFileName);

            Assertions.AssertTabIsSelected(editorTabs, otherFileName);

            Assert.Multiple(() =>
            {
                Assertions.AssertContentOfTabIsDisplayed(editorTabs, otherFileName, File.ReadAllText(otherFilePath));
                Assertions.AssertTabIsReadonly(editorTabs, otherFileName);
            });
        }
    }
}