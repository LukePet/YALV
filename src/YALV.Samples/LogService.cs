using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using log4net.Config;

namespace YALV.Samples
{
    public sealed class LogService
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static LogService()
        {
            log4net.GlobalContext.Properties["SampleDate"] = DateTime.Now.ToString("yyyyMMdd");
            //Log4Net Inizialization
            XmlConfigurator.Configure();
        }

        public static ILog Trace
        {
            get { return logger; }
        }
    }
}