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
	internal sealed class MapLineLayer : MapVectorLayer, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapLineTemplate m_mapLineTemplate;

		private MapLineRules m_mapLineRules;

		private List<MapLine> m_mapLines;

		internal MapLineTemplate MapLineTemplate
		{
			get
			{
				return m_mapLineTemplate;
			}
			set
			{
				m_mapLineTemplate = value;
			}
		}

		internal MapLineRules MapLineRules
		{
			get
			{
				return m_mapLineRules;
			}
			set
			{
				m_mapLineRules = value;
			}
		}

		internal List<MapLine> MapLines
		{
			get
			{
				return m_mapLines;
			}
			set
			{
				m_mapLines = value;
			}
		}

		protected override bool Embedded => MapLines != null;

		internal new MapLineLayerExprHost ExprHost => (MapLineLayerExprHost)m_exprHost;

		internal new MapLineLayerExprHost ExprHostMapMember => (MapLineLayerExprHost)m_exprHostMapMember;

		internal MapLineLayer()
		{
		}

		internal MapLineLayer(int ID, Map map)
			: base(ID, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineLayerStart(base.Name);
			base.Initialize(context);
			if (m_mapLineRules != null)
			{
				m_mapLineRules.Initialize(context);
			}
			if (base.MapDataRegionName == null)
			{
				if (m_mapLineTemplate != null)
				{
					m_mapLineTemplate.Initialize(context);
				}
				if (m_mapLines != null)
				{
					for (int i = 0; i < m_mapLines.Count; i++)
					{
						m_mapLines[i].Initialize(context, i);
					}
				}
			}
			m_exprHostID = context.ExprHostBuilder.MapLineLayerEnd();
		}

		internal override void InitializeMapMember(InitializationContext context)
		{
			context.ExprHostBuilder.MapLineLayerStart(base.Name);
			base.InitializeMapMember(context);
			if (m_mapLineRules != null)
			{
				m_mapLineRules.InitializeMapMember(context);
			}
			if (base.MapDataRegionName != null)
			{
				if (m_mapLineTemplate != null)
				{
					m_mapLineTemplate.Initialize(context);
				}
				if (m_mapLines != null)
				{
					for (int i = 0; i < m_mapLines.Count; i++)
					{
						m_mapLines[i].Initialize(context, i);
					}
				}
			}
			m_exprHostMapMemberID = context.ExprHostBuilder.MapLineLayerEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLineLayer mapLineLayer = (MapLineLayer)(context.CurrentMapVectorLayerClone = (MapLineLayer)base.PublishClone(context));
			if (m_mapLineTemplate != null)
			{
				mapLineLayer.m_mapLineTemplate = (MapLineTemplate)m_mapLineTemplate.PublishClone(context);
			}
			if (m_mapLineRules != null)
			{
				mapLineLayer.m_mapLineRules = (MapLineRules)m_mapLineRules.PublishClone(context);
			}
			if (m_mapLines != null)
			{
				mapLineLayer.m_mapLines = new List<MapLine>(m_mapLines.Count);
				{
					foreach (MapLine mapLine in m_mapLines)
					{
						mapLineLayer.m_mapLines.Add((MapLine)mapLine.PublishClone(context));
					}
					return mapLineLayer;
				}
			}
			return mapLineLayer;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			if (m_mapLineRules != null && ExprHost.MapLineRulesHost != null)
			{
				m_mapLineRules.SetExprHost(ExprHost.MapLineRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName != null)
			{
				return;
			}
			if (m_mapLineTemplate != null && ExprHost.MapLineTemplateHost != null)
			{
				m_mapLineTemplate.SetExprHost(ExprHost.MapLineTemplateHost, reportObjectModel);
			}
			IList<MapLineExprHost> mapLinesHostsRemotable = ExprHost.MapLinesHostsRemotable;
			if (m_mapLines == null || mapLinesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapLines.Count; i++)
			{
				MapLine mapLine = m_mapLines[i];
				if (mapLine != null && mapLine.ExpressionHostID > -1)
				{
					mapLine.SetExprHost(mapLinesHostsRemotable[mapLine.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal override void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHostMapMember(exprHost, reportObjectModel);
			if (m_mapLineRules != null && ExprHostMapMember.MapLineRulesHost != null)
			{
				m_mapLineRules.SetExprHostMapMember(ExprHostMapMember.MapLineRulesHost, reportObjectModel);
			}
			if (base.MapDataRegionName == null)
			{
				return;
			}
			if (m_mapLineTemplate != null && ExprHostMapMember.MapLineTemplateHost != null)
			{
				m_mapLineTemplate.SetExprHost(ExprHostMapMember.MapLineTemplateHost, reportObjectModel);
			}
			IList<MapLineExprHost> mapLinesHostsRemotable = ExprHostMapMember.MapLinesHostsRemotable;
			if (m_mapLines == null || mapLinesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapLines.Count; i++)
			{
				MapLine mapLine = m_mapLines[i];
				if (mapLine != null && mapLine.ExpressionHostID > -1)
				{
					mapLine.SetExprHost(mapLinesHostsRemotable[mapLine.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapLineTemplate, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineTemplate));
			list.Add(new MemberInfo(MemberName.MapLineRules, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineRules));
			list.Add(new MemberInfo(MemberName.MapLines, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLine));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapLineTemplate:
					writer.Write(m_mapLineTemplate);
					break;
				case MemberName.MapLineRules:
					writer.Write(m_mapLineRules);
					break;
				case MemberName.MapLines:
					writer.Write(m_mapLines);
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
				case MemberName.MapLineTemplate:
					m_mapLineTemplate = (MapLineTemplate)reader.ReadRIFObject();
					break;
				case MemberName.MapLineRules:
					m_mapLineRules = (MapLineRules)reader.ReadRIFObject();
					break;
				case MemberName.MapLines:
					m_mapLines = reader.ReadGenericListOfRIFObjects<MapLine>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLineLayer;
		}
	}
}
