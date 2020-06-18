using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Style : StyleBase
	{
		public enum StyleName
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
			NumeralVariant
		}

		internal sealed class StyleDefaults
		{
			private Hashtable m_nameMap;

			private string[] m_keyCollection;

			private object[] m_valueCollection;

			internal object this[int index] => m_valueCollection[index];

			internal object this[string styleName] => m_valueCollection[(int)m_nameMap[styleName]];

			internal StyleDefaults(bool isLine)
			{
				m_nameMap = new Hashtable(42);
				m_keyCollection = new string[42];
				m_valueCollection = new object[42];
				int num = 0;
				m_nameMap["BorderColor"] = num;
				m_keyCollection[num] = "BorderColor";
				m_valueCollection[num++] = new ReportColor("Black", parsed: false);
				m_nameMap["BorderColorTop"] = num;
				m_keyCollection[num] = "BorderColorTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorLeft"] = num;
				m_keyCollection[num] = "BorderColorLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorRight"] = num;
				m_keyCollection[num] = "BorderColorRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderColorBottom"] = num;
				m_keyCollection[num] = "BorderColorBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyle"] = num;
				m_keyCollection[num] = "BorderStyle";
				if (!isLine)
				{
					m_valueCollection[num++] = "None";
				}
				else
				{
					m_valueCollection[num++] = "Solid";
				}
				m_nameMap["BorderStyleTop"] = num;
				m_keyCollection[num] = "BorderStyleTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleLeft"] = num;
				m_keyCollection[num] = "BorderStyleLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleRight"] = num;
				m_keyCollection[num] = "BorderStyleRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderStyleBottom"] = num;
				m_keyCollection[num] = "BorderStyleBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidth"] = num;
				m_keyCollection[num] = "BorderWidth";
				m_valueCollection[num++] = new ReportSize("1pt", parsed: false);
				m_nameMap["BorderWidthTop"] = num;
				m_keyCollection[num] = "BorderWidthTop";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthLeft"] = num;
				m_keyCollection[num] = "BorderWidthLeft";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthRight"] = num;
				m_keyCollection[num] = "BorderWidthRight";
				m_valueCollection[num++] = null;
				m_nameMap["BorderWidthBottom"] = num;
				m_keyCollection[num] = "BorderWidthBottom";
				m_valueCollection[num++] = null;
				m_nameMap["BackgroundColor"] = num;
				m_keyCollection[num] = "BackgroundColor";
				m_valueCollection[num++] = new ReportColor("Transparent", parsed: false);
				m_nameMap["BackgroundGradientType"] = num;
				m_keyCollection[num] = "BackgroundGradientType";
				m_valueCollection[num++] = "None";
				m_nameMap["BackgroundGradientEndColor"] = num;
				m_keyCollection[num] = "BackgroundGradientEndColor";
				m_valueCollection[num++] = new ReportColor("Transparent", parsed: false);
				m_nameMap["BackgroundImage"] = num;
				m_keyCollection[num] = "BackgroundImage";
				m_valueCollection[num++] = null;
				m_nameMap["BackgroundRepeat"] = num;
				m_keyCollection[num] = "BackgroundRepeat";
				m_valueCollection[num++] = "Repeat";
				m_nameMap["FontStyle"] = num;
				m_keyCollection[num] = "FontStyle";
				m_valueCollection[num++] = "Normal";
				m_nameMap["FontFamily"] = num;
				m_keyCollection[num] = "FontFamily";
				m_valueCollection[num++] = "Arial";
				m_nameMap["FontSize"] = num;
				m_keyCollection[num] = "FontSize";
				m_valueCollection[num++] = new ReportSize("10pt", parsed: false);
				m_nameMap["FontWeight"] = num;
				m_keyCollection[num] = "FontWeight";
				m_valueCollection[num++] = "Normal";
				m_nameMap["Format"] = num;
				m_keyCollection[num] = "Format";
				m_valueCollection[num++] = null;
				m_nameMap["TextDecoration"] = num;
				m_keyCollection[num] = "TextDecoration";
				m_valueCollection[num++] = "None";
				m_nameMap["TextAlign"] = num;
				m_keyCollection[num] = "TextAlign";
				m_valueCollection[num++] = "General";
				m_nameMap["VerticalAlign"] = num;
				m_keyCollection[num] = "VerticalAlign";
				m_valueCollection[num++] = "Top";
				m_nameMap["Color"] = num;
				m_keyCollection[num] = "Color";
				m_valueCollection[num++] = new ReportColor("Black", parsed: false);
				m_nameMap["PaddingLeft"] = num;
				m_keyCollection[num] = "PaddingLeft";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingRight"] = num;
				m_keyCollection[num] = "PaddingRight";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingTop"] = num;
				m_keyCollection[num] = "PaddingTop";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["PaddingBottom"] = num;
				m_keyCollection[num] = "PaddingBottom";
				m_valueCollection[num++] = new ReportSize("0pt", 0.0);
				m_nameMap["LineHeight"] = num;
				m_keyCollection[num] = "LineHeight";
				m_valueCollection[num++] = null;
				m_nameMap["Direction"] = num;
				m_keyCollection[num] = "Direction";
				m_valueCollection[num++] = "LTR";
				m_nameMap["WritingMode"] = num;
				m_keyCollection[num] = "WritingMode";
				m_valueCollection[num++] = "lr-tb";
				m_nameMap["Language"] = num;
				m_keyCollection[num] = "Language";
				m_valueCollection[num++] = null;
				m_nameMap["UnicodeBiDi"] = num;
				m_keyCollection[num] = "UnicodeBiDi";
				m_valueCollection[num++] = "Normal";
				m_nameMap["Calendar"] = num;
				m_keyCollection[num] = "Calendar";
				m_valueCollection[num++] = "Gregorian";
				m_nameMap["NumeralLanguage"] = num;
				m_keyCollection[num] = "NumeralLanguage";
				m_valueCollection[num++] = null;
				m_nameMap["NumeralVariant"] = num;
				m_keyCollection[num] = "NumeralVariant";
				m_valueCollection[num++] = 1;
				m_nameMap["CurrencyLanguage"] = num;
				m_keyCollection[num] = "CurrencyLanguage";
				m_valueCollection[num++] = null;
				Global.Tracer.Assert(42 == num);
			}

			internal string GetName(int index)
			{
				return m_keyCollection[index];
			}
		}

		private ReportItem m_reportItem;

		private Microsoft.ReportingServices.ReportProcessing.ReportItem m_reportItemDef;

		private StyleDefaults m_styleDefaults;

		private static StyleDefaults NormalStyleDefaults = new StyleDefaults(isLine: false);

		private static StyleDefaults LineStyleDefaults = new StyleDefaults(isLine: true);

		public override int Count
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Count;
				}
				if (m_reportItemDef.StyleClass == null)
				{
					return 0;
				}
				return base.Count;
			}
		}

		public override ICollection Keys
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Keys;
				}
				if (m_reportItemDef.StyleClass == null)
				{
					return null;
				}
				return base.Keys;
			}
		}

		public override object this[string styleName]
		{
			get
			{
				if (base.IsCustomControl)
				{
					object obj = null;
					if (m_nonSharedProperties != null)
					{
						obj = m_nonSharedProperties[styleName];
					}
					if (obj == null && m_sharedProperties != null)
					{
						obj = m_sharedProperties[styleName];
					}
					return CreatePropertyOrReturnDefault(styleName, obj);
				}
				Global.Tracer.Assert(!base.IsCustomControl);
				if (m_reportItem.HeadingInstance == null && m_reportItemDef.StyleClass == null)
				{
					return m_styleDefaults[styleName];
				}
				StyleAttributeHashtable styleAttributeHashtable = null;
				if (m_reportItemDef.StyleClass != null)
				{
					styleAttributeHashtable = m_reportItemDef.StyleClass.StyleAttributes;
				}
				StyleAttributeHashtable styleAttributeHashtable2 = null;
				if (m_reportItem.HeadingInstance != null)
				{
					Global.Tracer.Assert(m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
					styleAttributeHashtable2 = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
				}
				AttributeInfo attributeInfo = null;
				if ("BackgroundImage" == styleName)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object imageValue = null;
					object mimeType = null;
					bool isMimeTypeExpression = false;
					bool isValueExpression;
					if (styleAttributeHashtable2 != null)
					{
						GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression);
					}
					if (imageValue == null && styleAttributeHashtable != null)
					{
						GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression);
					}
					if (imageValue != null)
					{
						string mimeType2 = null;
						if (!isMimeTypeExpression)
						{
							mimeType2 = (string)mimeType;
						}
						return new BackgroundImage(m_renderingContext, imageSource, imageValue, mimeType2);
					}
				}
				else
				{
					if (styleAttributeHashtable2 != null)
					{
						AttributeInfo attributeInfo2 = styleAttributeHashtable2[styleName];
						if (attributeInfo2 != null)
						{
							return CreatePropertyOrReturnDefault(styleName, GetStyleAttributeValue(styleName, attributeInfo2));
						}
					}
					if (styleAttributeHashtable != null)
					{
						attributeInfo = styleAttributeHashtable[styleName];
						if (attributeInfo != null)
						{
							return CreatePropertyOrReturnDefault(styleName, GetStyleAttributeValue(styleName, attributeInfo));
						}
					}
				}
				return m_styleDefaults[styleName];
			}
		}

		public override StyleProperties SharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return m_sharedProperties;
				}
				if (NeedPopulateSharedProps())
				{
					PopulateStyleProperties(populateAll: false);
					m_reportItem.ReportItemDef.SharedStyleProperties = m_sharedProperties;
				}
				return m_sharedProperties;
			}
		}

		public override StyleProperties NonSharedProperties
		{
			get
			{
				if (base.IsCustomControl)
				{
					return m_nonSharedProperties;
				}
				if (NeedPopulateNonSharedProps())
				{
					PopulateNonSharedStyleProperties();
					if (m_nonSharedProperties == null || m_nonSharedProperties.Count == 0)
					{
						m_reportItemDef.NoNonSharedStyleProps = true;
					}
				}
				return m_nonSharedProperties;
			}
		}

		public Style()
		{
			Global.Tracer.Assert(base.IsCustomControl);
			m_styleDefaults = NormalStyleDefaults;
		}

		internal Style(ReportItem reportItem, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, RenderingContext context)
			: base(context)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			m_reportItem = reportItem;
			m_reportItemDef = reportItemDef;
			if (reportItem is Line)
			{
				m_styleDefaults = LineStyleDefaults;
			}
			else
			{
				m_styleDefaults = NormalStyleDefaults;
			}
		}

		internal bool HasBackgroundImage(out bool isExpressionBased)
		{
			isExpressionBased = false;
			if (m_reportItem.HeadingInstance == null && m_reportItemDef.StyleClass == null)
			{
				return false;
			}
			if (GetStyleDefinition("BackgroundImageValue") == null)
			{
				return false;
			}
			StyleAttributeHashtable styleAttributeHashtable = null;
			if (m_reportItemDef.StyleClass != null)
			{
				styleAttributeHashtable = m_reportItemDef.StyleClass.StyleAttributes;
			}
			StyleAttributeHashtable styleAttributeHashtable2 = null;
			if (m_reportItem.HeadingInstance != null)
			{
				Global.Tracer.Assert(m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
				styleAttributeHashtable2 = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
			}
			Image.SourceType imageSource = Image.SourceType.External;
			object imageValue = null;
			object mimeType = null;
			object repeat = null;
			bool isValueExpression = false;
			bool isMimeTypeExpression = false;
			bool isRepeatExpression = false;
			if (styleAttributeHashtable2 != null)
			{
				GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], styleAttributeHashtable2["BackgroundRepeat"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression, out repeat, out isRepeatExpression);
			}
			if (imageValue == null && styleAttributeHashtable != null)
			{
				GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], styleAttributeHashtable["BackgroundRepeat"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression, out repeat, out isRepeatExpression);
			}
			if (imageValue != null)
			{
				isExpressionBased = (isValueExpression || isMimeTypeExpression || isRepeatExpression);
				return true;
			}
			return false;
		}

		internal AttributeInfo GetStyleDefinition(string styleName)
		{
			string expressionString = null;
			return GetStyleDefinition(styleName, out expressionString);
		}

		internal AttributeInfo GetStyleDefinition(string styleName, out string expressionString)
		{
			expressionString = null;
			if (base.IsCustomControl)
			{
				return null;
			}
			if (m_reportItem.HeadingInstance == null && m_reportItemDef.StyleClass == null)
			{
				return null;
			}
			StyleAttributeHashtable styleAttributeHashtable = null;
			ExpressionInfoList expressionInfoList = null;
			if (m_reportItem.HeadingInstance != null)
			{
				Global.Tracer.Assert(m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
				styleAttributeHashtable = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
				expressionInfoList = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.ExpressionList;
			}
			AttributeInfo attributeInfo = null;
			if ((styleAttributeHashtable == null || !styleAttributeHashtable.ContainsKey(styleName)) && m_reportItemDef.StyleClass != null)
			{
				styleAttributeHashtable = m_reportItemDef.StyleClass.StyleAttributes;
				expressionInfoList = m_reportItemDef.StyleClass.ExpressionList;
			}
			if (styleAttributeHashtable != null)
			{
				attributeInfo = styleAttributeHashtable[styleName];
				if (attributeInfo != null && attributeInfo.IsExpression)
				{
					expressionString = expressionInfoList[attributeInfo.IntValue].OriginalText;
				}
			}
			return attributeInfo;
		}

		private bool NeedPopulateSharedProps()
		{
			if (base.IsCustomControl)
			{
				return false;
			}
			if (m_reportItem.HeadingInstance != null)
			{
				return true;
			}
			if (m_sharedProperties != null)
			{
				return false;
			}
			if (m_reportItemDef.SharedStyleProperties != null)
			{
				if (42 != m_reportItemDef.SharedStyleProperties.Count + ((m_nonSharedProperties != null) ? m_nonSharedProperties.Count : 0))
				{
					return true;
				}
				m_sharedProperties = m_reportItemDef.SharedStyleProperties;
				return false;
			}
			return true;
		}

		private bool NeedPopulateNonSharedProps()
		{
			if (base.IsCustomControl)
			{
				return false;
			}
			if (m_reportItem.HeadingInstance != null)
			{
				return true;
			}
			if (m_nonSharedProperties == null && !m_reportItemDef.NoNonSharedStyleProps)
			{
				return true;
			}
			return false;
		}

		internal static object GetStyleValue(string styleName, Microsoft.ReportingServices.ReportProcessing.Style styleDef, object[] styleAttributeValues)
		{
			return GetStyleValue(styleName, styleDef, styleAttributeValues, returnDefaultStyle: true);
		}

		internal static object GetStyleValue(string styleName, Microsoft.ReportingServices.ReportProcessing.Style styleDef, object[] styleAttributeValues, bool returnDefaultStyle)
		{
			object styleValueBase = StyleBase.GetStyleValueBase(styleName, styleDef, styleAttributeValues);
			if (styleValueBase != null)
			{
				return styleValueBase;
			}
			if (returnDefaultStyle)
			{
				return NormalStyleDefaults[styleName];
			}
			return null;
		}

		internal override object GetStyleAttributeValue(string styleName, AttributeInfo attribute)
		{
			if (m_reportItem.HeadingInstance != null)
			{
				Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass;
				Global.Tracer.Assert(styleClass != null);
				AttributeInfo attributeInfo = styleClass.StyleAttributes[styleName];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						MatrixSubtotalHeadingInstanceInfo matrixSubtotalHeadingInstanceInfo = m_reportItem.HeadingInstance.GetInstanceInfo(m_reportItem.RenderingContext.ChunkManager) as MatrixSubtotalHeadingInstanceInfo;
						Global.Tracer.Assert(matrixSubtotalHeadingInstanceInfo != null);
						Global.Tracer.Assert(matrixSubtotalHeadingInstanceInfo.StyleAttributeValues != null);
						Global.Tracer.Assert(0 <= attributeInfo.IntValue && attributeInfo.IntValue < matrixSubtotalHeadingInstanceInfo.StyleAttributeValues.Length);
						return matrixSubtotalHeadingInstanceInfo.StyleAttributeValues[attributeInfo.IntValue];
					}
					if ("NumeralVariant" == styleName)
					{
						return attributeInfo.IntValue;
					}
					return attributeInfo.Value;
				}
			}
			if (attribute.IsExpression)
			{
				return m_reportItem.InstanceInfo?.GetStyleAttributeValue(attribute.IntValue);
			}
			if ("NumeralVariant" == styleName)
			{
				return attribute.IntValue;
			}
			return attribute.Value;
		}

		internal override void PopulateStyleProperties(bool populateAll)
		{
			if (base.IsCustomControl)
			{
				return;
			}
			bool flag = true;
			bool flag2 = false;
			if (populateAll)
			{
				flag = NeedPopulateSharedProps();
				flag2 = NeedPopulateNonSharedProps();
				if (!flag && !flag2)
				{
					return;
				}
			}
			Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_reportItemDef.StyleClass;
			StyleAttributeHashtable styleAttributeHashtable = null;
			if (styleClass != null)
			{
				styleAttributeHashtable = styleClass.StyleAttributes;
			}
			StyleAttributeHashtable styleAttributeHashtable2 = null;
			if (m_reportItem.HeadingInstance != null)
			{
				Global.Tracer.Assert(m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
				styleAttributeHashtable2 = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
			}
			for (int i = 0; i < 42; i++)
			{
				string name = m_styleDefaults.GetName(i);
				if (styleAttributeHashtable == null && styleAttributeHashtable2 == null)
				{
					AddStyleProperty(name, isExpression: false, flag2, flag, m_styleDefaults[i]);
				}
				else if (styleAttributeHashtable2 != null && styleAttributeHashtable2.ContainsKey(name))
				{
					AttributeInfo attribute = styleAttributeHashtable2[name];
					AddStyleProperty(name, isExpression: true, needNonSharedProps: true, needSharedProps: false, CreatePropertyOrReturnDefault(name, GetStyleAttributeValue(name, attribute)));
				}
				else if (styleAttributeHashtable != null && styleAttributeHashtable.ContainsKey(name))
				{
					AttributeInfo attributeInfo = styleAttributeHashtable[name];
					AddStyleProperty(name, attributeInfo.IsExpression, flag2, flag, CreatePropertyOrReturnDefault(name, GetStyleAttributeValue(name, attributeInfo)));
				}
				else if ("BackgroundImage" == name)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object imageValue = null;
					object mimeType = null;
					bool isValueExpression = false;
					bool isMimeTypeExpression = false;
					bool flag3 = false;
					if (styleAttributeHashtable2 != null)
					{
						flag3 = GetBackgroundImageProperties(styleAttributeHashtable2["BackgroundImageSource"], styleAttributeHashtable2["BackgroundImageValue"], styleAttributeHashtable2["BackgroundImageMIMEType"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression);
					}
					if (!flag3 && styleAttributeHashtable != null)
					{
						flag3 = GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression);
					}
					object styleProperty;
					if (imageValue != null)
					{
						string mimeType2 = null;
						if (!isMimeTypeExpression)
						{
							mimeType2 = (string)mimeType;
						}
						styleProperty = new BackgroundImage(m_renderingContext, imageSource, imageValue, mimeType2);
					}
					else
					{
						styleProperty = m_styleDefaults[i];
					}
					AddStyleProperty(name, isValueExpression || isMimeTypeExpression, flag2, flag, styleProperty);
				}
				else
				{
					AddStyleProperty(name, isExpression: false, flag2, flag, m_styleDefaults[i]);
				}
			}
		}

		private void PopulateNonSharedStyleProperties()
		{
			if (!base.IsCustomControl)
			{
				Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_reportItemDef.StyleClass;
				if (styleClass != null)
				{
					StyleAttributeHashtable styleAttributes = styleClass.StyleAttributes;
					Global.Tracer.Assert(styleAttributes != null);
					InternalPopulateNonSharedStyleProperties(styleAttributes, isSubtotal: false);
				}
				if (m_reportItem.HeadingInstance != null)
				{
					Global.Tracer.Assert(m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
					StyleAttributeHashtable styleAttributes2 = m_reportItem.HeadingInstance.MatrixHeadingDef.Subtotal.StyleClass.StyleAttributes;
					InternalPopulateNonSharedStyleProperties(styleAttributes2, isSubtotal: true);
				}
			}
		}

		private void InternalPopulateNonSharedStyleProperties(StyleAttributeHashtable styleAttributes, bool isSubtotal)
		{
			if (base.IsCustomControl || styleAttributes == null)
			{
				return;
			}
			IDictionaryEnumerator enumerator = styleAttributes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
				string text = (string)enumerator.Key;
				if ("BackgroundImageSource" == text)
				{
					if (!GetBackgroundImageProperties(attributeInfo, styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out Image.SourceType imageSource, out object imageValue, out bool isValueExpression, out object mimeType, out bool isMimeTypeExpression) || !(isValueExpression || isMimeTypeExpression))
					{
						continue;
					}
					object styleProperty;
					if (imageValue != null)
					{
						string mimeType2 = null;
						if (!isMimeTypeExpression)
						{
							mimeType2 = (string)mimeType;
						}
						styleProperty = new BackgroundImage(m_renderingContext, imageSource, imageValue, mimeType2);
					}
					else
					{
						styleProperty = m_styleDefaults["BackgroundImage"];
					}
					SetStyleProperty("BackgroundImage", isExpression: true, needNonSharedProps: true, needSharedProps: false, styleProperty);
				}
				else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text) && (isSubtotal || attributeInfo.IsExpression))
				{
					SetStyleProperty(text, isExpression: true, needNonSharedProps: true, needSharedProps: false, CreatePropertyOrReturnDefault(text, GetStyleAttributeValue(text, attributeInfo)));
				}
			}
		}

		private object CreatePropertyOrReturnDefault(string styleName, object styleValue)
		{
			if (styleValue == null)
			{
				return m_styleDefaults[styleName];
			}
			return StyleBase.CreateStyleProperty(styleName, styleValue);
		}
	}
}
