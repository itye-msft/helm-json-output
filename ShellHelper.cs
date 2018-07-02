using System.Diagnostics;
using System;

namespace jsonplugin
{
 /// A small helper class to execute helm from shell
 public static class ShellHelper
    {
        public static string Helm(this string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.GetEnvironmentVariable("HELM_BIN"),// this is injected when helm is executed
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}