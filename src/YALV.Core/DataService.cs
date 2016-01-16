using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
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
                System.Diagnostics.Trace.TraceError("Error saving Favorites list [{0}]:\r\n{1}\r\n{2}",path,ex.Message,ex.StackTrace);
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
                System.Diagnostics.Trace.TraceError("Error parsing Favorites list [{0}]:\r\n{1}\r\n{2}", path, ex.Message, ex.StackTrace);
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
                System.Diagnostics.Trace.TraceError("Error parsing log file [{0}]:\r\n{1}\r\n{2}", path, ex.Message, ex.StackTrace);
                throw;
            }
        }
    }
}