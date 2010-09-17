using System;
using System.Diagnostics;
using System.IO;

namespace Bounce {
    class ShellCommandExecutor {
        public void ExecuteProcess(string commandName, string commandArgs) {
            var processInfo = new ProcessStartInfo(commandName, commandArgs);
            processInfo.CreateNoWindow = true;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.UseShellExecute = false;

            var p = new Process { StartInfo = processInfo };

            var output = new CommandOutputReceiver();

            p.ErrorDataReceived += output.ErrorDataReceived;
            p.OutputDataReceived += output.OutputDataReceived;
            p.Start();

            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();
            if (p.ExitCode != 0) {
                throw new BuildException(String.Format("running: {0} {1}\nin: {2}\nexited with {3}", commandName, commandArgs, Directory.GetCurrentDirectory(), p.ExitCode), output.Output);
            }
        }
    }
}