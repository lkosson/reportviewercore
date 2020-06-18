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
	internal class MapSpatialElement : IPersistable
	{
		protected int m_exprHostID = -1;

		[NonSerialized]
		protected MapSpatialElementExprHost m_exprHost;

		[Reference]
		protected Map m_map;

		[Reference]
		protected MapVectorLayer m_mapVectorLayer;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private string m_vectorData;

		private List<MapField> m_mapFields;

		internal string VectorData
		{
			get
			{
				return m_vectorData;
			}
			set
			{
				m_vectorData = value;
			}
		}

		internal List<MapField> MapFields
		{
			get
			{
				return m_mapFields;
			}
			set
			{
				m_mapFields = value;
			}
		}

		internal string OwnerName => m_map.Name;

		internal MapSpatialElementExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		protected IInstancePath InstancePath => m_mapVectorLayer.InstancePath;

		internal MapSpatialElement()
		{
		}

		internal MapSpatialElement(MapVectorLayer mapVectorLayer, Map map)
		{
			m_map = map;
			m_mapVectorLayer = mapVectorLayer;
		}

		internal virtual void Initialize(InitializationContext context, int index)
		{
		}

		internal virtual object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialElement mapSpatialElement = (MapSpatialElement)MemberwiseClone();
			mapSpatialElement.m_map = context.CurrentMapClone;
			mapSpatialElement.m_mapVectorLayer = context.CurrentMapVectorLayerClone;
			if (m_mapFields != null)
			{
				mapSpatialElement.m_mapFields = new List<MapField>(m_mapFields.Count);
				{
					foreach (MapField mapField in m_mapFields)
					{
						mapSpatialElement.m_mapFields.Add((MapField)mapField.PublishClone(context));
					}
					return mapSpatialElement;
				}
			}
			return mapSpatialElement;
		}

		internal void SetExprHost(MapSpatialElementExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
		}

		internal static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.VectorData, Token.String));
			list.Add(new MemberInfo(MemberName.MapFields, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapField));
			list.Add(new MemberInfo(MemberName.Map, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Map, Token.Reference));
			list.Add(new MemberInfo(MemberName.MapVectorLayer, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapVectorLayer, Token.Reference));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		public virtual void Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Map:
					writer.WriteReference(m_map);
					break;
				case MemberName.MapVectorLayer:
					writer.WriteReference(m_mapVectorLayer);
					break;
				case MemberName.VectorData:
					writer.Write(m_vectorData);
					break;
				case MemberName.MapFields:
					writer.Write(m_mapFields);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Map:
					m_map = reader.ReadReference<Map>(this);
					break;
				case MemberName.MapVectorLayer:
					m_mapVectorLayer = reader.ReadReference<MapVectorLayer>(this);
					break;
				case MemberName.VectorData:
					m_vectorData = reader.ReadString();
					break;
				case MemberName.MapFields:
					m_mapFields = reader.ReadGenericListOfRIFObjects<MapField>();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.Map:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_map = (Map)referenceableItems[item.RefID];
					break;
				case MemberName.MapVectorLayer:
					Global.Tracer.Assert(referenceableItems.ContainsKey(item.RefID));
					m_mapVectorLayer = (MapVectorLayer)referenceableItems[item.RefID];
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public virtual Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialElement;
		}
	}
}
