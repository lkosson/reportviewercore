namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartThreeDPropertiesInstance : BaseInstance
	{
		private ChartThreeDProperties m_chartThreeDPropertiesDef;

		private bool? m_enabled;

		private ChartThreeDProjectionModes? m_projectionMode;

		private int? m_perspective;

		private int? m_rotation;

		private int? m_inclination;

		private int? m_depthRatio;

		private ChartThreeDShadingTypes? m_shading;

		private int? m_gapDepth;

		private int? m_wallThickness;

		private bool? m_clustered;

		public bool Enabled
		{
			get
			{
				if (!m_enabled.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_enabled = m_chartThreeDPropertiesDef.Enabled.Value;
					}
					else
					{
						m_enabled = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateEnabled(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_enabled.Value;
			}
		}

		public ChartThreeDProjectionModes ProjectionMode
		{
			get
			{
				if (!m_projectionMode.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_projectionMode = m_chartThreeDPropertiesDef.ProjectionMode.Value;
					}
					else
					{
						m_projectionMode = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateProjectionMode(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_projectionMode.Value;
			}
		}

		public int Perspective
		{
			get
			{
				if (!m_perspective.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_perspective = m_chartThreeDPropertiesDef.Perspective.Value;
					}
					else
					{
						m_perspective = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluatePerspective(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_perspective.Value;
			}
		}

		public int Rotation
		{
			get
			{
				if (!m_rotation.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_rotation = m_chartThreeDPropertiesDef.Rotation.Value;
					}
					else
					{
						m_rotation = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateRotation(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_rotation.Value;
			}
		}

		public int Inclination
		{
			get
			{
				if (!m_inclination.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_inclination = m_chartThreeDPropertiesDef.Inclination.Value;
					}
					else
					{
						m_inclination = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateInclination(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_inclination.Value;
			}
		}

		public int DepthRatio
		{
			get
			{
				if (!m_depthRatio.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_depthRatio = m_chartThreeDPropertiesDef.DepthRatio.Value;
					}
					else
					{
						m_depthRatio = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateDepthRatio(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_depthRatio.Value;
			}
		}

		public ChartThreeDShadingTypes Shading
		{
			get
			{
				if (!m_shading.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_shading = m_chartThreeDPropertiesDef.Shading.Value;
					}
					else
					{
						m_shading = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateShading(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_shading.Value;
			}
		}

		public int GapDepth
		{
			get
			{
				if (!m_gapDepth.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_gapDepth = m_chartThreeDPropertiesDef.GapDepth.Value;
					}
					else
					{
						m_gapDepth = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateGapDepth(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_gapDepth.Value;
			}
		}

		public int WallThickness
		{
			get
			{
				if (!m_wallThickness.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_wallThickness = m_chartThreeDPropertiesDef.WallThickness.Value;
					}
					else
					{
						m_wallThickness = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateWallThickness(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_wallThickness.Value;
			}
		}

		public bool Clustered
		{
			get
			{
				if (!m_clustered.HasValue)
				{
					if (m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						m_clustered = m_chartThreeDPropertiesDef.Clustered.Value;
					}
					else
					{
						m_clustered = m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateClustered(ReportScopeInstance, m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return m_clustered.Value;
			}
		}

		internal ChartThreeDPropertiesInstance(ChartThreeDProperties chartThreeDPropertiesDef)
			: base(chartThreeDPropertiesDef.ChartDef)
		{
			m_chartThreeDPropertiesDef = chartThreeDPropertiesDef;
		}

		protected override void ResetInstanceCache()
		{
			m_enabled = null;
			m_projectionMode = null;
			m_perspective = null;
			m_rotation = null;
			m_inclination = null;
			m_depthRatio = null;
			m_shading = null;
			m_gapDepth = null;
			m_wallThickness = null;
			m_clustered = null;
		}
	}
}
