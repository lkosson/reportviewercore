using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.IO;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetDefinition
	{
		private string m_description;

		private Guid m_sharedDataSourceReferenceId = Guid.Empty;

		private ParameterInfoCollection m_dataSetParameters;

		private DataSetCore m_dataSetCore;

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public Guid SharedDataSourceReferenceId
		{
			get
			{
				return m_sharedDataSourceReferenceId;
			}
			set
			{
				m_sharedDataSourceReferenceId = value;
			}
		}

		public ParameterInfoCollection DataSetParameters => m_dataSetParameters;

		public DataSetCore DataSetCore => m_dataSetCore;

		public DataSetDefinition(DataSetCore dataSetCore, string description, DataSourceInfo dataSourceInfo, ParameterInfoCollection dataSetParameters)
		{
			m_dataSetCore = dataSetCore;
			m_description = description;
			m_dataSetParameters = dataSetParameters;
			if (dataSourceInfo != null && dataSourceInfo.IsReference)
			{
				m_sharedDataSourceReferenceId = dataSourceInfo.ID;
			}
		}

		public DataSetDefinition(IChunkFactory getCompiledDefinition)
		{
			Global.Tracer.Assert(getCompiledDefinition != null, "Shared dataset definition chunk factory does not exist");
			string mimeType;
			Stream chunk = getCompiledDefinition.GetChunk("CompiledDefinition", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.CompiledDefinition, ChunkMode.Open, out mimeType);
			Global.Tracer.Assert(chunk != null, "Shared dataset definition stream does not exist");
			try
			{
				m_dataSetCore = (DataSetCore)new IntermediateFormatReader(chunk, (IRIFObjectCreator)new ProcessingRIFObjectCreator(null, null), (GlobalIDOwnerCollection)null).ReadRIFObject();
			}
			finally
			{
				chunk?.Close();
			}
		}
	}
}
