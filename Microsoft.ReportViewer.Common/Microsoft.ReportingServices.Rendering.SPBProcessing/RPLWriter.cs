using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class RPLWriter
	{
		private RPLReport m_rplReport;

		private BinaryWriter m_binaryWriter;

		private RPLTablixRow m_tablixRow;

		internal RPLReport Report
		{
			get
			{
				return m_rplReport;
			}
			set
			{
				m_rplReport = value;
			}
		}

		internal RPLTablixRow TablixRow
		{
			get
			{
				return m_tablixRow;
			}
			set
			{
				m_tablixRow = value;
			}
		}

		internal BinaryWriter BinaryWriter
		{
			get
			{
				return m_binaryWriter;
			}
			set
			{
				m_binaryWriter = value;
			}
		}

		internal RPLWriter()
		{
		}

		internal RPLWriter(BinaryWriter binaryWriter, RPLReport report, RPLTablixRow tablixRow)
		{
			BinaryWriter = binaryWriter;
			Report = report;
			TablixRow = tablixRow;
		}
	}
}
