using System.Windows;
using YALV.Common;

namespace YALV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            int? framerate = FrameRateHelper.DesiredFrameRate;
            BusyIndicatorBehavior.FRAMERATE = framerate;
            FrameRateHelper.SetTimelineDefaultFramerate(framerate);

            (new MainWindow(e.Args)).Show();
        }
    }
}
