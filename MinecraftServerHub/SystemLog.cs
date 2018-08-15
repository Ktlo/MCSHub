namespace MinecraftServerHub
{
    public abstract class SystemLog
    {
        public abstract void WriteDebug(string line);
        public abstract void WriteInfo(string line);
        public abstract void WriteWarn(string line);
        public abstract void WriteError(string line);
    }
}
