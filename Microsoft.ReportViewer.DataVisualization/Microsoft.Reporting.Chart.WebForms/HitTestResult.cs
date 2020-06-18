namespace Microsoft.Reporting.Chart.WebForms
{
	internal class HitTestResult
	{
		private object obj;

		private Series series;

		private int dataPoint = -1;

		private ChartArea chartArea;

		private Axis axis;

		private ChartElementType type;

		private object subObject;

		public Series Series
		{
			get
			{
				return series;
			}
			set
			{
				series = value;
			}
		}

		public int PointIndex
		{
			get
			{
				return dataPoint;
			}
			set
			{
				dataPoint = value;
			}
		}

		public ChartArea ChartArea
		{
			get
			{
				return chartArea;
			}
			set
			{
				chartArea = value;
			}
		}

		public Axis Axis
		{
			get
			{
				return axis;
			}
			set
			{
				axis = value;
			}
		}

		public ChartElementType ChartElementType
		{
			get
			{
				return type;
			}
			set
			{
				type = value;
			}
		}

		public object Object
		{
			get
			{
				return obj;
			}
			set
			{
				obj = value;
			}
		}

		public object SubObject
		{
			get
			{
				return subObject;
			}
			set
			{
				subObject = value;
			}
		}
	}
}
