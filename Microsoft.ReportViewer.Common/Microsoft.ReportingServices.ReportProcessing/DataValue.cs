using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class DataValue
	{
		private ExpressionInfo m_name;

		private ExpressionInfo m_value;

		private int m_exprHostID = -1;

		[NonSerialized]
		private DataValueExprHost m_exprHost;

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

		internal DataValue DeepClone(InitializationContext context)
		{
			DataValue dataValue = new DataValue();
			Global.Tracer.Assert(-1 == m_exprHostID);
			dataValue.m_name = ((m_name == null) ? null : m_name.DeepClone(context));
			dataValue.m_value = ((m_value == null) ? null : m_value.DeepClone(context));
			return dataValue;
		}

		internal void Initialize(string propertyName, bool isCustomProperty, CustomPropertyUniqueNameValidator validator, InitializationContext context)
		{
			context.ExprHostBuilder.DataValueStart();
			if (m_name != null)
			{
				m_name.Initialize(propertyName + ".Name", context);
				if (isCustomProperty && ExpressionInfo.Types.Constant == m_name.Type)
				{
					validator.Validate(Severity.Error, context.ObjectType, context.ObjectName, m_name.Value, context.ErrorContext);
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

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
