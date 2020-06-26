using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class DrillthroughAction : Action
	{
		internal string ReportId;

		public DrillthroughAction(string id, string label, RPLFormat.ShapeType shape, RectangleF position, float[] path, string reportId)
			: base(id, label, ActionType.DrillThrough, shape, position, path)
		{
			ReportId = reportId;
		}

		public DrillthroughAction(string id, string label, RectangleF position, string reportId)
			: this(id, label, RPLFormat.ShapeType.Rectangle, position, null, reportId)
		{
		}
	}
}
