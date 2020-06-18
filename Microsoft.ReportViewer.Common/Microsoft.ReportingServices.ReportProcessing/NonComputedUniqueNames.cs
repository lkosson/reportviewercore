using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class NonComputedUniqueNames
	{
		private int m_uniqueName;

		private NonComputedUniqueNames[] m_childrenUniqueNames;

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

		internal NonComputedUniqueNames[] ChildrenUniqueNames
		{
			get
			{
				return m_childrenUniqueNames;
			}
			set
			{
				m_childrenUniqueNames = value;
			}
		}

		private NonComputedUniqueNames(int uniqueName, NonComputedUniqueNames[] childrenUniqueNames)
		{
			m_uniqueName = uniqueName;
			m_childrenUniqueNames = childrenUniqueNames;
		}

		internal NonComputedUniqueNames()
		{
		}

		internal static NonComputedUniqueNames[] CreateNonComputedUniqueNames(ReportProcessing.ProcessingContext pc, ReportItemCollection reportItemsDef)
		{
			if (reportItemsDef == null || pc == null)
			{
				return null;
			}
			ReportItemList nonComputedReportItems = reportItemsDef.NonComputedReportItems;
			if (nonComputedReportItems == null)
			{
				return null;
			}
			if (nonComputedReportItems.Count == 0)
			{
				return null;
			}
			NonComputedUniqueNames[] array = new NonComputedUniqueNames[nonComputedReportItems.Count];
			for (int i = 0; i < nonComputedReportItems.Count; i++)
			{
				array[i] = CreateNonComputedUniqueNames(pc, nonComputedReportItems[i]);
			}
			return array;
		}

		internal static NonComputedUniqueNames CreateNonComputedUniqueNames(ReportProcessing.ProcessingContext pc, ReportItem reportItemDef)
		{
			if (reportItemDef == null || pc == null)
			{
				return null;
			}
			NonComputedUniqueNames[] childrenUniqueNames = null;
			if (reportItemDef is Rectangle)
			{
				childrenUniqueNames = CreateNonComputedUniqueNames(pc, ((Rectangle)reportItemDef).ReportItems);
			}
			return new NonComputedUniqueNames(pc.CreateUniqueName(), childrenUniqueNames);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ChildrenUniqueNames, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.NonComputedUniqueNames));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
