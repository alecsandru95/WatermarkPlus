using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WatermarkPlus.Engine;

namespace WatermarkPlusApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(MainWindow));

		public MainWindow()
		{
			InitializeComponent();
		}

		private void _BtnInputFolder_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (Directory.Exists(dialog.SelectedPath) == false)
				{
					_Log.WriteException($"Directory invalid : [{dialog.SelectedPath}]");
				}
				else
				{
					_TxtInputFolder.Text = dialog.SelectedPath;
				}
			}
		}

		private void _BtnInputWater_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog()
			{
				Multiselect = false,
				Filter = "PNG images (*.png)|*.png|JPG images (*.jpg)|*.jpg"
			};

			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (File.Exists(dialog.FileName) == false)
				{
					_Log.WriteException($"File invalid : [{dialog.FileName}]");
				}
				else
				{
					_TxtInputWater.Text = dialog.FileName;
				}
			}
		}

		private void _BtnOutputFolder_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				if (Directory.Exists(dialog.SelectedPath) == false)
				{
					_Log.WriteException($"Directory invalid : [{dialog.SelectedPath}]");
				}
				else
				{
					_TxtOutputFolder.Text = dialog.SelectedPath;
				}
			}
		}

		private void _BtnWork_Click(object sender, RoutedEventArgs e)
		{
			string inputFolder = _TxtInputFolder.Text;
			string inputWater = _TxtInputWater.Text;
			string outputFolder = _TxtOutputFolder.Text;

			if (string.IsNullOrEmpty(inputFolder))
			{
				_Log.WriteException($"Input folder not valid");
				return;
			}
			if (string.IsNullOrEmpty(inputWater))
			{
				_Log.WriteException($"Input watermark not valid");
				return;
			}
			if (string.IsNullOrEmpty(outputFolder))
			{
				_Log.WriteException($"Output folder not valid");
				return;
			}

			try
			{
				inputFolder = Path.GetFullPath(inputFolder);
				inputWater = Path.GetFullPath(inputWater);
				outputFolder = Path.GetFullPath(outputFolder);

				_TxtInputFolder.Text = inputFolder;
				_TxtInputWater.Text = inputWater;
				_TxtOutputFolder.Text = outputFolder;
			}
			catch (System.Exception exception)
			{
				_Log.WriteException(exception);
				return;
			}

			double waterScale = 1;
			if(double.TryParse(_TxtWaterScale.Text, out waterScale) == false)
			{
				_Log.WriteWarning($"Watermark scale not valid : [{_TxtWaterScale.Text}], using default scale 0.5");
			}
			else
			{
				_TxtWaterScale.Text = waterScale.ToString();
			}

			_BtnProcessImages.IsEnabled = false;
			_PrgWaterProcess.Value = 0;

			var waterTask = new WatermarkTask();
			waterTask.WatermarkBulkArgs = new WatermarkBulkArgs()
			{
				InputFolder = inputFolder,
				InputWaterImage = inputWater,
				OutputFolder = outputFolder,
				ParentTask = waterTask,
				ForceWrite = true,
				WaterScale = waterScale
			};
			waterTask.OnProgressChanged += WaterTask_OnProgressChanged;

			Task.Run(() =>
			{
				Watermark.MarkImages(waterTask.WatermarkBulkArgs);
			}).ContinueWith(t =>
			{
				if (t.Exception != null)
				{
					_Log.WriteException(t.Exception);
				}
				Dispatcher.Invoke(() =>
				{
					_BtnProcessImages.IsEnabled = true;
				});
			});
		}

		private void WaterTask_OnProgressChanged(double obj)
		{
			Dispatcher.Invoke(() =>
			{
				_PrgWaterProcess.Value = obj;
			});
		}
	}
}
