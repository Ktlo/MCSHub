using System.IO;

namespace MinecraftServerHub.Packet
{
    public class PacketSelector
    {
        internal Stream stream;
        internal int size;
        internal int id;

        public int ID { get => id; }
        public int Size { get => size; }

        public PacketSelector(Stream stream)
        {
            this.stream = stream;
        }

        public void Catch()
        {
            size = Packet.ReadVarInt(stream, out int n);
            id = Packet.ReadVarInt(stream, out n);
        }
    }
}
