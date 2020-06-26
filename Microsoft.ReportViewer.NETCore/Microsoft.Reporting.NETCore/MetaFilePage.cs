using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class MetaFilePage : DrawablePage
	{
		private Metafile m_metaFile;

		private float m_pageWidth;

		private float m_pageHeight;

		public override ReportActions Actions => new ReportActions();

		public override Dictionary<string, BookmarkPoint> Bookmarks => new Dictionary<string, BookmarkPoint>();

		public override ReportActions ToolTips => new ReportActions();

		public override bool NeedsFrame => true;

		public override int ExternalMargin => 20;

		public override bool DrawInPixels => true;

		public override bool IsRequireFullRedraw => false;

		public MetaFilePage(Stream metaFileStream, PageSettings pageSettings)
		{
			metaFileStream.Position = 0L;
			m_metaFile = new Metafile(metaFileStream);
			if (pageSettings.Landscape)
			{
				m_pageWidth = (float)pageSettings.PaperSize.Height / 100f;
				m_pageHeight = (float)pageSettings.PaperSize.Width / 100f;
			}
			else
			{
				m_pageWidth = (float)pageSettings.PaperSize.Width / 100f;
				m_pageHeight = (float)pageSettings.PaperSize.Height / 100f;
			}
		}

		public override void Draw(Graphics g, PointF scrollOffset, bool testMode)
		{
			Draw(g, new Rectangle(0, 0, Global.InchToPixels(m_pageWidth, g.DpiX), Global.InchToPixels(m_pageHeight, g.DpiY)));
		}

		public override void GetPageSize(Graphics g, out float width, out float height)
		{
			width = Global.InchToPixels(m_pageWidth, g.DpiX) + 2 * ExternalMargin;
			height = Global.InchToPixels(m_pageHeight, g.DpiY) + 2 * ExternalMargin;
		}

		public override void BuildInteractivityInfo(Graphics g)
		{
		}

		public void Draw(Graphics g, Rectangle destRect)
		{
			using (Brush brush = new SolidBrush(Color.White))
			{
				g.FillRectangle(brush, destRect);
			}
			g.DrawImage(m_metaFile, destRect);
		}
	}
}
