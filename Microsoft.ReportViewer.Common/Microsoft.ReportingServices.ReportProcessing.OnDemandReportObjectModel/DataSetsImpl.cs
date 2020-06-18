using Microsoft.Cloud.Platform.Utils;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Collections;
using System.Threading;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	internal sealed class DataSetsImpl : DataSets
	{
		private bool m_lockAdd;

		private Hashtable m_collection;

		public override Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet this[string key]
		{
			get
			{
				if (key == null || m_collection == null)
				{
					throw new ReportProcessingException_NonExistingDataSetReference(key.MarkAsPrivate());
				}
				Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet dataSet = m_collection[key] as Microsoft.ReportingServices.ReportProcessing.ReportObjectModel.DataSet;
				if (dataSet == null)
				{
					throw new ReportProcessingException_NonExistingDataSetReference(key.MarkAsPrivate());
				}
				return dataSet;
			}
		}

		internal DataSetsImpl(int size)
		{
			m_lockAdd = (size > 1);
			m_collection = new Hashtable(size);
		}

		internal void AddOrUpdate(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSetDef, DataSetInstance dataSetInstance, DateTime reportExecutionTime)
		{
			try
			{
				if (m_lockAdd)
				{
					Monitor.Enter(m_collection);
				}
				DataSetImpl dataSetImpl = m_collection[dataSetDef.Name] as DataSetImpl;
				if (dataSetImpl == null)
				{
					m_collection.Add(dataSetDef.Name, new DataSetImpl(dataSetDef, dataSetInstance, reportExecutionTime));
				}
				else
				{
					dataSetImpl.Update(dataSetInstance, reportExecutionTime);
				}
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
