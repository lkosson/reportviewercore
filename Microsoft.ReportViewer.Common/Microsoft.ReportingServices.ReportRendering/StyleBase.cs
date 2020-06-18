using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal abstract class StyleBase
	{
		protected const int StyleAttributeCount = 42;

		protected const string BorderColor = "BorderColor";

		protected const string BorderColorLeft = "BorderColorLeft";

		protected const string BorderColorRight = "BorderColorRight";

		protected const string BorderColorTop = "BorderColorTop";

		protected const string BorderColorBottom = "BorderColorBottom";

		protected const string BorderStyle = "BorderStyle";

		protected const string BorderStyleLeft = "BorderStyleLeft";

		protected const string BorderStyleRight = "BorderStyleRight";

		protected const string BorderStyleTop = "BorderStyleTop";

		protected const string BorderStyleBottom = "BorderStyleBottom";

		protected const string BorderWidth = "BorderWidth";

		protected const string BorderWidthLeft = "BorderWidthLeft";

		protected const string BorderWidthRight = "BorderWidthRight";

		protected const string BorderWidthTop = "BorderWidthTop";

		protected const string BorderWidthBottom = "BorderWidthBottom";

		protected const string BackgroundImage = "BackgroundImage";

		protected const string BackgroundImageSource = "BackgroundImageSource";

		protected const string BackgroundImageValue = "BackgroundImageValue";

		protected const string BackgroundImageMIMEType = "BackgroundImageMIMEType";

		protected const string BackgroundColor = "BackgroundColor";

		protected const string BackgroundGradientEndColor = "BackgroundGradientEndColor";

		protected const string BackgroundGradientType = "BackgroundGradientType";

		protected const string BackgroundRepeat = "BackgroundRepeat";

		protected const string FontStyle = "FontStyle";

		protected const string FontFamily = "FontFamily";

		protected const string FontSize = "FontSize";

		protected const string FontWeight = "FontWeight";

		protected const string Format = "Format";

		protected const string TextDecoration = "TextDecoration";

		protected const string TextAlign = "TextAlign";

		protected const string VerticalAlign = "VerticalAlign";

		protected const string Color = "Color";

		protected const string PaddingLeft = "PaddingLeft";

		protected const string PaddingRight = "PaddingRight";

		protected const string PaddingTop = "PaddingTop";

		protected const string PaddingBottom = "PaddingBottom";

		protected const string LineHeight = "LineHeight";

		protected const string Direction = "Direction";

		protected const string WritingMode = "WritingMode";

		protected const string Language = "Language";

		protected const string UnicodeBiDi = "UnicodeBiDi";

		protected const string Calendar = "Calendar";

		protected const string CurrencyLanguage = "CurrencyLanguage";

		protected const string NumeralLanguage = "NumeralLanguage";

		protected const string NumeralVariant = "NumeralVariant";

		internal RenderingContext m_renderingContext;

		protected StyleProperties m_sharedProperties;

		protected StyleProperties m_nonSharedProperties;

		protected bool m_isCustomControlGenerated;

		public object this[int index]
		{
			get
			{
				if (0 > index || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				PopulateStyleProperties(populateAll: true);
				int num = 0;
				if (m_sharedProperties != null)
				{
					num = m_sharedProperties.Count;
				}
				if (index < num)
				{
					return m_sharedProperties[index];
				}
				Global.Tracer.Assert(m_nonSharedProperties != null);
				return m_nonSharedProperties[index - num];
			}
		}

		public virtual int Count
		{
			get
			{
				PopulateStyleProperties(populateAll: true);
				int num = 0;
				if (m_sharedProperties != null)
				{
					num += m_sharedProperties.Count;
				}
				if (m_nonSharedProperties != null)
				{
					num += m_nonSharedProperties.Count;
				}
				return num;
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				string[] array = new string[Count];
				if (m_sharedProperties != null)
				{
					m_sharedProperties.Keys.CopyTo(array, 0);
				}
				if (m_nonSharedProperties != null)
				{
					m_nonSharedProperties.Keys.CopyTo(array, m_sharedProperties.Count);
				}
				return array;
			}
		}

		public abstract object this[string styleName]
		{
			get;
		}

		public virtual StyleProperties SharedProperties
		{
			get
			{
				return m_sharedProperties;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_nonSharedProperties = value;
			}
		}

		public virtual StyleProperties NonSharedProperties
		{
			get
			{
				return m_nonSharedProperties;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_nonSharedProperties = value;
			}
		}

		protected bool IsCustomControl => m_isCustomControlGenerated;

		protected StyleBase()
		{
			m_isCustomControlGenerated = true;
		}

		internal StyleBase(RenderingContext context)
		{
			m_isCustomControlGenerated = false;
			m_renderingContext = context;
		}

		public bool ContainStyleProperty(string styleName)
		{
			bool flag = false;
			if (Count == 0)
			{
				return flag;
			}
			if (m_sharedProperties != null)
			{
				flag = m_sharedProperties.ContainStyleProperty(styleName);
			}
			if (!flag && m_nonSharedProperties != null)
			{
				flag = m_nonSharedProperties.ContainStyleProperty(styleName);
			}
			return flag;
		}

		public IEnumerator GetEnumerator()
		{
			PopulateStyleProperties(populateAll: true);
			return new StyleEnumerator(m_sharedProperties, m_nonSharedProperties);
		}

		internal static object GetStyleValueBase(string styleName, Microsoft.ReportingServices.ReportProcessing.Style styleDef, object[] styleAttributeValues)
		{
			if (styleDef != null)
			{
				AttributeInfo attributeInfo = styleDef.StyleAttributes[styleName];
				if (attributeInfo != null)
				{
					object obj = null;
					if (!attributeInfo.IsExpression)
					{
						obj = ((!("NumeralVariant" == styleName)) ? attributeInfo.Value : ((object)attributeInfo.IntValue));
					}
					else if (styleAttributeValues != null)
					{
						Global.Tracer.Assert(0 <= attributeInfo.IntValue && attributeInfo.IntValue < styleAttributeValues.Length);
						obj = styleAttributeValues[attributeInfo.IntValue];
					}
					else
					{
						obj = null;
					}
					if (obj != null)
					{
						return CreateStyleProperty(styleName, obj);
					}
				}
			}
			return null;
		}

		internal static object CreateStyleProperty(string styleName, object styleValue)
		{
			switch (styleName)
			{
			case "BorderColor":
			case "BorderColorTop":
			case "BorderColorLeft":
			case "BorderColorRight":
			case "BorderColorBottom":
			case "BackgroundColor":
			case "BackgroundGradientEndColor":
			case "Color":
				return new ReportColor((string)styleValue, parsed: false);
			case "BorderStyle":
			case "BorderStyleTop":
			case "BorderStyleLeft":
			case "BorderStyleRight":
			case "BorderStyleBottom":
			case "BackgroundRepeat":
			case "FontStyle":
			case "FontFamily":
			case "FontWeight":
			case "Format":
			case "TextDecoration":
			case "TextAlign":
			case "VerticalAlign":
			case "Direction":
			case "WritingMode":
			case "Language":
			case "UnicodeBiDi":
			case "Calendar":
			case "NumeralLanguage":
			case "BackgroundGradientType":
				return styleValue;
			case "BorderWidth":
			case "BorderWidthTop":
			case "BorderWidthLeft":
			case "BorderWidthRight":
			case "BorderWidthBottom":
			case "FontSize":
			case "PaddingLeft":
			case "PaddingRight":
			case "PaddingTop":
			case "PaddingBottom":
			case "LineHeight":
				return new ReportSize((string)styleValue, parsed: false);
			case "NumeralVariant":
				return styleValue;
			default:
				return null;
			}
		}

		internal abstract object GetStyleAttributeValue(string styleName, AttributeInfo attribute);

		internal bool GetBackgroundImageSource(AttributeInfo sourceAttribute, out Image.SourceType imageSource)
		{
			if (sourceAttribute == null)
			{
				imageSource = Image.SourceType.External;
				return false;
			}
			Global.Tracer.Assert(!sourceAttribute.IsExpression);
			imageSource = (Image.SourceType)sourceAttribute.IntValue;
			return true;
		}

		internal bool GetBackgroundImageValue(AttributeInfo valueAttribute, out object imageValue, out bool isExpression)
		{
			if (valueAttribute == null)
			{
				imageValue = null;
				isExpression = false;
				return false;
			}
			imageValue = GetStyleAttributeValue("BackgroundImageValue", valueAttribute);
			isExpression = valueAttribute.IsExpression;
			return true;
		}

		internal bool GetBackgroundImageMIMEType(AttributeInfo mimeTypeAttribute, out object mimeType, out bool isExpression)
		{
			if (mimeTypeAttribute == null)
			{
				mimeType = null;
				isExpression = false;
				return false;
			}
			mimeType = GetStyleAttributeValue("BackgroundImageMIMEType", mimeTypeAttribute);
			isExpression = mimeTypeAttribute.IsExpression;
			return true;
		}

		internal bool GetBackgroundImageRepeat(AttributeInfo repeatAttribute, out object repeat, out bool isExpression)
		{
			if (repeatAttribute == null)
			{
				repeat = null;
				isExpression = false;
				return false;
			}
			repeat = GetStyleAttributeValue("BackgroundRepeat", repeatAttribute);
			isExpression = repeatAttribute.IsExpression;
			return true;
		}

		internal bool GetBackgroundImageProperties(AttributeInfo sourceAttribute, AttributeInfo valueAttribute, AttributeInfo mimeTypeAttribute, out Image.SourceType imageSource, out object imageValue, out bool isValueExpression, out object mimeType, out bool isMimeTypeExpression)
		{
			GetBackgroundImageValue(valueAttribute, out imageValue, out isValueExpression);
			GetBackgroundImageMIMEType(mimeTypeAttribute, out mimeType, out isMimeTypeExpression);
			return GetBackgroundImageSource(sourceAttribute, out imageSource);
		}

		internal bool GetBackgroundImageProperties(AttributeInfo sourceAttribute, AttributeInfo valueAttribute, AttributeInfo mimeTypeAttribute, AttributeInfo repeatAttribute, out Image.SourceType imageSource, out object imageValue, out bool isValueExpression, out object mimeType, out bool isMimeTypeExpression, out object repeat, out bool isRepeatExpression)
		{
			GetBackgroundImageValue(valueAttribute, out imageValue, out isValueExpression);
			GetBackgroundImageMIMEType(mimeTypeAttribute, out mimeType, out isMimeTypeExpression);
			GetBackgroundImageRepeat(repeatAttribute, out repeat, out isRepeatExpression);
			return GetBackgroundImageSource(sourceAttribute, out imageSource);
		}

		public void SetStyle(Style.StyleName style, object value, bool isShared)
		{
			object obj = null;
			bool flag = false;
			switch (style)
			{
			case Style.StyleName.BorderColor:
			case Style.StyleName.BorderColorTop:
			case Style.StyleName.BorderColorLeft:
			case Style.StyleName.BorderColorRight:
			case Style.StyleName.BorderColorBottom:
			case Style.StyleName.BackgroundColor:
			case Style.StyleName.Color:
				if (value is ReportColor)
				{
					obj = (value as ReportColor);
				}
				else if (value is string)
				{
					obj = new ReportColor(value as string);
				}
				if (obj == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidStyleArgumentType, "ReportColor");
				}
				obj = ((ReportColor)obj).ToString();
				flag = true;
				break;
			case Style.StyleName.BorderStyle:
			case Style.StyleName.BorderStyleTop:
			case Style.StyleName.BorderStyleLeft:
			case Style.StyleName.BorderStyleRight:
			case Style.StyleName.BorderStyleBottom:
				if (value != null)
				{
					obj = (value as string);
					if (obj == null)
					{
						throw new ReportRenderingException(ErrorCode.rrInvalidStyleArgumentType, "String");
					}
					if (!Validator.ValidateBorderStyle(obj as string, out string _))
					{
						throw new ReportRenderingException(ErrorCode.rrInvalidBorderStyle, obj);
					}
				}
				flag = true;
				break;
			case Style.StyleName.BorderWidth:
			case Style.StyleName.BorderWidthTop:
			case Style.StyleName.BorderWidthLeft:
			case Style.StyleName.BorderWidthRight:
			case Style.StyleName.BorderWidthBottom:
			case Style.StyleName.FontSize:
			case Style.StyleName.PaddingLeft:
			case Style.StyleName.PaddingRight:
			case Style.StyleName.PaddingTop:
			case Style.StyleName.PaddingBottom:
			case Style.StyleName.LineHeight:
				if (value is ReportSize)
				{
					obj = (value as ReportSize);
				}
				else if (value is string)
				{
					obj = new ReportSize(value as string);
				}
				if (obj == null)
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidStyleArgumentType, "ReportSize");
				}
				flag = true;
				break;
			case Style.StyleName.NumeralVariant:
			{
				if (!int.TryParse(value as string, out int result))
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidStyleArgumentType, "Int32");
				}
				obj = result;
				flag = true;
				break;
			}
			default:
				if (value != null)
				{
					obj = (value as string);
					if (obj == null)
					{
						throw new ReportRenderingException(ErrorCode.rrInvalidStyleArgumentType, "String");
					}
				}
				flag = true;
				break;
			}
			if (!flag)
			{
				return;
			}
			if (isShared)
			{
				if (m_sharedProperties == null)
				{
					m_sharedProperties = new StyleProperties();
				}
				m_sharedProperties.Set(style.ToString(), obj);
			}
			else
			{
				if (m_nonSharedProperties == null)
				{
					m_nonSharedProperties = new StyleProperties();
				}
				m_nonSharedProperties.Set(style.ToString(), obj);
			}
		}

		internal void AddStyleProperty(string styleName, bool isExpression, bool needNonSharedProps, bool needSharedProps, object styleProperty)
		{
			if (isExpression)
			{
				if (needNonSharedProps)
				{
					if (m_nonSharedProperties == null)
					{
						m_nonSharedProperties = new StyleProperties();
					}
					m_nonSharedProperties.Add(styleName, styleProperty);
				}
			}
			else if (needSharedProps)
			{
				if (m_sharedProperties == null)
				{
					m_sharedProperties = new StyleProperties(42);
				}
				m_sharedProperties.Add(styleName, styleProperty);
			}
		}

		internal void SetStyleProperty(string styleName, bool isExpression, bool needNonSharedProps, bool needSharedProps, object styleProperty)
		{
			if (isExpression)
			{
				if (needNonSharedProps)
				{
					if (m_nonSharedProperties == null)
					{
						m_nonSharedProperties = new StyleProperties();
					}
					m_nonSharedProperties.Set(styleName, styleProperty);
				}
			}
			else if (needSharedProps)
			{
				if (m_sharedProperties == null)
				{
					m_sharedProperties = new StyleProperties(42);
				}
				m_sharedProperties.Set(styleName, styleProperty);
			}
		}

		internal abstract void PopulateStyleProperties(bool populateAll);

		internal void ExtractRenderStyles(out DataValueInstanceList sharedStyles, out DataValueInstanceList nonSharedStyles)
		{
			sharedStyles = null;
			nonSharedStyles = null;
			if (m_sharedProperties != null)
			{
				sharedStyles = m_sharedProperties.ExtractRenderStyles();
			}
			if (m_nonSharedProperties != null)
			{
				nonSharedStyles = m_nonSharedProperties.ExtractRenderStyles();
			}
		}
	}
}
