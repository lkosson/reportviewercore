using System.Drawing;

namespace Microsoft.Reporting.WinForms
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
