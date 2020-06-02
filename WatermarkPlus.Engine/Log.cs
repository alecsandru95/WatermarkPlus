using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatermarkPlus.Engine
{
	public class Log
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(Log));

		private static readonly object _Lock = new object();

		public static Log Instance { get; private set; } = new Log();

		private StreamWriter _LogFile;

		private Log()
		{
			try
			{
				_LogFile = new StreamWriter($"watermark_log_{DateTime.Now.ToFileTime()}.log");
				_LogFile.AutoFlush = true;
			}
			catch(Exception exception)
			{
				_Log.WriteException(exception);
			}
		}

		public static void WriteLine(string messageType, string message, ConsoleColor consoleColor)
		{
			lock (_Lock)
			{
				string line = $"{messageType} [{Thread.CurrentThread.ManagedThreadId,2}] {message}";
				WriteLineFile(line);
				WriteLineConsole(line, consoleColor);
			}
		}


		private static void WriteLineFile(string line)
		{
			Instance._LogFile?.WriteLine(line);
		}

		private static void WriteLineConsole(string line, ConsoleColor consoleColor)
		{
			Console.ForegroundColor = consoleColor;
			Console.WriteLine(line);
		}
	}
}