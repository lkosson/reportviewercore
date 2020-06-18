using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class DocumentMapLabels : InteractivityChunks
	{
		internal DocumentMapLabels(Stream stream, int page)
			: base(stream, page)
		{
		}

		internal void WriteDocMapLabel(ReportItemInstance itemInstance)
		{
			if (itemInstance != null && itemInstance.DocumentMapLabel != null)
			{
				m_writer.Write((byte)1);
				m_writer.Write(itemInstance.UniqueName);
				m_writer.Write(m_page);
				m_writer.Write((byte)4);
			}
		}

		internal void WriteDocMapLabel(GroupInstance groupInstance)
		{
			if (groupInstance != null && groupInstance.DocumentMapLabel != null)
			{
				m_writer.Write((byte)1);
				m_writer.Write(groupInstance.UniqueName);
				m_writer.Write(m_page);
				m_writer.Write((byte)4);
			}
		}

		internal void WriteDocMapRootLabel(string rootLabelUniqueName)
		{
			m_writer.Write((byte)1);
			m_writer.Write(rootLabelUniqueName);
			m_writer.Write(m_page);
			m_writer.Write((byte)4);
		}
	}
}
