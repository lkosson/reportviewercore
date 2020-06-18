using Microsoft.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLegendTitle : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle m_defObject;

		private MapLegendTitleInstance m_instance;

		private Style m_style;

		private ReportStringProperty m_caption;

		private ReportEnumProperty<MapLegendTitleSeparator> m_titleSeparator;

		private ReportColorProperty m_titleSeparatorColor;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_map, m_map.ReportScope, m_defObject, m_map.RenderingContext);
				}
				return m_style;
			}
		}

		public ReportStringProperty Caption
		{
			get
			{
				if (m_caption == null && m_defObject.Caption != null)
				{
					m_caption = new ReportStringProperty(m_defObject.Caption);
				}
				return m_caption;
			}
		}

		public ReportEnumProperty<MapLegendTitleSeparator> TitleSeparator
		{
			get
			{
				if (m_titleSeparator == null && m_defObject.TitleSeparator != null)
				{
					m_titleSeparator = new ReportEnumProperty<MapLegendTitleSeparator>(m_defObject.TitleSeparator.IsExpression, m_defObject.TitleSeparator.OriginalText, EnumTranslator.TranslateMapLegendTitleSeparator(m_defObject.TitleSeparator.StringValue, null));
				}
				return m_titleSeparator;
			}
		}

		public ReportColorProperty TitleSeparatorColor
		{
			get
			{
				if (m_titleSeparatorColor == null && m_defObject.TitleSeparatorColor != null)
				{
					ExpressionInfo titleSeparatorColor = m_defObject.TitleSeparatorColor;
					if (titleSeparatorColor != null)
					{
						m_titleSeparatorColor = new ReportColorProperty(titleSeparatorColor.IsExpression, m_defObject.TitleSeparatorColor.OriginalText, titleSeparatorColor.IsExpression ? null : new ReportColor(titleSeparatorColor.StringValue.Trim(), allowTransparency: true), titleSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_titleSeparatorColor;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle MapLegendTitleDef => m_defObject;

		public MapLegendTitleInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapLegendTitleInstance(this);
				}
				return m_instance;
			}
		}

		internal MapLegendTitle(Microsoft.ReportingServices.ReportIntermediateFormat.MapLegendTitle defObject, Map map)
		{
			m_defObject = defObject;
			m_map = map;
		}

		internal void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
		}
	}
}
