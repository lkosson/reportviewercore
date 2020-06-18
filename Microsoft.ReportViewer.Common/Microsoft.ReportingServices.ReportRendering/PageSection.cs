using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class PageSection : ReportItem
	{
		private ReportItemCollection m_reportItems;

		private Microsoft.ReportingServices.ReportProcessing.PageSection m_pageSectionDef;

		private bool m_pageDef;

		private PageSectionInstance m_pageSectionInstance;

		internal const string PageHeaderUniqueNamePrefix = "ph";

		internal const string PageFooterUniqueNamePrefix = "pf";

		public bool PrintOnFirstPage => ((Microsoft.ReportingServices.ReportProcessing.PageSection)base.ReportItemDef).PrintOnFirstPage;

		public bool PrintOnLastPage => ((Microsoft.ReportingServices.ReportProcessing.PageSection)base.ReportItemDef).PrintOnLastPage;

		public ReportItemCollection ReportItemCollection
		{
			get
			{
				ReportItemCollection reportItemCollection = m_reportItems;
				if (m_reportItems == null)
				{
					reportItemCollection = new ReportItemCollection(m_pageSectionDef.ReportItems, (m_pageSectionInstance == null) ? null : m_pageSectionInstance.ReportItemColInstance, base.RenderingContext, null);
					if (base.RenderingContext.CacheState)
					{
						m_reportItems = reportItemCollection;
					}
				}
				return reportItemCollection;
			}
		}

		internal PageSection(string uniqueName, Microsoft.ReportingServices.ReportProcessing.PageSection pageSectionDef, PageSectionInstance pageSectionInstance, Report report, RenderingContext renderingContext, bool pageDef)
			: base(uniqueName, 0, pageSectionDef, pageSectionInstance, renderingContext)
		{
			m_pageSectionDef = pageSectionDef;
			m_pageSectionInstance = pageSectionInstance;
			m_pageDef = pageDef;
		}

		public ReportItem Find(string uniqueName)
		{
			if (uniqueName == null || uniqueName.Length <= 0)
			{
				return null;
			}
			if (uniqueName.Equals(base.UniqueName))
			{
				return this;
			}
			char[] separator = new char[1]
			{
				'a'
			};
			string[] array = uniqueName.Split(separator);
			if (array == null || array.Length < 2)
			{
				return null;
			}
			object obj = (m_pageSectionInstance != null) ? ((ISearchByUniqueName)m_pageSectionInstance) : ((ISearchByUniqueName)m_pageSectionDef);
			NonComputedUniqueNames nonComputedUniqueNames = null;
			int num = -1;
			for (int i = 1; i < array.Length; i++)
			{
				IIndexInto indexInto = obj as IIndexInto;
				if (indexInto == null)
				{
					obj = null;
					break;
				}
				num = ReportItem.StringToInt(array[i]);
				NonComputedUniqueNames nonCompNames = null;
				obj = indexInto.GetChildAt(num, out nonCompNames);
				if (nonComputedUniqueNames == null)
				{
					nonComputedUniqueNames = nonCompNames;
					continue;
				}
				if (nonComputedUniqueNames.ChildrenUniqueNames == null || num < 0 || num >= nonComputedUniqueNames.ChildrenUniqueNames.Length)
				{
					return null;
				}
				nonComputedUniqueNames = nonComputedUniqueNames.ChildrenUniqueNames[num];
			}
			if (obj == null)
			{
				return null;
			}
			if (obj is Microsoft.ReportingServices.ReportProcessing.ReportItem)
			{
				Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef = (Microsoft.ReportingServices.ReportProcessing.ReportItem)obj;
				return ReportItem.CreateItem(uniqueName, reportItemDef, null, base.RenderingContext, nonComputedUniqueNames);
			}
			ReportItemInstance reportItemInstance = (ReportItemInstance)obj;
			return ReportItem.CreateItem(uniqueName, reportItemInstance.ReportItemDef, reportItemInstance, base.RenderingContext, nonComputedUniqueNames);
		}

		internal override bool Search(SearchContext searchContext)
		{
			return ReportItemCollection?.Search(searchContext) ?? false;
		}
	}
}
