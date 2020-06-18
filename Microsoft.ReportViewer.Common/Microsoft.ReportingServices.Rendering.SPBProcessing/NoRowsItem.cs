using Microsoft.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class NoRowsItem : PageItem
	{
		internal NoRowsItem(ReportItem source, PageContext pageContext, bool createForRepeat)
			: base(source)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, isPadded: false);
				}
				else
				{
					m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, isPadded: false);
				}
			}
			else
			{
				m_itemPageSizes = new ItemSizes(source);
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			m_itemPageSizes.AdjustHeightTo(0.0);
			m_itemPageSizes.AdjustWidthTo(0.0);
			m_itemState = State.OnPage;
			interactivity?.RegisterItem(this, pageContext);
			if (rplWriter != null && m_itemRenderSizes == null)
			{
				CreateItemRenderSizes(null, pageContext, createForRepeat: false);
			}
			return true;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)2);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(2);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
