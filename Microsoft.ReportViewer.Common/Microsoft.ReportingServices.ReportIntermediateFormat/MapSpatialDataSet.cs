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
	internal sealed class MapSpatialDataSet : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_dataSetName;

		private ExpressionInfo m_spatialField;

		private List<MapFieldName> m_mapFieldNames;

		internal ExpressionInfo DataSetName
		{
			get
			{
				return m_dataSetName;
			}
			set
			{
				m_dataSetName = value;
			}
		}

		internal ExpressionInfo SpatialField
		{
			get
			{
				return m_spatialField;
			}
			set
			{
				m_spatialField = value;
			}
		}

		internal List<MapFieldName> MapFieldNames
		{
			get
			{
				return m_mapFieldNames;
			}
			set
			{
				m_mapFieldNames = value;
			}
		}

		internal new MapSpatialDataSetExprHost ExprHost => (MapSpatialDataSetExprHost)m_exprHost;

		internal MapSpatialDataSet()
		{
		}

		internal MapSpatialDataSet(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapSpatialDataSetStart();
			base.Initialize(context);
			if (m_dataSetName != null)
			{
				m_dataSetName.Initialize("DataSetName", context);
				context.ExprHostBuilder.MapSpatialDataSetDataSetName(m_dataSetName);
			}
			if (m_spatialField != null)
			{
				m_spatialField.Initialize("SpatialField", context);
				context.ExprHostBuilder.MapSpatialDataSetSpatialField(m_spatialField);
			}
			if (m_mapFieldNames != null)
			{
				for (int i = 0; i < m_mapFieldNames.Count; i++)
				{
					m_mapFieldNames[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapSpatialDataSetEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapSpatialDataSet mapSpatialDataSet = (MapSpatialDataSet)base.PublishClone(context);
			if (m_dataSetName != null)
			{
				mapSpatialDataSet.m_dataSetName = (ExpressionInfo)m_dataSetName.PublishClone(context);
			}
			if (m_spatialField != null)
			{
				mapSpatialDataSet.m_spatialField = (ExpressionInfo)m_spatialField.PublishClone(context);
			}
			if (m_mapFieldNames != null)
			{
				mapSpatialDataSet.m_mapFieldNames = new List<MapFieldName>(m_mapFieldNames.Count);
				{
					foreach (MapFieldName mapFieldName in m_mapFieldNames)
					{
						mapSpatialDataSet.m_mapFieldNames.Add((MapFieldName)mapFieldName.PublishClone(context));
					}
					return mapSpatialDataSet;
				}
			}
			return mapSpatialDataSet;
		}

		internal override void SetExprHost(MapSpatialDataExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHostInternal(exprHost, reportObjectModel);
			IList<MapFieldNameExprHost> mapFieldNamesHostsRemotable = ExprHost.MapFieldNamesHostsRemotable;
			if (m_mapFieldNames == null || mapFieldNamesHostsRemotable == null)
			{
				return;
			}
			for (int i = 0; i < m_mapFieldNames.Count; i++)
			{
				MapFieldName mapFieldName = m_mapFieldNames[i];
				if (mapFieldName != null && mapFieldName.ExpressionHostID > -1)
				{
					mapFieldName.SetExprHost(mapFieldNamesHostsRemotable[mapFieldName.ExpressionHostID], reportObjectModel);
				}
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.DataSetName, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.SpatialField, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapFieldNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldName));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataSet, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DataSetName:
					writer.Write(m_dataSetName);
					break;
				case MemberName.SpatialField:
					writer.Write(m_spatialField);
					break;
				case MemberName.MapFieldNames:
					writer.Write(m_mapFieldNames);
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
				case MemberName.DataSetName:
					m_dataSetName = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.SpatialField:
					m_spatialField = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapFieldNames:
					m_mapFieldNames = reader.ReadGenericListOfRIFObjects<MapFieldName>();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialDataSet;
		}

		internal string EvaluateDataSetName(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataSetDataSetNameExpression(this, m_map.Name);
		}

		internal string EvaluateSpatialField(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapSpatialDataSetSpatialFieldExpression(this, m_map.Name);
		}
	}
}
