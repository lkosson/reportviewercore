using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ActiveXControl : ReportItem
	{
		private string m_classID;

		private string m_codeBase;

		private ParameterValueList m_parameters;

		[NonSerialized]
		private ActiveXControlExprHost m_exprHost;

		internal override ObjectType ObjectType => ObjectType.ActiveXControl;

		internal string ClassID
		{
			get
			{
				return m_classID;
			}
			set
			{
				m_classID = value;
			}
		}

		internal string CodeBase
		{
			get
			{
				return m_codeBase;
			}
			set
			{
				m_codeBase = value;
			}
		}

		internal ParameterValueList Parameters
		{
			get
			{
				return m_parameters;
			}
			set
			{
				m_parameters = value;
			}
		}

		internal ActiveXControl(ReportItem parent)
			: base(parent)
		{
		}

		internal ActiveXControl(int id, ReportItem parent)
			: base(id, parent)
		{
			m_parameters = new ParameterValueList();
		}

		internal override bool Initialize(InitializationContext context)
		{
			context.ObjectType = ObjectType;
			context.ObjectName = m_name;
			context.ExprHostBuilder.ActiveXControlStart(m_name);
			base.Initialize(context);
			if (m_visibility != null)
			{
				m_visibility.Initialize(context, isContainer: false, tableRowCol: false);
			}
			if (m_parameters != null)
			{
				for (int i = 0; i < m_parameters.Count; i++)
				{
					ParameterValue parameterValue = m_parameters[i];
					context.ExprHostBuilder.ActiveXControlParameterStart();
					parameterValue.Initialize(context, queryParam: false);
					parameterValue.ExprHostID = context.ExprHostBuilder.ActiveXControlParameterEnd();
				}
			}
			base.ExprHostID = context.ExprHostBuilder.ActiveXControlEnd();
			return true;
		}

		internal override void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel)
		{
			if (base.ExprHostID < 0)
			{
				return;
			}
			Global.Tracer.Assert(reportExprHost != null && reportObjectModel != null);
			m_exprHost = reportExprHost.ActiveXControlHostsRemotable[base.ExprHostID];
			ReportItemSetExprHost(m_exprHost, reportObjectModel);
			if (m_exprHost.ParameterHostsRemotable != null)
			{
				Global.Tracer.Assert(m_parameters != null);
				for (int num = m_parameters.Count - 1; num >= 0; num--)
				{
					m_parameters[num].SetExprHost(m_exprHost.ParameterHostsRemotable, reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.ClassID, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.CodeBase, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.Parameters, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ParameterValueList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ReportItem, memberInfoList);
		}
	}
}
