using Microsoft.ReportingServices.ReportProcessing.Persistence;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EndUserSort
	{
		private int m_dataSetID = -1;

		[Reference]
		private ISortFilterScope m_sortExpressionScope;

		[Reference]
		private GroupingList m_groupsInSortTarget;

		[Reference]
		private ISortFilterScope m_sortTarget;

		private int m_sortExpressionIndex = -1;

		private SubReportList m_detailScopeSubReports;

		[NonSerialized]
		private ExpressionInfo m_sortExpression;

		[NonSerialized]
		private int m_sortExpressionScopeID = -1;

		[NonSerialized]
		private IntList m_groupInSortTargetIDs;

		[NonSerialized]
		private int m_sortTargetID = -1;

		[NonSerialized]
		private string m_sortExpressionScopeString;

		[NonSerialized]
		private string m_sortTargetString;

		[NonSerialized]
		private bool m_foundSortExpressionScope;

		internal int DataSetID
		{
			get
			{
				return m_dataSetID;
			}
			set
			{
				m_dataSetID = value;
			}
		}

		internal ISortFilterScope SortExpressionScope
		{
			get
			{
				return m_sortExpressionScope;
			}
			set
			{
				m_sortExpressionScope = value;
			}
		}

		internal GroupingList GroupsInSortTarget
		{
			get
			{
				return m_groupsInSortTarget;
			}
			set
			{
				m_groupsInSortTarget = value;
			}
		}

		internal ISortFilterScope SortTarget
		{
			get
			{
				return m_sortTarget;
			}
			set
			{
				m_sortTarget = value;
			}
		}

		internal int SortExpressionIndex
		{
			get
			{
				return m_sortExpressionIndex;
			}
			set
			{
				m_sortExpressionIndex = value;
			}
		}

		internal SubReportList DetailScopeSubReports
		{
			get
			{
				return m_detailScopeSubReports;
			}
			set
			{
				m_detailScopeSubReports = value;
			}
		}

		internal ExpressionInfo SortExpression
		{
			get
			{
				return m_sortExpression;
			}
			set
			{
				m_sortExpression = value;
			}
		}

		internal int SortExpressionScopeID
		{
			get
			{
				return m_sortExpressionScopeID;
			}
			set
			{
				m_sortExpressionScopeID = value;
			}
		}

		internal IntList GroupInSortTargetIDs
		{
			get
			{
				return m_groupInSortTargetIDs;
			}
			set
			{
				m_groupInSortTargetIDs = value;
			}
		}

		internal int SortTargetID
		{
			get
			{
				return m_sortTargetID;
			}
			set
			{
				m_sortTargetID = value;
			}
		}

		internal string SortExpressionScopeString
		{
			get
			{
				return m_sortExpressionScopeString;
			}
			set
			{
				m_sortExpressionScopeString = value;
			}
		}

		internal string SortTargetString
		{
			get
			{
				return m_sortTargetString;
			}
			set
			{
				m_sortTargetString = value;
			}
		}

		internal bool FoundSortExpressionScope
		{
			get
			{
				return m_foundSortExpressionScope;
			}
			set
			{
				m_foundSortExpressionScope = value;
			}
		}

		internal void SetSortTarget(ISortFilterScope target)
		{
			Global.Tracer.Assert(target != null);
			m_sortTarget = target;
			if (target.UserSortExpressions == null)
			{
				target.UserSortExpressions = new ExpressionInfoList();
			}
			m_sortExpressionIndex = target.UserSortExpressions.Count;
			target.UserSortExpressions.Add(m_sortExpression);
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.DataSetID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SortExpressionScope, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ISortFilterScope));
			memberInfoList.Add(new MemberInfo(MemberName.GroupsInSortTarget, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.GroupingList));
			memberInfoList.Add(new MemberInfo(MemberName.SortTarget, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ISortFilterScope));
			memberInfoList.Add(new MemberInfo(MemberName.SortExpressionIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DetailScopeSubReports, Token.Reference, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.SubReportList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
