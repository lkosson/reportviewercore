using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class BaseGaugeImage : IBaseImage
	{
		internal GaugePanel m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage m_defObject;

		internal BaseGaugeImageInstance m_instance;

		private ReportEnumProperty<Image.SourceType> m_source;

		private ReportVariantProperty m_value;

		private ReportStringProperty m_MIMEType;

		private ReportColorProperty m_transparentColor;

		ObjectType IBaseImage.ObjectType => m_gaugePanel.ReportItemDef.ObjectType;

		string IBaseImage.ObjectName => m_gaugePanel.Name;

		Image.SourceType IBaseImage.Source
		{
			get
			{
				ReportEnumProperty<Image.SourceType> source = Source;
				if (!source.IsExpression)
				{
					return source.Value;
				}
				return Instance.Source;
			}
		}

		ReportProperty IBaseImage.Value => Value;

		ReportStringProperty IBaseImage.MIMEType => MIMEType;

		string IBaseImage.ImageDataPropertyName => "ImageData";

		string IBaseImage.ImageValuePropertyName => "Value";

		string IBaseImage.MIMETypePropertyName => "MIMEType";

		Image.EmbeddingModes IBaseImage.EmbeddingMode => Image.EmbeddingModes.Inline;

		public ReportEnumProperty<Image.SourceType> Source
		{
			get
			{
				if (m_source == null && m_defObject.Source != null)
				{
					m_source = new ReportEnumProperty<Image.SourceType>(m_defObject.Source.IsExpression, m_defObject.Source.OriginalText, EnumTranslator.TranslateImageSourceType(m_defObject.Source.StringValue, null));
				}
				return m_source;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				if (m_value == null && m_defObject.Value != null)
				{
					m_value = new ReportVariantProperty(m_defObject.Value);
				}
				return m_value;
			}
		}

		public ReportStringProperty MIMEType
		{
			get
			{
				if (m_MIMEType == null && m_defObject.MIMEType != null)
				{
					m_MIMEType = new ReportStringProperty(m_defObject.MIMEType);
				}
				return m_MIMEType;
			}
		}

		public ReportColorProperty TransparentColor
		{
			get
			{
				if (m_transparentColor == null && m_defObject.TransparentColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo transparentColor = m_defObject.TransparentColor;
					if (transparentColor != null)
					{
						m_transparentColor = new ReportColorProperty(transparentColor.IsExpression, transparentColor.OriginalText, transparentColor.IsExpression ? null : new ReportColor(transparentColor.StringValue.Trim(), allowTransparency: true), transparentColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_transparentColor;
			}
		}

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage BaseGaugeImageDef => m_defObject;

		internal BaseGaugeImageInstance Instance => GetInstance();

		internal BaseGaugeImage(Microsoft.ReportingServices.ReportIntermediateFormat.BaseGaugeImage defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			fieldsUsedInValue = null;
			return m_defObject.EvaluateBinaryValue(Instance.ReportScopeInstance, m_gaugePanel.RenderingContext.OdpContext, out errorOccurred);
		}

		string IBaseImage.GetMIMETypeValue()
		{
			ReportStringProperty mIMEType = MIMEType;
			if (mIMEType == null)
			{
				return null;
			}
			if (!mIMEType.IsExpression)
			{
				return mIMEType.Value;
			}
			return BaseGaugeImageDef.EvaluateMIMEType(Instance.ReportScopeInstance, GaugePanelDef.RenderingContext.OdpContext);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			fieldsUsedInValue = null;
			ReportVariantProperty value = Value;
			errOccurred = false;
			if (!value.IsExpression)
			{
				object value2 = value.Value;
				if (value2 is string)
				{
					return (string)value2;
				}
				return null;
			}
			return m_defObject.EvaluateStringValue(Instance.ReportScopeInstance, m_gaugePanel.RenderingContext.OdpContext, out errOccurred);
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			mimeType = null;
			imageData = null;
			return null;
		}

		internal abstract BaseGaugeImageInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
