using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Utility
	{
		internal static bool IsBold(RPLFormat.FontWeights fontWeight)
		{
			if (fontWeight - 5 <= RPLFormat.FontWeights.Light)
			{
				return true;
			}
			return false;
		}

		internal static bool IsNullOrEmpty<T>(List<T> list)
		{
			if (list == null || list.Count == 0)
			{
				return true;
			}
			return false;
		}

		internal static void AddInstanceStyles(StyleInstance styleInst, ref Dictionary<byte, object> styles)
		{
			List<StyleAttributeNames> styleAttributes = styleInst.StyleAttributes;
			if (styleAttributes.Count == 0)
			{
				return;
			}
			if (styles == null)
			{
				styles = new Dictionary<byte, object>();
			}
			StyleWriterDictionary writer = new StyleWriterDictionary(styles);
			foreach (StyleAttributeNames item in styleAttributes)
			{
				object obj = styleInst[item];
				if (obj != null)
				{
					SetStyle(ConvertROMTORPL(item), obj, writer);
				}
			}
		}

		internal static void WriteSharedStyles(StyleWriter writeTo, Style styleDef)
		{
			foreach (StyleAttributeNames sharedStyleAttribute in styleDef.SharedStyleAttributes)
			{
				ReportProperty reportProperty = styleDef[sharedStyleAttribute];
				if (reportProperty != null)
				{
					WriteStyleProperty(writeTo, sharedStyleAttribute, reportProperty);
				}
			}
		}

		internal static void WriteStyleProperty(StyleWriter writeTo, Style style, StyleAttributeNames name)
		{
			WriteStyleProperty(writeTo, name, style[name]);
		}

		internal static void WriteStyleProperty(StyleWriter writeTo, StyleAttributeNames styleAtt, ReportProperty prop)
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
				AddStyle(27, prop as ReportColorProperty, writeTo);
				break;
			case StyleAttributeNames.FontFamily:
				AddStyle(20, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.FontSize:
				AddStyle(21, prop as ReportSizeProperty, writeTo);
				break;
			case StyleAttributeNames.FontStyle:
				AddStyle(19, prop, writeTo);
				break;
			case StyleAttributeNames.FontWeight:
				AddStyle(22, prop, writeTo);
				break;
			case StyleAttributeNames.TextDecoration:
				AddStyle(24, prop, writeTo);
				break;
			case StyleAttributeNames.Format:
				AddStyle(23, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.Language:
				AddStyle(32, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.NumeralLanguage:
				AddStyle(36, prop as ReportStringProperty, writeTo);
				break;
			case StyleAttributeNames.NumeralVariant:
				AddStyle(37, prop as ReportIntProperty, writeTo);
				break;
			case StyleAttributeNames.Calendar:
				AddStyle(38, prop, writeTo);
				break;
			case StyleAttributeNames.TextAlign:
				AddStyle(25, prop, writeTo);
				break;
			case StyleAttributeNames.LineHeight:
				AddStyle(28, prop as ReportStringProperty, writeTo);
				break;
			}
		}

		internal static byte ConvertROMTORPL(StyleAttributeNames ROMType)
		{
			switch (ROMType)
			{
			case StyleAttributeNames.Color:
				return 27;
			case StyleAttributeNames.FontFamily:
				return 20;
			case StyleAttributeNames.FontSize:
				return 21;
			case StyleAttributeNames.FontStyle:
				return 19;
			case StyleAttributeNames.FontWeight:
				return 22;
			case StyleAttributeNames.TextDecoration:
				return 24;
			case StyleAttributeNames.Format:
				return 23;
			case StyleAttributeNames.Language:
				return 32;
			case StyleAttributeNames.NumeralLanguage:
				return 36;
			case StyleAttributeNames.NumeralVariant:
				return 37;
			case StyleAttributeNames.Calendar:
				return 38;
			case StyleAttributeNames.TextAlign:
				return 25;
			case StyleAttributeNames.LineHeight:
				return 28;
			default:
				return 0;
			}
		}

		internal static void SetStyle(byte rplId, object styleProp, StyleWriter writer)
		{
			switch (rplId)
			{
			case 33:
				return;
			case 37:
			{
				int num = (int)styleProp;
				if (num > 0)
				{
					writer.Write(rplId, num);
				}
				return;
			}
			}
			bool convertToString = true;
			byte? stylePropByte = PageItem.GetStylePropByte(rplId, styleProp, ref convertToString);
			if (stylePropByte.HasValue)
			{
				writer.Write(rplId, stylePropByte.Value);
			}
			else if (convertToString)
			{
				writer.Write(rplId, styleProp.ToString());
			}
		}

		internal static double GetSizePropertyValue(ReportSizeProperty sizeProp, ReportSize instanceValue)
		{
			if (instanceValue != null)
			{
				return instanceValue.ToMillimeters();
			}
			if (sizeProp != null && sizeProp.Value != null)
			{
				return sizeProp.Value.ToMillimeters();
			}
			return 0.0;
		}

		internal static int GetIntPropertyValue(ReportIntProperty intProp, int? instanceValue)
		{
			if (instanceValue.HasValue)
			{
				return instanceValue.Value;
			}
			return intProp?.Value ?? 0;
		}

		internal static void WriteReportSize(BinaryWriter spbifWriter, byte rplid, ReportSize value)
		{
			if (value != null)
			{
				spbifWriter.Write(rplid);
				spbifWriter.Write(value.ToString());
			}
		}

		internal static void AddStyle(byte rplId, ReportIntProperty prop, StyleWriter writer)
		{
			if (prop != null)
			{
				writer.Write(rplId, prop.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportStringProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportSizeProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value.ToString());
			}
		}

		internal static void AddStyle(byte rplId, ReportProperty prop, StyleWriter writer)
		{
			byte? stylePropByte = PageItem.GetStylePropByte(rplId, prop);
			if (stylePropByte.HasValue)
			{
				writer.Write(rplId, stylePropByte.Value);
			}
		}

		internal static void AddStyle(byte rplId, ReportColorProperty prop, StyleWriter writer)
		{
			if (prop != null && prop.Value != null)
			{
				writer.Write(rplId, prop.Value.ToString());
			}
		}

		internal static string GetStringProp(byte rplId, StyleAttributeNames styleAttributeName, Style styleDef, Dictionary<byte, object> styles)
		{
			object value = null;
			if (styles != null && styles.TryGetValue(rplId, out value))
			{
				string text = value as string;
				if (text != null)
				{
					return text;
				}
			}
			ReportStringProperty reportStringProperty = styleDef[styleAttributeName] as ReportStringProperty;
			if (reportStringProperty != null && !reportStringProperty.IsExpression)
			{
				return reportStringProperty.Value;
			}
			return null;
		}

		internal static double GetSizeProp(byte rplId, StyleAttributeNames styleAttributeName, float defaultValue, Style styleDef, Dictionary<byte, object> styles)
		{
			object value = null;
			if (styles != null && styles.TryGetValue(rplId, out value))
			{
				string text = value as string;
				if (text != null)
				{
					return new RPLReportSize(text).ToPoints();
				}
			}
			ReportSizeProperty reportSizeProperty = styleDef[styleAttributeName] as ReportSizeProperty;
			if (reportSizeProperty != null && !reportSizeProperty.IsExpression && reportSizeProperty.Value != null)
			{
				return reportSizeProperty.Value.ToPoints();
			}
			return defaultValue;
		}

		internal static Color GetColorProp(byte rplId, StyleAttributeNames styleAttributeName, Color defaultColor, Style styleDef, Dictionary<byte, object> styles)
		{
			object value = null;
			if (styles != null && styles.TryGetValue(rplId, out value))
			{
				string text = value as string;
				if (text != null)
				{
					return new ReportColor(text).ToColor();
				}
			}
			ReportColorProperty reportColorProperty = styleDef[styleAttributeName] as ReportColorProperty;
			if (reportColorProperty != null && !reportColorProperty.IsExpression)
			{
				ReportColor value2 = reportColorProperty.Value;
				if (value2 != null)
				{
					return value2.ToColor();
				}
			}
			return defaultColor;
		}

		internal static byte? GetEnumProp(byte rplId, StyleAttributeNames styleAttributeName, Style styleDef, Dictionary<byte, object> styles)
		{
			object value = null;
			if (styles != null && styles.TryGetValue(rplId, out value))
			{
				byte? b = value as byte?;
				if (b.HasValue)
				{
					return b.Value;
				}
			}
			ReportProperty reportProperty = styleDef[styleAttributeName];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				return PageItem.GetStylePropByte(rplId, reportProperty);
			}
			return null;
		}

		internal static byte? GetNonCompiledEnumProp(byte styleAttributeRplId, StyleAttributeNames styleAttributeName, Style style, StyleInstance styleIntance)
		{
			if (style == null)
			{
				return null;
			}
			ReportProperty reportProperty = style[styleAttributeName];
			if (reportProperty == null)
			{
				return null;
			}
			if (reportProperty.IsExpression)
			{
				if (styleIntance != null)
				{
					object obj = styleIntance[styleAttributeName];
					if (obj != null)
					{
						bool convertToString = false;
						return PageItem.GetStylePropByte(styleAttributeRplId, obj, ref convertToString);
					}
				}
				return null;
			}
			return PageItem.GetStylePropByte(styleAttributeRplId, reportProperty);
		}

		public static ReportSize ReadReportSize(IntermediateFormatReader reader)
		{
			string text = reader.ReadString();
			if (text != null)
			{
				return new ReportSize(text);
			}
			return null;
		}

		public static void WriteReportSize(IntermediateFormatWriter writer, ReportSize reportSize)
		{
			if (reportSize == null)
			{
				writer.WriteNull();
			}
			else
			{
				writer.Write(reportSize.ToString());
			}
		}

		public static int ReportSizeItemSize(ReportSize reportSize)
		{
			if (reportSize == null)
			{
				return Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf((string)null);
			}
			return Microsoft.ReportingServices.OnDemandProcessing.Scalability.ItemSizes.SizeOf(reportSize.ToString());
		}
	}
}
