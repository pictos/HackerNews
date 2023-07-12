using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace HackerNews.Droid;
[Activity(
	MainLauncher = true,
	ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
	WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
)]
public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
{
	protected override void OnCreate(Bundle bundle)
	{
		base.OnCreate(bundle);
		Android.FrameMetrics.FrameMetricsReporter.Initialize(this);
	}
}
