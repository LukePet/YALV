using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using YALV.Core.Domain;
using YALV.Core.Providers;

namespace YALV.Core
{
    public static class DataService
    {
        public static void SaveFolderFile(IList<PathItem> folders, string path)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;
            try
            {
                if (folders != null)
                {
                    fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                    streamWriter = new StreamWriter(fileStream);
                    foreach (PathItem item in folders)
                    {
                        string line = String.Format("<folder name=\"{0}\" path=\"{1}\" />", item.Name, item.Path);
                        streamWriter.WriteLine(line);
                    }
                    streamWriter.Close();
                    streamWriter = null;
                    fileStream.Close();
                    fileStream = null;
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error saving Favorites list [{0}]:\r\n{1}\r\n{2}", path, ex.Message, ex.StackTrace);
                throw;
            }
            finally
            {
                if (streamWriter != null)
                    streamWriter.Close();
                if (fileStream != null)
                    fileStream.Close();
            }

        }

        public static IList<PathItem> ParseFolderFile(string path)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (!fileInfo.Exists)
                    return null;

                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(fileStream, true);
                string sBuffer = String.Format("<root>{0}</root>", streamReader.ReadToEnd());
                streamReader.Close();
                streamReader = null;
                fileStream.Close();
                fileStream = null;

                var stringReader = new StringReader(sBuffer);
                var xmlTextReader = new XmlTextReader(stringReader) { Namespaces = false };

                IList<PathItem> result = new List<PathItem>();
                while (xmlTextReader.Read())
                {
                    if ((xmlTextReader.NodeType != XmlNodeType.Element) || (xmlTextReader.Name != "folder"))
                        continue;

                    PathItem item = new PathItem(xmlTextReader.GetAttribute("name"), xmlTextReader.GetAttribute("path"));
                    result.Add(item);
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error parsing Favorites list [{0}]:\r\n{1}\r\n{2}", path, ex.Message, ex.StackTrace);
                throw;
            }
            finally
            {
                if (streamReader != null)
                    streamReader.Close();
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        public static IList<LogItem> ParseLogFile(string path)
        {
            IEnumerable<LogItem> result = null;
            try
            {
                AbstractEntriesProvider provider = EntriesProviderFactory.GetProvider();
                result = provider.GetEntries(path);
                return result.ToList();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error parsing log file [{0}]:\r\n{1}\r\n{2}", path, ex.Message, ex.StackTrace);
                throw;
            }
        }

        public static IList<ColumnRenderSettings> ParseSettings(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                var result = new List<ColumnRenderSettings>();

                var xml = XDocument.Load(path);
                foreach (var element in xml.Descendants("column"))
                {
                    var columnRenderSettings = new ColumnRenderSettings();
                    columnRenderSettings.Id = element.Attribute("id").Value; // must have ID

                    var displayIndexAttribute = element.Attribute("displayIndex");
                    if (displayIndexAttribute != null) columnRenderSettings.DisplayIndex = int.Parse(displayIndexAttribute.Value);

                    var widthAttribute = element.Attribute("width");
                    if (widthAttribute != null) columnRenderSettings.Width = int.Parse(widthAttribute.Value);

                    var visibleAttribute = element.Attribute("visible");
                    if (visibleAttribute != null) columnRenderSettings.Visible = bool.Parse(visibleAttribute.Value);

                    result.Add(columnRenderSettings);
                }
                return result;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error parsing column settings from file {0}:\r\n{1}", path, ex);
                throw;
            }
        }

        public static void SaveSettings(IList<ColumnRenderSettings> columnRenderSettingsCollection, string path)
        {
            try
            {
                var xml = File.Exists(path) ? XDocument.Load(path) : new XDocument();

                var columnsElement = xml.Descendants("columns").SingleOrDefault();
                if (columnsElement == null)
                {
                    if (xml.Root == null)
                    {
                        var root = new XElement("settings");
                        xml.Add(root);
                    }
                    columnsElement = new XElement("columns");
                    xml.Root.Add(columnsElement);
                }

                var columnElements = from columnSettings in columnRenderSettingsCollection
                                     select new XElement("column",
                                         new XAttribute("id", columnSettings.Id),
                                         new XAttribute("displayIndex", columnSettings.DisplayIndex),
                                         new XAttribute("width", columnSettings.Width),
                                         new XAttribute("visible", columnSettings.Visible));
                columnsElement.ReplaceAll(columnElements);
                xml.Save(path);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error saving column settings in file {0}:\r\n{1}", path, ex);
                throw;
            }
        }
    }
}