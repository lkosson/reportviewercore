using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartArea : ChartObjectCollectionItem<ChartAreaInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea m_chartAreaDef;

		private Style m_style;

		private ChartAxisCollection m_categoryAxes;

		private ChartAxisCollection m_valueAxes;

		private ChartThreeDProperties m_threeDProperties;

		private ReportBoolProperty m_hidden;

		private ReportEnumProperty<ChartAreaAlignOrientations> m_alignOrientation;

		private ChartAlignType m_chartAlignType;

		private ReportBoolProperty m_equallySizedAxesFont;

		private ChartElementPosition m_chartElementPosition;

		private ChartElementPosition m_chartInnerPlotPosition;

		public string Name
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return m_chartAreaDef.ChartAreaName;
			}
		}

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_chart.RenderChartDef.PlotArea != null)
						{
							m_style = new Style(m_chart.RenderChartDef.PlotArea.StyleClass, m_chart.ChartInstanceInfo.PlotAreaStyleAttributeValues, m_chart.RenderingContext);
						}
					}
					else if (m_chartAreaDef.StyleClass != null)
					{
						m_style = new Style(m_chart, m_chart, m_chartAreaDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ChartThreeDProperties ThreeDProperties
		{
			get
			{
				if (m_threeDProperties == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_chart.RenderChartDef.ThreeDProperties != null)
						{
							m_threeDProperties = new ChartThreeDProperties(m_chart.RenderChartDef.ThreeDProperties, m_chart);
						}
					}
					else if (ChartAreaDef.ThreeDProperties != null)
					{
						m_threeDProperties = new ChartThreeDProperties(ChartAreaDef.ThreeDProperties, m_chart);
					}
				}
				return m_threeDProperties;
			}
		}

		public ChartAxisCollection CategoryAxes
		{
			get
			{
				if (m_categoryAxes == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_chart.RenderChartDef.CategoryAxis != null)
						{
							m_categoryAxes = new ChartAxisCollection(this, m_chart, isCategory: true);
						}
					}
					else if (m_chartAreaDef.CategoryAxes != null)
					{
						m_categoryAxes = new ChartAxisCollection(this, m_chart, isCategory: true);
					}
				}
				return m_categoryAxes;
			}
		}

		public ChartAxisCollection ValueAxes
		{
			get
			{
				if (m_valueAxes == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						if (m_chart.RenderChartDef.ValueAxis != null)
						{
							m_valueAxes = new ChartAxisCollection(this, m_chart, isCategory: false);
						}
					}
					else if (ChartAreaDef.ValueAxes != null)
					{
						m_valueAxes = new ChartAxisCollection(this, m_chart, isCategory: false);
					}
				}
				return m_valueAxes;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && !m_chart.IsOldSnapshot && m_chartAreaDef.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(m_chartAreaDef.Hidden);
				}
				return m_hidden;
			}
		}

		public ReportEnumProperty<ChartAreaAlignOrientations> AlignOrientation
		{
			get
			{
				if (m_alignOrientation == null && !m_chart.IsOldSnapshot && m_chartAreaDef.AlignOrientation != null)
				{
					m_alignOrientation = new ReportEnumProperty<ChartAreaAlignOrientations>(m_chartAreaDef.AlignOrientation.IsExpression, m_chartAreaDef.AlignOrientation.OriginalText, EnumTranslator.TranslateChartAreaAlignOrientation(m_chartAreaDef.AlignOrientation.StringValue, null));
				}
				return m_alignOrientation;
			}
		}

		public ChartAlignType ChartAlignType
		{
			get
			{
				if (m_chartAlignType == null && !m_chart.IsOldSnapshot && m_chartAreaDef.ChartAlignType != null)
				{
					m_chartAlignType = new ChartAlignType(m_chartAreaDef.ChartAlignType, m_chart);
				}
				return m_chartAlignType;
			}
		}

		public string AlignWithChartArea
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return null;
				}
				return m_chartAreaDef.AlignWithChartArea;
			}
		}

		public ReportBoolProperty EquallySizedAxesFont
		{
			get
			{
				if (m_equallySizedAxesFont == null && !m_chart.IsOldSnapshot && m_chartAreaDef.EquallySizedAxesFont != null)
				{
					m_equallySizedAxesFont = new ReportBoolProperty(m_chartAreaDef.EquallySizedAxesFont);
				}
				return m_equallySizedAxesFont;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (m_chartElementPosition == null && !m_chart.IsOldSnapshot && m_chartAreaDef.ChartElementPosition != null)
				{
					m_chartElementPosition = new ChartElementPosition(m_chartAreaDef.ChartElementPosition, m_chart);
				}
				return m_chartElementPosition;
			}
		}

		public ChartElementPosition ChartInnerPlotPosition
		{
			get
			{
				if (m_chartInnerPlotPosition == null && !m_chart.IsOldSnapshot && m_chartAreaDef.ChartInnerPlotPosition != null)
				{
					m_chartInnerPlotPosition = new ChartElementPosition(m_chartAreaDef.ChartInnerPlotPosition, m_chart);
				}
				return m_chartInnerPlotPosition;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea ChartAreaDef => m_chartAreaDef;

		internal Chart ChartDef => m_chart;

		public ChartAreaInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartAreaInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartArea(Microsoft.ReportingServices.ReportIntermediateFormat.ChartArea chartAreaDef, Chart chart)
		{
			m_chart = chart;
			m_chartAreaDef = chartAreaDef;
		}

		internal ChartArea(Chart chart)
		{
			m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_categoryAxes != null)
			{
				m_categoryAxes.SetNewContext();
			}
			if (m_valueAxes != null)
			{
				m_valueAxes.SetNewContext();
			}
			if (m_threeDProperties != null)
			{
				m_threeDProperties.SetNewContext();
			}
			if (m_chartAlignType != null)
			{
				m_chartAlignType.SetNewContext();
			}
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.SetNewContext();
			}
			if (m_chartInnerPlotPosition != null)
			{
				m_chartInnerPlotPosition.SetNewContext();
			}
		}
	}
}
