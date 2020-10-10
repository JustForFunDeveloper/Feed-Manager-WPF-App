using System.IO;
using HS_Feed_Manager.Core.Handler;

namespace HS_Feed_Manager.Core.GlobalValues
{
    public static class LogicConstants
    {
        public static readonly string LogFilePath = Directory.GetCurrentDirectory() + "\\Logs\\";
        public static readonly string LogFileName = "log.txt";

        public static readonly string StandardXmlPath = Directory.GetCurrentDirectory() + "\\";
        public static readonly string StandardXmlName = "config.xml";
    }
}
