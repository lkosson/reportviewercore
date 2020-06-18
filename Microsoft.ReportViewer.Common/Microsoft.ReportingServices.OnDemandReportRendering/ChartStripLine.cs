using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartStripLine : ChartObjectCollectionItem<ChartStripLineInstance>, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private Chart m_chart;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine m_chartStripLineDef;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportStringProperty m_title;

		private ReportEnumProperty<TextOrientations> m_textOrientation;

		private ReportIntProperty m_titleAngle;

		private ReportStringProperty m_toolTip;

		private ReportDoubleProperty m_interval;

		private ReportEnumProperty<ChartIntervalType> m_intervalType;

		private ReportDoubleProperty m_intervalOffset;

		private ReportEnumProperty<ChartIntervalType> m_intervalOffsetType;

		private ReportDoubleProperty m_stripWidth;

		private ReportEnumProperty<ChartIntervalType> m_stripWidthType;

		public Style Style
		{
			get
			{
				if (m_style == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.StyleClass != null)
				{
					m_style = new Style(m_chart, m_chart, m_chartStripLineDef, m_chart.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_chart.ChartDef.UniqueName + "x" + m_chartStripLineDef.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_chart.RenderingContext, m_chart, m_chartStripLineDef.Action, m_chart.ChartDef, m_chart, ObjectType.Chart, m_chartStripLineDef.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportStringProperty Title
		{
			get
			{
				if (m_title == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.Title != null)
				{
					m_title = new ReportStringProperty(m_chartStripLineDef.Title);
				}
				return m_title;
			}
		}

		[Obsolete("Use TextOrientation instead.")]
		public ReportIntProperty TitleAngle
		{
			get
			{
				if (m_titleAngle == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.TitleAngle != null)
				{
					m_titleAngle = new ReportIntProperty(m_chartStripLineDef.TitleAngle.IsExpression, m_chartStripLineDef.TitleAngle.OriginalText, m_chartStripLineDef.TitleAngle.IntValue, 0);
				}
				return m_titleAngle;
			}
		}

		public ReportEnumProperty<TextOrientations> TextOrientation
		{
			get
			{
				if (m_textOrientation == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.TextOrientation != null)
				{
					m_textOrientation = new ReportEnumProperty<TextOrientations>(m_chartStripLineDef.TextOrientation.IsExpression, m_chartStripLineDef.TextOrientation.OriginalText, EnumTranslator.TranslateTextOrientations(m_chartStripLineDef.TextOrientation.StringValue, null));
				}
				return m_textOrientation;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_chartStripLineDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		public ReportDoubleProperty Interval
		{
			get
			{
				if (m_interval == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.Interval != null)
				{
					m_interval = new ReportDoubleProperty(m_chartStripLineDef.Interval);
				}
				return m_interval;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalType
		{
			get
			{
				if (m_intervalType == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.IntervalType != null)
				{
					m_intervalType = new ReportEnumProperty<ChartIntervalType>(m_chartStripLineDef.IntervalType.IsExpression, m_chartStripLineDef.IntervalType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_chartStripLineDef.IntervalType.StringValue, null));
				}
				return m_intervalType;
			}
		}

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_chartStripLineDef.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		public ReportEnumProperty<ChartIntervalType> IntervalOffsetType
		{
			get
			{
				if (m_intervalOffsetType == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.IntervalOffsetType != null)
				{
					m_intervalOffsetType = new ReportEnumProperty<ChartIntervalType>(m_chartStripLineDef.IntervalOffsetType.IsExpression, m_chartStripLineDef.IntervalOffsetType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_chartStripLineDef.IntervalOffsetType.StringValue, null));
				}
				return m_intervalOffsetType;
			}
		}

		public ReportDoubleProperty StripWidth
		{
			get
			{
				if (m_stripWidth == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.StripWidth != null)
				{
					m_stripWidth = new ReportDoubleProperty(m_chartStripLineDef.StripWidth);
				}
				return m_stripWidth;
			}
		}

		public ReportEnumProperty<ChartIntervalType> StripWidthType
		{
			get
			{
				if (m_stripWidthType == null && !m_chart.IsOldSnapshot && m_chartStripLineDef.StripWidthType != null)
				{
					m_stripWidthType = new ReportEnumProperty<ChartIntervalType>(m_chartStripLineDef.StripWidthType.IsExpression, m_chartStripLineDef.StripWidthType.OriginalText, EnumTranslator.TranslateChartIntervalType(m_chartStripLineDef.StripWidthType.StringValue, null));
				}
				return m_stripWidthType;
			}
		}

		internal Chart ChartDef => m_chart;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine ChartStripLineDef => m_chartStripLineDef;

		public ChartStripLineInstance Instance
		{
			get
			{
				if (m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ChartStripLineInstance(this);
				}
				return m_instance;
			}
		}

		internal ChartStripLine(Microsoft.ReportingServices.ReportIntermediateFormat.ChartStripLine chartStripLineDef, Chart chart)
		{
			m_chartStripLineDef = chartStripLineDef;
			m_chart = chart;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
		}
	}
}
