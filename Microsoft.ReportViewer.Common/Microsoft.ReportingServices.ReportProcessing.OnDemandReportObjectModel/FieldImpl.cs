using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal class FieldImpl : Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.Field, IStorable, IPersistable
	{
		[StaticReference]
		private ObjectModelImpl m_reportOM;

		private object m_value;

		private bool m_isAggregationField;

		private bool m_aggregationFieldChecked;

		private DataFieldStatus m_fieldStatus;

		private string m_exceptionMessage;

		private Hashtable m_properties;

		[StaticReference]
		private Microsoft.ReportingServices.ReportIntermediateFormat.Field m_fieldDef;

		private bool m_usedInExpression;

		private static Declaration m_declaration = GetDeclaration();

		public override object this[string key]
		{
			get
			{
				if (key == null)
				{
					return null;
				}
				m_reportOM.PerformPendingFieldValueUpdate();
				m_usedInExpression = true;
				if (ReportProcessing.CompareWithInvariantCulture(key, "Value", ignoreCase: true) == 0)
				{
					return Value;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "IsMissing", ignoreCase: true) == 0)
				{
					return IsMissing;
				}
				if (ReportProcessing.CompareWithInvariantCulture(key, "LevelNumber", ignoreCase: true) == 0)
				{
					return LevelNumber;
				}
				return GetProperty(key);
			}
		}

		public override object Value
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				m_usedInExpression = true;
				if (m_fieldStatus == DataFieldStatus.None)
				{
					if (IsCalculatedField && m_value != null)
					{
						return ((CalculatedFieldWrapper)m_value).Value;
					}
					return m_value;
				}
				throw new ReportProcessingException_FieldError(m_fieldStatus, m_exceptionMessage);
			}
		}

		public override bool IsMissing
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return DataFieldStatus.IsMissing == m_fieldStatus;
			}
		}

		public override string UniqueName
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("UniqueName") as string;
			}
		}

		public override string BackgroundColor
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("BackgroundColor") as string;
			}
		}

		public override string Color
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("Color") as string;
			}
		}

		public override string FontFamily
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("FontFamily") as string;
			}
		}

		public override string FontSize
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("FontSize") as string;
			}
		}

		public override string FontWeight
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("FontWeight") as string;
			}
		}

		public override string FontStyle
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("FontStyle") as string;
			}
		}

		public override string TextDecoration
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("TextDecoration") as string;
			}
		}

		public override string FormattedValue
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("FormattedValue") as string;
			}
		}

		public override object Key
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("Key");
			}
		}

		public override int LevelNumber
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				object property = GetProperty("LevelNumber");
				if (property == null)
				{
					return 0;
				}
				if (property is int)
				{
					return (int)property;
				}
				bool valid;
				int result = DataTypeUtility.ConvertToInt32(DataAggregate.GetTypeCode(property), property, out valid);
				if (valid)
				{
					return result;
				}
				return 0;
			}
		}

		public override string ParentUniqueName
		{
			get
			{
				m_reportOM.PerformPendingFieldValueUpdate();
				return GetProperty("ParentUniqueName") as string;
			}
		}

		internal DataFieldStatus FieldStatus
		{
			get
			{
				if (DataFieldStatus.IsMissing != m_fieldStatus)
				{
					m_reportOM.PerformPendingFieldValueUpdate();
				}
				if (IsCalculatedField)
				{
					if (m_value == null)
					{
						return DataFieldStatus.None;
					}
					if (((CalculatedFieldWrapperImpl)m_value).ErrorOccurred)
					{
						m_exceptionMessage = ((CalculatedFieldWrapperImpl)m_value).ExceptionMessage;
						return DataFieldStatus.IsError;
					}
				}
				return m_fieldStatus;
			}
		}

		internal string ExceptionMessage
		{
			get
			{
				if (!IsCalculatedField)
				{
					m_reportOM.PerformPendingFieldValueUpdate();
				}
				return m_exceptionMessage;
			}
		}

		internal bool IsAggregationField
		{
			get
			{
				if (!IsCalculatedField)
				{
					m_reportOM.PerformPendingFieldValueUpdate();
				}
				return m_isAggregationField;
			}
		}

		internal bool AggregationFieldChecked
		{
			get
			{
				return m_aggregationFieldChecked;
			}
			set
			{
				m_aggregationFieldChecked = value;
			}
		}

		internal Hashtable Properties => m_properties;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Field FieldDef => m_fieldDef;

		internal bool UsedInExpression
		{
			get
			{
				return m_usedInExpression;
			}
			set
			{
				m_usedInExpression = value;
			}
		}

		internal bool IsCalculatedField
		{
			get
			{
				if (m_fieldDef != null)
				{
					return m_fieldDef.IsCalculatedField;
				}
				return false;
			}
		}

		public int Size => ItemSizes.ReferenceSize + ItemSizes.SizeOf(m_value) + 1 + 1 + 4 + ItemSizes.SizeOf(m_exceptionMessage) + ItemSizes.SizeOf(m_properties) + ItemSizes.ReferenceSize + 1;

		internal FieldImpl()
		{
		}

		internal FieldImpl(ObjectModelImpl reportOM, object value, bool isAggregationField, Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			m_reportOM = reportOM;
			m_fieldDef = fieldDef;
			UpdateValue(value, isAggregationField, DataFieldStatus.None, null);
		}

		internal FieldImpl(ObjectModelImpl reportOM, DataFieldStatus status, string exceptionMessage, Microsoft.ReportingServices.ReportIntermediateFormat.Field fieldDef)
		{
			m_reportOM = reportOM;
			m_fieldDef = fieldDef;
			Global.Tracer.Assert(status != DataFieldStatus.None, "(DataFieldStatus.None != status)");
			UpdateValue(null, isAggregationField: false, status, exceptionMessage);
		}

		internal void UpdateValue(object value, bool isAggregationField, DataFieldStatus status, string exceptionMessage)
		{
			m_value = value;
			m_isAggregationField = isAggregationField;
			m_aggregationFieldChecked = false;
			m_fieldStatus = status;
			m_exceptionMessage = exceptionMessage;
			m_usedInExpression = false;
			m_properties = null;
		}

		internal bool ResetCalculatedField()
		{
			if (m_value == null)
			{
				return false;
			}
			m_fieldStatus = DataFieldStatus.None;
			((CalculatedFieldWrapperImpl)m_value).ResetValue();
			m_usedInExpression = false;
			return true;
		}

		internal void SetValue(object value)
		{
			m_value = value;
		}

		internal void SetProperty(string propertyName, object propertyValue)
		{
			if (m_properties == null)
			{
				m_properties = new Hashtable();
			}
			m_properties[propertyName] = propertyValue;
		}

		internal object GetProperty(string propertyName)
		{
			if (m_properties == null)
			{
				return null;
			}
			m_usedInExpression = true;
			return m_properties[propertyName];
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = writer.PersistenceHelper as IScalabilityCache;
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.ReportObjectModel:
				{
					int value2 = scalabilityCache.StoreStaticReference(m_reportOM);
					writer.Write(value2);
					break;
				}
				case MemberName.Value:
					writer.WriteVariantOrPersistable(m_value);
					break;
				case MemberName.IsAggregateField:
					writer.Write(m_isAggregationField);
					break;
				case MemberName.AggregationFieldChecked:
					writer.Write(m_aggregationFieldChecked);
					break;
				case MemberName.FieldStatus:
					writer.WriteEnum((int)m_fieldStatus);
					break;
				case MemberName.Message:
					writer.Write(m_exceptionMessage);
					break;
				case MemberName.Properties:
					writer.WriteStringObjectHashtable(m_properties);
					break;
				case MemberName.FieldDef:
				{
					int value = scalabilityCache.StoreStaticReference(m_fieldDef);
					writer.Write(value);
					break;
				}
				case MemberName.UsedInExpression:
					writer.Write(m_usedInExpression);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_declaration);
			IScalabilityCache scalabilityCache = reader.PersistenceHelper as IScalabilityCache;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.ReportObjectModel:
				{
					int id2 = reader.ReadInt32();
					m_reportOM = (ObjectModelImpl)scalabilityCache.FetchStaticReference(id2);
					break;
				}
				case MemberName.Value:
					m_value = reader.ReadVariant();
					break;
				case MemberName.IsAggregateField:
					m_isAggregationField = reader.ReadBoolean();
					break;
				case MemberName.AggregationFieldChecked:
					m_aggregationFieldChecked = reader.ReadBoolean();
					break;
				case MemberName.FieldStatus:
					m_fieldStatus = (DataFieldStatus)reader.ReadEnum();
					break;
				case MemberName.Message:
					m_exceptionMessage = reader.ReadString();
					break;
				case MemberName.Properties:
					m_properties = reader.ReadStringObjectHashtable<Hashtable>();
					break;
				case MemberName.FieldDef:
				{
					int id = reader.ReadInt32();
					m_fieldDef = (Microsoft.ReportingServices.ReportIntermediateFormat.Field)scalabilityCache.FetchStaticReference(id);
					break;
				}
				case MemberName.UsedInExpression:
					m_usedInExpression = reader.ReadBoolean();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl;
		}

		public static Declaration GetDeclaration()
		{
			if (m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.ReportObjectModel, Token.Int32));
				list.Add(new MemberInfo(MemberName.Value, Token.Object));
				list.Add(new MemberInfo(MemberName.IsAggregateField, Token.Boolean));
				list.Add(new MemberInfo(MemberName.AggregationFieldChecked, Token.Boolean));
				list.Add(new MemberInfo(MemberName.FieldStatus, Token.Enum));
				list.Add(new MemberInfo(MemberName.Message, Token.String));
				list.Add(new MemberInfo(MemberName.Properties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringObjectHashtable));
				list.Add(new MemberInfo(MemberName.FieldDef, Token.Int32));
				list.Add(new MemberInfo(MemberName.UsedInExpression, Token.Boolean));
				return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.FieldImpl, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
			}
			return m_declaration;
		}
	}
}
