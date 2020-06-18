using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordField : IPersistable
	{
		private object m_fieldValue;

		private bool m_isAggregationField;

		private List<object> m_fieldPropertyValues;

		private DataFieldStatus m_fieldStatus;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal object FieldValue
		{
			get
			{
				return m_fieldValue;
			}
			set
			{
				m_fieldValue = value;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				return m_isAggregationField;
			}
			set
			{
				m_isAggregationField = value;
			}
		}

		internal List<object> FieldPropertyValues
		{
			get
			{
				return m_fieldPropertyValues;
			}
			set
			{
				m_fieldPropertyValues = value;
			}
		}

		internal bool IsOverflow => DataFieldStatus.Overflow == m_fieldStatus;

		internal bool IsUnSupportedDataType => DataFieldStatus.UnSupportedDataType == m_fieldStatus;

		internal bool IsError => DataFieldStatus.IsError == m_fieldStatus;

		internal DataFieldStatus FieldStatus
		{
			get
			{
				return m_fieldStatus;
			}
			set
			{
				Global.Tracer.Assert(m_fieldStatus == DataFieldStatus.None, "(DataFieldStatus.None == m_fieldStatus)");
				m_fieldStatus = value;
			}
		}

		internal RecordField()
		{
		}

		internal RecordField(FieldImpl field, FieldInfo fieldInfo)
		{
			m_fieldStatus = field.FieldStatus;
			if (m_fieldStatus == DataFieldStatus.None)
			{
				m_fieldValue = field.Value;
				m_isAggregationField = field.IsAggregationField;
			}
			if (fieldInfo != null && 0 < fieldInfo.PropertyCount)
			{
				m_fieldPropertyValues = new List<object>(fieldInfo.PropertyCount);
				for (int i = 0; i < fieldInfo.PropertyCount; i++)
				{
					object property = field.GetProperty(fieldInfo.PropertyNames[i]);
					m_fieldPropertyValues.Add(property);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new ReadOnlyMemberInfo(MemberName.FieldValue, Token.Object));
			list.Add(new MemberInfo(MemberName.FieldValueSerializable, Token.Serializable));
			list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
			list.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
			list.Add(new MemberInfo(MemberName.FieldPropertyValues, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Object));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.FieldStatus:
					writer.WriteEnum((int)m_fieldStatus);
					break;
				case MemberName.FieldValueSerializable:
					if (!writer.TryWriteSerializable(m_fieldValue))
					{
						m_fieldValue = null;
						writer.WriteNull();
						m_fieldStatus = DataFieldStatus.UnSupportedDataType;
					}
					break;
				case MemberName.IsAggregateField:
					writer.Write(m_isAggregationField);
					break;
				case MemberName.FieldPropertyValues:
					writer.WriteListOfPrimitives(m_fieldPropertyValues);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.FieldStatus:
					m_fieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.FieldValue:
					m_fieldValue = reader.ReadVariant();
					break;
				case MemberName.FieldValueSerializable:
					m_fieldValue = reader.ReadSerializable();
					break;
				case MemberName.IsAggregateField:
					m_isAggregationField = reader.ReadBoolean();
					break;
				case MemberName.FieldPropertyValues:
					m_fieldPropertyValues = reader.ReadListOfPrimitives<object>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RecordField;
		}
	}
}
