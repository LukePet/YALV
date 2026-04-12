using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    class JsonEntriesProvider : AbstractEntriesProvider
    {
        // Reserved fields in Serilog Compact Log Event Format (CLEF)
        private static readonly HashSet<string> CompactReservedFields = new HashSet<string>(StringComparer.Ordinal)
        {
            "@t", "@mt", "@m", "@l", "@x", "@i", "@r", "@tr", "@sp"
        };

        public override IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter)
        {
            var entryId = 1;
            DateTime? prevTimeStamp = null;

            using (var fs = new FileStream(dataSource, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var lineObject = JsonConvert.DeserializeObject<JObject>(line);

                        // Detect format: Serilog Compact (CLEF) uses "@t", standard uses "Timestamp"
                        LogItem entry = lineObject.ContainsKey("@t")
                            ? ParseCompactEntry(lineObject, dataSource, entryId)
                            : ParseStandardEntry(lineObject, dataSource, entryId);

                        if (prevTimeStamp.HasValue)
                            entry.Delta = (entry.TimeStamp - prevTimeStamp.Value).TotalSeconds;
                        prevTimeStamp = entry.TimeStamp;

                        if (filterByParameters(entry, filter))
                        {
                            yield return entry;
                            entryId++;
                        }
                    }
                }
            }
        }

        private static LogItem ParseCompactEntry(JObject lineObject, string dataSource, int entryId)
        {
            var level = (lineObject.SelectToken("@l")?.Value<string>() ?? "Information").ToUpper();
            if (level == "INFORMATION")
                level = "INFO";
            else if (level == "WARNING")
                level = "WARN";

            var messageTemplate = lineObject.SelectToken("@mt")?.Value<string>() ?? "";
            var eventIdValue = GetEventIdValue(lineObject.SelectToken("EventId"));

            var entry = new LogItem()
            {
                File = dataSource,
                Message = string.IsNullOrEmpty(eventIdValue)
                    ? messageTemplate
                    : messageTemplate + Environment.NewLine + Environment.NewLine + eventIdValue,
                TimeStamp = lineObject.SelectToken("@t").Value<DateTime>(),
                Level = level,
                App = lineObject.SelectToken("ProcessName")?.Value<string>() ?? "",
                Thread = lineObject.SelectToken("ThreadId")?.Value<string>() ?? "",
                MachineName = lineObject.SelectToken("MachineName")?.Value<string>() ?? "",
                UserName = lineObject.SelectToken("EnvironmentUserName")?.Value<string>() ?? "",
                Logger = lineObject.SelectToken("SourceContext")?.Value<string>() ?? "",
                Throwable = lineObject.SelectToken("@x")?.Value<string>() ?? "",
                Method = lineObject.SelectToken("RequestPath")?.Value<string>() ?? "",
                Id = entryId,
                Path = dataSource
            };

            // Replace property tokens in the message template; root-level non-reserved fields are properties
            foreach (JProperty property in lineObject.Children<JProperty>())
            {
                if (CompactReservedFields.Contains(property.Name))
                    continue;

                string value = GetStringValue(property.Value);
                if (value != null)
                    entry.Message = entry.Message.Replace("{" + property.Name + "}", value);
            }

            return entry;
        }

        private static LogItem ParseStandardEntry(JObject lineObject, string dataSource, int entryId)
        {
            var level = (lineObject.SelectToken("Level")?.Value<string>() ?? "").ToUpper();
            if (level == "INFORMATION")
                level = "INFO";
            else if (level == "WARNING")
                level = "WARN";

            var properties = lineObject.SelectToken("Properties");

            var entry = new LogItem()
            {
                File = dataSource,
                Message = lineObject.SelectToken("MessageTemplate")?.Value<string>() ?? "",
                TimeStamp = lineObject.SelectToken("Timestamp").Value<DateTime>(),
                Level = level,
                App = properties?.SelectToken("ProcessName")?.Value<string>() ?? "",
                Thread = properties?.SelectToken("ThreadId")?.Value<string>() ?? "",
                MachineName = properties?.SelectToken("MachineName")?.Value<string>() ?? "",
                UserName = properties?.SelectToken("EnvironmentUserName")?.Value<string>() ?? "",
                Id = entryId,
                Path = dataSource
            };

            // Replace property tokens in the message template
            if (properties != null)
            {
                foreach (JProperty property in properties.Children<JProperty>())
                {
                    string value = GetStringValue(property.Value);
                    if (value != null)
                        entry.Message = entry.Message.Replace("{" + property.Name + "}", value);
                }
            }

            return entry;
        }

        private static string GetEventIdValue(JToken eventId)
        {
            if (eventId == null)
                return "";

            var info = "";
            if (eventId.Type == JTokenType.Object)
            {
                var id = eventId["Id"]?.ToString();
                var name = eventId["Name"]?.Value<string>();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(id))
                    return "[EventId: " + id + " - " + name + "]";

                info = name ?? id ?? "";
                if (!string.IsNullOrEmpty(info))
                    return $"[EventId: {info}]";
                return "";
            }

            info = eventId.ToString();
            if (!string.IsNullOrEmpty(info))
                return $"[EventId: {info}]";

            return "";
        }

        private static string GetStringValue(JToken token)
        {
            if (token == null)
                return null;
            switch (token.Type)
            {
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                    return token.ToString();
                default:
                    return null;
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