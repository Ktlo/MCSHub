using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.ServiceProcess;

namespace MinecraftServerHub.NTService
{
    [RunInstaller(true)]
    public partial class MinecraftServerHubInstaller : System.Configuration.Install.Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public MinecraftServerHubInstaller()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller
            {
                StartType = ServiceStartMode.Manual,
                ServiceName = "MCSHub",
                DisplayName = "Minecraft Server Hub",
                Description = "This service acts like Minecraft Server. "
                + "Different DNS names connects player to different Minecraft servers. "
                + "So it is possible to keep several servers on exact the same TCP socket."
            };
            processInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            };
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"MinecraftServerHub\");
            Directory.CreateDirectory(directory);
            string config = Path.Combine(directory, "config.json");
            if (!File.Exists(config))
            {
                File.WriteAllText(config, @"
{
  ""address"": ""0.0.0.0"",
  ""port"": 25565,
  ""default"": {
    ""login"": ""Default/Login.json"",
    ""status"": ""Default/Status.json"",
    ""log"": ""Default/Log.txt""
  },
  ""servers"": [
    {
      ""address"": ""123.123.123.123"",
      ""port"": 25565,
      ""mask"": ""localhost"",
      ""status"": ""Example/Status.json"",
      ""login"": ""Example/Login.json"",
      ""log"": ""Example/Log.txt"",
      ""sets"": [
        {
          ""Key"": ""name"",
          ""Value"": ""Minecraft Server Hub Example""
        }
      ]
    }
  ]
}
");
            }
            {
                string defaultDir = Path.Combine(directory, "Default");
                Directory.CreateDirectory(defaultDir);
                string status = Path.Combine(defaultDir, "Status.json");
                if (!File.Exists(status))
                {
                    File.WriteAllText(status, @"
{
  ""version"": {
    ""name"": ""not exists"",
    ""protocol"": 3
                },
  ""players"": {
    ""max"": 0,
    ""online"": 0,
    ""sample"": []
  },
  ""description"": {
    ""text"": ""Server ${hs:address}:${hs:port} doesn't exist!"",
    ""color"": ""red""
  }
}
");
                }
                string login = Path.Combine(defaultDir, "Login.json");
                if (!File.Exists(login))
                {
                    File.WriteAllText(login, @"
[
  {
    ""text"": ""Server ${hs:address}:${hs:port} doesn't exist!"",
    ""color"": ""red""
  }
]
");
                }
            }
            {
                string exampleDir = Path.Combine(directory, "Example");
                Directory.CreateDirectory(exampleDir);
                string status = Path.Combine(exampleDir, "Status.json");
                if (!File.Exists(status))
                {
                    File.WriteAllText(status, @"
{
  ""version"": {
    ""name"": ""MCSHub"",
    ""protocol"": ${ hs:protocol }
                },
  ""players"": {
    ""max"": 5,
    ""online"": 1,
    ""sample"": [
      {
        ""name"": ""Herobrine"",
        ""id"": ""$uuid""
      }
    ]
  },
  ""description"": {
    ""text"": ""${ sets:name }"",
    ""color"": ""gold""
  },
  ""favicon"": ""${ file:Example/Icon.txt }""
}
");
                }
                string login = Path.Combine(exampleDir, "Login.json");
                if (!File.Exists(login))
                {
                    File.WriteAllText(login, @"
[
  {
    ""text"": ""This server is currently down.""
  }
]
");
                }
                string icon = Path.Combine(exampleDir, "Icon.txt");
                if (!File.Exists(login))
                {
                    File.WriteAllText(login, @"data:image/png;base64,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");
                }
            }
        }

        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            string directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"MinecraftServerHub\");
            Directory.Delete(directory, true);
        }
    }
}
