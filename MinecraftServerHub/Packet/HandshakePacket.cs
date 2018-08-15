using MinecraftServerHub.ParamGroup;
using System.IO;

namespace MinecraftServerHub.Packet
{
    // Client
    public class HandshakePacket : Packet, IParameterGroup
    {
        public int Protocol { get; set; }
        public string Address { get; set; }
        public ushort Port { get; set; }
        public int State { get; set; }
        public Stream Stream { get; }

        public HandshakePacket(Stream stream) : base(stream)
        {
            Stream = stream;
        }

        public HandshakePacket(PacketSelector selector) : base(selector)
        {
            Stream = selector.stream;
        }

        protected override void Read()
        {
            Protocol = ReadVarInt();
            Address = ReadString();
            Port = ReadUInt16();
            State = ReadVarInt();
        }

        protected override void Write()
        {
            WriteVarInt(Protocol);
            WriteString(Address);
            WriteUInt16(Port);
            WriteVarInt(State);
        }

        public override string ToString()
        {
            return $"Handshake: ID - {ID}, version - {Protocol}, end point - {Address}:{Port}, state - {State}";
        }

        public string GetValueFor(string key)
        {
            switch (key.ToLower())
            {
                case "protocol":
                    return Protocol.ToString();
                case "address":
                    return Address;
                case "port":
                    return Port.ToString();
                case "state":
                    return State.ToString();
                case "this":
                    return ToString();
                default:
                    return null;
            }
        }
    }
}
