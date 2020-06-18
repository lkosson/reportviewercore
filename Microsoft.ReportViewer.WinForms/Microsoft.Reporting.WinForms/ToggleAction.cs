using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ToggleAction : Action
	{
		internal bool ToggleState;

		public ToggleAction(string id, string label, RPLFormat.ShapeType shape, RectangleF position, float[] path, bool state)
			: base(id, label, ActionType.Toggle, shape, position, path)
		{
			ToggleState = state;
		}

		public ToggleAction(string id, string label, RectangleF position, bool state)
			: this(id, label, RPLFormat.ShapeType.Rectangle, position, null, state)
		{
		}
	}
}
