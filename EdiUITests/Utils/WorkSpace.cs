namespace EdiUITests.Utils
{
    public class WorkSpace : IDisposable
    {
        public string RootPath { get; private set; }

        public WorkSpace()
        {
            RootPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(RootPath);
        }

        public string CopyFileToWorkspace(string sourceFilePath, bool isReadOnly = false, string? newName = null)
        {
            var fileName = newName ?? Path.GetFileName(sourceFilePath);
            var destinationFilePath = Path.Combine(RootPath, fileName);
            File.Copy(sourceFilePath, destinationFilePath);
            if (isReadOnly)
            {
                File.SetAttributes(destinationFilePath, FileAttributes.ReadOnly);
            }
            Assert.True(File.Exists(destinationFilePath), $"Failed to copy {fileName} test file to workspace");
            Assert.That(new FileInfo(destinationFilePath).IsReadOnly, Is.EqualTo(isReadOnly), $"Failed to set read-only = {isReadOnly} on {fileName}");
            return destinationFilePath;
        }

        public void Dispose()
        {
            try
            {
                if (!Directory.Exists(RootPath))
                    return;

                foreach (var file in Directory.EnumerateFiles(RootPath, "*", SearchOption.AllDirectories))
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                }
                Directory.Delete(RootPath, true);
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
            }
        }
    }
}