using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapDockableSubItem : MapSubItem, IROMActionOwner
	{
		private ActionInfo m_actionInfo;

		private ReportEnumProperty<MapPosition> m_position;

		private ReportBoolProperty m_dockOutsideViewport;

		private ReportBoolProperty m_hidden;

		private ReportStringProperty m_toolTip;

		public string UniqueName => m_map.MapDef.UniqueName + "x" + MapDockableSubItemDef.ID;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null && MapDockableSubItemDef.Action != null)
				{
					m_actionInfo = new ActionInfo(m_map.RenderingContext, m_map.ReportScope, MapDockableSubItemDef.Action, m_map.MapDef, m_map, ObjectType.Map, m_map.Name, this);
				}
				return m_actionInfo;
			}
		}

		public List<string> FieldsUsedInValueExpression => null;

		public ReportEnumProperty<MapPosition> Position
		{
			get
			{
				if (m_position == null && MapDockableSubItemDef.Position != null)
				{
					m_position = new ReportEnumProperty<MapPosition>(MapDockableSubItemDef.Position.IsExpression, MapDockableSubItemDef.Position.OriginalText, EnumTranslator.TranslateMapPosition(MapDockableSubItemDef.Position.StringValue, null));
				}
				return m_position;
			}
		}

		public ReportBoolProperty DockOutsideViewport
		{
			get
			{
				if (m_dockOutsideViewport == null && MapDockableSubItemDef.DockOutsideViewport != null)
				{
					m_dockOutsideViewport = new ReportBoolProperty(MapDockableSubItemDef.DockOutsideViewport);
				}
				return m_dockOutsideViewport;
			}
		}

		public ReportBoolProperty Hidden
		{
			get
			{
				if (m_hidden == null && MapDockableSubItemDef.Hidden != null)
				{
					m_hidden = new ReportBoolProperty(MapDockableSubItemDef.Hidden);
				}
				return m_hidden;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null && MapDockableSubItemDef.ToolTip != null)
				{
					m_toolTip = new ReportStringProperty(MapDockableSubItemDef.ToolTip);
				}
				return m_toolTip;
			}
		}

		internal Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem MapDockableSubItemDef => (Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)m_defObject;

		internal new MapDockableSubItemInstance Instance => (MapDockableSubItemInstance)GetInstance();

		internal MapDockableSubItem(Microsoft.ReportingServices.ReportIntermediateFormat.MapDockableSubItem defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
		}
	}
}
