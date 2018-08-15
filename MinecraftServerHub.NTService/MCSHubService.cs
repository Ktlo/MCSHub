using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace MinecraftServerHub.NTService
{
    public partial class MCSHubService : ServiceBase
    {
        ServerHub hub;

        public MCSHubService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length > 0)
                Environment.CurrentDirectory = args[0];
            else
            {
                string directory = Environment.ExpandEnvironmentVariables(@"%ALLUSERSPROFILE%\MinecraftServerHub\");
                Directory.CreateDirectory(directory);
                Environment.CurrentDirectory = directory;
            }
            hub = new ServerHub(ServiceSettings.Settings, new WindowsLog(eventLog));
            hub.Start();
        }

        protected override void OnStop()
        {
            hub.Stop();
            Thread.Sleep(1000);
        }
    }
}
