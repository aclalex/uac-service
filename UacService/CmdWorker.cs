namespace UacService
{
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public class CmdWorker : BackgroundService
    {
        private readonly IConfiguration Configuration;

        public CmdWorker(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var pooling = Int32.Parse(Configuration["PoolingInSeconds"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var cmd = new Process())
                {
                    var startInfo = new ProcessStartInfo
                    {
                        CreateNoWindow = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe"
                    };
                    cmd.StartInfo = startInfo;
                    cmd.Start();

                    await using (var sw = cmd.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            await sw.WriteLineAsync(@"%windir%\System32\reg.exe ADD  HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v ConsentPromptBehaviorAdmin /t REG_DWORD /d 2 /f").ConfigureAwait(true);
                            await sw.WriteLineAsync(@"%windir%\System32\reg.exe ADD  HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System /v  PromptOnSecureDesktop /t REG_DWORD /d 1 /f").ConfigureAwait(true);
                        }
                        else
                        {
                            throw new ApplicationException("Cannot write");
                        }
                    }
                    cmd.WaitForExit();
                }

                await Task.Delay(pooling  * 1000, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}
