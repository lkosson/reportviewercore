using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class Style : IPersistable
	{
		internal enum StyleId
		{
			BorderColor,
			BorderColorTop,
			BorderColorLeft,
			BorderColorRight,
			BorderColorBottom,
			BorderStyle,
			BorderStyleTop,
			BorderStyleLeft,
			BorderStyleRight,
			BorderStyleBottom,
			BorderWidth,
			BorderWidthTop,
			BorderWidthLeft,
			BorderWidthRight,
			BorderWidthBottom,
			BackgroundColor,
			FontStyle,
			FontFamily,
			FontSize,
			FontWeight,
			Format,
			TextDecoration,
			TextAlign,
			VerticalAlign,
			Color,
			PaddingLeft,
			PaddingRight,
			PaddingTop,
			PaddingBottom,
			LineHeight,
			Direction,
			WritingMode,
			Language,
			UnicodeBiDi,
			Calendar,
			NumeralLanguage,
			NumeralVariant,
			BackgroundGradientType,
			BackgroundGradientEndColor,
			BackgroundHatchType,
			TransparentColor,
			ShadowColor,
			ShadowOffset,
			Position,
			TextEffect,
			BackgroundImage,
			BackgroundImageRepeat,
			BackgroundImageSource,
			BackgroundImageValue,
			BackgroundImageMimeType,
			CurrencyLanguage
		}

		private sealed class EmptyStyleExprHost : StyleExprHost
		{
			internal static EmptyStyleExprHost Instance = new EmptyStyleExprHost();

			private EmptyStyleExprHost()
			{
			}
		}

		protected Dictionary<string, AttributeInfo> m_styleAttributes;

		protected List<ExpressionInfo> m_expressionList;

		[NonSerialized]
		private StyleExprHost m_exprHost;

		[NonSerialized]
		private int m_customSharedStyleCount = -1;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		internal Dictionary<string, AttributeInfo> StyleAttributes
		{
			get
			{
				return m_styleAttributes;
			}
			set
			{
				m_styleAttributes = value;
			}
		}

		internal List<ExpressionInfo> ExpressionList
		{
			get
			{
				return m_expressionList;
			}
			set
			{
				m_expressionList = value;
			}
		}

		internal StyleExprHost ExprHost => m_exprHost;

		internal int CustomSharedStyleCount
		{
			get
			{
				return m_customSharedStyleCount;
			}
			set
			{
				m_customSharedStyleCount = value;
			}
		}

		internal static string GetStyleString(StyleId styleId)
		{
			switch (styleId)
			{
			case StyleId.BorderColor:
				return "BorderColor";
			case StyleId.BorderColorTop:
				return "BorderColorTop";
			case StyleId.BorderColorLeft:
				return "BorderColorLeft";
			case StyleId.BorderColorRight:
				return "BorderColorRight";
			case StyleId.BorderColorBottom:
				return "BorderColorBottom";
			case StyleId.BorderStyle:
				return "BorderStyle";
			case StyleId.BorderStyleTop:
				return "BorderStyleTop";
			case StyleId.BorderStyleLeft:
				return "BorderStyleLeft";
			case StyleId.BorderStyleRight:
				return "BorderStyleRight";
			case StyleId.BorderStyleBottom:
				return "BorderStyleBottom";
			case StyleId.BorderWidth:
				return "BorderWidth";
			case StyleId.BorderWidthTop:
				return "BorderWidthTop";
			case StyleId.BorderWidthLeft:
				return "BorderWidthLeft";
			case StyleId.BorderWidthRight:
				return "BorderWidthRight";
			case StyleId.BorderWidthBottom:
				return "BorderWidthBottom";
			case StyleId.BackgroundColor:
				return "BackgroundColor";
			case StyleId.FontStyle:
				return "FontStyle";
			case StyleId.FontFamily:
				return "FontFamily";
			case StyleId.FontSize:
				return "FontSize";
			case StyleId.FontWeight:
				return "FontWeight";
			case StyleId.Format:
				return "Format";
			case StyleId.TextDecoration:
				return "TextDecoration";
			case StyleId.TextAlign:
				return "TextAlign";
			case StyleId.VerticalAlign:
				return "VerticalAlign";
			case StyleId.Color:
				return "Color";
			case StyleId.PaddingLeft:
				return "PaddingLeft";
			case StyleId.PaddingRight:
				return "PaddingRight";
			case StyleId.PaddingTop:
				return "PaddingTop";
			case StyleId.PaddingBottom:
				return "PaddingBottom";
			case StyleId.LineHeight:
				return "LineHeight";
			case StyleId.Direction:
				return "Direction";
			case StyleId.WritingMode:
				return "WritingMode";
			case StyleId.Language:
				return "Language";
			case StyleId.UnicodeBiDi:
				return "UnicodeBiDi";
			case StyleId.Calendar:
				return "Calendar";
			case StyleId.CurrencyLanguage:
				return "CurrencyLanguage";
			case StyleId.NumeralLanguage:
				return "NumeralLanguage";
			case StyleId.NumeralVariant:
				return "NumeralVariant";
			case StyleId.BackgroundGradientType:
				return "BackgroundGradientType";
			case StyleId.BackgroundGradientEndColor:
				return "BackgroundGradientEndColor";
			case StyleId.BackgroundImage:
				return "BackgroundImage";
			case StyleId.BackgroundImageRepeat:
				return "BackgroundRepeat";
			case StyleId.BackgroundImageSource:
				return "BackgroundImageSource";
			case StyleId.BackgroundImageValue:
				return "BackgroundImageValue";
			case StyleId.BackgroundImageMimeType:
				return "BackgroundImageMIMEType";
			case StyleId.Position:
				return "Position";
			case StyleId.TransparentColor:
				return "TransparentColor";
			case StyleId.TextEffect:
				return "TextEffect";
			case StyleId.ShadowColor:
				return "ShadowColor";
			case StyleId.ShadowOffset:
				return "ShadowOffset";
			case StyleId.BackgroundHatchType:
				return "BackgroundHatchType";
			default:
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal static StyleId GetStyleId(string styleString)
		{
			switch (styleString)
			{
			case "BorderColor":
				return StyleId.BorderColor;
			case "BorderColorTop":
				return StyleId.BorderColorTop;
			case "BorderColorLeft":
				return StyleId.BorderColorLeft;
			case "BorderColorRight":
				return StyleId.BorderColorRight;
			case "BorderColorBottom":
				return StyleId.BorderColorBottom;
			case "BorderStyle":
				return StyleId.BorderStyle;
			case "BorderStyleTop":
				return StyleId.BorderStyleTop;
			case "BorderStyleLeft":
				return StyleId.BorderStyleLeft;
			case "BorderStyleRight":
				return StyleId.BorderStyleRight;
			case "BorderStyleBottom":
				return StyleId.BorderStyleBottom;
			case "BorderWidth":
				return StyleId.BorderWidth;
			case "BorderWidthTop":
				return StyleId.BorderWidthTop;
			case "BorderWidthLeft":
				return StyleId.BorderWidthLeft;
			case "BorderWidthRight":
				return StyleId.BorderWidthRight;
			case "BorderWidthBottom":
				return StyleId.BorderWidthBottom;
			case "BackgroundColor":
				return StyleId.BackgroundColor;
			case "FontStyle":
				return StyleId.FontStyle;
			case "FontFamily":
				return StyleId.FontFamily;
			case "FontSize":
				return StyleId.FontSize;
			case "FontWeight":
				return StyleId.FontWeight;
			case "Format":
				return StyleId.Format;
			case "TextDecoration":
				return StyleId.TextDecoration;
			case "TextAlign":
				return StyleId.TextAlign;
			case "VerticalAlign":
				return StyleId.VerticalAlign;
			case "Color":
				return StyleId.Color;
			case "PaddingLeft":
				return StyleId.PaddingLeft;
			case "PaddingRight":
				return StyleId.PaddingRight;
			case "PaddingTop":
				return StyleId.PaddingTop;
			case "PaddingBottom":
				return StyleId.PaddingBottom;
			case "LineHeight":
				return StyleId.LineHeight;
			case "Direction":
				return StyleId.Direction;
			case "WritingMode":
				return StyleId.WritingMode;
			case "Language":
				return StyleId.Language;
			case "UnicodeBiDi":
				return StyleId.UnicodeBiDi;
			case "Calendar":
				return StyleId.Calendar;
			case "CurrencyLanguage":
				return StyleId.CurrencyLanguage;
			case "NumeralLanguage":
				return StyleId.NumeralLanguage;
			case "NumeralVariant":
				return StyleId.NumeralVariant;
			case "BackgroundGradientType":
				return StyleId.BackgroundGradientType;
			case "BackgroundGradientEndColor":
				return StyleId.BackgroundGradientEndColor;
			case "BackgroundImage":
				return StyleId.BackgroundImage;
			case "BackgroundRepeat":
				return StyleId.BackgroundImageRepeat;
			case "BackgroundImageSource":
				return StyleId.BackgroundImageSource;
			case "BackgroundImageValue":
				return StyleId.BackgroundImageValue;
			case "BackgroundImageMIMEType":
				return StyleId.BackgroundImageMimeType;
			case "TextEffect":
				return StyleId.TextEffect;
			case "ShadowColor":
				return StyleId.ShadowColor;
			case "ShadowOffset":
				return StyleId.ShadowOffset;
			case "BackgroundHatchType":
				return StyleId.BackgroundHatchType;
			default:
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
		}

		internal Style(ConstructionPhase phase)
		{
			if (phase == ConstructionPhase.Publishing)
			{
				m_styleAttributes = new Dictionary<string, AttributeInfo>();
			}
		}

		internal void SetStyleExprHost(StyleExprHost exprHost)
		{
			Global.Tracer.Assert(exprHost != null, "(null != exprHost)");
			m_exprHost = exprHost;
		}

		internal int GetStyleAttribute(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context, ref bool sharedFormatSettings, out string styleStringValue)
		{
			styleStringValue = null;
			int result = 0;
			object obj = null;
			AttributeInfo styleAttribute = null;
			if (GetAttributeInfo(styleAttributeName, out styleAttribute))
			{
				if (styleAttribute.IsExpression)
				{
					result = 1;
					sharedFormatSettings = false;
					obj = EvaluateStyle(objectType, objectName, styleAttributeName, context);
				}
				else
				{
					result = 2;
					obj = styleAttribute.Value;
				}
			}
			if (obj != null)
			{
				styleStringValue = (string)obj;
			}
			return result;
		}

		internal void GetStyleAttribute(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context, ref bool sharedFormatSettings, out int styleIntValue)
		{
			styleIntValue = 0;
			AttributeInfo styleAttribute = null;
			if (!GetAttributeInfo(styleAttributeName, out styleAttribute))
			{
				return;
			}
			if (styleAttribute.IsExpression)
			{
				sharedFormatSettings = false;
				object obj = EvaluateStyle(objectType, objectName, styleAttributeName, context);
				if (obj != null)
				{
					styleIntValue = (int)obj;
				}
			}
			else
			{
				styleIntValue = styleAttribute.IntValue;
			}
		}

		internal virtual bool GetAttributeInfo(string styleAttributeName, out AttributeInfo styleAttribute)
		{
			if (m_styleAttributes.TryGetValue(styleAttributeName, out styleAttribute) && styleAttribute != null)
			{
				return true;
			}
			return false;
		}

		internal object EvaluateStyle(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, StyleId styleId, OnDemandProcessingContext context)
		{
			AttributeInfo styleAttribute = null;
			if (GetAttributeInfo(GetStyleString(styleId), out styleAttribute))
			{
				return EvaluateStyle(objectType, objectName, styleAttribute, styleId, context);
			}
			return null;
		}

		internal object EvaluateStyle(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, string styleAttributeName, OnDemandProcessingContext context)
		{
			AttributeInfo styleAttribute = null;
			if (GetAttributeInfo(styleAttributeName, out styleAttribute))
			{
				StyleId styleId = GetStyleId(styleAttributeName);
				return EvaluateStyle(objectType, objectName, styleAttribute, styleId, context);
			}
			return null;
		}

		internal object EvaluateStyle(Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName, AttributeInfo attribute, StyleId styleId, OnDemandProcessingContext context)
		{
			if (attribute != null)
			{
				if (!attribute.IsExpression)
				{
					if (StyleId.NumeralLanguage == styleId || StyleId.BackgroundImageSource == styleId)
					{
						return attribute.IntValue;
					}
					if (attribute.Value != null && attribute.Value.Length != 0)
					{
						return attribute.Value;
					}
					return null;
				}
				switch (styleId)
				{
				case StyleId.BackgroundImageSource:
					return null;
				case StyleId.BackgroundImageValue:
				{
					AttributeInfo attributeInfo = m_styleAttributes["BackgroundImageSource"];
					if (attributeInfo == null)
					{
						return null;
					}
					switch (attributeInfo.IntValue)
					{
					case 2:
						return context.ReportRuntime.EvaluateStyleBackgroundDatabaseImageValue(this, m_expressionList[attribute.IntValue], objectType, objectName);
					case 1:
						return context.ReportRuntime.EvaluateStyleBackgroundEmbeddedImageValue(this, m_expressionList[attribute.IntValue], context.EmbeddedImages, objectType, objectName);
					case 0:
						return context.ReportRuntime.EvaluateStyleBackgroundUrlImageValue(this, m_expressionList[attribute.IntValue], objectType, objectName);
					}
					break;
				}
				case StyleId.BackgroundImageMimeType:
					return context.ReportRuntime.EvaluateStyleBackgroundImageMIMEType(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundImageRepeat:
					return context.ReportRuntime.EvaluateStyleBackgroundRepeat(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundColor:
					return context.ReportRuntime.EvaluateStyleBackgroundColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundGradientType:
					return context.ReportRuntime.EvaluateStyleBackgroundGradientType(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundGradientEndColor:
					return context.ReportRuntime.EvaluateStyleBackgroundGradientEndColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Color:
					return context.ReportRuntime.EvaluateStyleColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontSize:
					return context.ReportRuntime.EvaluateStyleFontSize(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontStyle:
					return context.ReportRuntime.EvaluateStyleFontStyle(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontFamily:
					return context.ReportRuntime.EvaluateStyleFontFamily(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.FontWeight:
					return context.ReportRuntime.EvaluateStyleFontWeight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Format:
					return context.ReportRuntime.EvaluateStyleFormat(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextDecoration:
					return context.ReportRuntime.EvaluateStyleTextDecoration(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextAlign:
					return context.ReportRuntime.EvaluateStyleTextAlign(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.VerticalAlign:
					return context.ReportRuntime.EvaluateStyleVerticalAlign(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Direction:
					return context.ReportRuntime.EvaluateStyleDirection(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.WritingMode:
					return context.ReportRuntime.EvaluateStyleWritingMode(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Language:
					return context.ReportRuntime.EvaluateStyleLanguage(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.UnicodeBiDi:
					return context.ReportRuntime.EvaluateStyleUnicodeBiDi(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Calendar:
					return context.ReportRuntime.EvaluateStyleCalendar(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.CurrencyLanguage:
					return context.ReportRuntime.EvaluateStyleCurrencyLanguage(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.NumeralLanguage:
					return context.ReportRuntime.EvaluateStyleNumeralLanguage(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.LineHeight:
					return context.ReportRuntime.EvaluateStyleLineHeight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.NumeralVariant:
					return context.ReportRuntime.EvaluateStyleNumeralVariant(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColor:
					return context.ReportRuntime.EvaluateStyleBorderColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorBottom:
					return context.ReportRuntime.EvaluateStyleBorderColorBottom(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorLeft:
					return context.ReportRuntime.EvaluateStyleBorderColorLeft(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorRight:
					return context.ReportRuntime.EvaluateStyleBorderColorRight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderColorTop:
					return context.ReportRuntime.EvaluateStyleBorderColorTop(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyle:
					return context.ReportRuntime.EvaluateStyleBorderStyle(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleTop:
					return context.ReportRuntime.EvaluateStyleBorderStyleTop(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleLeft:
					return context.ReportRuntime.EvaluateStyleBorderStyleLeft(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleRight:
					return context.ReportRuntime.EvaluateStyleBorderStyleRight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderStyleBottom:
					return context.ReportRuntime.EvaluateStyleBorderStyleBottom(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidth:
					return context.ReportRuntime.EvaluateStyleBorderWidth(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthTop:
					return context.ReportRuntime.EvaluateStyleBorderWidthTop(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthLeft:
					return context.ReportRuntime.EvaluateStyleBorderWidthLeft(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthRight:
					return context.ReportRuntime.EvaluateStyleBorderWidthRight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BorderWidthBottom:
					return context.ReportRuntime.EvaluateStyleBorderWidthBottom(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingLeft:
					return context.ReportRuntime.EvaluateStylePaddingLeft(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingRight:
					return context.ReportRuntime.EvaluateStylePaddingRight(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingTop:
					return context.ReportRuntime.EvaluateStylePaddingTop(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.PaddingBottom:
					return context.ReportRuntime.EvaluateStylePaddingBottom(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TextEffect:
					return context.ReportRuntime.EvaluateStyleTextEffect(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.ShadowColor:
					return context.ReportRuntime.EvaluateStyleShadowColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.ShadowOffset:
					return context.ReportRuntime.EvaluateStyleShadowOffset(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.BackgroundHatchType:
					return context.ReportRuntime.EvaluateStyleBackgroundHatchType(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.Position:
					return context.ReportRuntime.EvaluatePosition(this, m_expressionList[attribute.IntValue], objectType, objectName);
				case StyleId.TransparentColor:
					return context.ReportRuntime.EvaluateTransparentColor(this, m_expressionList[attribute.IntValue], objectType, objectName);
				}
			}
			return null;
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo)
		{
			AddAttribute(name, expressionInfo, ValueType.Constant);
		}

		internal void AddAttribute(StyleInformation.StyleInformationAttribute attribute)
		{
			AddAttribute(attribute.Name, attribute.Value, attribute.ValueType);
		}

		internal void AddAttribute(string name, ExpressionInfo expressionInfo, ValueType valueType)
		{
			AttributeInfo attributeInfo = new AttributeInfo();
			attributeInfo.ValueType = valueType;
			attributeInfo.IsExpression = (ExpressionInfo.Types.Constant != expressionInfo.Type);
			if (attributeInfo.IsExpression)
			{
				if (m_expressionList == null)
				{
					m_expressionList = new List<ExpressionInfo>();
				}
				m_expressionList.Add(expressionInfo);
				attributeInfo.IntValue = m_expressionList.Count - 1;
			}
			else
			{
				attributeInfo.Value = expressionInfo.StringValue;
				attributeInfo.BoolValue = expressionInfo.BoolValue;
				attributeInfo.IntValue = expressionInfo.IntValue;
				attributeInfo.FloatValue = expressionInfo.FloatValue;
			}
			Global.Tracer.Assert(m_styleAttributes != null, "(null != m_styleAttributes)");
			m_styleAttributes.Add(name, attributeInfo);
		}

		internal void Initialize(InitializationContext context)
		{
			Global.Tracer.Assert(m_styleAttributes != null, "(null != m_styleAttributes)");
			IDictionaryEnumerator dictionaryEnumerator = m_styleAttributes.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				string text = (string)dictionaryEnumerator.Key;
				AttributeInfo attributeInfo = (AttributeInfo)dictionaryEnumerator.Value;
				Global.Tracer.Assert(text != null, "(null != name)");
				Global.Tracer.Assert(attributeInfo != null, "(null != attribute)");
				if (attributeInfo.IsExpression)
				{
					string name = text;
					switch (text)
					{
					case "BorderColorLeft":
					case "BorderColorRight":
					case "BorderColorTop":
					case "BorderColorBottom":
						text = "BorderColor";
						break;
					case "BorderStyleLeft":
					case "BorderStyleRight":
					case "BorderStyleTop":
					case "BorderStyleBottom":
						text = "BorderStyle";
						break;
					case "BorderWidthLeft":
					case "BorderWidthRight":
					case "BorderWidthTop":
					case "BorderWidthBottom":
						text = "BorderWidth";
						break;
					}
					Global.Tracer.Assert(m_expressionList != null, "(null != m_expressionList)");
					ExpressionInfo expressionInfo = m_expressionList[attributeInfo.IntValue];
					expressionInfo.Initialize(text, context);
					context.ExprHostBuilder.StyleAttribute(name, expressionInfo);
				}
			}
			m_styleAttributes.TryGetValue("BackgroundImageSource", out AttributeInfo value);
			if (value != null)
			{
				Global.Tracer.Assert(!value.IsExpression, "(!source.IsExpression)");
				Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType intValue = (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType)value.IntValue;
				if (Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded == intValue && (!m_styleAttributes.TryGetValue("EmbeddingMode", out AttributeInfo value2) || value2.IntValue != 1))
				{
					AttributeInfo attributeInfo2 = m_styleAttributes["BackgroundImageValue"];
					Global.Tracer.Assert(attributeInfo2 != null, "(null != embeddedImageName)");
					Microsoft.ReportingServices.ReportPublishing.PublishingValidator.ValidateEmbeddedImageName(attributeInfo2, context.EmbeddedImages, context.ObjectType, context.ObjectName, "BackgroundImageValue", context.ErrorContext);
				}
			}
			context.CheckInternationalSettings(m_styleAttributes);
		}

		public object PublishClone(AutomaticSubtotalContext context)
		{
			Style style = (Style)MemberwiseClone();
			if (m_styleAttributes != null)
			{
				style.m_styleAttributes = new Dictionary<string, AttributeInfo>(m_styleAttributes.Count);
				foreach (KeyValuePair<string, AttributeInfo> styleAttribute in m_styleAttributes)
				{
					style.m_styleAttributes.Add(styleAttribute.Key, styleAttribute.Value.PublishClone(context));
				}
			}
			if (m_expressionList != null)
			{
				style.m_expressionList = new List<ExpressionInfo>(m_expressionList.Count);
				{
					foreach (ExpressionInfo expression in m_expressionList)
					{
						style.m_expressionList.Add((ExpressionInfo)expression.PublishClone(context));
					}
					return style;
				}
			}
			return style;
		}

		internal void InitializeForCRIGeneratedReportItem()
		{
			SetStyleExprHost(EmptyStyleExprHost.Instance);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StyleAttributes, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StringRIFObjectDictionary, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo));
			list.Add(new MemberInfo(MemberName.ExpressionList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					writer.WriteStringRIFObjectDictionary(m_styleAttributes);
					break;
				case MemberName.ExpressionList:
					writer.Write(m_expressionList);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					m_styleAttributes = reader.ReadStringRIFObjectDictionary<AttributeInfo>();
					break;
				case MemberName.ExpressionList:
					m_expressionList = reader.ReadGenericListOfRIFObjects<ExpressionInfo>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(condition: false);
		}

		public Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style;
		}
	}
}
