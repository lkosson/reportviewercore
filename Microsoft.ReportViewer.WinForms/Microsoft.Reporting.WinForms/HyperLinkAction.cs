using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class HyperLinkAction : Action
	{
		internal string Url;

		public HyperLinkAction(string id, string label, RPLFormat.ShapeType shape, RectangleF position, float[] path, string url)
			: base(id, label, ActionType.HyperLink, shape, position, path)
		{
			Url = url;
		}

		public HyperLinkAction(string id, string label, RectangleF position, string url)
			: this(id, label, RPLFormat.ShapeType.Rectangle, position, null, url)
		{
		}
	}
}
