using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class PointA
	{
		private ValueA x = new ValueA();

		private ValueA y = new ValueA();

		public ValueA X
		{
			get
			{
				return x;
			}
			set
			{
				x = value;
			}
		}

		public ValueA Y
		{
			get
			{
				return y;
			}
			set
			{
				y = value;
			}
		}

		public PointF ToPointF()
		{
			return new PointF(x.EndValue, y.EndValue);
		}
	}
}
