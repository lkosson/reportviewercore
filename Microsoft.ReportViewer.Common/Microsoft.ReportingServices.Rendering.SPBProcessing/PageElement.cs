using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.SPBProcessing
{
	internal abstract class PageElement : ItemOffset
	{
		protected ReportElement m_source;

		public abstract long Offset
		{
			get;
			set;
		}

		internal virtual ReportElement Source => m_source;

		internal virtual ReportElement OriginalSource => m_source;

		internal virtual bool HasBackground => true;

		internal abstract string SourceID
		{
			get;
		}

		internal abstract string SourceUniqueName
		{
			get;
		}

		protected PageElement(ReportElement source)
		{
			m_source = source;
		}

		internal virtual void WriteSharedStyle(BinaryWriter spbifWriter, Style style, PageContext pageContext, byte rplTag)
		{
			if (style == null)
			{
				style = OriginalSource.Style;
			}
			if (style != null)
			{
				spbifWriter.Write(rplTag);
				spbifWriter.Write((byte)0);
				WriteItemSharedStyleProps(spbifWriter, style, pageContext);
				WriteBorderProps(spbifWriter, style);
				spbifWriter.Write(byte.MaxValue);
			}
		}

		internal virtual RPLStyleProps WriteSharedStyle(Style style, PageContext pageContext)
		{
			if (style == null)
			{
				style = OriginalSource.Style;
			}
			if (style == null)
			{
				return null;
			}
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			WriteItemSharedStyleProps(rPLStyleProps, style, pageContext);
			WriteBorderProps(rPLStyleProps, style);
			return rPLStyleProps;
		}

		internal virtual void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
		}

		internal virtual void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext, byte? rplTag, ReportElementInstance compiledSource)
		{
			if (WriteCommonNonSharedStyle(spbifWriter, styleDef, style, pageContext, rplTag, compiledSource))
			{
				spbifWriter.Write(byte.MaxValue);
			}
		}

		protected bool WriteCommonNonSharedStyle(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext, byte? rplTag, ReportElementInstance compiledSource)
		{
			if (OriginalSource == null)
			{
				return false;
			}
			if (styleDef == null)
			{
				styleDef = OriginalSource.Style;
			}
			List<StyleAttributeNames> list = null;
			if (compiledSource == null)
			{
				list = styleDef.NonSharedStyleAttributes;
			}
			else
			{
				StyleInstance style2 = compiledSource.Style;
				if (style2 != null)
				{
					list = style2.StyleAttributes;
				}
			}
			if (list == null || list.Count == 0)
			{
				return false;
			}
			if (style == null)
			{
				style = GetStyleInstance(OriginalSource, compiledSource);
			}
			if (style == null)
			{
				return false;
			}
			if (rplTag.HasValue)
			{
				spbifWriter.Write(rplTag.Value);
			}
			spbifWriter.Write((byte)1);
			WriteNonSharedStyleFromAttributes(spbifWriter, styleDef, style, pageContext, list);
			return true;
		}

		internal virtual RPLStyleProps WriteNonSharedStyle(Style styleDef, StyleInstance style, PageContext pageContext, ReportElementInstance compiledSource)
		{
			RPLStyleProps result = null;
			if (OriginalSource != null)
			{
				if (styleDef == null)
				{
					styleDef = OriginalSource.Style;
				}
				List<StyleAttributeNames> list = null;
				if (compiledSource == null)
				{
					list = styleDef.NonSharedStyleAttributes;
				}
				else
				{
					StyleInstance style2 = compiledSource.Style;
					if (style2 != null)
					{
						list = style2.StyleAttributes;
					}
				}
				if (list == null || list.Count == 0)
				{
					return null;
				}
				if (style == null)
				{
					style = GetStyleInstance(OriginalSource, compiledSource);
				}
				if (style == null)
				{
					return null;
				}
				result = WriteNonSharedStyleFromAttributes(styleDef, style, pageContext, list);
			}
			return result;
		}

		protected virtual void WriteNonSharedStyleFromAttributes(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, PageContext pageContext, List<StyleAttributeNames> styleAttributes)
		{
			bool flag = false;
			for (int i = 0; i < styleAttributes.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = styleAttributes[i];
				if ((uint)(styleAttributeNames - 45) <= 1u)
				{
					if (HasBackground && !flag)
					{
						flag = true;
						WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
					}
				}
				else
				{
					WriteNonSharedStyleProp(spbifWriter, styleDef, style, styleAttributes[i], pageContext);
				}
			}
		}

		protected virtual RPLStyleProps WriteNonSharedStyleFromAttributes(Style styleDef, StyleInstance style, PageContext pageContext, List<StyleAttributeNames> styleAttributes)
		{
			bool flag = false;
			RPLStyleProps rPLStyleProps = new RPLStyleProps();
			for (int i = 0; i < styleAttributes.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = styleAttributes[i];
				if ((uint)(styleAttributeNames - 45) <= 1u)
				{
					if (HasBackground && !flag)
					{
						flag = true;
						WriteItemNonSharedStyleProp(rPLStyleProps, styleDef, style, StyleAttributeNames.BackgroundImage, pageContext);
					}
				}
				else
				{
					WriteNonSharedStyleProp(rPLStyleProps, styleDef, style, styleAttributes[i], pageContext);
				}
			}
			return rPLStyleProps;
		}

		internal virtual void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
		}

		internal virtual void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
		}

		internal virtual void WriteBorderProps(BinaryWriter spbifWriter, Style styleDef)
		{
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderColor, 0);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderStyle, 5);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderWidth, 10);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
			WriteStyleProp(styleDef, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal virtual void WriteBorderProps(RPLStyleProps rplStyleProps, Style styleDef)
		{
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderColor, 0);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
			WriteStyleProp(styleDef, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
		}

		internal virtual void WriteBackgroundImage(BinaryWriter spbifWriter, Style style, bool writeShared, PageContext pageContext)
		{
			if (style == null)
			{
				style = OriginalSource.Style;
			}
			if (style == null)
			{
				return;
			}
			BackgroundImage backgroundImage = style.BackgroundImage;
			if (backgroundImage == null || backgroundImage.Instance == null || (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null))
			{
				return;
			}
			if (backgroundImage.Value != null)
			{
				if (backgroundImage.Value.IsExpression)
				{
					if (!writeShared)
					{
						spbifWriter.Write((byte)33);
						WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null);
					}
				}
				else if (writeShared && backgroundImage.Value.Value != null)
				{
					spbifWriter.Write((byte)33);
					WriteImage(backgroundImage.Instance, null, spbifWriter, pageContext, null);
				}
			}
			if (backgroundImage.BackgroundRepeat == null)
			{
				return;
			}
			if (backgroundImage.BackgroundRepeat.IsExpression)
			{
				if (!writeShared)
				{
					spbifWriter.Write((byte)35);
					spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
				}
			}
			else if (writeShared)
			{
				spbifWriter.Write((byte)35);
				spbifWriter.Write(StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
			}
		}

		internal virtual void WriteBackgroundImage(RPLStyleProps rplStyleProps, Style style, bool writeShared, PageContext pageContext)
		{
			if (style == null)
			{
				style = OriginalSource.Style;
			}
			if (style == null)
			{
				return;
			}
			BackgroundImage backgroundImage = style.BackgroundImage;
			if (backgroundImage == null || backgroundImage.Instance == null || (backgroundImage.Instance.StreamName == null && backgroundImage.Instance.ImageData == null))
			{
				return;
			}
			if (backgroundImage.Value != null)
			{
				if (backgroundImage.Value.IsExpression)
				{
					if (!writeShared)
					{
						RPLImageData imageData = new RPLImageData();
						WriteImage(backgroundImage.Instance, null, ref imageData, pageContext, null);
						rplStyleProps.Add(33, imageData);
					}
				}
				else if (writeShared && backgroundImage.Value.Value != null)
				{
					RPLImageData imageData2 = new RPLImageData();
					WriteImage(backgroundImage.Instance, null, ref imageData2, pageContext, null);
					rplStyleProps.Add(33, imageData2);
				}
			}
			if (backgroundImage.BackgroundRepeat == null)
			{
				return;
			}
			if (backgroundImage.BackgroundRepeat.IsExpression)
			{
				if (!writeShared)
				{
					rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.Instance.BackgroundRepeat));
				}
			}
			else if (writeShared)
			{
				rplStyleProps.Add(35, StyleEnumConverter.Translate(backgroundImage.BackgroundRepeat.Value));
			}
		}

		internal void WriteStyleProp(Style style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = style[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
			}
		}

		internal void WriteStyleProp(Style style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType, RPLVersionEnum rplVersion)
		{
			ReportProperty reportProperty = style[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WriteStyleReportProperty(reportProperty, spbifWriter, spbifType, rplVersion);
			}
		}

		internal void WriteStyleProp(Style style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			ReportProperty reportProperty = style[name];
			if (reportProperty != null && !reportProperty.IsExpression)
			{
				WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
			}
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType)
		{
			if (OriginalSource == null)
			{
				return;
			}
			object obj = style[name];
			if (obj == null)
			{
				if (styleDef == null)
				{
					styleDef = OriginalSource.Style;
				}
				if (styleDef != null)
				{
					ReportProperty reportProperty = styleDef[name];
					if (reportProperty != null && reportProperty.IsExpression)
					{
						WriteStyleReportProperty(reportProperty, spbifWriter, spbifType);
					}
				}
				return;
			}
			switch (spbifType)
			{
			case 33:
				return;
			case 37:
			{
				int num = (int)obj;
				if (num > 0)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(num);
				}
				return;
			}
			}
			bool convertToString = true;
			byte? stylePropByte = GetStylePropByte(spbifType, obj, ref convertToString);
			if (stylePropByte.HasValue)
			{
				spbifWriter.Write(spbifType);
				spbifWriter.Write(stylePropByte.Value);
			}
			else if (convertToString)
			{
				string text = obj.ToString();
				if (text != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(text);
				}
			}
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, BinaryWriter spbifWriter, StyleAttributeNames name, byte spbifType, RPLVersionEnum rplVersion)
		{
			if (OriginalSource == null)
			{
				return;
			}
			object obj = style[name];
			if (obj == null)
			{
				if (styleDef == null)
				{
					styleDef = OriginalSource.Style;
				}
				if (styleDef != null)
				{
					ReportProperty reportProperty = styleDef[name];
					if (reportProperty != null && reportProperty.IsExpression)
					{
						WriteStyleReportProperty(reportProperty, spbifWriter, spbifType, rplVersion);
					}
				}
			}
			else if (spbifType == 30)
			{
				byte value = StyleEnumConverter.Translate((WritingModes)obj, rplVersion);
				spbifWriter.Write(spbifType);
				spbifWriter.Write(value);
			}
			else
			{
				WriteStyleProp(styleDef, style, spbifWriter, name, spbifType);
			}
		}

		internal void WriteStyleProp(Style styleDef, StyleInstance style, RPLStyleProps rplStyleProps, StyleAttributeNames name, byte spbifType)
		{
			if (OriginalSource == null)
			{
				return;
			}
			object obj = style[name];
			if (obj == null)
			{
				if (styleDef == null)
				{
					styleDef = OriginalSource.Style;
				}
				if (styleDef == null)
				{
					return;
				}
				ReportProperty reportProperty = styleDef[name];
				if (reportProperty != null && reportProperty.IsExpression)
				{
					obj = style[name];
					if (obj == null)
					{
						WriteStyleReportProperty(reportProperty, rplStyleProps, spbifType);
					}
				}
				return;
			}
			switch (spbifType)
			{
			case 33:
				return;
			case 37:
			{
				int num = (int)obj;
				if (num > 0)
				{
					rplStyleProps.Add(spbifType, num);
				}
				return;
			}
			}
			bool convertToString = true;
			byte? stylePropByte = GetStylePropByte(spbifType, obj, ref convertToString);
			if (stylePropByte.HasValue)
			{
				rplStyleProps.Add(spbifType, stylePropByte.Value);
			}
			else if (convertToString)
			{
				string text = obj.ToString();
				if (text != null)
				{
					rplStyleProps.Add(spbifType, text);
				}
			}
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, BinaryWriter spbifWriter, PageContext pageContext, System.Drawing.Image gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			long position = spbifWriter.BaseStream.Position;
			spbifWriter.Write((byte)42);
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, spbifWriter, pageContext, position, gdiImage);
				return;
			}
			spbifWriter.Write((byte)1);
			WriteImageProperties(imageInstance, null, spbifWriter, gdiImage, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, RPLImageProps elemProps, PageContext pageContext, System.Drawing.Image gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			RPLImageData imageData = new RPLImageData();
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, ref imageData, pageContext, gdiImage);
			}
			else
			{
				WriteImageProperties(imageInstance, null, imageData, gdiImage, pageContext);
			}
			elemProps.Image = imageData;
		}

		internal void WriteImage(IImageInstance imageInstance, string resourceName, ref RPLImageData imageData, PageContext pageContext, System.Drawing.Image gdiImage)
		{
			string text = resourceName;
			if (imageInstance != null)
			{
				text = imageInstance.StreamName;
			}
			if (text != null)
			{
				WriteShareableImages(imageInstance, text, ref imageData, pageContext, gdiImage);
			}
			else
			{
				WriteImageProperties(imageInstance, null, imageData, gdiImage, pageContext);
			}
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ReportElementInstance compiledReportElement)
		{
			bool isShared = true;
			return GetRichTextStyleValue(styleName, compiledReportElement, ref isShared);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ReportElementInstance compiledReportElement, RPLVersionEnum rplVersion)
		{
			bool isShared = true;
			return GetRichTextStyleValue(styleName, compiledReportElement, ref isShared, rplVersion);
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ReportElementInstance compiledReportElement, ref bool isShared)
		{
			object obj = null;
			ReportProperty reportProperty = null;
			reportProperty = OriginalSource.Style[styleName];
			if (reportProperty != null)
			{
				if (reportProperty.IsExpression)
				{
					obj = GetStyleInstance(OriginalSource, compiledReportElement)[styleName];
					isShared = false;
				}
				else if (compiledReportElement != null)
				{
					obj = compiledReportElement.Style[styleName];
					isShared = false;
				}
				if (obj == null)
				{
					switch (styleName)
					{
					case StyleAttributeNames.BorderColor:
					case StyleAttributeNames.BorderColorTop:
					case StyleAttributeNames.BorderColorLeft:
					case StyleAttributeNames.BorderColorRight:
					case StyleAttributeNames.BorderColorBottom:
					case StyleAttributeNames.BackgroundColor:
					case StyleAttributeNames.Color:
						obj = ((ReportColorProperty)reportProperty).Value;
						break;
					case StyleAttributeNames.BorderStyle:
					case StyleAttributeNames.BorderStyleTop:
					case StyleAttributeNames.BorderStyleLeft:
					case StyleAttributeNames.BorderStyleRight:
					case StyleAttributeNames.BorderStyleBottom:
						obj = ((ReportEnumProperty<BorderStyles>)reportProperty).Value;
						break;
					case StyleAttributeNames.BorderWidth:
					case StyleAttributeNames.BorderWidthTop:
					case StyleAttributeNames.BorderWidthLeft:
					case StyleAttributeNames.BorderWidthRight:
					case StyleAttributeNames.BorderWidthBottom:
					case StyleAttributeNames.FontSize:
					case StyleAttributeNames.PaddingLeft:
					case StyleAttributeNames.PaddingRight:
					case StyleAttributeNames.PaddingTop:
					case StyleAttributeNames.PaddingBottom:
					case StyleAttributeNames.LineHeight:
						obj = ((ReportSizeProperty)reportProperty).Value;
						break;
					case StyleAttributeNames.FontStyle:
						obj = ((ReportEnumProperty<FontStyles>)reportProperty).Value;
						break;
					case StyleAttributeNames.FontFamily:
					case StyleAttributeNames.Format:
					case StyleAttributeNames.Language:
					case StyleAttributeNames.NumeralLanguage:
						obj = ((ReportStringProperty)reportProperty).Value;
						break;
					case StyleAttributeNames.NumeralVariant:
						obj = ((ReportIntProperty)reportProperty).Value;
						break;
					case StyleAttributeNames.FontWeight:
						obj = ((ReportEnumProperty<FontWeights>)reportProperty).Value;
						break;
					case StyleAttributeNames.TextDecoration:
						obj = ((ReportEnumProperty<TextDecorations>)reportProperty).Value;
						break;
					case StyleAttributeNames.TextAlign:
						obj = ((ReportEnumProperty<TextAlignments>)reportProperty).Value;
						break;
					case StyleAttributeNames.VerticalAlign:
						obj = ((ReportEnumProperty<VerticalAlignments>)reportProperty).Value;
						break;
					case StyleAttributeNames.Direction:
						obj = ((ReportEnumProperty<Directions>)reportProperty).Value;
						break;
					case StyleAttributeNames.WritingMode:
						obj = ((ReportEnumProperty<WritingModes>)reportProperty).Value;
						break;
					case StyleAttributeNames.Calendar:
						obj = ((ReportEnumProperty<Calendars>)reportProperty).Value;
						break;
					default:
						RSTrace.RenderingTracer.Assert(condition: false, "Missing case for " + styleName);
						break;
					}
				}
			}
			else if (compiledReportElement != null)
			{
				obj = compiledReportElement.Style[styleName];
				isShared = false;
			}
			return obj;
		}

		internal object GetRichTextStyleValue(StyleAttributeNames styleName, ReportElementInstance compiledReportElement, ref bool isShared, RPLVersionEnum rplVersion)
		{
			object obj = GetRichTextStyleValue(styleName, compiledReportElement, ref isShared);
			if ((int)rplVersion <= 3 && styleName == StyleAttributeNames.WritingMode && (WritingModes)obj == WritingModes.Rotate270)
			{
				obj = WritingModes.Horizontal;
			}
			return obj;
		}

		internal StyleInstance GetStyleInstance(ReportElement reportElement, ReportElementInstance compiledReportElement)
		{
			StyleInstance result = null;
			if (reportElement != null)
			{
				result = ((compiledReportElement == null) ? reportElement.Instance.Style : compiledReportElement.Style);
			}
			return result;
		}

		private void WriteStyleReportProperty(ReportProperty styleProp, BinaryWriter spbifWriter, byte spbifType)
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
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(reportIntProperty.Value);
				}
			}
			else
			{
				byte? stylePropByte = GetStylePropByte(spbifType, styleProp);
				if (stylePropByte.HasValue)
				{
					spbifWriter.Write(spbifType);
					spbifWriter.Write(stylePropByte.Value);
				}
			}
		}

		private void WriteStyleReportProperty(ReportProperty styleProp, BinaryWriter spbifWriter, byte spbifType, RPLVersionEnum rplVersion)
		{
			if (styleProp != null)
			{
				if (spbifType == 30)
				{
					byte value = StyleEnumConverter.Translate(((ReportEnumProperty<WritingModes>)styleProp).Value, rplVersion);
					spbifWriter.Write(spbifType);
					spbifWriter.Write(value);
				}
				else
				{
					WriteStyleReportProperty(styleProp, spbifWriter, spbifType);
				}
			}
		}

		private void WriteStyleReportProperty(ReportProperty styleProp, RPLStyleProps rplStyleProps, byte spbifType)
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
					rplStyleProps.Add(spbifType, reportStringProperty.Value);
				}
			}
			else if (styleProp is ReportSizeProperty)
			{
				ReportSizeProperty reportSizeProperty = (ReportSizeProperty)styleProp;
				if (reportSizeProperty.Value != null)
				{
					rplStyleProps.Add(spbifType, reportSizeProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportColorProperty)
			{
				ReportColorProperty reportColorProperty = (ReportColorProperty)styleProp;
				if (reportColorProperty.Value != null)
				{
					rplStyleProps.Add(spbifType, reportColorProperty.Value.ToString());
				}
			}
			else if (styleProp is ReportIntProperty)
			{
				ReportIntProperty reportIntProperty = (ReportIntProperty)styleProp;
				if (reportIntProperty.Value > 0)
				{
					rplStyleProps.Add(spbifType, reportIntProperty.Value);
				}
			}
			else
			{
				byte? stylePropByte = GetStylePropByte(spbifType, styleProp);
				if (stylePropByte.HasValue)
				{
					rplStyleProps.Add(spbifType, stylePropByte.Value);
				}
			}
		}

		private byte? GetStylePropByte(byte spbifType, object styleProp, ref bool convertToString)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate((FontStyles)styleProp);
			case 22:
				return StyleEnumConverter.Translate((FontWeights)styleProp);
			case 24:
				return StyleEnumConverter.Translate((TextDecorations)styleProp);
			case 25:
				return StyleEnumConverter.Translate((TextAlignments)styleProp);
			case 26:
				return StyleEnumConverter.Translate((VerticalAlignments)styleProp);
			case 29:
				return StyleEnumConverter.Translate((Directions)styleProp);
			case 30:
				return StyleEnumConverter.Translate((WritingModes)styleProp);
			case 31:
				return StyleEnumConverter.Translate((UnicodeBiDiTypes)styleProp);
			case 38:
				return StyleEnumConverter.Translate((Calendars)styleProp);
			case 5:
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 6:
			case 7:
			case 8:
			case 9:
				convertToString = false;
				return StyleEnumConverter.Translate((BorderStyles)styleProp);
			case 35:
				return StyleEnumConverter.Translate((BackgroundRepeatTypes)styleProp);
			default:
				return null;
			}
		}

		private byte? GetStylePropByte(byte spbifType, ReportProperty styleProp)
		{
			switch (spbifType)
			{
			case 19:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontStyles>)styleProp).Value);
			case 22:
				return StyleEnumConverter.Translate(((ReportEnumProperty<FontWeights>)styleProp).Value);
			case 24:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextDecorations>)styleProp).Value);
			case 25:
				return StyleEnumConverter.Translate(((ReportEnumProperty<TextAlignments>)styleProp).Value);
			case 26:
				return StyleEnumConverter.Translate(((ReportEnumProperty<VerticalAlignments>)styleProp).Value);
			case 29:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Directions>)styleProp).Value);
			case 30:
				return StyleEnumConverter.Translate(((ReportEnumProperty<WritingModes>)styleProp).Value);
			case 31:
				return StyleEnumConverter.Translate(((ReportEnumProperty<UnicodeBiDiTypes>)styleProp).Value);
			case 38:
				return StyleEnumConverter.Translate(((ReportEnumProperty<Calendars>)styleProp).Value);
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BorderStyles>)styleProp).Value);
			case 35:
				return StyleEnumConverter.Translate(((ReportEnumProperty<BackgroundRepeatTypes>)styleProp).Value);
			default:
				return null;
			}
		}

		private string GetSourceInstanceName()
		{
			if (m_source != null)
			{
				return m_source.InstanceUniqueName;
			}
			return SourceUniqueName;
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, PageContext pageContext, long offsetStart, System.Drawing.Image gdiImage)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable[streamName];
				if (obj != null)
				{
					spbifWriter.Write((byte)2);
					spbifWriter.Write((long)obj);
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			hashtable.Add(streamName, offsetStart);
			spbifWriter.Write((byte)0);
			WriteImageProperties(imageInstance, streamName, spbifWriter, gdiImage, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		private void WriteShareableImages(IImageInstance imageInstance, string streamName, ref RPLImageData imageData, PageContext pageContext, System.Drawing.Image gdiImage)
		{
			Hashtable hashtable = pageContext.SharedImages;
			if (hashtable != null)
			{
				object obj = hashtable[streamName];
				if (obj != null)
				{
					imageData = (RPLImageData)obj;
					return;
				}
			}
			if (hashtable == null)
			{
				hashtable = (pageContext.SharedImages = new Hashtable());
			}
			imageData.IsShared = true;
			hashtable.Add(streamName, imageData);
			WriteImageProperties(imageInstance, streamName, imageData, gdiImage, pageContext);
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, BinaryWriter spbifWriter, System.Drawing.Image gdiImage, PageContext pageContext)
		{
			bool generatedStreamName = false;
			if (streamName == null && pageContext.AddSecondaryStreamNames && imageInstance != null && imageInstance.ImageData != null)
			{
				streamName = pageContext.GenerateStreamName(imageInstance, GetSourceInstanceName());
				generatedStreamName = true;
			}
			if (imageInstance != null)
			{
				string imageMimeType = imageInstance.MIMEType;
				if (pageContext.SecondaryStreams != 0)
				{
					CreateImageStream(imageInstance, streamName, generatedStreamName, pageContext);
				}
				else if (imageInstance.ImageData != null)
				{
					byte[] imageData = imageInstance.ImageData;
					if (pageContext.ConvertImages)
					{
						try
						{
							ImageConverter.Convert(ref imageData, ref imageMimeType);
						}
						catch (ArgumentOutOfRangeException)
						{
						}
					}
					spbifWriter.Write((byte)2);
					spbifWriter.Write(imageData.Length);
					spbifWriter.Write(imageData);
				}
				if (imageMimeType != null)
				{
					spbifWriter.Write((byte)0);
					spbifWriter.Write(imageMimeType);
				}
			}
			if (streamName != null)
			{
				spbifWriter.Write((byte)1);
				spbifWriter.Write(streamName);
			}
			if (gdiImage == null)
			{
				return;
			}
			ImageFormat imageFormat = gdiImage.RawFormat;
			if (pageContext.ConvertImages && ImageConverter.NeedsToConvert(imageFormat))
			{
				imageFormat = ImageFormat.Png;
			}
			if (imageInstance == null)
			{
				if (pageContext.SecondaryStreams != 0)
				{
					Stream stream = pageContext.CreateAndRegisterStream(streamName, string.Empty, null, null, willSeek: false, StreamOper.CreateAndRegister);
					gdiImage.Save(stream, imageFormat);
				}
				else
				{
					MemoryStream memoryStream = new MemoryStream();
					gdiImage.Save(memoryStream, imageFormat);
					spbifWriter.Write((byte)2);
					spbifWriter.Write((int)memoryStream.Length);
					WriteStreamContent(memoryStream, spbifWriter);
				}
			}
			spbifWriter.Write((byte)3);
			spbifWriter.Write(gdiImage.Width);
			spbifWriter.Write((byte)4);
			spbifWriter.Write(gdiImage.Height);
			spbifWriter.Write((byte)5);
			spbifWriter.Write(gdiImage.HorizontalResolution);
			spbifWriter.Write((byte)6);
			spbifWriter.Write(gdiImage.VerticalResolution);
			if (imageFormat.Equals(ImageFormat.Png))
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write((byte)3);
			}
			else if (imageFormat.Equals(ImageFormat.Jpeg))
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write((byte)1);
			}
			else if (imageFormat.Equals(ImageFormat.Gif))
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write((byte)2);
			}
			else if (imageFormat.Equals(ImageFormat.Bmp))
			{
				spbifWriter.Write((byte)7);
				spbifWriter.Write((byte)0);
			}
		}

		private void WriteImageProperties(IImageInstance imageInstance, string streamName, RPLImageData imageData, System.Drawing.Image gdiImage, PageContext pageContext)
		{
			bool generatedStreamName = false;
			if (streamName == null && pageContext.AddSecondaryStreamNames && imageInstance != null && imageInstance.ImageData != null)
			{
				streamName = pageContext.GenerateStreamName(imageInstance, GetSourceInstanceName());
				generatedStreamName = true;
			}
			if (imageInstance != null)
			{
				imageData.ImageMimeType = imageInstance.MIMEType;
				if (pageContext.SecondaryStreams != 0)
				{
					CreateImageStream(imageInstance, streamName, generatedStreamName, pageContext);
				}
				else
				{
					imageData.ImageData = imageInstance.ImageData;
				}
			}
			imageData.ImageName = streamName;
			if (gdiImage != null)
			{
				if (imageInstance == null)
				{
					MemoryStream memoryStream = new MemoryStream();
					gdiImage.Save(memoryStream, gdiImage.RawFormat);
					imageData.ImageData = memoryStream.ToArray();
				}
				imageData.GDIImageProps = new GDIImageProps(gdiImage);
			}
		}

		private void CreateImageStream(IImageInstance imageInstance, string streamName, bool generatedStreamName, PageContext pageContext)
		{
			if (streamName == null)
			{
				return;
			}
			if (!generatedStreamName && pageContext.SecondaryStreams == SecondaryStreams.Server)
			{
				pageContext.CreateAndRegisterStream(streamName, string.Empty, null, imageInstance.MIMEType, willSeek: false, StreamOper.RegisterOnly);
			}
			else if (imageInstance.ImageData != null)
			{
				Stream stream = null;
				if (!pageContext.RegisteredStreamNames.Contains(streamName))
				{
					stream = pageContext.CreateAndRegisterStream(streamName, string.Empty, null, imageInstance.MIMEType, willSeek: false, StreamOper.CreateAndRegister);
					pageContext.RegisteredStreamNames.Add(streamName, null);
				}
				stream?.Write(imageInstance.ImageData, 0, imageInstance.ImageData.Length);
			}
		}

		private void WriteStreamContent(Stream sourceStream, BinaryWriter spbifWriter)
		{
			byte[] buffer = new byte[1024];
			long num = 0L;
			int num2 = 0;
			sourceStream.Position = 0L;
			for (; num < sourceStream.Length; num += num2)
			{
				num2 = sourceStream.Read(buffer, 0, 1024);
				spbifWriter.Write(buffer, 0, num2);
			}
		}

		internal void WriteActionInfo(ActionInfo actionInfo, BinaryWriter spbifWriter, PageContext pageContext, byte rplIdStartInfo)
		{
			if (actionInfo == null)
			{
				return;
			}
			spbifWriter.Write(rplIdStartInfo);
			ActionCollection actions = actionInfo.Actions;
			Microsoft.ReportingServices.OnDemandReportRendering.Action action = null;
			ActionInstance actionInstance = null;
			if (actions != null && actions.Count > 0)
			{
				spbifWriter.Write((byte)2);
				spbifWriter.Write(actions.Count);
				for (int i = 0; i < actions.Count; i++)
				{
					action = actions[i];
					actionInstance = action.Instance;
					spbifWriter.Write((byte)3);
					if (action.Label != null && actionInstance.Label != null)
					{
						spbifWriter.Write((byte)4);
						spbifWriter.Write(actionInstance.Label);
					}
					if (action.Hyperlink != null)
					{
						ReportUrl hyperlink = actionInstance.Hyperlink;
						if (hyperlink != null)
						{
							Uri uri = hyperlink.ToUri();
							if (null != uri)
							{
								spbifWriter.Write((byte)6);
								spbifWriter.Write(uri.AbsoluteUri);
							}
						}
					}
					else if (action.BookmarkLink != null)
					{
						if (actionInstance.BookmarkLink != null)
						{
							spbifWriter.Write((byte)7);
							spbifWriter.Write(actionInstance.BookmarkLink);
						}
					}
					else if (action.Drillthrough != null)
					{
						if (pageContext.RegisterEvents)
						{
							action.Drillthrough.RegisterDrillthroughAction();
						}
						ActionDrillthroughInstance instance = action.Drillthrough.Instance;
						if (instance.DrillthroughID != null)
						{
							spbifWriter.Write((byte)8);
							spbifWriter.Write(instance.DrillthroughID);
						}
						if (instance.DrillthroughUrl != null)
						{
							spbifWriter.Write((byte)9);
							spbifWriter.Write(instance.DrillthroughUrl);
						}
					}
					spbifWriter.Write(byte.MaxValue);
				}
			}
			WriteImageMapAreaInstanceCollection(actionInfo as ActionInfoWithDynamicImageMap, spbifWriter, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		internal RPLActionInfo WriteActionInfo(ActionInfo actionInfo, PageContext pageContext)
		{
			if (actionInfo == null)
			{
				return null;
			}
			ActionCollection actions = actionInfo.Actions;
			if (actions == null || actions.Count == 0)
			{
				return null;
			}
			RPLActionInfo rPLActionInfo = new RPLActionInfo(actions.Count);
			WriteAction(actions, rPLActionInfo, pageContext);
			return rPLActionInfo;
		}

		internal void WriteImageMapAreaInstanceCollection(ActionInfoWithDynamicImageMapCollection actionImageMaps, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (actionImageMaps == null || actionImageMaps.Count == 0)
			{
				return;
			}
			spbifWriter.Write((byte)38);
			spbifWriter.Write(actionImageMaps.Count);
			for (int i = 0; i < actionImageMaps.Count; i++)
			{
				if (actionImageMaps[i] == null)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(byte.MaxValue);
				}
				else
				{
					WriteActionInfo(actionImageMaps[i], spbifWriter, pageContext, 7);
				}
			}
		}

		internal void WriteImageMapAreaInstanceCollection(ActionInfoWithDynamicImageMap[] actionImageMaps, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (actionImageMaps == null || actionImageMaps.Length == 0)
			{
				return;
			}
			spbifWriter.Write((byte)38);
			spbifWriter.Write(actionImageMaps.Length);
			for (int i = 0; i < actionImageMaps.Length; i++)
			{
				if (actionImageMaps[i] == null)
				{
					spbifWriter.Write((byte)7);
					spbifWriter.Write(byte.MaxValue);
				}
				else
				{
					WriteActionInfo(actionImageMaps[i], spbifWriter, pageContext, 7);
				}
			}
		}

		internal RPLActionInfoWithImageMap[] WriteImageMapAreaInstanceCollection(ActionInfoWithDynamicImageMapCollection actionImageMaps, PageContext pageContext)
		{
			if (actionImageMaps == null || actionImageMaps.Count == 0)
			{
				return null;
			}
			RPLActionInfoWithImageMap[] array = new RPLActionInfoWithImageMap[actionImageMaps.Count];
			for (int i = 0; i < actionImageMaps.Count; i++)
			{
				array[i] = WriteActionInfoWithMaps(actionImageMaps[i], pageContext);
			}
			return array;
		}

		internal RPLActionInfoWithImageMap[] WriteImageMapAreaInstanceCollection(ActionInfoWithDynamicImageMap[] actionImageMaps, PageContext pageContext)
		{
			if (actionImageMaps == null || actionImageMaps.Length == 0)
			{
				return null;
			}
			RPLActionInfoWithImageMap[] array = new RPLActionInfoWithImageMap[actionImageMaps.Length];
			for (int i = 0; i < actionImageMaps.Length; i++)
			{
				array[i] = WriteActionInfoWithMaps(actionImageMaps[i], pageContext);
			}
			return array;
		}

		private void WriteImageMapAreaInstanceCollection(ActionInfoWithDynamicImageMap actionInfo, BinaryWriter spbifWriter, PageContext pageContext)
		{
			if (actionInfo == null)
			{
				return;
			}
			ImageMapAreaInstanceCollection imageMapAreaInstances = actionInfo.ImageMapAreaInstances;
			if (imageMapAreaInstances == null || imageMapAreaInstances.Count == 0)
			{
				return;
			}
			spbifWriter.Write((byte)10);
			spbifWriter.Write(imageMapAreaInstances.Count);
			ImageMapAreaInstance imageMapAreaInstance = null;
			byte b = 0;
			float[] array = null;
			for (int i = 0; i < imageMapAreaInstances.Count; i++)
			{
				imageMapAreaInstance = imageMapAreaInstances[i];
				b = (byte)((imageMapAreaInstance.Shape == ImageMapArea.ImageMapAreaShape.Circle) ? 2 : ((imageMapAreaInstance.Shape == ImageMapArea.ImageMapAreaShape.Polygon) ? 1 : 0));
				spbifWriter.Write(b);
				array = imageMapAreaInstance.Coordinates;
				if (array != null && array.Length != 0)
				{
					spbifWriter.Write(array.Length);
					for (int j = 0; j < array.Length; j++)
					{
						spbifWriter.Write(array[j]);
					}
				}
				else
				{
					spbifWriter.Write(0);
				}
				if (imageMapAreaInstance.ToolTip != null)
				{
					spbifWriter.Write((byte)5);
					spbifWriter.Write(imageMapAreaInstance.ToolTip);
				}
				else
				{
					spbifWriter.Write(byte.MaxValue);
				}
			}
		}

		private void WriteAction(ActionCollection actions, RPLActionInfo rplActionInfo, PageContext pageContext)
		{
			Microsoft.ReportingServices.OnDemandReportRendering.Action action = null;
			ActionInstance actionInstance = null;
			RPLAction rPLAction = null;
			for (int i = 0; i < actions.Count; i++)
			{
				action = actions[i];
				actionInstance = action.Instance;
				rPLAction = new RPLAction(actionInstance.Label);
				if (action.Hyperlink != null)
				{
					ReportUrl hyperlink = actionInstance.Hyperlink;
					if (hyperlink != null)
					{
						Uri uri = hyperlink.ToUri();
						if (null != uri)
						{
							rPLAction.Hyperlink = uri.AbsoluteUri;
						}
					}
				}
				else if (action.BookmarkLink != null)
				{
					rPLAction.BookmarkLink = actionInstance.BookmarkLink;
				}
				else if (action.Drillthrough != null)
				{
					if (pageContext.RegisterEvents)
					{
						action.Drillthrough.RegisterDrillthroughAction();
					}
					ActionDrillthroughInstance instance = action.Drillthrough.Instance;
					rPLAction.DrillthroughId = instance.DrillthroughID;
					rPLAction.DrillthroughUrl = instance.DrillthroughUrl;
				}
				rplActionInfo.Actions[i] = rPLAction;
			}
		}

		internal RPLActionInfoWithImageMap WriteActionInfoWithMaps(ActionInfoWithDynamicImageMap actionInfo, PageContext pageContext)
		{
			if (actionInfo == null)
			{
				return null;
			}
			RPLActionInfoWithImageMap rPLActionInfoWithImageMap = null;
			ActionCollection actions = actionInfo.Actions;
			if (actions != null && actions.Count > 0)
			{
				rPLActionInfoWithImageMap = new RPLActionInfoWithImageMap(actions.Count);
				WriteAction(actions, rPLActionInfoWithImageMap, pageContext);
			}
			ImageMapAreaInstanceCollection imageMapAreaInstances = actionInfo.ImageMapAreaInstances;
			if (imageMapAreaInstances != null && imageMapAreaInstances.Count > 0)
			{
				if (rPLActionInfoWithImageMap == null)
				{
					rPLActionInfoWithImageMap = new RPLActionInfoWithImageMap();
				}
				rPLActionInfoWithImageMap.ImageMaps = new RPLImageMapCollection(imageMapAreaInstances.Count);
				ImageMapAreaInstance imageMapAreaInstance = null;
				RPLImageMap rPLImageMap = null;
				for (int i = 0; i < imageMapAreaInstances.Count; i++)
				{
					imageMapAreaInstance = imageMapAreaInstances[i];
					rPLImageMap = new RPLImageMap();
					if (imageMapAreaInstance.Shape == ImageMapArea.ImageMapAreaShape.Circle)
					{
						rPLImageMap.Shape = RPLFormat.ShapeType.Circle;
					}
					else if (imageMapAreaInstance.Shape == ImageMapArea.ImageMapAreaShape.Polygon)
					{
						rPLImageMap.Shape = RPLFormat.ShapeType.Polygon;
					}
					else
					{
						rPLImageMap.Shape = RPLFormat.ShapeType.Rectangle;
					}
					rPLImageMap.Coordinates = imageMapAreaInstance.Coordinates;
					rPLImageMap.ToolTip = imageMapAreaInstance.ToolTip;
					rPLActionInfoWithImageMap.ImageMaps[i] = rPLImageMap;
				}
			}
			return rPLActionInfoWithImageMap;
		}

		internal virtual void WriteElementProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
			spbifWriter.Write((byte)15);
			WriteSharedItemProps(spbifWriter, rplWriter, pageContext, offset);
			WriteNonSharedItemProps(spbifWriter, rplWriter, pageContext);
			spbifWriter.Write(byte.MaxValue);
		}

		internal virtual void WriteElementProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
			WriteSharedItemProps(elemProps, rplWriter, pageContext);
			WriteNonSharedItemProps(elemProps, rplWriter, pageContext);
		}

		internal virtual void WriteSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext, long offset)
		{
		}

		internal virtual void WriteSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal virtual void WriteCustomNonSharedItemProps(RPLElementProps elemProps, RPLWriter rplWriter, PageContext pageContext)
		{
		}
	}
}
