using System.Windows;
using YALV.Common;
using YALV.Common.Interfaces;
using YALV.ViewModel;

namespace YALV
{
    /// <summary>
    /// Interaction logic for SelectColumns.xaml
    /// </summary>
    public partial class SelectColumns : Window, IWinSimple
    {
        public SelectColumns()
        {
            InitializeComponent();
        }

        public bool? ShowDialog(FilteredGridManager filteredGrid)
        {
            using (var vm = new SelectColumnsVM(this, filteredGrid))
            {
                this.DataContext = vm;
                return this.ShowDialog();
            }
        }
    }
}
