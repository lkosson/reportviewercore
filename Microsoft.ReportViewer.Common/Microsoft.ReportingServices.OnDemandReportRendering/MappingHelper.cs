using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal static class MappingHelper
	{
		internal struct MapAreaInfo
		{
			internal string ToolTip;

			internal ImageMapArea.ImageMapAreaShape MapAreaShape;

			internal object Tag;

			internal float[] Coordinates;

			public MapAreaInfo(string toolTip, object tag, ImageMapArea.ImageMapAreaShape mapAreaShape, float[] coordinates)
			{
				ToolTip = toolTip;
				MapAreaShape = mapAreaShape;
				Tag = tag;
				Coordinates = coordinates;
			}
		}

		internal struct ActionParameterInfo
		{
			public string Name;

			public object Value;

			public bool Omit;
		}

		internal class ActionTag
		{
			public Action Action;

			public List<ActionParameterInfo> Parameters = new List<ActionParameterInfo>();
		}

		private static Color m_defaultShadowColor = Color.FromArgb(0, 0, 0, 127);

		internal static Color DefaultBackgroundColor => Color.Empty;

		internal static Color DefaultBorderColor => Color.Black;

		internal static Color DefaultColor => Color.Black;

		internal static string DefaultFontFamily => "Arial";

		internal static float DefaultFontSize => 10f;

		internal static double ConvertToDouble(object value, bool checkForMaxMinValue, bool checkForStringDate)
		{
			bool isDateTime = false;
			return ConvertToDouble(value, checkForMaxMinValue, checkForStringDate, ref isDateTime);
		}

		internal static double ConvertToDouble(object value, bool checkForMaxMinValue, bool checkForStringDate, ref bool isDateTime)
		{
			if (value == null)
			{
				return double.NaN;
			}
			switch (Type.GetTypeCode(value.GetType()))
			{
			case TypeCode.Byte:
				return (int)(byte)value;
			case TypeCode.Char:
				return (int)(char)value;
			case TypeCode.Decimal:
				return decimal.ToDouble((decimal)value);
			case TypeCode.Double:
				return (double)value;
			case TypeCode.Int16:
				return (short)value;
			case TypeCode.Int32:
				return (int)value;
			case TypeCode.Int64:
				return (long)value;
			case TypeCode.SByte:
				return (sbyte)value;
			case TypeCode.Single:
				return (float)value;
			case TypeCode.UInt16:
				return (int)(ushort)value;
			case TypeCode.UInt32:
				return (uint)value;
			case TypeCode.UInt64:
				return (ulong)value;
			case TypeCode.DateTime:
				isDateTime = true;
				return ((DateTime)value).ToOADate();
			case TypeCode.String:
			{
				double result = double.NaN;
				string text = value.ToString().Trim();
				if (checkForStringDate)
				{
					DateTime result2 = DateTime.MinValue;
					if (DateTime.TryParse(text, out result2))
					{
						isDateTime = true;
						return result2.ToOADate();
					}
				}
				if (double.TryParse(text, out result))
				{
					return result;
				}
				if (checkForMaxMinValue)
				{
					if (text == "MaxValue")
					{
						return double.MaxValue;
					}
					if (text == "MinValue")
					{
						return double.MinValue;
					}
				}
				break;
			}
			}
			return double.NaN;
		}

		internal static float[] ConvertCoordinatesToRelative(float[] pixelCoordinates, float width, float height)
		{
			float[] array = new float[pixelCoordinates.Length];
			for (int i = 0; i < array.Length; i += 2)
			{
				array[i] = pixelCoordinates[i] / width * 100f;
				if (i + 1 < array.Length)
				{
					array[i + 1] = pixelCoordinates[i + 1] / height * 100f;
				}
			}
			return array;
		}

		internal static float[] ConvertCoordinatesToRelative(int[] pixelCoordinates, float width, float height)
		{
			float[] array = new float[pixelCoordinates.Length];
			for (int i = 0; i < array.Length; i += 2)
			{
				array[i] = (float)pixelCoordinates[i] / width * 100f;
				if (i + 1 < array.Length)
				{
					array[i + 1] = (float)pixelCoordinates[i + 1] / height * 100f;
				}
			}
			return array;
		}

		private static Action GetActionFromActionInfo(ActionInfo actionInfo)
		{
			if (actionInfo == null)
			{
				return null;
			}
			if (actionInfo.Actions == null)
			{
				return null;
			}
			if (actionInfo.Actions.Count == 0)
			{
				return null;
			}
			return actionInfo.Actions[0];
		}

		private static string EvaluateHref(Action action, out bool isExpression)
		{
			isExpression = false;
			if (action.Hyperlink != null)
			{
				if (!action.Hyperlink.IsExpression)
				{
					if (action.Hyperlink.Value != null)
					{
						return action.Hyperlink.Value.ToString();
					}
				}
				else
				{
					isExpression = true;
					if (action.Instance != null && action.Instance.Hyperlink != null)
					{
						return action.Instance.Hyperlink.ToString();
					}
				}
			}
			else if (action.Drillthrough != null && action.Drillthrough.ReportName != null)
			{
				if (!action.Drillthrough.ReportName.IsExpression)
				{
					if (action.Drillthrough.ReportName.Value != null)
					{
						return action.Drillthrough.ReportName.Value;
					}
				}
				else
				{
					isExpression = true;
					if (action.Drillthrough.Instance != null && action.Drillthrough.Instance.ReportName != null)
					{
						return action.Drillthrough.Instance.ReportName;
					}
				}
			}
			if (action.BookmarkLink != null)
			{
				if (!action.BookmarkLink.IsExpression)
				{
					if (action.BookmarkLink.Value != null)
					{
						return action.BookmarkLink.Value;
					}
				}
				else
				{
					isExpression = true;
					if (action.Instance != null && action.Instance.BookmarkLink != null)
					{
						return action.Instance.BookmarkLink;
					}
				}
			}
			return null;
		}

		private static void EvaluateActionParameters(ActionDrillthrough actionDrillthroughSource, ActionDrillthrough actionDrillthroughDestination)
		{
			if (actionDrillthroughSource.Parameters == null)
			{
				return;
			}
			foreach (Parameter parameter2 in actionDrillthroughSource.Parameters)
			{
				Parameter parameter = actionDrillthroughDestination.CreateParameter(parameter2.Name);
				if (!parameter2.Value.IsExpression)
				{
					parameter.Instance.Value = parameter2.Value.Value;
				}
				else
				{
					parameter.Instance.Value = parameter2.Instance.Value;
				}
				if (!parameter2.Omit.IsExpression)
				{
					parameter.Instance.Omit = parameter2.Omit.Value;
				}
				else
				{
					parameter.Instance.Omit = parameter2.Instance.Omit;
				}
			}
		}

		internal static ActionInfoWithDynamicImageMap CreateActionInfoDynamic(ReportItem reportItem, ActionInfo actionInfo, string toolTip, out string href)
		{
			return CreateActionInfoDynamic(reportItem, actionInfo, toolTip, out href, applyExpression: true);
		}

		internal static ActionInfoWithDynamicImageMap CreateActionInfoDynamic(ReportItem reportItem, ActionInfo actionInfo, string toolTip, out string href, bool applyExpression)
		{
			Action actionFromActionInfo = GetActionFromActionInfo(actionInfo);
			if (actionFromActionInfo == null)
			{
				href = null;
			}
			else
			{
				href = EvaluateHref(actionFromActionInfo, out bool isExpression);
				if (isExpression && !applyExpression)
				{
					href = null;
				}
			}
			bool flag = actionFromActionInfo == null || href == null;
			bool flag2 = string.IsNullOrEmpty(toolTip);
			if (flag && flag2)
			{
				return null;
			}
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap;
			if (!flag)
			{
				actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(reportItem.RenderingContext, reportItem, actionInfo.ReportScope, actionInfo.InstancePath, actionInfo.ROMActionOwner, chartConstructor: true);
				if (actionFromActionInfo.BookmarkLink != null)
				{
					actionInfoWithDynamicImageMap.CreateBookmarkLinkAction().Instance.BookmarkLink = href;
				}
				else if (actionFromActionInfo.Hyperlink != null)
				{
					actionInfoWithDynamicImageMap.CreateHyperlinkAction().Instance.HyperlinkText = href;
				}
				else if (actionFromActionInfo.Drillthrough != null)
				{
					Action action = actionInfoWithDynamicImageMap.CreateDrillthroughAction();
					action.Drillthrough.Instance.ReportName = href;
					EvaluateActionParameters(actionFromActionInfo.Drillthrough, action.Drillthrough);
					_ = action.Drillthrough.Instance.DrillthroughID;
				}
			}
			else
			{
				actionInfoWithDynamicImageMap = new ActionInfoWithDynamicImageMap(reportItem.RenderingContext, reportItem, reportItem.ReportScope, reportItem.ReportItemDef, null, chartConstructor: true);
			}
			return actionInfoWithDynamicImageMap;
		}

		internal static ActionInfoWithDynamicImageMapCollection GetImageMaps(IEnumerable<MapAreaInfo> mapAreaInfoList, ActionInfoWithDynamicImageMapCollection actions, ReportItem reportItem)
		{
			List<ActionInfoWithDynamicImageMap> list = new List<ActionInfoWithDynamicImageMap>();
			bool[] array = new bool[actions.Count];
			foreach (MapAreaInfo mapAreaInfo in mapAreaInfoList)
			{
				int num = AddMapArea(mapAreaInfo, actions, reportItem);
				if (num > -1 && !array[num])
				{
					list.Add(actions[num]);
					array[num] = true;
				}
				else if (!string.IsNullOrEmpty(mapAreaInfo.ToolTip))
				{
					string href;
					ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = CreateActionInfoDynamic(reportItem, null, mapAreaInfo.ToolTip, out href);
					if (actionInfoWithDynamicImageMap != null)
					{
						actionInfoWithDynamicImageMap.CreateImageMapAreaInstance(mapAreaInfo.MapAreaShape, mapAreaInfo.Coordinates, mapAreaInfo.ToolTip);
						list.Add(actionInfoWithDynamicImageMap);
					}
				}
			}
			actions.InternalList.Clear();
			actions.InternalList.AddRange(list);
			if (actions.Count == 0)
			{
				return null;
			}
			return actions;
		}

		private static int AddMapArea(MapAreaInfo mapAreaInfo, ActionInfoWithDynamicImageMapCollection actions, ReportItem reportItem)
		{
			if (mapAreaInfo.Tag == null)
			{
				return -1;
			}
			int num = (int)mapAreaInfo.Tag;
			ActionInfoWithDynamicImageMap actionInfoWithDynamicImageMap = actions.InternalList[num];
			if (actionInfoWithDynamicImageMap.Actions.Count > 0 || !string.IsNullOrEmpty(mapAreaInfo.ToolTip))
			{
				actionInfoWithDynamicImageMap.CreateImageMapAreaInstance(mapAreaInfo.MapAreaShape, mapAreaInfo.Coordinates, mapAreaInfo.ToolTip);
				return num;
			}
			return -1;
		}

		internal static Color GetStyleColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty color = style.Color;
			Color color2 = Color.Black;
			if (!GetColorFromReportColorProperty(color, ref color2))
			{
				ReportColor color3 = styleInstance.Color;
				if (color3 != null)
				{
					color2 = color3.ToColor();
				}
			}
			return color2;
		}

		internal static Color GetStyleBackgroundColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty backgroundColor = style.BackgroundColor;
			Color color = Color.Empty;
			if (!GetColorFromReportColorProperty(backgroundColor, ref color))
			{
				ReportColor backgroundColor2 = styleInstance.BackgroundColor;
				if (backgroundColor2 != null)
				{
					color = backgroundColor2.ToColor();
				}
			}
			return color;
		}

		internal static Color GetStyleBackGradientEndColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty backgroundGradientEndColor = style.BackgroundGradientEndColor;
			Color color = Color.Empty;
			if (!GetColorFromReportColorProperty(backgroundGradientEndColor, ref color))
			{
				ReportColor backgroundGradientEndColor2 = styleInstance.BackgroundGradientEndColor;
				if (backgroundGradientEndColor2 != null)
				{
					color = backgroundGradientEndColor2.ToColor();
				}
			}
			return color;
		}

		internal static Color GetStyleShadowColor(Style style, StyleInstance styleInstance)
		{
			ReportColorProperty shadowColor = style.ShadowColor;
			Color color = m_defaultShadowColor;
			if (!GetColorFromReportColorProperty(shadowColor, ref color))
			{
				ReportColor shadowColor2 = styleInstance.ShadowColor;
				if (shadowColor2 != null)
				{
					color = shadowColor2.ToColor();
				}
			}
			return color;
		}

		internal static BackgroundGradients GetStyleBackGradientType(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<BackgroundGradients> backgroundGradientType = style.BackgroundGradientType;
			if (!backgroundGradientType.IsExpression)
			{
				return backgroundGradientType.Value;
			}
			return styleInstance.BackgroundGradientType;
		}

		internal static BackgroundHatchTypes GetStyleBackgroundHatchType(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<BackgroundHatchTypes> backgroundHatchType = style.BackgroundHatchType;
			if (!backgroundHatchType.IsExpression)
			{
				return backgroundHatchType.Value;
			}
			return styleInstance.BackgroundHatchType;
		}

		internal static int GetStyleShadowOffset(Style style, StyleInstance styleInstance, float dpi)
		{
			ReportSizeProperty shadowOffset = style.ShadowOffset;
			if (!shadowOffset.IsExpression)
			{
				return ToIntPixels(shadowOffset.Value, dpi);
			}
			ReportSize shadowOffset2 = styleInstance.ShadowOffset;
			if (shadowOffset2 != null)
			{
				return ToIntPixels(shadowOffset2, dpi);
			}
			return 0;
		}

		internal static Font GetDefaultFont()
		{
			return new Font(DefaultFontFamily, DefaultFontSize, GetStyleFontStyle(FontStyles.Normal, FontWeights.Normal, TextDecorations.None));
		}

		internal static TextDecorations GetStyleFontTextDecoration(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextDecorations> textDecoration = style.TextDecoration;
			if (IsStylePropertyDefined(textDecoration))
			{
				if (!textDecoration.IsExpression)
				{
					return textDecoration.Value;
				}
				return styleInstance.TextDecoration;
			}
			return TextDecorations.None;
		}

		internal static FontWeights GetStyleFontWeight(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<FontWeights> fontWeight = style.FontWeight;
			if (IsStylePropertyDefined(fontWeight))
			{
				if (!fontWeight.IsExpression)
				{
					return fontWeight.Value;
				}
				return styleInstance.FontWeight;
			}
			return FontWeights.Normal;
		}

		internal static FontStyles GetStyleFontStyle(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<FontStyles> fontStyle = style.FontStyle;
			if (IsStylePropertyDefined(fontStyle))
			{
				if (!fontStyle.IsExpression)
				{
					return fontStyle.Value;
				}
				return styleInstance.FontStyle;
			}
			return FontStyles.Normal;
		}

		internal static float GetStyleFontSize(Style style, StyleInstance styleInstance)
		{
			ReportSizeProperty fontSize = style.FontSize;
			if (IsStylePropertyDefined(fontSize))
			{
				if (!fontSize.IsExpression)
				{
					return (float)fontSize.Value.ToPoints();
				}
				if (styleInstance.FontSize != null)
				{
					ReportSize fontSize2 = styleInstance.FontSize;
					if (fontSize2 != null)
					{
						return (float)fontSize2.ToPoints();
					}
				}
			}
			return DefaultFontSize;
		}

		internal static string GetStyleFontFamily(Style style, StyleInstance styleInstance, string fallbackFont)
		{
			ReportStringProperty fontFamily = style.FontFamily;
			if (IsStylePropertyDefined(fontFamily))
			{
				if (!fontFamily.IsExpression)
				{
					if (fontFamily != null)
					{
						return fontFamily.Value;
					}
				}
				else if (styleInstance.FontFamily != null)
				{
					return styleInstance.FontFamily;
				}
			}
			return fallbackFont;
		}

		internal static FontStyle GetStyleFontStyle(FontStyles style, FontWeights weight, TextDecorations textDecoration)
		{
			FontStyle fontStyle = FontStyle.Regular;
			if (style == FontStyles.Italic)
			{
				fontStyle = FontStyle.Italic;
			}
			if ((uint)(weight - 2) > 3u && (uint)(weight - 6) <= 3u)
			{
				fontStyle |= FontStyle.Bold;
			}
			switch (textDecoration)
			{
			case TextDecorations.LineThrough:
				fontStyle |= FontStyle.Strikeout;
				break;
			case TextDecorations.Underline:
				fontStyle |= FontStyle.Underline;
				break;
			}
			return fontStyle;
		}

		internal static Color GetStyleBorderColor(Border border)
		{
			ReportColorProperty color = border.Color;
			Color color2 = Color.Black;
			if (!GetColorFromReportColorProperty(color, ref color2))
			{
				ReportColor color3 = border.Instance.Color;
				if (color3 != null)
				{
					color2 = color3.ToColor();
				}
			}
			return color2;
		}

		internal static int GetStyleBorderWidth(Border border, float dpi)
		{
			ReportSizeProperty width = border.Width;
			int result = GetDefaultBorderWidth(dpi);
			if (!width.IsExpression)
			{
				if (width.Value != null)
				{
					result = ToIntPixels(width.Value, dpi);
				}
			}
			else
			{
				ReportSize width2 = border.Instance.Width;
				if (width2 != null)
				{
					result = ToIntPixels(width2, dpi);
				}
			}
			return result;
		}

		internal static BorderStyles GetStyleBorderStyle(Border border)
		{
			ReportEnumProperty<BorderStyles> style = border.Style;
			if (!style.IsExpression)
			{
				return style.Value;
			}
			return border.Instance.Style;
		}

		internal static TextAlignments GetStyleTextAlign(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextAlignments> textAlign = style.TextAlign;
			if (!textAlign.IsExpression)
			{
				return textAlign.Value;
			}
			return styleInstance.TextAlign;
		}

		internal static VerticalAlignments GetStyleVerticalAlignment(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<VerticalAlignments> verticalAlign = style.VerticalAlign;
			if (!verticalAlign.IsExpression)
			{
				return verticalAlign.Value;
			}
			return styleInstance.VerticalAlign;
		}

		internal static TextEffects GetStyleTextEffect(Style style, StyleInstance styleInstance)
		{
			ReportEnumProperty<TextEffects> textEffect = style.TextEffect;
			if (!textEffect.IsExpression)
			{
				return textEffect.Value;
			}
			return styleInstance.TextEffect;
		}

		internal static string GetStyleFormat(Style style, StyleInstance styleInstance)
		{
			ReportStringProperty format = style.Format;
			string text = null;
			if (!format.IsExpression)
			{
				if (format.Value != null)
				{
					text = format.Value;
				}
			}
			else if (styleInstance.Format != null)
			{
				text = styleInstance.Format;
			}
			if (text == null)
			{
				return "";
			}
			return text;
		}

		internal static ContentAlignment GetStyleContentAlignment(Style style, StyleInstance styleInstance)
		{
			TextAlignments styleTextAlign = GetStyleTextAlign(style, styleInstance);
			VerticalAlignments styleVerticalAlignment = GetStyleVerticalAlignment(style, styleInstance);
			ContentAlignment result = ContentAlignment.TopLeft;
			switch (styleTextAlign)
			{
			case TextAlignments.Center:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomCenter;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleCenter;
					break;
				default:
					result = ContentAlignment.TopCenter;
					break;
				}
				break;
			case TextAlignments.Right:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomRight;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleRight;
					break;
				default:
					result = ContentAlignment.TopRight;
					break;
				}
				break;
			default:
				switch (styleVerticalAlignment)
				{
				case VerticalAlignments.Bottom:
					result = ContentAlignment.BottomLeft;
					break;
				case VerticalAlignments.Middle:
					result = ContentAlignment.MiddleLeft;
					break;
				}
				break;
			}
			return result;
		}

		internal static bool IsStylePropertyDefined(ReportProperty property)
		{
			if (property != null)
			{
				return property.ExpressionString != null;
			}
			return false;
		}

		internal static bool IsPropertyExpression(ReportProperty property)
		{
			return property?.IsExpression ?? false;
		}

		internal static bool GetColorFromReportColorProperty(ReportColorProperty reportColorProperty, ref Color color)
		{
			if (reportColorProperty.IsExpression || reportColorProperty.Value == null)
			{
				return false;
			}
			color = reportColorProperty.Value.ToColor();
			return true;
		}

		internal static RightToLeft GetStyleDirection(Style style, StyleInstance styleInstance)
		{
			if ((style.Direction.IsExpression ? styleInstance.Direction : style.Direction.Value) == Directions.RTL)
			{
				return RightToLeft.Yes;
			}
			return RightToLeft.No;
		}

		internal static double ToPixels(ReportSize size, float dpi)
		{
			return size.ToInches() * (double)dpi;
		}

		internal static int ToIntPixels(ReportSize size, float dpi)
		{
			return Convert.ToInt32(ToPixels(size, dpi));
		}

		internal static double ToPixels(double value, Unit unit, float dpi)
		{
			switch (unit)
			{
			case Unit.Centimeter:
				value /= 2.54;
				break;
			case Unit.Millimeter:
				value /= 25.4;
				break;
			case Unit.Pica:
				value /= 6.0;
				break;
			case Unit.Point:
				value /= 72.0;
				break;
			}
			return value * (double)dpi;
		}

		internal static int ToIntPixels(double value, Unit unit, float dpi)
		{
			return Convert.ToInt32(ToPixels(value, unit, dpi));
		}

		internal static int GetDefaultBorderWidth(float dpi)
		{
			return (int)Math.Round(0.013888888888888888 * (double)dpi);
		}
	}
}
