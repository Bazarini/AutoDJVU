using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Security;
using System.ComponentModel;
using System.Collections.Generic;
using EventLogger;

namespace PDFToDJVU
{
    public class Executor
    {

        public static bool NewPrepareDJVU(string inputDocument, string output, CancellationToken token, int dpi = 250)
        {
            string args = $"-o \"{output}\" --page-id-template=nb" + "{dpage:04*}.djvu" + $" \"{inputDocument}\"";
            string exe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase), @"Binaries\pdf2djvu.exe").Replace("file:\\", "");
            string tempFolder =
                Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase),
                    @"Temp")
                .Replace("file:\\", "");
            Process process = new Process { StartInfo = { FileName = exe, Arguments = args, UseShellExecute = false } };
            Action OnCancel = delegate { process.Kill(); };
            var registration = token.Register(OnCancel);
            process.Exited += (sender, e) => { registration.Dispose(); };
            process.Start();
            while (!process.HasExited && !token.IsCancellationRequested)
            {
                EventLogger.EventLogger.LogMessage($"Creating file {output}", EventLogEntryType.Information);
                process.WaitForExit(10000);
            }
            if (token.IsCancellationRequested)
            {
                EventLogger.EventLogger.LogMessage($"Creation file {output} is cancelled.", EventLogEntryType.Warning);

                return false;
            }
            if (File.Exists(output) && new FileInfo(output).Length > 0)
            {
                EventLogger.EventLogger.LogMessage($"File {output} was created.", EventLogEntryType.Information);

                return true;
            }
            EventLogger.EventLogger.LogMessage($"File {output} was not created.", EventLogEntryType.Error);
            return false;
        }
    }
}