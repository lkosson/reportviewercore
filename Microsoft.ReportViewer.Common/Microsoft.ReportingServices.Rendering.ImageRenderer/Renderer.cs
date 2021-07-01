using Microsoft.ReportingServices.Rendering.HPBProcessing;
using Microsoft.ReportingServices.Rendering.RichText;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace Microsoft.ReportingServices.Rendering.ImageRenderer
{
	internal class Renderer : IDisposable
	{
		internal WriterBase Writer;

		internal RPLReport RplReport;

		internal int SharedItemsCount;

		internal string CurrentLanguage;

		internal bool PhysicalPagination;

		private int m_pageNumber;

		internal Dictionary<string, int> SharedItems = new Dictionary<string, int>(50);

		private Dictionary<string, float> m_cachedReportSizes = new Dictionary<string, float>();

		private Dictionary<string, float> m_cachedFontSizes = new Dictionary<string, float>();

		private Dictionary<string, Color> m_cachedReportColors = new Dictionary<string, Color>();

		private FontCache m_fontCache;

		private SectionItemizedData m_sectionItemizedData;

		private Dictionary<string, List<TextRunItemizedData>> m_pageParagraphsItemizedData;

		private bool m_beginPage;

		internal static Dictionary<string, Bitmap> ImageResources;

		static Renderer()
		{
			var emptyImage = new Bitmap(2, 2);
			ImageResources = new Dictionary<string, Bitmap>(10);
			ImageResources.Add("toggleMinus", emptyImage);
			ImageResources.Add("togglePlus", emptyImage);
			ImageResources.Add("unsorted", emptyImage);
			ImageResources.Add("sortAsc", emptyImage);
			ImageResources.Add("sortDesc", emptyImage);
			ImageResources.Add("InvalidImage", Microsoft.ReportingServices.InvalidImage.Image);
		}

		internal Renderer(bool physicalPagination)
		{
			PhysicalPagination = physicalPagination;
		}

		internal Renderer(bool physicalPagination, FontCache fontCache)
			: this(physicalPagination)
		{
			m_fontCache = fontCache;
		}

		public void Dispose()
		{
			m_fontCache = null;
			if (Writer != null)
			{
				Writer.Dispose();
				Writer = null;
			}
			GC.SuppressFinalize(this);
		}

		private void CalculateUsableReportItemRectangle(RPLElementProps properties, ref RectangleF position)
		{
			GetReportItemPaddingStyleMM(properties, out float paddingLeft, out float paddingTop, out float paddingRight, out float paddingBottom);
			position.X += paddingLeft;
			position.Width -= paddingLeft;
			position.Y += paddingTop;
			position.Height -= paddingTop;
			position.Width -= paddingRight;
			position.Height -= paddingBottom;
		}

		private Color GetCachedReportColorStyle(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(properties, style, ref fromInstance);
			if (string.IsNullOrEmpty(stylePropertyValueString) || string.Compare(stylePropertyValueString, "TRANSPARENT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return Color.Empty;
			}
			if (!m_cachedReportColors.TryGetValue(stylePropertyValueString, out Color value))
			{
				value = new RPLReportColor(stylePropertyValueString).ToColor();
				m_cachedReportColors.Add(stylePropertyValueString, value);
			}
			return value;
		}

		private float GetCachedReportSizeStyleMM(RPLElementProps properties, byte style)
		{
			bool fromInstance = false;
			string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(properties, style, ref fromInstance);
			if (string.IsNullOrEmpty(stylePropertyValueString))
			{
				return float.NaN;
			}
			if (!m_cachedReportSizes.TryGetValue(stylePropertyValueString, out float value))
			{
				value = (float)new RPLReportSize(stylePropertyValueString).ToMillimeters();
				m_cachedReportSizes.Add(stylePropertyValueString, value);
			}
			return value;
		}

		private void GetReportItemPaddingStyleMM(RPLElementProps instanceProperties, out float paddingLeft, out float paddingTop, out float paddingRight, out float paddingBottom)
		{
			paddingLeft = GetCachedReportSizeStyleMM(instanceProperties, 15);
			paddingTop = GetCachedReportSizeStyleMM(instanceProperties, 17);
			paddingRight = GetCachedReportSizeStyleMM(instanceProperties, 16);
			paddingBottom = GetCachedReportSizeStyleMM(instanceProperties, 18);
		}

		private void ProcessBackgroundColorAndImage(RPLElementProps properties, RectangleF position, RectangleF bounds)
		{
			Color cachedReportColorStyle = GetCachedReportColorStyle(properties, 34);
			if (cachedReportColorStyle != Color.Empty)
			{
				Writer.FillRectangle(cachedReportColorStyle, bounds);
			}
			object stylePropertyValueObject = SharedRenderer.GetStylePropertyValueObject(properties, 33);
			if (stylePropertyValueObject != null)
			{
				object stylePropertyValueObject2 = SharedRenderer.GetStylePropertyValueObject(properties, 35);
				RPLFormat.BackgroundRepeatTypes repeat = (stylePropertyValueObject2 != null) ? ((RPLFormat.BackgroundRepeatTypes)stylePropertyValueObject2) : RPLFormat.BackgroundRepeatTypes.Repeat;
				Writer.DrawBackgroundImage((RPLImageData)stylePropertyValueObject, repeat, PointF.Empty, position);
			}
		}

		private List<Operation> ProcessPageBorders(RPLElementStyle style, RectangleF position, RectangleF bounds, ref bool inPageSection, RectangleF fullPageBounds)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(style, 5, RPLFormat.BorderStyles.None);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle2 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 6, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle3 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 8, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle4 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 7, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle5 = SharedRenderer.GetStylePropertyValueBorderStyle(style, 9, stylePropertyValueBorderStyle);
			if (stylePropertyValueBorderStyle2 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle3 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle4 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle5 == RPLFormat.BorderStyles.None)
			{
				return null;
			}
			float reportSizeStyleMM = SharedRenderer.GetReportSizeStyleMM(style, 10);
			float num = SharedRenderer.GetReportSizeStyleMM(style, 11);
			if (float.IsNaN(num) && !float.IsNaN(reportSizeStyleMM))
			{
				num = reportSizeStyleMM;
			}
			float num2 = SharedRenderer.GetReportSizeStyleMM(style, 13);
			if (float.IsNaN(num2) && !float.IsNaN(reportSizeStyleMM))
			{
				num2 = reportSizeStyleMM;
			}
			float num3 = SharedRenderer.GetReportSizeStyleMM(style, 12);
			if (float.IsNaN(num3) && !float.IsNaN(reportSizeStyleMM))
			{
				num3 = reportSizeStyleMM;
			}
			float num4 = SharedRenderer.GetReportSizeStyleMM(style, 14);
			if (float.IsNaN(num4) && !float.IsNaN(reportSizeStyleMM))
			{
				num4 = reportSizeStyleMM;
			}
			if (float.IsNaN(num) && float.IsNaN(num2) && float.IsNaN(num3) && float.IsNaN(num4))
			{
				return null;
			}
			Color reportColorStyle = SharedRenderer.GetReportColorStyle(style, 0);
			Color color = SharedRenderer.GetReportColorStyle(style, 1);
			if (color == Color.Empty && reportColorStyle != Color.Empty)
			{
				color = reportColorStyle;
			}
			Color color2 = SharedRenderer.GetReportColorStyle(style, 3);
			if (color2 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color2 = reportColorStyle;
			}
			Color color3 = SharedRenderer.GetReportColorStyle(style, 2);
			if (color3 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color3 = reportColorStyle;
			}
			Color color4 = SharedRenderer.GetReportColorStyle(style, 4);
			if (color4 == Color.Empty && reportColorStyle != Color.Empty)
			{
				color4 = reportColorStyle;
			}
			if (color == Color.Empty && color2 == Color.Empty && color3 == Color.Empty && color4 == Color.Empty)
			{
				return null;
			}
			if (!inPageSection)
			{
				Writer.BeginPageSection(fullPageBounds);
				inPageSection = true;
			}
			bounds.X = fullPageBounds.X;
			bounds.Y = fullPageBounds.Y;
			position.X = fullPageBounds.X;
			position.Y = fullPageBounds.Y;
			return ProcessBorders(stylePropertyValueBorderStyle2, stylePropertyValueBorderStyle3, stylePropertyValueBorderStyle4, stylePropertyValueBorderStyle5, num, num2, num3, num4, color, color2, color3, color4, position, bounds, renderBorders: false, 0);
		}

		private List<Operation> ProcessBorders(RPLElementProps properties, RectangleF position, RectangleF bounds, bool renderBorders, byte state)
		{
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 5, RPLFormat.BorderStyles.None);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle2 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 6, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle3 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 8, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle4 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 7, stylePropertyValueBorderStyle);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle5 = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 9, stylePropertyValueBorderStyle);
			if (stylePropertyValueBorderStyle2 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle3 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle4 == RPLFormat.BorderStyles.None && stylePropertyValueBorderStyle5 == RPLFormat.BorderStyles.None)
			{
				return null;
			}
			float cachedReportSizeStyleMM = GetCachedReportSizeStyleMM(properties, 10);
			float num = GetCachedReportSizeStyleMM(properties, 11);
			if (float.IsNaN(num) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num = cachedReportSizeStyleMM;
			}
			float num2 = GetCachedReportSizeStyleMM(properties, 13);
			if (float.IsNaN(num2) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num2 = cachedReportSizeStyleMM;
			}
			float num3 = GetCachedReportSizeStyleMM(properties, 12);
			if (float.IsNaN(num3) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num3 = cachedReportSizeStyleMM;
			}
			float num4 = GetCachedReportSizeStyleMM(properties, 14);
			if (float.IsNaN(num4) && !float.IsNaN(cachedReportSizeStyleMM))
			{
				num4 = cachedReportSizeStyleMM;
			}
			if (float.IsNaN(num) && float.IsNaN(num2) && float.IsNaN(num3) && float.IsNaN(num4))
			{
				return null;
			}
			Color cachedReportColorStyle = GetCachedReportColorStyle(properties, 0);
			Color color = GetCachedReportColorStyle(properties, 1);
			if (color == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color = cachedReportColorStyle;
			}
			Color color2 = GetCachedReportColorStyle(properties, 3);
			if (color2 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color2 = cachedReportColorStyle;
			}
			Color color3 = GetCachedReportColorStyle(properties, 2);
			if (color3 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color3 = cachedReportColorStyle;
			}
			Color color4 = GetCachedReportColorStyle(properties, 4);
			if (color4 == Color.Empty && cachedReportColorStyle != Color.Empty)
			{
				color4 = cachedReportColorStyle;
			}
			if (color == Color.Empty && color2 == Color.Empty && color3 == Color.Empty && color4 == Color.Empty)
			{
				return null;
			}
			return ProcessBorders(stylePropertyValueBorderStyle2, stylePropertyValueBorderStyle3, stylePropertyValueBorderStyle4, stylePropertyValueBorderStyle5, num, num2, num3, num4, color, color2, color3, color4, position, bounds, renderBorders, state);
		}

		private List<Operation> ProcessBorders(RPLFormat.BorderStyles borderStyleLeft, RPLFormat.BorderStyles borderStyleTop, RPLFormat.BorderStyles borderStyleRight, RPLFormat.BorderStyles borderStyleBottom, float borderWidthLeft, float borderWidthTop, float borderWidthRight, float borderWidthBottom, Color borderColorLeft, Color borderColorTop, Color borderColorRight, Color borderColorBottom, RectangleF position, RectangleF bounds, bool renderBorders, byte state)
		{
			float num = position.Top;
			float num2 = num;
			float borderTopEdgeUnclipped = 0f;
			float num3 = position.Left;
			float num4 = num3;
			float borderLeftEdgeUnclipped = 0f;
			float num5 = position.Bottom;
			float num6 = num5;
			float borderBottomEdgeUnclipped = 0f;
			float num7 = position.Right;
			float num8 = num7;
			float borderRightEdgeUnclipped = 0f;
			float borderWidthTopUnclipped = borderWidthTop;
			float borderWidthLeftUnclipped = borderWidthLeft;
			float borderWidthBottomUnclipped = borderWidthBottom;
			float borderWidthRightUnclipped = borderWidthRight;
			if (borderStyleLeft == RPLFormat.BorderStyles.Double && borderWidthLeft < 0.5292f)
			{
				borderStyleLeft = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleRight == RPLFormat.BorderStyles.Double && borderWidthRight < 0.5292f)
			{
				borderStyleRight = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleTop == RPLFormat.BorderStyles.Double && borderWidthTop < 0.5292f)
			{
				borderStyleTop = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleBottom == RPLFormat.BorderStyles.Double && borderWidthBottom < 0.5292f)
			{
				borderStyleBottom = RPLFormat.BorderStyles.Solid;
			}
			if (borderStyleTop != 0)
			{
				num2 -= borderWidthTop / 2f;
				borderTopEdgeUnclipped = num2;
				if (num2 < bounds.Top)
				{
					float num9 = bounds.Top - num2;
					borderWidthTop -= num9;
					num += num9 / 2f;
					num2 = bounds.Top;
					if (borderWidthTop <= 0f)
					{
						borderStyleTop = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthTop = 0f;
			}
			if (borderStyleLeft != 0)
			{
				num4 -= borderWidthLeft / 2f;
				borderLeftEdgeUnclipped = num4;
				if (num4 < bounds.Left)
				{
					float num10 = bounds.Left - num4;
					borderWidthLeft -= num10;
					num3 += num10 / 2f;
					num4 = bounds.Left;
					if (borderWidthLeft <= 0f)
					{
						borderStyleLeft = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthLeft = 0f;
			}
			if (borderStyleBottom != 0)
			{
				num6 += borderWidthBottom / 2f;
				borderBottomEdgeUnclipped = num6;
				if (num6 > bounds.Bottom)
				{
					float num11 = num6 - bounds.Bottom;
					borderWidthBottom -= num11;
					num5 -= num11 / 2f;
					num6 = bounds.Bottom;
					if (borderWidthBottom <= 0f)
					{
						borderStyleBottom = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthBottom = 0f;
			}
			if (borderStyleRight != 0)
			{
				num8 += borderWidthRight / 2f;
				borderRightEdgeUnclipped = num8;
				if (num8 > bounds.Right)
				{
					float num12 = num8 - bounds.Right;
					borderWidthRight -= num12;
					num7 -= num12 / 2f;
					num8 = bounds.Right;
					if (borderWidthRight <= 0f)
					{
						borderStyleRight = RPLFormat.BorderStyles.None;
					}
				}
			}
			else
			{
				borderWidthRight = 0f;
			}
			num4 = Math.Max(bounds.Left, num4);
			num8 = Math.Min(bounds.Right, num8);
			num2 = Math.Max(bounds.Top, num2);
			num6 = Math.Min(bounds.Bottom, num6);
			if (borderStyleTop != RPLFormat.BorderStyles.Double && state == 0 && borderStyleTop == borderStyleLeft && borderStyleTop == borderStyleBottom && borderStyleTop == borderStyleRight && borderWidthTop == borderWidthLeft && borderWidthTop == borderWidthBottom && borderWidthTop == borderWidthRight && borderColorTop == borderColorLeft && borderColorTop == borderColorBottom && borderColorTop == borderColorRight)
			{
				RectangleF rectangle = new RectangleF(num3, num, num7 - num3, num5 - num);
				if (renderBorders)
				{
					Writer.DrawRectangle(borderColorTop, borderWidthTop, borderStyleTop, rectangle);
					return null;
				}
				return new List<Operation>(1)
				{
					new DrawRectangleOp(borderColorTop, borderWidthTop, borderStyleTop, rectangle)
				};
			}
			List<Operation> list = null;
			if (!renderBorders)
			{
				list = new List<Operation>(8);
			}
			float halfPixelWidthX = Writer.HalfPixelWidthX;
			float halfPixelWidthY = Writer.HalfPixelWidthY;
			float num13 = Math.Min(halfPixelWidthY, borderWidthTop / 2f);
			float num14 = Math.Min(halfPixelWidthX, borderWidthLeft / 2f);
			float num15 = Math.Min(halfPixelWidthY, borderWidthBottom / 2f);
			float num16 = Math.Min(halfPixelWidthX, borderWidthRight / 2f);
			if (borderStyleTop != 0 && (state & 1) == 0)
			{
				SharedRenderer.ProcessTopBorder(Writer, list, borderWidthTop, borderStyleTop, borderColorTop, borderColorLeft, borderColorRight, num, num2, num4 + num14, num8 - num16, borderTopEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, borderWidthLeft, borderWidthRight, borderWidthTopUnclipped, borderWidthLeftUnclipped, borderWidthRightUnclipped);
			}
			if (borderStyleLeft != 0 && (state & 4) == 0)
			{
				SharedRenderer.ProcessLeftBorder(Writer, list, borderWidthLeft, borderStyleLeft, borderColorLeft, borderColorTop, borderColorBottom, num3, num4, num2 + num13, num6 - num15, borderLeftEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, borderWidthTop, borderWidthBottom, borderWidthLeftUnclipped, borderWidthTopUnclipped, borderWidthBottomUnclipped);
			}
			if (borderStyleBottom != 0 && (state & 2) == 0)
			{
				SharedRenderer.ProcessBottomBorder(Writer, list, borderWidthBottom, borderStyleBottom, borderColorBottom, borderColorLeft, borderColorRight, num5, num6, num4 + num14, num8 - num16, borderBottomEdgeUnclipped, borderLeftEdgeUnclipped, borderRightEdgeUnclipped, borderWidthLeft, borderWidthRight, borderWidthBottomUnclipped, borderWidthLeftUnclipped, borderWidthRightUnclipped);
			}
			if (borderStyleRight != 0 && (state & 8) == 0)
			{
				SharedRenderer.ProcessRightBorder(Writer, list, borderWidthRight, borderStyleRight, borderColorRight, borderColorTop, borderColorBottom, num7, num8, num2 + num13, num6 - num15, borderRightEdgeUnclipped, borderTopEdgeUnclipped, borderBottomEdgeUnclipped, borderWidthTop, borderWidthBottom, borderWidthRightUnclipped, borderWidthTopUnclipped, borderWidthBottomUnclipped);
			}
			return list;
		}

		private void ProcessDynamicImage(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)measurement.Element.ElementProps;
			if (rPLDynamicImageProps.DynamicImageContent != null || rPLDynamicImageProps.DynamicImageContentOffset != -1)
			{
				Writer.DrawDynamicImage(rPLDynamicImageProps.UniqueName, rPLDynamicImageProps.DynamicImageContent, rPLDynamicImageProps.DynamicImageContentOffset, position);
				ProcessImageMapActions(rPLDynamicImageProps.ActionImageMapAreas, rPLDynamicImageProps.UniqueName, position);
			}
		}

		private void ProcessImage(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLImage rPLImage = (RPLImage)measurement.Element;
			RPLImageProps rPLImageProps = (RPLImageProps)rPLImage.ElementProps;
			RPLImagePropsDef definitionProperties = (RPLImagePropsDef)rPLImageProps.Definition;
			CalculateUsableReportItemRectangle(rPLImageProps, ref position);
			if (!(position.Width <= 0f) && !(position.Height <= 0f))
			{
				Writer.DrawImage(position, rPLImage, rPLImageProps, definitionProperties);
				if (rPLImageProps.ActionInfo != null && rPLImageProps.ActionInfo.Actions.Length != 0)
				{
					Writer.ProcessAction(rPLImageProps.UniqueName, rPLImageProps.ActionInfo, position);
				}
				ProcessImageMapActions(rPLImageProps.ActionImageMapAreas, rPLImageProps.UniqueName, position);
			}
		}

		private void ProcessImageMapActions(RPLActionInfoWithImageMap[] imageMap, string uniqueName, RectangleF position)
		{
			if (imageMap != null)
			{
				for (int i = 0; i < imageMap.Length; i++)
				{
					Writer.ProcessAction(uniqueName, imageMap[i], position);
				}
			}
		}

		private bool ProcessLabelAndBookmark(RPLElementProps properties, RectangleF position)
		{
			RPLItemProps rPLItemProps = properties as RPLItemProps;
			RPLItemPropsDef rPLItemPropsDef = rPLItemProps.Definition as RPLItemPropsDef;
			bool result = false;
			string label = rPLItemProps.Label;
			if (string.IsNullOrEmpty(label))
			{
				label = rPLItemPropsDef.Label;
			}
			if (!string.IsNullOrEmpty(label))
			{
				Writer.ProcessLabel(rPLItemProps.UniqueName, label, position.Location);
				result = true;
			}
			string bookmark = rPLItemProps.Bookmark;
			if (string.IsNullOrEmpty(bookmark))
			{
				bookmark = rPLItemPropsDef.Bookmark;
			}
			if (!string.IsNullOrEmpty(bookmark))
			{
				Writer.ProcessBookmark(rPLItemProps.UniqueName, position.Location);
				result = true;
			}
			return result;
		}

		private void ProcessLine(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLLine obj = (RPLLine)measurement.Element;
			RPLLineProps properties = (RPLLineProps)obj.ElementProps;
			RPLLinePropsDef rPLLinePropsDef = (RPLLinePropsDef)obj.ElementProps.Definition;
			Color cachedReportColorStyle = GetCachedReportColorStyle(properties, 0);
			float cachedReportSizeStyleMM = GetCachedReportSizeStyleMM(properties, 10);
			RPLFormat.BorderStyles stylePropertyValueBorderStyle = SharedRenderer.GetStylePropertyValueBorderStyle(properties, 5, RPLFormat.BorderStyles.None);
			if (stylePropertyValueBorderStyle != 0)
			{
				if (!rPLLinePropsDef.Slant)
				{
					Writer.DrawLine(cachedReportColorStyle, cachedReportSizeStyleMM, stylePropertyValueBorderStyle, position.Left, position.Top, position.Right, position.Bottom);
				}
				else
				{
					Writer.DrawLine(cachedReportColorStyle, cachedReportSizeStyleMM, stylePropertyValueBorderStyle, position.Left, position.Bottom, position.Right, position.Top);
				}
			}
		}

		private void ProcessNonTablixContainerReportItems(RPLContainer container, RectangleF bounds)
		{
			if (container.Children == null || container.Children.Length == 0)
			{
				return;
			}
			if (container.Children.Length == 1)
			{
				ProcessReportItem(container.Children[0], bounds, renderBorders: true, RectangleF.Empty, renderStylesOnBounds: false);
				container.Children = null;
				return;
			}
			List<RPLItemMeasurement> list = new List<RPLItemMeasurement>(container.Children.Length);
			for (int i = 0; i < container.Children.Length; i++)
			{
				list.Add(container.Children[i]);
			}
			container.Children = null;
			list.Sort(new ZIndexComparer());
			for (int j = 0; j < list.Count; j++)
			{
				ProcessReportItem(list[j], bounds, renderBorders: true, RectangleF.Empty, renderStylesOnBounds: false);
				list[j] = null;
			}
		}

		internal void ProcessPage(RPLReport rplReport, int pageNumber, FontCache sharedFontCache, List<SectionItemizedData> glyphCache)
		{
			RplReport = rplReport;
			CurrentLanguage = RplReport.Language;
			m_pageNumber = pageNumber;
			m_fontCache = sharedFontCache;
			m_beginPage = true;
			if (RplReport.RPLPaginatedPages.Length == 0 || RplReport.RPLPaginatedPages[0] == null)
			{
				return;
			}
			RPLPageContent rPLPageContent = RplReport.RPLPaginatedPages[0];
			RPLPageLayout pageLayout = rPLPageContent.PageLayout;
			RPLReportSection nextReportSection = rPLPageContent.GetNextReportSection();
			if (nextReportSection == null)
			{
				return;
			}
			float pageWidth;
			float pageHeight;
			RectangleF rectangleF = Writer.CalculatePageBounds(rPLPageContent, out pageWidth, out pageHeight);
			Writer.BeginPage(pageWidth, pageHeight);
			RectangleF rectangleF2 = new RectangleF(0f, 0f, pageWidth, pageHeight);
			Writer.PreProcessPage(nextReportSection.ID, rectangleF2);
			Color backgroundColor = Color.Empty;
			object obj = pageLayout.Style[34];
			if (obj != null)
			{
				backgroundColor = new RPLReportColor((string)obj).ToColor();
			}
			RPLImageData backgroundImage = null;
			RPLFormat.BackgroundRepeatTypes backgroundRepeat = RPLFormat.BackgroundRepeatTypes.Clip;
			object obj2 = pageLayout.Style[33];
			if (obj2 != null)
			{
				backgroundImage = (RPLImageData)obj2;
				object obj3 = pageLayout.Style[35];
				backgroundRepeat = ((obj3 != null) ? ((RPLFormat.BackgroundRepeatTypes)obj3) : RPLFormat.BackgroundRepeatTypes.Repeat);
			}
			List<Operation> list = ProcessPageStyle(backgroundColor, backgroundImage, backgroundRepeat, pageLayout.Style, rectangleF, rectangleF2);
			float num = rectangleF.Top;
			int num2 = 0;
			float width = rectangleF.Width;
			while (nextReportSection != null)
			{
				if (glyphCache != null)
				{
					m_sectionItemizedData = glyphCache[num2];
				}
				float num3 = 0f;
				RectangleF rectangleF3 = Writer.CalculateHeaderBounds(nextReportSection, pageLayout, num, width);
				num3 = nextReportSection.BodyArea.Top;
				if (!PhysicalPagination)
				{
					num += rectangleF3.Height;
					num3 = 0f;
				}
				for (int i = 0; i < nextReportSection.Columns.Length; i++)
				{
					RPLItemMeasurement rPLItemMeasurement = nextReportSection.Columns[i];
					RectangleF rectangleF4 = Writer.CalculateColumnBounds(nextReportSection, pageLayout, rPLItemMeasurement, i, num + num3, nextReportSection.BodyArea.Height, width);
					Writer.BeginPageSection(rectangleF4);
					if (PhysicalPagination)
					{
						if (m_sectionItemizedData != null && m_sectionItemizedData.Columns.Count > i)
						{
							m_pageParagraphsItemizedData = m_sectionItemizedData.Columns[i];
						}
						else
						{
							m_pageParagraphsItemizedData = null;
						}
						rectangleF4.Location = PointF.Empty;
					}
					RectangleF styleBounds = rectangleF4;
					if (PhysicalPagination)
					{
						float num4 = rPLItemMeasurement.Left + rPLItemMeasurement.Width;
						if (rectangleF4.Width > num4)
						{
							rectangleF4.Width = num4;
						}
						float num5 = rPLItemMeasurement.Top + rPLItemMeasurement.Height;
						if (rectangleF4.Height > num5)
						{
							rectangleF4.Height = num5;
						}
					}
					ProcessReportItem(rPLItemMeasurement, rectangleF4, renderBorders: true, styleBounds, renderStylesOnBounds: true);
					Writer.EndPageSection();
				}
				num += nextReportSection.BodyArea.Height + num3;
				if (nextReportSection.Footer != null)
				{
					if (m_sectionItemizedData != null)
					{
						m_pageParagraphsItemizedData = m_sectionItemizedData.HeaderFooter;
					}
					RectangleF rectangleF5 = Writer.CalculateFooterBounds(nextReportSection, pageLayout, num, width);
					Writer.BeginPageSection(rectangleF5);
					if (PhysicalPagination)
					{
						rectangleF5.Location = PointF.Empty;
					}
					ProcessReportItem(nextReportSection.Footer, rectangleF5, renderBorders: true, rectangleF5, renderStylesOnBounds: true);
					Writer.EndPageSection();
					num += rectangleF5.Height;
				}
				if (nextReportSection.Header != null)
				{
					if (m_sectionItemizedData != null)
					{
						m_pageParagraphsItemizedData = m_sectionItemizedData.HeaderFooter;
					}
					Writer.BeginPageSection(rectangleF3);
					if (PhysicalPagination)
					{
						rectangleF3.Location = PointF.Empty;
					}
					ProcessReportItem(nextReportSection.Header, rectangleF3, renderBorders: true, rectangleF3, renderStylesOnBounds: true);
					Writer.EndPageSection();
				}
				m_pageParagraphsItemizedData = null;
				nextReportSection = rPLPageContent.GetNextReportSection();
				num2++;
			}
			Writer.BeginPageSection(rectangleF);
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					list[j].Perform(Writer);
				}
			}
			Writer.EndPageSection();
			Writer.EndPage();
			Writer.PostProcessPage();
			m_sectionItemizedData = null;
		}

		private List<Operation> ProcessPageStyle(Color backgroundColor, RPLImageData backgroundImage, RPLFormat.BackgroundRepeatTypes backgroundRepeat, RPLElementStyle style, RectangleF pageBounds, RectangleF fullPageBounds)
		{
			bool inPageSection = false;
			if ((backgroundColor != Color.Empty && backgroundColor != Color.Transparent) || backgroundImage != null)
			{
				Writer.BeginPageSection(fullPageBounds);
				inPageSection = true;
			}
			if (backgroundColor != Color.Empty && backgroundColor != Color.Transparent)
			{
				Writer.FillRectangle(backgroundColor, pageBounds);
			}
			if (backgroundImage != null)
			{
				Writer.DrawBackgroundImage(backgroundImage, backgroundRepeat, PointF.Empty, pageBounds);
			}
			List<Operation> result = ProcessPageBorders(style, pageBounds, pageBounds, ref inPageSection, fullPageBounds);
			if (inPageSection)
			{
				Writer.EndPageSection();
			}
			return result;
		}

		internal List<Operation> ProcessReportItem(RPLItemMeasurement measurement, RectangleF bounds, bool renderBorders, RectangleF styleBounds, bool renderStylesOnBounds)
		{
			return ProcessReportItem(measurement, bounds, renderBorders, styleBounds, renderStylesOnBounds, hasTablixCellParent: false);
		}

		private List<Operation> ProcessReportItem(RPLItemMeasurement measurement, RectangleF bounds, bool renderBorders, RectangleF styleBounds, bool renderStylesOnBounds, bool hasTablixCellParent)
		{
			List<Operation> result = null;
			RPLElement element = measurement.Element;
			if (element == null)
			{
				return null;
			}
			bool flag = element is RPLBody;
			bool flag2 = element is RPLLine;
			bool flag3 = element is RPLTablix;
			if (!flag2 && (measurement.Width <= 0f || measurement.Height <= 0f))
			{
				return result;
			}
			RPLElementProps elementProps = element.ElementProps;
			RectangleF measurementRectangle = SharedRenderer.GetMeasurementRectangle(measurement, bounds);
			bool hasLabel = ProcessLabelAndBookmark(elementProps, measurementRectangle);
			object state = Writer.PreProcessReportItem(element, elementProps, measurementRectangle, hasLabel);
			if (flag3)
			{
				RPLTablix rPLTablix = (RPLTablix)measurement.Element;
				if (rPLTablix.ColumnWidths != null && rPLTablix.ColumnWidths.Length != 0 && rPLTablix.RowHeights != null && rPLTablix.RowHeights.Length != 0)
				{
					RectangleF position = default(RectangleF);
					position.X = measurementRectangle.X + rPLTablix.ContentLeft;
					position.Y = measurementRectangle.Y + rPLTablix.ContentTop;
					float[] array = new float[rPLTablix.RowHeights.Length];
					float[] array2 = new float[rPLTablix.ColumnWidths.Length];
					for (int i = 1; i < rPLTablix.RowHeights.Length; i++)
					{
						array[i] = array[i - 1] + rPLTablix.RowHeights[i - 1];
					}
					for (int j = 1; j < rPLTablix.ColumnWidths.Length; j++)
					{
						array2[j] = array2[j - 1] + rPLTablix.ColumnWidths[j - 1];
					}
					position.Height = array[array.Length - 1] + rPLTablix.RowHeights[rPLTablix.RowHeights.Length - 1];
					position.Width = array2[array2.Length - 1] + rPLTablix.ColumnWidths[rPLTablix.ColumnWidths.Length - 1];
					if (!hasTablixCellParent)
					{
						measurementRectangle.X += rPLTablix.ContentLeft;
						measurementRectangle.Y += rPLTablix.ContentTop;
						measurementRectangle.Height = position.Height;
						measurementRectangle.Width = position.Width;
					}
					ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
					ProcessTablixContainer(rPLTablix, position, array, array2);
				}
				else
				{
					measurementRectangle.X += rPLTablix.ContentLeft;
					measurementRectangle.Y += rPLTablix.ContentTop;
					ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
				}
			}
			else
			{
				bool flag4 = element is RPLSubReport;
				if (!flag4)
				{
					if (renderStylesOnBounds && styleBounds != RectangleF.Empty)
					{
						ProcessBackgroundColorAndImage(elementProps, styleBounds, styleBounds);
					}
					else
					{
						ProcessBackgroundColorAndImage(elementProps, measurementRectangle, measurementRectangle);
					}
				}
				if (element is RPLTextBox)
				{
					ProcessTextBox(measurement, measurementRectangle);
				}
				else if (element is RPLRectangle || element is RPLHeaderFooter)
				{
					ProcessNonTablixContainerReportItems((RPLContainer)element, measurementRectangle);
				}
				else if (flag)
				{
					ProcessNonTablixContainerReportItems((RPLContainer)element, measurementRectangle);
				}
				else if (flag4)
				{
					string currentLanguage = CurrentLanguage;
					string stylePropertyValueString = SharedRenderer.GetStylePropertyValueString(element.ElementProps, 32);
					if (!string.IsNullOrEmpty(stylePropertyValueString))
					{
						CurrentLanguage = stylePropertyValueString;
					}
					RPLContainer rPLContainer = (RPLContainer)element;
					if (rPLContainer.Children.Length == 1)
					{
						ProcessReportItem(rPLContainer.Children[0], measurementRectangle, renderBorders: true, RectangleF.Empty, renderStylesOnBounds: false);
					}
					else
					{
						float num = 0f;
						for (int k = 0; k < rPLContainer.Children.Length; k++)
						{
							if (rPLContainer.Children[k].Top == 0f)
							{
								rPLContainer.Children[k].Top = num;
							}
							rPLContainer.Children[k].Width = measurement.Width;
							ProcessReportItem(rPLContainer.Children[k], measurementRectangle, renderBorders: true, RectangleF.Empty, renderStylesOnBounds: false);
							num += rPLContainer.Children[k].Height;
							rPLContainer.Children[k] = null;
						}
					}
					CurrentLanguage = currentLanguage;
				}
				else if (element is RPLImage)
				{
					ProcessImage(measurement, measurementRectangle);
				}
				else if (element is RPLChart || element is RPLGaugePanel || element is RPLMap)
				{
					ProcessDynamicImage(measurement, measurementRectangle);
				}
				else if (flag2)
				{
					ProcessLine(measurement, measurementRectangle);
				}
			}
			if (!flag2)
			{
				if (!flag || !renderStylesOnBounds)
				{
					result = ProcessBorders(elementProps, measurementRectangle, bounds, renderBorders, measurement.State);
				}
				else if (renderStylesOnBounds && styleBounds != RectangleF.Empty)
				{
					result = ProcessBorders(elementProps, styleBounds, styleBounds, renderBorders, measurement.State);
				}
			}
			Writer.PostProcessReportItem(state);
			return result;
		}

		private void ProcessTablixContainer(RPLTablix tablix, RectangleF position, float[] rowStarts, float[] columnStarts)
		{
			int[] array = new int[tablix.ColumnWidths.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = int.MaxValue;
			}
			Writer.ProcessFixedHeaders(tablix, position, rowStarts, columnStarts);
			List<Border> list = null;
			List<Border> list2 = null;
			List<Border> list3 = null;
			List<Border> list4 = null;
			int num = 0;
			int num2 = -1;
			RPLTablixRow nextRow;
			while ((nextRow = tablix.GetNextRow()) != null)
			{
				SharedRenderer.CalculateColumnZIndexes(tablix, nextRow, num, array);
				RPLTablixOmittedRow rPLTablixOmittedRow = nextRow as RPLTablixOmittedRow;
				if (rPLTablixOmittedRow != null)
				{
					for (int j = 0; j < rPLTablixOmittedRow.NumCells; j++)
					{
						RPLTablixMemberCell rPLTablixMemberCell = rPLTablixOmittedRow.OmittedHeaders[j];
						array[rPLTablixMemberCell.ColIndex] = Math.Min(array[rPLTablixMemberCell.ColIndex], SharedRenderer.CalculateZIndex(rPLTablixMemberCell));
						if (!string.IsNullOrEmpty(rPLTablixMemberCell.GroupLabel))
						{
							PointF point = new PointF(position.X, position.Y);
							if (rPLTablixMemberCell.ColIndex < columnStarts.Length)
							{
								point.X = columnStarts[rPLTablixMemberCell.ColIndex];
							}
							if (num < rowStarts.Length)
							{
								point.Y = rowStarts[num];
							}
							Writer.ProcessLabel(rPLTablixMemberCell.UniqueName, rPLTablixMemberCell.GroupLabel, point);
						}
					}
					continue;
				}
				if (nextRow.OmittedHeaders != null)
				{
					for (int k = 0; k < nextRow.OmittedHeaders.Count; k++)
					{
						RPLTablixMemberCell rPLTablixMemberCell2 = nextRow.OmittedHeaders[k];
						if (!string.IsNullOrEmpty(rPLTablixMemberCell2.GroupLabel))
						{
							PointF point2 = new PointF(position.X, position.Y);
							if (rPLTablixMemberCell2.ColIndex < columnStarts.Length)
							{
								point2.X = columnStarts[rPLTablixMemberCell2.ColIndex];
							}
							if (num < rowStarts.Length)
							{
								point2.Y = rowStarts[num];
							}
							Writer.ProcessLabel(rPLTablixMemberCell2.UniqueName, rPLTablixMemberCell2.GroupLabel, point2);
						}
					}
				}
				int num3 = int.MaxValue;
				for (int l = 0; l < nextRow.NumCells; l++)
				{
					RPLTablixCell rPLTablixCell = nextRow[l];
					RPLItemMeasurement rPLItemMeasurement = new RPLItemMeasurement();
					rPLItemMeasurement.Element = rPLTablixCell.Element;
					rPLItemMeasurement.Left = columnStarts[rPLTablixCell.ColIndex];
					rPLItemMeasurement.Top = rowStarts[rPLTablixCell.RowIndex];
					rPLItemMeasurement.Width = tablix.GetColumnWidth(rPLTablixCell.ColIndex, rPLTablixCell.ColSpan);
					rPLItemMeasurement.Height = tablix.GetRowHeight(rPLTablixCell.RowIndex, rPLTablixCell.RowSpan);
					rPLItemMeasurement.State = rPLTablixCell.ElementState;
					if (rPLTablixCell.ContentSizes != null)
					{
						rPLItemMeasurement.Top += rPLTablixCell.ContentSizes.Top;
						rPLItemMeasurement.Height -= rPLTablixCell.ContentSizes.Top;
						rPLItemMeasurement.Left += rPLTablixCell.ContentSizes.Left;
						if (rPLTablixCell.ContentSizes.Width != 0f)
						{
							rPLItemMeasurement.Width = rPLTablixCell.ContentSizes.Width;
						}
						else
						{
							rPLItemMeasurement.Width -= rPLTablixCell.ContentSizes.Left;
						}
					}
					RPLTablixMemberCell rPLTablixMemberCell3 = rPLTablixCell as RPLTablixMemberCell;
					if (rPLTablixMemberCell3 != null && !string.IsNullOrEmpty(rPLTablixMemberCell3.GroupLabel))
					{
						Writer.ProcessLabel(rPLTablixMemberCell3.UniqueName, rPLTablixMemberCell3.GroupLabel, new PointF(rPLItemMeasurement.Left + position.Left, rPLItemMeasurement.Top + position.Top));
					}
					List<Operation> list5 = ProcessReportItem(rPLItemMeasurement, position, renderBorders: false, position, renderStylesOnBounds: false, hasTablixCellParent: true);
					if (list5 != null)
					{
						Border border = new Border();
						border.RowIndex = rPLTablixCell.RowIndex;
						border.ColumnIndex = rPLTablixCell.ColIndex;
						border.Operations = list5;
						if (num < tablix.ColumnHeaderRows && rPLTablixCell is RPLTablixCornerCell)
						{
							if (list == null)
							{
								list = new List<Border>();
							}
							list.Add(border);
						}
						else
						{
							if (num3 == int.MaxValue)
							{
								num3 = SharedRenderer.CalculateRowZIndex(nextRow);
							}
							if (num3 == int.MaxValue)
							{
								num3 = num2;
							}
							border.RowZIndex = num3;
							border.ColumnZIndex = array[rPLTablixCell.ColIndex];
							if (rPLTablixMemberCell3 != null)
							{
								if (num < tablix.ColumnHeaderRows)
								{
									if (list2 == null)
									{
										list2 = new List<Border>();
									}
									list2.Add(border);
								}
								else
								{
									border.CompareRowFirst = false;
									if (list3 == null)
									{
										list3 = new List<Border>();
									}
									list3.Add(border);
								}
							}
							else
							{
								if (list4 == null)
								{
									list4 = new List<Border>();
								}
								list4.Add(border);
							}
						}
					}
					nextRow[l] = null;
				}
				num++;
				num2 = num3;
			}
			RenderBorders(list4);
			RenderBorders(list2);
			RenderBorders(list3);
			RenderBorders(list);
		}

		private void RenderBorders(List<Border> borders)
		{
			if (borders == null || borders.Count <= 0)
			{
				return;
			}
			borders.Sort(new ZIndexComparer());
			for (int i = 0; i < borders.Count; i++)
			{
				Border border = borders[i];
				for (int j = 0; j < border.Operations.Count; j++)
				{
					border.Operations[j].Perform(Writer);
				}
			}
		}

		private void ProcessTextBox(RPLItemMeasurement measurement, RectangleF position)
		{
			RPLTextBox rPLTextBox = (RPLTextBox)measurement.Element;
			RectangleF position2 = new RectangleF(position.Location, position.Size);
			RPLTextBoxProps rPLTextBoxProps = (RPLTextBoxProps)rPLTextBox.ElementProps;
			RPLTextBoxPropsDef rPLTextBoxPropsDef = (RPLTextBoxPropsDef)rPLTextBoxProps.Definition;
			string uniqueName = rPLTextBoxProps.UniqueName;
			CalculateUsableReportItemRectangle(rPLTextBoxProps, ref position2);
			if (position2.Width <= 0f || position2.Height <= 0f)
			{
				return;
			}
			if (rPLTextBoxProps.IsToggleParent)
			{
				Writer.ProcessToggle(uniqueName, rPLTextBoxProps.ToggleState, ref position2);
			}
			if (rPLTextBoxPropsDef.CanSort)
			{
				Writer.ProcessSort(uniqueName, rPLTextBoxProps.SortState, ref position2);
			}
			if (rPLTextBoxProps.ActionInfo != null && rPLTextBoxProps.ActionInfo.Actions.Length != 0)
			{
				Writer.ProcessAction(uniqueName, rPLTextBoxProps.ActionInfo, position2);
			}
			PointF offset = new PointF(0f, rPLTextBoxProps.ContentOffset);
			ReportTextBox rptTextBox = new ReportTextBox(rPLTextBoxProps, Writer);
			if (rPLTextBoxPropsDef.IsSimple)
			{
				string value = rPLTextBoxProps.Value;
				if ((measurement.State & 0x40) == 0 && string.IsNullOrEmpty(value))
				{
					value = rPLTextBoxPropsDef.Value;
				}
				if (!string.IsNullOrEmpty(value))
				{
					ReportTextRun reportTextRun = new ReportTextRun(rPLTextBoxProps.Style, m_cachedFontSizes, m_cachedReportColors);
					ReportParagraph reportParagraph = new ReportParagraph(rPLTextBoxProps.Style, rPLTextBoxProps.UniqueName);
					ProcessSimpleTextBox(value, position2, rptTextBox, reportParagraph, reportTextRun, offset);
				}
			}
			else
			{
				ProcessRichTextBox(position2, rPLTextBox, rptTextBox, offset);
			}
		}

		protected virtual void ProcessSimpleTextBox(string value, RectangleF textPosition, ReportTextBox rptTextBox, ReportParagraph reportParagraph, ReportTextRun reportTextRun, PointF offset)
		{
			Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph = new Microsoft.ReportingServices.Rendering.RichText.Paragraph(reportParagraph, 1);
			Microsoft.ReportingServices.Rendering.RichText.TextBox richTextBox = new Microsoft.ReportingServices.Rendering.RichText.TextBox(rptTextBox);
			bool flag = true;
			TextRunItemizedData textRunItemizedData = null;
			if (m_pageParagraphsItemizedData != null)
			{
				List<TextRunItemizedData> value2 = null;
				m_pageParagraphsItemizedData.TryGetValue(reportParagraph.UniqueName, out value2);
				if (value2 != null)
				{
					flag = false;
					textRunItemizedData = value2[0];
				}
			}
			if (textRunItemizedData != null)
			{
				CreateParagraphRuns(value, paragraph, reportTextRun, textRunItemizedData);
			}
			else
			{
				Microsoft.ReportingServices.Rendering.RichText.TextRun item = new Microsoft.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun);
				paragraph.Runs.Add(item);
			}
			richTextBox.Paragraphs = new List<Microsoft.ReportingServices.Rendering.RichText.Paragraph>(1);
			richTextBox.Paragraphs.Add(paragraph);
			if (flag)
			{
				richTextBox.ScriptItemize();
			}
			Writer.CommonGraphics.ExecuteSync(delegate
			{
				Win32DCSafeHandle hdc = Win32DCSafeHandle.Zero;
				try
				{
					Writer.GetHdc(m_beginPage, out hdc, out float dpiX);
					m_beginPage = false;
					FlowContext flowContext = new FlowContext(textPosition.Width, textPosition.Height, wordTrim: true, lineLimit: false);
					if (richTextBox.VerticalText)
					{
						if (textPosition.Y <= 0f)
						{
							flowContext.Height = textPosition.Bottom;
							textPosition.Height = flowContext.Height;
							textPosition.Y = 0f;
							rptTextBox.SpanPages = true;
						}
						if (textPosition.Bottom >= Writer.PageSectionBounds.Height)
						{
							flowContext.Height = Writer.PageSectionBounds.Height - textPosition.Y;
							textPosition.Height = flowContext.Height;
							rptTextBox.SpanPages = true;
						}
					}
					flowContext.ContentOffset = offset.Y;
					flowContext.Updatable = true;
					float height;
					List<Microsoft.ReportingServices.Rendering.RichText.Paragraph> paragraphs = LineBreaker.Flow(richTextBox, hdc, dpiX, m_fontCache, flowContext, keepLines: true, out height);
					Writer.ClipTextboxRectangle(hdc, textPosition);
					Microsoft.ReportingServices.Rendering.RichText.TextBox.Render(richTextBox, paragraphs, hdc, m_fontCache, offset, textPosition, dpiX);
					Writer.UnClipTextboxRectangle(hdc);
				}
				finally
				{
					Writer.ReleaseHdc(release: false);
				}
			});
		}

		protected virtual void ProcessRichTextBox(RectangleF textPosition, RPLTextBox textbox, ReportTextBox rptTextBox, PointF offset)
		{
			List<Microsoft.ReportingServices.Rendering.RichText.Paragraph> list = new List<Microsoft.ReportingServices.Rendering.RichText.Paragraph>();
			RPLParagraph rPLParagraph = null;
			List<TextRunItemizedData> value = null;
			bool flag = true;
			while ((rPLParagraph = textbox.GetNextParagraph()) != null)
			{
				RPLParagraphProps obj = (RPLParagraphProps)rPLParagraph.ElementProps;
				ReportParagraph reportParagraph = new ReportParagraph(obj);
				Microsoft.ReportingServices.Rendering.RichText.Paragraph paragraph = new Microsoft.ReportingServices.Rendering.RichText.Paragraph(reportParagraph, 1);
				if (!obj.FirstLine)
				{
					paragraph.Updated = true;
				}
				if (m_pageParagraphsItemizedData != null)
				{
					m_pageParagraphsItemizedData.TryGetValue(reportParagraph.UniqueName, out value);
					if (value != null)
					{
						flag = false;
					}
				}
				RPLTextRun rPLTextRun = null;
				int num = 0;
				while ((rPLTextRun = rPLParagraph.GetNextTextRun()) != null)
				{
					RPLTextRunProps rPLTextRunProps = (RPLTextRunProps)rPLTextRun.ElementProps;
					string value2 = rPLTextRunProps.Value;
					if (string.IsNullOrEmpty(value2))
					{
						value2 = ((RPLTextRunPropsDef)rPLTextRunProps.Definition).Value;
					}
					ReportTextRun reportTextRun = new ReportTextRun(rPLTextRunProps.Style, rPLTextRunProps.UniqueName, rPLTextRunProps.ActionInfo, m_cachedFontSizes, m_cachedReportColors);
					TextRunItemizedData textRunItemizedData = null;
					if (value != null)
					{
						textRunItemizedData = value[num];
					}
					if (textRunItemizedData != null)
					{
						CreateParagraphRuns(value2, paragraph, reportTextRun, textRunItemizedData);
					}
					else
					{
						Microsoft.ReportingServices.Rendering.RichText.TextRun item = new Microsoft.ReportingServices.Rendering.RichText.TextRun(value2, reportTextRun);
						paragraph.Runs.Add(item);
					}
					num++;
				}
				list.Add(paragraph);
			}
			if (list.Count == 0)
			{
				return;
			}
			Microsoft.ReportingServices.Rendering.RichText.TextBox richTextBox = new Microsoft.ReportingServices.Rendering.RichText.TextBox(rptTextBox);
			richTextBox.Paragraphs = list;
			if (flag)
			{
				richTextBox.ScriptItemize();
			}
			Writer.CommonGraphics.ExecuteSync(delegate
			{
				Win32DCSafeHandle hdc = Win32DCSafeHandle.Zero;
				try
				{
					Writer.GetHdc(m_beginPage, out hdc, out float dpiX);
					m_beginPage = false;
					FlowContext flowContext = new FlowContext(textPosition.Width, textPosition.Height, wordTrim: true, lineLimit: false);
					if (richTextBox.VerticalText)
					{
						if (textPosition.Y <= 0f)
						{
							flowContext.Height = textPosition.Bottom;
							textPosition.Height = flowContext.Height;
							textPosition.Y = 0f;
							rptTextBox.SpanPages = true;
						}
						if (textPosition.Bottom >= Writer.PageSectionBounds.Height)
						{
							flowContext.Height = Writer.PageSectionBounds.Height - textPosition.Y;
							textPosition.Height = flowContext.Height;
							rptTextBox.SpanPages = true;
						}
					}
					flowContext.ContentOffset = offset.Y;
					flowContext.Updatable = true;
					float height;
					List<Microsoft.ReportingServices.Rendering.RichText.Paragraph> list2 = LineBreaker.Flow(richTextBox, hdc, dpiX, m_fontCache, flowContext, keepLines: true, out height);
					if (list2 != null && list2.Count > 0)
					{
						Writer.ClipTextboxRectangle(hdc, textPosition);
						Microsoft.ReportingServices.Rendering.RichText.TextBox.Render(richTextBox, list2, hdc, m_fontCache, offset, textPosition, dpiX);
						Writer.UnClipTextboxRectangle(hdc);
					}
				}
				finally
				{
					Writer.ReleaseHdc(release: false);
				}
			});
		}

		private void CreateParagraphRuns(string value, Microsoft.ReportingServices.Rendering.RichText.Paragraph richTextParagraph, ReportTextRun reportTextRun, TextRunItemizedData textRunItemizedData)
		{
			Microsoft.ReportingServices.Rendering.RichText.TextRun textRun = null;
			if (textRunItemizedData.SplitIndexes != null && textRunItemizedData.SplitIndexes.Count > 0)
			{
				string text = null;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				for (int i = 0; i < textRunItemizedData.SplitIndexes.Count; i++)
				{
					num3 = textRunItemizedData.SplitIndexes[i];
					text = value.Substring(num2, num3 - num2);
					textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun) : new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun, textRunItemizedData.GlyphData[num]));
					num2 = num3;
					num++;
					richTextParagraph.Runs.Add(textRun);
				}
				if (num2 < value.Length)
				{
					num3 = value.Length;
					text = value.Substring(num2, num3 - num2);
					textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun) : new Microsoft.ReportingServices.Rendering.RichText.TextRun(text, reportTextRun, textRunItemizedData.GlyphData[num]));
					richTextParagraph.Runs.Add(textRun);
				}
			}
			else
			{
				textRun = ((textRunItemizedData.GlyphData == null || textRunItemizedData.GlyphData.Count <= 0) ? new Microsoft.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun) : new Microsoft.ReportingServices.Rendering.RichText.TextRun(value, reportTextRun, textRunItemizedData.GlyphData[0]));
				richTextParagraph.Runs.Add(textRun);
			}
		}
	}
}
