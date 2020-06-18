using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class HotRegion
	{
		private GraphicsPath path;

		private bool relativeCoordinates = true;

		private RectangleF boundingRectangle = RectangleF.Empty;

		private object selectedObject;

		private int pointIndex = -1;

		private string seriesName = "";

		private ChartElementType type;

		private object selectedSubObject;

		internal GraphicsPath Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
			}
		}

		internal bool RelativeCoordinates
		{
			get
			{
				return relativeCoordinates;
			}
			set
			{
				relativeCoordinates = value;
			}
		}

		internal RectangleF BoundingRectangle
		{
			get
			{
				return boundingRectangle;
			}
			set
			{
				boundingRectangle = value;
			}
		}

		internal object SelectedObject
		{
			get
			{
				return selectedObject;
			}
			set
			{
				selectedObject = value;
			}
		}

		internal object SelectedSubObject
		{
			get
			{
				return selectedSubObject;
			}
			set
			{
				selectedSubObject = value;
			}
		}

		internal int PointIndex
		{
			get
			{
				return pointIndex;
			}
			set
			{
				pointIndex = value;
			}
		}

		internal string SeriesName
		{
			get
			{
				return seriesName;
			}
			set
			{
				seriesName = value;
			}
		}

		internal ChartElementType Type
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
	}
}
