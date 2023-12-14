using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YALV.Core;
using YALV.Core.Domain;

namespace YALV.Common
{
    public class FilteredGridManagerBase
        : DisposableObject
    {
        public FilteredGridManagerBase(DataGrid dg, Panel txtSearchPanel, KeyEventHandler keyUpEvent)
        {
            _dg = dg;
            _txtSearchPanel = txtSearchPanel;
            _keyUpEvent = keyUpEvent;
            _filterPropertyList = new List<string>();
            IsFilteringEnabled = true;
        }

        protected override void OnDispose()
        {
            ClearCache();
            if (_filterPropertyList != null)
                _filterPropertyList.Clear();
            if (_dg != null)
                _dg.Columns.Clear();
            if (_cvs != null)
            {
                if (_cvs.View != null)
                    _cvs.View.Filter = null;
                BindingOperations.ClearAllBindings(_cvs);
            }
            base.OnDispose();
        }

        #region Private Properties

        protected IList<string> _filterPropertyList;
        protected DataGrid _dg;
        protected Panel _txtSearchPanel;
        protected KeyEventHandler _keyUpEvent;
        protected CollectionViewSource _cvs;
        protected Hashtable _txtCache;

        #endregion

        #region Public Methods

        public virtual void AssignSource(Binding sourceBind)
        {
            if (_cvs == null)
                _cvs = new CollectionViewSource();
            else
                BindingOperations.ClearBinding(_cvs, CollectionViewSource.SourceProperty);

            BindingOperations.SetBinding(_cvs, CollectionViewSource.SourceProperty, sourceBind);
            BindingOperations.ClearBinding(_dg, DataGrid.ItemsSourceProperty);
            Binding bind = new Binding() { Source = _cvs, Mode = BindingMode.OneWay };
            _dg.SetBinding(DataGrid.ItemsSourceProperty, bind);
        }

        public ICollectionView GetCollectionView()
        {
            if (_cvs != null)
            {
                //Assign filter method
                if (_cvs.View != null && _cvs.View.Filter == null)
                {
                    IsFilteringEnabled = false;
                    _cvs.View.Filter = itemCheckFilter;
                    IsFilteringEnabled = true;
                }
                return _cvs.View;
            }
            return null;
        }

        public void ResetSearchTextBox()
        {
            if (_filterPropertyList != null && _txtSearchPanel != null)
            {
                //Clear all textbox text
                foreach (string prop in _filterPropertyList)
                {
                    TextBox txt = _txtSearchPanel.FindName(getTextBoxName(prop)) as TextBox;
                    if (txt != null & !string.IsNullOrEmpty(txt.Text))
                        txt.Text = string.Empty;
                }
            }
        }

        public void ClearCache()
        {
            if (_txtCache != null)
                _txtCache.Clear();
        }

        public Func<object, bool> OnBeforeCheckFilter;

        public Func<object, bool, bool> OnAfterCheckFilter;

        public bool IsFilteringEnabled { get; set; }

        #endregion

        #region Private Methods

        protected string getTextBoxName(string prop)
        {
            return string.Format("txtFilter{0}", prop).Replace(".", "");
        }

        protected bool itemCheckFilter(object item)
        {
            bool res = true;

            if (!IsFilteringEnabled)
                return res;

            try
            {
                if (OnBeforeCheckFilter != null)
                    res = OnBeforeCheckFilter(item);

                if (!res)
                    return res;

                if (_filterPropertyList != null && _txtSearchPanel != null)
                {
                    //Check each filter property
                    foreach (string prop in _filterPropertyList)
                    {
                        TextBox txt = null;
						txt = _txtSearchPanel.FindName(getTextBoxName(prop)) as TextBox;

						res = false;
                        if (txt == null)
                            res = true;
                        else
                        {
                            if (string.IsNullOrEmpty(txt.Text))
                                res = true;
                            else
                            {
                                try
                                {
                                    //Get property value
                                    object val = getItemValue(item, prop);
                                    if (val != null)
                                    {
                                        string valToCompare = string.Empty;
                                        if (val is DateTime)
                                            valToCompare = ((DateTime)val).ToString(GlobalHelper.DisplayDateTimeFormat, System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName));
                                        else
                                            valToCompare = val.ToString();

                                        if (valToCompare.ToString().IndexOf(txt.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                                            res = true;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                    res = true;
                                }
                            }
                        }
                        if (!res)
                            return res;
                    }
                }
                res = true;
            }
            finally
            {
                if (OnAfterCheckFilter != null)
                    res = OnAfterCheckFilter(item, res);

            }
            return res;
        }

        protected object getItemValue(object item, string prop)
        {
            object val = null;

	        var type = item.GetType();
	        var properties = type.GetProperties();

			var isBase = properties.Any(p => p.Name == prop);
			if (isBase)
			{
				val = type.GetProperty(prop).GetValue(item, null);
			}
			else
			{
				try
				{
					var custom = type.GetProperty("CustomFields");
					var inf = custom.GetValue(item, null);

					val = ((Dictionary<string, string>)inf)[prop];
				}
				catch (Exception)
				{
					val = null;
				}
			}
			return val;
        }

        #endregion
    }

}