using System;

namespace MinecraftServerHub.ParamGroup
{
    class EnvGroup : IParameterGroup
    {
        private EnvGroup() { }

        public string GetValueFor(string key)
        {
            return Environment.GetEnvironmentVariable(key);
        }

        private static readonly IParameterGroup instance = new EnvGroup();
        public static IParameterGroup Instance { get => instance; }
    }
}
