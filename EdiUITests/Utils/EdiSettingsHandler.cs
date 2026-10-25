namespace EdiUITests.Utils
{
    public static class EdiSettingsHandler
    {
        private static readonly string ProfileDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Edi");

        public static void Reset()
        {
            if (Directory.Exists(ProfileDir))
                Directory.Delete(ProfileDir, recursive: true);
        }
    }
}