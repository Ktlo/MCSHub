using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace MinecraftServerHub
{
    [DataContract]
    public class ServiceSettings
    {
        [DataMember(Name = "port", IsRequired = false)]
        public ushort Port { get; set; }
        [DataMember(Name = "address", IsRequired = false)]
        public string Address { get; set; }

        [DataMember(Name = "servers", IsRequired = false)]
        public List<ServerRecord> Records { get; set; }

        [DataMember(Name = "default", IsRequired = true)]
        public ServerRecord Default { get; set; }

        public static ServiceSettings Settings
        {
            get
            {
                var jsonFormatter = new DataContractJsonSerializer(typeof(ServiceSettings));
                ServiceSettings settings;
                using (var stream = new FileStream("config.json", FileMode.Open))
                {
                    settings = (ServiceSettings)jsonFormatter.ReadObject(stream);
                }
                if (settings.Port == 0)
                    settings.Port = 25565;
                if (settings.Address == null)
                    settings.Address = "0.0.0.0";
                if (settings.Records == null)
                    settings.Records = new List<ServerRecord>();
                foreach (var record in settings.Records)
                {
                    if (record.Port == 0)
                        record.Port = 25565;
                    if (record.Mask == null)
                        record.Mask = "*";
                    if (record.Sets == null)
                        record.Sets = new Dictionary<string, string>();
                }
                return settings;
            }
        }
    }
}
