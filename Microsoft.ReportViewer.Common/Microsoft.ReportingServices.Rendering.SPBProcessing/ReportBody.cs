using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ReportBody : PageItemContainer
	{
		private new Body m_source;

		internal override string SourceUniqueName => m_source.InstanceUniqueName;

		internal override string SourceID => m_source.ID;

		internal override ReportElement OriginalSource => m_source;

		internal ReportBody(Body source, ReportSize width, PageContext pageContext)
			: base(null, createForRepeat: false)
		{
			if (pageContext != null)
			{
				m_itemPageSizes = pageContext.GetSharedItemSizesElement(width, source.Height, source.ID, isPadded: true);
			}
			else
			{
				m_itemPageSizes = new PaddItemSizes(width, source.Height, source.ID);
			}
			m_source = source;
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			WriteStartItemToStream(rplWriter, pageContext);
			PageItemHelper[] array = null;
			if (m_itemsCreated)
			{
				PageItemContainerHelper pageItemContainerHelper = lastPageInfo as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null || lastPageInfo == null, "This should be a container");
				CreateChildrenFromPaginationState(m_source.ReportItemCollection, pageContext, pageItemContainerHelper, isSimple: false);
				ResolveRepeatWithFromPaginationState(pageItemContainerHelper, rplWriter, pageContext);
				UpdateItemPageState(pageContext, omitBorderOnPageBreak: false);
				if (pageItemContainerHelper != null)
				{
					array = pageItemContainerHelper.Children;
				}
			}
			else
			{
				CreateChildren(m_source.ReportItemCollection, pageContext, m_itemPageSizes.Width, m_itemPageSizes.Height);
				ResolveRepeatWith(m_source.ReportItemCollection, pageContext);
				m_itemsCreated = true;
			}
			double num = parentTopInPage + m_itemPageSizes.Top;
			double pageItemHeight = 0.0;
			int num2 = 0;
			PageItem[] childrenOnPage = null;
			bool flag = true;
			bool flag2 = true;
			ProcessPageBreaks processPageBreaks = null;
			List<int> repeatedSiblings = null;
			double num3 = 0.0;
			if (m_children != null)
			{
				double num4 = m_itemPageSizes.PaddingBottom;
				PageItem pageItem = null;
				processPageBreaks = new ProcessPageBreaks();
				for (int i = 0; i < m_children.Length; i++)
				{
					pageItem = m_children[i];
					if (pageItem == null)
					{
						continue;
					}
					num3 = pageItem.ReserveSpaceForRepeatWith(m_repeatWithItems, pageContext);
					if (array != null)
					{
						pageItem.CalculatePage(rplWriter, array[i], pageContext, m_children, m_repeatWithItems, num + num3, ref pageItemHeight, interactivity);
					}
					else
					{
						pageItem.CalculatePage(rplWriter, null, pageContext, m_children, m_repeatWithItems, num + num3, ref pageItemHeight, interactivity);
					}
					if (!pageContext.FullOnPage)
					{
						processPageBreaks.ProcessItemPageBreaks(pageItem);
						if (pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
						{
							if (pageItem.ItemState != State.OnPagePBEnd)
							{
								flag = false;
							}
							if (pageItem.ItemState != State.Below)
							{
								flag2 = false;
							}
						}
						else
						{
							m_prevPageEnd = pageItemHeight;
							flag2 = false;
						}
						if (rplWriter != null)
						{
							pageItem.MergeRepeatSiblings(ref repeatedSiblings);
						}
					}
					num4 = Math.Max(num4, pageItem.ItemPageSizes.Bottom + m_itemPageSizes.PaddingBottom);
				}
				ConsumeWhitespaceVertical(m_itemPageSizes, num4, pageContext);
			}
			if (pageContext.CancelPage)
			{
				m_itemState = State.Below;
				m_children = null;
				m_rplElement = null;
				return false;
			}
			bool flag3 = false;
			if (processPageBreaks != null && processPageBreaks.HasPageBreaks(ref m_prevPageEnd, ref pageItemHeight))
			{
				if (flag)
				{
					if (pageItemHeight - m_itemPageSizes.Height != 0.0)
					{
						flag = false;
					}
					else
					{
						flag3 = true;
					}
				}
			}
			else if (!pageContext.FullOnPage)
			{
				if (flag)
				{
					double num5 = num + m_itemPageSizes.Height;
					if ((RoundedDouble)num5 > pageContext.PageHeight && (RoundedDouble)(num5 - m_itemPageSizes.PaddingBottom) <= pageContext.PageHeight)
					{
						double val = pageContext.PageHeight - num;
						m_prevPageEnd = Math.Max(pageItemHeight, val);
						pageItemHeight = m_prevPageEnd;
						flag = false;
					}
					else
					{
						pageItemHeight = m_itemPageSizes.Height;
					}
				}
				else if (flag2 && (RoundedDouble)(num + m_itemPageSizes.Height) > pageContext.PageHeight)
				{
					m_prevPageEnd = pageContext.PageHeight - num;
					pageItemHeight = m_prevPageEnd;
				}
			}
			else
			{
				pageItemHeight = m_itemPageSizes.Height;
			}
			if (pageContext.FullOnPage || flag)
			{
				m_itemState = State.OnPage;
				if (flag && flag3)
				{
					m_itemState = State.OnPagePBEnd;
				}
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Top + pageItemHeight);
				if (rplWriter != null)
				{
					CreateItemRenderSizes(null, pageContext, createForRepeat: false);
					num2 = CalculateRenderSizes(rplWriter, pageContext, interactivity, repeatedSiblings, out childrenOnPage);
					WriteEndItemToStream(rplWriter, num2, childrenOnPage);
				}
				m_indexesLeftToRight = null;
				m_children = null;
			}
			else
			{
				m_itemState = State.SpanPages;
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Top + pageItemHeight);
				if (rplWriter != null)
				{
					CreateItemRenderSizes(null, pageContext, createForRepeat: false);
					m_itemRenderSizes.PaddingBottom = 0.0;
					m_itemRenderSizes.AdjustHeightTo(pageItemHeight);
					num2 = CalculateRenderSizes(rplWriter, pageContext, interactivity, repeatedSiblings, out childrenOnPage);
					WriteEndItemToStream(rplWriter, num2, childrenOnPage);
				}
				else
				{
					ReleaseChildrenOnPage();
				}
			}
			return true;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				Stream baseStream = binaryWriter.BaseStream;
				m_offset = baseStream.Position;
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					binaryWriter.Write((byte)10);
				}
				else
				{
					binaryWriter.Write((byte)6);
				}
				binaryWriter.Write((byte)15);
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)1);
				binaryWriter.Write(SourceID);
				Style style = m_source.Style;
				if (style != null)
				{
					WriteSharedStyle(binaryWriter, style, pageContext, 6);
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Write((byte)1);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(SourceUniqueName);
					StyleInstance styleInstance = GetStyleInstance(m_source, null);
					if (styleInstance != null)
					{
						WriteNonSharedStyle(binaryWriter, style, styleInstance, pageContext, 6, null);
					}
				}
				else
				{
					binaryWriter.Write(byte.MaxValue);
					binaryWriter.Write((byte)1);
					binaryWriter.Write((byte)0);
					binaryWriter.Write(SourceUniqueName);
				}
				binaryWriter.Write(byte.MaxValue);
				binaryWriter.Write(byte.MaxValue);
				return;
			}
			m_rplElement = new RPLBody();
			m_rplElement.ElementProps.Definition.ID = SourceID;
			m_rplElement.ElementProps.UniqueName = SourceUniqueName;
			Style style2 = m_source.Style;
			if (style2 != null)
			{
				m_rplElement.ElementProps.Definition.SharedStyle = WriteSharedStyle(style2, pageContext);
				StyleInstance styleInstance2 = GetStyleInstance(m_source, null);
				if (styleInstance2 != null)
				{
					m_rplElement.ElementProps.NonSharedStyle = WriteNonSharedStyle(style2, styleInstance2, pageContext, null);
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(spbifWriter, style, writeShared: true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(rplStyleProps, style, writeShared: true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(spbifWriter, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(rplStyleProps, styleDef, writeShared: false, pageContext);
				break;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)7);
				WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(7);
			WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
