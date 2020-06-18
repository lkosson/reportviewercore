using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartThreeDProperties
	{
		private Chart m_chart;

		private ThreeDProperties m_renderThreeDPropertiesDef;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties m_chartThreeDPropertiesDef;

		private ChartThreeDPropertiesInstance m_instance;

		private ReportBoolProperty m_clustered;

		private ReportIntProperty m_wallThickness;

		private ReportIntProperty m_gapDepth;

		private ReportEnumProperty<ChartThreeDShadingTypes> m_shading;

		private ReportIntProperty m_depthRatio;

		private ReportIntProperty m_rotation;

		private ReportIntProperty m_inclination;

		private ReportIntProperty m_perspective;

		private ReportEnumProperty<ChartThreeDProjectionModes> m_projectionMode;

		private ReportBoolProperty m_enabled;

		public ReportBoolProperty Enabled
		{
			get
			{
				if (m_enabled == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_enabled = new ReportBoolProperty(m_renderThreeDPropertiesDef.Enabled);
					}
					else if (m_chartThreeDPropertiesDef.Enabled != null)
					{
						m_enabled = new ReportBoolProperty(m_chartThreeDPropertiesDef.Enabled);
					}
				}
				return m_enabled;
			}
		}

		public ReportEnumProperty<ChartThreeDProjectionModes> ProjectionMode
		{
			get
			{
				if (m_projectionMode == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_projectionMode = new ReportEnumProperty<ChartThreeDProjectionModes>((!m_renderThreeDPropertiesDef.PerspectiveProjectionMode) ? ChartThreeDProjectionModes.Perspective : ChartThreeDProjectionModes.Oblique);
					}
					else if (m_chartThreeDPropertiesDef.ProjectionMode != null)
					{
						m_projectionMode = new ReportEnumProperty<ChartThreeDProjectionModes>(m_chartThreeDPropertiesDef.ProjectionMode.IsExpression, m_chartThreeDPropertiesDef.ProjectionMode.OriginalText, EnumTranslator.TranslateChartThreeDProjectionMode(m_chartThreeDPropertiesDef.ProjectionMode.StringValue, null));
					}
				}
				return m_projectionMode;
			}
		}

		public ReportIntProperty Perspective
		{
			get
			{
				if (m_perspective == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_perspective = new ReportIntProperty(m_renderThreeDPropertiesDef.Perspective);
					}
					else if (m_chartThreeDPropertiesDef.Perspective != null)
					{
						m_perspective = new ReportIntProperty(m_chartThreeDPropertiesDef.Perspective);
					}
				}
				return m_perspective;
			}
		}

		public ReportIntProperty Rotation
		{
			get
			{
				if (m_rotation == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_rotation = new ReportIntProperty(m_renderThreeDPropertiesDef.Rotation);
					}
					else if (m_chartThreeDPropertiesDef.Rotation != null)
					{
						m_rotation = new ReportIntProperty(m_chartThreeDPropertiesDef.Rotation);
					}
				}
				return m_rotation;
			}
		}

		public ReportIntProperty Inclination
		{
			get
			{
				if (m_inclination == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_inclination = new ReportIntProperty(m_renderThreeDPropertiesDef.Inclination);
					}
					else if (m_chartThreeDPropertiesDef.Inclination != null)
					{
						m_inclination = new ReportIntProperty(m_chartThreeDPropertiesDef.Inclination);
					}
				}
				return m_inclination;
			}
		}

		public ReportIntProperty DepthRatio
		{
			get
			{
				if (m_depthRatio == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_depthRatio = new ReportIntProperty(m_renderThreeDPropertiesDef.DepthRatio);
					}
					else if (m_chartThreeDPropertiesDef.DepthRatio != null)
					{
						m_depthRatio = new ReportIntProperty(m_chartThreeDPropertiesDef.DepthRatio);
					}
				}
				return m_depthRatio;
			}
		}

		public ReportEnumProperty<ChartThreeDShadingTypes> Shading
		{
			get
			{
				if (m_shading == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartThreeDShadingTypes value = ChartThreeDShadingTypes.None;
						if (m_renderThreeDPropertiesDef.Shading == ThreeDProperties.ShadingTypes.Real)
						{
							value = ChartThreeDShadingTypes.Real;
						}
						else if (m_renderThreeDPropertiesDef.Shading == ThreeDProperties.ShadingTypes.Simple)
						{
							value = ChartThreeDShadingTypes.Simple;
						}
						m_shading = new ReportEnumProperty<ChartThreeDShadingTypes>(value);
					}
					else if (m_chartThreeDPropertiesDef.Shading != null)
					{
						m_shading = new ReportEnumProperty<ChartThreeDShadingTypes>(m_chartThreeDPropertiesDef.Shading.IsExpression, m_chartThreeDPropertiesDef.Shading.OriginalText, EnumTranslator.TranslateChartThreeDShading(m_chartThreeDPropertiesDef.Shading.StringValue, null));
					}
				}
				return m_shading;
			}
		}

		public ReportIntProperty GapDepth
		{
			get
			{
				if (m_gapDepth == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_gapDepth = new ReportIntProperty(m_renderThreeDPropertiesDef.GapDepth);
					}
					else if (m_chartThreeDPropertiesDef.GapDepth != null)
					{
						m_gapDepth = new ReportIntProperty(m_chartThreeDPropertiesDef.GapDepth);
					}
				}
				return m_gapDepth;
			}
		}

		public ReportIntProperty WallThickness
		{
			get
			{
				if (m_wallThickness == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_wallThickness = new ReportIntProperty(m_renderThreeDPropertiesDef.WallThickness);
					}
					else if (m_chartThreeDPropertiesDef.WallThickness != null)
					{
						m_wallThickness = new ReportIntProperty(m_chartThreeDPropertiesDef.WallThickness);
					}
				}
				return m_wallThickness;
			}
		}

		public ReportBoolProperty Clustered
		{
			get
			{
				if (m_clustered == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_clustered = new ReportBoolProperty(m_renderThreeDPropertiesDef.Clustered);
					}
					else if (m_chartThreeDPropertiesDef.Clustered != null)
					{
						m_clustered = new ReportBoolProperty(m_chartThreeDPropertiesDef.Clustered);
					}
				}
				return m_clustered;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties ChartThreeDPropertiesDef => m_chartThreeDPropertiesDef;

		public ChartThreeDPropertiesInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartThreeDPropertiesInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartThreeDProperties(Microsoft.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties threeDPropertiesDef, Chart chart)
		{
			m_chart = chart;
			m_chartThreeDPropertiesDef = threeDPropertiesDef;
		}

		internal ChartThreeDProperties(ThreeDProperties renderThreeDPropertiesDef, Chart chart)
		{
			m_chart = chart;
			m_renderThreeDPropertiesDef = renderThreeDPropertiesDef;
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
