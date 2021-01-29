using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class HTMLStyleRenderer : IHtmlRenderer
	{
		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		private Stream m_textWriter;

		private CreateAndRegisterStream m_createAndRegisterStreamCallback;

		private Encoding m_encoding;

		private DeviceInfo m_deviceInfo;

		private byte[] m_stylePrefixIdBytes;

		private UrlWriter m_urlWriter;

		public HTMLStyleRenderer(Microsoft.ReportingServices.OnDemandReportRendering.Report report, CreateAndRegisterStream createAndRegisterStream, DeviceInfo deviceInfo, UrlWriter urlWriter)
		{
			m_report = report;
			m_createAndRegisterStreamCallback = createAndRegisterStream;
			m_deviceInfo = deviceInfo;
			m_stylePrefixIdBytes = Encoding.UTF8.GetBytes(m_deviceInfo.StylePrefixId);
			m_urlWriter = urlWriter;
		}

		protected bool IsSharedStyleProperty(ReportProperty prop)
		{
			if (prop != null)
			{
				return !prop.IsExpression;
			}
			return false;
		}

		private bool BorderInstanceFromROM(Style reportItemStyle, ReportProperty defWidth, ReportProperty defStyle, ReportProperty defColor, HTML4Renderer.Border border)
		{
			StyleAttributeNames style = StyleAttributeNames.BorderWidthTop;
			StyleAttributeNames style2 = StyleAttributeNames.BorderStyleTop;
			StyleAttributeNames style3 = StyleAttributeNames.BorderColorTop;
			switch (border)
			{
			case HTML4Renderer.Border.Bottom:
				style = StyleAttributeNames.BorderWidthBottom;
				style2 = StyleAttributeNames.BorderStyleBottom;
				style3 = StyleAttributeNames.BorderColorBottom;
				break;
			case HTML4Renderer.Border.Left:
				style = StyleAttributeNames.BorderWidthLeft;
				style2 = StyleAttributeNames.BorderStyleLeft;
				style3 = StyleAttributeNames.BorderColorLeft;
				break;
			case HTML4Renderer.Border.Right:
				style = StyleAttributeNames.BorderWidthRight;
				style2 = StyleAttributeNames.BorderStyleRight;
				style3 = StyleAttributeNames.BorderColorRight;
				break;
			}
			ReportProperty reportProperty = reportItemStyle[style2];
			if (IsSharedStyleProperty(reportProperty))
			{
				if (((ReportEnumProperty<BorderStyles>)reportProperty).Value == BorderStyles.None)
				{
					return false;
				}
				if (((ReportEnumProperty<BorderStyles>)reportProperty).Value == BorderStyles.Default)
				{
					reportProperty = defStyle;
				}
			}
			else
			{
				reportProperty = defStyle;
			}
			ReportSizeProperty reportSizeProperty = (ReportSizeProperty)reportItemStyle[style];
			if (reportSizeProperty == null || reportSizeProperty.IsExpression || reportSizeProperty.Value == null)
			{
				reportSizeProperty = (ReportSizeProperty)defWidth;
			}
			ReportColorProperty reportColorProperty = (ReportColorProperty)reportItemStyle[style3];
			if (reportColorProperty == null || reportColorProperty.IsExpression || reportColorProperty.Value == null)
			{
				reportColorProperty = (ReportColorProperty)defColor;
			}
			RenderBorderStyleFromROM(reportSizeProperty, reportProperty, reportColorProperty, border);
			return true;
		}

		private void RenderBorderFromROM(ReportProperty styleAttribute, HTML4Renderer.Border border, HTML4Renderer.BorderAttribute borderAttribute)
		{
			if (styleAttribute != null)
			{
				switch (border)
				{
				case HTML4Renderer.Border.All:
					BorderAllAtribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Bottom:
					BorderBottomAttribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Right:
					BorderRightAttribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Top:
					BorderTopAttribute(borderAttribute);
					break;
				default:
					BorderLeftAttribute(borderAttribute);
					break;
				}
				WriteStream(styleAttribute);
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderBorderFromROM(ReportEnumProperty<BorderStyles> styleAttribute, HTML4Renderer.Border border, HTML4Renderer.BorderAttribute borderAttribute)
		{
			if (styleAttribute != null)
			{
				string value = ROMEnumStrings.GetValue(styleAttribute.Value);
				switch (border)
				{
				case HTML4Renderer.Border.All:
					BorderAllAtribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Bottom:
					BorderBottomAttribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Right:
					BorderRightAttribute(borderAttribute);
					break;
				case HTML4Renderer.Border.Top:
					BorderTopAttribute(borderAttribute);
					break;
				default:
					BorderLeftAttribute(borderAttribute);
					break;
				}
				WriteStream(value);
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderBorderStyleFromROM(ReportProperty width, ReportProperty style, ReportProperty color, HTML4Renderer.Border border)
		{
			if (width == null && color == null && style == null)
			{
				return;
			}
			if (width != null && color != null && style != null)
			{
				string value = ROMEnumStrings.GetValue(((ReportEnumProperty<BorderStyles>)style).Value);
				switch (border)
				{
				case HTML4Renderer.Border.All:
					WriteStream(HTML4Renderer.m_border);
					break;
				case HTML4Renderer.Border.Bottom:
					WriteStream(HTML4Renderer.m_borderBottom);
					break;
				case HTML4Renderer.Border.Left:
					WriteStream(HTML4Renderer.m_borderLeft);
					break;
				case HTML4Renderer.Border.Right:
					WriteStream(HTML4Renderer.m_borderRight);
					break;
				default:
					WriteStream(HTML4Renderer.m_borderTop);
					break;
				}
				WriteStream(width);
				WriteStream(HTML4Renderer.m_space);
				WriteStream(value);
				WriteStream(HTML4Renderer.m_space);
				WriteStream(color);
				WriteStream(HTML4Renderer.m_semiColon);
			}
			else
			{
				RenderBorderFromROM(color, border, HTML4Renderer.BorderAttribute.BorderColor);
				RenderBorderFromROM((ReportEnumProperty<BorderStyles>)style, border, HTML4Renderer.BorderAttribute.BorderStyle);
				RenderBorderFromROM(width, border, HTML4Renderer.BorderAttribute.BorderWidth);
			}
		}

		private void BorderBottomAttribute(HTML4Renderer.BorderAttribute attribute)
		{
			if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
			{
				WriteStream(HTML4Renderer.m_borderBottomColor);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
			{
				WriteStream(HTML4Renderer.m_borderBottomStyle);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderWidth)
			{
				WriteStream(HTML4Renderer.m_borderBottomWidth);
			}
		}

		private void BorderLeftAttribute(HTML4Renderer.BorderAttribute attribute)
		{
			if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
			{
				WriteStream(HTML4Renderer.m_borderLeftColor);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
			{
				WriteStream(HTML4Renderer.m_borderLeftStyle);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderWidth)
			{
				WriteStream(HTML4Renderer.m_borderLeftWidth);
			}
		}

		private void BorderRightAttribute(HTML4Renderer.BorderAttribute attribute)
		{
			if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
			{
				WriteStream(HTML4Renderer.m_borderRightColor);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
			{
				WriteStream(HTML4Renderer.m_borderRightStyle);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderWidth)
			{
				WriteStream(HTML4Renderer.m_borderRightWidth);
			}
		}

		private void BorderTopAttribute(HTML4Renderer.BorderAttribute attribute)
		{
			if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
			{
				WriteStream(HTML4Renderer.m_borderTopColor);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
			{
				WriteStream(HTML4Renderer.m_borderTopStyle);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderWidth)
			{
				WriteStream(HTML4Renderer.m_borderTopWidth);
			}
		}

		private void BorderAllAtribute(HTML4Renderer.BorderAttribute attribute)
		{
			if (attribute == HTML4Renderer.BorderAttribute.BorderColor)
			{
				WriteStream(HTML4Renderer.m_borderColor);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderStyle)
			{
				WriteStream(HTML4Renderer.m_borderStyle);
			}
			if (attribute == HTML4Renderer.BorderAttribute.BorderWidth)
			{
				WriteStream(HTML4Renderer.m_borderWidth);
			}
		}

		protected void RenderHtmlBordersFromROM(Style styleProps, ref int borderContext, bool padding)
		{
			if (padding)
			{
				ReportProperty reportProperty = styleProps[StyleAttributeNames.PaddingLeft];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					WriteStream(HTML4Renderer.m_paddingLeft);
					WriteStream(reportProperty);
					WriteStream(HTML4Renderer.m_semiColon);
				}
				reportProperty = styleProps[StyleAttributeNames.PaddingTop];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					WriteStream(HTML4Renderer.m_paddingTop);
					WriteStream(reportProperty);
					WriteStream(HTML4Renderer.m_semiColon);
				}
				reportProperty = styleProps[StyleAttributeNames.PaddingRight];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					WriteStream(HTML4Renderer.m_paddingRight);
					WriteStream(reportProperty);
					WriteStream(HTML4Renderer.m_semiColon);
				}
				reportProperty = styleProps[StyleAttributeNames.PaddingBottom];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					WriteStream(HTML4Renderer.m_paddingBottom);
					WriteStream(reportProperty);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			if (styleProps == null || borderContext == 15)
			{
				return;
			}
			ReportProperty reportProperty2 = styleProps[StyleAttributeNames.BorderWidth];
			ReportProperty reportProperty3 = styleProps[StyleAttributeNames.BorderStyle];
			ReportProperty reportProperty4 = styleProps[StyleAttributeNames.BorderColor];
			reportProperty2 = (IsSharedStyleProperty(reportProperty2) ? reportProperty2 : null);
			reportProperty3 = (IsSharedStyleProperty(reportProperty3) ? reportProperty3 : null);
			reportProperty4 = (IsSharedStyleProperty(reportProperty4) ? reportProperty4 : null);
			if (!OnlyGeneralBorderFromROM(styleProps) || borderContext != 0)
			{
				if (reportProperty3 == null || ((ReportEnumProperty<BorderStyles>)reportProperty3).Value == BorderStyles.None)
				{
					RenderBorderStyleFromROM(reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.All);
				}
				if ((borderContext & 8) == 0 && BorderInstanceFromROM(styleProps, reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.Bottom))
				{
					borderContext |= 8;
				}
				if ((borderContext & 1) == 0 && BorderInstanceFromROM(styleProps, reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.Left))
				{
					borderContext |= 1;
				}
				if ((borderContext & 2) == 0 && BorderInstanceFromROM(styleProps, reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.Right))
				{
					borderContext |= 2;
				}
				if ((borderContext & 4) == 0 && BorderInstanceFromROM(styleProps, reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.Top))
				{
					borderContext |= 4;
				}
				return;
			}
			if (reportProperty3 != null)
			{
				if (((ReportEnumProperty<BorderStyles>)reportProperty3).Value == BorderStyles.None)
				{
					reportProperty3 = null;
				}
				else
				{
					borderContext = 15;
				}
			}
			RenderBorderStyleFromROM(reportProperty2, reportProperty3, reportProperty4, HTML4Renderer.Border.All);
		}

		private bool OnlyGeneralBorderFromROM(Style style)
		{
			bool result = true;
			if (style == null)
			{
				return result;
			}
			if (style.LeftBorder != null || style.RightBorder != null || style.BottomBorder != null || style.TopBorder != null)
			{
				result = false;
			}
			return result;
		}

		private void RenderOpenStyle(string id)
		{
			WriteStream(HTML4Renderer.m_dot);
			WriteStream(m_stylePrefixIdBytes);
			WriteStream(id);
			WriteStream(HTML4Renderer.m_openAccol);
		}

		internal void RenderRichTextBox(TextBox textBox)
		{
			if (textBox.IsSimple)
			{
				return;
			}
			foreach (Paragraph paragraph in textBox.Paragraphs)
			{
				RenderParagraphStyles(paragraph.ID, textBox, paragraph, ParagraphStyleWriter.Mode.All);
				RenderParagraphStyles(paragraph.ID + "l", textBox, paragraph, ParagraphStyleWriter.Mode.ListOnly);
				RenderParagraphStyles(paragraph.ID + "p", textBox, paragraph, ParagraphStyleWriter.Mode.ParagraphOnly);
				foreach (TextRun textRun in paragraph.TextRuns)
				{
					RenderTextRunStyle(textRun.ID, textRun.Style);
				}
			}
		}

		private void RenderTextRunStyle(string id, Style style)
		{
			RenderOpenStyle(id);
			RenderFontProps(null, null, style, null);
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		internal void RenderTextBoxStyleProps(TextBox item, StyleContext styleContext, ref int borderContext, string id, bool usePercent)
		{
			RenderTextBoxStyleProps(item, item.Style, null, null, styleContext, ref borderContext, id, usePercent, TextAlignments.General);
		}

		private double GetInnerContainerWidth(double width, Style style)
		{
			double num = 0.0;
			object obj = style[StyleAttributeNames.PaddingLeft];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += rPLReportSize.ToMillimeters();
			}
			obj = style[StyleAttributeNames.PaddingRight];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += rPLReportSize2.ToMillimeters();
			}
			obj = style[StyleAttributeNames.BorderWidthLeft];
			if (obj == null)
			{
				obj = style[StyleAttributeNames.BorderWidth];
			}
			if (obj != null)
			{
				RPLReportSize rPLReportSize3 = new RPLReportSize(obj as string);
				num += rPLReportSize3.ToMillimeters();
			}
			obj = style[StyleAttributeNames.BorderWidthRight];
			if (obj == null)
			{
				obj = style[StyleAttributeNames.BorderWidth];
			}
			if (obj != null)
			{
				RPLReportSize rPLReportSize4 = new RPLReportSize(obj as string);
				num += rPLReportSize4.ToMillimeters();
			}
			double num2 = width - num;
			if (num2 <= 0.0)
			{
				num2 = 1.0;
			}
			return num2;
		}

		private double GetInnerContainerHeight(double height, Style style)
		{
			double num = 0.0;
			object obj = style[StyleAttributeNames.PaddingTop];
			if (obj != null)
			{
				RPLReportSize rPLReportSize = new RPLReportSize(obj as string);
				num += rPLReportSize.ToMillimeters();
			}
			obj = style[StyleAttributeNames.PaddingBottom];
			if (obj != null)
			{
				RPLReportSize rPLReportSize2 = new RPLReportSize(obj as string);
				num += rPLReportSize2.ToMillimeters();
			}
			obj = style[StyleAttributeNames.BorderWidthTop];
			if (obj == null)
			{
				obj = style[StyleAttributeNames.BorderWidth];
			}
			if (obj != null)
			{
				RPLReportSize rPLReportSize3 = new RPLReportSize(obj as string);
				num += rPLReportSize3.ToMillimeters();
			}
			obj = style[StyleAttributeNames.BorderWidthBottom];
			if (obj == null)
			{
				obj = style[StyleAttributeNames.BorderWidth];
			}
			if (obj != null)
			{
				RPLReportSize rPLReportSize4 = new RPLReportSize(obj as string);
				num += rPLReportSize4.ToMillimeters();
			}
			double num2 = height - num;
			if (num2 <= 0.0)
			{
				num2 = 1.0;
			}
			return num2;
		}

		internal void RenderTextBoxStyleProps(TextBox item, Style styleProps, Style paragraphStyleProps, Style textRunStyleProps, StyleContext styleContext, ref int borderContext, string id, bool usePercent, TextAlignments defaultTextAlign)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			ReportProperty reportProperty = null;
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (item.IsSimple)
			{
				if (paragraphStyleProps == null)
				{
					paragraphStyleProps = item.Paragraphs[0].Style;
				}
				if (textRunStyleProps == null)
				{
					textRunStyleProps = item.Paragraphs[0].TextRuns[0].Style;
				}
			}
			else
			{
				WriteStream(HTML4Renderer.m_fontSize);
				if (!m_deviceInfo.IsBrowserGeckoEngine)
				{
					WriteStream(HTML4Renderer.m_zeroPoint);
				}
				else
				{
					WriteStream(HTML4Renderer.m_smallPoint);
				}
				WriteStream(HTML4Renderer.m_semiColon);
			}
			if (styleContext.StyleOnCell)
			{
				if (styleProps != null)
				{
					if (!styleContext.IgnoreVerticalAlign)
					{
						reportProperty = styleProps[StyleAttributeNames.VerticalAlign];
						if (reportProperty != null && !reportProperty.IsExpression)
						{
							string value = ROMEnumStrings.GetValue(((ReportEnumProperty<VerticalAlignments>)reportProperty).Value);
							if (value != null)
							{
								WriteStream(HTML4Renderer.m_verticalAlign);
								WriteStream(value);
								WriteStream(HTML4Renderer.m_semiColon);
							}
						}
					}
					if (!styleContext.NoBorders)
					{
						RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: true);
						RenderBackgroundColor(styleProps);
						BackgroundImage backgroundImage = styleProps.BackgroundImage;
						RenderBackgroundImage(styleProps, backgroundImage);
					}
				}
				RenderTextAlignStyle(item, paragraphStyleProps, defaultTextAlign);
				if (!m_deviceInfo.IsBrowserIE && styleContext.InTablix)
				{
					double num = item.Width.ToMillimeters();
					RenderMeasurementWidth((float)num);
				}
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			WriteStream(HTML4Renderer.m_wordWrap);
			WriteStream(HTML4Renderer.m_semiColon);
			WriteStream(HTML4Renderer.m_whiteSpacePreWrap);
			WriteStream(HTML4Renderer.m_semiColon);
			if (styleContext.RenderMeasurements && styleProps != null)
			{
				if (usePercent)
				{
					WriteStream(HTML4Renderer.m_styleWidth);
					if (styleContext.InTablix && m_deviceInfo.BrowserMode == BrowserMode.Quirks && m_deviceInfo.IsBrowserIE)
					{
						WriteStream(HTML4Renderer.m_ninetyninepercent);
					}
					else
					{
						WriteStream(HTML4Renderer.m_percent);
					}
					WriteStream(HTML4Renderer.m_semiColon);
					if (item.CanGrow)
					{
						WriteStream(HTML4Renderer.m_overflowXHidden);
					}
					else
					{
						WriteStream(HTML4Renderer.m_styleHeight);
						WriteStream(HTML4Renderer.m_percent);
						WriteStream(HTML4Renderer.m_semiColon);
						WriteStream(HTML4Renderer.m_overflowHidden);
					}
					WriteStream(HTML4Renderer.m_semiColon);
				}
				else
				{
					double num2 = item.Width.ToMillimeters();
					double num3 = item.Height.ToMillimeters();
					if (m_deviceInfo.IsBrowserIE6Or7StandardsMode && !styleContext.NoBorders && !styleContext.InTablix)
					{
						num2 = GetInnerContainerWidth(num2, styleProps);
						num3 = GetInnerContainerHeight(num3, styleProps);
					}
					if (item.CanGrow && item.CanShrink)
					{
						RenderMeasurementWidth((float)num2);
					}
					else
					{
						WriteStream(HTML4Renderer.m_overflowHidden);
						WriteStream(HTML4Renderer.m_semiColon);
						RenderMeasurementStyle((float)num3, (float)num2);
					}
				}
			}
			if (!styleContext.NoBorders && !styleContext.InTablix)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: true);
				RenderBackgroundColor(styleProps);
				BackgroundImage backgroundImage2 = styleProps.BackgroundImage;
				RenderBackgroundImage(styleProps, backgroundImage2);
			}
			if (styleContext.OnlyRenderMeasurementsBackgroundBorders)
			{
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			if (item.CanGrow)
			{
				styleContext.RotationApplied = true;
			}
			RenderFontProps(styleProps, paragraphStyleProps, textRunStyleProps, styleContext);
			RenderTextAlignStyle(item, paragraphStyleProps, defaultTextAlign);
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		private void RenderTextAlignStyle(TextBox item, Style paragraphStyleProps, TextAlignments defaultTextAlign)
		{
			if (paragraphStyleProps == null)
			{
				return;
			}
			ReportProperty reportProperty = paragraphStyleProps[StyleAttributeNames.TextAlign];
			if (reportProperty == null || reportProperty.IsExpression)
			{
				return;
			}
			if (((ReportEnumProperty<TextAlignments>)reportProperty).Value != TextAlignments.General)
			{
				string value = ROMEnumStrings.GetValue(((ReportEnumProperty<TextAlignments>)reportProperty).Value);
				if (value != null)
				{
					WriteStream(HTML4Renderer.m_textAlign);
					WriteStream(value);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			else if (item.SharedTypeCode != TypeCode.Object)
			{
				bool textAlignForType = HTML4Renderer.GetTextAlignForType(item.SharedTypeCode);
				RenderDefaultTextAlignStyle(item, item.Style, textAlignForType);
			}
			else if (defaultTextAlign != TextAlignments.General)
			{
				bool isRightAligned = defaultTextAlign == TextAlignments.Right;
				RenderDefaultTextAlignStyle(item, item.Style, isRightAligned);
			}
		}

		private void RenderDefaultTextAlignStyle(TextBox item, Style styleProps, bool isRightAligned)
		{
			bool flag = isRightAligned;
			ReportProperty reportProperty = styleProps[StyleAttributeNames.Direction];
			if (reportProperty != null)
			{
				if (!reportProperty.IsExpression)
				{
					Directions value = ((ReportEnumProperty<Directions>)reportProperty).Value;
					if (value != Directions.LTR)
					{
						flag = !flag;
					}
					WriteStream(HTML4Renderer.m_textAlign);
					if (flag)
					{
						WriteStream(HTML4Renderer.m_rightValue);
					}
					else
					{
						WriteStream(HTML4Renderer.m_leftValue);
					}
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			else
			{
				WriteStream(HTML4Renderer.m_textAlign);
				if (flag)
				{
					WriteStream(HTML4Renderer.m_rightValue);
				}
				else
				{
					WriteStream(HTML4Renderer.m_leftValue);
				}
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderFontProps(Style textBoxStyleProps, Style paragraphStyleProps, Style textRunStyleProps, StyleContext styleContext)
		{
			if (textRunStyleProps != null)
			{
				ReportProperty reportProperty = textRunStyleProps[StyleAttributeNames.FontStyle];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					string value = ROMEnumStrings.GetValue(((ReportEnumProperty<FontStyles>)reportProperty).Value);
					if (value != null)
					{
						WriteStream(HTML4Renderer.m_fontStyle);
						WriteStream(value);
						WriteStream(HTML4Renderer.m_semiColon);
					}
				}
				string styleProp = GetStyleProp(textRunStyleProps, StyleAttributeNames.FontFamily);
				if (styleProp != null)
				{
					WriteStream(HTML4Renderer.m_fontFamily);
					WriteStream(HTML4Renderer.HandleSpecialFontCharacters(styleProp));
					WriteStream(HTML4Renderer.m_semiColon);
				}
				styleProp = GetStyleProp(textRunStyleProps, StyleAttributeNames.FontSize);
				if (styleProp != null && !reportProperty.IsExpression)
				{
					WriteStream(HTML4Renderer.m_fontSize);
					if (string.Compare(styleProp, "0pt", StringComparison.OrdinalIgnoreCase) != 0 || !m_deviceInfo.IsBrowserGeckoEngine)
					{
						WriteStream(styleProp);
					}
					else
					{
						WriteStream(HTML4Renderer.m_smallPoint);
					}
					WriteStream(HTML4Renderer.m_semiColon);
				}
				reportProperty = textRunStyleProps[StyleAttributeNames.FontWeight];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					string value2 = ROMEnumStrings.GetValue(((ReportEnumProperty<FontWeights>)reportProperty).Value);
					if (value2 != null)
					{
						WriteStream(HTML4Renderer.m_fontWeight);
						WriteStream(value2);
						WriteStream(HTML4Renderer.m_semiColon);
					}
				}
				reportProperty = textRunStyleProps[StyleAttributeNames.TextDecoration];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					string value3 = ROMEnumStrings.GetValue(((ReportEnumProperty<TextDecorations>)reportProperty).Value);
					if (value3 != null)
					{
						WriteStream(HTML4Renderer.m_textDecoration);
						WriteStream(value3);
						WriteStream(HTML4Renderer.m_semiColon);
					}
				}
				styleProp = GetStyleProp(textRunStyleProps, StyleAttributeNames.Color);
				if (styleProp != null)
				{
					WriteStream(HTML4Renderer.m_color);
					WriteStream(styleProp);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			RenderOnlyTextBoxProps(textBoxStyleProps, styleContext);
			if (paragraphStyleProps != null)
			{
				string styleProp2 = GetStyleProp(paragraphStyleProps, StyleAttributeNames.LineHeight);
				if (styleProp2 != null)
				{
					WriteStream(HTML4Renderer.m_lineHeight);
					WriteStream(styleProp2);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
		}

		private void RenderOnlyTextBoxProps(Style textBoxStyleProps, StyleContext styleContext)
		{
			if (textBoxStyleProps == null)
			{
				return;
			}
			ReportProperty reportProperty = textBoxStyleProps[StyleAttributeNames.Direction];
			Directions? directions = null;
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				directions = ((ReportEnumProperty<Directions>)reportProperty).Value;
				string value = ROMEnumStrings.GetValue(directions.Value);
				if (value != null)
				{
					WriteStream(HTML4Renderer.m_direction);
					WriteStream(value);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			reportProperty = textBoxStyleProps[StyleAttributeNames.UnicodeBiDi];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				string value2 = ROMEnumStrings.GetValue(((ReportEnumProperty<UnicodeBiDiTypes>)reportProperty).Value);
				if (value2 != null)
				{
					WriteStream(HTML4Renderer.m_unicodeBiDi);
					WriteStream(value2);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			reportProperty = textBoxStyleProps[StyleAttributeNames.WritingMode];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WritingModes value3 = ((ReportEnumProperty<WritingModes>)reportProperty).Value;
				if (directions.HasValue)
				{
					WriteStream(HTML4Renderer.m_writingMode);
					if (value3 == WritingModes.Vertical || value3 == WritingModes.Rotate270)
					{
						if (directions.Value == Directions.RTL)
						{
							WriteStream(HTML4Renderer.m_btrl);
						}
						else
						{
							WriteStream(HTML4Renderer.m_tbrl);
						}
						if (value3 == WritingModes.Rotate270)
						{
							HTML4Renderer.WriteRotate270(m_deviceInfo, styleContext, WriteStream);
						}
					}
					else if (directions.Value == Directions.RTL)
					{
						WriteStream(HTML4Renderer.m_rltb);
					}
					else
					{
						WriteStream(HTML4Renderer.m_lrtb);
					}
					WriteStream(HTML4Renderer.m_semiColon);
				}
				WriteStream(HTML4Renderer.m_layoutFlow);
				if (value3 == WritingModes.Vertical || value3 == WritingModes.Rotate270)
				{
					WriteStream(HTML4Renderer.m_verticalIdeographic);
				}
				else
				{
					WriteStream(HTML4Renderer.m_horizontal);
				}
				WriteStream(HTML4Renderer.m_semiColon);
			}
			if (styleContext == null || styleContext.IgnoreVerticalAlign)
			{
				return;
			}
			reportProperty = textBoxStyleProps[StyleAttributeNames.VerticalAlign];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				string value4 = ROMEnumStrings.GetValue(((ReportEnumProperty<VerticalAlignments>)reportProperty).Value);
				if (value4 != null)
				{
					WriteStream(HTML4Renderer.m_verticalAlign);
					WriteStream(value4);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
		}

		private static ReportSize GetSharedValue(ReportSizeProperty property)
		{
			if (property == null || property.IsExpression)
			{
				return null;
			}
			return property.Value;
		}

		private ReportSize FixHangingIndent(ReportSize leftIndent, ReportSize hangingIndent)
		{
			if (hangingIndent == null)
			{
				return leftIndent;
			}
			double num = hangingIndent.ToMillimeters();
			if (num < 0.0)
			{
				double num2 = 0.0;
				if (leftIndent != null)
				{
					num2 = leftIndent.ToMillimeters();
				}
				num2 -= num;
				leftIndent = ReportSize.FromMillimeters(num2);
			}
			return leftIndent;
		}

		private void FixIndents(ref ReportSize leftIndent, ref ReportSize rightIndent, ref ReportSize spaceBefore, ref ReportSize spaceAfter, ReportSize hangingIndent, bool isRTL, bool isVertical)
		{
			if (isRTL)
			{
				rightIndent = FixHangingIndent(rightIndent, hangingIndent);
			}
			else
			{
				leftIndent = FixHangingIndent(leftIndent, hangingIndent);
			}
			if (isVertical)
			{
				ReportSize reportSize = leftIndent;
				leftIndent = spaceAfter;
				spaceAfter = rightIndent;
				rightIndent = spaceBefore;
				spaceBefore = reportSize;
			}
		}

		private void WriteStyle(byte[] text, object value)
		{
			if (value != null)
			{
				WriteStream(text);
				WriteStream(value.ToString());
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderParagraphStyles(string id, TextBox tb, Paragraph p, ParagraphStyleWriter.Mode paragraphMode)
		{
			Style style = tb.Style;
			Style style2 = p.Style;
			ReportEnumProperty<WritingModes> writingMode = style.WritingMode;
			ReportEnumProperty<Directions> direction = style.Direction;
			bool flag = true;
			if (writingMode != null && writingMode.IsExpression)
			{
				flag = false;
			}
			else if (direction != null && direction.IsExpression)
			{
				flag = false;
			}
			RenderOpenStyle(id);
			if (flag)
			{
				bool isRTL = direction != null && direction.Value == Directions.RTL;
				bool flag2 = writingMode != null && (writingMode.Value == WritingModes.Vertical || writingMode.Value == WritingModes.Rotate270) && m_deviceInfo.IsBrowserIE;
				ReportSize reportSize = GetSharedValue(p.HangingIndent);
				if (reportSize != null && reportSize.ToMillimeters() < 0.0 && p.ListLevel.Value > 0 && !m_deviceInfo.IsBrowserIE)
				{
					reportSize = null;
				}
				ReportSize leftIndent = GetSharedValue(p.LeftIndent);
				ReportSize rightIndent = GetSharedValue(p.RightIndent);
				ReportSize spaceBefore = GetSharedValue(p.SpaceBefore);
				ReportSize spaceAfter = GetSharedValue(p.SpaceAfter);
				if (paragraphMode != ParagraphStyleWriter.Mode.ParagraphOnly)
				{
					FixIndents(ref leftIndent, ref rightIndent, ref spaceBefore, ref spaceAfter, reportSize, isRTL, flag2);
					if (flag2)
					{
						WriteStyle(HTML4Renderer.m_paddingLeft, leftIndent);
					}
					else
					{
						WriteStyle(HTML4Renderer.m_marginLeft, leftIndent);
					}
					WriteStyle(HTML4Renderer.m_marginRight, rightIndent);
					WriteStyle(HTML4Renderer.m_marginTop, spaceBefore);
					if (flag2)
					{
						WriteStyle(HTML4Renderer.m_marginBottom, spaceAfter);
					}
					else
					{
						WriteStyle(HTML4Renderer.m_paddingBottom, spaceAfter);
					}
				}
				WriteStyle(HTML4Renderer.m_textIndent, reportSize);
			}
			if (paragraphMode == ParagraphStyleWriter.Mode.ListOnly)
			{
				WriteStyle(HTML4Renderer.m_fontFamily, "Arial");
				WriteStyle(HTML4Renderer.m_fontSize, "10pt");
			}
			ReportProperty reportProperty = style2[StyleAttributeNames.TextAlign];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				if (((ReportEnumProperty<TextAlignments>)reportProperty).Value != TextAlignments.General)
				{
					string value = ROMEnumStrings.GetValue(((ReportEnumProperty<TextAlignments>)reportProperty).Value);
					if (value != null)
					{
						WriteStream(HTML4Renderer.m_textAlign);
						WriteStream(value);
						WriteStream(HTML4Renderer.m_semiColon);
					}
				}
				else if (tb.SharedTypeCode != TypeCode.Object)
				{
					RenderDefaultTextAlignStyle(tb, tb.Style, isRightAligned: false);
				}
			}
			string styleProp = GetStyleProp(style2, StyleAttributeNames.LineHeight);
			if (styleProp != null)
			{
				WriteStream(HTML4Renderer.m_lineHeight);
				WriteStream(styleProp);
				WriteStream(HTML4Renderer.m_semiColon);
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		internal void RenderPageStyleProps(Style styleProps)
		{
			if (styleProps != null)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProps[StyleAttributeNames.BackgroundColor];
				BackgroundImage backgroundImage = styleProps.BackgroundImage;
				int borderContext = 0;
				if ((reportColorProperty != null && reportColorProperty.Value != null && !reportColorProperty.IsExpression) || (backgroundImage != null && backgroundImage.Value != null && !backgroundImage.Value.IsExpression && backgroundImage.Value.Value != null) || HasBorder(styleProps))
				{
					RenderOpenStyle("p");
					RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
					RenderBackgroundImage(styleProps, backgroundImage);
					RenderBackgroundColor(styleProps);
					WriteStream(HTML4Renderer.m_closeAccol);
				}
			}
		}

		internal bool HasBorder(Style styleProps)
		{
			string styleProp = GetStyleProp(styleProps, StyleAttributeNames.BackgroundColor);
			if (!HasBorder(styleProps, styleProp, StyleAttributeNames.BorderStyle, StyleAttributeNames.BorderColor) && !HasBorder(styleProps, styleProp, StyleAttributeNames.BorderStyleLeft, StyleAttributeNames.BorderColorLeft) && !HasBorder(styleProps, styleProp, StyleAttributeNames.BorderStyleRight, StyleAttributeNames.BorderColorRight) && !HasBorder(styleProps, styleProp, StyleAttributeNames.BorderStyleTop, StyleAttributeNames.BorderColorTop))
			{
				return HasBorder(styleProps, styleProp, StyleAttributeNames.BorderStyleBottom, StyleAttributeNames.BorderColorBottom);
			}
			return true;
		}

		internal bool HasBorder(Style styleProps, string bgcolor, StyleAttributeNames styleName, StyleAttributeNames colorName)
		{
			ReportEnumProperty<BorderStyles> reportEnumProperty = (ReportEnumProperty<BorderStyles>)styleProps[styleName];
			if (reportEnumProperty != null && !reportEnumProperty.IsExpression && reportEnumProperty.Value != BorderStyles.None)
			{
				string styleProp = GetStyleProp(styleProps, colorName);
				if (styleProp != null)
				{
					return styleProp != bgcolor;
				}
				return false;
			}
			return false;
		}

		internal void RenderContainerStyleProps(ReportElement item, Style styleProps, StyleContext styleContext, ref int borderContext, string id, bool usePercent)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (styleContext.StyleOnCell)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				RenderBackgroundColor(styleProps);
				BackgroundImage backgroundImage = styleProps.BackgroundImage;
				RenderBackgroundImage(styleProps, backgroundImage);
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			if (styleContext.RenderMeasurements && !(item is Tablix))
			{
				bool flag = IsCollectionWithoutContent(item as Rectangle);
				if (usePercent)
				{
					RenderPercentSizes();
				}
				else if (item is ReportItem)
				{
					ReportItem reportItem = (ReportItem)item;
					if (!(reportItem is Rectangle))
					{
						RenderMeasurementStyle((float)reportItem.Height.ToMillimeters(), (float)reportItem.Width.ToMillimeters());
					}
					if (flag)
					{
						WriteStream(HTML4Renderer.m_overflowHidden);
						WriteStream(HTML4Renderer.m_semiColon);
					}
				}
			}
			if (!styleContext.InTablix && !styleContext.NoBorders)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				RenderBackgroundColor(styleProps);
				BackgroundImage backgroundImage2 = styleProps.BackgroundImage;
				RenderBackgroundImage(styleProps, backgroundImage2);
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		internal void RenderNoRowsStyleProps(Style styleProps, StyleContext styleContext, ref int borderContext, string id)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (styleContext.StyleOnCell)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				RenderBackgroundColor(styleProps);
				BackgroundImage backgroundImage = styleProps.BackgroundImage;
				RenderBackgroundImage(styleProps, backgroundImage);
				ReportProperty reportProperty = styleProps[StyleAttributeNames.TextAlign];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					string theString = "left";
					if (((ReportEnumProperty<TextAlignments>)reportProperty).Value != TextAlignments.General)
					{
						theString = ROMEnumStrings.GetValue(((ReportEnumProperty<TextAlignments>)reportProperty).Value);
					}
					WriteStream(HTML4Renderer.m_textAlign);
					WriteStream(theString);
					WriteStream(HTML4Renderer.m_semiColon);
				}
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			RenderFontProps(styleProps, styleProps, styleProps, styleContext);
			if (!styleContext.InTablix)
			{
				if (!styleContext.NoBorders)
				{
					RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
					RenderBackgroundColor(styleProps);
					BackgroundImage backgroundImage2 = styleProps.BackgroundImage;
					RenderBackgroundImage(styleProps, backgroundImage2);
				}
				ReportProperty reportProperty2 = styleProps[StyleAttributeNames.TextAlign];
				if (reportProperty2 != null && !reportProperty2.IsExpression)
				{
					string theString2 = "left";
					if (((ReportEnumProperty<TextAlignments>)reportProperty2).Value != TextAlignments.General)
					{
						theString2 = ROMEnumStrings.GetValue(((ReportEnumProperty<TextAlignments>)reportProperty2).Value);
					}
					WriteStream(HTML4Renderer.m_textAlign);
					WriteStream(theString2);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		private void RenderBackgroundColor(Style styleProps)
		{
			string styleProp = GetStyleProp(styleProps, StyleAttributeNames.BackgroundColor);
			if (styleProp != null)
			{
				WriteStream(HTML4Renderer.m_backgroundColor);
				WriteStream(styleProp);
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		private void RenderBackgroundImage(Style styleProps, BackgroundImage bgImage)
		{
			if (bgImage == null)
			{
				return;
			}
			if (bgImage.Value != null && !bgImage.Value.IsExpression && bgImage.Value.Value != null)
			{
				RenderBackgroundImageData(bgImage);
			}
			ReportEnumProperty<BackgroundRepeatTypes> backgroundRepeat = bgImage.BackgroundRepeat;
			if (backgroundRepeat != null && !backgroundRepeat.IsExpression)
			{
				string value = ROMEnumStrings.GetValue(backgroundRepeat.Value);
				if (value != null)
				{
					WriteStream(HTML4Renderer.m_backgroundRepeat);
					WriteStream(value);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
		}

		private void RenderBackgroundImageData(BackgroundImage bgImage)
		{
			RPLImageData rPLImageData = new RPLImageData();
			BackgroundImageInstance instance = bgImage.Instance;
			string text = null;
			if (instance != null)
			{
				text = instance.StreamName;
			}
			if (text != null)
			{
				rPLImageData.ImageName = text;
				if (instance != null)
				{
					rPLImageData.ImageMimeType = instance.MIMEType;
					rPLImageData.ImageData = instance.ImageData;
				}
				WriteStream(HTML4Renderer.m_backgroundImage);
				RenderHTMLImageUrl(rPLImageData);
				WriteStream(HTML4Renderer.m_closeBrace);
				WriteStream(HTML4Renderer.m_semiColon);
			}
		}

		internal void RenderDynamicImageStyleProps(ReportItem item, Style styleProps, StyleContext styleContext, ref int borderContext, string id, bool usePercent)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (styleContext.StyleOnCell)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				RenderBackgroundColor(styleProps);
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			if (styleContext.RenderMeasurements)
			{
				if (usePercent)
				{
					RenderPercentSizes();
				}
				else
				{
					RenderMeasurementStyle((float)item.Height.ToMillimeters(), (float)item.Width.ToMillimeters());
				}
			}
			if (!styleContext.InTablix && !styleContext.NoBorders)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				RenderBackgroundColor(styleProps);
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		internal void RenderImageStyleProps(Image item, Style styleProps, StyleContext styleContext, ref int borderContext, string id, bool usePercent)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (styleContext.StyleOnCell)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: true);
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			if (styleContext.RenderMeasurements)
			{
				if (usePercent)
				{
					RenderPercentSizes();
				}
				else if (item.Sizing != 0)
				{
					double num = item.Height.ToMillimeters();
					double num2 = item.Width.ToMillimeters();
					if (item.Sizing != Image.Sizings.FitProportional && m_deviceInfo.IsBrowserIE6Or7StandardsMode && !styleContext.NoBorders && !styleContext.OnlyRenderMeasurementsBackgroundBorders && !styleContext.InTablix)
					{
						num = GetInnerContainerHeight(num, styleProps);
						num2 = GetInnerContainerWidth(num2, styleProps);
					}
					RenderMeasurementStyle((float)num, (float)num2);
					WriteStream(HTML4Renderer.m_overflowHidden);
					WriteStream(HTML4Renderer.m_semiColon);
				}
			}
			if (!styleContext.InTablix && !styleContext.NoBorders)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: true);
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		internal void RenderLineStyleProps(Line item, Style styleProps, StyleContext styleContext, ref int borderContext, string id, bool usePercent)
		{
			if (styleProps == null)
			{
				return;
			}
			RenderOpenStyle(id);
			if (styleContext.ZeroWidth)
			{
				WriteStream(HTML4Renderer.m_displayNone);
			}
			if (styleContext.StyleOnCell)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
				WriteStream(HTML4Renderer.m_closeAccol);
				return;
			}
			if (styleContext.RenderMeasurements)
			{
				if (usePercent)
				{
					RenderPercentSizes();
				}
				else
				{
					RenderMeasurementStyle((float)item.Height.ToMillimeters(), (float)item.Width.ToMillimeters());
				}
			}
			if (!styleContext.InTablix && !styleContext.NoBorders)
			{
				RenderHtmlBordersFromROM(styleProps, ref borderContext, padding: false);
			}
			WriteStream(HTML4Renderer.m_closeAccol);
		}

		public void WriteStream(byte[] theBytes)
		{
			m_textWriter.Write(theBytes, 0, theBytes.Length);
		}

		protected void WriteStreamLineBreak()
		{
			m_textWriter.Write(HTML4Renderer.m_newLine, 0, HTML4Renderer.m_newLine.Length);
		}

		public bool IsCollectionWithoutContent(Rectangle item)
		{
			if (item != null)
			{
				return item.ReportItemCollection.Count == 0;
			}
			return false;
		}

		private void RenderHTMLImageUrl(RPLImageData image)
		{
			m_createAndRegisterStreamCallback(image.ImageName, string.Empty, null, image.ImageMimeType, willSeek: false, StreamOper.RegisterOnly);
			if (m_urlWriter != null)
			{
				m_urlWriter.WriteImage(m_textWriter, image);
			}
			else
			{
				WriteStream(m_report.GetStreamUrl(useSessionId: false, image.ImageName));
			}
		}

		protected void WriteStream(ReportProperty styleProp)
		{
			if (styleProp == null)
			{
				return;
			}
			if (styleProp is ReportStringProperty)
			{
				ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
				if (reportStringProperty.Value != null)
				{
					WriteStream(reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					WriteStream(reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					WriteStream(reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					WriteStream(reportIntProperty.Value.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		public void WriteStream(string theString)
		{
			if (theString.Length > 0)
			{
				byte[] array = null;
				array = m_encoding.GetBytes(theString);
				m_textWriter.Write(array, 0, array.Length);
			}
		}

		private void RenderPercentSizes()
		{
			WriteStream(HTML4Renderer.m_styleHeight);
			WriteStream(HTML4Renderer.m_percent);
			WriteStream(HTML4Renderer.m_semiColon);
			WriteStream(HTML4Renderer.m_styleWidth);
			WriteStream(HTML4Renderer.m_percent);
			WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void RenderMeasurementStyle(float height, float width)
		{
			RenderMeasurementHeight(height);
			RenderMeasurementWidth(width);
		}

		protected void RenderMeasurementHeight(float height)
		{
			WriteStream(HTML4Renderer.m_styleHeight);
			WriteRSStream(height);
			WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void RenderMeasurementWidth(float width)
		{
			WriteStream(HTML4Renderer.m_styleWidth);
			WriteRSStream(width);
			WriteStream(HTML4Renderer.m_semiColon);
		}

		protected void WriteAttrEncoded(string theString)
		{
			WriteStream(HttpUtility.HtmlAttributeEncode(theString));
		}

		protected void WriteAttrEncoded(ReportProperty styleProp)
		{
			if (styleProp == null)
			{
				return;
			}
			if (styleProp is ReportStringProperty)
			{
				ReportStringProperty reportStringProperty = (ReportStringProperty)styleProp;
				if (reportStringProperty.Value != null)
				{
					WriteAttrEncoded(reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					WriteAttrEncoded(reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					WriteAttrEncoded(reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					WriteAttrEncoded(reportIntProperty.Value.ToString(CultureInfo.InvariantCulture));
				}
			}
		}

		protected void EncodeCSSStyle(string input)
		{
			if (input == null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder(input);
			for (int i = 0; i < stringBuilder.Length; i++)
			{
				if (stringBuilder[i] == '\\' || stringBuilder[i] == '\'' || stringBuilder[i] == '(' || stringBuilder[i] == ')' || stringBuilder[i] == ',')
				{
					stringBuilder.Insert(i, '\\');
					i++;
				}
			}
			WriteStream(stringBuilder.ToString());
		}

		protected void WriteRSStream(float size)
		{
			WriteStream(size.ToString("f2", CultureInfo.InvariantCulture));
			WriteStream(HTML4Renderer.m_mm);
		}

		internal void Render(Stream styleStream)
		{
			m_textWriter = styleStream;
			m_encoding = Encoding.UTF8;
			StyleContext styleContext = new StyleContext();
			RenderPageStyleProps(m_report.ReportSections[0].Page.Style);
			foreach (ReportSection reportSection in m_report.ReportSections)
			{
				PageSection pageHeader = reportSection.Page.PageHeader;
				PageSection pageFooter = reportSection.Page.PageFooter;
				bool flag = false;
				if (pageHeader != null)
				{
					int borderContext = 0;
					styleContext.StyleOnCell = true;
					RenderContainerStyleProps(pageHeader, pageHeader.Style, styleContext, ref borderContext, pageHeader.ID + "c", usePercent: false);
					styleContext.StyleOnCell = false;
					RenderContainerStyleProps(pageHeader, pageHeader.Style, styleContext, ref borderContext, pageHeader.ID, usePercent: false);
					ReportItemCollection reportItemCollection = pageHeader.ReportItemCollection;
					if (reportItemCollection != null)
					{
						for (int i = 0; i < reportItemCollection.Count; i++)
						{
							RenderStylesOnlyRecursive(reportItemCollection[i], new StyleContext());
						}
					}
					flag = true;
				}
				if (pageFooter != null)
				{
					int borderContext2 = 0;
					styleContext.StyleOnCell = true;
					RenderContainerStyleProps(pageFooter, pageFooter.Style, styleContext, ref borderContext2, pageFooter.ID + "c", usePercent: false);
					styleContext.StyleOnCell = false;
					RenderContainerStyleProps(pageFooter, pageFooter.Style, styleContext, ref borderContext2, pageFooter.ID, usePercent: false);
					ReportItemCollection reportItemCollection2 = pageFooter.ReportItemCollection;
					if (reportItemCollection2 != null)
					{
						for (int j = 0; j < reportItemCollection2.Count; j++)
						{
							RenderStylesOnlyRecursive(reportItemCollection2[j], new StyleContext());
						}
					}
					flag = true;
				}
				Body body = reportSection.Body;
				if (body == null)
				{
					continue;
				}
				int borderContext3 = 0;
				styleContext = new StyleContext();
				if (flag)
				{
					styleContext.StyleOnCell = true;
					RenderContainerStyleProps(body, body.Style, styleContext, ref borderContext3, body.ID + "c", usePercent: false);
					styleContext.StyleOnCell = false;
				}
				RenderContainerStyleProps(body, body.Style, styleContext, ref borderContext3, body.ID, usePercent: false);
				ReportItemCollection reportItemCollection3 = body.ReportItemCollection;
				if (reportItemCollection3 != null && reportItemCollection3.Count > 0)
				{
					for (int k = 0; k < reportItemCollection3.Count; k++)
					{
						RenderStylesOnlyRecursive(reportItemCollection3[k], new StyleContext());
					}
				}
			}
			HTML4Renderer.PredefinedStyles(m_deviceInfo, this);
		}

		internal void RenderStylesOnlyRecursive(ReportItem item, StyleContext styleContext)
		{
			int borderContext = 0;
			string iD = item.ID;
			if (item is TextBox)
			{
				TextBox textBox = (TextBox)item;
				bool ignoreVerticalAlign = styleContext.IgnoreVerticalAlign;
				if (textBox.CanGrow)
				{
					styleContext.RenderMeasurements = false;
					if (textBox.CanSort)
					{
						styleContext.StyleOnCell = true;
						RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD + "p", usePercent: false);
						styleContext.StyleOnCell = false;
						styleContext.NoBorders = true;
					}
					if (textBox.CanShrink)
					{
						Directions? directions = null;
						WritingModes? writingModes = null;
						if (textBox.Style.Direction != null && !textBox.Style.Direction.IsExpression)
						{
							directions = textBox.Style.Direction.Value;
						}
						if (textBox.Style.WritingMode != null && !textBox.Style.WritingMode.IsExpression)
						{
							writingModes = textBox.Style.WritingMode.Value;
						}
						if (directions != Directions.RTL && ((writingModes != WritingModes.Vertical && writingModes != WritingModes.Rotate270) || !m_deviceInfo.IsBrowserIE) && !textBox.CanSort)
						{
							styleContext.IgnoreVerticalAlign = true;
						}
					}
				}
				else
				{
					ReportProperty reportProperty = textBox.Style[StyleAttributeNames.VerticalAlign];
					string text = string.Empty;
					if (reportProperty != null && !reportProperty.IsExpression)
					{
						text = ROMEnumStrings.GetValue(((ReportEnumProperty<VerticalAlignments>)reportProperty).Value);
					}
					if (!m_deviceInfo.IsBrowserIE || m_deviceInfo.BrowserMode == BrowserMode.Standards || (!string.IsNullOrEmpty(text) && text != "top") || m_deviceInfo.OutlookCompat)
					{
						if (!textBox.CanSort)
						{
							bool onlyRenderMeasurementsBackgroundBorders = styleContext.OnlyRenderMeasurementsBackgroundBorders;
							styleContext.OnlyRenderMeasurementsBackgroundBorders = true;
							bool noBorders = styleContext.NoBorders;
							int borderContext2 = 0;
							if (textBox.CanShrink)
							{
								styleContext.NoBorders = true;
							}
							styleContext.IgnoreVerticalAlign = true;
							RenderTextBoxStyleProps(textBox, styleContext, ref borderContext2, iD + "v", usePercent: false);
							styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
							styleContext.OnlyRenderMeasurementsBackgroundBorders = onlyRenderMeasurementsBackgroundBorders;
							if (textBox.CanShrink)
							{
								styleContext.NoBorders = noBorders;
							}
							else
							{
								styleContext.NoBorders = true;
							}
						}
					}
					else
					{
						styleContext.IgnoreVerticalAlign = true;
					}
					if (textBox.CanShrink)
					{
						bool noBorders2 = styleContext.NoBorders;
						styleContext.NoBorders = true;
						RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD + "s", usePercent: false);
						styleContext.IgnoreVerticalAlign = true;
						styleContext.NoBorders = noBorders2;
						styleContext.StyleOnCell = true;
					}
					if (textBox.CanSort)
					{
						styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
						RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD + "p", usePercent: false);
						styleContext.StyleOnCell = false;
					}
				}
				if (textBox.CanSort)
				{
					styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
					styleContext.RenderMeasurements = false;
					styleContext.InTablix = true;
				}
				if (textBox.SharedTypeCode == TypeCode.Object)
				{
					ReportEnumProperty<TextAlignments> textAlign = textBox.Style.TextAlign;
					int borderContext3 = borderContext;
					if (textAlign.Value == TextAlignments.General)
					{
						RenderTextBoxStyleProps(textBox, textBox.Style, null, null, styleContext, ref borderContext3, iD + "l", usePercent: true, TextAlignments.Left);
						borderContext3 = borderContext;
						RenderTextBoxStyleProps(textBox, textBox.Style, null, null, styleContext, ref borderContext3, iD + "r", usePercent: true, TextAlignments.Right);
					}
				}
				ActionInfo actionInfo = textBox.ActionInfo;
				if (actionInfo != null && actionInfo.Actions != null && actionInfo.Actions.Count > 0)
				{
					Style style = textBox.Style;
					if (textBox.IsSimple)
					{
						style = textBox.Paragraphs[0].TextRuns[0].Style;
					}
					RenderTextRunStyle(iD + "a", style);
				}
				RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD, usePercent: false);
				styleContext.IgnoreVerticalAlign = ignoreVerticalAlign;
				RenderRichTextBox(textBox);
			}
			else if (item is SubReport)
			{
				styleContext.RenderMeasurements = false;
				int borderContext4 = borderContext;
				RenderNoRowsStyleProps(item.Style, styleContext, ref borderContext, iD + "_NR");
				RenderContainerStyleProps(item, item.Style, styleContext, ref borderContext4, iD, usePercent: false);
				Microsoft.ReportingServices.OnDemandReportRendering.Report report = ((SubReport)item).Report;
				if (report == null)
				{
					return;
				}
				foreach (ReportSection reportSection in report.ReportSections)
				{
					Body body = reportSection.Body;
					RenderContainerStyleProps(body, body.Style, styleContext, ref borderContext, body.ID, usePercent: false);
					ReportItemCollection reportItemCollection = body.ReportItemCollection;
					if (reportItemCollection != null && reportItemCollection.Count > 0)
					{
						for (int i = 0; i < reportItemCollection.Count; i++)
						{
							RenderStylesOnlyRecursive(reportItemCollection[i], new StyleContext());
						}
					}
				}
			}
			else if (item is Rectangle)
			{
				RenderContainerStyleProps(item, item.Style, styleContext, ref borderContext, iD, usePercent: false);
				ReportItemCollection reportItemCollection2 = ((Rectangle)item).ReportItemCollection;
				if (reportItemCollection2 != null && reportItemCollection2.Count > 0)
				{
					for (int j = 0; j < reportItemCollection2.Count; j++)
					{
						RenderStylesOnlyRecursive(reportItemCollection2[j], new StyleContext());
					}
				}
			}
			else if (item is Tablix)
			{
				int borderContext5 = borderContext;
				RenderContainerStyleProps(item, item.Style, styleContext, ref borderContext, iD, usePercent: false);
				RenderNoRowsStyleProps(item.Style, styleContext, ref borderContext5, iD + "_NR");
				Tablix tablix = (Tablix)item;
				TablixCorner corner = tablix.Corner;
				TablixCornerRowCollection rowCollection = corner.RowCollection;
				for (int k = 0; k < rowCollection.Count; k++)
				{
					TablixCornerRow tablixCornerRow = rowCollection[k];
					for (int l = 0; l < tablixCornerRow.Count; l++)
					{
						TablixCornerCell tablixCornerCell = tablixCornerRow[l];
						if (tablixCornerCell != null && tablixCornerCell.CellContents != null)
						{
							RenderStyleForTablixCell(tablixCornerCell.CellContents.ReportItem, styleContext);
						}
					}
				}
				TablixHierarchy rowHierarchy = tablix.RowHierarchy;
				TablixMemberCollection memberCollection = rowHierarchy.MemberCollection;
				for (int m = 0; m < memberCollection.Count; m++)
				{
					RenderTablixMemberRecursive(memberCollection[m], styleContext);
				}
				TablixHierarchy columnHierarchy = tablix.ColumnHierarchy;
				memberCollection = columnHierarchy.MemberCollection;
				for (int n = 0; n < memberCollection.Count; n++)
				{
					RenderTablixMemberRecursive(memberCollection[n], styleContext);
				}
				TablixBody body2 = tablix.Body;
				TablixRowCollection rowCollection2 = body2.RowCollection;
				for (int num = 0; num < rowCollection2.Count; num++)
				{
					TablixRow tablixRow = rowCollection2[num];
					for (int num2 = 0; num2 < tablixRow.Count; num2++)
					{
						TablixCell tablixCell = tablixRow[num2];
						if (tablixCell != null && tablixCell.CellContents != null)
						{
							RenderStyleForTablixCell(tablixCell.CellContents.ReportItem, styleContext);
						}
					}
				}
			}
			else if (item is Chart || item is GaugePanel || item is Map)
			{
				int borderContext6 = borderContext;
				RenderNoRowsStyleProps(item.Style, styleContext, ref borderContext, iD + "_NR");
				if (!styleContext.InTablix && (m_deviceInfo.IsBrowserIE7 || m_deviceInfo.IsBrowserIE6))
				{
					styleContext.RenderMeasurements = false;
					styleContext.RenderMinMeasurements = false;
				}
				RenderDynamicImageStyleProps(item, item.Style, styleContext, ref borderContext6, iD, usePercent: false);
			}
			else if (item is Line)
			{
				RenderLineStyleProps((Line)item, item.Style, styleContext, ref borderContext, iD, usePercent: false);
			}
			else if (item is Image)
			{
				RenderImageStyleProps((Image)item, item.Style, styleContext, ref borderContext, iD, usePercent: false);
			}
		}

		private void RenderTablixMemberRecursive(TablixMember member, StyleContext styleContext)
		{
			styleContext.Reset();
			TablixHeader tablixHeader = member.TablixHeader;
			if (tablixHeader != null)
			{
				RenderStyleForTablixCell(tablixHeader.CellContents.ReportItem, styleContext);
			}
			TablixMemberCollection children = member.Children;
			if (children != null)
			{
				for (int i = 0; i < children.Count; i++)
				{
					RenderTablixMemberRecursive(children[i], styleContext);
				}
			}
		}

		private void RenderStyleForTablixCell(ReportItem cellElement, StyleContext styleContext)
		{
			if (cellElement == null)
			{
				return;
			}
			if (cellElement.Width.ToMillimeters() == 0.0)
			{
				styleContext.ZeroWidth = true;
			}
			string iD = cellElement.ID;
			int borderContext = 0;
			if (cellElement is Line)
			{
				RenderLineStyleProps((Line)cellElement, cellElement.Style, styleContext, ref borderContext, iD, usePercent: true);
			}
			else if (cellElement is Rectangle)
			{
				styleContext.StyleOnCell = true;
				RenderContainerStyleProps(cellElement, cellElement.Style, styleContext, ref borderContext, iD + "c", usePercent: true);
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				styleContext.RenderMeasurements = false;
				RenderStylesOnlyRecursive(cellElement, styleContext);
			}
			else if (cellElement is SubReport)
			{
				styleContext.StyleOnCell = true;
				int borderContext2 = borderContext;
				RenderNoRowsStyleProps(cellElement.Style, styleContext, ref borderContext, iD + "_NRc");
				RenderContainerStyleProps(cellElement, cellElement.Style, styleContext, ref borderContext2, iD + "c", usePercent: true);
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				styleContext.RenderMeasurements = false;
				RenderStylesOnlyRecursive(cellElement, styleContext);
			}
			else if (cellElement is Tablix)
			{
				styleContext.StyleOnCell = true;
				int borderContext3 = borderContext;
				RenderContainerStyleProps(cellElement, cellElement.Style, styleContext, ref borderContext, iD + "c", usePercent: true);
				RenderNoRowsStyleProps(cellElement.Style, styleContext, ref borderContext3, iD + "_NRc");
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				styleContext.RenderMeasurements = false;
				RenderStylesOnlyRecursive(cellElement, styleContext);
			}
			else if (cellElement is TextBox)
			{
				styleContext.StyleOnCell = true;
				TextBox textBox = (TextBox)cellElement;
				if (textBox.SharedTypeCode == TypeCode.Object)
				{
					ReportEnumProperty<TextAlignments> textAlign = cellElement.Style.TextAlign;
					int borderContext4 = borderContext;
					if (textAlign.Value == TextAlignments.General)
					{
						RenderTextBoxStyleProps(textBox, textBox.Style, null, null, styleContext, ref borderContext4, iD + "cl", usePercent: true, TextAlignments.Left);
						borderContext4 = borderContext;
						RenderTextBoxStyleProps(textBox, textBox.Style, null, null, styleContext, ref borderContext4, iD + "cr", usePercent: true, TextAlignments.Right);
					}
				}
				RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD + "c", usePercent: true);
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				if (textBox.CanSort)
				{
					styleContext.RenderMeasurements = false;
				}
				ActionInfo actionInfo = textBox.ActionInfo;
				if (actionInfo != null && actionInfo.Actions != null && actionInfo.Actions.Count > 0)
				{
					Style style = textBox.Style;
					if (textBox.IsSimple)
					{
						style = textBox.Paragraphs[0].TextRuns[0].Style;
					}
					RenderTextRunStyle(iD + "a", style);
				}
				bool usePercent = true;
				ReportProperty reportProperty = textBox.Style[StyleAttributeNames.WritingMode];
				if (reportProperty != null && !reportProperty.IsExpression)
				{
					WritingModes value = ((ReportEnumProperty<WritingModes>)reportProperty).Value;
					if (m_deviceInfo.IsBrowserIE && (value == WritingModes.Vertical || value == WritingModes.Rotate270))
					{
						usePercent = false;
					}
				}
				bool inTablix = styleContext.InTablix;
				styleContext.InTablix = true;
				RenderTextBoxStyleProps(textBox, styleContext, ref borderContext, iD, usePercent);
				styleContext.InTablix = inTablix;
				RenderRichTextBox(textBox);
			}
			else if (cellElement is Chart || cellElement is GaugePanel || cellElement is Map)
			{
				styleContext.StyleOnCell = true;
				int borderContext5 = borderContext;
				RenderNoRowsStyleProps(cellElement.Style, styleContext, ref borderContext, iD + "_NRc");
				RenderDynamicImageStyleProps(cellElement, cellElement.Style, styleContext, ref borderContext5, iD + "c", usePercent: true);
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				RenderNoRowsStyleProps(cellElement.Style, styleContext, ref borderContext, iD + "_NR");
				RenderDynamicImageStyleProps(cellElement, cellElement.Style, styleContext, ref borderContext, iD, usePercent: true);
			}
			else if (cellElement is Image)
			{
				styleContext.StyleOnCell = true;
				RenderImageStyleProps((Image)cellElement, cellElement.Style, styleContext, ref borderContext, iD + "c", usePercent: true);
				styleContext.StyleOnCell = false;
				styleContext.InTablix = true;
				RenderImageStyleProps((Image)cellElement, cellElement.Style, styleContext, ref borderContext, iD, usePercent: true);
			}
		}

		private string GetStyleProp(Style styleProps, StyleAttributeNames name)
		{
			ReportProperty reportProperty = styleProps[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				if (reportProperty is ReportStringProperty)
				{
					ReportStringProperty reportStringProperty = (ReportStringProperty)reportProperty;
					if (reportStringProperty.Value != null)
					{
						return reportStringProperty.Value;
					}
				}
				else if (reportProperty is ReportSizeProperty)
				{
					ReportSizeProperty reportSizeProperty = (ReportSizeProperty)reportProperty;
					if (reportSizeProperty.Value != null)
					{
						return reportSizeProperty.Value.ToString();
					}
				}
				else if (reportProperty is ReportColorProperty)
				{
					ReportColorProperty reportColorProperty = (ReportColorProperty)reportProperty;
					if (reportColorProperty.Value != null)
					{
						return reportColorProperty.Value.ToString();
					}
				}
				else if (reportProperty is ReportIntProperty)
				{
					ReportIntProperty reportIntProperty = (ReportIntProperty)reportProperty;
					if (reportIntProperty.Value > 0)
					{
						return reportIntProperty.Value.ToString(CultureInfo.InvariantCulture);
					}
				}
			}
			return null;
		}
	}
}
