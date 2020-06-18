using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugeScale : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ScaleRangeCollection m_scaleRanges;

		private CustomLabelCollection m_customLabels;

		private ReportDoubleProperty m_interval;

		private ReportDoubleProperty m_intervalOffset;

		private ReportBoolProperty m_logarithmic;

		private ReportDoubleProperty m_logarithmicBase;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ReportDoubleProperty m_multiplier;

		private ReportBoolProperty m_reversed;

		private GaugeTickMarks m_gaugeMajorTickMarks;

		private GaugeTickMarks m_gaugeMinorTickMarks;

		private ScalePin m_maximumPin;

		private ScalePin m_minimumPin;

		private ScaleLabels m_scaleLabels;

		private ReportBoolProperty m_tickMarksOnTop;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_gaugePanel, m_gaugePanel, m_defObject, m_gaugePanel.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_gaugePanel.GaugePanelDef.UniqueName + "x" + m_defObject.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && m_defObject.Action != null)
				{
					m_actionInfo = new ActionInfo(m_gaugePanel.RenderingContext, m_gaugePanel, m_defObject.Action, m_gaugePanel.GaugePanelDef, m_gaugePanel, ObjectType.GaugePanel, m_gaugePanel.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public string Name => m_defObject.Name;

		public ScaleRangeCollection ScaleRanges
		{
			get
			{
				if (m_scaleRanges == null && m_defObject.ScaleRanges != null)
				{
					m_scaleRanges = new ScaleRangeCollection(this, m_gaugePanel);
				}
				return m_scaleRanges;
			}
		}

		public CustomLabelCollection CustomLabels
		{
			get
			{
				if (m_customLabels == null && m_defObject.CustomLabels != null)
				{
					m_customLabels = new CustomLabelCollection(this, m_gaugePanel);
				}
				return m_customLabels;
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

		public ReportDoubleProperty IntervalOffset
		{
			get
			{
				if (m_intervalOffset == null && m_defObject.IntervalOffset != null)
				{
					m_intervalOffset = new ReportDoubleProperty(m_defObject.IntervalOffset);
				}
				return m_intervalOffset;
			}
		}

		public ReportBoolProperty Logarithmic
		{
			get
			{
				if (m_logarithmic == null && m_defObject.Logarithmic != null)
				{
					m_logarithmic = new ReportBoolProperty(m_defObject.Logarithmic);
				}
				return m_logarithmic;
			}
		}

		public ReportDoubleProperty LogarithmicBase
		{
			get
			{
				if (m_logarithmicBase == null && m_defObject.LogarithmicBase != null)
				{
					m_logarithmicBase = new ReportDoubleProperty(m_defObject.LogarithmicBase);
				}
				return m_logarithmicBase;
			}
		}

		public GaugeInputValue MaximumValue
		{
			get
			{
				if (m_maximumValue == null && m_defObject.MaximumValue != null)
				{
					m_maximumValue = new GaugeInputValue(m_defObject.MaximumValue, m_gaugePanel);
				}
				return m_maximumValue;
			}
		}

		public GaugeInputValue MinimumValue
		{
			get
			{
				if (m_minimumValue == null && m_defObject.MinimumValue != null)
				{
					m_minimumValue = new GaugeInputValue(m_defObject.MinimumValue, m_gaugePanel);
				}
				return m_minimumValue;
			}
		}

		public ReportDoubleProperty Multiplier
		{
			get
			{
				if (m_multiplier == null && m_defObject.Multiplier != null)
				{
					m_multiplier = new ReportDoubleProperty(m_defObject.Multiplier);
				}
				return m_multiplier;
			}
		}

		public ReportBoolProperty Reversed
		{
			get
			{
				if (m_reversed == null && m_defObject.Reversed != null)
				{
					m_reversed = new ReportBoolProperty(m_defObject.Reversed);
				}
				return m_reversed;
			}
		}

		public GaugeTickMarks GaugeMajorTickMarks
		{
			get
			{
				if (m_gaugeMajorTickMarks == null && m_defObject.GaugeMajorTickMarks != null)
				{
					m_gaugeMajorTickMarks = new GaugeTickMarks(m_defObject.GaugeMajorTickMarks, m_gaugePanel);
				}
				return m_gaugeMajorTickMarks;
			}
		}

		public GaugeTickMarks GaugeMinorTickMarks
		{
			get
			{
				if (m_gaugeMinorTickMarks == null && m_defObject.GaugeMinorTickMarks != null)
				{
					m_gaugeMinorTickMarks = new GaugeTickMarks(m_defObject.GaugeMinorTickMarks, m_gaugePanel);
				}
				return m_gaugeMinorTickMarks;
			}
		}

		public ScalePin MaximumPin
		{
			get
			{
				if (m_maximumPin == null && m_defObject.MaximumPin != null)
				{
					m_maximumPin = new ScalePin(m_defObject.MaximumPin, m_gaugePanel);
				}
				return m_maximumPin;
			}
		}

		public ScalePin MinimumPin
		{
			get
			{
				if (m_minimumPin == null && m_defObject.MinimumPin != null)
				{
					m_minimumPin = new ScalePin(m_defObject.MinimumPin, m_gaugePanel);
				}
				return m_minimumPin;
			}
		}

		public ScaleLabels ScaleLabels
		{
			get
			{
				if (m_scaleLabels == null && m_defObject.ScaleLabels != null)
				{
					m_scaleLabels = new ScaleLabels(m_defObject.ScaleLabels, m_gaugePanel);
				}
				return m_scaleLabels;
			}
		}

		public ReportBoolProperty TickMarksOnTop
		{
			get
			{
				if (m_tickMarksOnTop == null && m_defObject.TickMarksOnTop != null)
				{
					m_tickMarksOnTop = new ReportBoolProperty(m_defObject.TickMarksOnTop);
				}
				return m_tickMarksOnTop;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && m_defObject.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(m_defObject.ToolTip);
				}
				return m_toolTip;
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

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale GaugeScaleDef => m_defObject;

		public GaugeScaleInstance Instance => GetInstance();

		internal GaugeScale(Microsoft.ReportingServices.ReportIntermediateFormat.GaugeScale defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal abstract GaugeScaleInstance GetInstance();

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_style != null)
			{
				m_style.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_scaleRanges != null)
			{
				m_scaleRanges.SetNewContext();
			}
			if (m_customLabels != null)
			{
				m_customLabels.SetNewContext();
			}
			if (m_maximumValue != null)
			{
				m_maximumValue.SetNewContext();
			}
			if (m_minimumValue != null)
			{
				m_minimumValue.SetNewContext();
			}
			if (m_gaugeMajorTickMarks != null)
			{
				m_gaugeMajorTickMarks.SetNewContext();
			}
			if (m_gaugeMinorTickMarks != null)
			{
				m_gaugeMinorTickMarks.SetNewContext();
			}
			if (m_maximumPin != null)
			{
				m_maximumPin.SetNewContext();
			}
			if (m_minimumPin != null)
			{
				m_minimumPin.SetNewContext();
			}
			if (m_scaleLabels != null)
			{
				m_scaleLabels.SetNewContext();
			}
		}
	}
}
