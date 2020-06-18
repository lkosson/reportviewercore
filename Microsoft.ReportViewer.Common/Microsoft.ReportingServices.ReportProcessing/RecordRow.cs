using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordRow
	{
		private RecordField[] m_recordFields;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		internal RecordField[] RecordFields
		{
			get
			{
				return m_recordFields;
			}
			set
			{
				m_recordFields = value;
			}
		}

		internal bool IsAggregateRow
		{
			get
			{
				return m_isAggregateRow;
			}
			set
			{
				m_isAggregateRow = value;
			}
		}

		internal int AggregationFieldCount
		{
			get
			{
				return m_aggregationFieldCount;
			}
			set
			{
				m_aggregationFieldCount = value;
			}
		}

		internal RecordRow(FieldsImpl fields, int fieldCount)
		{
			m_recordFields = new RecordField[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				if (!fields[i].IsMissing)
				{
					m_recordFields[i] = new RecordField(fields[i]);
				}
			}
			m_isAggregateRow = fields.IsAggregateRow;
			m_aggregationFieldCount = fields.AggregationFieldCount;
		}

		internal RecordRow()
		{
		}

		internal object GetFieldValue(int aliasIndex)
		{
			RecordField recordField = m_recordFields[aliasIndex];
			Global.Tracer.Assert(recordField != null);
			if (recordField.FieldStatus != 0)
			{
				throw new ReportProcessingException_FieldError(recordField.FieldStatus, ReportRuntime.GetErrorName(recordField.FieldStatus, null));
			}
			return recordField.FieldValue;
		}

		internal bool IsAggregationField(int aliasIndex)
		{
			return m_recordFields[aliasIndex].IsAggregationField;
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RecordFields, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.RecordField));
			memberInfoList.Add(new MemberInfo(MemberName.IsAggregateRow, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.AggregationFieldCount, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
