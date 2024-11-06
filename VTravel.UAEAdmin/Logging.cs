using System;
using System.IO;

namespace VTravel.Admin
{
    public interface ILoggingService
    {
        void LogException(Exception exception);
        void LogReqResp(string content, string type);
    }
    public class Logging : ILoggingService
    {
        public void LogException(Exception exception)
        {
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logFile = Path.Combine(logFolder, $"{DateTime.Now:yyyyMMdd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:G}: {exception}\n\n");
        }
        public void LogReqResp(string content, string type)
        {
            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            var logFile = Path.Combine(logFolder, $"{DateTime.Now:yyyyMMdd}.txt");
            File.AppendAllText(logFile, $"{DateTime.Now:G}: {content} : {type}\n\n");
        }
    }
}
