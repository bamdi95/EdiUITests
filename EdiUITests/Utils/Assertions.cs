using EdiUITests.Utils.Editor;

namespace EdiUITests.Utils
{
    public static class Assertions
    {
        public static void AssertTabIsSelected(EditorTabs editorTabs, string name)
        {
            var selectedTabName = editorTabs.GetSelectedTabItemHeader();
            Assert.That(selectedTabName, Is.EqualTo(name), $"The selected tab is '{selectedTabName}' but expected '{name}'");
        }

        public static void AssertContentOfTabIsDisplayed(EditorTabs editorTabs, string name, string expectedContent)
        {
            Assert.That(editorTabs.GetTabItemContentText(name), Is.EqualTo(expectedContent), $"The content of the {name} file does not match the expected content");
        }

        public static void AssertTabIsReadonly(EditorTabs editorTabs, string name)
        {
            Assert.That(editorTabs.IsTabItemContentEditable(name), Is.False, $"The content of the {name} file is editable");
            Assert.That(editorTabs.IsLockSymbolPresentOnTabItem(name), Is.True, $"The lock symbol is not present on the {name} file tab");
        }

        public static void AssertTabIsEditable(EditorTabs editorTabs, string name)
        {
            Assert.That(editorTabs.IsTabItemContentEditable(name), Is.True, $"The content of the {name} file is not editable");
            Assert.That(editorTabs.IsLockSymbolPresentOnTabItem(name), Is.False, $"The lock symbol is present on the {name} file tab");
        }
    }
}
