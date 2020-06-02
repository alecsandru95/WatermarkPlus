using System;

namespace WatermarkPlus.Engine
{
	public class WatermarkTask
	{
		private static readonly LogHelper _Log = new LogHelper(typeof(WatermarkTask));

		private readonly object _Lock = new object();

		private int _Progress = 0;
		private int _MaxProgress = 1;

		public WatermarkBulkArgs WatermarkBulkArgs { get; set; }

		public event Action<double> OnProgressChanged;

		internal void ResetProgress(int maxProgress)
		{
			lock (_Lock)
			{
				_Progress = 0;
				_MaxProgress = maxProgress;

				OnProgressChangedInternal(_Progress, _MaxProgress);
			}
		}

		internal void IncrementProgress()
		{
			lock (_Lock)
			{
				_Progress++;

				OnProgressChangedInternal(_Progress, _MaxProgress);
			}
		}

		private void OnProgressChangedInternal(int progress, int maxProgress)
		{
			try
			{
				OnProgressChanged?.Invoke((double)_Progress / _MaxProgress);
			}
			catch (Exception exception)
			{
				_Log.WriteException(exception);
			}
		}
	}
}