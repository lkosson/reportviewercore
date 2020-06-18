using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class RenderingInfoManager
	{
		private const string RenderingInfoChunkPrefix = "RenderingInfo_";

		private RenderingInfoRoot m_renderingInfoRoot;

		private string m_chunkName;

		internal Hashtable RenderingInfo => RenderingInfoRoot.RenderingInfo;

		internal Hashtable SharedRenderingInfo => RenderingInfoRoot.SharedRenderingInfo;

		internal Hashtable PageSectionRenderingInfo => RenderingInfoRoot.PageSectionRenderingInfo;

		internal PaginationInfo PaginationInfo
		{
			get
			{
				return RenderingInfoRoot.PaginationInfo;
			}
			set
			{
				RenderingInfoRoot.PaginationInfo = value;
			}
		}

		private RenderingInfoRoot RenderingInfoRoot
		{
			get
			{
				if (m_renderingInfoRoot == null)
				{
					m_renderingInfoRoot = new RenderingInfoRoot();
				}
				return m_renderingInfoRoot;
			}
		}

		internal RenderingInfoManager(string rendererID, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback, bool retrieveRenderingInfo)
		{
			m_chunkName = "RenderingInfo_" + rendererID;
			if (retrieveRenderingInfo)
			{
				m_renderingInfoRoot = Deserialize(getChunkCallback);
			}
			else
			{
				m_renderingInfoRoot = null;
			}
		}

		internal void Save(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CreateReportChunk createChunkCallback)
		{
			if (m_renderingInfoRoot != null)
			{
				Serialize(m_renderingInfoRoot, createChunkCallback);
			}
		}

		private RenderingInfoRoot Deserialize(Microsoft.ReportingServices.ReportProcessing.ReportProcessing.GetReportChunk getChunkCallback)
		{
			Stream stream = null;
			try
			{
				stream = getChunkCallback(m_chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Other, out string _);
				RenderingInfoRoot result = null;
				if (stream != null)
				{
					BinaryFormatter bFormatter = new BinaryFormatter();
					RevertImpersonationContext.Run(delegate
					{
						result = (RenderingInfoRoot)bFormatter.Deserialize(stream);
					});
				}
				return result;
			}
			catch (SerializationException)
			{
				return null;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		private void Serialize(RenderingInfoRoot renderingInfoRoot, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.CreateReportChunk createChunkCallback)
		{
			Stream stream = null;
			try
			{
				stream = createChunkCallback(m_chunkName, Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Other, null);
				if (stream != null)
				{
					new BinaryFormatter().Serialize(stream, renderingInfoRoot);
				}
			}
			finally
			{
				stream?.Close();
			}
		}
	}
}
