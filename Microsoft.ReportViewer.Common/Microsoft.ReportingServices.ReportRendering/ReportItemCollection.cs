using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ReportItemCollection
	{
		private ReportItem[] m_reportItems;

		private Microsoft.ReportingServices.ReportProcessing.ReportItemCollection m_reportItemColDef;

		private ReportItemColInstance m_reportItemColInstance;

		private NonComputedUniqueNames[] m_childrenNonComputedUniqueNames;

		private RenderingContext m_renderingContext;

		public ReportItem this[int index]
		{
			get
			{
				if (0 > index || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				ReportItem reportItem = null;
				if (m_reportItems == null || m_reportItems[index] == null)
				{
					int internalIndex = 0;
					bool computed = false;
					Microsoft.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					m_reportItemColDef.GetReportItem(index, out computed, out internalIndex, out reportItem2);
					NonComputedUniqueNames nonComputedUniqueNames = null;
					ReportItemInstance reportItemInstance = null;
					if (!computed)
					{
						if (m_childrenNonComputedUniqueNames != null)
						{
							nonComputedUniqueNames = m_childrenNonComputedUniqueNames[internalIndex];
						}
					}
					else if (m_reportItemColInstance != null)
					{
						reportItemInstance = m_reportItemColInstance[internalIndex];
					}
					reportItem = ReportItem.CreateItem(index, reportItem2, reportItemInstance, m_renderingContext, nonComputedUniqueNames);
					if (m_renderingContext.CacheState)
					{
						if (m_reportItems == null)
						{
							m_reportItems = new ReportItem[Count];
						}
						m_reportItems[index] = reportItem;
					}
				}
				else
				{
					reportItem = m_reportItems[index];
				}
				return reportItem;
			}
		}

		public int Count => m_reportItemColDef.Count;

		public object SharedRenderingInfo
		{
			get
			{
				return m_renderingContext.RenderingInfoManager.SharedRenderingInfo[m_reportItemColDef.ID];
			}
			set
			{
				m_renderingContext.RenderingInfoManager.SharedRenderingInfo[m_reportItemColDef.ID] = value;
			}
		}

		internal ReportItemCollection(Microsoft.ReportingServices.ReportProcessing.ReportItemCollection reportItemColDef, ReportItemColInstance reportItemColInstance, RenderingContext renderingContext, NonComputedUniqueNames[] childrenNonComputedUniqueNames)
		{
			if (reportItemColInstance != null)
			{
				ReportItemColInstanceInfo instanceInfo = reportItemColInstance.GetInstanceInfo(renderingContext.ChunkManager, renderingContext.InPageSection);
				Global.Tracer.Assert(childrenNonComputedUniqueNames == null || instanceInfo.ChildrenNonComputedUniqueNames == null);
				if (childrenNonComputedUniqueNames == null)
				{
					childrenNonComputedUniqueNames = instanceInfo.ChildrenNonComputedUniqueNames;
				}
			}
			m_childrenNonComputedUniqueNames = childrenNonComputedUniqueNames;
			m_reportItemColInstance = reportItemColInstance;
			m_reportItemColDef = reportItemColDef;
			m_renderingContext = renderingContext;
		}

		public void GetReportItemStartAndEndPages(int currentPage, int index, out int startPage, out int endPage)
		{
			if (0 > index || index >= Count)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
			}
			startPage = currentPage;
			endPage = currentPage;
			if (m_reportItemColInstance != null)
			{
				m_reportItemColInstance.GetReportItemStartAndEndPages(index, ref startPage, ref endPage);
			}
		}

		internal bool Search(SearchContext searchContext)
		{
			if (m_reportItemColDef.Count == 0)
			{
				return false;
			}
			bool flag = false;
			int startPage = 0;
			int endPage = 0;
			ReportItem reportItem = null;
			SearchContext searchContext2 = new SearchContext(searchContext);
			int num = 0;
			while (!flag && num < m_reportItemColDef.Count)
			{
				reportItem = this[num];
				if (searchContext.ItemStartPage != searchContext.ItemEndPage)
				{
					GetReportItemStartAndEndPages(searchContext.SearchPage, num, out startPage, out endPage);
					searchContext2.ItemStartPage = startPage;
					searchContext2.ItemEndPage = endPage;
					if (searchContext2.IsItemOnSearchPage)
					{
						flag = SearchRepeatedSiblings(reportItem as DataRegion, searchContext2);
						if (!flag)
						{
							flag = reportItem.Search(searchContext2);
						}
					}
				}
				else
				{
					flag = reportItem.Search(searchContext2);
				}
				num++;
			}
			return flag;
		}

		private bool SearchRepeatedSiblings(DataRegion dataRegion, SearchContext searchContext)
		{
			if (dataRegion == null)
			{
				return false;
			}
			bool flag = false;
			int[] repeatSiblings = dataRegion.GetRepeatSiblings();
			if (repeatSiblings != null)
			{
				int num = 0;
				SearchContext searchContext2 = new SearchContext(searchContext);
				int num2 = 0;
				while (!flag && num2 < repeatSiblings.Length)
				{
					num = repeatSiblings[num2];
					flag = this[num].Search(searchContext2);
					num2++;
				}
			}
			return flag;
		}
	}
}
