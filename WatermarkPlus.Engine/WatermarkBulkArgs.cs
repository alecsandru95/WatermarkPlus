using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatermarkPlus.Engine
{
	public class WatermarkBulkArgs
	{
		public string InputFolder { get; set; }
		public string InputWaterImage { get; set; }
		public string OutputFolder { get; set; }

		public bool ForceWrite { get; set; } = true;
		public double WaterScale { get; set; } = 1;

		public WatermarkTask ParentTask { get; set; }
	}
}
