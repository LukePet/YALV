using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YALV.Common.Converters;
using YALV.Core.Domain;
using YALV.Properties;

namespace YALV.Common
{
    public class FilteredGridManager
        : FilteredGridManagerBase
    {
        private static readonly DependencyProperty ColumnIdProperty = DependencyProperty.RegisterAttached("Id", typeof(string), typeof(DataGridTextColumn));

        public FilteredGridManager(DataGrid dg, Panel txtSearchPanel, KeyEventHandler keyUpEvent)
            : base(dg, txtSearchPanel, keyUpEvent)
        {
            _centerCellStyle = Application.Current.FindResource("CenterDataGridCellStyle") as Style;
            _adjConv = new AdjustValueConverter();
        }

        #region Private Properties

        private Style _centerCellStyle;
        private AdjustValueConverter _adjConv;

        #endregion

        #region Public Methods

        public void BuildDataGrid(IList<ColumnItem> columns)
        {
            if (_dg == null)
                return;

            if (_filterPropertyList == null)
                _filterPropertyList = new List<string>();
            else
                _filterPropertyList.Clear();

            if (columns != null)
            {
                foreach (ColumnItem item in columns)
                {
                    DataGridTextColumn col = new DataGridTextColumn();
                    col.SetValue(ColumnIdProperty, item.Field);
                    col.Header = item.Header;
                    if (item.Alignment == CellAlignment.CENTER && _centerCellStyle != null)
                        col.CellStyle = _centerCellStyle;
                    if (item.MinWidth != null)
                        col.MinWidth = item.MinWidth.Value;
                    if (item.Width != null)
                        col.Width = item.Width.Value;
                    col.Visibility = item.Visible ? Visibility.Visible : Visibility.Collapsed;

                    Binding bind = new Binding(item.Field) { Mode = BindingMode.OneWay };
                    bind.ConverterCulture = CultureInfo.GetCultureInfo(Resources.CultureName);
                    if (!String.IsNullOrWhiteSpace(item.StringFormat))
                        bind.StringFormat = item.StringFormat;
                    col.Binding = bind;

                    //Add column to datagrid
                    _dg.Columns.Add(col);

                    if (_txtSearchPanel != null)
                    {
                        Binding widthBind = new Binding()
                        {
                            Path = new PropertyPath("ActualWidth"),
                            Source = col,
                            Mode = BindingMode.OneWay,
                            Converter = _adjConv,
                            ConverterParameter = "-2"
                        };

                        TextBox txt = new TextBox();
                        Style txtStyle = Application.Current.FindResource("RoundWatermarkTextBox") as Style;
                        if (txtStyle != null)
                            txt.Style = txtStyle;
                        txt.Name = getTextBoxName(item.Field);
                        txt.ToolTip = String.Format(Resources.FilteredGridManager_BuildDataGrid_FilterTextBox_Tooltip, item.Header);
                        txt.Tag = txt.ToolTip.ToString().ToLower();
                        txt.Text = string.Empty;
                        txt.AcceptsReturn = false;
                        txt.SetBinding(TextBox.WidthProperty, widthBind);

                        var visibilityBind = new Binding
                        {
                            Path = new PropertyPath(nameof(DataGridColumn.Visibility)),
                            Source = col,
                            Mode = BindingMode.OneWay
                        };
                        txt.SetBinding(TextBox.VisibilityProperty, visibilityBind);

                        _filterPropertyList.Add(item.Field);
                        if (_keyUpEvent != null)
                            txt.KeyUp += _keyUpEvent;

                        RegisterControl<TextBox>(_txtSearchPanel, txt.Name, txt);
                        _txtSearchPanel.Children.Add(txt);
                    }
                }
            }

            _dg.ColumnReordered += OnColumnReordered;
        }

        public IEnumerable<ColumnRenderSettings> GetColumnRenderSettings()
        {
            foreach (var column in _dg.Columns.OrderBy(c => c.DisplayIndex))
            {
                var columnSettings = new ColumnRenderSettings
                {
                    Id = (string)column.GetValue(ColumnIdProperty),
                    Width = (int)column.ActualWidth,
                    DisplayIndex = column.DisplayIndex,
                    Visible = column.Visibility == Visibility.Visible
                };
                yield return columnSettings;
            }
        }

        public ColumnVisibilitySettings GetColumnVisibilitySettings()
        {
            var settings = new ColumnVisibilitySettings();
            var map = GetColumnVisibilityMap();
            foreach (var item in map)
            {
                var show = item.Column.Visibility == Visibility.Visible;
                item.ColumnVisibilitySettingsProperty.SetValue(settings, show, null);
            }
            return settings;
        }

        public void UpdateColumVisibilitySettings(ColumnVisibilitySettings settings)
        {
            var map = GetColumnVisibilityMap();
            foreach (var tuple in map)
            {
                var shouldBeVisibile = (bool)tuple.ColumnVisibilitySettingsProperty.GetValue(settings, null);
                var isVisible = tuple.Column.Visibility == Visibility.Visible;

                if (shouldBeVisibile && !isVisible)
                {
                    tuple.Column.Visibility = Visibility.Visible;
                }
                else if (!shouldBeVisibile && isVisible)
                {
                    tuple.Column.Visibility = Visibility.Collapsed;
                }
            }
        }

        #endregion

        #region Private methods

        private void OnColumnReordered(object sender, DataGridColumnEventArgs dataGridColumnEventArgs)
        {
            if (dataGridColumnEventArgs.Column == null)
                return;

            var field = (string)dataGridColumnEventArgs.Column.GetValue(ColumnIdProperty);
            int displayOrder = dataGridColumnEventArgs.Column.DisplayIndex;
            string textBoxName = getTextBoxName(field);

            TextBox textBox = (from tb in _txtSearchPanel.Children.OfType<TextBox>()
                               where tb.Name == textBoxName
                               select tb).FirstOrDefault<TextBox>();

            if (textBox == null)
                return;

            _txtSearchPanel.Children.Remove(textBox);
            _txtSearchPanel.Children.Insert(displayOrder, textBox);
        }

        private void RegisterControl<T>(FrameworkElement element, string controlName, T control)
        {
            if ((T)element.FindName(controlName) != null)
            {
                element.UnregisterName(controlName);
            }
            element.RegisterName(controlName, control);
        }

        private IEnumerable<ColumnVisibilityTuple> GetColumnVisibilityMap()
        {
            foreach (var column in _dg.Columns)
            {
                var columnId = (string)column.GetValue(ColumnIdProperty);
                var tuple = new ColumnVisibilityTuple
                {
                    Column = column,
                    ColumnVisibilitySettingsProperty = ColumnVisibilitySettings.GetPropertyByLogItemType(columnId)
                };
                yield return tuple;
            }
        }

        #endregion

        /// <summary>
        /// Maps a data grid column to a property of <see cref="ColumnVisibilitySettings"/>.
        /// </summary>
        private class ColumnVisibilityTuple
        {
            public DataGridColumn Column { get; set; }
            public PropertyInfo ColumnVisibilitySettingsProperty { get; set; }
        }
    }
}
