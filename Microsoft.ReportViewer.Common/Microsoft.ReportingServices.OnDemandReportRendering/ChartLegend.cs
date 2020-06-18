using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartLegend : ChartObjectCollectionItem<ChartLegendInstance>, IROMStyleDefinitionContainer
	{
		private Chart m_chart;

		private Legend m_renderLegendDef;

		private object[] m_styleValues;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend m_legendDef;

		private Style m_style;

		private ChartLegendCustomItemCollection m_chartLegendCustomItems;

		private ChartLegendColumnCollection m_chartLegendColumns;

		private ChartLegendTitle m_legendTitle;

		private ReportEnumProperty<ChartLegendLayouts> m_layout;

		private ReportEnumProperty<ChartLegendPositions> m_position;

		private ReportBoolProperty m_hidden;

		private ReportBoolProperty m_dockOutsideChartArea;

		private ReportBoolProperty m_autoFitTextDisabled;

		private ReportSizeProperty m_minFontSize;

		private ReportEnumProperty<ChartSeparators> m_headerSeparator;

		private ReportColorProperty m_headerSeparatorColor;

		private ReportEnumProperty<ChartSeparators> m_columnSeparator;

		private ReportColorProperty m_columnSeparatorColor;

		private ReportIntProperty m_columnSpacing;

		private ReportBoolProperty m_interlacedRows;

		private ReportColorProperty m_interlacedRowsColor;

		private ReportBoolProperty m_equallySpacedItems;

		private ReportEnumProperty<ChartAutoBool> m_reversed;

		private ReportIntProperty m_maxAutoSize;

		private ReportIntProperty m_textWrapThreshold;

		private ChartElementPosition m_chartElementPosition;

		public string Name
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					return "Default";
				}
				return m_legendDef.LegendName;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_hidden = new ReportBoolProperty(!m_renderLegendDef.Visible);
					}
					else
					{
						m_hidden = new ReportBoolProperty(m_legendDef.Hidden);
					}
				}
				return m_hidden;
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
						m_style = new Style(m_renderLegendDef.StyleClass, m_styleValues, m_chart.RenderingContext);
					}
					else if (m_legendDef.StyleClass != null)
					{
						m_style = new Style(m_chart, m_chart, m_legendDef, m_chart.RenderingContext);
					}
				}
				return m_style;
			}
		}

		public ReportEnumProperty<ChartLegendPositions> Position
		{
			get
			{
				if (m_position == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartLegendPositions value = ChartLegendPositions.TopRight;
						switch (m_renderLegendDef.Position)
						{
						case Legend.Positions.BottomCenter:
							value = ChartLegendPositions.BottomCenter;
							break;
						case Legend.Positions.BottomLeft:
							value = ChartLegendPositions.BottomLeft;
							break;
						case Legend.Positions.BottomRight:
							value = ChartLegendPositions.BottomRight;
							break;
						case Legend.Positions.LeftBottom:
							value = ChartLegendPositions.LeftBottom;
							break;
						case Legend.Positions.LeftCenter:
							value = ChartLegendPositions.LeftCenter;
							break;
						case Legend.Positions.LeftTop:
							value = ChartLegendPositions.LeftTop;
							break;
						case Legend.Positions.RightBottom:
							value = ChartLegendPositions.RightBottom;
							break;
						case Legend.Positions.RightCenter:
							value = ChartLegendPositions.RightCenter;
							break;
						case Legend.Positions.RightTop:
							value = ChartLegendPositions.RightTop;
							break;
						case Legend.Positions.TopCenter:
							value = ChartLegendPositions.TopCenter;
							break;
						case Legend.Positions.TopLeft:
							value = ChartLegendPositions.TopLeft;
							break;
						case Legend.Positions.TopRight:
							value = ChartLegendPositions.TopRight;
							break;
						}
						m_position = new ReportEnumProperty<ChartLegendPositions>(value);
					}
					else if (m_legendDef.Position != null)
					{
						m_position = new ReportEnumProperty<ChartLegendPositions>(m_legendDef.Position.IsExpression, m_legendDef.Position.OriginalText, EnumTranslator.TranslateChartLegendPositions(m_legendDef.Position.StringValue, null));
					}
				}
				return m_position;
			}
		}

		public ReportEnumProperty<ChartLegendLayouts> Layout
		{
			get
			{
				if (m_layout == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						ChartLegendLayouts value = ChartLegendLayouts.AutoTable;
						switch (m_renderLegendDef.Layout)
						{
						case Legend.LegendLayout.Column:
							value = ChartLegendLayouts.Column;
							break;
						case Legend.LegendLayout.Row:
							value = ChartLegendLayouts.Row;
							break;
						case Legend.LegendLayout.Table:
							value = ChartLegendLayouts.AutoTable;
							break;
						}
						m_layout = new ReportEnumProperty<ChartLegendLayouts>(value);
					}
					else if (m_legendDef.Layout != null)
					{
						m_layout = new ReportEnumProperty<ChartLegendLayouts>(m_legendDef.Layout.IsExpression, m_legendDef.Layout.OriginalText, EnumTranslator.TranslateChartLegendLayout(m_legendDef.Layout.StringValue, null));
					}
				}
				return m_layout;
			}
		}

		public ReportBoolProperty DockOutsideChartArea
		{
			get
			{
				if (m_dockOutsideChartArea == null)
				{
					if (m_chart.IsOldSnapshot)
					{
						m_dockOutsideChartArea = new ReportBoolProperty(!m_renderLegendDef.InsidePlotArea);
					}
					else if (m_legendDef.DockOutsideChartArea != null)
					{
						m_dockOutsideChartArea = new ReportBoolProperty(m_legendDef.DockOutsideChartArea);
					}
				}
				return m_dockOutsideChartArea;
			}
		}

		public ChartLegendCustomItemCollection LegendCustomItems
		{
			get
			{
				if (m_chartLegendCustomItems == null && !m_chart.IsOldSnapshot && ChartLegendDef.LegendCustomItems != null)
				{
					m_chartLegendCustomItems = new ChartLegendCustomItemCollection(this, m_chart);
				}
				return m_chartLegendCustomItems;
			}
		}

		public ChartLegendColumnCollection LegendColumns
		{
			get
			{
				if (m_chartLegendColumns == null && !m_chart.IsOldSnapshot && ChartLegendDef.LegendColumns != null)
				{
					m_chartLegendColumns = new ChartLegendColumnCollection(this, m_chart);
				}
				return m_chartLegendColumns;
			}
		}

		public ChartLegendTitle LegendTitle
		{
			get
			{
				if (m_legendTitle == null && !m_chart.IsOldSnapshot && m_legendDef.LegendTitle != null)
				{
					m_legendTitle = new ChartLegendTitle(m_legendDef.LegendTitle, m_chart);
				}
				return m_legendTitle;
			}
		}

		public string DockToChartArea
		{
			get
			{
				if (m_chart.IsOldSnapshot)
				{
					if (!DockOutsideChartArea.Value)
					{
						return "Default";
					}
					return null;
				}
				return m_legendDef.DockToChartArea;
			}
		}

		public ChartElementPosition ChartElementPosition
		{
			get
			{
				if (m_chartElementPosition == null && !m_chart.IsOldSnapshot && m_legendDef.ChartElementPosition != null)
				{
					m_chartElementPosition = new ChartElementPosition(m_legendDef.ChartElementPosition, m_chart);
				}
				return m_chartElementPosition;
			}
		}

		public ReportBoolProperty AutoFitTextDisabled
		{
			get
			{
				if (m_autoFitTextDisabled == null && !m_chart.IsOldSnapshot && m_legendDef.AutoFitTextDisabled != null)
				{
					m_autoFitTextDisabled = new ReportBoolProperty(m_legendDef.AutoFitTextDisabled);
				}
				return m_autoFitTextDisabled;
			}
		}

		public ReportSizeProperty MinFontSize
		{
			get
			{
				if (m_minFontSize == null && !m_chart.IsOldSnapshot && m_legendDef.MinFontSize != null)
				{
					m_minFontSize = new ReportSizeProperty(m_legendDef.MinFontSize);
				}
				return m_minFontSize;
			}
		}

		public ReportEnumProperty<ChartSeparators> HeaderSeparator
		{
			get
			{
				if (m_headerSeparator == null && !m_chart.IsOldSnapshot && m_legendDef.HeaderSeparator != null)
				{
					m_headerSeparator = new ReportEnumProperty<ChartSeparators>(m_legendDef.HeaderSeparator.IsExpression, m_legendDef.HeaderSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(m_legendDef.HeaderSeparator.StringValue, null));
				}
				return m_headerSeparator;
			}
		}

		public ReportColorProperty HeaderSeparatorColor
		{
			get
			{
				if (m_headerSeparatorColor == null && !m_chart.IsOldSnapshot && m_legendDef.HeaderSeparatorColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo headerSeparatorColor = m_legendDef.HeaderSeparatorColor;
					m_headerSeparatorColor = new ReportColorProperty(headerSeparatorColor.IsExpression, headerSeparatorColor.OriginalText, headerSeparatorColor.IsExpression ? null : new ReportColor(headerSeparatorColor.StringValue.Trim(), allowTransparency: true), headerSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_headerSeparatorColor;
			}
		}

		public ReportEnumProperty<ChartSeparators> ColumnSeparator
		{
			get
			{
				if (m_columnSeparator == null && !m_chart.IsOldSnapshot && m_legendDef.ColumnSeparator != null)
				{
					m_columnSeparator = new ReportEnumProperty<ChartSeparators>(m_legendDef.ColumnSeparator.IsExpression, m_legendDef.ColumnSeparator.OriginalText, EnumTranslator.TranslateChartSeparator(m_legendDef.ColumnSeparator.StringValue, null));
				}
				return m_columnSeparator;
			}
		}

		public ReportColorProperty ColumnSeparatorColor
		{
			get
			{
				if (m_columnSeparatorColor == null && !m_chart.IsOldSnapshot && m_legendDef.ColumnSeparatorColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo columnSeparatorColor = m_legendDef.ColumnSeparatorColor;
					m_columnSeparatorColor = new ReportColorProperty(columnSeparatorColor.IsExpression, columnSeparatorColor.OriginalText, columnSeparatorColor.IsExpression ? null : new ReportColor(columnSeparatorColor.StringValue.Trim(), allowTransparency: true), columnSeparatorColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_columnSeparatorColor;
			}
		}

		public ReportIntProperty ColumnSpacing
		{
			get
			{
				if (m_columnSpacing == null && !m_chart.IsOldSnapshot && m_legendDef.ColumnSpacing != null)
				{
					m_columnSpacing = new ReportIntProperty(m_legendDef.ColumnSpacing.IsExpression, m_legendDef.ColumnSpacing.OriginalText, m_legendDef.ColumnSpacing.IntValue, 50);
				}
				return m_columnSpacing;
			}
		}

		public ReportBoolProperty InterlacedRows
		{
			get
			{
				if (m_interlacedRows == null && !m_chart.IsOldSnapshot && m_legendDef.InterlacedRows != null)
				{
					m_interlacedRows = new ReportBoolProperty(m_legendDef.InterlacedRows);
				}
				return m_interlacedRows;
			}
		}

		public ReportColorProperty InterlacedRowsColor
		{
			get
			{
				if (m_interlacedRowsColor == null && !m_chart.IsOldSnapshot && m_legendDef.InterlacedRowsColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo interlacedRowsColor = m_legendDef.InterlacedRowsColor;
					m_interlacedRowsColor = new ReportColorProperty(interlacedRowsColor.IsExpression, interlacedRowsColor.OriginalText, interlacedRowsColor.IsExpression ? null : new ReportColor(interlacedRowsColor.StringValue.Trim(), allowTransparency: true), interlacedRowsColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
				}
				return m_interlacedRowsColor;
			}
		}

		public ReportBoolProperty EquallySpacedItems
		{
			get
			{
				if (m_equallySpacedItems == null && !m_chart.IsOldSnapshot && m_legendDef.EquallySpacedItems != null)
				{
					m_equallySpacedItems = new ReportBoolProperty(m_legendDef.EquallySpacedItems);
				}
				return m_equallySpacedItems;
			}
		}

		public ReportEnumProperty<ChartAutoBool> Reversed
		{
			get
			{
				if (m_reversed == null && !m_chart.IsOldSnapshot && m_legendDef.Reversed != null)
				{
					m_reversed = new ReportEnumProperty<ChartAutoBool>(m_legendDef.Reversed.IsExpression, m_legendDef.Reversed.OriginalText, EnumTranslator.TranslateChartAutoBool(m_legendDef.Reversed.StringValue, null));
				}
				return m_reversed;
			}
		}

		public ReportIntProperty MaxAutoSize
		{
			get
			{
				if (m_maxAutoSize == null && !m_chart.IsOldSnapshot && m_legendDef.MaxAutoSize != null)
				{
					m_maxAutoSize = new ReportIntProperty(m_legendDef.MaxAutoSize.IsExpression, m_legendDef.MaxAutoSize.OriginalText, m_legendDef.MaxAutoSize.IntValue, 50);
				}
				return m_maxAutoSize;
			}
		}

		public ReportIntProperty TextWrapThreshold
		{
			get
			{
				if (m_textWrapThreshold == null && !m_chart.IsOldSnapshot && m_legendDef.TextWrapThreshold != null)
				{
					m_textWrapThreshold = new ReportIntProperty(m_legendDef.TextWrapThreshold.IsExpression, m_legendDef.TextWrapThreshold.OriginalText, m_legendDef.TextWrapThreshold.IntValue, 25);
				}
				return m_textWrapThreshold;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend ChartLegendDef => m_legendDef;

		internal Chart ChartDef => m_chart;

		public ChartLegendInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartLegendInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartLegend(Microsoft.ReportingServices.ReportIntermediateFormat.ChartLegend legendDef, Chart chart)
		{
			m_chart = chart;
			m_legendDef = legendDef;
		}

		internal ChartLegend(Legend renderLegendDef, object[] styleValues, Chart chart)
		{
			m_chart = chart;
			m_renderLegendDef = renderLegendDef;
			m_styleValues = styleValues;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_chartLegendCustomItems != null)
			{
				m_chartLegendCustomItems.SetNewContext();
			}
			if (m_chartLegendColumns != null)
			{
				m_chartLegendColumns.SetNewContext();
			}
			if (m_legendTitle != null)
			{
				m_legendTitle.SetNewContext();
			}
			if (m_chartElementPosition != null)
			{
				m_chartElementPosition.SetNewContext();
			}
		}
	}
}
