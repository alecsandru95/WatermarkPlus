namespace WatermarkPlus.Engine
{
	public class WatermarkArgs
	{
		public string InputBaseImage { get; set; }
		public string InputWaterImage { get; set; }
		public string OutputImage { get; set; }

		public bool ForceWrite { get; set; } = true;
		public double WaterScale { get; set; } = 1;

		public WatermarkTask ParentTask { get; set; }
	}
}
