using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class GaugePanelItem : GaugePanelObjectCollectionItem, IROMStyleDefinitionContainer, IROMActionOwner
	{
		internal GaugePanel m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem m_defObject;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportDoubleProperty m_top;

		private ReportDoubleProperty m_left;

		private ReportDoubleProperty m_height;

		private ReportDoubleProperty m_width;

		private ReportIntProperty m_zIndex;

		private ReportBoolProperty m_hidden;

		private ReportStringProperty m_toolTip;

		string IROMActionOwner.UniqueName => m_gaugePanel.GaugePanelDef.UniqueName + "x" + m_defObject.ID;

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

		public ReportDoubleProperty Height
		{
			get
			{
				if (m_height == null && m_defObject.Height != null)
				{
					m_height = new ReportDoubleProperty(m_defObject.Height);
				}
				return m_height;
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

		public ReportIntProperty ZIndex
		{
			get
			{
				if (m_zIndex == null && m_defObject.ZIndex != null)
				{
					m_zIndex = new ReportIntProperty(m_defObject.ZIndex.IsExpression, m_defObject.ZIndex.OriginalText, m_defObject.ZIndex.IntValue, 0);
				}
				return m_zIndex;
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

		public string ParentItem => m_defObject.ParentItem;

		internal GaugePanel GaugePanelDef => m_gaugePanel;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem GaugePanelItemDef => m_defObject;

		public GaugePanelItemInstance Instance => (GaugePanelItemInstance)GetInstance();

		internal GaugePanelItem(Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanelItem defObject, GaugePanel gaugePanel)
		{
			m_defObject = defObject;
			m_gaugePanel = gaugePanel;
		}

		internal abstract BaseInstance GetInstance();

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
		}
	}
}
