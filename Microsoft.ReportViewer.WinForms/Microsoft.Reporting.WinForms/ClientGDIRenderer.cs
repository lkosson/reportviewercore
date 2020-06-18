using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Microsoft.Reporting.WinForms
{
	internal class ClientGDIRenderer : IDisposable
	{
		internal const float MEASUREMENT_DPI = 96f;

		internal const float MEASUREMENT_DELTA = 0.001f;

		private RenderingReport m_report;

		private GdiContext m_context;

		internal GdiContext Context => m_context;

		internal float PageWidth => m_report.Position.Width;

		internal float PageHeight => m_report.Position.Height;

		internal RenderingReport Report => m_report;

		internal ClientGDIRenderer(byte[] pageByteArray)
		{
			if (pageByteArray == null)
			{
				throw new Exception(ReportPreviewStrings.InvalidRGDIStream);
			}
			try
			{
				BinaryReader reader = new BinaryReader(new MemoryStream(pageByteArray), Encoding.Unicode);
				m_context = new GdiContext();
				m_context.RplReport = new RPLReport(reader);
				m_report = new RenderingReport(m_context);
			}
			catch (Exception renderingException)
			{
				throw new ClientRenderingException(renderingException);
			}
		}

		private void Dispose(bool disposing)
		{
			if (disposing && m_context != null)
			{
				m_context.Dispose();
				m_context = null;
			}
		}

		~ClientGDIRenderer()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		internal void DrawToPage(Graphics graphics, bool firstDraw, bool testMode)
		{
			if (graphics != null)
			{
				Context.Graphics = graphics;
				Context.FirstDraw = firstDraw;
				Context.TestMode = testMode;
				m_report.DrawToPage(Context);
			}
		}

		internal void ClearSearchResults()
		{
			Context.SearchMatchIndex = -1;
			Context.SearchMatches = null;
			Context.SearchText = null;
			m_report.Search(Context);
		}

		internal void Search(string searchText)
		{
			Context.SearchText = searchText;
			m_report.Search(Context);
			Context.SearchMatchIndex = 0;
		}

		internal bool FindNext()
		{
			if (Context.SearchMatches == null || Context.SearchMatchIndex == Context.SearchMatches.Count - 1)
			{
				return false;
			}
			Context.SearchMatchIndex++;
			return true;
		}
	}
}
