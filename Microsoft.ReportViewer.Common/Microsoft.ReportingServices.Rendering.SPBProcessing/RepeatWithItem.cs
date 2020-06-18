using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class RepeatWithItem
	{
		internal const string RepeatSuffix = "_REPEAT";

		private double m_relativeTop;

		private double m_relativeBottom;

		private double m_relativeTopToBottom;

		private int m_dataRegionIndex;

		private PageItem m_pageItem;

		private ItemSizes m_renderItemSize;

		internal int DataRegionIndex => m_dataRegionIndex;

		internal PageItem SourcePageItem => m_pageItem;

		internal double RelativeTop => m_relativeTop;

		internal double RelativeBottom => m_relativeBottom;

		internal RepeatWithItem(PageItem pageItem, PageContext pageContext)
		{
			m_pageItem = pageItem;
			m_pageItem.ItemState = PageItem.State.OnPage;
		}

		internal void UpdateCreateState(PageItem dataRegion, int dataRegionIndex, List<int> pageItemsAbove, PageContext pageContext)
		{
			m_dataRegionIndex = dataRegionIndex;
			m_relativeTop = m_pageItem.ItemPageSizes.Top - dataRegion.ItemPageSizes.Top;
			m_relativeBottom = m_pageItem.ItemPageSizes.Bottom - dataRegion.ItemPageSizes.Bottom;
			m_relativeTopToBottom = m_pageItem.ItemPageSizes.Top - dataRegion.ItemPageSizes.Bottom;
			if (pageItemsAbove != null)
			{
				m_pageItem.PageItemsAbove = new List<int>(pageItemsAbove);
			}
			PaddItemSizes paddItemSizes = m_pageItem.ItemRenderSizes as PaddItemSizes;
			if (paddItemSizes != null)
			{
				if (pageContext != null)
				{
					m_renderItemSize = pageContext.GetSharedRenderRepeatItemSizesElement(paddItemSizes, isPadded: true, returnPaddings: true);
				}
				else
				{
					m_renderItemSize = new PaddItemSizes(paddItemSizes);
				}
			}
			else if (pageContext != null)
			{
				m_renderItemSize = pageContext.GetSharedRenderRepeatItemSizesElement(m_pageItem.ItemRenderSizes, isPadded: false, returnPaddings: false);
			}
			else
			{
				m_renderItemSize = new ItemSizes(m_pageItem.ItemRenderSizes);
			}
		}

		internal void UpdateSizes(PageContext pageContext)
		{
			PaddItemSizes paddItemSizes = m_renderItemSize as PaddItemSizes;
			if (paddItemSizes != null)
			{
				if (pageContext != null)
				{
					m_pageItem.ItemRenderSizes = pageContext.GetSharedRenderItemSizesElement(paddItemSizes, isPadded: true, returnPaddings: true);
				}
				else
				{
					m_pageItem.ItemRenderSizes = new PaddItemSizes(paddItemSizes);
				}
			}
			else if (pageContext != null)
			{
				m_pageItem.ItemRenderSizes = pageContext.GetSharedRenderItemSizesElement(m_renderItemSize, isPadded: false, returnPaddings: false);
			}
			else
			{
				m_pageItem.ItemRenderSizes = new ItemSizes(m_renderItemSize);
			}
		}

		internal bool AddOnPage(ItemSizes dataRegionSizes, PageItem[] siblings, int itemIndex, ref List<int> parentOverlappedItems, ref double header)
		{
			if (siblings == null)
			{
				return true;
			}
			double num = dataRegionSizes.Top + m_relativeTop;
			num = ((!(m_relativeTopToBottom < 0.0)) ? (dataRegionSizes.Bottom - dataRegionSizes.DeltaY + m_relativeTopToBottom) : (dataRegionSizes.Top + m_relativeTop));
			double x = num + m_pageItem.ItemRenderSizes.Height - m_pageItem.ItemRenderSizes.DeltaY;
			PageItem pageItem = null;
			List<int> list = null;
			RoundedDouble roundedDouble = new RoundedDouble(0.0);
			RoundedDouble roundedDouble2 = new RoundedDouble(0.0);
			for (int i = 0; i < siblings.Length; i++)
			{
				pageItem = siblings[i];
				if (pageItem == null || pageItem.ItemState == PageItem.State.Below || pageItem.ItemState == PageItem.State.TopNextPage)
				{
					continue;
				}
				roundedDouble.Value = pageItem.ItemRenderSizes.Left;
				roundedDouble2.Value = pageItem.ItemRenderSizes.Right - pageItem.ItemRenderSizes.DeltaX;
				if (roundedDouble2 <= m_pageItem.ItemRenderSizes.Left || roundedDouble >= m_pageItem.ItemRenderSizes.Right - m_pageItem.ItemRenderSizes.DeltaX)
				{
					continue;
				}
				roundedDouble.Value = pageItem.ItemRenderSizes.Top;
				roundedDouble2.Value = pageItem.ItemRenderSizes.Bottom - pageItem.ItemRenderSizes.DeltaY;
				if (roundedDouble2 <= num || roundedDouble >= x)
				{
					roundedDouble2.Value = pageItem.ItemRenderSizes.Bottom;
					x = num + m_pageItem.ItemRenderSizes.Height;
					if (roundedDouble2 <= num || roundedDouble >= x)
					{
						continue;
					}
				}
				if (roundedDouble >= num)
				{
					if (pageItem.PageItemsAbove == null)
					{
						return false;
					}
					if (pageItem.PageItemsAbove.BinarySearch(itemIndex) < 0)
					{
						return false;
					}
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(i);
				}
				else
				{
					if (m_pageItem.PageItemsAbove == null)
					{
						return false;
					}
					if (m_pageItem.PageItemsAbove.BinarySearch(i) < 0)
					{
						return false;
					}
					if (list == null)
					{
						list = new List<int>();
					}
					list.Add(itemIndex);
				}
			}
			m_pageItem.ItemRenderSizes.Top = num;
			header = Math.Min(header, num);
			if (parentOverlappedItems == null)
			{
				parentOverlappedItems = list;
			}
			else if (list != null)
			{
				int j = 0;
				for (int k = 0; k < list.Count; k++)
				{
					for (; parentOverlappedItems[j] < list[k]; j++)
					{
					}
					if (j < parentOverlappedItems.Count)
					{
						if (parentOverlappedItems[j] > list[k])
						{
							parentOverlappedItems.Insert(j, list[k]);
						}
					}
					else
					{
						parentOverlappedItems.Add(list[k]);
					}
					j++;
				}
			}
			return true;
		}

		internal void WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			m_pageItem.WriteRepeatWithToPage(rplWriter, pageContext);
		}

		internal void UpdateItem(PageItemHelper itemHelper, RPLWriter rplWriter, PageContext pageContext)
		{
			if (itemHelper != null)
			{
				PageItemRepeatWithHelper pageItemRepeatWithHelper = itemHelper as PageItemRepeatWithHelper;
				RSTrace.RenderingTracer.Assert(pageItemRepeatWithHelper != null, "This should be a RepeatWith");
				m_relativeTop = pageItemRepeatWithHelper.RelativeTop;
				m_relativeBottom = pageItemRepeatWithHelper.RelativeBottom;
				m_relativeTopToBottom = pageItemRepeatWithHelper.RelativeTopToBottom;
				m_dataRegionIndex = pageItemRepeatWithHelper.DataRegionIndex;
				if (pageItemRepeatWithHelper.RenderItemSize != null)
				{
					m_renderItemSize = pageItemRepeatWithHelper.RenderItemSize.GetNewItem();
				}
				if (m_pageItem != null)
				{
					PageContext pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Repeated);
					m_pageItem.CalculateRepeatWithPage(rplWriter, pageContext2, null);
				}
			}
		}

		internal void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)13);
				reportPageInfo.Write((byte)12);
				reportPageInfo.Write(m_relativeTop);
				reportPageInfo.Write((byte)13);
				reportPageInfo.Write(m_relativeBottom);
				reportPageInfo.Write((byte)14);
				reportPageInfo.Write(m_relativeTopToBottom);
				reportPageInfo.Write((byte)15);
				reportPageInfo.Write(m_dataRegionIndex);
				if (m_renderItemSize != null)
				{
					m_renderItemSize.WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal PageItemHelper WritePaginationInfo()
		{
			PageItemRepeatWithHelper pageItemRepeatWithHelper = new PageItemRepeatWithHelper(13);
			pageItemRepeatWithHelper.RelativeTop = m_relativeTop;
			pageItemRepeatWithHelper.RelativeBottom = m_relativeBottom;
			pageItemRepeatWithHelper.RelativeTopToBottom = m_relativeTopToBottom;
			pageItemRepeatWithHelper.DataRegionIndex = m_dataRegionIndex;
			if (m_renderItemSize != null)
			{
				pageItemRepeatWithHelper.RenderItemSize = m_renderItemSize.WritePaginationInfo();
			}
			return pageItemRepeatWithHelper;
		}
	}
}
