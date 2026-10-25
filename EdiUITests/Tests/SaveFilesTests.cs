using EdiUITests.Utils;
using EdiUITests.Utils.Editor;
using EdiUITests.Utils.Menu;

namespace EdiUITests.Tests
{
    public class SaveFilesTests : TestBase
    {
        private FlaUI.Core.AutomationElements.Window _mainWindow;

        [SetUp]
        public void Setup()
        {
            _mainWindow = LaunchEdi();
        }

        [Test]
        [Category("File New")]
        [Description("Given app is running, when creating a new text file, then a new 'Untitled.txt' tab is displayed and editable")]
        public void AppRun_CreateNewText_ShownEditable()
        {
            var menuBar = new MenuBar(_mainWindow);

            CreateAndAssertNewTxtFileCreation(menuBar);
        }

        [Test]
        [Category("File New")]
        [Category("File Save As")]
        [Description("Given app is running, when creating a new text file and saving it, then the file is created and empty")]
        public void AppRun_CreateNewTextSave_Saved()
        {
            var menuBar = new MenuBar(_mainWindow);
            var editorTabs = CreateAndAssertNewTxtFileCreation(menuBar);

            var newFullPath = Path.Combine(WorkSpace.RootPath, TextConstants.TempTxtSaveName);
            menuBar.SaveFileAs(newFullPath);

            editorTabs = new EditorTabs(_mainWindow, TextConstants.TempTxtSaveName);

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(newFullPath), Is.True, $"The file '{newFullPath}' was not created");
                Assert.That(File.ReadAllText(newFullPath), Is.EqualTo(string.Empty), $"The content of the saved file '{newFullPath}' does not match the expected content");
            });
        }

        [Test]
        [Category("File New")]
        [Description("Given app is running, when creating a new text file and editing it, then a star icon is displayed on the tab")]
        public void AppRun_CreateNewTextWrite_ShownStar()
        {
            var menuBar = new MenuBar(_mainWindow);
            var editorTabs = CreateAndAssertNewTxtFileCreation(menuBar);
            editorTabs.WriteToEditor(TextConstants.TempTxtSaveContent, TextConstants.NewTxtDefaultFileName);

            editorTabs = new EditorTabs(_mainWindow, TextConstants.NewTxtDefaultFileName+ "*");
        }

        [Test]
        [Category("File New")]
        [Category("File Save")]
        [Description("Given app is running, when creating a new text file, writing content and saving it, then the file is created and content is saved")]
        public void AppRun_CreateNewTextWriteSave_Saved()
        {
            var menuBar = new MenuBar(_mainWindow);
            var editorTabs = CreateAndAssertNewTxtFileCreation(menuBar);

            editorTabs.WriteToEditor(TextConstants.TempTxtSaveContent, TextConstants.NewTxtDefaultFileName);

            var newFullPath = Path.Combine(WorkSpace.RootPath, TextConstants.TempTxtSaveName);
            menuBar.SaveNewFile(newFullPath);

            editorTabs = new EditorTabs(_mainWindow, TextConstants.TempTxtSaveName);

            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(newFullPath), Is.True, $"The file {newFullPath} was not created");
                Assert.That(File.ReadAllText(newFullPath), Is.EqualTo(TextConstants.TempTxtSaveContent), "The content of the saved file does not match the expected content");
            });
        }

        private EditorTabs CreateAndAssertNewTxtFileCreation(MenuBar menuBar)
        {
            menuBar.CreateNewTextFile();
            var editorTabs = new EditorTabs(_mainWindow, TextConstants.NewTxtDefaultFileName);

            Assertions.AssertTabIsSelected(editorTabs, TextConstants.NewTxtDefaultFileName);
            Assertions.AssertTabIsEditable(editorTabs, TextConstants.NewTxtDefaultFileName);
            return editorTabs;
        }
    }
}