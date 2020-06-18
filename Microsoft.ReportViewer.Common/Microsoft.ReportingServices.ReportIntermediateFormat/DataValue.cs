using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class DataValue : IPersistable
	{
		private ExpressionInfo m_name;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		[NonSerialized]
		private DataValueExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal ExpressionInfo Name
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

		internal DataValueExprHost ExprHost => m_exprHost;

		public object PublishClone(AutomaticSubtotalContext context)
		{
			DataValue dataValue = (DataValue)MemberwiseClone();
			if (m_name == null)
			{
				dataValue.m_name = (ExpressionInfo)m_name.PublishClone(context);
			}
			if (m_value == null)
			{
				dataValue.m_value = (ExpressionInfo)m_value.PublishClone(context);
			}
			return dataValue;
		}

		internal void Initialize(string propertyName, bool isCustomProperty, DynamicImageOrCustomUniqueNameValidator validator, InitializationContext context)
		{
			context.ExprHostBuilder.DataValueStart();
			if (m_name != null)
			{
				m_name.Initialize(propertyName + ".Name", context);
				if (isCustomProperty && ExpressionInfo.Types.Constant == m_name.Type)
				{
					validator.Validate(Severity.Error, propertyName + ".Name", context.ObjectType, context.ObjectName, m_name.StringValue, context.ErrorContext);
				}
				context.ExprHostBuilder.DataValueName(m_name);
			}
			if (m_value != null)
			{
				m_value.Initialize(propertyName + ".Value", context);
				context.ExprHostBuilder.DataValueValue(m_value);
			}
			m_exprHostID = context.ExprHostBuilder.DataValueEnd(isCustomProperty);
		}

		internal void SetExprHost(IList<DataValueExprHost> dataValueHosts, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				Global.Tracer.Assert(dataValueHosts != null && dataValueHosts.Count > m_exprHostID && reportObjectModel != null);
				m_exprHost = dataValueHosts[m_exprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal void EvaluateNameAndValue(ReportElement reportElementOwner, IReportScopeInstance romInstance, IInstancePath instancePath, OnDemandProcessingContext context, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, out string name, out object value, out TypeCode valueTypeCode)
		{
			context.SetupContext(instancePath, romInstance);
			name = null;
			value = null;
			valueTypeCode = TypeCode.Empty;
			if (m_name != null)
			{
				if (!m_name.IsExpression)
				{
					name = m_name.StringValue;
				}
				else if (reportElementOwner == null || (reportElementOwner != null && reportElementOwner.CriOwner == null))
				{
					name = context.ReportRuntime.EvaluateDataValueNameExpression(this, objectType, objectName, "Name");
				}
			}
			if (m_value != null)
			{
				if (!m_value.IsExpression)
				{
					value = m_value.Value;
				}
				else if (reportElementOwner == null || (reportElementOwner != null && reportElementOwner.CriOwner == null))
				{
					value = context.ReportRuntime.EvaluateDataValueValueExpression(this, objectType, objectName, "Value", out valueTypeCode);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
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
				case MemberName.Value:
					writer.Write(m_value);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
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
				case MemberName.Name:
					m_name = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Value:
					m_value = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue;
		}
	}
}
