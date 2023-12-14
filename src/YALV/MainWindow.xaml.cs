#region About
/*
 * YALV! - Yet Another Log4Net Viewer
 * 
 * YALV! is a log viewer for Log4Net that allow to compare multiple logs file simultaneously.
 * Log4Net config file must be setup with XmlLayoutSchemaLog4j layout.
 * It is a WPF Application based on .NET Framework 4.0 and written with C# language.
 *
 * An open source application developed by Luca Petrini - http://www.linkedin.com/in/lucapetrini
 * 
 * Copyright: (c) 2012 Luca Petrini
 * 
 * YALV! is a free software distributed on CodePlex: http://yalv.codeplex.com/ under the Microsoft Public License (Ms-PL)
 */
#endregion

using System;
using System.Configuration;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using YALV.Common;
using YALV.Common.Interfaces;
using YALV.ViewModel;

namespace YALV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IWinSimple
    {
        public MainWindow(string[] args)
        {
            InitCulture();

            InitializeComponent();

            //Initialize and assign ViewModel
            MainWindowVM _vm = new MainWindowVM(this);
            _vm.GridManager = new FilteredGridManager(dgItems, txtSearchPanel, delegate (object sender, KeyEventArgs e)
            {
                if (e.OriginalSource is TextBox)
                    _vm.RefreshView();
            });
            _vm.InitDataGrid();
            _vm.RecentFileList = mainMenu.RecentFileList;
            _vm.RefreshUI = OnRefreshUI;
            this.DataContext = _vm;

	        dgItems.EnableColumnVirtualization = true;
	        dgItems.EnableRowVirtualization = true;

            //Assign events
            dgItems.SelectionChanged += dgItems_SelectionChanged;
            txtItemId.KeyUp += txtItemId_KeyUp;

            this.Loaded += delegate
            {
                if (args != null && args.Length > 0)
                    _vm.LoadFileList(args);
            };
            this.Closing += delegate
            {
                dgItems.SelectionChanged -= dgItems_SelectionChanged;
                txtItemId.KeyUp -= txtItemId_KeyUp;
                _vm.Dispose();
            };
            this.Drop += (object sender, DragEventArgs e) =>
            {
                if (e.Data != null)
                {
                    string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop);
                    bool add = e.KeyStates.HasFlag(DragDropKeyStates.ControlKey);
                    _vm.LoadFileList(pathList, add);
                }
            };
        }

        public static System.Globalization.CultureInfo ResolvedCulture
        {
            get { return System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName); }
        }

        private void txtItemId_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Enter || e.Key == Key.Return) && Keyboard.Modifiers == ModifierKeys.None)
            {
                OnRefreshUI(MainWindowVM.NOTIFY_ScrollIntoView);
            }
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnRefreshUI(MainWindowVM.NOTIFY_ScrollIntoView);
        }

        private void InitCulture()
        {
            try
            {
                var culture = ConfigurationManager.AppSettings["Culture"];
                if (!String.IsNullOrWhiteSpace(culture))
                    Properties.Resources.Culture = new CultureInfo(culture);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, String.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void OnRefreshUI(string eventName, object parameter = null)
        {
            try
            {
				
                switch (eventName)
                {
                    case MainWindowVM.NOTIFY_ScrollIntoView:
                        if (dgItems != null && dgItems.SelectedItem != null)
                        {
                            dgItems.UpdateLayout();
                            dgItems.ScrollIntoView(dgItems.SelectedItem);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, String.Empty, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
