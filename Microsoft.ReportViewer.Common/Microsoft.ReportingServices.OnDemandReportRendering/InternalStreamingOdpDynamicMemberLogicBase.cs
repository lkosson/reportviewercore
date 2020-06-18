using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class InternalStreamingOdpDynamicMemberLogicBase : InternalDynamicMemberLogic
	{
		protected readonly DataRegionMember m_memberDef;

		protected readonly OnDemandProcessingContext m_odpContext;

		protected readonly Microsoft.ReportingServices.ReportIntermediateFormat.Grouping m_grouping;

		private readonly Microsoft.ReportingServices.ReportIntermediateFormat.Sorting m_sorting;

		private readonly List<ScopeIDType> m_memberGroupAndSortExpressionFlag;

		private ScopeID m_scopeID;

		private ScopeID m_lastScopeID;

		protected InternalStreamingOdpDynamicMemberLogicBase(DataRegionMember memberDef, OnDemandProcessingContext odpContext)
		{
			m_memberDef = memberDef;
			m_sorting = m_memberDef.DataRegionMemberDefinition.Sorting;
			m_grouping = m_memberDef.DataRegionMemberDefinition.Grouping;
			m_memberGroupAndSortExpressionFlag = m_memberDef.DataRegionMemberDefinition.MemberGroupAndSortExpressionFlag;
			m_odpContext = odpContext;
		}

		public override void ResetContext()
		{
			m_isNewContext = true;
			m_currentContext = -1;
			m_scopeID = null;
			m_memberDef.DataRegionMemberDefinition.InstanceCount = -1;
			m_memberDef.DataRegionMemberDefinition.InstancePathItem.ResetContext();
			((IRIFReportDataScope)m_memberDef.ReportScope.RIFReportScope).ClearStreamingScopeInstanceBinding();
		}

		protected bool MoveNextCore(System.Action actionOnNextInstance)
		{
			IRIFReportDataScope obj = (IRIFReportDataScope)m_memberDef.ReportScope.RIFReportScope;
			if (obj.IsBoundToStreamingScopeInstance)
			{
				m_odpContext.BindNextMemberInstance(m_memberDef.DataRegionMemberDefinition, m_memberDef.ReportScopeInstance, m_currentContext + 1);
			}
			else
			{
				m_odpContext.SetupContext(m_memberDef.DataRegionMemberDefinition, m_memberDef.ReportScopeInstance, -1);
			}
			if (obj.CurrentStreamingScopeInstance.Value().IsNoRows)
			{
				return false;
			}
			actionOnNextInstance?.Invoke();
			m_isNewContext = true;
			m_currentContext++;
			m_memberDef.DataRegionMemberDefinition.InstancePathItem.MoveNext();
			m_memberDef.SetNewContext(fromMoveNext: true);
			return true;
		}

		public override bool SetInstanceIndex(int index)
		{
			ResetContext();
			if (index < 0)
			{
				return true;
			}
			int i;
			for (i = -1; i < index; i++)
			{
				if (!MoveNext())
				{
					break;
				}
			}
			return i == index;
		}

		internal override ScopeID GetScopeID()
		{
			if (m_grouping.IsDetail)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsDetailGroupsNotSupportedInStreamingMode, "GetScopeID");
			}
			if (m_scopeID != null)
			{
				return m_scopeID;
			}
			m_odpContext.SetupContext(m_memberDef.DataRegionMemberDefinition, m_memberDef.ReportScopeInstance);
			IRIFReportDataScope iRIFReportDataScope = (IRIFReportDataScope)m_memberDef.ReportScope.RIFReportScope;
			if (iRIFReportDataScope.IsBoundToStreamingScopeInstance && !iRIFReportDataScope.CurrentStreamingScopeInstance.Value().IsNoRows)
			{
				List<ScopeValue> list = null;
				IOnDemandMemberInstance onDemandMemberInstance = (IOnDemandMemberInstance)iRIFReportDataScope.CurrentStreamingScopeInstance.Value();
				m_scopeID = new ScopeID(EvaluateSortAndGroupExpressionValues(onDemandMemberInstance.GroupExprValues)?.ToArray());
			}
			return m_scopeID;
		}

		internal override ScopeID GetLastScopeID()
		{
			return m_lastScopeID;
		}

		private List<ScopeValue> EvaluateSortAndGroupExpressionValues(List<object> groupExpressionValues)
		{
			if (m_memberGroupAndSortExpressionFlag == null)
			{
				return null;
			}
			return AddSortAndGroupExpressionValues(groupExpressionValues);
		}

		private List<ScopeValue> AddSortAndGroupExpressionValues(List<object> groupExpValues)
		{
			List<ScopeValue> list = new List<ScopeValue>(m_memberGroupAndSortExpressionFlag.Count);
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < m_memberGroupAndSortExpressionFlag.Count; i++)
			{
				ScopeValue scopeValue = null;
				switch (m_memberGroupAndSortExpressionFlag[i])
				{
				case ScopeIDType.SortValues:
					scopeValue = CreateScopeValueFromSortExpression(num, m_memberGroupAndSortExpressionFlag[i]);
					num++;
					break;
				case ScopeIDType.GroupValues:
					scopeValue = new ScopeValue(NormalizeValue(groupExpValues[num2]), m_memberGroupAndSortExpressionFlag[i]);
					num2++;
					break;
				case ScopeIDType.SortGroup:
					scopeValue = ((groupExpValues.Count != 1) ? CreateScopeValueFromSortExpression(num, m_memberGroupAndSortExpressionFlag[i]) : new ScopeValue(NormalizeValue(groupExpValues[num2]), m_memberGroupAndSortExpressionFlag[i]));
					num2++;
					num++;
					break;
				}
				if (scopeValue != null)
				{
					list.Add(scopeValue);
				}
			}
			return list;
		}

		private ScopeValue CreateScopeValueFromSortExpression(int sortCursor, ScopeIDType scopeIdType)
		{
			RuntimeExpressionInfo runtimeExpression = new RuntimeExpressionInfo(m_sorting.SortExpressions, m_sorting.ExprHost, m_sorting.SortDirections, sortCursor);
			object value = m_odpContext.ReportRuntime.EvaluateRuntimeExpression(runtimeExpression, ObjectType.Grouping, m_memberDef.DataRegionMemberDefinition.Grouping.Name, "Sort");
			return new ScopeValue(NormalizeValue(value), scopeIdType);
		}

		private object NormalizeValue(object value)
		{
			if (value is DBNull)
			{
				return null;
			}
			return value;
		}

		protected void ResetScopeID()
		{
			m_scopeID = null;
		}
	}
}
