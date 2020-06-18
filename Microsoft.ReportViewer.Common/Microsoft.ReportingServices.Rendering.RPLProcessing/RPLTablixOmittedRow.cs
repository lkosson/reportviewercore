using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLTablixOmittedRow : RPLTablixRow
	{
		private List<RPLTablixMemberCell> m_omittedHeaders;

		public override int BodyStart => -1;

		public override List<RPLTablixMemberCell> OmittedHeaders => m_omittedHeaders;

		internal RPLTablixOmittedRow()
		{
		}

		internal RPLTablixOmittedRow(List<RPLTablixMemberCell> omittedHeaders)
			: base(null)
		{
			m_omittedHeaders = omittedHeaders;
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
