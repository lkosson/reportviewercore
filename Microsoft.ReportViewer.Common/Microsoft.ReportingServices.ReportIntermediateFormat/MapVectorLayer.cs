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
	internal abstract class MapVectorLayer : MapLayer, IPersistable, IReferenceable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_mapDataRegionName;

		private int m_ID;

		private List<MapBindingFieldPair> m_mapBindingFieldPairs;

		private List<MapFieldDefinition> m_mapFieldDefinitions;

		private MapSpatialData m_mapSpatialData;

		private string m_dataElementName;

		private DataElementOutputTypes m_dataElementOutput;

		protected int m_exprHostMapMemberID = -1;

		[NonSerialized]
		protected MapVectorLayerExprHost m_exprHostMapMember;

		[NonSerialized]
		private IInstancePath m_instancePath;

		internal string DataElementName
		{
			get
			{
				if (string.IsNullOrEmpty(m_dataElementName))
				{
					return base.Name;
				}
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		public int ID => m_ID;

		internal string MapDataRegionName
		{
			get
			{
				return m_mapDataRegionName;
			}
			set
			{
				m_mapDataRegionName = value;
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

		internal List<MapFieldDefinition> MapFieldDefinitions
		{
			get
			{
				return m_mapFieldDefinitions;
			}
			set
			{
				m_mapFieldDefinitions = value;
			}
		}

		internal MapSpatialData MapSpatialData
		{
			get
			{
				return m_mapSpatialData;
			}
			set
			{
				m_mapSpatialData = value;
			}
		}

		internal new MapVectorLayerExprHost ExprHost => (MapVectorLayerExprHost)m_exprHost;

		internal MapVectorLayerExprHost ExprHostMapMember => m_exprHostMapMember;

		internal int ExpressionHostMapMemberID
		{
			get
			{
				return m_exprHostMapMemberID;
			}
			set
			{
				m_exprHostMapMemberID = value;
			}
		}

		internal IInstancePath InstancePath
		{
			get
			{
				if (m_instancePath == null)
				{
					if (m_mapDataRegionName != null)
					{
						foreach (MapDataRegion mapDataRegion in m_map.MapDataRegions)
						{
							if (string.CompareOrdinal(m_mapDataRegionName, mapDataRegion.Name) == 0)
							{
								m_instancePath = mapDataRegion.InnerMostMapMember;
							}
						}
					}
					if (m_instancePath == null)
					{
						m_instancePath = m_map;
					}
				}
				return m_instancePath;
			}
		}

		protected abstract bool Embedded
		{
			get;
		}

		internal MapVectorLayer()
		{
		}

		internal MapVectorLayer(int ID, Map map)
			: base(map)
		{
			m_ID = ID;
		}

		internal void Validate(PublishingErrorContext errorContext)
		{
			if (MapSpatialData is MapSpatialDataRegion && MapDataRegionName == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMapLayerMissingProperty, Severity.Error, m_map.ObjectType, m_map.Name, m_name, "MapDataRegionName");
			}
			if (!(MapSpatialData is MapSpatialDataRegion) && MapDataRegionName != null && MapBindingFieldPairs == null)
			{
				errorContext.Register(ProcessingErrorCode.rsMapLayerMissingProperty, Severity.Error, m_map.ObjectType, m_map.Name, m_name, "MapBindingFieldPairs");
			}
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_mapSpatialData != null)
			{
				m_mapSpatialData.Initialize(context);
			}
			if (m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
				{
					m_mapBindingFieldPairs[i].Initialize(context, i);
				}
			}
		}

		internal virtual void InitializeMapMember(InitializationContext context)
		{
			if (m_mapSpatialData != null)
			{
				m_mapSpatialData.InitializeMapMember(context);
			}
			if (m_mapBindingFieldPairs != null)
			{
				for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
				{
					m_mapBindingFieldPairs[i].InitializeMapMember(context, i);
				}
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapVectorLayer mapVectorLayer2 = context.CurrentMapVectorLayerClone = (MapVectorLayer)base.PublishClone(context);
			mapVectorLayer2.m_ID = context.GenerateID();
			if (MapDataRegionName != null)
			{
				mapVectorLayer2.MapDataRegionName = context.GetNewScopeName(MapDataRegionName);
			}
			if (m_mapBindingFieldPairs != null)
			{
				mapVectorLayer2.m_mapBindingFieldPairs = new List<MapBindingFieldPair>(m_mapBindingFieldPairs.Count);
				foreach (MapBindingFieldPair mapBindingFieldPair in m_mapBindingFieldPairs)
				{
					mapVectorLayer2.m_mapBindingFieldPairs.Add((MapBindingFieldPair)mapBindingFieldPair.PublishClone(context));
				}
			}
			if (m_mapFieldDefinitions != null)
			{
				mapVectorLayer2.m_mapFieldDefinitions = new List<MapFieldDefinition>(m_mapFieldDefinitions.Count);
				foreach (MapFieldDefinition mapFieldDefinition in m_mapFieldDefinitions)
				{
					mapVectorLayer2.m_mapFieldDefinitions.Add((MapFieldDefinition)mapFieldDefinition.PublishClone(context));
				}
			}
			if (m_mapSpatialData != null)
			{
				mapVectorLayer2.m_mapSpatialData = (MapSpatialData)m_mapSpatialData.PublishClone(context);
			}
			return mapVectorLayer2;
		}

		internal override void SetExprHost(MapLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			base.SetExprHost(exprHost, reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = ExprHost.MapBindingFieldPairsHostsRemotable;
			if (m_mapBindingFieldPairs != null && mapBindingFieldPairsHostsRemotable != null)
			{
				for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
				{
					MapBindingFieldPair mapBindingFieldPair = m_mapBindingFieldPairs[i];
					if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostID > -1)
					{
						mapBindingFieldPair.SetExprHost(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_mapSpatialData != null && ExprHost.MapSpatialDataHost != null)
			{
				m_mapSpatialData.SetExprHost(ExprHost.MapSpatialDataHost, reportObjectModel);
			}
		}

		internal virtual void SetExprHostMapMember(MapVectorLayerExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHostMapMember = exprHost;
			m_exprHostMapMember.SetReportObjectModel(reportObjectModel);
			IList<MapBindingFieldPairExprHost> mapBindingFieldPairsHostsRemotable = ExprHostMapMember.MapBindingFieldPairsHostsRemotable;
			if (m_mapBindingFieldPairs != null && mapBindingFieldPairsHostsRemotable != null)
			{
				for (int i = 0; i < m_mapBindingFieldPairs.Count; i++)
				{
					MapBindingFieldPair mapBindingFieldPair = m_mapBindingFieldPairs[i];
					if (mapBindingFieldPair != null && mapBindingFieldPair.ExpressionHostMapMemberID > -1)
					{
						mapBindingFieldPair.SetExprHostMapMember(mapBindingFieldPairsHostsRemotable[mapBindingFieldPair.ExpressionHostMapMemberID], reportObjectModel);
					}
				}
			}
			if (m_mapSpatialData != null && ExprHostMapMember.MapSpatialDataHost != null)
			{
				m_mapSpatialData.SetExprHostMapMember(ExprHostMapMember.MapSpatialDataHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapDataRegionName, Token.String));
			list.Add(new MemberInfo(MemberName.MapBindingFieldPairs, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapBindingFieldPair));
			list.Add(new MemberInfo(MemberName.MapFieldDefinitions, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldDefinition));
			list.Add(new MemberInfo(MemberName.MapSpatialData, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData));
			list.Add(new MemberInfo(MemberName.ExprHostMapMemberID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLayer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapDataRegionName:
					writer.Write(m_mapDataRegionName);
					break;
				case MemberName.MapBindingFieldPairs:
					writer.Write(m_mapBindingFieldPairs);
					break;
				case MemberName.MapFieldDefinitions:
					writer.Write(m_mapFieldDefinitions);
					break;
				case MemberName.MapSpatialData:
					writer.Write(m_mapSpatialData);
					break;
				case MemberName.ExprHostMapMemberID:
					writer.Write(m_exprHostMapMemberID);
					break;
				case MemberName.ID:
					writer.Write(m_ID);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
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
				case MemberName.MapDataRegionName:
					m_mapDataRegionName = reader.ReadString();
					break;
				case MemberName.MapBindingFieldPairs:
					m_mapBindingFieldPairs = reader.ReadGenericListOfRIFObjects<MapBindingFieldPair>();
					break;
				case MemberName.MapFieldDefinitions:
					m_mapFieldDefinitions = reader.ReadGenericListOfRIFObjects<MapFieldDefinition>();
					break;
				case MemberName.MapSpatialData:
					m_mapSpatialData = (MapSpatialData)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostMapMemberID:
					m_exprHostMapMemberID = reader.ReadInt32();
					break;
				case MemberName.ID:
					m_ID = reader.ReadInt32();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer;
		}
	}
}
