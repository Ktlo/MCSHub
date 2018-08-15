using MinecraftServerHub.Packet;
using MinecraftServerHub.ParamGroup;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MinecraftServerHub
{

    public class ServerShadow
    {
        readonly string pseudoAddress;
        IPEndPoint destination;
        ServerRecord record;
        readonly IParameterGroup setsGroup;
        readonly SystemLog log;

        public ServerShadow(ServerRecord record, SystemLog stdLog)
        {
            if (record.LogPath != null)
            {
                if (record.LogPath == ":std:")
                    log = stdLog;
                else
                    log = new FileLog(record.LogPath);
            }
            pseudoAddress = record.Mask;
            if (record.Address == null)
                destination = null;
            else
                destination = new IPEndPoint(ServerHub.GetIPAddress(record.Address), record.Port);
            this.record = record;
            setsGroup = new SetsGroup(record.Sets);
        }

        public void NewConnetcion(HandshakePacket handshake, IPEndPoint endPoint, Task stopTask) {
            var stream = handshake.Stream;
            Task connection = null;
            TcpClient client = null;
            if (destination != null)
            {
                client = new TcpClient();
                connection = client.ConnectAsync(destination.Address, destination.Port);
            }
            var parser = new ParameterParser(new KeyValuePair<string, IParameterGroup>[]
            {
                new KeyValuePair<string, IParameterGroup>("hs", handshake),
                new KeyValuePair<string, IParameterGroup>("main", MainGroup.Instance),
                new KeyValuePair<string, IParameterGroup>("file", FileGroup.Instance),
                new KeyValuePair<string, IParameterGroup>("env", EnvGroup.Instance),
                new KeyValuePair<string, IParameterGroup>("sets", setsGroup)
            });
            if (connection != null)
            {
                try
                {
                    connection.Wait();
                    var realStream = client.GetStream();
                    handshake.Port = (ushort)destination.Port;
                    handshake.Send(realStream);
                    if (handshake.State == 2)
                    {
                        var loginPacket = new LoginStartPacket(stream);
                        loginPacket.Send(realStream);
                        log.WriteInfo($"Player '{loginPacket.PlayerName}' successfully login from {endPoint} to {record.Mask}.");
                    }
                    Task task1 = realStream.CopyToAsync(stream);
                    Task task2 = stream.CopyToAsync(realStream);
                    switch (Task.WaitAny(task1, task2, stopTask))
                    {
                        case 0:
                            Task.WaitAny(task2, stopTask);
                            break;
                        case 1:
                            Task.WaitAny(task1, stopTask);
                            break;
                        case 2:
                        default:
                            break;
                    }
                    realStream.Close();
                    client.Close();
                    return;
                }
                catch (AggregateException) { }
            }
            var selector = new PacketSelector(stream);
            switch (handshake.State)
            {
                case 1:
                    selector.Catch();
                    new RequestPacket(selector);
                    var response = new ResponsePacket(parser, record.StatusPath);
                    response.Send(stream);
                    selector.Catch();
                    if (selector.ID == 1 && selector.Size == 9)
                    {
                        new PingPongPacket(selector).Send(stream);
                    }
                    break;
                case 2:
                    var loginPacket = new LoginStartPacket(stream);
                    new DisconnectPacket(parser, record.LoginPath).Send(stream);
                    log.WriteInfo($"Player '{loginPacket.PlayerName}' tried to login from {endPoint} to {record.Mask}.");
                    break;
                default:
                    break;
            }
        }
    }
}
