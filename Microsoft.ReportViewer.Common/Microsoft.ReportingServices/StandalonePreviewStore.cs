using Microsoft.Reporting;
using Microsoft.ReportingServices.DataExtensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices
{
	[Serializable]
	internal sealed class StandalonePreviewStore : ILocalCatalog
	{
		private Dictionary<string, byte[]> m_directReportDefinitions = new Dictionary<string, byte[]>();

		public byte[] GetResource(string resourcePath, out string mimeType)
		{
			mimeType = null;
			return null;
		}

		public DataSetInfo GetDataSet(string dataSetPath)
		{
			return null;
		}

		public DataSourceInfo GetDataSource(string dataSourcePath)
		{
			return null;
		}

		public byte[] GetReportDefinition(PreviewItemContext itemContext)
		{
			byte[] value = null;
			Exception ex = null;
			try
			{
				if (itemContext.DefinitionSource == DefinitionSource.Direct)
				{
					m_directReportDefinitions.TryGetValue(itemContext.PreviewStorePath, out value);
				}
				else
				{
					Stream stream = null;
					try
					{
						if (itemContext.DefinitionSource == DefinitionSource.File)
						{
							stream = File.OpenRead(itemContext.PreviewStorePath);
						}
						else if (itemContext.DefinitionSource == DefinitionSource.EmbeddedResource)
						{
							stream = itemContext.EmbeddedResourceAssembly.GetManifestResourceStream(itemContext.PreviewStorePath);
						}
						value = new byte[stream.Length];
						stream.Read(value, 0, (int)stream.Length);
					}
					finally
					{
						stream?.Close();
					}
				}
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
			if (value == null || ex != null)
			{
				throw new ApplicationException(ProcessingStrings.MissingDefinition(itemContext.ItemName), ex);
			}
			return value;
		}

		public void SetReportDefinition(string reportName, byte[] reportBytes)
		{
			m_directReportDefinitions[reportName] = reportBytes;
		}

		public bool HasDirectReportDefinition(string reportName)
		{
			return m_directReportDefinitions.ContainsKey(reportName);
		}

		public void GetReportDataSourceCredentials(PreviewItemContext itemContext, string dataSourceName, out string userName, out string password)
		{
			userName = (password = null);
		}

		public string GetReportDataFileInfo(PreviewItemContext itemContext, out bool isOutOfDate)
		{
			isOutOfDate = false;
			return null;
		}

		public void UpdateReportDataFileStatus(PreviewItemContext itemContext, bool isOutOfDate)
		{
			throw new NotSupportedException();
		}
	}
}
