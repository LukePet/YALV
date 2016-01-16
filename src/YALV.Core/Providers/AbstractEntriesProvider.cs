using System;
using System.Collections.Generic;
using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    public abstract class AbstractEntriesProvider
    {
        protected static readonly DateTime MinDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public IEnumerable<LogItem> GetEntries(string dataSource)
        {
            return this.GetEntries(dataSource, new FilterParams());
        }

        public abstract IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter);                
    }
}