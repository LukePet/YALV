using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    class XmlEntriesProvider : AbstractEntriesProvider
    {
        public override IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter)
        {
            var settings = new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment };
            var nt = new NameTable();
            var mgr = new XmlNamespaceManager(nt);
            mgr.AddNamespace("log4j", Constants.LAYOUT_LOG4J);
            var pc = new XmlParserContext(nt, mgr, string.Empty, XmlSpace.Default);
            var date = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            using (var stream = new FileStream(dataSource, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream, System.Text.Encoding.Default, true))
                {
                    using (var xmlTextReader = XmlReader.Create(reader, settings, pc))
                    {
                        var entryId = 1;
                        DateTime? prevTimeStamp = null;
                        while (xmlTextReader.Read())
                        {
                            if ((xmlTextReader.NodeType != XmlNodeType.Element) || (xmlTextReader.Name != "log4j:event"))
                                continue;

                            var entry = new LogItem { Id = entryId, Path = dataSource };

                            entry.Logger = xmlTextReader.GetAttribute("logger");

                            entry.TimeStamp = date.AddMilliseconds(Convert.ToDouble(xmlTextReader.GetAttribute("timestamp"))).ToLocalTime();
                            if (prevTimeStamp.HasValue)
                                entry.Delta = (entry.TimeStamp - prevTimeStamp.Value).TotalSeconds;
                            prevTimeStamp = entry.TimeStamp;

                            entry.Level = xmlTextReader.GetAttribute("level");
                            entry.Thread = xmlTextReader.GetAttribute("thread");

                            while (xmlTextReader.Read())
                            {
                                var breakLoop = false;
                                switch (xmlTextReader.Name)
                                {
                                    case "log4j:event":
                                        breakLoop = true;
                                        break;
                                    default:
                                        switch (xmlTextReader.Name)
                                        {
                                            case ("log4j:message"):
                                                entry.Message = xmlTextReader.ReadString();
                                                break;
                                            case ("log4j:data"):
                                                switch (xmlTextReader.GetAttribute("name"))
                                                {
                                                    case ("log4net:UserName"):
                                                        entry.UserName = xmlTextReader.GetAttribute("value");
                                                        break;
                                                    case ("log4japp"):
                                                        entry.App = xmlTextReader.GetAttribute("value");
                                                        break;
                                                    case ("log4jmachinename"):
                                                        entry.MachineName = xmlTextReader.GetAttribute("value");
                                                        break;
                                                    case ("log4net:HostName"):
                                                        entry.HostName = xmlTextReader.GetAttribute("value");
                                                        break;
													default:
		                                                var name = xmlTextReader.GetAttribute("name");
		                                                if (!name.StartsWith("log4net:"))
		                                                {
															var val = xmlTextReader.GetAttribute("value");
															entry.CustomFields.Add(name, val);
														}
														break;
                                                }
                                                break;
                                            case ("log4j:throwable"):
                                                entry.Throwable = xmlTextReader.ReadString();
                                                break;
                                            case ("log4j:locationInfo"):
                                                entry.Class = xmlTextReader.GetAttribute("class");
                                                entry.Method = xmlTextReader.GetAttribute("method");
                                                entry.File = xmlTextReader.GetAttribute("file");
                                                entry.Line = xmlTextReader.GetAttribute("line");
                                                break;
                                        }
                                        break;
                                }
                                if (breakLoop) break;
                            }

                            if (filterByParameters(entry, filter))
                            {
                                yield return entry;
                                entryId++;
                            }
                        }
                    }
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