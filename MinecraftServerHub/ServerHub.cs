using MinecraftServerHub.Packet;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MinecraftServerHub
{
    class ServerHub
    {
        private readonly ServiceSettings settings;
        private TcpListener listener;
        private bool running;
        private readonly SystemLog log;


        private Dictionary<string, ServerShadow> servers;
        private readonly ServerShadow standard;

        internal static IPAddress GetIPAddress(string hostname)
        {
            var ok = IPAddress.TryParse(hostname, out IPAddress ip);
            if (!ok)
            {
                var entry = Dns.GetHostEntry(hostname);
                ip = entry.AddressList[0];
            }
            return ip;
        }

        public ServerHub(ServiceSettings settings, SystemLog log)
        {
            this.log = log;
            this.settings = settings;
            listener = new TcpListener(GetIPAddress(settings.Address), settings.Port);
            running = false;
            servers = new Dictionary<string, ServerShadow>(settings.Records.Count);
            foreach (var record in settings.Records)
            {
                servers.Add(record.Mask, new ServerShadow(record, log));
            }
            standard = new ServerShadow(settings.Default, log);
        }

        private class StoppedTask : TaskCompletionSource<object>
        {
            public void Stop()
            {
                SetResult(null);
            }
        }

        StoppedTask task;

        public Task Task => task.Task;

        public SystemLog Log => log;

        public void Start()
        {
            running = true;
            task = new StoppedTask();
            listener.Start();
            Task.Run(new Action(Background));
        }

        public void Stop()
        {
            running = false;
            task.Stop();
        }

        private void Background()
        {
            var stopTask = task.Task;
            while (running)
            {
                var clientTask = listener.AcceptTcpClientAsync();
                if (Task.WaitAny(stopTask, clientTask) == 0) {
                    task = null;
                    return;
                }
                var client = clientTask.Result;
                Task.Run(() =>
                {
                    try
                    {
                        var stream = client.GetStream();
                        var sel = new PacketSelector(stream);
                        sel.Catch();
                        if (sel.ID != 0)
                            return;
                        var endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
                        var handshake = new HandshakePacket(sel);
                        ServerShadow server;
                        if (servers.ContainsKey(handshake.Address))
                            server = servers[handshake.Address];
                        else
                            server = standard;
                        server.NewConnetcion(handshake, endPoint, stopTask);
                        stream.Close();
                    }
                    catch (Exception exception)
                    {
                        Log.WriteWarn(exception.Message);
                    }
                    finally
                    {
                        client.Close();
                    }
                });
            }
        }
    }
}
