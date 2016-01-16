using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YALV.Common
{
    /// <summary>
    /// Interaction logic for MainToolbar.xaml
    /// </summary>
    public partial class MainToolbar : ToolBar
    {
        public MainToolbar()
        {
            InitializeComponent();

            this.Loaded += delegate(object sender, RoutedEventArgs e)
            {
                ToolBar toolBar = sender as ToolBar;
                if (toolBar != null)
                {
                    FrameworkElement overflowGrid = (FrameworkElement)toolBar.Template.FindName("OverflowGrid", toolBar);
                    if (overflowGrid != null)
                        overflowGrid.Visibility = Visibility.Collapsed;
                }
            };
        }
    }
}
