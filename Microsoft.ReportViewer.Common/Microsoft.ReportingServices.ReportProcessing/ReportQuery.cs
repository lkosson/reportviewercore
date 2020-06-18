using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Data;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ReportQuery
	{
		private CommandType m_commandType = CommandType.Text;

		private ExpressionInfo m_commandText;

		private ParameterValueList m_queryParameters;

		private int m_timeOut;

		private string m_commandTextValue;

		private string m_writtenCommandText;

		[NonSerialized]
		private string m_dataSourceName;

		[NonSerialized]
		private IndexedExprHost m_queryParamsExprHost;

		internal CommandType CommandType
		{
			get
			{
				return m_commandType;
			}
			set
			{
				m_commandType = value;
			}
		}

		internal ExpressionInfo CommandText
		{
			get
			{
				return m_commandText;
			}
			set
			{
				m_commandText = value;
			}
		}

		internal ParameterValueList Parameters
		{
			get
			{
				return m_queryParameters;
			}
			set
			{
				m_queryParameters = value;
			}
		}

		internal int TimeOut
		{
			get
			{
				return m_timeOut;
			}
			set
			{
				m_timeOut = value;
			}
		}

		internal string CommandTextValue
		{
			get
			{
				return m_commandTextValue;
			}
			set
			{
				m_commandTextValue = value;
			}
		}

		internal string RewrittenCommandText
		{
			get
			{
				return m_writtenCommandText;
			}
			set
			{
				m_writtenCommandText = value;
			}
		}

		internal string DataSourceName
		{
			get
			{
				return m_dataSourceName;
			}
			set
			{
				m_dataSourceName = value;
			}
		}

		internal void Initialize(InitializationContext context)
		{
			if (m_commandText != null)
			{
				m_commandText.Initialize("CommandText", context);
				context.ExprHostBuilder.DataSetQueryCommandText(m_commandText);
			}
			if (m_queryParameters != null)
			{
				ObjectType objectType = context.ObjectType;
				string objectName = context.ObjectName;
				context.ObjectType = ObjectType.QueryParameter;
				context.ExprHostBuilder.QueryParametersStart();
				for (int i = 0; i < m_queryParameters.Count; i++)
				{
					ParameterValue parameterValue = m_queryParameters[i];
					context.ObjectName = parameterValue.Name;
					parameterValue.Initialize(context, queryParam: true);
				}
				context.ExprHostBuilder.QueryParametersEnd();
				context.ObjectType = objectType;
				context.ObjectName = objectName;
			}
		}

		internal void SetExprHost(IndexedExprHost queryParamsExprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(queryParamsExprHost != null && reportObjectModel != null);
			m_queryParamsExprHost = queryParamsExprHost;
			m_queryParamsExprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CommandType, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.CommandText, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.QueryParameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			memberInfoList.Add(new MemberInfo(MemberName.Timeout, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.CommandTextValue, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RewrittenCommandText, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
