using System.IO;

namespace MinecraftServerHub.Packet
{
    class DisconnectPacket : Packet
    {
        public DisconnectPacket(ParameterParser parser, string filepath)
        {
            if (File.Exists(filepath))
                Reason = parser.Parse(new FileInfo(filepath));
            else
                Reason = "";
        }

        public string Reason { get; set; }

        protected override void Write()
        {
            WriteString(Reason);
        }
    }
}
