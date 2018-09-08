using System;

namespace MinecraftServerHub.App
{
    public class StdOutLog : SystemLog
    {
        private static readonly string programName = "MCSHub";

        private void Write(string whatWrite)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}][{programName}]{whatWrite}");
        }

        private void WriteColor(ConsoleColor color, string line)
        {
            var save = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Write(line);
            Console.ForegroundColor = save;
        }
        
        public override void WriteDebug(string line)
        {
            WriteColor(ConsoleColor.Gray, $"[Debug]: {line}");
        }

        public override void WriteError(string line)
        {
            WriteColor(ConsoleColor.DarkRed, $"[Error]: {line}");
        }

        public override void WriteInfo(string line)
        {
            Write($"[Info]: {line}");
        }

        public override void WriteWarn(string line)
        {
            WriteColor(ConsoleColor.DarkYellow, $"[Warn]: {line}");
        }
    }
}
