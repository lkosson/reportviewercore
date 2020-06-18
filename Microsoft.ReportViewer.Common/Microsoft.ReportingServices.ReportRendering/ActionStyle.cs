using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class ActionStyle : StyleBase
	{
		private ActionInfo m_actionInfo;

		public override int Count
		{
			get
			{
				if (base.IsCustomControl)
				{
					return base.Count;
				}
				if (m_actionInfo.ActionInfoDef.StyleClass == null)
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
				if (m_actionInfo.ActionInfoDef.StyleClass == null)
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
					return CreateProperty(styleName, obj);
				}
				Global.Tracer.Assert(!base.IsCustomControl);
				if (m_actionInfo.ActionInfoDef.StyleClass == null)
				{
					return null;
				}
				StyleAttributeHashtable styleAttributes = m_actionInfo.ActionInfoDef.StyleClass.StyleAttributes;
				AttributeInfo attributeInfo = null;
				if ("BackgroundImage" == styleName)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object imageValue = null;
					object mimeType = null;
					bool isMimeTypeExpression = false;
					GetBackgroundImageProperties(styleAttributes["BackgroundImageSource"], styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out imageSource, out imageValue, out bool _, out mimeType, out isMimeTypeExpression);
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
					attributeInfo = styleAttributes[styleName];
					if (attributeInfo != null)
					{
						return CreateProperty(styleName, GetStyleAttributeValue(styleName, attributeInfo));
					}
				}
				return null;
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
					m_actionInfo.ActionInfoDef.SharedStyleProperties = m_sharedProperties;
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
						m_actionInfo.ActionInfoDef.NoNonSharedStyleProps = true;
					}
				}
				return m_nonSharedProperties;
			}
		}

		public ActionStyle()
		{
			Global.Tracer.Assert(base.IsCustomControl);
		}

		internal ActionStyle(ActionInfo actionInfo, RenderingContext context)
			: base(context)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			m_actionInfo = actionInfo;
		}

		private bool NeedPopulateSharedProps()
		{
			if (m_sharedProperties != null)
			{
				return false;
			}
			if (m_actionInfo.ActionInfoDef.SharedStyleProperties != null)
			{
				m_sharedProperties = m_actionInfo.ActionInfoDef.SharedStyleProperties;
				return false;
			}
			return true;
		}

		private bool NeedPopulateNonSharedProps()
		{
			if (m_nonSharedProperties == null && !m_actionInfo.ActionInfoDef.NoNonSharedStyleProps)
			{
				return true;
			}
			return false;
		}

		internal override object GetStyleAttributeValue(string styleName, AttributeInfo attribute)
		{
			Global.Tracer.Assert(!base.IsCustomControl);
			if (attribute.IsExpression)
			{
				return m_actionInfo.ActionInfoInstance?.GetStyleAttributeValue(attribute.IntValue);
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
			Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_actionInfo.ActionInfoDef.StyleClass;
			StyleAttributeHashtable styleAttributeHashtable = null;
			if (styleClass != null)
			{
				styleAttributeHashtable = styleClass.StyleAttributes;
			}
			Global.Tracer.Assert(styleAttributeHashtable != null);
			IDictionaryEnumerator enumerator = styleAttributeHashtable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				AttributeInfo attributeInfo = (AttributeInfo)enumerator.Value;
				string text = (string)enumerator.Key;
				if ("BackgroundImage" == text)
				{
					Image.SourceType imageSource = Image.SourceType.External;
					object imageValue = null;
					object mimeType = null;
					bool isValueExpression = false;
					bool isMimeTypeExpression = false;
					GetBackgroundImageProperties(styleAttributeHashtable["BackgroundImageSource"], styleAttributeHashtable["BackgroundImageValue"], styleAttributeHashtable["BackgroundImageMIMEType"], out imageSource, out imageValue, out isValueExpression, out mimeType, out isMimeTypeExpression);
					if (imageValue != null)
					{
						string mimeType2 = null;
						if (!isMimeTypeExpression)
						{
							mimeType2 = (string)mimeType;
						}
						object styleProperty = new BackgroundImage(m_renderingContext, imageSource, imageValue, mimeType2);
						AddStyleProperty(text, isValueExpression || isMimeTypeExpression, flag2, flag, styleProperty);
					}
				}
				else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text))
				{
					AddStyleProperty(text, attributeInfo.IsExpression, flag2, flag, CreateProperty(text, GetStyleAttributeValue(text, attributeInfo)));
				}
			}
		}

		private void PopulateNonSharedStyleProperties()
		{
			if (!base.IsCustomControl)
			{
				Microsoft.ReportingServices.ReportProcessing.Style styleClass = m_actionInfo.ActionInfoDef.StyleClass;
				if (styleClass != null)
				{
					StyleAttributeHashtable styleAttributes = styleClass.StyleAttributes;
					Global.Tracer.Assert(styleAttributes != null);
					InternalPopulateNonSharedStyleProperties(styleAttributes);
				}
			}
		}

		private void InternalPopulateNonSharedStyleProperties(StyleAttributeHashtable styleAttributes)
		{
			if (styleAttributes == null)
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
					if (GetBackgroundImageProperties(attributeInfo, styleAttributes["BackgroundImageValue"], styleAttributes["BackgroundImageMIMEType"], out Image.SourceType imageSource, out object imageValue, out bool isValueExpression, out object mimeType, out bool isMimeTypeExpression) && (isValueExpression || isMimeTypeExpression) && imageValue != null)
					{
						string mimeType2 = null;
						if (!isMimeTypeExpression)
						{
							mimeType2 = (string)mimeType;
						}
						object styleProperty = new BackgroundImage(m_renderingContext, imageSource, imageValue, mimeType2);
						SetStyleProperty("BackgroundImage", isExpression: true, needNonSharedProps: true, needSharedProps: false, styleProperty);
					}
				}
				else if (!("BackgroundImageValue" == text) && !("BackgroundImageMIMEType" == text) && attributeInfo.IsExpression)
				{
					SetStyleProperty(text, isExpression: true, needNonSharedProps: true, needSharedProps: false, CreateProperty(text, GetStyleAttributeValue(text, attributeInfo)));
				}
			}
		}

		private object CreateProperty(string styleName, object styleValue)
		{
			if (styleValue == null)
			{
				return null;
			}
			return StyleBase.CreateStyleProperty(styleName, styleValue);
		}
	}
}
