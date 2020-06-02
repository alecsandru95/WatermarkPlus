using System;
using System.Windows;
using WatermarkPlus.Engine;

namespace WatermarkPlusApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(App));

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			_Log.WriteInfoHightligh("App started");

			if (e?.Args != null && e.Args.Length != 0)
			{
				if (e.Args.Length != 3)
				{
					_Log.WriteException("3 arguments are required");
					_Log.WriteException("INPUT_FOLDER INPUT_WATERMARK_IMAGE OUTPUT_FOLDER");
					Current.Shutdown();

					return;
				}

				var inputFolder = e.Args[0];
				var inputWater = e.Args[1];
				var outputFolder = e.Args[2];

				Watermark.MarkImages(inputFolder, inputWater, outputFolder);

				_Log.WriteInfo("Closing app");
				Current.Shutdown();
			}
			else
			{
				var mainWindow = new MainWindow();
				mainWindow.Show();
				mainWindow.Closed += MainWindow_Closed;
			}
		}

		private void MainWindow_Closed(object sender, EventArgs e)
		{
			_Log.WriteInfo("Closing app");
			Current.Shutdown();
		}
	}
}
