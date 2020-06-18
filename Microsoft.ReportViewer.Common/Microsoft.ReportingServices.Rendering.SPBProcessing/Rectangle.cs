using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class Rectangle : PageItemContainer
	{
		private bool m_staticItem;

		protected override PageBreak PageBreak => ((Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)m_source).PageBreak;

		protected override string PageName => ((RectangleInstance)m_source.Instance).PageName;

		internal override bool StaticItem => m_staticItem;

		internal Rectangle(Microsoft.ReportingServices.OnDemandReportRendering.Rectangle source, PageContext pageContext, bool createForRepeat)
			: base(source, createForRepeat)
		{
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					m_itemPageSizes = pageContext.GetSharedFromRepeatItemSizesElement(source, isPadded: true);
				}
				else
				{
					m_itemPageSizes = pageContext.GetSharedItemSizesElement(source, isPadded: true);
				}
			}
			else
			{
				m_itemPageSizes = new PaddItemSizes(source);
			}
		}

		private void CalculateHiddenItemRenderSize(PageContext pageContext, bool createForRepeat)
		{
			if (m_itemRenderSizes != null)
			{
				return;
			}
			if (pageContext != null)
			{
				if (createForRepeat)
				{
					m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(m_itemPageSizes, isPadded: true, returnPaddings: false);
				}
				else
				{
					m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(m_itemPageSizes, isPadded: true, returnPaddings: false);
				}
			}
			if (m_itemRenderSizes == null)
			{
				m_itemRenderSizes = new ItemSizes(m_itemPageSizes);
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
			if (!m_itemsCreated && ResolveItemHiddenState(rplWriter, interactivity, pageContext, createForRepeat: false, ref contentSize))
			{
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
				if (rplWriter != null)
				{
					CalculateHiddenItemRenderSize(pageContext, createForRepeat: false);
				}
				return true;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			PageItemHelper[] array = null;
			bool flag = false;
			WriteStartItemToStream(rplWriter, pageContext);
			bool overrideChild = !pageContext.IsPageBreakRegistered;
			if (m_itemsCreated)
			{
				PageItemContainerHelper pageItemContainerHelper = lastPageInfo as PageItemContainerHelper;
				RSTrace.RenderingTracer.Assert(pageItemContainerHelper != null || lastPageInfo == null, "This should be a container");
				m_staticItem = CreateChildrenFromPaginationState(rectangle.ReportItemCollection, pageContext, pageItemContainerHelper, rectangle.IsSimple);
				ResolveRepeatWithFromPaginationState(pageItemContainerHelper, rplWriter, pageContext);
				UpdateItemPageState(pageContext, rectangle.OmitBorderOnPageBreak);
				if (pageItemContainerHelper != null)
				{
					array = pageItemContainerHelper.Children;
				}
			}
			else
			{
				flag = true;
				if (!pageContext.IgnorePageBreaks)
				{
					pageContext.RegisterPageName(PageName);
				}
				m_staticItem = CreateChildren(rectangle.ReportItemCollection, pageContext, rectangle.Width.ToMillimeters(), rectangle.Height.ToMillimeters(), rectangle.IsSimple);
				ResolveRepeatWith(rectangle.ReportItemCollection, pageContext);
				m_itemsCreated = true;
				contentSize?.SetPaddings(m_itemPageSizes.PaddingRight, m_itemPageSizes.PaddingBottom);
			}
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (flag && rectangle.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
					if (pageContext.TracingEnabled && parentTopInPage + m_itemPageSizes.Height >= pageContext2.OriginalPageHeight)
					{
						TracePageGrownOnKeepTogetherItem(pageContext.PageNumber);
					}
				}
			}
			double num = parentTopInPage + m_itemPageSizes.Top;
			double pageItemHeight = 0.0;
			int num2 = 0;
			PageItem[] childrenOnPage = null;
			bool flag2 = true;
			bool flag3 = true;
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
					num3 = pageItem.ReserveSpaceForRepeatWith(m_repeatWithItems, pageContext2);
					if (array != null)
					{
						pageItem.CalculatePage(rplWriter, array[i], pageContext2, m_children, m_repeatWithItems, num + num3, ref pageItemHeight, interactivity);
					}
					else
					{
						pageItem.CalculatePage(rplWriter, null, pageContext2, m_children, m_repeatWithItems, num + num3, ref pageItemHeight, interactivity);
					}
					if (!pageContext2.FullOnPage)
					{
						processPageBreaks.ProcessItemPageBreaks(pageItem);
						if (pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
						{
							if (pageItem.ItemState != State.OnPagePBEnd)
							{
								flag2 = false;
							}
							if (pageItem.ItemState != State.Below)
							{
								flag3 = false;
							}
						}
						else
						{
							m_prevPageEnd = pageItemHeight;
							flag3 = false;
						}
						if (rplWriter != null)
						{
							pageItem.MergeRepeatSiblings(ref repeatedSiblings);
						}
					}
					else if (!pageContext.FullOnPage && rplWriter != null)
					{
						pageItem.MergeRepeatSiblings(ref repeatedSiblings);
					}
					num4 = Math.Max(num4, pageItem.ItemPageSizes.Bottom + m_itemPageSizes.PaddingBottom);
				}
				if (contentSize != null)
				{
					ConsumeWhitespaceVertical(contentSize, num4, pageContext2);
				}
				else
				{
					ConsumeWhitespaceVertical(m_itemPageSizes, num4, pageContext2);
				}
			}
			if (pageContext2.CancelPage)
			{
				m_itemState = State.Below;
				m_children = null;
				m_rplElement = null;
				return false;
			}
			bool flag4 = false;
			if (processPageBreaks != null && processPageBreaks.HasPageBreaks(ref m_prevPageEnd, ref pageItemHeight))
			{
				if (flag2)
				{
					if (pageItemHeight - m_itemPageSizes.Height != 0.0)
					{
						flag2 = false;
					}
					else
					{
						flag4 = true;
					}
				}
			}
			else if (!pageContext2.FullOnPage)
			{
				if (flag2)
				{
					double num5 = num + m_itemPageSizes.Height;
					if ((RoundedDouble)num5 > pageContext2.PageHeight && (RoundedDouble)(num5 - m_itemPageSizes.PaddingBottom) <= pageContext2.PageHeight)
					{
						double val = pageContext2.PageHeight - num;
						m_prevPageEnd = Math.Max(pageItemHeight, val);
						pageItemHeight = m_prevPageEnd;
						flag2 = false;
					}
					else
					{
						pageItemHeight = m_itemPageSizes.Height;
					}
				}
				else if (flag3 && (RoundedDouble)(num + m_itemPageSizes.Height) > pageContext2.PageHeight)
				{
					m_prevPageEnd = pageContext2.PageHeight - num;
					pageItemHeight = m_prevPageEnd;
				}
			}
			else
			{
				pageItemHeight = m_itemPageSizes.Height;
			}
			if (pageContext2.FullOnPage || flag2)
			{
				m_itemState = State.OnPage;
				if (flag2)
				{
					if (!pageContext2.IgnorePageBreaks && base.PageBreakAtEnd)
					{
						pageContext.RegisterPageBreak(new PageBreakInfo(PageBreak, base.ItemName), overrideChild);
						m_itemState = State.OnPagePBEnd;
					}
					else if (flag4)
					{
						m_itemState = State.OnPagePBEnd;
					}
					if (pageContext2.TracingEnabled && pageContext2.IgnorePageBreaks)
					{
						TracePageBreakAtEndIgnored(pageContext2);
					}
				}
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Top + pageItemHeight);
				if (rplWriter != null)
				{
					CreateItemRenderSizes(contentSize, pageContext2, createForRepeat: false);
					num2 = CalculateRenderSizes(rplWriter, pageContext2, interactivity, repeatedSiblings, out childrenOnPage);
					WriteEndItemToStream(rplWriter, num2, childrenOnPage);
				}
				m_indexesLeftToRight = null;
				m_children = null;
			}
			else
			{
				m_itemState = State.SpanPages;
				if (rectangle.OmitBorderOnPageBreak)
				{
					m_rplItemState |= 2;
				}
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Top + pageItemHeight);
				if (rplWriter != null)
				{
					CreateItemRenderSizes(null, pageContext2, createForRepeat: false);
					m_itemRenderSizes.PaddingBottom = 0.0;
					m_itemRenderSizes.AdjustHeightTo(pageItemHeight);
					num2 = CalculateRenderSizes(rplWriter, pageContext2, interactivity, repeatedSiblings, out childrenOnPage);
					WriteEndItemToStream(rplWriter, num2, childrenOnPage);
				}
				else
				{
					ReleaseChildrenOnPage();
				}
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			if (ResolveItemHiddenState(rplWriter, null, pageContext, createForRepeat: true, ref contentSize))
			{
				CalculateHiddenItemRenderSize(pageContext, createForRepeat: true);
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			m_staticItem = CreateChildren(rectangle.ReportItemCollection, pageContext, rectangle.Width.ToMillimeters(), rectangle.Height.ToMillimeters(), rectangle.IsSimple);
			m_itemsCreated = true;
			contentSize?.SetPaddings(m_itemPageSizes.PaddingRight, m_itemPageSizes.PaddingBottom);
			if (m_children != null)
			{
				double num = m_itemPageSizes.PaddingBottom;
				PageItem pageItem = null;
				for (int i = 0; i < m_children.Length; i++)
				{
					pageItem = m_children[i];
					if (pageItem != null)
					{
						pageItem.CalculateRepeatWithPage(rplWriter, pageContext, m_children);
						num = Math.Max(num, pageItem.ItemPageSizes.Bottom + m_itemPageSizes.PaddingBottom);
					}
				}
				if (contentSize != null)
				{
					ConsumeWhitespaceVertical(contentSize, num, pageContext);
				}
				else
				{
					ConsumeWhitespaceVertical(m_itemPageSizes, num, pageContext);
				}
			}
			m_itemState = State.OnPage;
			CreateItemRenderSizes(contentSize, pageContext, createForRepeat: true);
			CalculateRepeatWithRenderSizes(pageContext);
		}

		internal override int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			if (base.ItemState == State.OnPageHidden)
			{
				return 0;
			}
			WriteStartItemToStream(rplWriter, pageContext);
			int num = 0;
			if (m_children != null)
			{
				for (int i = 0; i < m_children.Length; i++)
				{
					if (m_children[i] != null)
					{
						num += m_children[i].WriteRepeatWithToPage(rplWriter, pageContext);
					}
				}
			}
			WriteRepeatWithEndItemToStream(rplWriter, num);
			return 1;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					m_offset = baseStream.Position;
					binaryWriter.Write((byte)10);
					WriteElementProps(binaryWriter, rplWriter, pageContext, m_offset + 1);
				}
				else
				{
					m_rplElement = new RPLRectangle();
					WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = reportItemCollection[rectangle.LinkToChild];
				spbifWriter.Write((byte)43);
				spbifWriter.Write(reportItem.ID);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = reportItemCollection[rectangle.LinkToChild];
				((RPLRectanglePropsDef)sharedProps).LinkToChildId = reportItem.ID;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)6);
				WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemContainerHelper(6);
			WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}
	}
}
