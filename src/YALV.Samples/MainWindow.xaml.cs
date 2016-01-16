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

namespace YALV.Samples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 5000; i++)
            {
                int value = r.Next(13);

                switch (value)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        method1();
                        break;
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                        method2();
                        break;
                    case 9:
                        method3();
                        break;
                    case 10:
                    case 11:
                        method4();
                        break;
                    case 12:
                        method5();
                        break;
                }
            }
            MessageBox.Show("Generation Complete!", "YALV! Samples", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void method1()
        {
            LogService.Trace.Debug("This is a debug message");
        }

        private void method2()
        {
            LogService.Trace.Info("This is an information message");
        }

        private void method3()
        {
            LogService.Trace.Warn("This is a warning message!", new Exception("Warning Exception!"));
        }

        private void method4()
        {
            LogService.Trace.Error("This is an error message!", new Exception("Warning Exception!"));
        }

        private void method5()
        {
            LogService.Trace.Fatal("This is a fatal message!", new Exception("Warning Exception!"));
        }
    }
}
