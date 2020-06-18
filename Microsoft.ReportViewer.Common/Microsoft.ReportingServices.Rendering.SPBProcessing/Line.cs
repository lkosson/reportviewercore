using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Line : PageItem
	{
		internal Line(Microsoft.ReportingServices.OnDemandReportRendering.Line source, PageContext pageContext, bool createForRepeat)
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
			ItemSizes contentSize = null;
			bool flag = ResolveItemHiddenState(rplWriter, interactivity, pageContext, createForRepeat: false, ref contentSize);
			parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
			if (rplWriter != null)
			{
				if (m_itemRenderSizes == null)
				{
					CreateItemRenderSizes(contentSize, pageContext, createForRepeat: false);
				}
				if (!flag)
				{
					WriteItemToStream(rplWriter, pageContext);
				}
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			ResolveItemHiddenState(rplWriter, null, pageContext, createForRepeat: true, ref contentSize);
			if (m_itemRenderSizes == null)
			{
				CreateItemRenderSizes(contentSize, pageContext, createForRepeat: true);
			}
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			WriteItemToStream(rplWriter, pageContext);
			return 1;
		}

		internal void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				long position = baseStream.Position;
				binaryWriter.Write((byte)8);
				WriteElementProps(binaryWriter, rplWriter, pageContext, position + 1);
				m_offset = baseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(position);
				binaryWriter.Write(byte.MaxValue);
			}
			else
			{
				m_rplElement = new RPLLine();
				WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			if (((Microsoft.ReportingServices.OnDemandReportRendering.Line)m_source).Slant)
			{
				spbifWriter.Write((byte)24);
				spbifWriter.Write(value: true);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			((RPLLinePropsDef)sharedProps).Slant = ((Microsoft.ReportingServices.OnDemandReportRendering.Line)m_source).Slant;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)8);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(8);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
