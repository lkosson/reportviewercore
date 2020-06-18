using Microsoft.ReportingServices.DataExtensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Reporting
{
	[Serializable]
	internal class LocalCatalogTempDB : IDisposable
	{
		[Serializable]
		protected class ReportID
		{
			public string Name
			{
				get;
				private set;
			}

			public DefinitionSource Source
			{
				get;
				private set;
			}

			public Assembly EmbeddedResourceAssembly
			{
				get;
				private set;
			}

			public ReportID(string directDefinitionReportName)
				: this(directDefinitionReportName, DefinitionSource.Direct, null)
			{
			}

			public ReportID(PreviewItemContext itemContext)
				: this(itemContext.PreviewStorePath, itemContext.DefinitionSource, itemContext.EmbeddedResourceAssembly)
			{
			}

			private ReportID(string name, DefinitionSource source, Assembly embeddedResourceAssembly)
			{
				Name = name;
				Source = source;
				EmbeddedResourceAssembly = embeddedResourceAssembly;
			}

			public override bool Equals(object obj)
			{
				ReportID reportID = obj as ReportID;
				if (reportID == null)
				{
					return false;
				}
				if (reportID.Source == Source && string.Compare(reportID.Name, Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return reportID.EmbeddedResourceAssembly == EmbeddedResourceAssembly;
				}
				return false;
			}

			public override int GetHashCode()
			{
				if (Name == null)
				{
					return 0;
				}
				return Name.GetHashCode();
			}
		}

		private Dictionary<ReportID, StoredReport> m_compiledReports = new Dictionary<ReportID, StoredReport>();

		private readonly Dictionary<string, StoredDataSet> m_compiledDataSets = new Dictionary<string, StoredDataSet>();

		public void Dispose()
		{
			foreach (StoredReport value in m_compiledReports.Values)
			{
				value.Dispose();
			}
			m_compiledReports.Clear();
			m_compiledDataSets.Clear();
			GC.SuppressFinalize(this);
		}

		public StoredDataSet GetCompiledDataSet(DataSetInfo dataSetInfo)
		{
			if (dataSetInfo == null)
			{
				throw new ArgumentNullException("dataSetInfo");
			}
			m_compiledDataSets.TryGetValue(dataSetInfo.AbsolutePath, out StoredDataSet value);
			return value;
		}

		public void SetCompiledDataSet(DataSetInfo dataSetInfo, StoredDataSet storedDataSet)
		{
			if (storedDataSet == null)
			{
				throw new ArgumentNullException("storedDataSet");
			}
			m_compiledDataSets[dataSetInfo.AbsolutePath] = storedDataSet;
		}

		public StoredReport GetCompiledReport(PreviewItemContext context)
		{
			ReportID key = new ReportID(context);
			m_compiledReports.TryGetValue(key, out StoredReport value);
			return value;
		}

		public void SetCompiledReport(PreviewItemContext context, StoredReport storedReport)
		{
			ReportID key = new ReportID(context);
			m_compiledReports[key] = storedReport;
		}
	}
}
