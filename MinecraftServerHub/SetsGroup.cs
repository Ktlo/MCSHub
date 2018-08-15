using System.Collections.Generic;

namespace MinecraftServerHub.ParamGroup
{
    class SetsGroup : IParameterGroup
    {
        private readonly Dictionary<string, string> sets;

        public SetsGroup(Dictionary<string, string> sets)
        {
            this.sets = sets;
        }

        public string GetValueFor(string key)
        {
            lock (sets)
                if (sets.ContainsKey(key))
                    return sets[key];
                else
                    return null;
        }
    }
}
