using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class RectangleA
	{
		private ValueA x = new ValueA();

		private ValueA y = new ValueA();

		private ValueA width = new ValueA();

		private ValueA height = new ValueA();

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

		public ValueA Width
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		public ValueA Height
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public RectangleF ToRectangleF()
		{
			return new RectangleF(x.EndValue, y.EndValue, width.EndValue, height.EndValue);
		}
	}
}
