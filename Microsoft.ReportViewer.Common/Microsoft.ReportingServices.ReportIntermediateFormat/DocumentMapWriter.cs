using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal sealed class DocumentMapWriter
	{
		private DocumentMapNode m_node;

		private int m_level;

		private IntermediateFormatWriter m_writer;

		private Stream m_chunkStream;

		private bool m_isClosed;

		private static List<Declaration> m_docMapDeclarations = GetDocumentMapDeclarations();

		public DocumentMapWriter(Stream aChunkStream, OnDemandProcessingContext odpContext)
		{
			m_chunkStream = aChunkStream;
			m_writer = new IntermediateFormatWriter(m_chunkStream, m_docMapDeclarations, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
		}

		public void WriteBeginContainer(string aLabel, string aId)
		{
			Global.Tracer.Assert(!m_isClosed, "(!m_isClosed)");
			m_level++;
			m_writer.Write(DocumentMapBeginContainer.Instance);
			WriteNode(aLabel, aId);
		}

		public void WriteNode(string aLabel, string aId)
		{
			Global.Tracer.Assert(!m_isClosed, "(!m_isClosed)");
			if (m_node == null)
			{
				m_node = new DocumentMapNode();
			}
			m_node.Label = aLabel;
			m_node.Id = aId;
			m_writer.Write(m_node);
		}

		public void WriteEndContainer()
		{
			Global.Tracer.Assert(!m_isClosed, "(!m_isClosed)");
			m_level--;
			Global.Tracer.Assert(m_level >= 0, "Mismatched EndContainer");
			m_writer.Write(DocumentMapEndContainer.Instance);
			if (m_level == 0)
			{
				Close();
			}
		}

		public void Close()
		{
			Global.Tracer.Assert(m_level == 0, "Mismatched Container Structure.  There are still open containers");
			m_isClosed = true;
		}

		public bool IsClosed()
		{
			return m_isClosed;
		}

		private static List<Declaration> GetDocumentMapDeclarations()
		{
			return new List<Declaration>(3)
			{
				DocumentMapNode.GetDeclaration(),
				DocumentMapBeginContainer.GetDeclaration(),
				DocumentMapEndContainer.GetDeclaration()
			};
		}
	}
}
