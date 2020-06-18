using System;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms.ChartTypes
{
	internal sealed class TreeMapNode
	{
		public double Value
		{
			get;
			set;
		}

		public RectangleF Rectangle
		{
			get;
			set;
		}

		public Series Series
		{
			get;
			set;
		}

		public DataPoint DataPoint
		{
			get;
			set;
		}

		public List<TreeMapNode> Children
		{
			get;
			set;
		}

		public TreeMapNode(Series series, double value)
		{
			Series = series;
			Value = Math.Abs(value);
		}

		public TreeMapNode(DataPoint dataPoint)
		{
			DataPoint = dataPoint;
			Value = Math.Abs(dataPoint.YValues[0]);
		}
	}
}
