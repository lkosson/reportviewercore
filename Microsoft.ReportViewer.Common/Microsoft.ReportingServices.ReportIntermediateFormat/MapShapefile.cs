using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class MapShapefile : MapSpatialData, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_source;

		private List<MapFieldName> m_mapFieldNames;

		internal ExpressionInfo Source
		{
			get
			{
				return m_source;
			}
			set
			{
				m_source = value;
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

		internal new MapShapefileExprHost ExprHost => (MapShapefileExprHost)m_exprHost;

		internal MapShapefile()
		{
		}

		internal MapShapefile(MapVectorLayer mapVectorLayer, Map map)
			: base(mapVectorLayer, map)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapShapefileStart();
			base.Initialize(context);
			if (m_source != null)
			{
				m_source.Initialize("Source", context);
				context.ExprHostBuilder.MapShapefileSource(m_source);
			}
			if (m_mapFieldNames != null)
			{
				for (int i = 0; i < m_mapFieldNames.Count; i++)
				{
					m_mapFieldNames[i].Initialize(context, i);
				}
			}
			context.ExprHostBuilder.MapShapefileEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapShapefile mapShapefile = (MapShapefile)base.PublishClone(context);
			if (m_source != null)
			{
				mapShapefile.m_source = (ExpressionInfo)m_source.PublishClone(context);
			}
			if (m_mapFieldNames != null)
			{
				mapShapefile.m_mapFieldNames = new List<MapFieldName>(m_mapFieldNames.Count);
				{
					foreach (MapFieldName mapFieldName in m_mapFieldNames)
					{
						mapShapefile.m_mapFieldNames.Add((MapFieldName)mapFieldName.PublishClone(context));
					}
					return mapShapefile;
				}
			}
			return mapShapefile;
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
			list.Add(new MemberInfo(MemberName.Source, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapFieldNames, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapFieldName));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapShapefile, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapSpatialData, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Source:
					writer.Write(m_source);
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
				case MemberName.Source:
					m_source = (ExpressionInfo)reader.ReadRIFObject();
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
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapShapefile;
		}

		internal string EvaluateSource(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapShapefileSourceExpression(this, m_map.Name);
		}

		internal string GetFileStreamName(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string url)
		{
			string text = null;
			OnDemandMetadata odpMetadata = renderingContext.OdpContext.OdpMetadata;
			if (odpMetadata.TryGetShapefile(url, out ShapefileInfo shapefileInfo))
			{
				if (shapefileInfo.ErrorOccurred)
				{
					return null;
				}
				text = shapefileInfo.StreamName;
			}
			else
			{
				if (!GetFileData(renderingContext, url, out byte[] data) || data == null)
				{
					shapefileInfo = new ShapefileInfo(null);
					shapefileInfo.ErrorOccurred = true;
				}
				else
				{
					text = StoreShapefileInChunk(renderingContext, data);
					shapefileInfo = new ShapefileInfo(text);
				}
				odpMetadata.AddShapefile(url, shapefileInfo);
			}
			return text;
		}

		private bool GetFileData(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, string url, out byte[] data)
		{
			data = null;
			string mimeType = null;
			try
			{
				if (!renderingContext.OdpContext.TopLevelContext.ReportContext.IsSupportedProtocol(url, protocolRestriction: true, out bool _))
				{
					renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsUnsupportedProtocol, Severity.Error, m_map.ObjectType, m_map.Name, "Source", url, "http://, https://, ftp://, file:, mailto:, or news:");
					return false;
				}
				renderingContext.OdpContext.GetResource(url, out data, out mimeType, out bool _);
				return data != null;
			}
			catch (Exception ex)
			{
				renderingContext.OdpContext.ErrorContext.Register(ProcessingErrorCode.rsMapInvalidShapefileReference, Severity.Warning, m_map.ObjectType, m_map.Name, url, ex.Message);
				return false;
			}
		}

		private string StoreShapefileInChunk(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, byte[] data)
		{
			string text = Guid.NewGuid().ToString("N");
			using (Stream stream = renderingContext.OdpContext.ChunkFactory.CreateChunk(text, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Shapefile, null))
			{
				stream.Write(data, 0, data.Length);
				return text;
			}
		}
	}
}
