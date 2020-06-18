using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTitle : MapDockableSubItem
	{
		private ReportStringProperty m_text;

		private ReportDoubleProperty m_angle;

		private ReportSizeProperty m_textShadowOffset;

		public string Name => MapTitleDef.Name;

		public ReportStringProperty Text
		{
			get
			{
				if (m_text == null && MapTitleDef.Text != null)
				{
					m_text = new ReportStringProperty(MapTitleDef.Text);
				}
				return m_text;
			}
		}

		public ReportDoubleProperty Angle
		{
			get
			{
				if (m_angle == null && MapTitleDef.Angle != null)
				{
					m_angle = new ReportDoubleProperty(MapTitleDef.Angle);
				}
				return m_angle;
			}
		}

		public ReportSizeProperty TextShadowOffset
		{
			get
			{
				if (m_textShadowOffset == null && MapTitleDef.TextShadowOffset != null)
				{
					m_textShadowOffset = new ReportSizeProperty(MapTitleDef.TextShadowOffset);
				}
				return m_textShadowOffset;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle MapTitleDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle)m_defObject;

		public new MapTitleInstance Instance => (MapTitleInstance)GetInstance();

		internal MapTitle(Microsoft.ReportingServices.ReportIntermediateFormat.MapTitle defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapSubItemInstance GetInstance()
		{
			if (m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (m_instance == null)
			{
				m_instance = new MapTitleInstance(this);
			}
			return (MapSubItemInstance)m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
		}
	}
}
