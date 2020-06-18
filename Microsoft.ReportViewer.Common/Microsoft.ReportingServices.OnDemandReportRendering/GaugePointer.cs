using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePointer : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private GaugeInputValue m_gaugeInputValue;

		private ReportEnumProperty<GaugeBarStarts> m_barStart;

		private ReportDoubleProperty m_distanceFromScale;

		private PointerImage m_pointerImage;

		private ReportDoubleProperty m_markerLength;

		private ReportEnumProperty<GaugeMarkerStyles> m_markerStyle;

		private ReportEnumProperty<GaugePointerPlacements> m_placement;

		private ReportBoolProperty m_snappingEnabled;

		private ReportDoubleProperty m_snappingInterval;

		private ReportStringProperty m_toolTip;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_width;

		private CompiledGaugePointerInstance[] m_compiledInstances;

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

		public GaugeInputValue GaugeInputValue
		{
			get
			{
				if (m_gaugeInputValue == null && m_defObject.GaugeInputValue != null)
				{
					m_gaugeInputValue = new GaugeInputValue(m_defObject.GaugeInputValue, m_gaugePanel);
				}
				return m_gaugeInputValue;
			}
		}

		public ReportEnumProperty<GaugeBarStarts> BarStart
		{
			get
			{
				if (m_barStart == null && m_defObject.BarStart != null)
				{
					m_barStart = new ReportEnumProperty<GaugeBarStarts>(m_defObject.BarStart.IsExpression, m_defObject.BarStart.OriginalText, EnumTranslator.TranslateGaugeBarStarts(m_defObject.BarStart.StringValue, null));
				}
				return m_barStart;
			}
		}

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

		public PointerImage PointerImage
		{
			get
			{
				if (m_pointerImage == null && m_defObject.PointerImage != null)
				{
					m_pointerImage = new PointerImage(m_defObject.PointerImage, m_gaugePanel);
				}
				return m_pointerImage;
			}
		}

		public ReportDoubleProperty MarkerLength
		{
			get
			{
				if (m_markerLength == null && m_defObject.MarkerLength != null)
				{
					m_markerLength = new ReportDoubleProperty(m_defObject.MarkerLength);
				}
				return m_markerLength;
			}
		}

		public ReportEnumProperty<GaugeMarkerStyles> MarkerStyle
		{
			get
			{
				if (m_markerStyle == null && m_defObject.MarkerStyle != null)
				{
					m_markerStyle = new ReportEnumProperty<GaugeMarkerStyles>(m_defObject.MarkerStyle.IsExpression, m_defObject.MarkerStyle.OriginalText, EnumTranslator.TranslateGaugeMarkerStyles(m_defObject.MarkerStyle.StringValue, null));
				}
				return m_markerStyle;
			}
		}

		public ReportEnumProperty<GaugePointerPlacements> Placement
		{
			get
			{
				if (m_placement == null && m_defObject.Placement != null)
				{
					m_placement = new ReportEnumProperty<GaugePointerPlacements>(m_defObject.Placement.IsExpression, m_defObject.Placement.OriginalText, EnumTranslator.TranslateGaugePointerPlacements(m_defObject.Placement.StringValue, null));
				}
				return m_placement;
			}
		}

		public ReportBoolProperty SnappingEnabled
		{
			get
			{
				if (m_snappingEnabled == null && m_defObject.SnappingEnabled != null)
				{
					m_snappingEnabled = new ReportBoolProperty(m_defObject.SnappingEnabled);
				}
				return m_snappingEnabled;
			}
		}

		public ReportDoubleProperty SnappingInterval
		{
			get
			{
				if (m_snappingInterval == null && m_defObject.SnappingInterval != null)
				{
					m_snappingInterval = new ReportDoubleProperty(m_defObject.SnappingInterval);
				}
				return m_snappingInterval;
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

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer GaugePointerDef => m_defObject;

		public GaugePointerInstance Instance => GetInstance();

		public CompiledGaugePointerInstance[] CompiledInstances
		{
			get
			{
				GaugePanelDef.ProcessCompiledInstances();
				return m_compiledInstances;
			}
			internal set
			{
				m_compiledInstances = value;
			}
		}

		internal GaugePointer(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePointer defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal abstract GaugePointerInstance GetInstance();

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
			if (m_gaugeInputValue != null)
			{
				m_gaugeInputValue.SetNewContext();
			}
			if (m_pointerImage != null)
			{
				m_pointerImage.SetNewContext();
			}
		}
	}
}
