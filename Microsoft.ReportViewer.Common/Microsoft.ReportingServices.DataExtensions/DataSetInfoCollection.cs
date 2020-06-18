using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSetInfoCollection
	{
		private Dictionary<string, DataSetInfo> m_dataSetsByName;

		private Dictionary<Guid, DataSetInfo> m_dataSetsByID;

		public int Count => m_dataSetsByID.Count;

		public DataSetInfoCollection()
		{
			m_dataSetsByID = new Dictionary<Guid, DataSetInfo>();
			m_dataSetsByName = new Dictionary<string, DataSetInfo>(StringComparer.Ordinal);
		}

		public IEnumerator<DataSetInfo> GetEnumerator()
		{
			return m_dataSetsByID.Values.GetEnumerator();
		}

		public void Add(DataSetInfo dataSet)
		{
			m_dataSetsByID.Add(dataSet.ID, dataSet);
			if (!m_dataSetsByName.ContainsKey(dataSet.DataSetName))
			{
				m_dataSetsByName.Add(dataSet.DataSetName, dataSet);
			}
		}

		public DataSetInfo GetByName(string name)
		{
			DataSetInfo value = null;
			if (m_dataSetsByName != null)
			{
				m_dataSetsByName.TryGetValue(name, out value);
			}
			return value;
		}

		public void CombineOnSetDataSets(DataSetInfoCollection newDataSets)
		{
			if (newDataSets == null)
			{
				return;
			}
			foreach (DataSetInfo newDataSet in newDataSets)
			{
				DataSetInfo byName = GetByName(newDataSet.DataSetName);
				if (byName == null)
				{
					throw new DataSetNotFoundException(newDataSet.DataSetName.MarkAsPrivate());
				}
				byName.AbsolutePath = newDataSet.AbsolutePath;
				byName.LinkedSharedDataSetID = Guid.Empty;
			}
		}

		public DataSetInfoCollection CombineOnSetDefinition(DataSetInfoCollection newDataSets)
		{
			DataSetInfoCollection dataSetInfoCollection = new DataSetInfoCollection();
			if (newDataSets == null)
			{
				return dataSetInfoCollection;
			}
			foreach (DataSetInfo newDataSet in newDataSets)
			{
				DataSetInfo byName = GetByName(newDataSet.DataSetName);
				if (byName == null)
				{
					dataSetInfoCollection.Add(newDataSet);
					continue;
				}
				byName.ID = newDataSet.ID;
				dataSetInfoCollection.Add(byName);
			}
			return dataSetInfoCollection;
		}
	}
}
