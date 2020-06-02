using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WatermarkPlus.Engine
{
	public static class ImageHelper
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(ImageHelper));

		private static HashSet<string> _ImageExtensionSet = new HashSet<string> { ".jpg", ".png" };

		public static List<string> GetAllImages(string directory)
		{
			try
			{
				var filesQuerry = Directory
					.EnumerateFiles(directory, "*.*")
					.Where(f => _ImageExtensionSet.Contains(Path.GetExtension(f).ToLowerInvariant()));

				return filesQuerry.ToList();
			}
			catch (Exception exception)
			{
				_Log.WriteException(exception);
			}

			return null;
		}
	}
}
