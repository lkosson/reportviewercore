using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal class QueryRestartInfo
	{
		private bool m_queryRestartEnabled;

		private List<ScopeIDContext> m_queryRestartPosition;

		private Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataSet, List<RelationshipRestartContext>> m_relationshipRestartPositions = new Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.DataSet, List<RelationshipRestartContext>>();

		internal bool QueryRestartEnabled
		{
			get
			{
				return m_queryRestartEnabled;
			}
			set
			{
				m_queryRestartEnabled = value;
			}
		}

		internal List<ScopeIDContext> QueryRestartPosition => m_queryRestartPosition;

		private ScopeIDContext LastScopeIDContext
		{
			get
			{
				if (m_queryRestartPosition.Count == 0)
				{
					return null;
				}
				return m_queryRestartPosition[m_queryRestartPosition.Count - 1];
			}
		}

		internal QueryRestartInfo()
		{
			m_queryRestartPosition = new List<ScopeIDContext>();
		}

		private bool IsRestartable()
		{
			for (int i = 0; i < QueryRestartPosition.Count; i++)
			{
				if (QueryRestartPosition[i].IsRowLevelRestart)
				{
					return true;
				}
			}
			return false;
		}

		internal void EnableQueryRestart()
		{
			if (IsRestartable())
			{
				m_queryRestartEnabled = true;
			}
		}

		public DataSetQueryRestartPosition GetRestartPositionForDataSet(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet)
		{
			if (!m_queryRestartEnabled)
			{
				return null;
			}
			List<RestartContext> list = new List<RestartContext>();
			if (m_relationshipRestartPositions.TryGetValue(targetDataSet, out List<RelationshipRestartContext> value))
			{
				foreach (RelationshipRestartContext item in value)
				{
					list.Add(item);
				}
			}
			foreach (ScopeIDContext item2 in m_queryRestartPosition)
			{
				if (item2.MemberDefinition.DataScopeInfo.DataSet == targetDataSet && item2.RestartMode != RestartMode.Rom)
				{
					list.Add(item2);
				}
			}
			DataSetQueryRestartPosition result = null;
			if (list.Count > 0)
			{
				result = new DataSetQueryRestartPosition(list);
			}
			return result;
		}

		public void AddRelationshipRestartPosition(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet, RelationshipRestartContext relationshipRestart)
		{
			List<RelationshipRestartContext> value = null;
			if (m_relationshipRestartPositions.TryGetValue(dataSet, out value))
			{
				m_relationshipRestartPositions[dataSet].Add(relationshipRestart);
				return;
			}
			value = new List<RelationshipRestartContext>();
			value.Add(relationshipRestart);
			m_relationshipRestartPositions.Add(dataSet, value);
		}

		internal bool TryAddScopeID(ScopeID scopeID, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, InternalStreamingOdpDynamicMemberLogic memberLogic)
		{
			if (IsParentScopeIDAlreadySet(memberDef))
			{
				RestartMode restartMode = (!CanMarkRestartable(memberDef)) ? RestartMode.Rom : RestartMode.Query;
				m_queryRestartPosition.Add(new ScopeIDContext(scopeID, memberDef, memberLogic, restartMode));
				return true;
			}
			return false;
		}

		internal void RomBasedRestart()
		{
			for (int i = 0; i < m_queryRestartPosition.Count; i++)
			{
				ScopeIDContext scopeIDContext = m_queryRestartPosition[i];
				if (!scopeIDContext.IsRowLevelRestart && !scopeIDContext.RomBasedRestart())
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsRombasedRestartFailed, scopeIDContext.MemberDefinition.Grouping.Name.MarkAsModelInfo());
				}
			}
			ClearRomRestartScopeIDs();
		}

		private void ClearRomRestartScopeIDs()
		{
			for (int num = m_queryRestartPosition.Count - 1; num >= 0; num--)
			{
				if (!m_queryRestartPosition[num].IsRowLevelRestart)
				{
					m_queryRestartPosition.RemoveAt(num);
				}
			}
		}

		private bool CanMarkRestartable(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef)
		{
			if (memberDef.DataScopeInfo.IsDecomposable && memberDef.Sorting != null && memberDef.Sorting.NaturalSort)
			{
				if (LastScopeIDContext != null)
				{
					return LastScopeIDContext.IsRowLevelRestart;
				}
				return true;
			}
			return false;
		}

		private bool IsParentScopeIDAlreadySet(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode target)
		{
			if (m_queryRestartPosition.Count == 0)
			{
				return !ParentScopeIsDynamic(target);
			}
			return IsParentScopeAdded(target);
		}

		private bool ParentScopeIsDynamic(IRIFReportDataScope target)
		{
			for (IRIFReportDataScope parentReportScope = target.ParentReportScope; parentReportScope != null; parentReportScope = parentReportScope.ParentReportScope)
			{
				if (parentReportScope.IsGroup)
				{
					return true;
				}
				if (parentReportScope.IsDataIntersectionScope)
				{
					IRIFReportIntersectionScope iRIFReportIntersectionScope = (IRIFReportIntersectionScope)parentReportScope;
					if (!ParentScopeIsDynamic(iRIFReportIntersectionScope.ParentRowReportScope))
					{
						return ParentScopeIsDynamic(iRIFReportIntersectionScope.ParentColumnReportScope);
					}
					return true;
				}
			}
			return false;
		}

		private bool IsParentScopeAdded(Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode target)
		{
			if (target.DataScopeInfo.DataSet != LastScopeIDContext.MemberDefinition.DataScopeInfo.DataSet && !target.IsChildScopeOf(LastScopeIDContext.MemberDefinition) && !LastScopeIDContext.MemberDefinition.IsChildScopeOf(target))
			{
				return true;
			}
			if (target.IsChildScopeOf(LastScopeIDContext.MemberDefinition))
			{
				return true;
			}
			if (!target.DataScopeInfo.IsDecomposable)
			{
				for (int num = m_queryRestartPosition.Count - 2; num >= 0; num--)
				{
					if (target.IsChildScopeOf(m_queryRestartPosition[num].MemberDefinition))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}
	}
}
