using Microsoft.ReportingServices.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class RuntimeDataSetInfoCollection
	{
		private Dictionary<Guid, DataSetInfo> m_collectionByID;

		private Dictionary<string, DataSetInfoCollection> m_collectionByReport;

		public byte[] Serialize()
		{
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream();
				new BinaryFormatter().Serialize(memoryStream, this);
				return memoryStream.ToArray();
			}
			finally
			{
				memoryStream?.Close();
			}
		}

		public static RuntimeDataSetInfoCollection Deserialize(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			MemoryStream memoryStream = null;
			try
			{
				memoryStream = new MemoryStream(data, writable: false);
				return (RuntimeDataSetInfoCollection)new BinaryFormatter().Deserialize(memoryStream);
			}
			finally
			{
				memoryStream?.Close();
			}
		}

		internal DataSetInfo GetByID(Guid ID)
		{
			DataSetInfo value = null;
			if (m_collectionByID != null)
			{
				m_collectionByID.TryGetValue(ID, out value);
			}
			return value;
		}

		internal DataSetInfo GetByName(string name, ICatalogItemContext item)
		{
			DataSetInfo result = null;
			if (m_collectionByReport != null)
			{
				DataSetInfoCollection value = null;
				if (m_collectionByReport.TryGetValue(item.StableItemPath, out value))
				{
					result = value.GetByName(name);
				}
			}
			return result;
		}

		internal void Add(DataSetInfo dataSet, ICatalogItemContext report)
		{
			if (Guid.Empty == dataSet.ID)
			{
				AddToCollectionByReport(dataSet, report);
			}
			else
			{
				AddToCollectionByID(dataSet);
			}
		}

		private void AddToCollectionByReport(DataSetInfo dataSet, ICatalogItemContext report)
		{
			DataSetInfoCollection value = null;
			if (m_collectionByReport == null)
			{
				m_collectionByReport = new Dictionary<string, DataSetInfoCollection>(StringComparer.Ordinal);
			}
			else
			{
				m_collectionByReport.TryGetValue(report.StableItemPath, out value);
			}
			if (value == null)
			{
				value = new DataSetInfoCollection();
				m_collectionByReport.Add(report.StableItemPath, value);
			}
			value.Add(dataSet);
		}

		private void AddToCollectionByID(DataSetInfo dataSet)
		{
			if (m_collectionByID == null)
			{
				m_collectionByID = new Dictionary<Guid, DataSetInfo>();
			}
			else if (m_collectionByID.ContainsKey(dataSet.ID))
			{
				return;
			}
			m_collectionByID.Add(dataSet.ID, dataSet);
		}
	}
}
