using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixFullRow : RPLTablixRow
	{
		private int m_headerStart = -1;

		private int m_bodyStart = -1;

		private List<RPLTablixMemberCell> m_omittedHeaders;

		public override int HeaderStart => m_headerStart;

		public override int BodyStart => m_bodyStart;

		public override List<RPLTablixMemberCell> OmittedHeaders => m_omittedHeaders;

		internal RPLTablixFullRow(int headerStart, int bodyStart)
		{
			m_headerStart = headerStart;
			m_bodyStart = bodyStart;
		}

		internal RPLTablixFullRow(List<RPLTablixCell> cells, List<RPLTablixMemberCell> omittedHeaders, int headerStart, int bodyStart)
			: base(cells)
		{
			m_headerStart = headerStart;
			m_bodyStart = bodyStart;
			m_omittedHeaders = omittedHeaders;
		}

		internal override void SetHeaderStart()
		{
			if (m_headerStart < 0)
			{
				if (m_cells == null)
				{
					m_headerStart = 0;
				}
				else
				{
					m_headerStart = m_cells.Count;
				}
			}
		}

		internal override void SetBodyStart()
		{
			if (m_bodyStart < 0)
			{
				if (m_cells == null)
				{
					m_bodyStart = 0;
				}
				else
				{
					m_bodyStart = m_cells.Count;
				}
			}
		}

		internal override void AddOmittedHeader(RPLTablixMemberCell cell)
		{
			if (m_omittedHeaders == null)
			{
				m_omittedHeaders = new List<RPLTablixMemberCell>();
			}
			m_omittedHeaders.Add(cell);
		}
	}
}
