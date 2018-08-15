using System.Diagnostics;

namespace MinecraftServerHub.NTService
{
    class WindowsLog : SystemLog
    {
        private readonly EventLog log;
        private readonly bool debug;

        public WindowsLog(EventLog eventLog, bool debug = false)
        {
            log = eventLog;
            this.debug = debug;
        }

        public override void WriteDebug(string line)
        {
            if (debug)
                log.WriteEntry(line, EventLogEntryType.Information);
        }

        public override void WriteError(string line)
        {
            log.WriteEntry(line, EventLogEntryType.Error);
        }

        public override void WriteInfo(string line)
        {
            log.WriteEntry(line, EventLogEntryType.Information);
        }

        public override void WriteWarn(string line)
        {
            log.WriteEntry(line, EventLogEntryType.Warning);
        }
    }
}
