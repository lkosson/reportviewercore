using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal sealed class PDFPagePoint
	{
		internal int PageObjectId;

		internal PointF Point;

		internal PDFPagePoint(int pageObjectId, PointF point)
		{
			PageObjectId = pageObjectId;
			Point = point;
		}
	}
}
