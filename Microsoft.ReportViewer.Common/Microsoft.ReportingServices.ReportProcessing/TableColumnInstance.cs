using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableColumnInstance
	{
		private int m_uniqueName;

		private bool m_startHidden;

		internal int UniqueName
		{
			get
			{
				return m_uniqueName;
			}
			set
			{
				m_uniqueName = value;
			}
		}

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

		internal TableColumnInstance(ReportProcessing.ProcessingContext pc, TableColumn tableColumnDef, Table tableDef)
		{
			m_uniqueName = pc.CreateUniqueName();
			if (pc.ShowHideType != 0)
			{
				m_startHidden = pc.ProcessReceiver(m_uniqueName, tableColumnDef.Visibility, (tableDef.TableExprHost != null) ? tableDef.TableExprHost.TableColumnVisibilityHiddenExpressions : null, tableDef.ObjectType, tableDef.Name);
			}
		}

		internal TableColumnInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.StartHidden, Token.Boolean));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
