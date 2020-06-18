using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RunningValueInfo : DataAggregateInfo
	{
		private string m_scope;

		internal string Scope
		{
			get
			{
				return m_scope;
			}
			set
			{
				m_scope = value;
			}
		}

		internal new RunningValueInfo DeepClone(InitializationContext context)
		{
			RunningValueInfo runningValueInfo = new RunningValueInfo();
			DeepCloneInternal(runningValueInfo, context);
			runningValueInfo.m_scope = context.EscalateScope(m_scope);
			return runningValueInfo;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Scope, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataAggregateInfo, memberInfoList);
		}
	}
}
