using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class PageHeadFoot : PageItemContainer
	{
		private new PageSection m_source;

		internal override string SourceUniqueName => m_source.InstanceUniqueName;

		internal override string SourceID => m_source.ID;

		internal override ReportElement OriginalSource => m_source;

		internal PageHeadFoot(PageSection source, ReportSize width, PageContext pageContext)
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

		internal void CalculateItem(RPLWriter rplWriter, PageContext pageContext, bool isHeader, Interactivity interactivity, bool native)
		{
			WriteStartItemToStream(rplWriter, isHeader, pageContext, native);
			CreateChildren(m_source.ReportItemCollection, pageContext, m_itemPageSizes.Width, m_itemPageSizes.Height);
			double parentHeight = 0.0;
			if (m_children != null)
			{
				double num = 0.0;
				PageItem pageItem = null;
				for (int i = 0; i < m_children.Length; i++)
				{
					pageItem = m_children[i];
					if (pageItem != null)
					{
						pageItem.CalculatePage(rplWriter, null, pageContext, m_children, null, 0.0, ref parentHeight, interactivity);
						num = Math.Max(num, pageItem.ItemPageSizes.Bottom + m_itemPageSizes.PaddingBottom);
					}
				}
				ConsumeWhitespaceVertical(m_itemPageSizes, num, pageContext);
			}
			if (pageContext.CancelPage)
			{
				m_itemState = State.Below;
				m_children = null;
				m_rplElement = null;
				return;
			}
			if (rplWriter != null)
			{
				CreateItemRenderSizes(null, pageContext, createForRepeat: false);
				PageItem[] childrenOnPage = null;
				int itemsOnPage = CalculateRenderSizes(rplWriter, pageContext, interactivity, null, out childrenOnPage);
				WriteEndItemToStream(rplWriter, itemsOnPage, childrenOnPage);
			}
			m_indexesLeftToRight = null;
			m_children = null;
		}

		internal void WriteStartItemToStream(RPLWriter rplWriter, bool isHeader, PageContext pageContext, bool native)
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
				bool flag = false;
				if (pageContext.VersionPicker == RPLVersionEnum.RPL2008 || pageContext.VersionPicker == RPLVersionEnum.RPL2008WithImageConsolidation)
				{
					if (native)
					{
						if (isHeader)
						{
							binaryWriter.Write((byte)4);
						}
						else
						{
							binaryWriter.Write((byte)5);
						}
					}
					else
					{
						binaryWriter.Write((byte)10);
						flag = true;
					}
				}
				else if (isHeader)
				{
					binaryWriter.Write((byte)4);
				}
				else
				{
					binaryWriter.Write((byte)5);
				}
				binaryWriter.Write((byte)15);
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)1);
				binaryWriter.Write(SourceID);
				if (!flag)
				{
					binaryWriter.Write((byte)44);
					binaryWriter.Write(m_source.PrintOnFirstPage);
					if (pageContext.VersionPicker != 0 && pageContext.VersionPicker != RPLVersionEnum.RPL2008WithImageConsolidation)
					{
						binaryWriter.Write((byte)47);
						binaryWriter.Write(m_source.PrintBetweenSections);
					}
				}
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
			m_rplElement = new RPLHeaderFooter();
			m_rplElement.ElementProps.Definition.ID = SourceID;
			RPLHeaderFooterPropsDef obj = m_rplElement.ElementProps.Definition as RPLHeaderFooterPropsDef;
			obj.PrintOnFirstPage = m_source.PrintOnFirstPage;
			obj.PrintBetweenSections = m_source.PrintBetweenSections;
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

		internal override void WriteEndItemToStream(RPLWriter rplWriter, int itemsOnPage, PageItem[] childrenOnPage)
		{
			if (rplWriter == null)
			{
				return;
			}
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			int num = (childrenOnPage != null) ? childrenOnPage.Length : 0;
			long value = 0L;
			RPLItemMeasurement[] array = null;
			if (binaryWriter != null)
			{
				value = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)16);
				binaryWriter.Write(m_offset);
				binaryWriter.Write(itemsOnPage);
			}
			else if (itemsOnPage > 0)
			{
				array = new RPLItemMeasurement[itemsOnPage];
				((RPLContainer)m_rplElement).Children = array;
			}
			PageItem pageItem = null;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				pageItem = childrenOnPage[i];
				if (pageItem == null || pageItem.ItemState == State.Below || pageItem.ItemState == State.TopNextPage)
				{
					continue;
				}
				if (pageItem.ItemState != State.OnPageHidden && !(pageItem is NoRowsItem))
				{
					if (binaryWriter != null)
					{
						pageItem.WritePageItemRenderSizes(binaryWriter);
					}
					else
					{
						array[num2] = pageItem.WritePageItemRenderSizes();
						num2++;
					}
				}
				childrenOnPage[i] = null;
				m_children[i] = null;
			}
			if (binaryWriter != null)
			{
				m_offset = binaryWriter.BaseStream.Position;
				binaryWriter.Write((byte)254);
				binaryWriter.Write(value);
				binaryWriter.Write(byte.MaxValue);
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
	}
}
