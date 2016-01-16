using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using YALV.Core.Domain;
using YALV.Core.Exceptions;

namespace YALV.Core.Providers
{
    public class FileEntriesProvider : AbstractEntriesProvider
    {
        private const string Separator = "[---]";
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss,fff";

        public override IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter)
        {
            if (String.IsNullOrEmpty(dataSource))
                throw new ArgumentNullException("dataSource");
            if (filter == null)
                throw new ArgumentNullException("filter");

            string pattern = filter.Pattern;
            if (String.IsNullOrEmpty(pattern))
                throw new NotValidValueException("filter pattern null");

            FileInfo file = new FileInfo(dataSource);
            if (!file.Exists)
                throw new FileNotFoundException("file not found", dataSource);

            Regex regex = new Regex(@"%\b(date|message|level)\b");
            MatchCollection matches = regex.Matches(pattern);

            using (StreamReader reader = file.OpenText())
            {
                string s;
                while ((s = reader.ReadLine()) != null)
                {
                    string[] items = s.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
                    LogItem entry = CreateEntry(items, matches);
                    entry.Logger = filter.Logger;
                    yield return entry;
                }
            }            
        }

        private static LogItem CreateEntry(string[] items, MatchCollection matches)
        {
            if (items == null) 
                throw new ArgumentNullException("items");
            if (matches == null) 
                throw new ArgumentNullException("matches");

            if (items.Length != matches.Count)
                throw new NotValidValueException("different length of items/matches values");

            LogItem entry = new LogItem();
            for (int i = 0; i < matches.Count; i++)
            {
                string value = items[i];
                Match match = matches[i];
                string name = match.Value;
                switch (name)
                {
                    case "%date":
                        entry.TimeStamp = DateTime.ParseExact(
                            value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                        break;

                    case "%message":
                        entry.Message = value;
                        break;

                    case "%level":
                        entry.Level = value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(name, "unmanaged value");
                }
            }
            return entry;
        }
    }
}