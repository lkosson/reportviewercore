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
	internal sealed class MapPointLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapPointTemplate m_mapPointTemplate;

		private MapPointRules m_mapPointRules;

		private List<MapPoint> m_mapPoints;

		internal MapPointTemplate MapPointTemplate
		{
			get
			{
				return m_mapPointTemplate;
			}
			set
			{
				m_mapPointTemplate = value;
			}
		}

		internal MapPointRules MapPointRules
		{
			get
			{
				return m_mapPointRules;
			}
			set
			{
				m_mapPointRules = value;
			}
		}

		internal List<MapPoint> MapPoints
		{
			get
			{
				return m_mapPoints;
			}
			set
			{
				m_mapPoints = value;
			}
		}

		protected override bool Embedded => MapPoints != null;

		internal new MapPointLayerExprHost ExprHost => (MapPointLayerExprHost)m_exprHost;

		internal new MapPointLayerExprHost ExprHostMapMember => (MapPointLayerExprHost)m_exprHostMapMember;

		internal MapPointLayer()
		{
		}

		internal MapPointLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointLayerStart(base.Name);
			base.Initialize(context);
			if (m_mapPointRules != null)
			{
				m_mapPointRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (m_mapPointTemplate != null)
				{
					m_mapPointTemplate.Initialize(context);
				}
				if (m_mapPoints != null)
				{
					for (int i = 0; i < m_mapPoints.Count; i++)
					{
						m_mapPoints[i].Initialize(context, i);
					}
				}
			}
			m_exprHostID = context.ExprHostBuilder.MapPointLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapPointLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (m_mapPointRules != null)
			{
				m_mapPointRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (m_mapPointTemplate != null)
				{
					m_mapPointTemplate.Initialize(context);
				}
				if (m_mapPoints != null)
				{
					for (int i = 0; i < m_mapPoints.Count; i++)
					{
						m_mapPoints[i].Initialize(context, i);
					}
				}
			}
			m_exprHostMapMemberID = context.ExprHostBuilder.MapPointLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapPointLayer mapPointLayer = (MapPointLayer)(context.CurrentMapVectorLayerClone = (MapPointLayer)base.PublishClone(context));
			if (m_mapPointTemplate != null)
			{
				mapPointLayer.m_mapPointTemplate = (MapPointTemplate)m_mapPointTemplate.PublishClone(context);
			}
			if (m_mapPointRules != null)
			{
				mapPointLayer.m_mapPointRules = (MapPointRules)m_mapPointRules.PublishClone(context);
			}
			if (m_mapPoints != null)
			{
				mapPointLayer.m_mapPoints = new List<MapPoint>(m_mapPoints.Count);
				{
					foreach (MapPoint mapPoint in m_mapPoints)
					{
						mapPointLayer.m_mapPoints.Add((MapPoint)mapPoint.PublishClone(context));
					}
					return mapPointLayer;
				}
			}
			return mapPointLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_mapPointRules != null && ExprHost.MapPointRulesHost != null)
			{
				m_mapPointRules.SetExprHost(ExprHost.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				return;
			}
			if (m_mapPointTemplate != null && ExprHost.MapPointTemplateHost != null)
			{
				m_mapPointTemplate.SetExprHost(ExprHost.MapPointTemplateHost, reportObjectModel);
			}
			IList<MapPointExprHost> mapPointsHostsRemotable = ExprHost.MapPointsHostsRemotable;
			if (m_mapPoints == null || mapPointsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapPoints.Count; i++)
			{
				MapPoint mapPoint = m_mapPoints[i];
				if (mapPoint != null && mapPoint.ExpressionHostID > -1)
				{
					mapPoint.SetExprHost(mapPointsHostsRemotable[mapPoint.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (m_mapPointRules != null && ExprHostMapMember.MapPointRulesHost != null)
			{
				m_mapPointRules.SetExprHostMapMember(ExprHostMapMember.MapPointRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				return;
			}
			if (m_mapPointTemplate != null && ExprHostMapMember.MapPointTemplateHost != null)
			{
				m_mapPointTemplate.SetExprHost(ExprHostMapMember.MapPointTemplateHost, reportObjectModel);
			}
			IList<MapPointExprHost> mapPointsHostsRemotable = ExprHostMapMember.MapPointsHostsRemotable;
			if (m_mapPoints == null || mapPointsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapPoints.Count; i++)
			{
				MapPoint mapPoint = m_mapPoints[i];
				if (mapPoint != null && mapPoint.ExpressionHostID > -1)
				{
					mapPoint.SetExprHost(mapPointsHostsRemotable[mapPoint.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapPointTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointTemplate));
			list.Add(new MemberInfo(MemberName.MapPointRules, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointRules));
			list.Add(new MemberInfo(MemberName.MapPoints, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPoint));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapPointTemplate:
					writer.Write(m_mapPointTemplate);
					break;
				case MemberName.MapPointRules:
					writer.Write(m_mapPointRules);
					break;
				case MemberName.MapPoints:
					writer.Write(m_mapPoints);
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
				case MemberName.MapPointTemplate:
					m_mapPointTemplate = (MapPointTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapPointRules:
					m_mapPointRules = (MapPointRules)reader.ReadRIFObject();
					break;
				case MemberName.MapPoints:
					m_mapPoints = reader.ReadGenericListOfRIFObjects<MapPoint>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapPointLayer;
		}
	}
}
