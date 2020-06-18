using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class List : DataRegion
	{
		private ListContentCollection m_contents;

		public override bool PageBreakAtEnd
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping == null)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtEnd;
				}
				if (!((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtEnd)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping.PageBreakAtEnd;
				}
				return true;
			}
		}

		public override bool PageBreakAtStart
		{
			get
			{
				if (((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping == null)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtStart;
				}
				if (!((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).PageBreakAtStart)
				{
					return ((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemDef).Grouping.PageBreakAtStart;
				}
				return true;
			}
		}

		public bool GroupBreakAtStart => Contents[0].PageBreakAtStart;

		public bool GroupBreakAtEnd => Contents[0].PageBreakAtEnd;

		public ListContentCollection Contents
		{
			get
			{
				ListContentCollection listContentCollection = m_contents;
				if (m_contents == null)
				{
					listContentCollection = new ListContentCollection(this);
					if (base.RenderingContext.CacheState)
					{
						m_contents = listContentCollection;
					}
				}
				return listContentCollection;
			}
		}

		public override bool NoRows
		{
			get
			{
				if (base.ReportItemInstance == null || ((ListInstance)base.ReportItemInstance).ListContents.Count == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal override string InstanceInfoNoRowMessage
		{
			get
			{
				if (base.InstanceInfo != null)
				{
					return ((ListInstanceInfo)base.InstanceInfo).NoRows;
				}
				return null;
			}
		}

		internal List(int intUniqueName, Microsoft.ReportingServices.ReportProcessing.List reportItemDef, ListInstance reportItemInstance, RenderingContext renderingContext)
			: base(intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
		}

		public bool IsListContentOnThisPage(int contentIndex, int pageNumber, int listStartPage, out int startPage, out int endPage)
		{
			startPage = -1;
			endPage = -1;
			RenderingPagesRangesList childrenStartAndEndPages = ((ListInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages == null)
			{
				return true;
			}
			if (((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemInstance.ReportItemDef).Grouping != null)
			{
				Global.Tracer.Assert(contentIndex >= 0 && contentIndex < childrenStartAndEndPages.Count);
				if (contentIndex >= childrenStartAndEndPages.Count)
				{
					return false;
				}
				RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[contentIndex];
				startPage = renderingPagesRanges.StartPage;
				endPage = renderingPagesRanges.EndPage;
				if (pageNumber >= startPage)
				{
					return pageNumber <= endPage;
				}
				return false;
			}
			pageNumber -= listStartPage;
			Global.Tracer.Assert(pageNumber >= 0 && pageNumber < childrenStartAndEndPages.Count);
			RenderingPagesRanges renderingPagesRanges2 = childrenStartAndEndPages[pageNumber];
			startPage = pageNumber;
			endPage = pageNumber;
			if (contentIndex >= renderingPagesRanges2.StartRow)
			{
				return contentIndex < renderingPagesRanges2.StartRow + renderingPagesRanges2.NumberOfDetails;
			}
			return false;
		}

		public void GetListContentOnPage(int page, int listStartPage, out int startChild, out int endChild)
		{
			startChild = -1;
			endChild = -1;
			if (base.ReportItemInstance == null)
			{
				return;
			}
			RenderingPagesRangesList childrenStartAndEndPages = ((ListInstance)base.ReportItemInstance).ChildrenStartAndEndPages;
			if (childrenStartAndEndPages != null)
			{
				if (((Microsoft.ReportingServices.ReportProcessing.List)base.ReportItemInstance.ReportItemDef).Grouping != null)
				{
					RenderingContext.FindRange(childrenStartAndEndPages, 0, childrenStartAndEndPages.Count - 1, page, ref startChild, ref endChild);
				}
				else if (childrenStartAndEndPages != null)
				{
					page -= listStartPage;
					Global.Tracer.Assert(page >= 0 && page < childrenStartAndEndPages.Count);
					RenderingPagesRanges renderingPagesRanges = childrenStartAndEndPages[page];
					startChild = renderingPagesRanges.StartRow;
					endChild = startChild + renderingPagesRanges.NumberOfDetails - 1;
				}
			}
		}

		internal override bool Search(SearchContext searchContext)
		{
			if (base.SkipSearch || NoRows)
			{
				return false;
			}
			bool flag = false;
			ListContentCollection contents = Contents;
			if (searchContext.ItemStartPage != searchContext.ItemEndPage)
			{
				int startChild = 0;
				int endChild = 0;
				SearchContext searchContext2 = new SearchContext(searchContext);
				GetListContentOnPage(searchContext.SearchPage, searchContext.ItemStartPage, out startChild, out endChild);
				IsListContentOnThisPage(startChild, searchContext.SearchPage, searchContext.ItemStartPage, out int startPage, out int endPage);
				searchContext2.ItemStartPage = startPage;
				searchContext2.ItemEndPage = endPage;
				flag = SearchPartialList(contents, searchContext2, startChild, startChild);
				startChild++;
				if (!flag && startChild < endChild)
				{
					searchContext2.ItemStartPage = searchContext.SearchPage;
					searchContext2.ItemEndPage = searchContext.SearchPage;
					flag = SearchPartialList(contents, searchContext2, startChild, endChild - 1);
					startChild = endChild;
				}
				if (!flag && startChild == endChild)
				{
					IsListContentOnThisPage(endChild, searchContext.SearchPage, searchContext.ItemStartPage, out startPage, out endPage);
					searchContext2.ItemStartPage = startPage;
					searchContext2.ItemEndPage = endPage;
					flag = SearchPartialList(contents, searchContext2, endChild, endChild);
				}
			}
			else
			{
				flag = SearchFullList(contents, searchContext);
			}
			return flag;
		}

		internal static bool SearchPartialList(ListContentCollection contents, SearchContext searchContext, int startChild, int endChild)
		{
			if (contents == null)
			{
				return false;
			}
			bool flag = false;
			while (startChild <= endChild && !flag)
			{
				flag = contents[startChild].ReportItemCollection.Search(searchContext);
				startChild++;
			}
			return flag;
		}

		internal static bool SearchFullList(ListContentCollection contents, SearchContext searchContext)
		{
			if (contents == null)
			{
				return false;
			}
			bool flag = false;
			ListContent listContent = null;
			for (int i = 0; i < contents.Count; i++)
			{
				if (flag)
				{
					break;
				}
				listContent = contents[i];
				if (!listContent.Hidden)
				{
					flag = listContent.ReportItemCollection.Search(searchContext);
				}
			}
			return flag;
		}
	}
}
