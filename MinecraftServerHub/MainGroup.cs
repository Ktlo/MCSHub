using System;

namespace MinecraftServerHub.ParamGroup
{
    class MainGroup : IParameterGroup
    {
        private MainGroup() { }

        public string GetValueFor(string key)
        {
            switch (key)
            {
                case "uuid":
                    return Guid.NewGuid().ToString("D");
                default:
                    return null;
            }
        }

        private static readonly IParameterGroup instance = new MainGroup();
        public static IParameterGroup Instance { get => instance; }
    }
}
