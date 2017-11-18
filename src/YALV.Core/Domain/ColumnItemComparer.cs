using System.Collections.Generic;

namespace YALV.Core.Domain
{
    public class ColumnItemComparer : IComparer<ColumnItem>
    {
        public int Compare(ColumnItem x, ColumnItem y)
        {
            if (x.Field == y.Field)
            {
                return 0;
            }
            if (x.Visible && !y.Visible)
            {
                return -1;
            }
            if (!x.Visible && y.Visible)
            {
                return 1;
            }
            return x.DisplayIndex.CompareTo(y.DisplayIndex);
        }
    }
}