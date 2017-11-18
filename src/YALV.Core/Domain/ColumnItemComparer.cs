using System.Collections.Generic;

namespace YALV.Core.Domain
{
    public class ColumnItemComparer : IComparer<ColumnItem>
    {
        public int Compare(ColumnItem x, ColumnItem y)
        {
            return x.DisplayIndex.CompareTo(y.DisplayIndex);
        }
    }
}