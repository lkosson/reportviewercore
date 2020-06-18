using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	[PersistedWithinRequestOnly]
	internal class DataFieldRow : IStorable, IPersistable
	{
		protected FieldImpl[] m_fields;

		protected long m_streamOffset = UnInitializedStreamOffset;

		internal static readonly long UnInitializedStreamOffset = -1L;

		private static readonly Declaration m_declaration = GetDeclaration();

		internal FieldImpl this[int index] => m_fields[index];

		internal long StreamOffset => m_streamOffset;

		public virtual int Size => ItemSizes.SizeOf(m_fields) + 8;

		internal DataFieldRow()
		{
		}

		internal DataFieldRow(FieldsImpl fields, bool getAndSave)
		{
			if (getAndSave)
			{
				m_fields = fields.GetAndSaveFields();
			}
			else
			{
				m_fields = fields.GetFields();
			}
			m_streamOffset = fields.StreamOffset;
		}

		internal virtual void SetFields(FieldsImpl fields)
		{
			fields.SetFields(m_fields, m_streamOffset);
		}

		internal virtual void RestoreDataSetAndSetFields(OnDemandProcessingContext odpContext, FieldsContext fieldsContext)
		{
			odpContext.ReportObjectModel.RestoreFields(fieldsContext);
			SetFields(odpContext.ReportObjectModel.FieldsImpl);
		}

		internal virtual void SaveAggregateInfo(OnDemandProcessingContext odpContext)
		{
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Fields:
					writer.Write(m_fields);
					break;
				case MemberName.Offset:
					writer.Write(m_streamOffset);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Fields:
					m_fields = reader.ReadArrayOfRIFObjects<FieldImpl>();
					break;
				case MemberName.Offset:
					m_streamOffset = reader.ReadInt64();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.Fields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectArray, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl));
				list.Add(new MemberInfo(MemberName.Offset, Token.Int64));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataFieldRow, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
