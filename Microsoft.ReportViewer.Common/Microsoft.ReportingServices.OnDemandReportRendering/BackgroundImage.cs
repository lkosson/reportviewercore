using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackgroundImage : ReportProperty, IImage, IBaseImage
	{
		private bool m_isOldSnapshot;

		private Microsoft.ReportingServices.ReportRendering.Style m_renderStyle;

		private Style m_styleDef;

		private ReportStringProperty m_value;

		private ReportStringProperty m_mimeType;

		private ReportEnumProperty<BackgroundRepeatTypes> m_repeat;

		private BackgroundImageInstance m_instance;

		private Image.SourceType? m_imageSource;

		private ReportEnumProperty<Positions> m_position;

		private ReportColorProperty m_transparentColor;

		public Image.SourceType Source
		{
			get
			{
				if (!m_imageSource.HasValue)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderStyle.GetBackgroundImageSource(m_renderStyle.GetStyleDefinition("BackgroundImageSource"), out Microsoft.ReportingServices.ReportRendering.Image.SourceType imageSource))
						{
							m_imageSource = (Image.SourceType)imageSource;
						}
					}
					else
					{
						int? num = m_styleDef.EvaluateInstanceStyleEnum(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageSource);
						m_imageSource = (Image.SourceType)(num.HasValue ? num.Value : 0);
					}
				}
				return m_imageSource.Value;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (m_value == null)
				{
					if (m_isOldSnapshot)
					{
						if (m_renderStyle.GetBackgroundImageValue(m_renderStyle.GetStyleDefinition("BackgroundImageValue"), out object imageValue, out bool isExpression))
						{
							m_value = new ReportStringProperty(isExpression, null, (imageValue is string) ? ((string)imageValue) : null);
						}
						else
						{
							m_value = new ReportStringProperty();
						}
					}
					else
					{
						string expressionString;
						Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = m_styleDef.GetAttributeInfo("BackgroundImageValue", out expressionString);
						if (attributeInfo == null)
						{
							m_value = new ReportStringProperty();
						}
						else
						{
							m_value = new ReportStringProperty(attributeInfo.IsExpression, expressionString, attributeInfo.Value);
						}
					}
				}
				return m_value;
			}
		}

		public ReportStringProperty MIMEType
		{
			get
			{
				if (m_mimeType == null)
				{
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = m_renderStyle.GetStyleDefinition("BackgroundImageMIMEType");
						if (styleDefinition == null)
						{
							m_mimeType = new ReportStringProperty();
						}
						else
						{
							m_mimeType = new ReportStringProperty(styleDefinition.IsExpression, null, styleDefinition.Value);
						}
					}
					else
					{
						string expressionString;
						Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = m_styleDef.GetAttributeInfo("BackgroundImageMIMEType", out expressionString);
						if (attributeInfo == null)
						{
							m_mimeType = new ReportStringProperty();
						}
						else
						{
							m_mimeType = new ReportStringProperty(attributeInfo.IsExpression, expressionString, attributeInfo.Value);
						}
					}
				}
				return m_mimeType;
			}
		}

		public ReportEnumProperty<BackgroundRepeatTypes> BackgroundRepeat
		{
			get
			{
				if (m_repeat == null)
				{
					if (m_isOldSnapshot)
					{
						Microsoft.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = m_renderStyle.GetStyleDefinition("BackgroundRepeat");
						if (styleDefinition == null)
						{
							m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(Style.DefaultEnumBackgroundRepeatType);
						}
						else
						{
							m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(styleDefinition.IsExpression, null, StyleTranslator.TranslateBackgroundRepeat(styleDefinition.Value, null, m_styleDef.IsDynamicImageStyle), Style.DefaultEnumBackgroundRepeatType);
						}
					}
					else
					{
						string expressionString;
						Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = m_styleDef.GetAttributeInfo("BackgroundRepeat", out expressionString);
						if (attributeInfo == null)
						{
							m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(Style.DefaultEnumBackgroundRepeatType);
						}
						else
						{
							m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(attributeInfo.IsExpression, expressionString, (BackgroundRepeatTypes)StyleTranslator.TranslateStyle(StyleAttributeNames.BackgroundImageRepeat, attributeInfo.Value, null, m_styleDef.IsDynamicImageStyle), Style.DefaultEnumBackgroundRepeatType);
						}
					}
				}
				return m_repeat;
			}
		}

		public ReportEnumProperty<Positions> Position
		{
			get
			{
				if (m_position == null)
				{
					if (m_isOldSnapshot)
					{
						m_position = new ReportEnumProperty<Positions>();
					}
					else
					{
						string expressionString;
						Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = m_styleDef.GetAttributeInfo("Position", out expressionString);
						bool isExpression = false;
						string styleString = null;
						if (attributeInfo != null)
						{
							styleString = attributeInfo.Value;
							isExpression = attributeInfo.IsExpression;
						}
						m_position = new ReportEnumProperty<Positions>(isExpression, expressionString, StyleTranslator.TranslatePosition(styleString, null, m_styleDef.IsDynamicImageStyle));
					}
				}
				return m_position;
			}
		}

		public ReportColorProperty TransparentColor
		{
			get
			{
				if (m_transparentColor == null)
				{
					ReportColor defaultValue = new ReportColor("Transparent", Color.Transparent, parsed: true);
					if (m_isOldSnapshot)
					{
						m_transparentColor = new ReportColorProperty(isExpression: false, null, null, defaultValue);
					}
					else
					{
						string expressionString;
						Microsoft.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = m_styleDef.GetAttributeInfo("TransparentColor", out expressionString);
						bool flag = false;
						ReportColor value = null;
						if (attributeInfo != null)
						{
							flag = attributeInfo.IsExpression;
							if (!flag)
							{
								value = new ReportColor(attributeInfo.Value, m_styleDef.IsDynamicImageStyle);
							}
						}
						m_transparentColor = new ReportColorProperty(flag, expressionString, value, defaultValue);
					}
				}
				return m_transparentColor;
			}
		}

		public BackgroundImageInstance Instance
		{
			get
			{
				if (!m_isOldSnapshot && m_styleDef.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					if (m_isOldSnapshot)
					{
						m_instance = new ShimBackgroundImageInstance(this, m_renderStyle["BackgroundImage"] as Microsoft.ReportingServices.ReportRendering.BackgroundImage, m_renderStyle["BackgroundRepeat"] as string);
					}
					else
					{
						m_instance = new InternalBackgroundImageInstance(this);
					}
				}
				return m_instance;
			}
		}

		internal Style StyleDef => m_styleDef;

		ObjectType IBaseImage.ObjectType => m_styleDef.StyleContainer.ObjectType;

		string IBaseImage.ObjectName => m_styleDef.StyleContainer.Name;

		ReportProperty IBaseImage.Value => Value;

		string IBaseImage.ImageDataPropertyName => "BackgroundImageValue";

		string IBaseImage.ImageValuePropertyName => "BackgroundImageValue";

		string IBaseImage.MIMETypePropertyName => "BackgroundImageMIMEType";

		Image.EmbeddingModes IBaseImage.EmbeddingMode => Image.EmbeddingModes.Inline;

		internal BackgroundImage(bool isExpression, string expressionString, Style styleDef)
			: base(isExpression, expressionString)
		{
			m_styleDef = styleDef;
			m_isOldSnapshot = false;
		}

		internal BackgroundImage(bool isExpression, string expressionString, Microsoft.ReportingServices.ReportRendering.Style renderStyle, Style styleDef)
			: base(isExpression, expressionString)
		{
			m_styleDef = styleDef;
			m_renderStyle = renderStyle;
			m_isOldSnapshot = true;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			fieldsUsedInValue = null;
			errorOccurred = false;
			return m_styleDef.EvaluateInstanceStyleVariant(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageValue) as byte[];
		}

		string IBaseImage.GetMIMETypeValue()
		{
			return m_styleDef.EvaluateInstanceStyleString(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageMimeType);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			errOccurred = false;
			fieldsUsedInValue = null;
			return m_styleDef.EvaluateInstanceStyleString(Microsoft.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageValue);
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			mimeType = null;
			imageData = null;
			return null;
		}
	}
}
