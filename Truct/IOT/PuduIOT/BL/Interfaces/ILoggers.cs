namespace PuduIOT.BL
{
    public interface ILoggers
    {
        void LogInfo(string text);
        void LogError(string text);
        void LogWarn(string text);
    }
}
