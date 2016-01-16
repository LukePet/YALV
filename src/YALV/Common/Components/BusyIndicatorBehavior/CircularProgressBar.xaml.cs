namespace YALV.Common
{
	#region #using Directives

	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media.Animation;

	#endregion

	/// <summary>
	/// Provides a circular progress bar
	/// </summary>
	public partial class CircularProgressBar : UserControl
	{
		public CircularProgressBar()
		{
			InitializeComponent();
            tbMessage.Text = Properties.Resources.CircularProgressBar_CircularProgressBar_BusyText;
            Timeline.SetDesiredFrameRate(sbAnimation, BusyIndicatorBehavior.FRAMERATE);
		}
	}
}