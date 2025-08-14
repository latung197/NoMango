using NLog;

namespace PuduIOT.BL.SystemLog
{
    public class LoggerFactoryInfo
    {
        public string Name { get; private set; }
        private LogFactory LogFactory { get; set; }
        public LoggerFactoryInfo(string name, LogFactory lgFact)
        {
            Name = name;
            LogFactory = lgFact;
        }

        public Logger GetLogger()
        {
            return LogFactory.GetLogger(Name);
        }
    }
}
