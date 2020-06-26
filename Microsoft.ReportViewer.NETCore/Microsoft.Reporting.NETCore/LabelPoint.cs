using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class LabelPoint : ActionPoint
	{
		internal LabelPoint(PointF point)
			: base(point)
		{
		}

		internal LabelPoint(float x, float y)
			: this(new PointF(x, y))
		{
		}
	}
}
