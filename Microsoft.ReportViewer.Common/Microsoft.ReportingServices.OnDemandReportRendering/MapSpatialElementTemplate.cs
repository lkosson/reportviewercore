using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSpatialElementTemplate : IROMStyleDefinitionContainer, IROMActionOwner
	{
		protected Map m_map;

		protected MapVectorLayer m_mapVectorLayer;

		private Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate m_defObject;

		protected MapSpatialElementTemplateInstance m_instance;

		private Style m_style;

		private ActionInfo m_actionInfo;

		private ReportBoolProperty m_hidden;

		private ReportDoubleProperty m_offsetX;

		private ReportDoubleProperty m_offsetY;

		private ReportStringProperty m_label;

		private ReportStringProperty m_toolTip;

		private ReportStringProperty m_dataElementLabel;

		public Style Style
		{
			get
			{
				if (m_style == null)
				{
					m_style = new Style(m_map, ReportScope, m_defObject, m_map.RenderingContext);
				}
				return m_style;
			}
		}

		public string UniqueName => m_mapVectorLayer.ReportScope.ReportScopeInstance.UniqueName + "x" + m_defObject.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && m_defObject.Action != null)
				{
					m_actionInfo = new ActionInfo(m_map.RenderingContext, ReportScope, m_defObject.Action, m_map.MapDef, m_map, ObjectType.Map, m_map.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

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

		public ReportDoubleProperty OffsetX
		{
			get
			{
				if (m_offsetX == null && m_defObject.OffsetX != null)
				{
					m_offsetX = new ReportDoubleProperty(m_defObject.OffsetX);
				}
				return m_offsetX;
			}
		}

		public ReportDoubleProperty OffsetY
		{
			get
			{
				if (m_offsetY == null && m_defObject.OffsetY != null)
				{
					m_offsetY = new ReportDoubleProperty(m_defObject.OffsetY);
				}
				return m_offsetY;
			}
		}

		public ReportStringProperty Label
		{
			get
			{
				if (m_label == null && m_defObject.Label != null)
				{
					m_label = new ReportStringProperty(m_defObject.Label);
				}
				return m_label;
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

		public ReportStringProperty DataElementLabel
		{
			get
			{
				if (m_dataElementLabel == null)
				{
					if (m_defObject.DataElementLabel == null)
					{
						return Label;
					}
					m_dataElementLabel = new ReportStringProperty(m_defObject.DataElementLabel);
				}
				return m_dataElementLabel;
			}
		}

		public string DataElementName => m_defObject.DataElementName;

		public DataElementOutputTypes DataElementOutput => m_defObject.DataElementOutput;

		internal IReportScope ReportScope => m_mapVectorLayer.ReportScope;

		internal Map MapDef => m_map;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate MapSpatialElementTemplateDef => m_defObject;

		public MapSpatialElementTemplateInstance Instance => GetInstance();

		internal MapSpatialElementTemplate(Microsoft.ReportingServices.ReportIntermediateFormat.MapSpatialElementTemplate defObject, MapVectorLayer mapVectorLayer, Map map)
		{
			m_defObject = defObject;
			m_mapVectorLayer = mapVectorLayer;
			m_map = map;
		}

		internal abstract MapSpatialElementTemplateInstance GetInstance();

		internal virtual void SetNewContext()
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
