using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableRowInstanceInfo : InstanceInfo
	{
		private bool m_startHidden;

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal TableRowInstanceInfo(ReportProcessing.ProcessingContext pc, TableRow rowDef, TableRowInstance owner, Table tableDef, IndexedExprHost rowVisibilityHiddenExprHost)
		{
			if (pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(owner.UniqueName, rowDef.Visibility, rowVisibilityHiddenExprHost, tableDef.ObjectType, tableDef.Name);
			}
			rowDef.StartHidden = m_startHidden;
			pc.ChunkManager.AddInstance(this, owner, pc.InPageSection);
		}

		internal TableRowInstanceInfo()
		{
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfo, memberInfoList);
		}
	}
}
