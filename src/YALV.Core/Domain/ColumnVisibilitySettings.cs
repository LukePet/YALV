using System;
using System.Reflection;

namespace YALV.Core.Domain
{
    public class ColumnVisibilitySettings
    {
        public bool ShowId { get; set; }

        public bool ShowTimeStamp { get; set; }

        public bool ShowThread { get; set; }

        public bool ShowLevel { get; set; }

        public bool ShowLogger { get; set; }

        public bool ShowMessage { get; set; }

        public bool ShowApp { get; set; }

        public bool ShowUserName { get; set; }

        public bool ShowMachineName { get; set; }

        public bool ShowHostName { get; set; }

        public bool ShowClass { get; set; }

        public bool ShowMethod { get; set; }

        /// <summary>
        /// Gets this class's property that matches the property of a <see cref="LogItem"/>.
        /// </summary>
        /// <param name="logItemPropertyName">The property of the log item.</param>
        /// <returns>The matching property.</returns>
        public static PropertyInfo GetPropertyByLogItemType(string logItemPropertyName)
        {
            var propertyName = "Show" + logItemPropertyName;
            var property = typeof(ColumnVisibilitySettings).GetProperty(propertyName);
            if (property == null) throw new MemberAccessException("Could not find property " + logItemPropertyName);
            return property;
        }
    }
}