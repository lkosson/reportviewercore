using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class DrawablePage
	{
		public abstract ReportActions Actions
		{
			get;
		}

		public abstract Dictionary<string, BookmarkPoint> Bookmarks
		{
			get;
		}

		public abstract ReportActions ToolTips
		{
			get;
		}

		public abstract bool NeedsFrame
		{
			get;
		}

		public abstract int ExternalMargin
		{
			get;
		}

		public abstract bool DrawInPixels
		{
			get;
		}

		public abstract bool IsRequireFullRedraw
		{
			get;
		}

		public abstract void Draw(Graphics g, PointF scrollOffset, bool testMode);

		public abstract void GetPageSize(Graphics g, out float width, out float height);

		public abstract void BuildInteractivityInfo(Graphics g);
	}
}
