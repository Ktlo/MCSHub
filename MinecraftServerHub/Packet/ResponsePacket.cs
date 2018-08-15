using System.IO;

namespace MinecraftServerHub.Packet
{
    // Server
    class ResponsePacket : Packet
    {
        public string JSON { get; set; }

        public ResponsePacket(ParameterParser parser, string filepath)
        {
            ID = 0;
            if (File.Exists(filepath))
                JSON = parser.Parse(new FileInfo(filepath));
            else
                JSON = "";
        }

        protected override void Write()
        {
            WriteString(JSON);
        }

    }
}
