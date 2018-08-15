using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MinecraftServerHub.Packet
{
    abstract public class Packet
    {
        private int size;
        protected int ID { get; set; }
        private Stream stream;
        private List<byte> buffer;
        private int resultSize;

        public int Size { get => size; }

        public Packet(Stream stream)
        {
            this.stream = stream;
            size = ReadVarInt();
            resultSize = 0;
            ID = ReadVarInt();
            Read();
            if (size != resultSize)
                throw new IOException($"Packet size differs ({size} expected, got {resultSize})");
        }

        public Packet(PacketSelector selector)
        {
            stream = selector.stream;
            size = selector.size;
            ID = selector.id;
            resultSize = 1;
            Read();
            if (size != resultSize)
                throw new IOException($"Packet size differs ({size} expected, got {resultSize})");
        }

        public Packet() { }

        public void Send(Stream stream)
        {
            buffer = new List<byte>();
            byte[] data = new byte[5];
            WriteVarInt(ID);
            Write();
            size = buffer.Count;
            int length = WriteVarIntInternal(data, size);
            stream.Write(data, 0, length);
            stream.Write(buffer.ToArray(), 0, size);
        }

        protected virtual void Read() { }
        protected virtual void Write() { }

        internal static int ReadVarInt(Stream stream, out int numRead)
        {
            numRead = 0;
            int result = 0;
            int read;
            do
            {
                read = stream.ReadByte();
                int value = (read & 0b01111111);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                {
                    throw new IOException("VarInt is too big");
                }
            } while ((read & 0b10000000) != 0);

            return result;
        }

        protected int ReadVarInt()
        {
            int result = ReadVarInt(stream, out int read);
            resultSize += read;
            return result;
        }

        protected string ReadString()
        {
            int stringSize = ReadVarInt();
            byte[] buffer = new byte[stringSize];
            stream.Read(buffer, 0, stringSize);
            resultSize += stringSize;
            return Encoding.UTF8.GetString(buffer);
        }

        protected ushort ReadUInt16()
        {
            resultSize += 2;
            return (ushort)(stream.ReadByte() << 8 | stream.ReadByte());
        }

        protected long ReadLong()
        {
            byte[] bytes = new byte[8];
            stream.Read(bytes, 0, 8);
            resultSize += 8;
            return BitConverter.ToInt64(bytes, 0);
        }

        private int WriteVarIntInternal(byte[] data, int value)
        {
            int i = 0;
            do
            {
                byte temp = (byte)(value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                data[i] = temp;
                i++;
            } while (value != 0);
            return i;
        }

        protected void WriteVarInt(int value)
        {
            do
            {
                byte temp = (byte)(value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                buffer.Add(temp);
            } while (value != 0);
        }

        protected void WriteString(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            WriteVarInt(bytes.Length);
            buffer.AddRange(bytes);
        }

        protected void WriteUInt16(ushort value)
        {
            buffer.Add((byte)(value >> 8));
            buffer.Add((byte)(value & 255));
        }

        protected void WriteLong(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            buffer.AddRange(bytes);
        }

        internal int GetID() => ID;
    }
}
