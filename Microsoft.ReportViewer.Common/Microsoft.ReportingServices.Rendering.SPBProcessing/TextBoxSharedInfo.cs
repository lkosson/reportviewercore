using System;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class TextBoxSharedInfo : IDisposable
	{
		private CanvasFont m_sharedFont;

		private int m_sharedState;

		private int m_pageNumber;

		internal CanvasFont SharedFont
		{
			get
			{
				return m_sharedFont;
			}
			set
			{
				m_sharedFont = value;
			}
		}

		internal int SharedState
		{
			get
			{
				return m_sharedState;
			}
			set
			{
				m_sharedState = value;
			}
		}

		internal int PageNumber
		{
			get
			{
				return m_pageNumber;
			}
			set
			{
				m_pageNumber = value;
			}
		}

		internal TextBoxSharedInfo(CanvasFont font, int sharedState)
		{
			m_sharedFont = font;
			m_sharedState = sharedState;
		}

		internal TextBoxSharedInfo(int pageNumber)
		{
			m_pageNumber = pageNumber;
		}

		private void Dispose(bool disposing)
		{
			if (disposing && m_sharedFont != null)
			{
				m_sharedFont.Dispose();
				m_sharedFont = null;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
