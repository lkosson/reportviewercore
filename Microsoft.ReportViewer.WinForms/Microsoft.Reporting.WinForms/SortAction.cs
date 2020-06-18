using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class SortAction : Action
	{
		internal SortOrder Direction;

		public SortAction(string id, string label, RPLFormat.ShapeType shape, RectangleF position, float[] path, SortOrder direction)
			: base(id, label, ActionType.Sort, shape, position, path)
		{
			Direction = direction;
		}

		public SortAction(string id, string label, RectangleF position, SortOrder direction)
			: this(id, label, RPLFormat.ShapeType.Rectangle, position, null, direction)
		{
		}
	}
}
