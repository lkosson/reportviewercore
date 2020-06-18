using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class HitTestResult
	{
		private PointF htPoint;

		private object obj;

		private HotRegion region;

		public object Object => obj;

		public bool Success => obj != null;

		public string Name
		{
			get
			{
				if (obj != null)
				{
					if (obj is NamedElement)
					{
						return ((NamedElement)obj).Name;
					}
					return obj.ToString();
				}
				return null;
			}
		}

		public double ScaleValue
		{
			get
			{
				if (obj is ScaleBase)
				{
					return ((ScaleBase)obj).GetValue(region.PinPoint, htPoint);
				}
				return 0.0;
			}
		}

		internal HitTestResult(HotRegion region, PointF hitTestPoint)
		{
			this.region = region;
			if (region != null)
			{
				obj = region.SelectedObject;
			}
			htPoint = hitTestPoint;
		}
	}
}
