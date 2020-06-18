using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLocation
	{
		private Map m_map;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation m_defObject;

		private MapLocationInstance m_instance;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_top;

		private ReportEnumProperty<Unit> m_unit;

		public ReportDoubleProperty Left
		{
			get
			{
				if (m_left == null && m_defObject.Left != null)
				{
					m_left = new ReportDoubleProperty(m_defObject.Left);
				}
				return m_left;
			}
		}

		public ReportDoubleProperty Top
		{
			get
			{
				if (m_top == null && m_defObject.Top != null)
				{
					m_top = new ReportDoubleProperty(m_defObject.Top);
				}
				return m_top;
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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation MapLocationDef => m_defObject;

		public MapLocationInstance Instance
		{
			get
			{
				if (m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new MapLocationInstance(this);
				}
				return m_instance;
			}
		}

		internal MapLocation(Microsoft.ReportingServices.ReportIntermediateFormat.MapLocation defObject, Map map)
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
