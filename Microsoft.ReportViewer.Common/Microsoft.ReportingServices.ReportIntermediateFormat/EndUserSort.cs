using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class EndUserSort : IPersistable
	{
		[Reference]
		private DataSet m_dataSet;

		[Reference]
		private ISortFilterScope m_sortExpressionScope;

		[Reference]
		private GroupingList m_groupsInSortTarget;

		[Reference]
		private ISortFilterScope m_sortTarget;

		private int m_sortExpressionIndex = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ExpressionInfo m_sortExpression;

		[NonSerialized]
		private string m_sortExpressionScopeString;

		[NonSerialized]
		private string m_sortTargetString;

		private List<SubReport> m_detailScopeSubReports;

		[NonSerialized]
		private int m_subReportDataSetGlobalId = -1;

		internal DataSet DataSet
		{
			get
			{
				return m_dataSet;
			}
			set
			{
				m_dataSet = value;
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

		internal List<SubReport> DetailScopeSubReports
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

		internal int SubReportDataSetGlobalId
		{
			get
			{
				return m_subReportDataSetGlobalId;
			}
			set
			{
				m_subReportDataSetGlobalId = value;
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

		internal void SetSortTarget(ISortFilterScope target)
		{
			Global.Tracer.Assert(target != null);
			m_sortTarget = target;
			if (target.UserSortExpressions == null)
			{
				target.UserSortExpressions = new List<ExpressionInfo>();
			}
			m_sortExpressionIndex = target.UserSortExpressions.Count;
			target.UserSortExpressions.Add(m_sortExpression);
		}

		internal void SetDefaultSortTarget(ISortFilterScope target)
		{
			SetSortTarget(target);
			m_sortTargetString = target.ScopeName;
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			EndUserSort endUserSort = (EndUserSort)MemberwiseClone();
			if (m_sortExpression != null)
			{
				endUserSort.m_sortExpression = (ExpressionInfo)m_sortExpression.PublishClone(context);
			}
			if (m_sortExpressionScopeString != null)
			{
				endUserSort.m_sortExpressionScopeString = (string)m_sortExpressionScopeString.Clone();
			}
			if (m_sortTargetString != null)
			{
				endUserSort.m_sortTargetString = (string)m_sortTargetString.Clone();
			}
			if (m_sortTargetString != null || m_sortExpressionScopeString != null)
			{
				context.AddEndUserSort(endUserSort);
			}
			return endUserSort;
		}

		internal void UpdateSortScopeAndTargetReference(AutomaticSubtotalContext context)
		{
			if (m_sortExpressionScopeString != null)
			{
				m_sortExpressionScopeString = context.GetNewScopeName(m_sortExpressionScopeString);
			}
			if (m_sortTargetString == null)
			{
				return;
			}
			m_sortTargetString = context.GetNewScopeName(m_sortTargetString);
			if (m_sortTarget != null)
			{
				ISortFilterScope target = null;
				if (context.TryGetNewSortTarget(m_sortTargetString, out target))
				{
					SetSortTarget(target);
				}
			}
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataSet, Token.Reference));
			list.Add(new MemberInfo(MemberName.SortExpressionScope, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortFilterScope, Token.Reference));
			list.Add(new MemberInfo(MemberName.GroupsInSortTarget, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Grouping));
			list.Add(new MemberInfo(MemberName.SortTarget, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ISortFilterScope, Token.Reference));
			list.Add(new MemberInfo(MemberName.SortExpressionIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.DetailScopeSubReports, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Token.Reference, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.SubReport));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSet:
					writer.WriteReference(m_dataSet);
					break;
				case MemberName.SortExpressionScope:
					writer.WriteReference(m_sortExpressionScope);
					break;
				case MemberName.GroupsInSortTarget:
					writer.WriteListOfReferences(m_groupsInSortTarget);
					break;
				case MemberName.SortTarget:
					writer.WriteReference(m_sortTarget);
					break;
				case MemberName.SortExpressionIndex:
					writer.Write(m_sortExpressionIndex);
					break;
				case MemberName.DetailScopeSubReports:
					writer.WriteListOfReferences(m_detailScopeSubReports);
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
				case MemberName.DataSet:
					m_dataSet = reader.ReadReference<DataSet>(this);
					break;
				case MemberName.SortExpressionScope:
					m_sortExpressionScope = reader.ReadReference<ISortFilterScope>(this);
					break;
				case MemberName.GroupsInSortTarget:
					m_groupsInSortTarget = reader.ReadListOfReferences<GroupingList, Grouping>(this);
					break;
				case MemberName.SortTarget:
					m_sortTarget = reader.ReadReference<ISortFilterScope>(this);
					break;
				case MemberName.SortExpressionIndex:
					m_sortExpressionIndex = reader.ReadInt32();
					break;
				case MemberName.DetailScopeSubReports:
					m_detailScopeSubReports = reader.ReadGenericListOfReferences<SubReport>(this);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.DataSet:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is DataSet);
					Global.Tracer.Assert(m_dataSet != (DataSet)referenceableItems[item.RefID]);
					m_dataSet = (DataSet)referenceableItems[item.RefID];
					break;
				case MemberName.SortTarget:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is ISortFilterScope);
					Global.Tracer.Assert(m_sortTarget != (ISortFilterScope)referenceableItems[item.RefID]);
					m_sortTarget = (ISortFilterScope)referenceableItems[item.RefID];
					break;
				case MemberName.DetailScopeSubReports:
					if (m_detailScopeSubReports == null)
					{
						m_detailScopeSubReports = new List<SubReport>();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is SubReport);
					Global.Tracer.Assert(!m_detailScopeSubReports.Contains((SubReport)referenceableItems[item.RefID]));
					m_detailScopeSubReports.Add((SubReport)referenceableItems[item.RefID]);
					break;
				case MemberName.SortExpressionScope:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is ISortFilterScope);
					Global.Tracer.Assert(m_sortExpressionScope != (ISortFilterScope)referenceableItems[item.RefID]);
					m_sortExpressionScope = (ISortFilterScope)referenceableItems[item.RefID];
					break;
				case MemberName.GroupsInSortTarget:
					if (m_groupsInSortTarget == null)
					{
						m_groupsInSortTarget = new GroupingList();
					}
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					Global.Tracer.Assert(referenceableItems[item.RefID] is Grouping);
					Global.Tracer.Assert(!m_groupsInSortTarget.Contains((Grouping)referenceableItems[item.RefID]));
					m_groupsInSortTarget.Add((Grouping)referenceableItems[item.RefID]);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.EndUserSort;
		}
	}
}
