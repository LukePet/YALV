using System;
using System.Diagnostics;

namespace YALV.Core.Domain
{
    [Serializable]
    [DebuggerDisplay("#{DisplayIndex} {Id,nq}, Width={Width}px")]
    public class ColumnRenderSettings
    {
        public string Id { get; set; }
        public int DisplayIndex { get; set; }
        public int Width { get; set; }
    }
}