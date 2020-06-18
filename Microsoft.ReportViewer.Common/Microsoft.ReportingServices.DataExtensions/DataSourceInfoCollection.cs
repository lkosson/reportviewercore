using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.Xml;

namespace Microsoft.ReportingServices.DataExtensions
{
	[Serializable]
	internal sealed class DataSourceInfoCollection : IPowerViewDataSourceCollection, IEnumerable
	{
		private Hashtable m_collection = new Hashtable();

		public int Count => m_collection.Count;

		public DataSourceInfoCollection()
		{
		}

		public DataSourceInfoCollection(DataSourceInfoCollection other)
		{
			RSTrace processingTracer = RSTrace.ProcessingTracer;
			processingTracer.Assert(other != null);
			processingTracer.Assert(other.m_collection != null);
			m_collection = (Hashtable)other.m_collection.Clone();
		}

		public DataSourceInfoCollection(string dataSourcesXml, IDataProtection dataProtection)
		{
			ConstructFromXml(dataSourcesXml, clientLoad: false, dataProtection);
		}

		public DataSourceInfoCollection(string dataSourcesXml, bool clientLoad, IDataProtection dataProtection)
		{
			ConstructFromXml(dataSourcesXml, clientLoad, dataProtection);
		}

		private void ConstructFromXml(string dataSourcesXml, bool clientLoad, IDataProtection dataProtection)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				XmlUtil.SafeOpenXmlDocumentString(xmlDocument, dataSourcesXml);
			}
			catch (XmlException ex)
			{
				throw new MalformedXmlException(ex);
			}
			try
			{
				foreach (XmlNode childNode in (xmlDocument.SelectSingleNode("/DataSources") ?? throw new InvalidXmlException()).ChildNodes)
				{
					DataSourceInfo dataSource = DataSourceInfo.ParseDataSourceNode(childNode, clientLoad, dataProtection);
					Add(dataSource);
				}
			}
			catch (XmlException)
			{
				throw new InvalidXmlException();
			}
		}

		public DataSourceInfo GetTheOnlyDataSource()
		{
			if (Count != 1)
			{
				throw new InternalCatalogException(string.Format(CultureInfo.CurrentCulture, "Data source collection for a standalone datasource contains {0} items, must be 1.", Count));
			}
			IEnumerator enumerator = GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					return (DataSourceInfo)enumerator.Current;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}

		public DataSourceInfoCollection CombineOnSetDefinition(DataSourceInfoCollection newDataSources)
		{
			return CombineOnSetDefinition(newDataSources, keepOriginalDataSourceId: false, overrideOriginalConnectString: true);
		}

		public DataSourceInfoCollection CombineOnSetDefinitionWithoutSideEffects(DataSourceInfoCollection newDataSources)
		{
			return CombineOnSetDefinition(newDataSources, keepOriginalDataSourceId: false, overrideOriginalConnectString: false);
		}

		public DataSourceInfoCollection CombineOnSetDefinitionKeepOriginalDataSourceId(DataSourceInfoCollection newDataSources)
		{
			return CombineOnSetDefinition(newDataSources, keepOriginalDataSourceId: true, overrideOriginalConnectString: true);
		}

		private DataSourceInfoCollection CombineOnSetDefinition(DataSourceInfoCollection newDataSources, bool keepOriginalDataSourceId, bool overrideOriginalConnectString)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo newDataSource in newDataSources)
			{
				DataSourceInfo byOriginalName = GetByOriginalName(newDataSource.OriginalName);
				if (byOriginalName == null)
				{
					dataSourceInfoCollection.Add(newDataSource);
					continue;
				}
				if (!keepOriginalDataSourceId)
				{
					byOriginalName.ID = newDataSource.ID;
				}
				if (overrideOriginalConnectString)
				{
					byOriginalName.SetOriginalConnectionString(newDataSource.OriginalConnectionStringEncrypted);
					byOriginalName.SetOriginalConnectStringExpressionBased(newDataSource.OriginalConnectStringExpressionBased);
				}
				dataSourceInfoCollection.Add(byOriginalName);
			}
			return dataSourceInfoCollection;
		}

		public DataSourceInfoCollection CombineOnSetDataSources(DataSourceInfoCollection newDataSources)
		{
			DataSourceInfoCollection dataSourceInfoCollection = new DataSourceInfoCollection();
			foreach (DataSourceInfo newDataSource in newDataSources)
			{
				DataSourceInfo byOriginalName = GetByOriginalName(newDataSource.OriginalName);
				if (byOriginalName == null)
				{
					throw new DataSourceNotFoundException(newDataSource.OriginalName);
				}
				newDataSource.ID = byOriginalName.ID;
				newDataSource.SetOriginalConnectionString(byOriginalName.OriginalConnectionStringEncrypted);
				newDataSource.SetOriginalConnectStringExpressionBased(byOriginalName.OriginalConnectStringExpressionBased);
				dataSourceInfoCollection.Add(newDataSource);
			}
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DataSourceInfo dataSourceInfo2 = (DataSourceInfo)enumerator.Current;
					if (newDataSources.GetByOriginalName(dataSourceInfo2.OriginalName) == null)
					{
						dataSourceInfoCollection.Add(dataSourceInfo2);
					}
				}
				return dataSourceInfoCollection;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public bool TryGetCachedDataSourceId(string dataSourceName, out Guid dataSourceId)
		{
			dataSourceId = Guid.Empty;
			DataSourceInfo byOriginalName = GetByOriginalName(dataSourceName);
			if (byOriginalName != null)
			{
				dataSourceId = byOriginalName.ID;
				return true;
			}
			return false;
		}

		public void Add(DataSourceInfo dataSource)
		{
			if (dataSource.OriginalName == null)
			{
				RSTrace.ProcessingTracer.Assert(m_collection.Count == 0, "Adding more than one data source with null original name");
				m_collection.Add("", dataSource);
			}
			else if (!m_collection.ContainsKey(dataSource.OriginalName))
			{
				m_collection.Add(dataSource.OriginalName, dataSource);
			}
		}

		public DataSourceInfo GetByOriginalName(string name)
		{
			return (DataSourceInfo)m_collection[name];
		}

		public IEnumerator GetEnumerator()
		{
			return m_collection.Values.GetEnumerator();
		}

		public bool GoodForDataCaching()
		{
			foreach (DataSourceInfo value in m_collection.Values)
			{
				if (value.CredentialsRetrieval == DataSourceInfo.CredentialsRetrievalOption.Prompt)
				{
					return false;
				}
				if (value.HasConnectionStringUseridReference)
				{
					return false;
				}
			}
			return true;
		}

		public bool HasConnectionStringUseridReference()
		{
			foreach (DataSourceInfo value in m_collection.Values)
			{
				if (value.HasConnectionStringUseridReference)
				{
					return true;
				}
			}
			return false;
		}

		public void AddOrUpdate(string key, DataSourceInfo dsInfo)
		{
			RSTrace.ProcessingTracer.Assert(key == ((dsInfo.OriginalName != null) ? dsInfo.OriginalName : string.Empty), "DataSourceInfo.AddOrUpdate: (dsInfo.OriginalName != null ? dsInfo.OriginalName : string.Empty)");
			if (m_collection.ContainsKey(key))
			{
				m_collection.Remove(key);
			}
			Add(dsInfo);
		}

		public DataSourceInfo GetDataSourceFromKey(string key)
		{
			return GetByOriginalName(key);
		}
	}
}
