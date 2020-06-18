using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupInstance : BaseInstance
	{
		private string m_uniqueName;

		private bool m_documentMapLabelEvaluated;

		private string m_documentMapLabel;

		private GroupExpressionValueCollection m_groupExpressions;

		private Group m_owner;

		private int m_recursiveLevel = -1;

		private bool m_pageNameEvaluated;

		private string m_pageName;

		public string UniqueName
		{
			get
			{
				if (m_owner.IsOldSnapshot)
				{
					if (m_owner.CurrentRenderGroupIndex >= 0)
					{
						if (m_owner.IsDetailGroup)
						{
							return m_owner.TableDetailMember.DetailInstanceUniqueName;
						}
						return m_owner.CurrentShimRenderGroup.UniqueName;
					}
					return string.Empty;
				}
				if (m_uniqueName == null)
				{
					m_uniqueName = InstancePathItem.GenerateUniqueNameString(m_owner.MemberDefinition.ID, m_owner.MemberDefinition.InstancePath);
				}
				return m_uniqueName;
			}
		}

		public string DocumentMapLabel
		{
			get
			{
				if (!m_documentMapLabelEvaluated)
				{
					m_documentMapLabelEvaluated = true;
					if (m_owner.IsOldSnapshot)
					{
						if (!m_owner.IsDetailGroup && m_owner.CurrentRenderGroupIndex >= 0)
						{
							m_documentMapLabel = m_owner.CurrentShimRenderGroup.Label;
						}
					}
					else if (m_owner.MemberDefinition.Grouping != null && m_owner.MemberDefinition.Grouping.GroupLabel != null)
					{
						m_documentMapLabel = m_owner.MemberDefinition.Grouping.EvaluateGroupingLabelExpression(ReportScopeInstance, m_owner.OwnerDataRegion.RenderingContext.OdpContext);
					}
				}
				return m_documentMapLabel;
			}
		}

		public GroupExpressionValueCollection GroupExpressions
		{
			get
			{
				if (m_groupExpressions == null)
				{
					if (m_owner.IsOldSnapshot)
					{
						if (!m_owner.IsDetailGroup && m_owner.OwnerDataRegion != null && m_owner.CurrentRenderGroupIndex >= 0 && m_owner.OwnerDataRegion.DataRegionType == DataRegion.Type.Chart)
						{
							ChartHeadingInstanceInfo instanceInfo = ((Microsoft.ReportingServices.ReportRendering.ChartMember)m_owner.CurrentShimRenderGroup).InstanceInfo;
							if (instanceInfo != null)
							{
								if (m_groupExpressions == null)
								{
									m_groupExpressions = new GroupExpressionValueCollection();
								}
								m_groupExpressions.UpdateValues(instanceInfo.GroupExpressionValue);
							}
						}
					}
					else if (!m_owner.IsDetailGroup && m_owner.OwnerDataRegion != null && m_owner.MemberDefinition.CurrentMemberIndex >= 0)
					{
						object[] groupInstanceExpressionValues = m_owner.MemberDefinition.Grouping.GetGroupInstanceExpressionValues(ReportScopeInstance, m_owner.OwnerDataRegion.RenderingContext.OdpContext);
						if (m_groupExpressions == null)
						{
							m_groupExpressions = new GroupExpressionValueCollection();
						}
						m_groupExpressions.UpdateValues(groupInstanceExpressionValues);
					}
				}
				return m_groupExpressions;
			}
		}

		public int RecursiveLevel
		{
			get
			{
				if (m_recursiveLevel < 0 && !m_owner.IsOldSnapshot)
				{
					m_recursiveLevel = m_owner.MemberDefinition.Grouping.GetRecursiveLevel(ReportScopeInstance, m_owner.OwnerDataRegion.RenderingContext.OdpContext);
				}
				return m_recursiveLevel;
			}
		}

		public string PageName
		{
			get
			{
				if (!m_pageNameEvaluated)
				{
					if (m_owner.IsOldSnapshot)
					{
						m_pageName = null;
					}
					else
					{
						m_pageNameEvaluated = true;
						Microsoft.ReportingServices.ReportIntermediateFormat.Grouping grouping = m_owner.MemberDefinition.Grouping;
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo pageName = grouping.PageName;
						if (pageName != null)
						{
							if (pageName.IsExpression)
							{
								m_pageName = grouping.EvaluatePageName(ReportScopeInstance, m_owner.OwnerDataRegion.RenderingContext.OdpContext);
							}
							else
							{
								m_pageName = pageName.StringValue;
							}
						}
					}
				}
				return m_pageName;
			}
		}

		internal GroupInstance(Group owner)
			: base(null)
		{
			m_owner = owner;
		}

		internal GroupInstance(Group owner, IReportScope reportScope)
			: base(reportScope)
		{
			m_owner = owner;
		}

		protected override void ResetInstanceCache()
		{
			m_uniqueName = null;
			m_documentMapLabelEvaluated = false;
			m_documentMapLabel = null;
			m_groupExpressions = null;
			m_recursiveLevel = -1;
			m_pageNameEvaluated = false;
			m_pageName = null;
			if (!m_owner.IsOldSnapshot && m_owner.MemberDefinition.Grouping != null)
			{
				m_owner.MemberDefinition.Grouping.ResetReportItemsWithHideDuplicates();
			}
		}
	}
}
