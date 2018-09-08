using System;
using System.Threading;

namespace MinecraftServerHub.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new StdOutLog();
            try
            {
                log.WriteInfo("Starting...");
                var hub = new ServerHub(ServiceSettings.Settings, log);
                AppDomain.CurrentDomain.ProcessExit += (sender, e) => { hub.Stop(); Thread.Sleep(1000); };
                hub.Start();
                log.WriteInfo("Successfully started!");
                hub.Task.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                log.WriteError(e.Message);
            }
        }
    }
}
