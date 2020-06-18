using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ColorA
	{
		private Color startColor = Color.Transparent;

		private Color endColor = Color.Transparent;

		private double startTime;

		private double endTime;

		private bool repeat;

		private double repeatDelay;

		public Color StartColor
		{
			get
			{
				return startColor;
			}
			set
			{
				startColor = value;
			}
		}

		public Color EndColor
		{
			get
			{
				return endColor;
			}
			set
			{
				endColor = value;
			}
		}

		public double StartTime
		{
			get
			{
				return startTime;
			}
			set
			{
				startTime = value;
			}
		}

		public double EndTime
		{
			get
			{
				return endTime;
			}
			set
			{
				endTime = value;
			}
		}

		public bool Repeat
		{
			get
			{
				return repeat;
			}
			set
			{
				repeat = value;
			}
		}

		public double RepeatDelay
		{
			get
			{
				return repeatDelay;
			}
			set
			{
				repeatDelay = value;
			}
		}

		internal ColorA Copy()
		{
			return new ColorA
			{
				endColor = endColor,
				endTime = endTime,
				repeat = repeat,
				startColor = startColor,
				startTime = startTime
			};
		}
	}
}
