using System;
using System.IO;
using System.Net;

namespace MinecraftServerHub.Packet
{
    class LoginStartPacket : Packet
    {
        public string PlayerName;

        public LoginStartPacket(Stream stream) : base(stream)
        {
        }

        protected override void Read()
        {
            PlayerName = ReadString();
        }

        protected override void Write()
        {
            WriteString(PlayerName);
        }
    }
}
