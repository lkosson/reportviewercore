using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class PageItem : PageElement
	{
		internal enum State : byte
		{
			Unknow,
			OnPage,
			Below,
			TopNextPage,
			SpanPages,
			OnPagePBEnd,
			OnPageHidden
		}

		protected new ReportItem m_source;

		protected ItemSizes m_itemPageSizes;

		protected ItemSizes m_itemRenderSizes;

		protected List<int> m_pageItemsAbove;

		protected List<int> m_pageItemsLeft;

		protected long m_offset;

		protected RPLItem m_rplElement;

		protected State m_itemState;

		protected byte m_rplItemState;

		protected double m_defLeftValue;

		public override long Offset
		{
			get
			{
				return m_offset;
			}
			set
			{
				m_offset = value;
			}
		}

		internal new ReportItem Source => m_source;

		internal override ReportElement OriginalSource => m_source;

		internal override string SourceID => m_source.ID;

		internal override string SourceUniqueName => m_source.InstanceUniqueName;

		internal virtual bool StaticItem => false;

		internal byte RplItemState => m_rplItemState;

		internal bool RepeatedSibling
		{
			get
			{
				if (m_source == null)
				{
					return false;
				}
				return m_source.RepeatedSibling;
			}
		}

		internal ItemSizes ItemPageSizes => m_itemPageSizes;

		internal ItemSizes ItemRenderSizes
		{
			get
			{
				return m_itemRenderSizes;
			}
			set
			{
				m_itemRenderSizes = value;
			}
		}

		internal List<int> PageItemsAbove
		{
			get
			{
				return m_pageItemsAbove;
			}
			set
			{
				m_pageItemsAbove = value;
			}
		}

		internal List<int> PageItemsLeft
		{
			get
			{
				return m_pageItemsLeft;
			}
			set
			{
				m_pageItemsLeft = value;
			}
		}

		public RPLItem RPLElement
		{
			get
			{
				return m_rplElement;
			}
			set
			{
				m_rplElement = value;
			}
		}

		internal State ItemState
		{
			get
			{
				return m_itemState;
			}
			set
			{
				m_itemState = value;
			}
		}

		internal double DefLeftValue
		{
			get
			{
				return m_defLeftValue;
			}
			set
			{
				m_defLeftValue = value;
			}
		}

		protected virtual PageBreak PageBreak => null;

		protected virtual string PageName => null;

		internal string ItemName
		{
			get
			{
				if (m_source != null)
				{
					return m_source.Name;
				}
				return null;
			}
		}

		internal bool PageBreakAtStart
		{
			get
			{
				if (PageBreak == null || PageBreak.Instance.Disabled || IgnorePageBreaks)
				{
					return false;
				}
				PageBreakLocation breakLocation = PageBreak.BreakLocation;
				if (breakLocation != PageBreakLocation.Start)
				{
					return breakLocation == PageBreakLocation.StartAndEnd;
				}
				return true;
			}
		}

		internal bool PageBreakAtEnd
		{
			get
			{
				if (PageBreak == null || PageBreak.Instance.Disabled || IgnorePageBreaks)
				{
					return false;
				}
				PageBreakLocation breakLocation = PageBreak.BreakLocation;
				if (breakLocation != PageBreakLocation.End)
				{
					return breakLocation == PageBreakLocation.StartAndEnd;
				}
				return true;
			}
		}

		internal bool IgnorePageBreaks
		{
			get
			{
				if (m_source.RepeatedSibling)
				{
					return true;
				}
				if (m_source.Visibility == null)
				{
					return false;
				}
				if (m_source.Visibility.ToggleItem != null)
				{
					return true;
				}
				bool flag = false;
				if (m_source.Visibility.Hidden.IsExpression)
				{
					return m_source.Instance.Visibility.StartHidden;
				}
				return m_source.Visibility.Hidden.Value;
			}
		}

		internal virtual double SourceWidthInMM => m_source.Width.ToMillimeters();

		protected PageItem(ReportItem source)
			: base(source)
		{
			m_source = source;
			if (m_source != null)
			{
				m_defLeftValue = m_source.Left.ToMillimeters();
			}
		}

		internal static List<int> GetNewList(List<int> source)
		{
			if (source == null || source.Count == 0)
			{
				return null;
			}
			List<int> list = new List<int>(source.Count);
			for (int i = 0; i < source.Count; i++)
			{
				list.Add(source[i]);
			}
			return list;
		}

		internal static int[] GetNewArray(int[] source)
		{
			if (source == null || source.Length == 0)
			{
				return null;
			}
			int[] array = new int[source.Length];
			Array.Copy(source, array, source.Length);
			return array;
		}

		internal static PageItem Create(ReportItem source, PageContext pageContext, bool tablixCellParent, bool createForRepeat)
		{
			if (source == null)
			{
				return null;
			}
			PageItem result = null;
			if (source is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
			{
				result = new TextBox((Microsoft.ReportingServices.OnDemandReportRendering.TextBox)source, pageContext, createForRepeat);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Line)
			{
				result = new Line((Microsoft.ReportingServices.OnDemandReportRendering.Line)source, pageContext, createForRepeat);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Image)
			{
				result = new Image((Microsoft.ReportingServices.OnDemandReportRendering.Image)source, pageContext, createForRepeat);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Chart)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.Chart source2 = (Microsoft.ReportingServices.OnDemandReportRendering.Chart)source;
				result = ((!TransformToTextBox(source2)) ? ((PageItem)new Chart(source2, pageContext, createForRepeat)) : ((PageItem)new TextBox(source2, pageContext, createForRepeat)));
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel source3 = (Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel)source;
				result = ((!TransformToTextBox(source3)) ? ((PageItem)new GaugePanel(source3, pageContext, createForRepeat)) : ((PageItem)new TextBox(source3, pageContext, createForRepeat)));
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Map)
			{
				result = new Map((Microsoft.ReportingServices.OnDemandReportRendering.Map)source, pageContext, createForRepeat);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
			{
				bool noRows = false;
				Microsoft.ReportingServices.OnDemandReportRendering.Tablix source4 = (Microsoft.ReportingServices.OnDemandReportRendering.Tablix)source;
				result = (TransformToTextBox(source4, tablixCellParent, ref noRows) ? new TextBox(source4, pageContext, createForRepeat) : ((!noRows) ? ((PageItem)new Tablix(source4, pageContext, createForRepeat)) : ((PageItem)new NoRowsItem(source4, pageContext, createForRepeat))));
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				result = new Rectangle((Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)source, pageContext, createForRepeat);
			}
			else if (source is Microsoft.ReportingServices.OnDemandReportRendering.SubReport)
			{
				bool noRows2 = false;
				Microsoft.ReportingServices.OnDemandReportRendering.SubReport source5 = (Microsoft.ReportingServices.OnDemandReportRendering.SubReport)source;
				result = (TransformToTextBox(source5, tablixCellParent, ref noRows2) ? new TextBox(source5, pageContext, createForRepeat) : ((!noRows2) ? ((PageItem)new SubReport(source5, pageContext, createForRepeat)) : ((PageItem)new NoRowsItem(source5, pageContext, createForRepeat))));
			}
			return result;
		}

		internal static bool TransformToTextBox(DataRegion source)
		{
			bool result = false;
			DataRegionInstance dataRegionInstance = (DataRegionInstance)source.Instance;
			if (dataRegionInstance.NoRows && source.NoRowsMessage != null)
			{
				string text = null;
				text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : dataRegionInstance.NoRowsMessage);
				if (text != null)
				{
					result = true;
				}
			}
			return result;
		}

		internal static bool TransformToTextBox(Microsoft.ReportingServices.OnDemandReportRendering.Tablix source, bool tablixCellParent, ref bool noRows)
		{
			bool flag = false;
			TablixInstance tablixInstance = (TablixInstance)source.Instance;
			if (tablixInstance.NoRows)
			{
				noRows = true;
				if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : tablixInstance.NoRowsMessage);
					if (text != null)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					if (!source.HideStaticsIfNoRows)
					{
						noRows = false;
					}
					else if (tablixCellParent)
					{
						flag = true;
					}
				}
			}
			return flag;
		}

		internal static bool TransformToTextBox(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source, bool tablixCellParent, ref bool noRows)
		{
			bool result = false;
			SubReportInstance subReportInstance = (SubReportInstance)source.Instance;
			if (subReportInstance.NoRows)
			{
				noRows = true;
				if (tablixCellParent)
				{
					result = true;
				}
				else if (source.NoRowsMessage != null)
				{
					string text = null;
					text = ((!source.NoRowsMessage.IsExpression) ? source.NoRowsMessage.Value : subReportInstance.NoRowsMessage);
					if (text != null)
					{
						result = true;
					}
				}
			}
			else if (subReportInstance.ProcessedWithError)
			{
				result = true;
			}
			return result;
		}

		internal virtual void CreateItemRenderSizes(ItemSizes contentSize, PageContext pageContext, bool createForRepeat)
		{
			if (contentSize == null)
			{
				if (pageContext != null)
				{
					if (createForRepeat)
					{
						m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
					}
					else
					{
						m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
					}
				}
				if (m_itemRenderSizes == null)
				{
					m_itemRenderSizes = new ItemSizes(m_itemPageSizes);
				}
			}
			else
			{
				m_itemRenderSizes = contentSize;
			}
		}

		internal double ReserveSpaceForRepeatWith(RepeatWithItem[] repeatWithItems, PageContext pageContext)
		{
			if (repeatWithItems == null)
			{
				return 0.0;
			}
			DataRegion dataRegion = m_source as DataRegion;
			if (dataRegion == null)
			{
				return 0.0;
			}
			int[] repeatSiblings = dataRegion.GetRepeatSiblings();
			if (repeatSiblings == null || repeatSiblings.Length == 0)
			{
				return 0.0;
			}
			double num = 0.0;
			double num2 = 0.0;
			RepeatWithItem repeatWithItem = null;
			for (int i = 0; i < repeatSiblings.Length; i++)
			{
				repeatWithItem = repeatWithItems[repeatSiblings[i]];
				if (repeatWithItem != null)
				{
					if (repeatWithItem.RelativeTop < 0.0)
					{
						num = Math.Max(num, 0.0 - repeatWithItem.RelativeTop);
					}
					if (repeatWithItem.RelativeBottom > 0.0)
					{
						num2 = Math.Max(num2, repeatWithItem.RelativeBottom);
					}
				}
			}
			if (num + num2 >= pageContext.PageHeight)
			{
				if (num >= pageContext.PageHeight)
				{
					return 0.0;
				}
				num2 = 0.0;
			}
			return num + num2;
		}

		internal void WritePageItemRenderSizes(BinaryWriter spbifWriter)
		{
			WritePageItemRenderSizes(spbifWriter, m_offset);
		}

		internal void WritePageItemRenderSizes(BinaryWriter spbifWriter, long offset)
		{
			spbifWriter.Write((float)m_itemRenderSizes.Left);
			spbifWriter.Write((float)m_itemRenderSizes.Top);
			spbifWriter.Write((float)m_itemRenderSizes.Width);
			spbifWriter.Write((float)m_itemRenderSizes.Height);
			if (m_source == null)
			{
				spbifWriter.Write(0);
			}
			else
			{
				spbifWriter.Write(m_source.ZIndex);
			}
			spbifWriter.Write(m_rplItemState);
			spbifWriter.Write(offset);
		}

		internal RPLItemMeasurement WritePageItemRenderSizes()
		{
			RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
			rPLItemMeasurement.Left = (float)m_itemRenderSizes.Left;
			rPLItemMeasurement.Top = (float)m_itemRenderSizes.Top;
			rPLItemMeasurement.Width = (float)m_itemRenderSizes.Width;
			rPLItemMeasurement.Height = (float)m_itemRenderSizes.Height;
			if (m_source != null)
			{
				rPLItemMeasurement.ZIndex = m_source.ZIndex;
			}
			rPLItemMeasurement.State = m_rplItemState;
			rPLItemMeasurement.Element = m_rplElement;
			return rPLItemMeasurement;
		}

		internal void MergeRepeatSiblings(ref List<int> repeatedSiblings)
		{
			if (ItemState != State.OnPage && ItemState != State.OnPagePBEnd && ItemState != State.SpanPages)
			{
				return;
			}
			DataRegion dataRegion = m_source as DataRegion;
			if (dataRegion == null)
			{
				return;
			}
			int[] repeatSiblings = dataRegion.GetRepeatSiblings();
			if (repeatSiblings == null || repeatSiblings.Length == 0)
			{
				return;
			}
			if (repeatedSiblings == null)
			{
				repeatedSiblings = new List<int>(repeatSiblings);
				return;
			}
			int i = 0;
			for (int j = 0; j < repeatSiblings.Length; j++)
			{
				for (; repeatedSiblings[i] <= repeatSiblings[j]; i++)
				{
				}
				if (i < repeatedSiblings.Count)
				{
					repeatedSiblings.Insert(i, repeatSiblings[j]);
				}
				else
				{
					repeatedSiblings.Add(repeatSiblings[j]);
				}
				i++;
			}
		}

		internal override void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, offset);
			spbifWriter.Write((byte)0);
			spbifWriter.Write((byte)1);
			spbifWriter.Write(SourceID);
			if (m_source.Name != null)
			{
				spbifWriter.Write((byte)2);
				spbifWriter.Write(m_source.Name);
			}
			if (!m_source.ToolTip.IsExpression && m_source.ToolTip.Value != null)
			{
				spbifWriter.Write((byte)5);
				spbifWriter.Write(m_source.ToolTip.Value);
			}
			if (!m_source.Bookmark.IsExpression && m_source.Bookmark.Value != null)
			{
				spbifWriter.Write((byte)4);
				spbifWriter.Write(m_source.Bookmark.Value);
			}
			if (!m_source.DocumentMapLabel.IsExpression && m_source.DocumentMapLabel.Value != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(m_source.DocumentMapLabel.Value);
			}
			if (m_source.Visibility != null && m_source.Visibility.ToggleItem != null)
			{
				spbifWriter.Write((byte)8);
				spbifWriter.Write(m_source.Visibility.ToggleItem);
			}
			WriteCustomSharedItemProps(spbifWriter, rplWriter, pageContext);
			WriteSharedStyle(spbifWriter, null, pageContext, 6);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			spbifWriter.Write((byte)1);
			spbifWriter.Write((byte)0);
			spbifWriter.Write(m_source.Instance.UniqueName);
			if (m_source.ToolTip.IsExpression && m_source.Instance.ToolTip != null)
			{
				spbifWriter.Write((byte)5);
				spbifWriter.Write(m_source.Instance.ToolTip);
			}
			if (m_source.Bookmark.IsExpression && m_source.Instance.Bookmark != null)
			{
				spbifWriter.Write((byte)4);
				spbifWriter.Write(m_source.Instance.Bookmark);
			}
			if (m_source.DocumentMapLabel.IsExpression && m_source.Instance.DocumentMapLabel != null)
			{
				spbifWriter.Write((byte)3);
				spbifWriter.Write(m_source.Instance.DocumentMapLabel);
			}
			WriteCustomNonSharedItemProps(spbifWriter, rplWriter, pageContext);
			WriteNonSharedStyle(spbifWriter, null, null, pageContext, 6, null);
			spbifWriter.Write(byte.MaxValue);
		}

		internal override void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Hashtable hashtable = pageContext.ItemPropsStart;
			if (hashtable != null)
			{
				object obj = hashtable[SourceID];
				if (obj != null)
				{
					elemProps.Definition = (RPLElementPropsDef)obj;
					return;
				}
			}
			RPLItemPropsDef rPLItemPropsDef = (elemProps as RPLItemProps).Definition as RPLItemPropsDef;
			if (hashtable == null)
			{
				hashtable = (pageContext.ItemPropsStart = new Hashtable());
			}
			hashtable.Add(SourceID, rPLItemPropsDef);
			rPLItemPropsDef.ID = SourceID;
			rPLItemPropsDef.Name = m_source.Name;
			if (!m_source.ToolTip.IsExpression)
			{
				rPLItemPropsDef.ToolTip = m_source.ToolTip.Value;
			}
			if (!m_source.Bookmark.IsExpression)
			{
				rPLItemPropsDef.Bookmark = m_source.Bookmark.Value;
			}
			if (!m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemPropsDef.Label = m_source.DocumentMapLabel.Value;
			}
			if (m_source.Visibility != null)
			{
				rPLItemPropsDef.ToggleItem = m_source.Visibility.ToggleItem;
			}
			WriteCustomSharedItemProps(rPLItemPropsDef, rplWriter, pageContext);
			rPLItemPropsDef.SharedStyle = WriteSharedStyle(null, pageContext);
		}

		internal override void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			elemProps.UniqueName = m_source.Instance.UniqueName;
			RPLItemProps rPLItemProps = elemProps as RPLItemProps;
			if (m_source.ToolTip.IsExpression)
			{
				rPLItemProps.ToolTip = m_source.Instance.ToolTip;
			}
			if (m_source.Bookmark.IsExpression)
			{
				rPLItemProps.Bookmark = m_source.Instance.Bookmark;
			}
			if (m_source.DocumentMapLabel.IsExpression)
			{
				rPLItemProps.Label = m_source.Instance.DocumentMapLabel;
			}
			WriteCustomNonSharedItemProps(elemProps, rplWriter, pageContext);
			elemProps.NonSharedStyle = WriteNonSharedStyle(null, null, pageContext, null);
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				WriteItemNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal abstract bool CalculatePage(RPLWriter rplWriter, PageItemHelper lastPageInfo, PageContext pageContext, PageItem[] siblings, RepeatWithItem[] repeatWithItems, double parentTopInPage, ref double parentHeight, Interactivity interactivity);

		internal bool HiddenForOverlap(PageContext pageContext)
		{
			if (m_source.Visibility == null)
			{
				return false;
			}
			if (pageContext.AddToggledItems && m_source.Visibility.ToggleItem != null)
			{
				return false;
			}
			bool flag = false;
			if (m_source.Visibility.Hidden.IsExpression)
			{
				return m_source.Instance.Visibility.CurrentlyHidden;
			}
			return m_source.Visibility.Hidden.Value;
		}

		internal bool ResolveItemHiddenState(RPLWriter rplWriter, Interactivity interactivity, PageContext pageContext, bool createForRepeat, ref ItemSizes contentSize)
		{
			if (m_source.Visibility == null)
			{
				interactivity?.RegisterItem(this, pageContext);
				return false;
			}
			Visibility visibility = m_source.Visibility;
			bool result = false;
			if (visibility.HiddenState == SharedHiddenState.Sometimes)
			{
				bool flag = false;
				if (visibility.ToggleItem != null)
				{
					bool flag2 = false;
					flag2 = ((!visibility.Hidden.IsExpression) ? visibility.Hidden.Value : m_source.Instance.Visibility.StartHidden);
					if (pageContext.AddToggledItems)
					{
						m_rplItemState |= 16;
						if (m_source.Instance.Visibility.CurrentlyHidden)
						{
							m_rplItemState |= 32;
						}
					}
					else
					{
						flag = m_source.Instance.Visibility.CurrentlyHidden;
					}
					if (flag)
					{
						result = true;
						m_itemState = State.OnPageHidden;
						if (flag2)
						{
							m_itemPageSizes.AdjustHeightTo(0.0);
							m_itemPageSizes.AdjustWidthTo(0.0);
						}
						else if (rplWriter != null)
						{
							if (pageContext != null)
							{
								PaddItemSizes paddItemSizes = m_itemPageSizes as PaddItemSizes;
								if (paddItemSizes != null)
								{
									if (createForRepeat)
									{
										m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(paddItemSizes, isPadded: true, returnPaddings: false);
									}
									else
									{
										m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(paddItemSizes, isPadded: true, returnPaddings: false);
									}
								}
								else if (createForRepeat)
								{
									m_itemRenderSizes = pageContext.GetSharedRenderFromRepeatItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
								}
								else
								{
									m_itemRenderSizes = pageContext.GetSharedRenderItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
								}
							}
							if (m_itemRenderSizes == null)
							{
								m_itemRenderSizes = new ItemSizes(m_itemPageSizes);
							}
							m_itemRenderSizes.AdjustHeightTo(0.0);
							m_itemRenderSizes.AdjustWidthTo(0.0);
						}
					}
					else
					{
						double height = m_itemPageSizes.Height;
						if (flag2)
						{
							m_itemPageSizes.AdjustHeightTo(0.0);
						}
						PaddItemSizes paddItemSizes2 = m_itemPageSizes as PaddItemSizes;
						if (paddItemSizes2 != null)
						{
							if (pageContext != null)
							{
								if (createForRepeat)
								{
									contentSize = pageContext.GetSharedRenderFromRepeatItemSizesElement(paddItemSizes2, isPadded: true, returnPaddings: true);
								}
								else
								{
									contentSize = pageContext.GetSharedRenderItemSizesElement(paddItemSizes2, isPadded: true, returnPaddings: true);
								}
							}
							if (contentSize == null)
							{
								contentSize = new PaddItemSizes(paddItemSizes2);
							}
						}
						else
						{
							if (pageContext != null)
							{
								if (createForRepeat)
								{
									contentSize = pageContext.GetSharedRenderFromRepeatItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
								}
								else
								{
									contentSize = pageContext.GetSharedRenderItemSizesElement(m_itemPageSizes, isPadded: false, returnPaddings: false);
								}
							}
							if (contentSize == null)
							{
								contentSize = new ItemSizes(m_itemPageSizes);
							}
						}
						contentSize.AdjustHeightTo(height);
					}
				}
				else if (m_source.Instance.Visibility.CurrentlyHidden)
				{
					m_itemPageSizes.AdjustHeightTo(0.0);
					m_itemPageSizes.AdjustWidthTo(0.0);
					result = true;
					m_itemState = State.OnPageHidden;
				}
			}
			else if (visibility.HiddenState == SharedHiddenState.Always)
			{
				result = true;
				m_itemState = State.OnPageHidden;
			}
			interactivity?.RegisterItem(this, pageContext);
			return result;
		}

		internal virtual bool HitsCurrentPage(PageContext pageContext, double parentTopInPage)
		{
			if (pageContext.CancelPage)
			{
				m_itemState = State.Below;
				return false;
			}
			if (pageContext.FullOnPage)
			{
				m_itemState = State.OnPage;
				if (pageContext.CancelMode)
				{
					RoundedDouble pageHeight = new RoundedDouble(parentTopInPage + m_itemPageSizes.Top);
					pageContext.CheckPageSize(pageHeight);
				}
				return true;
			}
			if (m_itemState == State.Unknow)
			{
				RoundedDouble roundedDouble = new RoundedDouble(parentTopInPage + m_itemPageSizes.Top);
				pageContext.CheckPageSize(roundedDouble);
				if (!pageContext.StretchPage && roundedDouble >= pageContext.PageHeight)
				{
					m_itemState = State.Below;
					return false;
				}
				if (pageContext.TracingEnabled)
				{
					if (!pageContext.IgnorePageBreaks)
					{
						TracePageBreakIgnoredBecauseDisabled(pageContext.PageNumber);
					}
					else
					{
						TracePageBreakAtStartIgnored(pageContext);
					}
				}
				if (!pageContext.IgnorePageBreaks && PageBreakAtStart)
				{
					if (roundedDouble > 0.0)
					{
						m_itemState = State.TopNextPage;
						pageContext.RegisterPageBreak(new PageBreakInfo(PageBreak, ItemName));
						return false;
					}
					if (pageContext.TracingEnabled)
					{
						TracePageBreakIgnoredAtTop(pageContext.PageNumber);
					}
				}
				m_itemState = State.OnPage;
				return true;
			}
			if (m_itemState == State.OnPage || m_itemState == State.OnPagePBEnd || m_itemState == State.SpanPages)
			{
				return true;
			}
			return false;
		}

		internal bool HasItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			if (m_pageItemsAbove == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < m_pageItemsAbove.Count; i++)
			{
				if (siblings[m_pageItemsAbove[i]] == null)
				{
					if (repeatWithItems == null || repeatWithItems[m_pageItemsAbove[i]] == null)
					{
						m_pageItemsAbove.RemoveAt(i);
						i--;
					}
					else
					{
						num++;
					}
				}
			}
			if (m_pageItemsAbove.Count == 0)
			{
				m_pageItemsAbove = null;
				return false;
			}
			if (m_pageItemsAbove.Count == num)
			{
				return false;
			}
			return true;
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			AdjustOriginFromItemsAbove(siblings, repeatWithItems, adjustForRender: false);
		}

		internal void AdjustOriginFromItemsAbove(PageItem[] siblings, RepeatWithItem[] repeatWithItems, bool adjustForRender)
		{
			if (m_pageItemsAbove == null)
			{
				return;
			}
			double num = double.MinValue;
			double num2 = double.MaxValue;
			double num3 = double.MaxValue;
			double num4 = double.MaxValue;
			double num5 = 0.0;
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			for (int i = 0; i < m_pageItemsAbove.Count; i++)
			{
				pageItem = siblings[m_pageItemsAbove[i]];
				if (pageItem == null)
				{
					if (repeatWithItems == null || repeatWithItems[m_pageItemsAbove[i]] == null)
					{
						m_pageItemsAbove.RemoveAt(i);
						i--;
					}
					continue;
				}
				if (pageItem.ItemState != 0 && m_itemState == State.Unknow && pageItem.ItemState != State.OnPage && pageItem.ItemState != State.OnPageHidden)
				{
					m_itemState = State.Below;
				}
				if (adjustForRender)
				{
					if (pageItem.ItemState == State.Below || pageItem.ItemState == State.TopNextPage)
					{
						continue;
					}
					itemSizes = pageItem.ItemRenderSizes;
					num5 = ItemRenderSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
				}
				else
				{
					itemSizes = pageItem.ItemPageSizes;
					num5 = ItemPageSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
				}
				if ((RoundedDouble)itemSizes.DeltaY < num3)
				{
					num3 = itemSizes.DeltaY;
				}
				if ((RoundedDouble)itemSizes.DeltaY > num)
				{
					num = itemSizes.DeltaY;
					num4 = num5;
				}
				else if ((RoundedDouble)itemSizes.DeltaY == num)
				{
					num4 = Math.Min(num4, num5);
				}
				num2 = Math.Min(num2, num5);
			}
			if (num != double.MinValue)
			{
				if (num2 < 0.01 || num2 == double.MaxValue)
				{
					num2 = 0.0;
				}
				if (num4 < 0.01 || num4 == double.MaxValue)
				{
					num4 = 0.0;
				}
				num += num2 - num4;
				if (num < num3)
				{
					num = num3;
				}
				if (adjustForRender)
				{
					ItemRenderSizes.Top += num;
					ItemRenderSizes.DeltaY += num;
				}
				else
				{
					ItemPageSizes.Top += num;
					ItemPageSizes.DeltaY += num;
				}
			}
			if (m_pageItemsAbove.Count == 0)
			{
				m_pageItemsAbove = null;
			}
		}

		internal void AdjustOriginFromItemsAtLeft(PageItem[] siblings, bool adjustForRender)
		{
			if (m_pageItemsLeft == null)
			{
				return;
			}
			double num = double.MinValue;
			double num2 = double.MaxValue;
			double num3 = double.MaxValue;
			double num4 = double.MaxValue;
			double num5 = 0.0;
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			for (int i = 0; i < m_pageItemsLeft.Count; i++)
			{
				pageItem = siblings[m_pageItemsLeft[i]];
				if (pageItem == null)
				{
					m_pageItemsLeft.RemoveAt(i);
					i--;
				}
				else if (pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
				{
					if (adjustForRender)
					{
						itemSizes = pageItem.ItemRenderSizes;
						num5 = ItemRenderSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
					}
					else
					{
						itemSizes = pageItem.ItemPageSizes;
						num5 = ItemPageSizes.Left - (itemSizes.Right - itemSizes.DeltaX);
					}
					if ((RoundedDouble)itemSizes.DeltaX < num4)
					{
						num4 = itemSizes.DeltaX;
					}
					if ((RoundedDouble)itemSizes.DeltaX > num)
					{
						num = itemSizes.DeltaX;
						num3 = num5;
					}
					else if ((RoundedDouble)itemSizes.DeltaX == num)
					{
						num3 = Math.Min(num3, num5);
					}
					num2 = Math.Min(num2, num5);
				}
			}
			if (num != double.MinValue)
			{
				if (num2 < 0.01 || num2 == double.MaxValue)
				{
					num2 = 0.0;
				}
				if (num3 < 0.01 || num3 == double.MaxValue)
				{
					num3 = 0.0;
				}
				num += num2 - num3;
				if (num < num4)
				{
					num = num4;
				}
				if (adjustForRender)
				{
					ItemRenderSizes.Left += num;
					ItemRenderSizes.DeltaX += num;
				}
				else
				{
					ItemPageSizes.Left += num;
					ItemPageSizes.DeltaX += num;
				}
			}
		}

		internal virtual void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
		}

		internal void AdjustOriginFromRepeatItems(PageItem[] siblings)
		{
			if (m_pageItemsAbove == null)
			{
				return;
			}
			double num = double.MaxValue;
			double num2 = double.MaxValue;
			double num3 = 0.0;
			PageItem pageItem = null;
			ItemSizes itemSizes = null;
			for (int i = 0; i < m_pageItemsAbove.Count; i++)
			{
				pageItem = siblings[m_pageItemsAbove[i]];
				if (pageItem != null && pageItem.ItemState != State.Below && pageItem.ItemState != State.TopNextPage)
				{
					itemSizes = pageItem.ItemRenderSizes;
					num3 = ItemRenderSizes.Top - (itemSizes.Bottom - itemSizes.DeltaY);
					if (num3 >= 0.0)
					{
						num = Math.Min(num, num3);
					}
					else
					{
						num2 = Math.Min(num2, num3);
					}
				}
			}
			if (num2 != double.MaxValue)
			{
				if (num != double.MaxValue)
				{
					num2 -= num;
				}
				ItemRenderSizes.Top -= num2;
				ItemRenderSizes.DeltaY -= num2;
			}
		}

		internal virtual int WriteRepeatWithToPage(RPLWriter rplWriter, PageContext pageContext)
		{
			return 0;
		}

		internal void UpdateSizes(double topDelta, PageItem[] siblings, RepeatWithItem[] repeatWithItems)
		{
			m_itemPageSizes.UpdateSizes(topDelta, this, siblings, repeatWithItems);
			m_itemRenderSizes = null;
			m_offset = 0L;
			m_rplItemState = 0;
		}

		internal virtual void UpdateItem(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				if (itemHelper.ItemPageSizes != null)
				{
					m_itemPageSizes = itemHelper.ItemPageSizes.GetNewItem();
				}
				m_itemState = itemHelper.State;
				m_pageItemsAbove = GetNewList(itemHelper.PageItemsAbove);
				m_pageItemsLeft = GetNewList(itemHelper.PageItemsLeft);
				m_defLeftValue = itemHelper.DefLeftValue;
			}
		}

		internal virtual void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)1);
				WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal virtual PageItemHelper WritePaginationInfo()
		{
			return null;
		}

		internal virtual void WritePaginationInfoProperties(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo == null)
			{
				return;
			}
			if (m_itemPageSizes != null)
			{
				m_itemPageSizes.WritePaginationInfo(reportPageInfo);
			}
			reportPageInfo.Write((byte)3);
			reportPageInfo.Write((byte)m_itemState);
			reportPageInfo.Write((byte)21);
			reportPageInfo.Write(m_defLeftValue);
			if (m_pageItemsAbove != null && m_pageItemsAbove.Count > 0)
			{
				reportPageInfo.Write((byte)4);
				reportPageInfo.Write(m_pageItemsAbove.Count);
				for (int i = 0; i < m_pageItemsAbove.Count; i++)
				{
					reportPageInfo.Write(m_pageItemsAbove[i]);
				}
			}
			if (m_pageItemsLeft != null && m_pageItemsLeft.Count > 0)
			{
				reportPageInfo.Write((byte)5);
				reportPageInfo.Write(m_pageItemsLeft.Count);
				for (int j = 0; j < m_pageItemsLeft.Count; j++)
				{
					reportPageInfo.Write(m_pageItemsLeft[j]);
				}
			}
		}

		internal virtual void WritePaginationInfoProperties(PageItemHelper itemHelper)
		{
			if (itemHelper != null)
			{
				if (m_itemPageSizes != null)
				{
					itemHelper.ItemPageSizes = m_itemPageSizes.WritePaginationInfo();
				}
				itemHelper.State = m_itemState;
				itemHelper.DefLeftValue = m_defLeftValue;
				itemHelper.PageItemsAbove = GetNewList(m_pageItemsAbove);
				itemHelper.PageItemsLeft = GetNewList(m_pageItemsLeft);
			}
		}

		protected void TracePageBreakAtStartIgnored(PageContext context)
		{
			if (PageBreak != null && !PageBreak.Instance.Disabled && (PageBreak.BreakLocation == PageBreakLocation.Start || PageBreak.BreakLocation == PageBreakLocation.StartAndEnd))
			{
				TracePageBreakIgnored(context);
			}
		}

		protected void TracePageBreakAtEndIgnored(PageContext context)
		{
			if (PageBreak != null && !PageBreak.Instance.Disabled && (PageBreak.BreakLocation == PageBreakLocation.End || PageBreak.BreakLocation == PageBreakLocation.StartAndEnd))
			{
				TracePageBreakIgnored(context);
			}
		}

		private void TracePageBreakIgnored(PageContext context)
		{
			if (!context.Common.RegisteredPBIgnored.Contains(SourceID))
			{
				string str = "PR-DIAG [Page {0}] Page break on '{1}' ignored";
				switch (context.IgnorePBReason)
				{
				default:
					return;
				case PageContext.IgnorePBReasonFlag.TablixParent:
					str += " - inside TablixCell";
					break;
				case PageContext.IgnorePBReasonFlag.Toggled:
					str += " - part of toggleable region";
					break;
				}
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, str, context.PageNumber, ItemName);
				context.Common.RegisteredPBIgnored.Add(SourceID, null);
			}
		}

		protected void TracePageGrownOnKeepTogetherItem(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.KeepTogether, TraceLevel.Verbose, "PR-DIAG [Page {0}] Item '{1}' kept together - Explicit - Page grown", pageNumber, ItemName);
		}

		private void TracePageBreakIgnoredBecauseDisabled(int pageNumber)
		{
			if (PageBreak != null && PageBreak.Instance.Disabled && (PageBreak.BreakLocation == PageBreakLocation.Start || PageBreak.BreakLocation == PageBreakLocation.StartAndEnd))
			{
				RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – Disable is True", pageNumber, ItemName);
			}
		}

		private void TracePageBreakIgnoredAtTop(int pageNumber)
		{
			RenderingDiagnostics.Trace(RenderingArea.PageCreation, TraceLevel.Verbose, "PR-DIAG [Page {0}] Page break on '{1}' ignored – at top of page", pageNumber, ItemName);
		}
	}
}
