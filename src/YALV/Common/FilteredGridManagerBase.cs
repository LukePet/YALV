using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YALV.Core.Domain;

namespace YALV.Common
{
	public class FilteredGridManagerBase
		: DisposableObject
	{
		public FilteredGridManagerBase(DataGrid dg, Panel txtSearchPanel, KeyEventHandler keyUpEvent)
		{
			Dg = dg;
			TxtSearchPanel = txtSearchPanel;
			KeyUpEvent = keyUpEvent;
			FilterPropertyList = new List<string>();
			TxtCache = new Hashtable();
			IsFilteringEnabled = true;
		}

		protected override void OnDispose()
		{
			ClearCache();
			FilterPropertyList?.Clear();
			Dg?.Columns.Clear();
			if (Cvs != null)
			{
				if (Cvs.View != null)
					Cvs.View.Filter = null;
				BindingOperations.ClearAllBindings(Cvs);
			}
			base.OnDispose();
		}

		#region Private Properties

		protected IList<string> FilterPropertyList;
		protected DataGrid Dg;
		protected Panel TxtSearchPanel;
		protected KeyEventHandler KeyUpEvent;
		protected CollectionViewSource Cvs;
		protected Hashtable TxtCache;

	    private bool _goodregex;
	    private Timer _timer;

		#endregion

		#region Public Methods

		public virtual void AssignSource(Binding sourceBind)
		{
			if (Cvs == null)
				Cvs = new CollectionViewSource();
			else
				BindingOperations.ClearBinding(Cvs, CollectionViewSource.SourceProperty);

			BindingOperations.SetBinding(Cvs, CollectionViewSource.SourceProperty, sourceBind);
			BindingOperations.ClearBinding(Dg, ItemsControl.ItemsSourceProperty);
			Binding bind = new Binding() { Source = Cvs, Mode = BindingMode.OneWay };
			Dg.SetBinding(ItemsControl.ItemsSourceProperty, bind);
		}

	    private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _goodregex = false;
        }

        public ICollectionView GetCollectionView()
        {
            // Don't allow search to run more than 3 seconds
            _timer = new Timer(3000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Enabled = true;

			if (Cvs != null)
			{
				//Assign filter method
				if (Cvs.View != null && Cvs.View.Filter == null)
				{
					IsFilteringEnabled = false;
					Cvs.View.Filter = ItemCheckFilter;
					IsFilteringEnabled = true;
				}
				return Cvs.View;
			}
			return null;
		}

		public void ResetSearchTextBox()
		{
			if (FilterPropertyList != null && TxtSearchPanel != null)
			{
				//Clear all textbox text
				foreach (string prop in FilterPropertyList)
				{
					var txt = TxtSearchPanel.FindName(GetTextBoxName(prop)) as TextBox;

					if (!string.IsNullOrEmpty(txt?.Text))
						txt.Text = string.Empty;
				}
			}
		}

		public void ClearCache()
		{
			TxtCache?.Clear();
		}

		public Func<object, bool> OnBeforeCheckFilter;

		public Func<object, bool, bool> OnAfterCheckFilter;

		public bool IsFilteringEnabled { get; set; }

		#endregion

		#region Private Methods

		protected string GetTextBoxName(string prop)
		{
			return $"txtFilter{prop}".Replace(".", "");
		}

		protected bool ItemCheckFilter(object item)
		{
			bool res = true;

			if (!IsFilteringEnabled)
				return true;

			try
			{
				if (OnBeforeCheckFilter != null)
					res = OnBeforeCheckFilter(item);

				if (!res)
					return false;

				if (FilterPropertyList != null && TxtSearchPanel != null)
				{
					//Check each filter property
					foreach (string prop in FilterPropertyList)
					{
						TextBox txt;
						if (TxtCache.ContainsKey(prop))
							txt = TxtCache[prop] as TextBox;
						else
						{
							txt = TxtSearchPanel.FindName(GetTextBoxName(prop)) as TextBox;
							TxtCache[prop] = txt;
						}

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
									object val = GetItemValue(item, prop);

									if (val != null)
									{
										string valToCompare;
										if (val is DateTime)
											valToCompare = ((DateTime)val).ToString(GlobalHelper.DisplayDateTimeFormat, System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName));
										else
											valToCompare = val.ToString().ToLower();

										var pattern = txt.Text.ToLower();

									    var terms = pattern.Split(' ');

                                        // check for negated/added values
									    if (terms.Any(term => term.StartsWith("-") || term.StartsWith("+")))
									    {
									        res = true;

									        foreach (var term in terms)
									        {
									            if (term.StartsWith("-") && valToCompare.Contains(term.Substring(1)))
									            {
									                res = false;
									                break;
									            }

									            if (term.StartsWith("+") && valToCompare.Contains(term.Substring(1)))
									            {
									                res = true;
									            }
									            else
									            {
									                if (valToCompare.Contains(term))
									                {
									                    res = true;
									                }
									            }
									        }
									    }
                                        // check for regex
                                        else if (pattern.Length > 2 && pattern.StartsWith("/") && pattern.EndsWith("/") && _goodregex)
										{
											try
											{
												if (Regex.IsMatch(valToCompare, pattern.Substring(1, pattern.Length - 2)))
												{
													res = true;
												}
											}
											catch
											{
												res = true;
											    _goodregex = false;
											}
										}
                                        // normal search
										else
										{
											if (valToCompare.ToLower().IndexOf(pattern, StringComparison.Ordinal) >= 0)
											{
												res = true;
											}
										}

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
							return false;
					}
				}
			}
			finally
			{
				if (OnAfterCheckFilter != null)
					res = OnAfterCheckFilter(item, res);

			}

			return res;
		}

		protected object GetItemValue(object item, string prop)
		{
			object val;
			try
			{
				val = item.GetType().GetProperty(prop).GetValue(item, null);
			}
			catch
			{
				val = null;
			}
			return val;
		}

		#endregion
	}

}