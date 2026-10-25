using System.Diagnostics;
using System.Text;

namespace EdiUITests.Utils
{
    public class EdiProcess : IDisposable
    {
        public Process Process { get; }
        private readonly StringBuilder _ediStderr = new();
        public string Output => _ediStderr.ToString();

        public string LogFile { get; }

        public EdiProcess(string exePath, string logDir, params string[] args)
        {
            var psi = new ProcessStartInfo(exePath)
            {
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
            };

            LogFile = Path.Combine(logDir, EdiLogConstants.EdiLogFileName);

            psi.Environment[EdiLogConstants.EdiLogFolderEnvironmentVariable] = logDir;

            foreach (var arg in args)
            {
                psi.ArgumentList.Add(arg);
            }
                
            Process = Process.Start(psi)!;
            Process.ErrorDataReceived += (_, e) => { if (e.Data != null) _ediStderr.AppendLine(e.Data); };
            Process.OutputDataReceived += (_, _) => { };
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        public void Dispose()
        {
            try
            {
                if (!Process.HasExited)
                {
                    Process.Kill(entireProcessTree: true);
                }
                    
                Process.WaitForExit();
            }
            catch (InvalidOperationException) { /* already gone */ }
            Process.Dispose();
        }
    }
}