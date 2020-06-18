using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ReportPrintDocument : PrintDocument
	{
		private FileManager m_fileManager;

		private PageSettings m_pageSettings;

		private int m_currentPage;

		private int m_endPage;

		private int m_hardMarginX = -1;

		private int m_hardMarginY = -1;

		internal ReportPrintDocument(FileManager fileManager, PageSettings pageSettings)
		{
			m_fileManager = fileManager;
			m_pageSettings = pageSettings;
		}

		protected override void OnBeginPrint(PrintEventArgs e)
		{
			base.OnBeginPrint(e);
			m_hardMarginX = -1;
			m_hardMarginY = -1;
			switch (base.PrinterSettings.PrintRange)
			{
			case PrintRange.AllPages:
				m_currentPage = 1;
				m_endPage = base.PrinterSettings.MaximumPage;
				break;
			case PrintRange.SomePages:
				m_currentPage = base.PrinterSettings.FromPage;
				m_endPage = base.PrinterSettings.ToPage;
				break;
			default:
				throw new NotSupportedException();
			}
		}

		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage(e);
			if (m_hardMarginX == -1)
			{
				m_hardMarginX = (int)e.PageSettings.HardMarginX;
				m_hardMarginY = (int)e.PageSettings.HardMarginY;
			}
			Stream stream = m_fileManager.Get(m_currentPage);
			if (stream != null)
			{
				MetaFilePage metaFilePage = new MetaFilePage(stream, m_pageSettings);
				Rectangle destRect = new Rectangle(e.PageBounds.Left - m_hardMarginX, e.PageBounds.Top - m_hardMarginY, e.PageBounds.Width, e.PageBounds.Height);
				metaFilePage.Draw(e.Graphics, destRect);
				m_currentPage++;
				e.HasMorePages = (m_currentPage <= m_endPage && m_fileManager.Count >= m_currentPage);
			}
			else
			{
				e.Cancel = true;
			}
		}

		protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
		{
			e.PageSettings = (PageSettings)m_pageSettings.Clone();
		}
	}
}
