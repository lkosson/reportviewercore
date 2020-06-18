using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapGridLines : IROMStyleDefinitionContainer
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines m_defObject;

		private MapGridLinesInstance m_instance;

		private Style m_style;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_interval;

		private ReportBoolProperty m_showLabels;

		private ReportEnumProperty<MapLabelPosition> m_labelPosition;

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

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && m_defObject.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_defObject.Hidden);
				}
				return m_hidden;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && m_defObject.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_defObject.Interval);
				}
				return m_interval;
			}
		}

		public ReportBoolProperty ShowLabels
		{
			get
			{
				if (m_showLabels == null && m_defObject.ShowLabels != null)
				{
					m_showLabels = new ReportBoolProperty(m_defObject.ShowLabels);
				}
				return m_showLabels;
			}
		}

		public ReportEnumProperty<MapLabelPosition> LabelPosition
		{
			get
			{
				if (m_labelPosition == null && m_defObject.LabelPosition != null)
				{
					m_labelPosition = new ReportEnumProperty<MapLabelPosition>(m_defObject.LabelPosition.IsExpression, m_defObject.LabelPosition.OriginalText, EnumTranslator.TranslateLabelPosition(m_defObject.LabelPosition.StringValue, null));
				}
				return m_labelPosition;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines MapGridLinesDef => m_defObject;

		public MapGridLinesInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapGridLinesInstance(this);
				}
				return m_instance;
			}
		}

		internal MapGridLines(Microsoft.ReportingServices.ReportIntermediateFormat.MapGridLines defObject, Map map)
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
