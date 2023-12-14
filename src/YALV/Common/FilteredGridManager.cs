using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using YALV.Common.Converters;
using YALV.Core.Domain;
using YALV.Properties;
using Application = System.Windows.Application;
using Binding = System.Windows.Data.Binding;
using DataGrid = System.Windows.Controls.DataGrid;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;

namespace YALV.Common
{
    public class FilteredGridManager
        : FilteredGridManagerBase
    {
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

        public void BuildDataGrid(IList<ColumnItem> columns, List<ColumnItem> newColumns = null)
        {
            if (_dg == null)
                return;

            if (_filterPropertyList == null)
                _filterPropertyList = new List<string>();
            else
                _filterPropertyList.Clear();

            if (columns != null)
            {
				_dg.Columns.Clear();
				_txtSearchPanel.Children.Clear();
                foreach (ColumnItem item in columns)
                {
                    DataGridTextColumn col = new DataGridTextColumn();

					col.Header = item.Header;
                    if (item.Alignment == CellAlignment.CENTER && _centerCellStyle != null)
                        col.CellStyle = _centerCellStyle;
                    if (item.MinWidth != null)
                        col.MinWidth = item.MinWidth.Value;
                    if (item.Width != null)
                        col.Width = item.Width.Value;

	                Binding bind;
	                if (newColumns != null && newColumns.Count > 0)
	                {
		                if (newColumns.Contains(item))
		                {
			                bind = new Binding(string.Format("CustomFields[{0}]", item.Field)) {Mode = BindingMode.OneWay};
		                }
		                else
		                {
							bind = new Binding(item.Field) { Mode = BindingMode.OneWay };
						}
					}
	                else
	                {
						bind = new Binding(item.Field) { Mode = BindingMode.OneWay };
					}

					bind.ConverterCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName);
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
                        txt.Name = getTextBoxName(item.Field.Replace(" ", ""));
                        txt.ToolTip = String.Format(Resources.FilteredGridManager_BuildDataGrid_FilterTextBox_Tooltip, item.Header);
                        txt.Tag = txt.ToolTip.ToString().ToLower();
                        txt.Text = string.Empty;
                        txt.AcceptsReturn = false;
                        txt.SetBinding(TextBox.WidthProperty, widthBind);
                        _filterPropertyList.Add(item.Field.Replace(" ",""));
                        if (_keyUpEvent != null)
                            txt.KeyUp += _keyUpEvent;

                        RegisterControl<TextBox>(_txtSearchPanel, txt.Name, txt);
                        _txtSearchPanel.Children.Add(txt);
                    }
                }
            }

            _dg.ColumnReordered += OnColumnReordered;
        }

        #endregion

        #region Private methods

        private void OnColumnReordered(object sender, DataGridColumnEventArgs dataGridColumnEventArgs)
        {
            if (dataGridColumnEventArgs.Column == null || !(dataGridColumnEventArgs.Column is DataGridBoundColumn))
                return;

            Binding colBind = ((DataGridBoundColumn)dataGridColumnEventArgs.Column).Binding as Binding;
            if (colBind == null || colBind.Path == null)
                return;

            string field = colBind.Path.Path;
            if (String.IsNullOrWhiteSpace(field))
                return;

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

        #endregion
    }
}
