using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class TableGroup : Group, IDocumentMapEntry
	{
		private Microsoft.ReportingServices.ReportProcessing.TableGroup m_groupDef;

		private TableGroupInstance m_groupInstance;

		private TableGroupInstanceInfo m_groupInstanceInfo;

		private TableGroupCollection m_subGroups;

		private TableRowsCollection m_detailRows;

		private TableHeaderFooterRows m_headerRows;

		private TableHeaderFooterRows m_footerRows;

		private TableGroup m_parent;

		public override string ID
		{
			get
			{
				if (m_groupDef.RenderingModelID == null)
				{
					m_groupDef.RenderingModelID = m_groupDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return m_groupDef.RenderingModelID;
			}
		}

		public override string Label
		{
			get
			{
				string result = null;
				if (m_groupingDef != null && m_groupingDef.GroupLabel != null)
				{
					result = ((m_groupingDef.GroupLabel.Type == ExpressionInfo.Types.Constant) ? m_groupingDef.GroupLabel.Value : ((m_groupInstance != null) ? InstanceInfo.Label : null));
				}
				return result;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (m_groupInstance != null && m_groupingDef != null)
				{
					return m_groupingDef.GroupLabel != null;
				}
				return false;
			}
		}

		public TableHeaderFooterRows GroupHeader
		{
			get
			{
				TableHeaderFooterRows tableHeaderFooterRows = m_headerRows;
				if (m_headerRows == null && m_groupDef.HeaderRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows((Table)base.OwnerDataRegion, m_groupDef.HeaderRepeatOnNewPage, m_groupDef.HeaderRows, (m_groupInstance == null) ? null : m_groupInstance.HeaderRowInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_headerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public TableHeaderFooterRows GroupFooter
		{
			get
			{
				TableHeaderFooterRows tableHeaderFooterRows = m_footerRows;
				if (m_footerRows == null && m_groupDef.FooterRows != null)
				{
					tableHeaderFooterRows = new TableHeaderFooterRows((Table)base.OwnerDataRegion, m_groupDef.FooterRepeatOnNewPage, m_groupDef.FooterRows, (m_groupInstance == null) ? null : m_groupInstance.FooterRowInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_footerRows = tableHeaderFooterRows;
					}
				}
				return tableHeaderFooterRows;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (m_groupDef.Grouping.PageBreakAtStart)
				{
					return true;
				}
				if (m_groupDef.HeaderRows == null || m_groupDef.HeaderRepeatOnNewPage)
				{
					return m_groupDef.PropagatedPageBreakAtStart;
				}
				return false;
			}
		}

		public override bool PageBreakAtEnd
		{
			get
			{
				if (m_groupDef.Grouping.PageBreakAtEnd)
				{
					return true;
				}
				if (m_groupDef.FooterRows == null || m_groupDef.FooterRepeatOnNewPage)
				{
					return m_groupDef.PropagatedPageBreakAtEnd;
				}
				return false;
			}
		}

		public TableGroupCollection SubGroups
		{
			get
			{
				TableGroupCollection tableGroupCollection = m_subGroups;
				if (m_subGroups == null && m_groupDef.SubGroup != null)
				{
					tableGroupCollection = new TableGroupCollection((Table)base.OwnerDataRegion, this, m_groupDef.SubGroup, (m_groupInstance == null) ? null : m_groupInstance.SubGroupInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_subGroups = tableGroupCollection;
					}
				}
				return tableGroupCollection;
			}
		}

		public TableRowsCollection DetailRows
		{
			get
			{
				TableRowsCollection tableRowsCollection = m_detailRows;
				Microsoft.ReportingServices.ReportProcessing.Table table = (Microsoft.ReportingServices.ReportProcessing.Table)base.OwnerDataRegion.ReportItemDef;
				if (m_detailRows == null && m_groupDef.SubGroup == null && table.TableDetail != null)
				{
					tableRowsCollection = new TableRowsCollection((Table)base.OwnerDataRegion, table.TableDetail, (m_groupInstance == null) ? null : m_groupInstance.TableDetailInstances);
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_detailRows = tableRowsCollection;
					}
				}
				return tableRowsCollection;
			}
		}

		public override bool Hidden
		{
			get
			{
				if (m_groupInstance == null)
				{
					return true;
				}
				if (m_groupDef.Visibility == null)
				{
					return false;
				}
				if (m_groupDef.Visibility.Toggle != null)
				{
					return base.OwnerDataRegion.RenderingContext.IsItemHidden(m_groupInstance.UniqueName, potentialSender: false);
				}
				return InstanceInfo.StartHidden;
			}
		}

		public override CustomPropertyCollection CustomProperties
		{
			get
			{
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null)
				{
					if (m_groupDef.Grouping == null || m_groupDef.Grouping.CustomProperties == null)
					{
						return null;
					}
					customPropertyCollection = ((m_groupInstance != null) ? new CustomPropertyCollection(m_groupDef.Grouping.CustomProperties, InstanceInfo.CustomPropertyInstances) : new CustomPropertyCollection(m_groupDef.Grouping.CustomProperties, null));
					if (base.OwnerDataRegion.RenderingContext.CacheState)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
		}

		public TableGroup Parent => m_parent;

		internal TableGroupInstanceInfo InstanceInfo
		{
			get
			{
				if (m_groupInstance == null)
				{
					return null;
				}
				if (m_groupInstanceInfo == null)
				{
					m_groupInstanceInfo = m_groupInstance.GetInstanceInfo(base.OwnerDataRegion.RenderingContext.ChunkManager);
				}
				return m_groupInstanceInfo;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.TableGroup GroupDefinition => m_groupDef;

		internal TableGroup(Table owner, TableGroup parent, Microsoft.ReportingServices.ReportProcessing.TableGroup groupDef, TableGroupInstance groupInstance)
			: base(owner, groupDef.Grouping, groupDef.Visibility)
		{
			m_parent = parent;
			m_groupDef = groupDef;
			m_groupInstance = groupInstance;
			if (m_groupInstance != null)
			{
				m_uniqueName = m_groupInstance.UniqueName;
			}
		}

		public void GetDetailsOnThisPage(int pageIndex, out int start, out int numberOfDetails)
		{
			start = 0;
			numberOfDetails = 0;
			if (m_groupInstance.ChildrenStartAndEndPages != null)
			{
				Global.Tracer.Assert(pageIndex >= 0 && pageIndex < m_groupInstance.ChildrenStartAndEndPages.Count);
				RenderingPagesRanges renderingPagesRanges = m_groupInstance.ChildrenStartAndEndPages[pageIndex];
				start = renderingPagesRanges.StartRow;
				numberOfDetails = renderingPagesRanges.NumberOfDetails;
			}
		}

		public bool IsGroupOnThisPage(int groupIndex, int pageNumber, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			if (m_groupInstance.ChildrenStartAndEndPages == null || groupIndex >= m_groupInstance.ChildrenStartAndEndPages.Count)
			{
				return false;
			}
			RenderingPagesRanges renderingPagesRanges = m_groupInstance.ChildrenStartAndEndPages[groupIndex];
			startPage = renderingPagesRanges.StartPage;
			endPage = renderingPagesRanges.EndPage;
			if (pageNumber >= startPage)
			{
				return pageNumber <= endPage;
			}
			return false;
		}

		public void GetSubGroupsOnPage(int page, out int startGroup, out int endGroup)
		{
			startGroup = -1;
			endGroup = -1;
			if (m_groupInstance != null)
			{
				RenderingPagesRangesList childrenStartAndEndPages = m_groupInstance.ChildrenStartAndEndPages;
				if (childrenStartAndEndPages != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startGroup, ref endGroup);
				}
			}
		}
	}
}
