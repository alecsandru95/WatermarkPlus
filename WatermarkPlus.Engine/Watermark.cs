using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace WatermarkPlus.Engine
{
	public static class Watermark
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(Watermark));

		public static void MarkImages(string inputFolderName, string inputWatermarkName, string outputFolderName, bool forceWrite = true)
		{
			var waterBulkArgs = new WatermarkBulkArgs()
			{
				InputFolder = inputFolderName,
				InputWaterImage = inputWatermarkName,
				OutputFolder = outputFolderName,
				ForceWrite = forceWrite
			};

			MarkImages(waterBulkArgs);
		}

		public static void MarkImages(WatermarkBulkArgs waterBulkArgs)
		{
			if (string.IsNullOrEmpty(waterBulkArgs.InputFolder) || Directory.Exists(waterBulkArgs.InputFolder) == false)
			{
				_Log.WriteException($"{nameof(waterBulkArgs.InputFolder)} does not exists : [{waterBulkArgs.InputFolder}]");
				return;
			}
			if (string.IsNullOrEmpty(waterBulkArgs.InputWaterImage) || File.Exists(waterBulkArgs.InputWaterImage) == false)
			{
				_Log.WriteException($"{nameof(waterBulkArgs.InputWaterImage)} does not exists : [{waterBulkArgs.InputWaterImage}]");
				return;
			}
			if (string.IsNullOrEmpty(waterBulkArgs.OutputFolder) || Directory.Exists(waterBulkArgs.OutputFolder) == false)
			{
				_Log.WriteException($"{nameof(waterBulkArgs.OutputFolder)} does not exists : [{waterBulkArgs.OutputFolder}]");
				return;
			}

			_Log.WriteInfo($"Input folder : {waterBulkArgs.InputFolder}");
			_Log.WriteInfo($"Input water : {waterBulkArgs.InputWaterImage}");
			_Log.WriteInfo($"Output folder : {waterBulkArgs.OutputFolder}");
			_Log.WriteInfo($"Force write : {waterBulkArgs.ForceWrite}");
			_Log.WriteInfo($"Watermark scale : {waterBulkArgs.WaterScale}");

			var inputImagesPaths = ImageHelper.GetAllImages(waterBulkArgs.InputFolder);

			if (inputImagesPaths == null || inputImagesPaths.Count == 0)
			{
				_Log.WriteWarning($"No images found in {nameof(waterBulkArgs.InputFolder)} directory : {waterBulkArgs.InputFolder}");
				return;
			}

			var waterArgs = inputImagesPaths.Select(
					(i, index) => new WatermarkArgs
					{
						InputBaseImage = i,
						OutputImage = Path.Combine(waterBulkArgs.OutputFolder, $"{Path.GetFileNameWithoutExtension(i)}_W.png"),
						InputWaterImage = waterBulkArgs.InputWaterImage,
						ForceWrite = waterBulkArgs.ForceWrite,
						ParentTask = waterBulkArgs.ParentTask,
						WaterScale = waterBulkArgs.WaterScale
					});

			waterBulkArgs.ParentTask?.ResetProgress(waterArgs.Count());

			MarkImages(waterArgs);
		}

		public static void MarkImages(IEnumerable<WatermarkArgs> waterArgsQuerry)
		{
			try
			{
				_Log.WriteInfoHightligh($"Will try to watermark {waterArgsQuerry.Count()} images");

				waterArgsQuerry.AsParallel().ForAll(wa => MarkImage(wa));
			}
			catch (Exception exception)
			{
				_Log.WriteException(exception);
			}
		}

		public static bool MarkImage(WatermarkArgs watermarkArgs)
		{
			try
			{
				var baseImageName = watermarkArgs.InputBaseImage;
				var waterImageName = watermarkArgs.InputWaterImage;
				var outputImageName = watermarkArgs.OutputImage;
				var waterScale = watermarkArgs.WaterScale;

				if (File.Exists(outputImageName) && watermarkArgs.ForceWrite == false)
				{
					_Log.WriteWarning($"File already exists! Cancelling operation for {outputImageName}");
					return false;
				}

				using (var baseImage = Image.FromFile(baseImageName))
				using (var waterImage = Image.FromFile(waterImageName))
				using (var baseGraphics = Graphics.FromImage(baseImage))
				using (var waterBrush = new TextureBrush(waterImage, System.Drawing.Drawing2D.WrapMode.Tile))
				{
					waterBrush.RotateTransform(-45);
					waterBrush.ScaleTransform((float)waterScale, (float)waterScale);

					baseGraphics.FillRectangle(waterBrush,
						new Rectangle(
							new Point(0, 0),
							new Size(baseImage.Width, baseImage.Height)
					));

					baseImage.Save(outputImageName, ImageFormat.Png);
				}

				watermarkArgs.ParentTask?.IncrementProgress();

				return true;
			}
			catch (Exception exception)
			{
				_Log.WriteException(exception);
			}

			return false;
		}
	}
}
