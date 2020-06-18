using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Bookmarks : InteractivityChunks
	{
		internal Bookmarks(Stream stream, int page)
			: base(stream, page)
		{
		}

		internal void WriteBookmark(ReportItemInstance itemInstance)
		{
			if (itemInstance != null && itemInstance.Bookmark != null)
			{
				m_writer.Write((byte)0);
				m_writer.Write(itemInstance.Bookmark);
				m_writer.Write(itemInstance.UniqueName);
				m_writer.Write(m_page);
				m_writer.Write((byte)4);
			}
		}
	}
}
