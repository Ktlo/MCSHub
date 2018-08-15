using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MinecraftServerHub
{
    [DataContract]
    public class ServerRecord
    {
        [DataMember(Name = "address", IsRequired = false)]
        public string Address { get; set; }
        [DataMember(Name = "port", IsRequired = false)]
        public ushort Port { get; set; }
        [DataMember(Name = "mask", IsRequired = false)]
        public string Mask { get; set; }
        [DataMember(Name = "status", IsRequired = true)]
        public string StatusPath { get; set; }
        [DataMember(Name = "login", IsRequired = true)]
        public string LoginPath { get; set; }
        [DataMember(Name = "log", IsRequired = false)]
        public string LogPath { get; set; }
        [DataMember(Name = "sets", IsRequired = false)]
        public Dictionary<string, string> Sets { get; set; } = new Dictionary<string, string>();
    }
}
