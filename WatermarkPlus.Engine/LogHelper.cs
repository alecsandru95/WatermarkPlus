using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatermarkPlus.Engine
{
	public class LogHelper
	{
		private string _Source = "";

		public LogHelper(Type typeSource)
		{
			_Source = typeSource.Name;
		}

		public void WriteInfo(string line)
		{
			Log.WriteLine("INFO", $"{_Source} -> {line}", ConsoleColor.White);
		}

		public void WriteInfoHightligh(string line)
		{
			Log.WriteLine("INFO", $"{_Source} -> {line}", ConsoleColor.Green);
		}

		public void WriteWarning(string line)
		{
			Log.WriteLine("WARNING", $"{_Source} -> {line}", ConsoleColor.Yellow);
		}

		public void WriteException(Exception exception)
		{
			if(exception != null)
			{
				Log.WriteLine("ERROR", $"{_Source} -> {exception.Message}", ConsoleColor.Red);
			}
			else
			{
				Log.WriteLine("ERROR", $"{_Source} -> NO EXCEPTION TO WRITE", ConsoleColor.Red);
			}
		}

		public void WriteException(string line)
		{
			Log.WriteLine("ERROR", $"{_Source} -> {line}", ConsoleColor.Red);
		}
	}
}
