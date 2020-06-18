using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class BookmarkLinkAction : Action
	{
		internal string ActionLink;

		public BookmarkLinkAction(string id, string label, RPLFormat.ShapeType shape, RectangleF position, float[] path, string action)
			: base(id, label, ActionType.BookmarkLink, shape, position, path)
		{
			ActionLink = action;
		}

		public BookmarkLinkAction(string id, string label, RectangleF position, string action)
			: this(id, label, RPLFormat.ShapeType.Rectangle, position, null, action)
		{
		}
	}
}
