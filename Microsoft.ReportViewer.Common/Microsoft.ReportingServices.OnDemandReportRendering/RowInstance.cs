using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class RowInstance
	{
		private Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow m_recordRow;

		private FieldInstance[] m_fields;

		private readonly FieldInfo[] m_fieldInfos;

		public FieldInstance this[int fieldIndex]
		{
			get
			{
				if (fieldIndex < 0 || fieldIndex >= m_recordRow.RecordFields.Length)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, fieldIndex, 0, m_recordRow.RecordFields.Length);
				}
				if (m_fields == null)
				{
					m_fields = new FieldInstance[m_recordRow.RecordFields.Length];
				}
				if (m_fields[fieldIndex] == null)
				{
					m_fields[fieldIndex] = new FieldInstance((m_fieldInfos != null) ? m_fieldInfos[fieldIndex] : null, m_recordRow.RecordFields[fieldIndex]);
				}
				return m_fields[fieldIndex];
			}
		}

		public bool IsAggregateRow => m_recordRow.IsAggregateRow;

		public int AggregationFieldCount => m_recordRow.AggregationFieldCount;

		internal RowInstance(FieldInfo[] fieldInfos, Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			m_recordRow = recordRow;
			m_fieldInfos = fieldInfos;
		}

		internal void UpdateRecordRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow recordRow)
		{
			m_recordRow = recordRow;
			if (m_fields != null)
			{
				for (int i = 0; i < m_fields.Length; i++)
				{
					m_fields[i]?.UpdateRecordField(m_recordRow.RecordFields[i]);
				}
			}
		}
	}
}
