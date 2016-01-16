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
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Menu
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public RecentFileList RecentFileList
        {
            get 
            {
                return this.RecentFileListMenu;
            }
        }
    }
}
