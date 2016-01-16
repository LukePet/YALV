using System;

namespace YALV.Core.Domain
{
    public class FilterParams
    {
        public DateTime? Date { get; set; }

        public int Level { get; set; }

        public string Thread { get; set; }

        public string Logger { get; set; }

        public string Message { get; set; }

        public string Pattern { get; set; }

        public override string ToString()
        {
            return String.Format(
                "Date: {0}, Level: {1}, Thread: {2}, Logger: {3}, Message: {4}",
                this.Date, this.Level, this.Thread, this.Logger, this.Message);
        }
    }
}