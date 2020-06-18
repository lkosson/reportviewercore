using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Group : IPageBreakItem
	{
		private bool m_isOldSnapshot;

		private bool m_isDetailGroup;

		private DataRegion m_ownerItem;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode m_memberDef;

		private PageBreak m_pageBreak;

		private ReportStringProperty m_pageName;

		private GroupExpressionCollection m_groupExpressions;

		private CustomPropertyCollection m_customProperties;

		private ReportStringProperty m_documentMapLabel;

		private GroupInstance m_instance;

		private DataRegionMember m_dataMember;

		private ShimTablixMember m_dynamicMember;

		private ShimTableMember m_tableDetailMember;

		private CustomReportItem m_criOwner;

		private ShimRenderGroups m_renderGroups;

		private Microsoft.ReportingServices.ReportRendering.Group m_currentRenderGroupCache;

		private int m_currentRenderGroupIndex = -1;

		public string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_currentRenderGroupIndex < 0 || m_renderGroups == null)
					{
						return null;
					}
					return m_renderGroups[m_currentRenderGroupIndex].ID;
				}
				if (m_memberDef != null)
				{
					return m_memberDef.RenderingModelID;
				}
				return null;
			}
		}

		public string Name
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderGroups == null)
					{
						return null;
					}
					return CurrentShimRenderGroup.Name;
				}
				if (m_memberDef != null && m_memberDef.Grouping != null)
				{
					return m_memberDef.Grouping.Name;
				}
				return null;
			}
		}

		public ReportStringProperty DocumentMapLabel
		{
			get
			{
				if (m_documentMapLabel == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderGroups == null)
						{
							return null;
						}
						if (CurrentShimRenderGroup.m_groupingDef != null)
						{
							Microsoft.ReportingServices.ReportProcessing.ExpressionInfo groupLabel = CurrentShimRenderGroup.m_groupingDef.GroupLabel;
							m_documentMapLabel = new ReportStringProperty(groupLabel);
						}
					}
					else
					{
						if (m_memberDef == null || m_memberDef.Grouping == null)
						{
							return null;
						}
						Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo groupLabel2 = m_memberDef.Grouping.GroupLabel;
						if (groupLabel2 != null)
						{
							m_documentMapLabel = new ReportStringProperty(groupLabel2.IsExpression, groupLabel2.OriginalText, groupLabel2.StringValue);
						}
						else
						{
							m_documentMapLabel = new ReportStringProperty();
						}
					}
				}
				return m_documentMapLabel;
			}
		}

		public PageBreak PageBreak
		{
			get
			{
				if (m_pageBreak == null)
				{
					RenderingContext renderingContext = (m_criOwner != null) ? m_criOwner.RenderingContext : m_ownerItem.RenderingContext;
					if (IsOldSnapshot)
					{
						if (m_dynamicMember != null)
						{
							m_pageBreak = new PageBreak(renderingContext, m_dataMember, m_dynamicMember.PropagatedGroupBreak);
						}
						else
						{
							m_pageBreak = new PageBreak(renderingContext, null, PageBreakLocation.None);
						}
					}
					else if (m_memberDef != null && m_memberDef.Grouping != null)
					{
						m_pageBreak = new PageBreak(renderingContext, m_dataMember, m_memberDef.Grouping);
					}
					else
					{
						m_pageBreak = new PageBreak(renderingContext, m_dataMember, PageBreakLocation.None);
					}
				}
				return m_pageBreak;
			}
		}

		public ReportStringProperty PageName
		{
			get
			{
				if (m_pageName == null)
				{
					if (m_isOldSnapshot)
					{
						m_pageName = new ReportStringProperty();
					}
					else if (m_memberDef != null && m_memberDef.Grouping != null)
					{
						m_pageName = new ReportStringProperty(m_memberDef.Grouping.PageName);
					}
					else
					{
						m_pageName = new ReportStringProperty();
					}
				}
				return m_pageName;
			}
		}

		public GroupExpressionCollection GroupExpressions
		{
			get
			{
				if (m_groupExpressions == null)
				{
					if (m_isOldSnapshot)
					{
						if (CurrentShimRenderGroup != null && CurrentShimRenderGroup.m_groupingDef != null)
						{
							m_groupExpressions = new GroupExpressionCollection(CurrentShimRenderGroup.m_groupingDef);
						}
					}
					else
					{
						m_groupExpressions = new GroupExpressionCollection(m_memberDef.Grouping);
					}
				}
				return m_groupExpressions;
			}
		}

		internal CustomPropertyCollection CustomProperties
		{
			get
			{
				if (m_customProperties == null && m_isOldSnapshot && CurrentShimRenderGroup != null && CurrentShimRenderGroup.CustomProperties != null)
				{
					RenderingContext renderingContext = (m_criOwner != null) ? m_criOwner.RenderingContext : m_ownerItem.RenderingContext;
					m_customProperties = new CustomPropertyCollection(renderingContext, CurrentShimRenderGroup.CustomProperties);
					if (m_currentRenderGroupIndex < 0)
					{
						m_customProperties.UpdateCustomProperties(null);
					}
				}
				return m_customProperties;
			}
		}

		public string DataElementName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderGroups != null)
					{
						if (m_criOwner == null)
						{
							return CurrentShimRenderGroup.DataElementName;
						}
						return null;
					}
					if (DataRegion.Type.Table == m_ownerItem.DataRegionType)
					{
						return ((Tablix)m_ownerItem).RenderTable.DetailDataElementName;
					}
					return null;
				}
				if (m_memberDef == null || m_memberDef.Grouping == null)
				{
					return null;
				}
				return m_memberDef.Grouping.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderGroups != null)
					{
						if (m_criOwner == null)
						{
							return (DataElementOutputTypes)CurrentShimRenderGroup.DataElementOutput;
						}
						return DataElementOutputTypes.Output;
					}
					if (DataRegion.Type.Table == m_ownerItem.DataRegionType)
					{
						return (DataElementOutputTypes)((Tablix)m_ownerItem).RenderTable.DetailDataElementOutput;
					}
					return DataElementOutputTypes.Output;
				}
				if (m_memberDef == null || m_memberDef.Grouping == null)
				{
					return DataElementOutputTypes.Output;
				}
				return m_memberDef.Grouping.DataElementOutput;
			}
		}

		public bool IsRecursive
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_renderGroups != null)
					{
						Microsoft.ReportingServices.ReportProcessing.Grouping groupingDef = CurrentShimRenderGroup.m_groupingDef;
						if (groupingDef != null)
						{
							if (groupingDef.Parent != null)
							{
								return groupingDef.Parent.Count > 0;
							}
							return false;
						}
					}
				}
				else if (m_memberDef != null && m_memberDef.Grouping != null)
				{
					if (m_memberDef.Grouping.Parent != null)
					{
						return m_memberDef.Grouping.Parent.Count > 0;
					}
					return false;
				}
				return false;
			}
		}

		internal DataRegion OwnerDataRegion => m_ownerItem;

		[Obsolete("Use PageBreak.BreakLocation instead.")]
		PageBreakLocation IPageBreakItem.PageBreakLocation
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_dynamicMember != null)
					{
						return m_dynamicMember.PropagatedGroupBreak;
					}
				}
				else if (m_memberDef != null && m_memberDef.Grouping != null && m_memberDef.Grouping.PageBreak != null)
				{
					return m_memberDef.Grouping.PageBreak.BreakLocation;
				}
				return PageBreakLocation.None;
			}
		}

		internal ShimRenderGroups RenderGroups
		{
			get
			{
				return m_renderGroups;
			}
			set
			{
				m_renderGroups = value;
			}
		}

		internal int CurrentRenderGroupIndex
		{
			get
			{
				return m_currentRenderGroupIndex;
			}
			set
			{
				if (value != 0 || m_currentRenderGroupIndex != -1)
				{
					m_currentRenderGroupCache = null;
				}
				m_currentRenderGroupIndex = value;
				if (m_instance != null)
				{
					m_instance.SetNewContext();
				}
				if (m_isOldSnapshot && m_renderGroups != null && m_customProperties != null)
				{
					if (m_currentRenderGroupIndex < 0)
					{
						m_customProperties.UpdateCustomProperties(null);
					}
					else
					{
						m_customProperties.UpdateCustomProperties(m_renderGroups[m_currentRenderGroupIndex].CustomProperties);
					}
				}
			}
		}

		internal Microsoft.ReportingServices.ReportRendering.Group CurrentShimRenderGroup
		{
			get
			{
				if (m_isOldSnapshot && m_renderGroups != null)
				{
					if (m_currentRenderGroupCache == null)
					{
						if (m_currentRenderGroupIndex < 0)
						{
							m_currentRenderGroupCache = m_renderGroups[0];
						}
						else
						{
							m_currentRenderGroupCache = m_renderGroups[m_currentRenderGroupIndex];
						}
					}
					return m_currentRenderGroupCache;
				}
				return null;
			}
		}

		internal bool IsOldSnapshot => m_isOldSnapshot;

		internal bool IsDetailGroup => m_isDetailGroup;

		internal ShimTableMember TableDetailMember => m_tableDetailMember;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode MemberDefinition => m_memberDef;

		public GroupInstance Instance
		{
			get
			{
				if ((m_ownerItem != null && m_ownerItem.RenderingContext.InstanceAccessDisallowed) || (m_criOwner != null && m_criOwner.RenderingContext.InstanceAccessDisallowed))
				{
					return null;
				}
				if (m_instance == null)
				{
					if (m_isOldSnapshot)
					{
						m_instance = new GroupInstance(this);
					}
					else
					{
						m_instance = new GroupInstance(this, m_dataMember);
					}
				}
				return m_instance;
			}
		}

		internal Group(DataRegion owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, DataRegionMember dataMember)
		{
			m_isOldSnapshot = false;
			m_ownerItem = owner;
			m_memberDef = memberDef;
			m_dataMember = dataMember;
		}

		internal Group(CustomReportItem owner, Microsoft.ReportingServices.ReportIntermediateFormat.ReportHierarchyNode memberDef, DataRegionMember dataMember)
		{
			m_isOldSnapshot = false;
			m_criOwner = owner;
			m_memberDef = memberDef;
			m_dataMember = dataMember;
		}

		internal Group(DataRegion owner, ShimRenderGroups renderGroups, ShimTablixMember dynamicMember)
		{
			m_isOldSnapshot = true;
			m_ownerItem = owner;
			m_renderGroups = renderGroups;
			m_dynamicMember = dynamicMember;
		}

		internal Group(DataRegion owner, ShimRenderGroups renderGroups)
		{
			m_isOldSnapshot = true;
			m_ownerItem = owner;
			m_renderGroups = renderGroups;
		}

		internal Group(DataRegion owner, ShimTableMember tableDetailMember)
		{
			m_isOldSnapshot = true;
			m_isDetailGroup = true;
			m_tableDetailMember = tableDetailMember;
			m_dynamicMember = tableDetailMember;
			m_ownerItem = owner;
			m_renderGroups = null;
		}

		internal Group(CustomReportItem owner, ShimRenderGroups renderGroups)
		{
			m_isOldSnapshot = true;
			m_renderGroups = renderGroups;
			m_criOwner = owner;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_pageBreak != null)
			{
				m_pageBreak.SetNewContext();
			}
		}
	}
}
