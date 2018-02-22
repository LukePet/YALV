using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    class JsonEntriesProvider : AbstractEntriesProvider
    {
        public override IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter)
        {
            var entryId = 1;

            FileStream fs = new FileStream(dataSource, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();
                var lineObject = JsonConvert.DeserializeObject<JObject>(line);

                LogItem entry = new LogItem()
                {
                    File = dataSource,
                    Message = lineObject.SelectToken("MessageTemplate")?.Value<String>() ?? "",
                    TimeStamp = lineObject.SelectToken("Timestamp").Value<DateTime>(),
                    Level = lineObject.SelectToken("Level")?.Value<String>().ToUpper() ?? "",
                    App = lineObject.SelectToken("Properties").SelectToken("ProcessName")?.Value<String>() ?? "",
                    Thread = lineObject.SelectToken("Properties").SelectToken("ThreadId")?.Value<String>() ?? "",
                    MachineName = lineObject.SelectToken("Properties").SelectToken("MachineName")?.Value<String>() ?? "",
                    UserName = lineObject.SelectToken("Properties").SelectToken("EnvironmentUserName")?.Value<String>() ?? "",
                    Id = entryId,
                    Path = dataSource
                };

                // adjust LEVEL:
                if (entry.Level == "INFORMATION")
                    entry.Level = "INFO";
                else if (entry.Level == "WARNING")
                    entry.Level = "WARN";


                // replace properties into message template
                foreach(JProperty property in lineObject.SelectToken("Properties").Children())
                {
                    string token = "{" + property.Name + "}";
                    string value = property.Value.Value<string>();

                    entry.Message = entry.Message.Replace(token, value);
                }

                if (filterByParameters(entry, filter))
                {
                    yield return entry;
                    entryId++;
                }
            }
        }

        private static bool filterByParameters(LogItem entry, FilterParams parameters)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            bool accept = false;
            switch (parameters.Level)
            {
                case 1:
                    if (String.Equals(entry.Level, "ERROR",
                        StringComparison.InvariantCultureIgnoreCase))
                        accept = true;
                    break;

                case 2:
                    if (String.Equals(entry.Level, "INFO",
                        StringComparison.InvariantCultureIgnoreCase))
                        accept = true;
                    break;

                case 3:
                    if (String.Equals(entry.Level, "DEBUG",
                        StringComparison.InvariantCultureIgnoreCase))
                        accept = true;
                    break;

                case 4:
                    if (String.Equals(entry.Level, "WARN",
                        StringComparison.InvariantCultureIgnoreCase))
                        accept = true;
                    break;

                case 5:
                    if (String.Equals(entry.Level, "FATAL",
                        StringComparison.InvariantCultureIgnoreCase))
                        accept = true;
                    break;

                default:
                    accept = true;
                    break;
            }

            if (parameters.Date.HasValue)
                if (entry.TimeStamp < parameters.Date)
                    accept = false;

            if (!String.IsNullOrEmpty(parameters.Thread))
                if (!String.Equals(entry.Thread, parameters.Thread, StringComparison.InvariantCultureIgnoreCase))
                    accept = false;

            if (!String.IsNullOrEmpty(parameters.Message))
                if (!entry.Message.ToUpper().Contains(parameters.Message.ToUpper()))
                    accept = false;

            if (!String.IsNullOrEmpty(parameters.Logger))
                if (!entry.Logger.ToUpper().Contains(parameters.Logger.ToUpper()))
                    accept = false;

            return accept;
        }
    }
}