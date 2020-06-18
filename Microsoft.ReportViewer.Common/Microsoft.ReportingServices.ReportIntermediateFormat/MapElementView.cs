using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
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
	internal sealed class MapElementView : MapView, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_layerName;

		private List<MapBindingFieldPair> m_mapBindingFieldPairs;

		internal ExpressionInfo LayerName
		{
			get
			{
				return m_layerName;
			}
			set
			{
				m_layerName = value;
			}
		}

		internal List<MapBindingFieldPair> MapBindingFieldPairs
		{
			get
			{
				return m_mapBindingFieldPairs;
			}
			set
			{
				m_mapBindingFieldPairs = value;
			}
		}

		internal new MapElementViewExprHost ExprHost => (MapElementViewExprHost)m_exprHost;

		internal MapElementView()
		{
		}

		internal MapElementView(Map map)
			: base(map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapElementViewStart();
			base.Initialize(context);
			if (m_layerName != null)
			{
				m_layerName.Initialize("LayerName", context);
				context.ExprHostBuilder.MapElementViewLayerName(m_layerName);
			}
			if (m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
				{
					m_mapBindingFieldPairs[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapElementViewEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapElementView mapElementView = (MapElementView)base.PublishClone(context);
			if (m_layerName != null)
			{
				mapElementView.m_layerName = (ExpressionInfo)m_layerName.PublishClone(context);
			}
			if (m_mapBindingFieldPairs != null)
			{
				mapElementView.m_mapBindingFieldPairs = new List<MapBindingFieldPair>(m_mapBindingFieldPairs.Count);
				{
					foreach (MapBindingFieldPair mapBindingFieldPair in m_mapBindingFieldPairs)
					{
						mapElementView.m_mapBindingFieldPairs.Add((MapBindingFieldPair)mapBindingFieldPair.PublishClone(context));
					}
					return mapElementView;
				}
			}
			return mapElementView;
		}

		internal override void SetExprHost(MapViewExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = ExprHost.MapBindingFieldPairsHostsRemotable;
			if (m_mapBindingFieldPairs == null || mapBindingFieldPairsHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
			{
				MapBindingFieldPair mapBindingFieldPair = m_mapBindingFieldPairs[i];
				if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostID > -1)
				{
					mapBindingFieldPair.SetExprHost(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.LayerName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapBindingFieldPairs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapElementView, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapView, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.LayerName:
					writer.Write(m_layerName);
					break;
				case MemberName.MapBindingFieldPairs:
					writer.Write(m_mapBindingFieldPairs);
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
				case MemberName.LayerName:
					m_layerName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapBindingFieldPairs:
					m_mapBindingFieldPairs = reader.ReadGenericListOfRIFObjects<MapBindingFieldPair>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapElementView;
		}

		internal string EvaluateLayerName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapElementViewLayerNameExpression(this, m_map.Name);
		}
	}
}
