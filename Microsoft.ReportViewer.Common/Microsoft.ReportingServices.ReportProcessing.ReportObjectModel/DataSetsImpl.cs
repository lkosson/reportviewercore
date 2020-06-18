using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal sealed class DataSetsImpl : DataSets
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		internal const string Name = "DataSets";

		internal const string FullName = "Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSets";

		public override DataSet this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingDataSetReference(key);
				}
				try
				{
					DataSet dataSet = m_collection[key] as DataSet;
					if (dataSet == null)
					{
						throw new ReportProcessingException_NonExistingDataSetReference(key);
					}
					return dataSet;
				}
				catch
				{
					throw new ReportProcessingException_NonExistingDataSetReference(key);
				}
			}
		}

		internal DataSetsImpl()
		{
		}

		internal DataSetsImpl(bool lockAdd, int size)
		{
			m_lockAdd = lockAdd;
			m_collection = new Hashtable(size);
		}

		internal void Add(Microsoft.ReportingServices.ReportProcessing.DataSet dataSetDef)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				m_collection.Add(dataSetDef.Name, new DataSetImpl(dataSetDef));
			}
			finally
			{
				if (m_lockAdd)
				{
					Monitor.Exit(m_collection);
				}
			}
		}
	}
}
