using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using YALV.Core.Domain;

namespace YALV.Core.Providers
{
    public abstract class AbstractEntriesProviderBase : AbstractEntriesProvider
    {
        public override IEnumerable<LogItem> GetEntries(string dataSource, FilterParams filter)
        {
            IEnumerable<LogItem> enumerable = this.InternalGetEntries(dataSource, filter);
            return enumerable.ToArray(); // avoid file locks            
        }

        private IEnumerable<LogItem> InternalGetEntries(string dataSource, FilterParams filter)
        {
            using (IDbConnection connection = this.CreateConnection(dataSource))
            {
                connection.Open();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {                    
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText =
                            @"select caller, date, level, logger, thread, message, exception from log where date >= @date";

                        IDbDataParameter parameter = command.CreateParameter();
                        parameter.ParameterName = "@date";
                        parameter.Value = filter.Date.HasValue ? filter.Date.Value : MinDateTime;
                        command.Parameters.Add(parameter);

                        switch (filter.Level)
                        {
                            case 1:
                                AddLevelClause(command, "ERROR");
                                break;

                            case 2:
                                AddLevelClause(command, "INFO");
                                break;

                            case 3:
                                AddLevelClause(command, "DEBUG");
                                break;

                            case 4:
                                AddLevelClause(command, "WARN");
                                break;

                            case 5:
                                AddLevelClause(command, "FATAL");
                                break;

                            default:
                                break;
                        }

                        AddLoggerClause(command, filter.Logger);
                        AddThreadClause(command, filter.Thread);
                        AddMessageClause(command, filter.Message);

                        AddOrderByClause(command);

                        using (IDataReader reader = command.ExecuteReader())
                        {
                            int index = 0;
                            while (reader.Read())
                            {
                                string caller = reader.GetString(0);
                                string[] split = caller.Split(',');

                                const string machineKey = "{log4jmachinename=";
                                string item0 = Find(split, machineKey);
                                string machineName = GetValue(item0, machineKey);

                                const string hostKey = " log4net:HostName=";
                                string item1 = Find(split, hostKey);
                                string hostName = GetValue(item1, hostKey);

                                const string userKey = " log4net:UserName=";
                                string item2 = Find(split, userKey);
                                string userName = GetValue(item2, userKey);

                                const string appKey = " log4japp=";
                                string item3 = Find(split, appKey);
                                string app = GetValue(item3, appKey);

                                DateTime timeStamp = reader.GetDateTime(1);
                                string level = reader.GetString(2);
                                string logger = reader.GetString(3);
                                string thread = reader.GetString(4);
                                string message = reader.GetString(5);
                                string exception = reader.GetString(6);

                                LogItem entry = new LogItem
                                {
                                    Id = ++index,
                                    TimeStamp = timeStamp,
                                    Level = level,
                                    Thread = thread,
                                    Logger = logger,
                                    Message = message,
                                    Throwable = exception,
                                    MachineName = machineName,
                                    HostName = hostName,
                                    UserName = userName,
                                    App = app,
                                };
                                // TODO: altri filtri
                                yield return entry;
                            }
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        protected abstract IDbConnection CreateConnection(string dataSource);

        private static void AddLevelClause(IDbCommand command, string level)
        {
            if (command == null) 
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(level))
                throw new ArgumentNullException("level");

            command.CommandText += @" and level = @level";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@level";
            parameter.Value = level;
            command.Parameters.Add(parameter);
        }

        private static void AddLoggerClause(IDbCommand command, string logger)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(logger))
                return;

            command.CommandText += @" and logger like @logger";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@logger";
            parameter.Value = String.Format("%{0}%", logger);
            command.Parameters.Add(parameter);
        }

        private static void AddThreadClause(IDbCommand command, string thread)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(thread))
                return;

            command.CommandText += @" and thread like @thread";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@thread";
            parameter.Value = String.Format("%{0}%", thread);
            command.Parameters.Add(parameter);
        }

        private static void AddMessageClause(IDbCommand command, string message)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (String.IsNullOrEmpty(message))
                return;

            command.CommandText += @" and message like @message";

            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@message";
            parameter.Value = String.Format("%{0}%", message);
            command.Parameters.Add(parameter);
        }

        private static void AddOrderByClause(IDbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            command.CommandText += @" order by date ";
        }

        private static string GetValue(string item, string key)
        {
            return String.IsNullOrEmpty(item) ? String.Empty : item.Remove(0, key.Length);
        }

        private static string Find(IEnumerable<string> items, string key)
        {
            return items.Where(i => i.StartsWith(key)).SingleOrDefault();
        }
    }
}