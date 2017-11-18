using System;

namespace YALV.Core.Domain
{
    [Serializable]
    public class ColumnItem
    {
        public string Header { get; set; }
        public string Field { get; set; }
        public string StringFormat { get; set; }
        public double? MinWidth { get; set; }
        public double? Width { get; set; }
        public CellAlignment Alignment { get; set; }
        public bool Visible { get; set; } = true; // for backwards compatibility

        public int DisplayIndex { get; set; }

        public ColumnItem(string field, double? minWidth, double? width)
            : this(field, minWidth, width, CellAlignment.DEFAULT, string.Empty, field)
        {
        }

        public ColumnItem(string field, double? minWidth, double? width, CellAlignment align)
            : this(field, minWidth, width, align, string.Empty, field)
        {
        }

        public ColumnItem(string field, double? minWidth, double? width, CellAlignment align, string stringFormat)
            : this(field, minWidth, width, align, stringFormat, field)
        {
        }

        public ColumnItem(string field, double? minWidth, double? width, CellAlignment align, string stringFormat, string header)
        {
            Field = field;
            MinWidth = minWidth;
            Width = width;
            Alignment = align;
            StringFormat = stringFormat;
            Header = header;
        }
    }

    public enum CellAlignment
    {
        DEFAULT = 0,
        CENTER = 1
    }
}
