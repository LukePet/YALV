using System;
using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    public static class EntriesProviderFactory
    {
        public static AbstractEntriesProvider GetProvider(EntriesProviderType type = EntriesProviderType.Xml)
        {
            switch (type)
            {
                case EntriesProviderType.Xml:
                    return new XmlEntriesProvider();

                case EntriesProviderType.Sqlite:
                    return new SqliteEntriesProvider();

                case EntriesProviderType.MsSqlServer:
                    return new MsSqlServerEntriesProvider();

                default:
                    var message = String.Format((string) "Type {0} not supported", (object) type);
                    throw new NotImplementedException(message);
            }
        }
    }
}