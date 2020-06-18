using Microsoft.ReportingServices.RdlExpressions;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordRow : IPersistable
	{
		private RecordField[] m_recordFields;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		[NonSerialized]
		private long m_streamPosition;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

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

		internal long StreamPosition
		{
			get
			{
				return m_streamPosition;
			}
			set
			{
				m_streamPosition = value;
			}
		}

		internal RecordRow()
		{
		}

		internal RecordRow(FieldsImpl fields, int fieldCount, FieldInfo[] fieldInfos)
		{
			m_recordFields = new RecordField[fieldCount];
			for (int i = 0; i < fieldCount; i++)
			{
				if (!fields[i].IsMissing)
				{
					FieldInfo fieldInfo = null;
					if (fieldInfos != null && i < fieldInfos.Length)
					{
						fieldInfo = fieldInfos[i];
					}
					m_recordFields[i] = new RecordField(fields[i], fieldInfo);
				}
			}
			m_isAggregateRow = fields.IsAggregateRow;
			m_aggregationFieldCount = fields.AggregationFieldCount;
		}

		internal RecordRow(RecordRow original, int[] mappingDataSetFieldIndexesToDataChunk)
		{
			m_streamPosition = original.m_streamPosition;
			m_isAggregateRow = original.m_isAggregateRow;
			m_recordFields = original.m_recordFields;
			ApplyFieldMapping(mappingDataSetFieldIndexesToDataChunk);
		}

		internal void ApplyFieldMapping(int[] mappingDataSetFieldIndexesToDataChunk)
		{
			if (mappingDataSetFieldIndexesToDataChunk == null)
			{
				return;
			}
			RecordField[] recordFields = m_recordFields;
			m_recordFields = new RecordField[mappingDataSetFieldIndexesToDataChunk.Length];
			m_aggregationFieldCount = 0;
			for (int i = 0; i < mappingDataSetFieldIndexesToDataChunk.Length; i++)
			{
				if (mappingDataSetFieldIndexesToDataChunk[i] >= 0)
				{
					m_recordFields[i] = recordFields[mappingDataSetFieldIndexesToDataChunk[i]];
					if (m_recordFields[i] != null && m_recordFields[i].IsAggregationField)
					{
						m_aggregationFieldCount++;
					}
				}
			}
		}

		internal object GetFieldValue(int aliasIndex)
		{
			RecordField recordField = m_recordFields[aliasIndex];
			if (recordField == null)
			{
				throw new ReportProcessingException_FieldError(DataFieldStatus.IsMissing, null);
			}
			if (recordField.FieldStatus != 0)
			{
				throw new ReportProcessingException_FieldError(recordField.FieldStatus, Microsoft.ReportingServices.RdlExpressions.ReportRuntime.GetErrorName(recordField.FieldStatus, null));
			}
			return recordField.FieldValue;
		}

		internal bool IsAggregationField(int aliasIndex)
		{
			return m_recordFields[aliasIndex].IsAggregationField;
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.RecordFields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField));
			list.Add(new MemberInfo(MemberName.IsAggregateRow, Token.Boolean));
			list.Add(new MemberInfo(MemberName.AggregationFieldCount, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.RecordFields:
					writer.Write(m_recordFields);
					break;
				case MemberName.IsAggregateRow:
					writer.Write(m_isAggregateRow);
					break;
				case MemberName.AggregationFieldCount:
					writer.Write(m_aggregationFieldCount);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			m_streamPosition = reader.ObjectStartPosition;
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.RecordFields:
					m_recordFields = reader.ReadArrayOfRIFObjects<RecordField>();
					break;
				case MemberName.IsAggregateRow:
					m_isAggregateRow = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldCount:
					m_aggregationFieldCount = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordRow;
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}
	}
}
