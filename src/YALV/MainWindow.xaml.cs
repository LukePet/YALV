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
            initCulture();

            InitializeComponent();

            //Initialize and assign ViewModel
            MainWindowVM _vm = new MainWindowVM(this);
            _vm.GridManager = new FilteredGridManager(dgItems, txtSearchPanel, delegate(object sender, KeyEventArgs e)
            {
                if (e.OriginalSource is TextBox)
                    _vm.RefreshView();
            });
            _vm.InitDataGrid();
            _vm.RecentFileList = mainMenu.RecentFileList;
            this.DataContext = _vm;

            //Assign events
            dgItems.SelectionChanged += dgItems_SelectionChanged;
            this.Loaded += delegate
            {
                if (args != null && args.Length > 0)
                    _vm.LoadFileList(args);
            };
            this.Closing += delegate
            {
                dgItems.SelectionChanged -= dgItems_SelectionChanged;
                _vm.Dispose();
            };
        }

        public static System.Globalization.CultureInfo ResolvedCulture
        {
            get { return System.Globalization.CultureInfo.GetCultureInfo(Properties.Resources.CultureName); }
        }

        private void dgItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgItems.SelectedItem != null)
            {
                dgItems.UpdateLayout();
                dgItems.ScrollIntoView(dgItems.SelectedItem);
            }
        }

        private void initCulture()
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
    }
}
