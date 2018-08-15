using System.IO;

namespace MinecraftServerHub.ParamGroup
{
    public class FileGroup : IParameterGroup
    {
        private FileGroup() { }

        public string GetValueFor(string key)
        {
            if (File.Exists(key))
                using (var reader = File.OpenText(key))
                {
                    return reader.ReadToEnd();
                }
            else
                return null;
        }

        private static readonly IParameterGroup instance = new FileGroup();
        public static IParameterGroup Instance { get => instance; }
    }
}
