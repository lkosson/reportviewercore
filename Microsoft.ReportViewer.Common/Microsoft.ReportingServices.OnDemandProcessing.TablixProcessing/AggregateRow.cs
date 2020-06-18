using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal sealed class AggregateRow : DataFieldRow
	{
		[NonSerialized]
		private AggregateRowInfo m_aggregateInfo;

		private bool m_isAggregateRow;

		private int m_aggregationFieldCount;

		private bool m_validAggregateRow;

		private static Declaration m_declaration = GetDeclaration();

		public override int Size => base.Size + 1 + 4 + 1;

		internal AggregateRow()
		{
		}

		internal AggregateRow(FieldsImpl fields, bool getAndSave)
			: base(fields, getAndSave)
		{
			m_isAggregateRow = fields.IsAggregateRow;
			m_aggregationFieldCount = fields.AggregationFieldCount;
			m_validAggregateRow = fields.ValidAggregateRow;
		}

		internal override void SetFields(FieldsImpl fields)
		{
			fields.SetFields(m_fields, m_streamOffset, m_isAggregateRow, m_aggregationFieldCount, m_validAggregateRow);
		}

		internal override void RestoreDataSetAndSetFields(OnDemandProcessingContext odpContext, FieldsContext fieldsContext)
		{
			base.RestoreDataSetAndSetFields(odpContext, fieldsContext);
			if (m_aggregateInfo != null)
			{
				m_aggregateInfo.RestoreAggregateInfo(odpContext);
			}
		}

		internal override void SaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
			m_aggregateInfo = new AggregateRowInfo();
			m_aggregateInfo.SaveAggregateInfo(odpContext);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.IsAggregateRow:
					writer.Write(m_isAggregateRow);
					break;
				case MemberName.AggregationFieldCount:
					writer.Write(m_aggregationFieldCount);
					break;
				case MemberName.ValidAggregateRow:
					writer.Write(m_validAggregateRow);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.IsAggregateRow:
					m_isAggregateRow = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldCount:
					m_aggregationFieldCount = reader.ReadInt32();
					break;
				case MemberName.ValidAggregateRow:
					m_validAggregateRow = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow;
		}

		public new static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.IsAggregateRow, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregationFieldCount, Token.Int32));
				list.Add(new MemberInfo(MemberName.ValidAggregateRow, Token.Boolean));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AggregateRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow, list);
			}
			return m_declaration;
		}
	}
}
