using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class ReportToolTip : Action
	{
		internal string Caption;

		public ReportToolTip(string id, RPLFormat.ShapeType shape, RectangleF position, float[] path, string caption)
			: base(id, null, ActionType.None, shape, position, path)
		{
			Caption = caption;
		}

		public ReportToolTip(string id, RectangleF position, string caption)
			: this(id, RPLFormat.ShapeType.Rectangle, position, null, caption)
		{
		}
	}
}
