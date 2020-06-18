using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ScaleRange : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		private GaugePanel m_gaugePanel;

		private Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportDoubleProperty m_distanceFromScale;

		private GaugeInputValue m_startValue;

		private GaugeInputValue m_endValue;

		private ReportDoubleProperty m_startWidth;

		private ReportDoubleProperty m_endWidth;

		private ReportColorProperty m_inRangeBarPointerColor;

		private ReportColorProperty m_inRangeLabelColor;

		private ReportColorProperty m_inRangeTickMarksColor;

		private ReportEnumProperty<ScaleRangePlacements> m_placement;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportEnumProperty<BackgroundGradientTypes> m_backgroundGradientType;

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

		public ReportDoubleProperty DistanceFromScale
		{
			get
			{
				if (m_distanceFromScale == null && m_defObject.DistanceFromScale != null)
				{
					m_distanceFromScale = new ReportDoubleProperty(m_defObject.DistanceFromScale);
				}
				return m_distanceFromScale;
			}
		}

		public GaugeInputValue StartValue
		{
			get
			{
				if (m_startValue == null && m_defObject.StartValue != null)
				{
					m_startValue = new GaugeInputValue(m_defObject.StartValue, m_gaugePanel);
				}
				return m_startValue;
			}
		}

		public GaugeInputValue EndValue
		{
			get
			{
				if (m_endValue == null && m_defObject.EndValue != null)
				{
					m_endValue = new GaugeInputValue(m_defObject.EndValue, m_gaugePanel);
				}
				return m_endValue;
			}
		}

		public ReportDoubleProperty StartWidth
		{
			get
			{
				if (m_startWidth == null && m_defObject.StartWidth != null)
				{
					m_startWidth = new ReportDoubleProperty(m_defObject.StartWidth);
				}
				return m_startWidth;
			}
		}

		public ReportDoubleProperty EndWidth
		{
			get
			{
				if (m_endWidth == null && m_defObject.EndWidth != null)
				{
					m_endWidth = new ReportDoubleProperty(m_defObject.EndWidth);
				}
				return m_endWidth;
			}
		}

		public ReportColorProperty InRangeBarPointerColor
		{
			get
			{
				if (m_inRangeBarPointerColor == null && m_defObject.InRangeBarPointerColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeBarPointerColor = m_defObject.InRangeBarPointerColor;
					if (inRangeBarPointerColor != null)
					{
						m_inRangeBarPointerColor = new ReportColorProperty(inRangeBarPointerColor.IsExpression, inRangeBarPointerColor.OriginalText, inRangeBarPointerColor.IsExpression ? null : new ReportColor(inRangeBarPointerColor.StringValue.Trim(), allowTransparency: true), inRangeBarPointerColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_inRangeBarPointerColor;
			}
		}

		public ReportColorProperty InRangeLabelColor
		{
			get
			{
				if (m_inRangeLabelColor == null && m_defObject.InRangeLabelColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeLabelColor = m_defObject.InRangeLabelColor;
					if (inRangeLabelColor != null)
					{
						m_inRangeLabelColor = new ReportColorProperty(inRangeLabelColor.IsExpression, inRangeLabelColor.OriginalText, inRangeLabelColor.IsExpression ? null : new ReportColor(inRangeLabelColor.StringValue.Trim(), allowTransparency: true), inRangeLabelColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_inRangeLabelColor;
			}
		}

		public ReportColorProperty InRangeTickMarksColor
		{
			get
			{
				if (m_inRangeTickMarksColor == null && m_defObject.InRangeTickMarksColor != null)
				{
					Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo inRangeTickMarksColor = m_defObject.InRangeTickMarksColor;
					if (inRangeTickMarksColor != null)
					{
						m_inRangeTickMarksColor = new ReportColorProperty(inRangeTickMarksColor.IsExpression, inRangeTickMarksColor.OriginalText, inRangeTickMarksColor.IsExpression ? null : new ReportColor(inRangeTickMarksColor.StringValue.Trim(), allowTransparency: true), inRangeTickMarksColor.IsExpression ? new ReportColor("", Color.Empty, parsed: true) : null);
					}
				}
				return m_inRangeTickMarksColor;
			}
		}

		public ReportEnumProperty<BackgroundGradientTypes> BackgroundGradientType
		{
			get
			{
				if (m_backgroundGradientType == null && m_defObject.BackgroundGradientType != null)
				{
					m_backgroundGradientType = new ReportEnumProperty<BackgroundGradientTypes>(m_defObject.BackgroundGradientType.IsExpression, m_defObject.BackgroundGradientType.OriginalText, EnumTranslator.TranslateBackgroundGradientTypes(m_defObject.BackgroundGradientType.StringValue, null));
				}
				return m_backgroundGradientType;
			}
		}

		public ReportEnumProperty<ScaleRangePlacements> Placement
		{
			get
			{
				if (m_placement == null && m_defObject.Placement != null)
				{
					m_placement = new ReportEnumProperty<ScaleRangePlacements>(m_defObject.Placement.IsExpression, m_defObject.Placement.OriginalText, EnumTranslator.TranslateScaleRangePlacements(m_defObject.Placement.StringValue, null));
				}
				return m_placement;
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

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange ScaleRangeDef => m_defObject;

		public ScaleRangeInstance Instance
		{
			get
			{
				if (m_gaugePanel.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (m_instance == null)
				{
					m_instance = new ScaleRangeInstance(this);
				}
				return (ScaleRangeInstance)m_instance;
			}
		}

		internal ScaleRange(Microsoft.ReportingServices.ReportIntermediateFormat.ScaleRange defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

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
			if (m_startValue != null)
			{
				m_startValue.SetNewContext();
			}
			if (m_endValue != null)
			{
				m_endValue.SetNewContext();
			}
		}
	}
}
