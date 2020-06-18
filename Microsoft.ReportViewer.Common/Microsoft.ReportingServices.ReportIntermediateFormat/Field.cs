using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class Field : IPersistable, IStaticReferenceable
	{
		private string m_name;

		private string m_dataField;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		private DataType m_constantDataType = DataType.String;

		private int m_aggregateIndicatorFieldIndex = -1;

		[NonSerialized]
		private const int AggregateIndicatorFieldNotSpecified = -1;

		[NonSerialized]
		private CalcFieldExprHost m_exprHost;

		[NonSerialized]
		private ObjectModelImpl m_lastReportOm;

		[NonSerialized]
		private int m_id = int.MinValue;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		internal string DataField
		{
			get
			{
				return m_dataField;
			}
			set
			{
				m_dataField = value;
			}
		}

		internal int AggregateIndicatorFieldIndex
		{
			get
			{
				return m_aggregateIndicatorFieldIndex;
			}
			set
			{
				m_aggregateIndicatorFieldIndex = value;
			}
		}

		internal bool HasAggregateIndicatorField => m_aggregateIndicatorFieldIndex != -1;

		internal ExpressionInfo Value
		{
			get
			{
				return m_value;
			}
			set
			{
				m_value = value;
			}
		}

		internal bool IsCalculatedField => m_dataField == null;

		internal DataType DataType
		{
			get
			{
				return m_constantDataType;
			}
			set
			{
				m_constantDataType = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal CalcFieldExprHost ExprHost => m_exprHost;

		public int ID => m_id;

		internal void Initialize(InitializationContext context)
		{
			if (Value != null)
			{
				context.ExprHostBuilder.CalcFieldStart(m_name);
				m_value.Initialize("Field", context);
				context.ExprHostBuilder.GenericValue(m_value);
				m_exprHostID = context.ExprHostBuilder.CalcFieldEnd();
			}
		}

		internal void SetExprHost(DataSetExprHost dataSetExprHost, ObjectModelImpl reportObjectModel)
		{
			if (ExprHostID >= 0)
			{
				Global.Tracer.Assert(dataSetExprHost != null && reportObjectModel != null, "(dataSetExprHost != null && reportObjectModel != null)");
				m_exprHost = dataSetExprHost.FieldHostsRemotable[ExprHostID];
				EnsureExprHostReportObjectModelBinding(reportObjectModel);
			}
		}

		internal void EnsureExprHostReportObjectModelBinding(ObjectModelImpl reportObjectModel)
		{
			if (m_exprHost != null && m_lastReportOm != reportObjectModel)
			{
				m_lastReportOm = reportObjectModel;
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.DataField, Token.String));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataType, Token.Enum));
			list.Add(new MemberInfo(MemberName.AggregateIndicatorFieldIndex, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.DataField:
					writer.Write(m_dataField);
					break;
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.DataType:
					writer.WriteEnum((int)m_constantDataType);
					break;
				case MemberName.AggregateIndicatorFieldIndex:
					writer.Write(m_aggregateIndicatorFieldIndex);
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
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
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.DataField:
					m_dataField = reader.ReadString();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataType:
					m_constantDataType = (DataType)reader.ReadEnum();
					break;
				case MemberName.AggregateIndicatorFieldIndex:
					m_aggregateIndicatorFieldIndex = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false, string.Empty);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false, string.Empty);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Field;
		}

		public void SetID(int id)
		{
			m_id = id;
		}
	}
}
