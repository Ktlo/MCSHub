using System;
using System.IO;

namespace MinecraftServerHub.Packet
{
    class PingPongPacket : Packet
    {
        public long Payload { get; set; }

        public PingPongPacket()
        {
            ID = 1;
            Payload = DateTime.Now.Ticks;
        }

        public PingPongPacket(Stream stream) : base(stream)
        {

        }

        public PingPongPacket(PacketSelector selector) : base(selector)
        {

        }

        protected override void Read()
        {
            Payload = ReadLong();
        }

        protected override void Write()
        {
            WriteLong(Payload);
        }
    }
}
