using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapShapefileInstance : MapSpatialDataInstance
	{
		private MapShapefile m_defObject;

		private string m_source;

		private Stream m_stream;

		private Stream m_dbfStream;

		public string Source
		{
			get
			{
				if (m_source == null)
				{
					m_source = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile)m_defObject.MapSpatialDataDef).EvaluateSource(ReportScopeInstance, m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return m_source;
			}
		}

		public Stream Stream
		{
			get
			{
				if (m_stream == null)
				{
					m_stream = GetFileStream(Source);
				}
				return m_stream;
			}
		}

		public Stream DBFStream
		{
			get
			{
				if (m_dbfStream == null)
				{
					m_dbfStream = GetFileStream(GetDBFUrl());
				}
				return m_dbfStream;
			}
		}

		internal MapShapefileInstance(MapShapefile defObject)
			: base(defObject)
		{
			m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			m_source = null;
			m_stream = null;
			m_dbfStream = null;
		}

		private Stream GetFileStream(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}
			string fileStreamName = ((Microsoft.ReportingServices.ReportIntermediateFormat.MapShapefile)m_defObject.MapSpatialDataDef).GetFileStreamName(m_defObject.MapDef.RenderingContext, url);
			string mimeType;
			if (fileStreamName != null)
			{
				return m_defObject.MapDef.RenderingContext.OdpContext.ChunkFactory.GetChunk(fileStreamName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Shapefile, ChunkMode.Open, out mimeType);
			}
			return null;
		}

		private string GetDBFUrl()
		{
			if (Source.EndsWith(".shp", StringComparison.OrdinalIgnoreCase))
			{
				return Source.Substring(0, Source.Length - 3) + "dbf";
			}
			return null;
		}
	}
}
