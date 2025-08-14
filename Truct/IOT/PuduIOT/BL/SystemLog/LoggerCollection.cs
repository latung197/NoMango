using PuduIOT.BL;
using PuduIOT.BL.SystemLog;
using PuduIOT.DA;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Reflection;
using LogLevel = NLog.LogLevel;

namespace Infrastructure.SystemLog
{
    public class LoggerCollection : BaseRepositoryDictionary<string, LoggerFactoryInfo>, ILoggers
    {
        private static readonly LoggingConfiguration _configuration = new();
        private static readonly string LayoutLog = "${longdate} ${level:uppercase=true} [${threadname:whenEmpty=${threadid}}] ${message:uppercase=true}";
        private static readonly string RootPath = Path.Combine(Path.GetDirectoryName(path: Assembly.GetEntryAssembly().Location), "wwwroot", "Externals", "Logs");

        private readonly string LogInfoPath = Path.Combine(RootPath, "LogInfo");
        private readonly string LogErrorPath = Path.Combine(RootPath, "LogError");
        private readonly string LogWarnPath = Path.Combine(RootPath, "LogWarning");
        private void CreateLog(string name, string fileName)
        {
            LogFactory NloggerFactory;
            var _guidId = Guid.NewGuid().ToString("N");
            name = $"{_guidId}_{name}";
            lock (_sync)
            {
                var fileTarget = new FileTarget(name)
                {
                    FileName = $"{fileName}.log",
                    Layout = LayoutLog
                };
                var configRule = new LoggingRule($"log_{name}", LogLevel.Info, fileTarget);
                _configuration.LoggingRules.Add(configRule);
                NloggerFactory = new LogFactory(_configuration);
            }

            if (NloggerFactory is not null)
            {
                var lg = new LoggerFactoryInfo($"log_{name}", NloggerFactory);
                AddOrUpdate(fileName, lg);
            }

        }

        private Logger? GetLoggerByName(string name, string fileName)
        {
            if (ContainsKey(fileName))
            {
                return GetValueByKey(fileName)?.GetLogger();
            }
            CreateLog(name, fileName);
            return GetValueByKey(fileName)?.GetLogger();
        }

        public void LogInfo(string text)
        {
            var log = GetLoggerByName("Info", LogInfoPath);
            log?.Info(text);
        }

        public void LogError(string text)
        {
            var log = GetLoggerByName("Error", LogErrorPath);
            log?.Error(text);
        }

        public void LogWarn(string text)
        {
            var log = GetLoggerByName("Warn", LogWarnPath);
            log?.Warn(text);
        }
    }
}
