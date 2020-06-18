using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSize
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSize m_defObject;

		private MapSizeInstance m_instance;

		private ReportDoubleProperty m_width;

		private ReportDoubleProperty m_height;

		private ReportEnumProperty<Unit> m_unit;

		public ReportDoubleProperty Width
		{
			get
			{
				if (m_width == null && m_defObject.Width != null)
				{
					m_width = new ReportDoubleProperty(m_defObject.Width);
				}
				return m_width;
			}
		}

		public ReportDoubleProperty Height
		{
			get
			{
				if (m_height == null && m_defObject.Height != null)
				{
					m_height = new ReportDoubleProperty(m_defObject.Height);
				}
				return m_height;
			}
		}

		public ReportEnumProperty<Unit> Unit
		{
			get
			{
				if (m_unit == null && m_defObject.Unit != null)
				{
					m_unit = new ReportEnumProperty<Unit>(m_defObject.Unit.IsExpression, m_defObject.Unit.OriginalText, EnumTranslator.TranslateUnit(m_defObject.Unit.StringValue, null));
				}
				return m_unit;
			}
		}

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSize MapSizeDef => m_defObject;

		public MapSizeInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapSizeInstance(this);
				}
				return m_instance;
			}
		}

		internal MapSize(Microsoft.ReportingServices.ReportIntermediateFormat.MapSize defObject, Map map)
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
		}
	}
}
