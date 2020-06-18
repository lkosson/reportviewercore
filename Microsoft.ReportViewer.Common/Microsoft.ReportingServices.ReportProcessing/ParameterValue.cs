using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal class ParameterValue
	{
		private string m_name;

		private ExpressionInfo m_value;

		private ExpressionInfo m_omit;

		private int m_exprHostID = -1;

		[NonSerialized]
		private ParamExprHost m_exprHost;

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

		internal ExpressionInfo Omit
		{
			get
			{
				return m_omit;
			}
			set
			{
				m_omit = value;
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

		internal ParamExprHost ExprHost
		{
			get
			{
				return m_exprHost;
			}
			set
			{
				m_exprHost = value;
			}
		}

		internal void Initialize(InitializationContext context, bool queryParam)
		{
			if (m_value != null)
			{
				m_value.Initialize("Value", context);
				if (!queryParam)
				{
					context.ExprHostBuilder.GenericValue(m_value);
				}
				else
				{
					context.ExprHostBuilder.QueryParameterValue(m_value);
				}
			}
			if (m_omit != null)
			{
				m_omit.Initialize("Omit", context);
				context.ExprHostBuilder.ParameterOmit(m_omit);
			}
		}

		internal void SetExprHost(IList<ParamExprHost> paramExprHosts, ObjectModelImpl reportObjectModel)
		{
			if (m_exprHostID >= 0)
			{
				Global.Tracer.Assert(paramExprHosts != null && reportObjectModel != null);
				m_exprHost = paramExprHosts[m_exprHostID];
				m_exprHost.SetReportObjectModel(reportObjectModel);
			}
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Value, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Omit, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
