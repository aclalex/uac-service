using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace UacService
{
    using System.IO;

    public class CmdWorker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe"
                };
                cmd.StartInfo = startInfo;
                cmd.Start();

                await using (StreamWriter sw = cmd.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine(@"%windir%\System32\reg.exe ADD  HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v ConsentPromptBehaviorAdmin /t REG_DWORD /d 2 /f");
                        sw.WriteLine(@"%windir%\System32\reg.exe ADD  HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v  PromptOnSecureDesktop /t REG_DWORD /d 1 /f");
                    }
                    else
                        throw new ApplicationException("Cannot write");
                }
                cmd.WaitForExit();
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
