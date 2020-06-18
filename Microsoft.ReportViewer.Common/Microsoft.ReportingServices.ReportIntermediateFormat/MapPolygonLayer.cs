using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapPolygonLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapPolygonTemplate m_mapPolygonTemplate;

		private MapPolygonRules m_mapPolygonRules;

		private MapPointTemplate m_mapCenterPointTemplate;

		private MapPointRules m_mapCenterPointRules;

		private List<MapPolygon> m_mapPolygons;

		internal MapPolygonTemplate MapPolygonTemplate
		{
			get
			{
				return m_mapPolygonTemplate;
			}
			set
			{
				m_mapPolygonTemplate = value;
			}
		}

		internal MapPolygonRules MapPolygonRules
		{
			get
			{
				return m_mapPolygonRules;
			}
			set
			{
				m_mapPolygonRules = value;
			}
		}

		internal MapPointTemplate MapCenterPointTemplate
		{
			get
			{
				return m_mapCenterPointTemplate;
			}
			set
			{
				m_mapCenterPointTemplate = value;
			}
		}

		internal MapPointRules MapCenterPointRules
		{
			get
			{
				return m_mapCenterPointRules;
			}
			set
			{
				m_mapCenterPointRules = value;
			}
		}

		internal List<MapPolygon> MapPolygons
		{
			get
			{
				return m_mapPolygons;
			}
			set
			{
				m_mapPolygons = value;
			}
		}

		protected override bool Embedded => MapPolygons != null;

		internal new MapPolygonLayerExprHost ExprHost => (MapPolygonLayerExprHost)m_exprHost;

		internal new MapPolygonLayerExprHost ExprHostMapMember => (MapPolygonLayerExprHost)m_exprHostMapMember;

		internal MapPolygonLayer()
		{
		}

		internal MapPolygonLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPolygonLayerStart(base.Name);
			base.Initialize(context);
			if (m_mapPolygonRules != null)
			{
				m_mapPolygonRules.Initialize(context);
			}
			if (m_mapCenterPointRules != null)
			{
				m_mapCenterPointRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (m_mapPolygonTemplate != null)
				{
					m_mapPolygonTemplate.Initialize(context);
				}
				if (m_mapCenterPointTemplate != null)
				{
					m_mapCenterPointTemplate.Initialize(context);
				}
				if (m_mapPolygons != null)
				{
					for (int i = 0; i < m_mapPolygons.Count; i++)
					{
						m_mapPolygons[i].Initialize(context, i);
					}
				}
			}
			m_exprHostID = context.ExprHostBuilder.MapPolygonLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapPolygonLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (m_mapPolygonRules != null)
			{
				m_mapPolygonRules.InitializeMapMember(context);
			}
			if (m_mapCenterPointRules != null)
			{
				m_mapCenterPointRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (m_mapPolygonTemplate != null)
				{
					m_mapPolygonTemplate.Initialize(context);
				}
				if (m_mapCenterPointTemplate != null)
				{
					m_mapCenterPointTemplate.Initialize(context);
				}
				if (m_mapPolygons != null)
				{
					for (int i = 0; i < m_mapPolygons.Count; i++)
					{
						m_mapPolygons[i].Initialize(context, i);
					}
				}
			}
			m_exprHostMapMemberID = context.ExprHostBuilder.MapPolygonLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPolygonLayer mapPolygonLayer = (MapPolygonLayer)(context.CurrentMapVectorLayerClone = (MapPolygonLayer)base.PublishClone(context));
			if (m_mapPolygonTemplate != null)
			{
				mapPolygonLayer.m_mapPolygonTemplate = (MapPolygonTemplate)m_mapPolygonTemplate.PublishClone(context);
			}
			if (m_mapPolygonRules != null)
			{
				mapPolygonLayer.m_mapPolygonRules = (MapPolygonRules)m_mapPolygonRules.PublishClone(context);
			}
			if (m_mapCenterPointTemplate != null)
			{
				mapPolygonLayer.m_mapCenterPointTemplate = (MapPointTemplate)m_mapCenterPointTemplate.PublishClone(context);
			}
			if (m_mapCenterPointRules != null)
			{
				mapPolygonLayer.m_mapCenterPointRules = (MapPointRules)m_mapCenterPointRules.PublishClone(context);
			}
			if (m_mapPolygons != null)
			{
				mapPolygonLayer.m_mapPolygons = new List<MapPolygon>(m_mapPolygons.Count);
				{
					foreach (MapPolygon mapPolygon in m_mapPolygons)
					{
						mapPolygonLayer.m_mapPolygons.Add((MapPolygon)mapPolygon.PublishClone(context));
					}
					return mapPolygonLayer;
				}
			}
			return mapPolygonLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_mapPolygonRules != null && ExprHost.MapPolygonRulesHost != null)
			{
				m_mapPolygonRules.SetExprHost(ExprHost.MapPolygonRulesHost, reportObjectModel);
			}
			if (m_mapCenterPointRules != null && ExprHost.MapPointRulesHost != null)
			{
				m_mapCenterPointRules.SetExprHost(ExprHost.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				return;
			}
			if (m_mapPolygonTemplate != null && ExprHost.MapPolygonTemplateHost != null)
			{
				m_mapPolygonTemplate.SetExprHost(ExprHost.MapPolygonTemplateHost, reportObjectModel);
			}
			if (m_mapCenterPointTemplate != null && ExprHost.MapPointTemplateHost != null)
			{
				m_mapCenterPointTemplate.SetExprHost(ExprHost.MapPointTemplateHost, reportObjectModel);
			}
			IList<MapPolygonExprHost> mapPolygonsHostsRemotable = ExprHost.MapPolygonsHostsRemotable;
			if (m_mapPolygons == null || mapPolygonsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapPolygons.Count; i++)
			{
				MapPolygon mapPolygon = m_mapPolygons[i];
				if (mapPolygon != null && mapPolygon.ExpressionHostID > -1)
				{
					mapPolygon.SetExprHost(mapPolygonsHostsRemotable[mapPolygon.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (m_mapPolygonRules != null && ExprHostMapMember.MapPolygonRulesHost != null)
			{
				m_mapPolygonRules.SetExprHostMapMember(ExprHostMapMember.MapPolygonRulesHost, reportObjectModel);
			}
			if (m_mapCenterPointRules != null && ExprHostMapMember.MapPointRulesHost != null)
			{
				m_mapCenterPointRules.SetExprHostMapMember(ExprHostMapMember.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				return;
			}
			if (m_mapPolygonTemplate != null && ExprHostMapMember.MapPolygonTemplateHost != null)
			{
				m_mapPolygonTemplate.SetExprHost(ExprHostMapMember.MapPolygonTemplateHost, reportObjectModel);
			}
			if (m_mapCenterPointTemplate != null && ExprHostMapMember.MapPointTemplateHost != null)
			{
				m_mapCenterPointTemplate.SetExprHost(ExprHostMapMember.MapPointTemplateHost, reportObjectModel);
			}
			IList<MapPolygonExprHost> mapPolygonsHostsRemotable = ExprHostMapMember.MapPolygonsHostsRemotable;
			if (m_mapPolygons == null || mapPolygonsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapPolygons.Count; i++)
			{
				MapPolygon mapPolygon = m_mapPolygons[i];
				if (mapPolygon != null && mapPolygon.ExpressionHostID > -1)
				{
					mapPolygon.SetExprHost(mapPolygonsHostsRemotable[mapPolygon.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapPolygonTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonTemplate));
			list.Add(new MemberInfo(MemberName.MapPolygonRules, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonRules));
			list.Add(new MemberInfo(MemberName.MapPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			list.Add(new MemberInfo(MemberName.MapPointRules, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules));
			list.Add(new MemberInfo(MemberName.MapPolygons, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygon));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapPolygonTemplate:
					writer.Write(m_mapPolygonTemplate);
					break;
				case MemberName.MapPolygonRules:
					writer.Write(m_mapPolygonRules);
					break;
				case MemberName.MapPointTemplate:
					writer.Write(m_mapCenterPointTemplate);
					break;
				case MemberName.MapPointRules:
					writer.Write(m_mapCenterPointRules);
					break;
				case MemberName.MapPolygons:
					writer.Write(m_mapPolygons);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.MapPolygonTemplate:
					m_mapPolygonTemplate = (MapPolygonTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygonRules:
					m_mapPolygonRules = (MapPolygonRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPointTemplate:
					m_mapCenterPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPointRules:
					m_mapCenterPointRules = (MapPointRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPolygons:
					m_mapPolygons = reader.ReadGenericListOfRIFObjects<MapPolygon>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPolygonLayer;
		}
	}
}
