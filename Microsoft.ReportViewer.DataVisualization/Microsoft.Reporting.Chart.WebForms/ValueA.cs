namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ValueA
	{
		private float startValue;

		private float endValue;

		private double startTime;

		private double endTime;

		private bool repeat;

		private double repeatDelay;

		public float StartValue
		{
			get
			{
				return startValue;
			}
			set
			{
				startValue = value;
			}
		}

		public float EndValue
		{
			get
			{
				return endValue;
			}
			set
			{
				endValue = value;
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
	}
}
