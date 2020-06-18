using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class SubReport : PageItem
	{
		private PageItem[] m_childrenBody;

		private double m_prevPageEnd;

		private int m_bodyIndex = -1;

		private int m_bodiesOnPage;

		private long m_bodyOffset;

		internal SubReport(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext, bool createForRepeat)
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

		internal void UpdateItemPageState(bool omitBordersOnPageBreaks)
		{
			if (m_itemState == State.SpanPages)
			{
				if (omitBordersOnPageBreaks)
				{
					m_rplItemState |= 1;
				}
				m_itemState = State.OnPage;
			}
			m_itemPageSizes.AdjustHeightTo(base.ItemPageSizes.Height - m_prevPageEnd);
			m_prevPageEnd = 0.0;
		}

		internal override void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				base.UpdateItem(itemHelper);
				m_prevPageEnd = itemHelper.PrevPageEnd;
			}
		}

		internal override bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentPageHeight, Interactivity interactivity)
		{
			AdjustOriginFromItemsAbove(siblings, repeatWithItems);
			if (!HitsCurrentPage(pageContext, parentTopInPage))
			{
				return false;
			}
			bool firstPage = false;
			bool needsFirstBodyUpdate = false;
			CalculateIndexOfFirstBodyOnPage(lastPageInfo, out firstPage, out needsFirstBodyUpdate);
			ItemSizes contentSize = null;
			if (firstPage && ResolveItemHiddenState(rplWriter, interactivity, pageContext, createForRepeat: false, ref contentSize))
			{
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
				if (rplWriter != null && m_itemRenderSizes == null)
				{
					CreateItemRenderSizes(null, pageContext, createForRepeat: false);
				}
				return true;
			}
			WriteStartItemToStream(rplWriter, pageContext);
			CreateChildren(pageContext, lastPageInfo, needsFirstBodyUpdate);
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			PageContext pageContext2 = pageContext;
			if (!pageContext2.FullOnPage)
			{
				if (base.IgnorePageBreaks)
				{
					pageContext2 = new PageContext(pageContext, PageContext.PageContextFlags.FullOnPage, PageContext.IgnorePBReasonFlag.Toggled);
				}
				else if (firstPage && subReport.KeepTogether && !pageContext2.KeepTogether)
				{
					pageContext2 = new PageContext(pageContext);
					pageContext2.KeepTogether = true;
				}
			}
			m_bodiesOnPage = 0;
			double num = parentTopInPage + m_itemPageSizes.Top;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			bool flag = false;
			for (int i = m_bodyIndex; i < m_childrenBody.Length; i++)
			{
				if (flag)
				{
					break;
				}
				PageItem pageItem = m_childrenBody[i];
				double parentHeight = 0.0;
				if (needsFirstBodyUpdate)
				{
					pageItem.CalculatePage(rplWriter, lastPageInfo.ChildPage, pageContext2, null, null, num, ref parentHeight, interactivity);
					needsFirstBodyUpdate = false;
				}
				else
				{
					pageItem.CalculatePage(rplWriter, null, pageContext2, null, null, num, ref parentHeight, interactivity);
				}
				if (pageContext2.CancelPage)
				{
					m_itemState = State.Below;
					m_rplElement = null;
					m_childrenBody = null;
					return false;
				}
				if (pageItem.ItemState == State.TopNextPage && i == 0)
				{
					m_itemState = State.TopNextPage;
					m_bodyIndex = -1;
					return false;
				}
				m_bodiesOnPage++;
				num += parentHeight;
				num2 += parentHeight;
				m_itemState = State.OnPage;
				if (!pageContext2.FullOnPage)
				{
					m_prevPageEnd = num2;
					if (pageItem.ItemState != State.OnPage)
					{
						if (pageItem.ItemState == State.OnPagePBEnd && i == m_childrenBody.Length - 1)
						{
							m_itemState = State.OnPagePBEnd;
						}
						else
						{
							if (pageItem.ItemState == State.Below)
							{
								m_bodiesOnPage--;
							}
							m_itemState = State.SpanPages;
							m_prevPageEnd = 0.0;
						}
					}
					if (m_itemState == State.SpanPages || m_itemState == State.OnPagePBEnd)
					{
						flag = true;
					}
					else if ((RoundedDouble)num >= pageContext2.PageHeight)
					{
						flag = true;
						if (m_itemState == State.OnPage && i < m_childrenBody.Length - 1)
						{
							m_itemState = State.SpanPages;
						}
					}
				}
				if (rplWriter != null)
				{
					num3 = Math.Max(num3, pageItem.ItemRenderSizes.Width);
					num4 += pageItem.ItemRenderSizes.Height;
				}
			}
			if (contentSize == null)
			{
				m_itemPageSizes.AdjustHeightTo(m_childrenBody[m_bodyIndex + m_bodiesOnPage - 1].ItemPageSizes.Bottom);
			}
			else
			{
				contentSize.AdjustHeightTo(m_childrenBody[m_bodyIndex + m_bodiesOnPage - 1].ItemPageSizes.Bottom);
			}
			if (rplWriter != null)
			{
				CreateItemRenderSizes(contentSize, pageContext2, createForRepeat: false);
				m_itemRenderSizes.AdjustWidthTo(num3);
				m_itemRenderSizes.AdjustHeightTo(num4);
				WriteEndItemToStream(rplWriter, pageContext2);
			}
			if (m_itemState != State.SpanPages)
			{
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Bottom);
			}
			else
			{
				if (subReport.OmitBorderOnPageBreak)
				{
					m_rplItemState |= 2;
				}
				parentPageHeight = Math.Max(parentPageHeight, m_itemPageSizes.Top + num2);
			}
			ReleaseBodyChildrenOnPage();
			m_bodyIndex += m_bodiesOnPage - 1;
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
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					WriteStartItemToRPLStream2008(rplWriter, binaryWriter, pageContext);
				}
				else
				{
					WriteStartItemToRPLStream(rplWriter, binaryWriter, pageContext);
				}
			}
			else
			{
				WriteStartItemToRPLOM(rplWriter, pageContext);
			}
		}

		internal void WriteEndItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					WriteEndItemToRPLStream2008(binaryWriter);
				}
				else
				{
					WriteEndItemToRPLStream(binaryWriter);
				}
			}
			else
			{
				WriteEndItemToRPLOM(rplWriter);
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			if (subReport.ReportName != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(subReport.ReportName);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			if (subReport.ReportName != null)
			{
				((RPLSubReportPropsDef)sharedProps).ReportName = subReport.ReportName;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = ((Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source).Report;
			if (report != null && report.Language != null)
			{
				string text = null;
				text = ((!report.Language.IsExpression) ? report.Language.Value : report.Instance.Language);
				if (text != null)
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(text);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = ((Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source).Report;
			if (report != null && report.Language != null)
			{
				string text = null;
				text = ((!report.Language.IsExpression) ? report.Language.Value : report.Instance.Language);
				((RPLSubReportProps)nonSharedProps).Language = text;
			}
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)4);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write((byte)23);
				reportPageInfo.Write(m_bodyIndex);
				reportPageInfo.Write((byte)11);
				reportPageInfo.Write(m_prevPageEnd);
				if (m_childrenBody != null && m_childrenBody[m_bodyIndex] != null)
				{
					reportPageInfo.Write((byte)19);
					m_childrenBody[m_bodyIndex].WritePaginationInfo(reportPageInfo);
				}
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(4);
			base.WritePaginationInfoProperties(pageItemHelper);
			pageItemHelper.BodyIndex = m_bodyIndex;
			pageItemHelper.PrevPageEnd = m_prevPageEnd;
			if (m_childrenBody != null && m_childrenBody[m_bodyIndex] != null)
			{
				pageItemHelper.ChildPage = m_childrenBody[m_bodyIndex].WritePaginationInfo();
			}
			return pageItemHelper;
		}

		private void CalculateIndexOfFirstBodyOnPage(PageItemHelper lastPageInfo, out bool firstPage, out bool needsFirstBodyUpdate)
		{
			firstPage = false;
			needsFirstBodyUpdate = false;
			if (lastPageInfo != null)
			{
				m_bodyIndex = lastPageInfo.BodyIndex;
				if (lastPageInfo.ChildPage != null)
				{
					needsFirstBodyUpdate = true;
					return;
				}
				if (m_bodyIndex < 0)
				{
					firstPage = true;
				}
				m_bodyIndex++;
			}
			else
			{
				m_bodyIndex = 0;
				firstPage = true;
			}
		}

		private void CreateChildren(PageContext pageContext, PageItemHelper lastPageInfo, bool needsFirstBodyUpdate)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.SubReport subReport = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)m_source;
			ReportSectionCollection reportSections = subReport.Report.ReportSections;
			if (m_childrenBody == null)
			{
				m_childrenBody = new PageItem[reportSections.Count];
				for (int i = m_bodyIndex; i < reportSections.Count; i++)
				{
					m_childrenBody[i] = new ReportBody(subReport.Report.ReportSections[i].Body, subReport.Report.ReportSections[i].Width, pageContext);
				}
			}
			if (needsFirstBodyUpdate)
			{
				m_childrenBody[m_bodyIndex].UpdateItem(lastPageInfo.ChildPage);
				UpdateItemPageState(subReport.OmitBorderOnPageBreak);
			}
		}

		private void ReleaseBodyChildrenOnPage()
		{
			for (int i = 0; i < m_bodiesOnPage; i++)
			{
				PageItem pageItem = m_childrenBody[i + m_bodyIndex];
				if (pageItem != null)
				{
					if (i < m_bodiesOnPage - 1)
					{
						pageItem = null;
					}
					else if (pageItem.ItemState == State.OnPage || pageItem.ItemState == State.OnPagePBEnd)
					{
						pageItem = null;
					}
				}
			}
		}

		private void WriteStartItemToRPLStream(RPLWriter rplWriter, BinaryWriter spbifWriter, PageContext pageContext)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)12);
			WriteElementProps(spbifWriter, rplWriter, pageContext, m_offset + 1);
		}

		private void WriteStartItemToRPLOM(RPLWriter rplWriter, PageContext pageContext)
		{
			m_rplElement = new RPLSubReport();
			WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
		}

		private void WriteStartItemToRPLStream2008(RPLWriter rplWriter, BinaryWriter spbifWriter, PageContext pageContext)
		{
			Stream baseStream = spbifWriter.BaseStream;
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)12);
			WriteElementProps(spbifWriter, rplWriter, pageContext, m_offset + 1);
			m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)6);
			spbifWriter.Write((byte)15);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(m_source.ID + "_SBID");
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(byte.MaxValue);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndItemToRPLStream(BinaryWriter spbifWriter)
		{
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(m_bodiesOnPage);
			for (int i = 0; i < m_bodiesOnPage; i++)
			{
				m_childrenBody[i + m_bodyIndex].WritePageItemRenderSizes(spbifWriter);
			}
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteEndItemToRPLOM(RPLWriter rplWriter)
		{
			RPLItemMeasurement[] array = new RPLItemMeasurement[m_bodiesOnPage];
			((RPLContainer)m_rplElement).Children = array;
			for (int i = 0; i < m_bodiesOnPage; i++)
			{
				array[i] = m_childrenBody[i + m_bodyIndex].WritePageItemRenderSizes();
			}
		}

		private void WriteEndItemToRPLStream2008(BinaryWriter spbifWriter)
		{
			double num = 0.0;
			double num2 = 0.0;
			Stream baseStream = spbifWriter.BaseStream;
			long position = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_bodyOffset);
			spbifWriter.Write(m_bodiesOnPage);
			for (int i = 0; i < m_bodiesOnPage; i++)
			{
				spbifWriter.Write((float)m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Left);
				spbifWriter.Write((float)(m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Top + num2));
				spbifWriter.Write((float)m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Width);
				spbifWriter.Write((float)m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Height);
				spbifWriter.Write(0);
				spbifWriter.Write((byte)0);
				spbifWriter.Write(m_childrenBody[i + m_bodyIndex].Offset);
				num = Math.Max(num, m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Width);
				num2 += m_childrenBody[i + m_bodyIndex].ItemRenderSizes.Height;
			}
			m_bodyOffset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position);
			spbifWriter.Write(byte.MaxValue);
			long position2 = baseStream.Position;
			spbifWriter.Write((byte)16);
			spbifWriter.Write(m_offset);
			spbifWriter.Write(1);
			spbifWriter.Write(0f);
			spbifWriter.Write(0f);
			spbifWriter.Write((float)num);
			spbifWriter.Write((float)num2);
			spbifWriter.Write(0);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(m_bodyOffset);
			m_offset = baseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(position2);
			spbifWriter.Write(byte.MaxValue);
		}
	}
}
