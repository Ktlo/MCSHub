using System;
using System.IO;

namespace MinecraftServerHub
{
    public class FileLog : SystemLog
    {
        private static readonly string programName = "MCSHub";
        private readonly string filePath;

        public FileLog(string filepath)
        {
            filePath = filepath;
        }

        private void Write(string whatWrite)
        {
            File.AppendAllText(filePath, $"[{DateTime.Now:hh:mm:ss}][{programName}]{whatWrite}\n");
        }

        public override void WriteDebug(string line)
        {
            Write($"[Debug] {line}");
        }

        public override void WriteError(string line)
        {
            Write($"[Error] {line}");
        }

        public override void WriteInfo(string line)
        {
            Write($"[Info] {line}");
        }

        public override void WriteWarn(string line)
        {
            Write($"[Warn] {line}");
        }
    }
}
