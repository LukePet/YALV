using System.Diagnostics;
using System.Windows;
using System;

namespace YALV
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();

            FileVersionInfo verInfo = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string version = String.Format(Properties.Resources.About_Version_Text, verInfo != null ? verInfo.FileVersion : "---");
            lblVersion.Text = version;

            string config1 = @"<log4net>
    <appender name=""FileAppenderXml"" type=""log4net.Appender.FileAppender"">
        <file type=""log4net.Util.PatternString"" value=""sample-log.xml""/>
        <appendToFile value=""true""/>        
        <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
            <locationInfo value=""true""/>
        </layout>
        <param name = ""Encoding"" value=""utf-8"" />
    </appender>
    <!-- other appenders defined here -->

    <root>
        <level value=""ALL"" />
        <appender-ref ref=""FileAppenderXml"" />
        <!-- other appenders enabled here -->
    </root>
</log4net>";
            tbConfig1.Text = config1;

            string config2 = @"<log4net>
    <appender name=""RollingFileAppenderXml"" type=""log4net.Appender.RollingFileAppender"">
        <file type=""log4net.Util.PatternString"" value=""sample-log.xml""/>
        <appendToFile value=""true""/>
        <datePattern value=""yyyyMMdd""/>
        <rollingStyle value=""Size""/>
        <maxSizeRollBackups value=""5""/>
        <maximumFileSize value=""5000KB""/>
        <layout type=""log4net.Layout.XmlLayoutSchemaLog4j"">
            <locationInfo value=""true""/>
        </layout>
        <param name = ""Encoding"" value=""utf-8"" />
    </appender>
    <!-- other appenders defined here -->

    <root>
        <level value=""ALL"" />
        <appender-ref ref=""RollingFileAppenderXml"" />
        <!-- other appenders enabled here -->
    </root>
</log4net>";
            tbConfig2.Text = config2;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
