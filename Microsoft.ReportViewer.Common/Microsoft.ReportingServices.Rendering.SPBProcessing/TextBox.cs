using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class TextBox : PageItem, ITextBoxProps
	{
		internal enum CalcSize : byte
		{
			None,
			Done,
			Delay
		}

		private CalcSize m_calcSizeState;

		private bool m_isSimple;

		private List<Paragraph> m_paragraphs;

		private Microsoft.ReportingServices.Rendering.RichText.TextBox m_richTextBox;

		private float m_contentHeight;

		private RPLFormat.WritingModes m_writingMode;

		private long m_contentHeightPosition = -1L;

		private long m_startTextBoxOffset;

		private const int MAX_GDI_TEXT_SIZE = 32000;

		internal override string SourceID
		{
			get
			{
				if (m_source is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
				{
					return m_source.ID;
				}
				return m_source.ID + "_NR";
			}
		}

		internal CalcSize CalcSizeState
		{
			get
			{
				return m_calcSizeState;
			}
			set
			{
				m_calcSizeState = value;
			}
		}

		internal bool IsSimple => m_isSimple;

		public RPLFormat.TextAlignments DefaultAlignment
		{
			get
			{
				if (m_isSimple)
				{
					TypeCode typeCode = (m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox).SharedTypeCode;
					if (typeCode == TypeCode.Object)
					{
						TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
						if (textBoxInstance != null)
						{
							typeCode = textBoxInstance.TypeCode;
						}
					}
					if ((uint)(typeCode - 5) <= 11u)
					{
						return RPLFormat.TextAlignments.Right;
					}
				}
				return RPLFormat.TextAlignments.Left;
			}
		}

		public RPLFormat.Directions Direction => (RPLFormat.Directions)StyleEnumConverter.Translate((Directions)GetRichTextStyleValue(StyleAttributeNames.Direction, null));

		public RPLFormat.WritingModes WritingMode => m_writingMode;

		public Color BackgroundColor => Color.Empty;

		public bool CanGrow => (m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox)?.CanGrow ?? false;

		internal TextBox(Microsoft.ReportingServices.OnDemandReportRendering.TextBox source, PageContext pageContext, bool createForRepeat)
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
			m_isSimple = source.IsSimple;
			WritingModes aValue = (WritingModes)GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (source.CanGrow && m_itemPageSizes.Width > 0.0 && m_itemPageSizes.Height == 0.0)
			{
				bool flag = false;
				if (m_isSimple)
				{
					if (!string.IsNullOrEmpty(((TextBoxInstance)source.Instance).Value))
					{
						flag = true;
					}
				}
				else if (source.Paragraphs.Count > 1)
				{
					flag = true;
				}
				else
				{
					for (int i = 0; i < source.Paragraphs[0].TextRuns.Count; i++)
					{
						if (flag)
						{
							break;
						}
						if (!string.IsNullOrEmpty(source.Paragraphs[0].TextRuns[i].Value.Value))
						{
							flag = true;
						}
					}
				}
				if (flag)
				{
					m_itemPageSizes.Height = 0.3528;
				}
			}
			if (WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				m_contentHeight = (float)m_itemPageSizes.Height;
			}
			else
			{
				m_contentHeight = (float)m_itemPageSizes.Width;
			}
		}

		internal TextBox(DataRegion source, PageContext pageContext, bool createForRepeat)
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
			m_isSimple = true;
			WritingModes aValue = (WritingModes)GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				m_contentHeight = (float)m_itemPageSizes.Height;
			}
			else
			{
				m_contentHeight = (float)m_itemPageSizes.Width;
			}
		}

		internal TextBox(Microsoft.ReportingServices.OnDemandReportRendering.SubReport source, PageContext pageContext, bool createForRepeat)
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
			m_isSimple = true;
			WritingModes aValue = (WritingModes)GetRichTextStyleValue(StyleAttributeNames.WritingMode, null);
			m_writingMode = (RPLFormat.WritingModes)StyleEnumConverter.Translate(aValue, pageContext.VersionPicker);
			if (WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				m_contentHeight = (float)m_itemPageSizes.Height;
			}
			else
			{
				m_contentHeight = (float)m_itemPageSizes.Width;
			}
		}

		public void DrawTextRun(Microsoft.ReportingServices.Rendering.RichText.TextRun run, Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle)
		{
		}

		public void DrawClippedTextRun(Microsoft.ReportingServices.Rendering.RichText.TextRun run, Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph, Win32DCSafeHandle hdc, float dpiX, FontCache fontCache, int x, int y, int baselineY, int lineHeight, System.Drawing.Rectangle layoutRectangle, uint fontColorOverride, System.Drawing.Rectangle clipRect)
		{
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
				if (!flag)
				{
					CalculateRichTextElements(pageContext);
					MeasureTextBox(pageContext, contentSize, createForRepeat: false);
					WriteItemToStream(rplWriter, pageContext);
				}
				else
				{
					m_calcSizeState = CalcSize.Done;
				}
				if (m_itemRenderSizes == null)
				{
					CreateItemRenderSizes(contentSize, pageContext, createForRepeat: false);
				}
			}
			else
			{
				m_calcSizeState = CalcSize.Done;
			}
			return true;
		}

		internal override void CalculateRepeatWithPage(RPLWriter rplWriter, PageContext pageContext, PageItem[] siblings)
		{
			AdjustOriginFromItemsAbove(siblings, null);
			ItemSizes contentSize = null;
			if (!ResolveItemHiddenState(rplWriter, null, pageContext, createForRepeat: true, ref contentSize))
			{
				CalculateRichTextElements(pageContext);
				MeasureTextBox(pageContext, contentSize, createForRepeat: true);
			}
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
			RegisterItem.RegisterPageItem(this, pageContext, pageContext.EvaluatePageHeaderFooter, null);
			WriteItemToStream(rplWriter, pageContext);
			return 1;
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			if (textBox.CanGrow)
			{
				spbifWriter.Write((byte)25);
				spbifWriter.Write(textBox.CanGrow);
			}
			if (textBox.CanShrink)
			{
				spbifWriter.Write((byte)26);
				spbifWriter.Write(textBox.CanShrink);
			}
			if (textBox.CanSort)
			{
				spbifWriter.Write((byte)29);
				spbifWriter.Write(textBox.CanSort);
			}
			ReportStringProperty reportStringProperty = null;
			bool isSimple = textBox.IsSimple;
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = null;
			if (isSimple)
			{
				textRun = textBox.Paragraphs[0].TextRuns[0];
				reportStringProperty = textRun.Value;
			}
			if (reportStringProperty != null && reportStringProperty.IsExpression)
			{
				spbifWriter.Write((byte)31);
				spbifWriter.Write(reportStringProperty.ExpressionString);
			}
			if (textBox.IsToggleParent)
			{
				spbifWriter.Write((byte)32);
				spbifWriter.Write(textBox.IsToggleParent);
			}
			if (textBox.SharedTypeCode != TypeCode.String)
			{
				spbifWriter.Write((byte)33);
				spbifWriter.Write((byte)textBox.SharedTypeCode);
			}
			if (textBox.FormattedValueExpressionBased)
			{
				spbifWriter.Write((byte)45);
				spbifWriter.Write(textBox.FormattedValueExpressionBased);
			}
			if (!textBox.HideDuplicates && reportStringProperty != null && reportStringProperty.Value != null && !textBox.FormattedValueExpressionBased)
			{
				spbifWriter.Write((byte)27);
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					spbifWriter.Write(reportStringProperty.Value);
				}
				else
				{
					spbifWriter.Write(textRun.Instance.Value);
				}
			}
			if (!isSimple)
			{
				spbifWriter.Write((byte)35);
				spbifWriter.Write(isSimple);
			}
			pageContext.HideDuplicates = textBox.HideDuplicates;
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)sharedProps;
			rPLTextBoxPropsDef.CanGrow = textBox.CanGrow;
			rPLTextBoxPropsDef.CanShrink = textBox.CanShrink;
			rPLTextBoxPropsDef.CanSort = textBox.CanSort;
			ReportStringProperty reportStringProperty = null;
			bool isSimple = textBox.IsSimple;
			Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = null;
			if (isSimple)
			{
				textRun = textBox.Paragraphs[0].TextRuns[0];
				reportStringProperty = textRun.Value;
			}
			if (reportStringProperty != null && reportStringProperty.IsExpression)
			{
				rPLTextBoxPropsDef.Formula = reportStringProperty.ExpressionString;
			}
			rPLTextBoxPropsDef.IsToggleParent = textBox.IsToggleParent;
			rPLTextBoxPropsDef.SharedTypeCode = textBox.SharedTypeCode;
			rPLTextBoxPropsDef.FormattedValueExpressionBased = textBox.FormattedValueExpressionBased;
			if (!textBox.HideDuplicates && reportStringProperty != null && reportStringProperty.Value != null && !textBox.FormattedValueExpressionBased)
			{
				if (textRun.SharedTypeCode == TypeCode.String)
				{
					rPLTextBoxPropsDef.Value = reportStringProperty.Value;
				}
				else
				{
					rPLTextBoxPropsDef.Value = textRun.Instance.Value;
				}
			}
			rPLTextBoxPropsDef.IsSimple = isSimple;
			pageContext.HideDuplicates = textBox.HideDuplicates;
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = m_source as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					if (dataRegionInstance.NoRowsMessage != null)
					{
						spbifWriter.Write((byte)27);
						spbifWriter.Write(dataRegionInstance.NoRowsMessage);
					}
					return;
				}
				SubReportInstance subReportInstance = (SubReportInstance)(m_source as Microsoft.ReportingServices.OnDemandReportRendering.SubReport).Instance;
				if (subReportInstance.ProcessedWithError)
				{
					spbifWriter.Write((byte)27);
					spbifWriter.Write(subReportInstance.ErrorMessage);
				}
				else if (subReportInstance.NoRowsMessage != null)
				{
					spbifWriter.Write((byte)27);
					spbifWriter.Write(subReportInstance.NoRowsMessage);
				}
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = (Microsoft.ReportingServices.OnDemandReportRendering.TextBox)m_source;
			pageContext.HideDuplicates = textBox.HideDuplicates;
			if (textBox.IsToggleParent && textBoxInstance.IsToggleParent)
			{
				spbifWriter.Write((byte)32);
				spbifWriter.Write(textBoxInstance.IsToggleParent);
				if (textBoxInstance.ToggleState)
				{
					spbifWriter.Write((byte)28);
					spbifWriter.Write(textBoxInstance.ToggleState);
				}
				if (pageContext.RegisterEvents)
				{
					textBoxInstance.RegisterToggleSender();
				}
			}
			if (textBox.CanSort)
			{
				switch (textBoxInstance.SortState)
				{
				case SortOptions.Ascending:
					spbifWriter.Write((byte)30);
					spbifWriter.Write((byte)1);
					break;
				case SortOptions.Descending:
					spbifWriter.Write((byte)30);
					spbifWriter.Write((byte)2);
					break;
				}
			}
			spbifWriter.Write((byte)36);
			m_contentHeightPosition = spbifWriter.BaseStream.Position;
			spbifWriter.Write(m_contentHeight);
			if (m_isSimple && !HideDuplicate(textBox, textBoxInstance, pageContext))
			{
				_ = m_paragraphs[0].TextRuns[0].Source;
				if (textBoxInstance.ProcessedWithError)
				{
					spbifWriter.Write((byte)46);
					spbifWriter.Write(value: true);
				}
				if (pageContext.HideDuplicates || textBox.FormattedValueExpressionBased)
				{
					if (pageContext.AddOriginalValue)
					{
						object originalValue = textBoxInstance.OriginalValue;
						TypeCode typeCode = textBoxInstance.TypeCode;
						TypeCode sharedTypeCode = textBox.SharedTypeCode;
						if ((byte)typeCode != (byte)sharedTypeCode)
						{
							spbifWriter.Write((byte)33);
							spbifWriter.Write((byte)typeCode);
						}
						pageContext.TypeCodeNonString = WriteOriginalValue(spbifWriter, typeCode, originalValue);
						if (!pageContext.TypeCodeNonString && textBoxInstance.Value != null)
						{
							if (pageContext.AddToggledItems)
							{
								object richTextStyleValue = m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
								if (richTextStyleValue != null)
								{
									spbifWriter.Write((byte)27);
									spbifWriter.Write(textBoxInstance.Value);
								}
							}
							else
							{
								spbifWriter.Write((byte)27);
								spbifWriter.Write(textBoxInstance.Value);
							}
						}
					}
					else
					{
						TypeCode typeCode = textBoxInstance.TypeCode;
						TypeCode sharedTypeCode = textBox.SharedTypeCode;
						if ((byte)typeCode != (byte)sharedTypeCode)
						{
							spbifWriter.Write((byte)33);
							spbifWriter.Write((byte)typeCode);
						}
						if (textBoxInstance.Value != null)
						{
							spbifWriter.Write((byte)27);
							spbifWriter.Write(textBoxInstance.Value);
						}
					}
				}
				else if (pageContext.AddOriginalValue && textBox.SharedTypeCode != TypeCode.String)
				{
					pageContext.TypeCodeNonString = WriteOriginalValue(spbifWriter, textBox.SharedTypeCode, textBoxInstance.OriginalValue);
					if (textBoxInstance.Value != null)
					{
						if (pageContext.AddToggledItems)
						{
							object richTextStyleValue2 = m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
							if (richTextStyleValue2 != null)
							{
								spbifWriter.Write((byte)27);
								spbifWriter.Write(textBoxInstance.Value);
							}
						}
						else
						{
							spbifWriter.Write((byte)27);
							spbifWriter.Write(textBoxInstance.Value);
						}
					}
				}
			}
			WriteActionInfo(textBox.ActionInfo, spbifWriter, pageContext, 7);
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)nonSharedProps;
			TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = m_source as DataRegion;
				if (dataRegion != null)
				{
					DataRegionInstance dataRegionInstance = (DataRegionInstance)dataRegion.Instance;
					rPLTextBoxProps.Value = dataRegionInstance.NoRowsMessage;
					return;
				}
				SubReportInstance subReportInstance = (SubReportInstance)(m_source as Microsoft.ReportingServices.OnDemandReportRendering.SubReport).Instance;
				if (subReportInstance.ProcessedWithError)
				{
					rPLTextBoxProps.Value = subReportInstance.ErrorMessage;
				}
				else
				{
					rPLTextBoxProps.Value = subReportInstance.NoRowsMessage;
				}
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = (Microsoft.ReportingServices.OnDemandReportRendering.TextBox)m_source;
			pageContext.HideDuplicates = textBox.HideDuplicates;
			if (textBox.IsToggleParent && textBoxInstance.IsToggleParent)
			{
				rPLTextBoxProps.IsToggleParent = textBoxInstance.IsToggleParent;
				rPLTextBoxProps.ToggleState = textBoxInstance.ToggleState;
				if (pageContext.RegisterEvents)
				{
					textBoxInstance.RegisterToggleSender();
				}
			}
			switch (textBoxInstance.SortState)
			{
			case SortOptions.Ascending:
				rPLTextBoxProps.SortState = RPLFormat.SortOptions.Ascending;
				break;
			case SortOptions.Descending:
				rPLTextBoxProps.SortState = RPLFormat.SortOptions.Descending;
				break;
			case SortOptions.None:
				rPLTextBoxProps.SortState = RPLFormat.SortOptions.None;
				break;
			}
			rPLTextBoxProps.ContentHeight = m_contentHeight;
			rPLTextBoxProps.TypeCode = textBox.SharedTypeCode;
			if (m_isSimple && !HideDuplicate(textBox, textBoxInstance, pageContext))
			{
				_ = m_paragraphs[0].TextRuns[0].Source;
				rPLTextBoxProps.ProcessedWithError = textBoxInstance.ProcessedWithError;
				if (pageContext.HideDuplicates || textBox.FormattedValueExpressionBased)
				{
					if (pageContext.AddOriginalValue)
					{
						object originalValue = textBoxInstance.OriginalValue;
						TypeCode typeCode = textBoxInstance.TypeCode;
						TypeCode sharedTypeCode = textBox.SharedTypeCode;
						if ((byte)typeCode != (byte)sharedTypeCode)
						{
							rPLTextBoxProps.TypeCode = typeCode;
						}
						pageContext.TypeCodeNonString = WriteOriginalValue(rPLTextBoxProps, typeCode, originalValue);
						if (!pageContext.TypeCodeNonString && textBoxInstance.Value != null)
						{
							if (pageContext.AddToggledItems)
							{
								object richTextStyleValue = m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
								if (richTextStyleValue != null)
								{
									rPLTextBoxProps.Value = textBoxInstance.Value;
								}
							}
							else
							{
								rPLTextBoxProps.Value = textBoxInstance.Value;
							}
						}
					}
					else
					{
						TypeCode typeCode = textBoxInstance.TypeCode;
						TypeCode sharedTypeCode = textBox.SharedTypeCode;
						if ((byte)typeCode != (byte)sharedTypeCode)
						{
							rPLTextBoxProps.TypeCode = typeCode;
						}
						rPLTextBoxProps.Value = textBoxInstance.Value;
					}
				}
				else if (pageContext.AddOriginalValue && textBox.SharedTypeCode != TypeCode.String)
				{
					pageContext.TypeCodeNonString = WriteOriginalValue(rPLTextBoxProps, textBox.SharedTypeCode, textBoxInstance.OriginalValue);
					if (textBoxInstance.Value != null)
					{
						if (pageContext.AddToggledItems)
						{
							object richTextStyleValue2 = m_paragraphs[0].TextRuns[0].GetRichTextStyleValue(StyleAttributeNames.Format, null);
							if (richTextStyleValue2 != null)
							{
								rPLTextBoxProps.Value = textBoxInstance.Value;
							}
						}
						else
						{
							rPLTextBoxProps.Value = textBoxInstance.Value;
						}
					}
				}
			}
			rPLTextBoxProps.ActionInfo = WriteActionInfo(textBox.ActionInfo, pageContext);
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(spbifWriter, style, writeShared: true, pageContext);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.UnicodeBiDi, 31);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.WritingMode, 30, pageContext.VersionPicker);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.Direction, 29);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
			WriteStyleProp(style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Color, 27);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontFamily, 20);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontSize, 21);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontStyle, 19);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.FontWeight, 22);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Format, 23);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Language, 32);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.LineHeight, 28);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.Calendar, 38);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextAlign, 25);
				WriteStyleProp(style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
			}
			else if (m_isSimple)
			{
				Style style2 = textBox.Paragraphs[0].Style;
				m_paragraphs[0].WriteItemSharedStyleProps(spbifWriter, style2, pageContext);
				Style style3 = textBox.Paragraphs[0].TextRuns[0].Style;
				m_paragraphs[0].TextRuns[0].WriteItemSharedStyleProps(spbifWriter, style3, pageContext);
			}
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			WriteBackgroundImage(rplStyleProps, style, writeShared: true, pageContext);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.UnicodeBiDi, 31);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Direction, 29);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
			WriteStyleProp(style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Color, 27);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontSize, 21);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Format, 23);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Language, 32);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.LineHeight, 28);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.Calendar, 38);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
				WriteStyleProp(style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
			}
			else if (m_isSimple)
			{
				Style style2 = textBox.Paragraphs[0].Style;
				m_paragraphs[0].WriteItemSharedStyleProps(rplStyleProps, style2, pageContext);
				Style style3 = textBox.Paragraphs[0].TextRuns[0].Style;
				m_paragraphs[0].TextRuns[0].WriteItemSharedStyleProps(rplStyleProps, style3, pageContext);
			}
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				return;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(spbifWriter, styleDef, writeShared: false, pageContext);
				return;
			case StyleAttributeNames.UnicodeBiDi:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.UnicodeBiDi, 31);
				return;
			case StyleAttributeNames.VerticalAlign:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.VerticalAlign, 26);
				return;
			case StyleAttributeNames.WritingMode:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.WritingMode, 30, pageContext.VersionPicker);
				return;
			case StyleAttributeNames.Direction:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Direction, 29);
				return;
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingBottom, 18);
				return;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingLeft, 15);
				return;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingRight, 16);
				return;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.PaddingTop, 17);
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				switch (styleAtt)
				{
				case StyleAttributeNames.VerticalAlign:
				case StyleAttributeNames.PaddingLeft:
				case StyleAttributeNames.PaddingRight:
				case StyleAttributeNames.PaddingTop:
				case StyleAttributeNames.PaddingBottom:
				case StyleAttributeNames.Direction:
				case StyleAttributeNames.WritingMode:
				case StyleAttributeNames.UnicodeBiDi:
					break;
				case StyleAttributeNames.TextAlign:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextAlign, 25);
					break;
				case StyleAttributeNames.LineHeight:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.LineHeight, 28);
					break;
				case StyleAttributeNames.Color:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Color, 27);
					break;
				case StyleAttributeNames.FontFamily:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontFamily, 20);
					break;
				case StyleAttributeNames.FontSize:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontSize, 21);
					break;
				case StyleAttributeNames.FontStyle:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontStyle, 19);
					break;
				case StyleAttributeNames.FontWeight:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.FontWeight, 22);
					break;
				case StyleAttributeNames.Format:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Format, 23);
					break;
				case StyleAttributeNames.Language:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Language, 32);
					break;
				case StyleAttributeNames.NumeralLanguage:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralLanguage, 36);
					break;
				case StyleAttributeNames.NumeralVariant:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.NumeralVariant, 37);
					break;
				case StyleAttributeNames.Calendar:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.Calendar, 38);
					break;
				case StyleAttributeNames.TextDecoration:
					WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.TextDecoration, 24);
					break;
				}
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				return;
			case StyleAttributeNames.BackgroundImage:
				WriteBackgroundImage(rplStyleProps, styleDef, writeShared: false, pageContext);
				return;
			case StyleAttributeNames.UnicodeBiDi:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.UnicodeBiDi, 31);
				return;
			case StyleAttributeNames.VerticalAlign:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.VerticalAlign, 26);
				return;
			case StyleAttributeNames.WritingMode:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.WritingMode, 30);
				return;
			case StyleAttributeNames.Direction:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Direction, 29);
				return;
			case StyleAttributeNames.PaddingBottom:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingBottom, 18);
				return;
			case StyleAttributeNames.PaddingLeft:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingLeft, 15);
				return;
			case StyleAttributeNames.PaddingRight:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingRight, 16);
				return;
			case StyleAttributeNames.PaddingTop:
				WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.PaddingTop, 17);
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				switch (styleAtt)
				{
				case StyleAttributeNames.VerticalAlign:
				case StyleAttributeNames.PaddingLeft:
				case StyleAttributeNames.PaddingRight:
				case StyleAttributeNames.PaddingTop:
				case StyleAttributeNames.PaddingBottom:
				case StyleAttributeNames.Direction:
				case StyleAttributeNames.WritingMode:
				case StyleAttributeNames.UnicodeBiDi:
					break;
				case StyleAttributeNames.Color:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Color, 27);
					break;
				case StyleAttributeNames.FontFamily:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontFamily, 20);
					break;
				case StyleAttributeNames.FontSize:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontSize, 21);
					break;
				case StyleAttributeNames.FontStyle:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontStyle, 19);
					break;
				case StyleAttributeNames.FontWeight:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.FontWeight, 22);
					break;
				case StyleAttributeNames.Format:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Format, 23);
					break;
				case StyleAttributeNames.Language:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Language, 32);
					break;
				case StyleAttributeNames.LineHeight:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.LineHeight, 28);
					break;
				case StyleAttributeNames.NumeralLanguage:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralLanguage, 36);
					break;
				case StyleAttributeNames.NumeralVariant:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.NumeralVariant, 37);
					break;
				case StyleAttributeNames.Calendar:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.Calendar, 38);
					break;
				case StyleAttributeNames.TextAlign:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextAlign, 25);
					break;
				case StyleAttributeNames.TextDecoration:
					WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.TextDecoration, 24);
					break;
				}
			}
		}

		internal override void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext, byte? rplTag, ReportElementInstance compiledSource)
		{
			if (m_source == null)
			{
				return;
			}
			bool flag = WriteCommonNonSharedStyle(spbifWriter, styleDef, style, pageContext, rplTag, compiledSource);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (m_isSimple && textBox != null)
			{
				if (!flag)
				{
					if (rplTag.HasValue)
					{
						spbifWriter.Write(rplTag.Value);
					}
					spbifWriter.Write((byte)1);
					flag = true;
				}
				Paragraph paragraph = m_paragraphs[0];
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph2 = textBox.Paragraphs[0];
				Style style2 = paragraph2.Style;
				List<StyleAttributeNames> nonSharedStyleAttributes = style2.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					StyleInstance style3 = paragraph2.Instance.Style;
					for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
					{
						paragraph.WriteNonSharedStyleProp(spbifWriter, style2, style3, nonSharedStyleAttributes[i], pageContext);
					}
				}
				Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = paragraph2.TextRuns[0];
				Style style4 = textRun.Style;
				nonSharedStyleAttributes = style4.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					TextRun textRun2 = paragraph.TextRuns[0];
					StyleInstance style5 = textRun.Instance.Style;
					for (int j = 0; j < nonSharedStyleAttributes.Count; j++)
					{
						textRun2.WriteNonSharedStyleProp(spbifWriter, style4, style5, nonSharedStyleAttributes[j], pageContext);
					}
				}
			}
			if (flag)
			{
				spbifWriter.Write(byte.MaxValue);
			}
		}

		internal override RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext, ReportElementInstance compiledSource)
		{
			RPLStyleProps rPLStyleProps = base.WriteNonSharedStyle(styleDef, style, pageContext, compiledSource);
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (m_isSimple && textBox != null)
			{
				if (rPLStyleProps == null)
				{
					rPLStyleProps = new RPLStyleProps();
				}
				Paragraph paragraph = m_paragraphs[0];
				Microsoft.ReportingServices.OnDemandReportRendering.Paragraph paragraph2 = textBox.Paragraphs[0];
				Style style2 = paragraph2.Style;
				List<StyleAttributeNames> nonSharedStyleAttributes = style2.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					StyleInstance style3 = paragraph2.Instance.Style;
					for (int i = 0; i < nonSharedStyleAttributes.Count; i++)
					{
						paragraph.WriteNonSharedStyleProp(rPLStyleProps, style2, style3, nonSharedStyleAttributes[i], pageContext);
					}
				}
				Microsoft.ReportingServices.OnDemandReportRendering.TextRun textRun = paragraph2.TextRuns[0];
				Style style4 = textRun.Style;
				nonSharedStyleAttributes = style4.NonSharedStyleAttributes;
				if (nonSharedStyleAttributes != null && nonSharedStyleAttributes.Count > 0)
				{
					TextRun textRun2 = paragraph.TextRuns[0];
					StyleInstance style5 = textRun.Instance.Style;
					for (int j = 0; j < nonSharedStyleAttributes.Count; j++)
					{
						textRun2.WriteNonSharedStyleProp(rPLStyleProps, style4, style5, nonSharedStyleAttributes[j], pageContext);
					}
				}
			}
			return rPLStyleProps;
		}

		internal override void WritePaginationInfo(BinaryWriter reportPageInfo)
		{
			if (reportPageInfo != null)
			{
				reportPageInfo.Write((byte)12);
				base.WritePaginationInfoProperties(reportPageInfo);
				reportPageInfo.Write(byte.MaxValue);
			}
		}

		internal override PageItemHelper WritePaginationInfo()
		{
			PageItemHelper pageItemHelper = new PageItemHelper(12);
			base.WritePaginationInfoProperties(pageItemHelper);
			return pageItemHelper;
		}

		internal void MeasureTextBox(PageContext pageContext, ItemSizes contentSize, bool createForRepeat)
		{
			if (m_calcSizeState == CalcSize.Done)
			{
				return;
			}
			if (pageContext != null && pageContext.MeasureItems)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
				if (textBox != null && m_richTextBox == null && (textBox.CanGrow || textBox.CanShrink))
				{
					if (m_calcSizeState == CalcSize.Delay)
					{
						return;
					}
					if (!pageContext.Common.EmSquare)
					{
						m_richTextBox = new Microsoft.ReportingServices.Rendering.RichText.TextBox(this);
					}
					if (!HideDuplicate(textBox, (TextBoxInstance)textBox.Instance, pageContext))
					{
						double padVertical = 0.0;
						float num = (!pageContext.Common.EmSquare) ? MeasureTextBox_Uniscribe(pageContext, contentSize, textBox, ref padVertical) : MeasureTextBox_GDI(pageContext, contentSize, textBox, ref padVertical);
						double num2 = 0.0;
						num2 = ((contentSize != null) ? ((double)num - (contentSize.Height - padVertical)) : ((double)num - (m_itemPageSizes.Height - padVertical)));
						if ((num2 > 0.0 && textBox.CanGrow) || (num2 < 0.0 && textBox.CanShrink))
						{
							if (m_itemRenderSizes == null)
							{
								CreateItemRenderSizes(contentSize, pageContext, createForRepeat);
							}
							m_itemRenderSizes.AdjustHeightTo(m_itemRenderSizes.Height + num2 + 0.0001);
						}
					}
					else if (textBox.CanShrink)
					{
						if (m_itemRenderSizes == null)
						{
							CreateItemRenderSizes(contentSize, pageContext, createForRepeat);
						}
						m_itemRenderSizes.AdjustHeightTo(0.0);
					}
				}
			}
			m_calcSizeState = CalcSize.Done;
		}

		private float MeasureTextBox_Uniscribe(PageContext pageContext, ItemSizes contentSize, Microsoft.ReportingServices.OnDemandReportRendering.TextBox tbDef, ref double padVertical)
		{
			double padHorizontal = 0.0;
			CalculateTotalPaddings(pageContext, ref padHorizontal, ref padVertical);
			float num = 0f;
			float num2 = 0f;
			float contentHeight = 0f;
			FlowContext flowContext = new FlowContext(TextBoxWidth(tbDef, pageContext, padHorizontal), float.MaxValue);
			if (tbDef.CanGrow && !tbDef.CanShrink)
			{
				flowContext.Height = (float)(m_itemPageSizes.Height - padVertical);
			}
			if (WritingMode == RPLFormat.WritingModes.Horizontal)
			{
				m_richTextBox.Paragraphs = new List<Microsoft.ReportingServices.Rendering.RichText.Paragraph>(1);
				for (int i = 0; i < m_paragraphs.Count; i++)
				{
					Microsoft.ReportingServices.Rendering.RichText.Paragraph richTextParagraph = m_paragraphs[i].GetRichTextParagraph();
					m_richTextBox.Paragraphs.Add(richTextParagraph);
					m_richTextBox.ScriptItemize();
					float contentHeight2 = 0f;
					float num3 = pageContext.MeasureFullTextBoxHeight(m_richTextBox, flowContext, out contentHeight2);
					num += num3;
					num2 += num3;
					m_richTextBox.Paragraphs.RemoveAt(0);
				}
				m_contentHeight = num2;
			}
			else
			{
				m_richTextBox.Paragraphs = new List<Microsoft.ReportingServices.Rendering.RichText.Paragraph>(m_paragraphs.Count);
				for (int j = 0; j < m_paragraphs.Count; j++)
				{
					Microsoft.ReportingServices.Rendering.RichText.Paragraph richTextParagraph2 = m_paragraphs[j].GetRichTextParagraph();
					m_richTextBox.Paragraphs.Add(richTextParagraph2);
				}
				m_richTextBox.ScriptItemize();
				num = pageContext.MeasureFullTextBoxHeight(m_richTextBox, flowContext, out contentHeight);
				m_contentHeight = contentHeight;
			}
			return num;
		}

		private float MeasureTextBox_GDI(PageContext pageContext, ItemSizes contentSize, Microsoft.ReportingServices.OnDemandReportRendering.TextBox tbDef, ref double padVertical)
		{
			float result = 0f;
			double padHorizontal = 0.0;
			int charactersFitted = 0;
			int linesFilled = 0;
			bool newFont;
			bool newStringFormat;
			CanvasFont font;
			string text = AggregateTextBoxStyle(pageContext, ref padHorizontal, ref padVertical, out font, out newFont, out newStringFormat);
			if (!string.IsNullOrEmpty(text))
			{
				result = (m_contentHeight = pageContext.MeasureStringGDI(text, font, new SizeF(TextBoxWidth(tbDef, pageContext, padHorizontal), float.MaxValue), out charactersFitted, out linesFilled).Height);
			}
			if (font != null)
			{
				if (newFont && newStringFormat)
				{
					font.Dispose();
					font = null;
				}
				else if (newFont)
				{
					if (font.GDIFont != null)
					{
						font.GDIFont.Dispose();
					}
				}
				else if (newStringFormat && font.TrimStringFormat != null)
				{
					font.TrimStringFormat.Dispose();
				}
			}
			return result;
		}

		private string AggregateTextBoxStyle(PageContext pageContext, ref double padHorizontal, ref double padVertical, out CanvasFont font, out bool newFont, out bool newStringFormat)
		{
			CalculateTotalPaddings(pageContext, ref padHorizontal, ref padVertical);
			StringBuilder stringBuilder = new StringBuilder();
			font = null;
			bool flag = true;
			newFont = false;
			newStringFormat = false;
			if (m_paragraphs != null)
			{
				foreach (Paragraph paragraph2 in m_paragraphs)
				{
					if (paragraph2.TextRuns == null)
					{
						continue;
					}
					if (!flag)
					{
						stringBuilder.AppendLine();
					}
					flag = false;
					StringBuilder stringBuilder2 = new StringBuilder();
					foreach (TextRun textRun in paragraph2.TextRuns)
					{
						string value = textRun.ComputeValue();
						if (!string.IsNullOrEmpty(value))
						{
							stringBuilder2.Append(value);
							if (font == null)
							{
								font = CreateFontPens(pageContext, paragraph2, textRun, out newFont, out newStringFormat);
							}
						}
					}
					if (stringBuilder2.Length > 0)
					{
						string text = stringBuilder2.ToString();
						AddNewLinesAtGdiLimits(ref text);
						stringBuilder.Append(text);
					}
				}
			}
			if (font == null && stringBuilder.Length > 0)
			{
				Paragraph paragraph = m_paragraphs[0];
				font = CreateFontPens(pageContext, paragraph, paragraph.TextRuns[0], out newFont, out newStringFormat);
			}
			return stringBuilder.ToString();
		}

		internal bool SearchTextBox(string findValue, PageContext pageContext)
		{
			string text = null;
			TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				DataRegion dataRegion = m_source as DataRegion;
				if (dataRegion != null)
				{
					text = ((DataRegionInstance)dataRegion.Instance).NoRowsMessage;
				}
				else
				{
					SubReportInstance subReportInstance = (SubReportInstance)(m_source as Microsoft.ReportingServices.OnDemandReportRendering.SubReport).Instance;
					text = ((!subReportInstance.ProcessedWithError) ? subReportInstance.NoRowsMessage : SPBRes.RenderSubreportError);
				}
			}
			else
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TextBox tbDef = (Microsoft.ReportingServices.OnDemandReportRendering.TextBox)m_source;
				if (!HideDuplicate(tbDef, textBoxInstance, pageContext))
				{
					text = textBoxInstance.Value;
				}
			}
			if (text != null)
			{
				string text2 = findValue;
				if (text2.IndexOf(' ') >= 0)
				{
					text2 = text2.Replace('\u00a0', ' ');
					text = text.Replace('\u00a0', ' ');
				}
				if (text.IndexOf(text2, 0, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private float TextBoxWidth(Microsoft.ReportingServices.OnDemandReportRendering.TextBox tbDef, PageContext pageContext, double padHorizontal)
		{
			double num = m_itemPageSizes.Width - padHorizontal;
			double num2 = pageContext.ConvertToMillimeters(11, pageContext.DpiX) + 2.2;
			if (num > num2 && tbDef.IsToggleParent && ((TextBoxInstance)tbDef.Instance).IsToggleParent)
			{
				num -= num2;
			}
			num2 = pageContext.ConvertToMillimeters(16, pageContext.DpiY) + 2.2;
			if (num > num2 && tbDef.CanSort)
			{
				num -= num2;
			}
			return (float)num;
		}

		private bool HideDuplicate(Microsoft.ReportingServices.OnDemandReportRendering.TextBox tbDef, TextBoxInstance tbInst, PageContext pageContext)
		{
			if (!tbDef.HideDuplicates)
			{
				return false;
			}
			TextBoxSharedInfo textBoxSharedInfo = null;
			if (pageContext.TextBoxSharedInfo != null)
			{
				textBoxSharedInfo = (TextBoxSharedInfo)pageContext.TextBoxSharedInfo[tbDef.ID];
			}
			if (!tbInst.Duplicate)
			{
				if (textBoxSharedInfo == null)
				{
					if (pageContext.TextBoxSharedInfo == null)
					{
						pageContext.TextBoxSharedInfo = new Hashtable();
					}
					pageContext.TextBoxSharedInfo.Add(tbDef.ID, new TextBoxSharedInfo(pageContext.PageNumber));
				}
				else
				{
					textBoxSharedInfo.PageNumber = pageContext.PageNumber;
				}
			}
			else
			{
				if (textBoxSharedInfo != null && textBoxSharedInfo.PageNumber >= pageContext.PageNumber)
				{
					return true;
				}
				if (textBoxSharedInfo == null)
				{
					if (pageContext.TextBoxSharedInfo == null)
					{
						pageContext.TextBoxSharedInfo = new Hashtable();
					}
					pageContext.TextBoxSharedInfo.Add(tbDef.ID, new TextBoxSharedInfo(pageContext.PageNumber));
				}
				else
				{
					textBoxSharedInfo.PageNumber = pageContext.PageNumber;
				}
			}
			return false;
		}

		private void CalculateTotalPaddings(PageContext pageContext, ref double padHorizontal, ref double padVertical)
		{
			ReportSize reportSize = (ReportSize)GetRichTextStyleValue(StyleAttributeNames.PaddingTop, null);
			if (reportSize != null)
			{
				padVertical += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)GetRichTextStyleValue(StyleAttributeNames.PaddingBottom, null);
			if (reportSize != null)
			{
				padVertical += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)GetRichTextStyleValue(StyleAttributeNames.PaddingLeft, null);
			if (reportSize != null)
			{
				padHorizontal += reportSize.ToMillimeters();
			}
			reportSize = (ReportSize)GetRichTextStyleValue(StyleAttributeNames.PaddingRight, null);
			if (reportSize != null)
			{
				padHorizontal += reportSize.ToMillimeters();
			}
		}

		private void CalculateRichTextElements(PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			TextBoxInstance textBoxInstance = textBox.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				return;
			}
			IEnumerator<ParagraphInstance> enumerator = textBoxInstance.ParagraphInstances.GetEnumerator();
			ParagraphNumberCalculator paragraphNumberCalculator = new ParagraphNumberCalculator();
			while (enumerator.MoveNext())
			{
				ParagraphInstance current = enumerator.Current;
				if (m_paragraphs == null)
				{
					m_paragraphs = new List<Paragraph>();
				}
				Paragraph paragraph = null;
				paragraph = ((!current.IsCompiled) ? new Paragraph(current.Definition, pageContext) : new Paragraph(current.Definition, current as CompiledParagraphInstance, pageContext));
				paragraphNumberCalculator.UpdateParagraph(paragraph);
				m_paragraphs.Add(paragraph);
			}
		}

		private bool WriteOriginalValue(BinaryWriter spbifWriter, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return WriteObjectValue(spbifWriter, 34, typeCode, value);
		}

		private bool WriteOriginalValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			if (value == null)
			{
				return false;
			}
			return WriteObjectValue(textBoxProps, typeCode, value);
		}

		private bool WriteObjectValue(BinaryWriter spbifWriter, byte name, TypeCode typeCode, object value)
		{
			bool result = false;
			spbifWriter.Write(name);
			switch (typeCode)
			{
			case TypeCode.Byte:
				spbifWriter.Write((byte)value);
				break;
			case TypeCode.Decimal:
				spbifWriter.Write((decimal)value);
				break;
			case TypeCode.Double:
				spbifWriter.Write((double)value);
				break;
			case TypeCode.Int16:
				spbifWriter.Write((short)value);
				break;
			case TypeCode.Int32:
				spbifWriter.Write((int)value);
				break;
			case TypeCode.Int64:
				spbifWriter.Write((long)value);
				break;
			case TypeCode.SByte:
				spbifWriter.Write((sbyte)value);
				break;
			case TypeCode.Single:
				spbifWriter.Write((float)value);
				break;
			case TypeCode.UInt16:
				spbifWriter.Write((ushort)value);
				break;
			case TypeCode.UInt32:
				spbifWriter.Write((uint)value);
				break;
			case TypeCode.UInt64:
				spbifWriter.Write((ulong)value);
				break;
			case TypeCode.DateTime:
				spbifWriter.Write(((DateTime)value).ToBinary());
				break;
			case TypeCode.Boolean:
				spbifWriter.Write((bool)value);
				break;
			case TypeCode.Char:
				spbifWriter.Write((char)value);
				break;
			default:
				spbifWriter.Write(value.ToString());
				result = true;
				break;
			}
			return result;
		}

		private bool WriteObjectValue(RPLTextBoxProps textBoxProps, TypeCode typeCode, object value)
		{
			bool result = false;
			textBoxProps.OriginalValue = value;
			if ((uint)(typeCode - 3) > 13u)
			{
				result = true;
			}
			return result;
		}

		internal void DelayWriteContent(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				if (m_contentHeightPosition != -1)
				{
					BinaryWriter propertyCacheWriter = pageContext.PropertyCacheWriter;
					long position = propertyCacheWriter.BaseStream.Position;
					propertyCacheWriter.BaseStream.Seek(m_contentHeightPosition - position, SeekOrigin.Current);
					propertyCacheWriter.Write(m_contentHeight);
					BinaryReader propertyCacheReader = pageContext.PropertyCacheReader;
					propertyCacheReader.BaseStream.Seek(m_startTextBoxOffset, SeekOrigin.Begin);
					CopyData(propertyCacheReader, rplWriter, position - m_startTextBoxOffset);
				}
			}
			else
			{
				((RPLTextBoxProps)m_rplElement.ElementProps).ContentHeight = m_contentHeight;
			}
		}

		internal void CopyData(BinaryReader cacheReader, RPLWriter rplWriter, long length)
		{
			byte[] array = new byte[1024];
			while (length >= array.Length)
			{
				cacheReader.Read(array, 0, array.Length);
				rplWriter.BinaryWriter.Write(array, 0, array.Length);
				length -= array.Length;
			}
			if (length > 0)
			{
				cacheReader.Read(array, 0, (int)length);
				rplWriter.BinaryWriter.Write(array, 0, (int)length);
			}
		}

		private void WriteItemToStream(BinaryWriter spbifWriter, RPLWriter rplWriter, long offsetStart, PageContext pageContext)
		{
			spbifWriter.Write((byte)7);
			WriteElementProps(spbifWriter, rplWriter, pageContext, offsetStart + 1);
			List<long> list = null;
			if (!m_isSimple)
			{
				list = WriteRichTextElementProps(spbifWriter, rplWriter, pageContext);
			}
			offsetStart = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)18);
			spbifWriter.Write(m_offset);
			if (list != null)
			{
				spbifWriter.Write(list.Count);
				foreach (long item in list)
				{
					spbifWriter.Write(item);
				}
			}
			else
			{
				spbifWriter.Write(0);
			}
			spbifWriter.Write(byte.MaxValue);
			m_offset = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)254);
			spbifWriter.Write(offsetStart);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			BinaryWriter binaryWriter = rplWriter.BinaryWriter;
			if (binaryWriter != null)
			{
				long num = m_startTextBoxOffset = (m_offset = binaryWriter.BaseStream.Position);
				if (m_calcSizeState == CalcSize.Delay)
				{
					pageContext.CreateCacheStream(num);
					RPLWriter rplWriter2 = new RPLWriter(pageContext.PropertyCacheWriter, rplWriter.Report, rplWriter.TablixRow);
					WriteItemToStream(pageContext.PropertyCacheWriter, rplWriter2, num, pageContext);
				}
				else
				{
					WriteItemToStream(binaryWriter, rplWriter, num, pageContext);
				}
			}
			else
			{
				m_rplElement = new RPLTextBox();
				WriteElementProps(m_rplElement.ElementProps, rplWriter, pageContext);
				if (!m_isSimple)
				{
					WriteRichTextElementProps(rplWriter, pageContext);
				}
			}
		}

		private List<long> WriteRichTextElementProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return null;
			}
			TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				return null;
			}
			List<long> result = null;
			bool flag = HideDuplicate(textBox, textBoxInstance, pageContext);
			if (m_paragraphs != null && !flag)
			{
				result = new List<long>();
				{
					foreach (Paragraph paragraph in m_paragraphs)
					{
						paragraph.WriteItemToStream(rplWriter, pageContext);
						result.Add(paragraph.Offset);
					}
					return result;
				}
			}
			return result;
		}

		private void WriteRichTextElementProps(RPLWriter rplWriter, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return;
			}
			TextBoxInstance textBoxInstance = m_source.Instance as TextBoxInstance;
			if (textBoxInstance == null)
			{
				return;
			}
			RPLTextBox rPLTextBox = m_rplElement as RPLTextBox;
			bool flag = HideDuplicate(textBox, textBoxInstance, pageContext);
			if (m_paragraphs == null || flag)
			{
				return;
			}
			foreach (Paragraph paragraph in m_paragraphs)
			{
				paragraph.WriteItemToStream(rplWriter, pageContext);
				rPLTextBox.AddParagraph(paragraph.RPLElement);
			}
		}

		private TextAlignments GetTextAlignmentValue(Paragraph paragraph, ref bool shared)
		{
			TextAlignments textAlignments = (TextAlignments)paragraph.GetRichTextStyleValue(StyleAttributeNames.TextAlign, ref shared);
			if (textAlignments == TextAlignments.General)
			{
				textAlignments = TextAlignments.Left;
				if (GetAlignmentRight(ref shared))
				{
					textAlignments = TextAlignments.Right;
				}
			}
			return textAlignments;
		}

		private bool GetAlignmentRight(ref bool shared)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.TextBox textBox = m_source as Microsoft.ReportingServices.OnDemandReportRendering.TextBox;
			if (textBox == null)
			{
				return false;
			}
			TypeCode typeCode = textBox.SharedTypeCode;
			if (typeCode == TypeCode.Object)
			{
				shared = false;
				TextBoxInstance textBoxInstance = (TextBoxInstance)textBox.Instance;
				if (textBoxInstance.OriginalValue == null)
				{
					return false;
				}
				typeCode = Type.GetTypeCode(textBoxInstance.OriginalValue.GetType());
			}
			bool result = false;
			if ((uint)(typeCode - 5) <= 11u)
			{
				result = true;
			}
			return result;
		}

		private CanvasFont CreateFontPens(PageContext pageContext, Paragraph paragraph, TextRun textRun, out bool newFont, out bool newFormatString)
		{
			CanvasFont canvasFont = null;
			TextBoxSharedInfo textBoxSharedInfo = null;
			if (pageContext.TextBoxSharedInfo != null)
			{
				textBoxSharedInfo = (TextBoxSharedInfo)pageContext.TextBoxSharedInfo[m_source.ID];
			}
			if (textBoxSharedInfo != null && textBoxSharedInfo.SharedFont != null)
			{
				canvasFont = CreatePartialSharedFont(textBoxSharedInfo.SharedFont, textBoxSharedInfo.SharedState, paragraph, textRun, pageContext, out newFont, out newFormatString);
			}
			else
			{
				int num = 0;
				bool shared = true;
				TextAlignments textAlignmentValue = GetTextAlignmentValue(paragraph, ref shared);
				if (shared)
				{
					num = 2;
				}
				shared = true;
				string family = (string)textRun.GetRichTextStyleValue(StyleAttributeNames.FontFamily, ref shared);
				ReportSize size = (ReportSize)textRun.GetRichTextStyleValue(StyleAttributeNames.FontSize, ref shared);
				FontStyles style = (FontStyles)textRun.GetRichTextStyleValue(StyleAttributeNames.FontStyle, ref shared);
				FontWeights weight = (FontWeights)textRun.GetRichTextStyleValue(StyleAttributeNames.FontWeight, ref shared);
				TextDecorations decoration = (TextDecorations)textRun.GetRichTextStyleValue(StyleAttributeNames.TextDecoration, ref shared);
				if (shared)
				{
					num |= 1;
				}
				shared = true;
				WritingModes writingMode = (WritingModes)GetRichTextStyleValue(StyleAttributeNames.WritingMode, null, ref shared, pageContext.VersionPicker);
				if (shared)
				{
					num |= 8;
				}
				bool flag = shared;
				bool flag2 = shared;
				shared = true;
				VerticalAlignments verticalAlignment = (VerticalAlignments)GetRichTextStyleValue(StyleAttributeNames.VerticalAlign, null, ref shared);
				if (!shared)
				{
					flag = false;
				}
				shared = true;
				Directions direction = (Directions)GetRichTextStyleValue(StyleAttributeNames.Direction, null, ref shared);
				if (!shared)
				{
					flag2 = false;
				}
				if (flag2)
				{
					num |= 0x10;
				}
				if (flag)
				{
					num |= 4;
				}
				canvasFont = new CanvasFont(family, size, style, weight, decoration, textAlignmentValue, verticalAlignment, direction, writingMode);
				if (num > 0)
				{
					if (textBoxSharedInfo == null)
					{
						if (pageContext.TextBoxSharedInfo == null)
						{
							pageContext.TextBoxSharedInfo = new Hashtable();
						}
						pageContext.TextBoxSharedInfo.Add(m_source.ID, new TextBoxSharedInfo(canvasFont, num));
					}
					else
					{
						if (textBoxSharedInfo.SharedFont != null)
						{
							textBoxSharedInfo.SharedFont.Dispose();
							textBoxSharedInfo.SharedFont = null;
						}
						textBoxSharedInfo.SharedFont = canvasFont;
						textBoxSharedInfo.SharedState = num;
					}
					newFont = false;
					newFormatString = false;
				}
				else
				{
					newFont = true;
					newFormatString = true;
				}
			}
			return canvasFont;
		}

		private CanvasFont CreatePartialSharedFont(CanvasFont shareFont, int fontSharedState, Paragraph paragraph, TextRun textRun, PageContext pageContext, out bool newFont, out bool newFormatString)
		{
			newFont = false;
			newFormatString = false;
			if (fontSharedState == 31)
			{
				return shareFont;
			}
			CanvasFont canvasFont = new CanvasFont(shareFont);
			bool flag = true;
			bool setWritingMode = false;
			if ((fontSharedState & 8) == 0)
			{
				setWritingMode = true;
				WritingModes writingMode = (WritingModes)GetRichTextStyleValue(StyleAttributeNames.WritingMode, null, pageContext.VersionPicker);
				canvasFont.SetWritingMode(writingMode);
			}
			if ((fontSharedState & 1) == 0)
			{
				canvasFont.CreateGDIFont((string)textRun.GetRichTextStyleValue(StyleAttributeNames.FontFamily), (ReportSize)textRun.GetRichTextStyleValue(StyleAttributeNames.FontSize), (FontStyles)textRun.GetRichTextStyleValue(StyleAttributeNames.FontStyle), (FontWeights)textRun.GetRichTextStyleValue(StyleAttributeNames.FontWeight), (TextDecorations)textRun.GetRichTextStyleValue(StyleAttributeNames.TextDecoration));
				newFont = true;
			}
			if ((fontSharedState & 2) == 0)
			{
				TextAlignments alignment = (TextAlignments)paragraph.GetRichTextStyleValue(StyleAttributeNames.TextAlign);
				canvasFont.SetTextStringAlignment(alignment, flag);
				flag = false;
			}
			if ((fontSharedState & 4) == 0)
			{
				VerticalAlignments verticalAlignment = (VerticalAlignments)GetRichTextStyleValue(StyleAttributeNames.VerticalAlign, null);
				canvasFont.SetLineStringAlignment(verticalAlignment, flag);
				flag = false;
			}
			if ((fontSharedState & 0x10) == 0)
			{
				Directions direction = (Directions)GetRichTextStyleValue(StyleAttributeNames.Direction, null);
				canvasFont.SetFormatFlags(direction, setWritingMode, flag);
				flag = false;
			}
			newFormatString = !flag;
			return canvasFont;
		}

		internal static void AddNewLinesAtGdiLimits(ref string text)
		{
			if (text == null || text.Length <= 32000)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int num = 0;
			int num2 = text.IndexOf('\n', num);
			while (num2 >= 0)
			{
				flag = true;
				string text2 = text.Substring(num, num2 - num);
				if (text2.Length > 32000)
				{
					InsertNewLines(stringBuilder, text2);
				}
				else
				{
					stringBuilder.Append(text2);
				}
				stringBuilder.Append('\n');
				num = num2 + 1;
				num2 = -1;
				if (num < text.Length)
				{
					num2 = text.IndexOf('\n', num);
				}
			}
			if (num < text.Length)
			{
				string text3 = text;
				if (flag)
				{
					text3 = text.Substring(num, text.Length - num);
				}
				if (text3.Length > 32000)
				{
					InsertNewLines(stringBuilder, text3);
				}
				else
				{
					stringBuilder.Append(text3);
				}
			}
			text = stringBuilder.ToString();
		}

		private static void InsertNewLines(StringBuilder text, string value)
		{
			if (text == null || string.IsNullOrEmpty(value))
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			while (num < value.Length)
			{
				num2 = value.Length - num;
				if (num2 > 32000)
				{
					num2 = 32000;
					if (char.IsHighSurrogate(value[num + num2 - 1]))
					{
						num2--;
					}
					text.Append(value, num, num2);
					text.AppendLine();
					num += num2;
				}
				else
				{
					text.Append(value, num, num2);
					num += num2;
				}
			}
		}
	}
}
