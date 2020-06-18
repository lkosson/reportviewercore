using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerImage : IBaseImage
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage m_defObject;

		private MapMarkerImageInstance m_instance;

		private ReportEnumProperty<Image.SourceType> m_source;

		private ReportVariantProperty m_value;

		private ReportStringProperty m_mIMEType;

		private ReportColorProperty m_transparentColor;

		private ReportEnumProperty<MapResizeMode> m_resizeMode;

		ObjectType IBaseImage.ObjectType => m_map.ReportItemDef.ObjectType;

		string IBaseImage.ObjectName => m_map.Name;

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
				if (m_mIMEType == null && m_defObject.MIMEType != null)
				{
					m_mIMEType = new ReportStringProperty(m_defObject.MIMEType);
				}
				return m_mIMEType;
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
						m_transparentColor = new ReportColorProperty(transparentColor.IsExpression, m_defObject.TransparentColor.OriginalText, transparentColor.IsExpression ? null : new ReportColor(transparentColor.StringValue.Trim(), allowTransparency: true), transparentColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_transparentColor;
			}
		}

		public ReportEnumProperty<MapResizeMode> ResizeMode
		{
			get
			{
				if (m_resizeMode == null && m_defObject.ResizeMode != null)
				{
					m_resizeMode = new ReportEnumProperty<MapResizeMode>(m_defObject.ResizeMode.IsExpression, m_defObject.ResizeMode.OriginalText, EnumTranslator.TranslateMapResizeMode(m_defObject.ResizeMode.StringValue, null));
				}
				return m_resizeMode;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage MapMarkerImageDef => m_defObject;

		public MapMarkerImageInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapMarkerImageInstance(this);
				}
				return m_instance;
			}
		}

		internal MapMarkerImage(Microsoft.ReportingServices.ReportIntermediateFormat.MapMarkerImage defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			fieldsUsedInValue = null;
			return m_defObject.EvaluateBinaryValue(Instance.ReportScopeInstance, m_map.RenderingContext.OdpContext, out errorOccurred);
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
			return MapMarkerImageDef.EvaluateMIMEType(Instance.ReportScopeInstance, MapDef.RenderingContext.OdpContext);
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
			return m_defObject.EvaluateStringValue(Instance.ReportScopeInstance, m_map.RenderingContext.OdpContext, out errOccurred);
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			mimeType = null;
			imageData = null;
			return null;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
