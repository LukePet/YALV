using System;
using System.IO;

namespace YALV.Core
{
    public static class Constants
    {
        public const string DISPLAY_DATETIME_FORMAT = "MMM d, HH:mm:ss.fff";

        public const string LAYOUT_LOG4J = "http://jakarta.apache.org/log4j";

        public const int DEFAULT_REFRESH_INTERVAL = 30;

        public static string FOLDERS_FILE_PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YALVFolders.xml");

        
    }
}