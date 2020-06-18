using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataSetCollection : ReportElementCollectionBase<DataSet>
	{
		private DataSet[] m_collection;

		private Microsoft.ReportingServices.ReportIntermediateFormat.Report m_reportDef;

		private RenderingContext m_rendringContext;

		public override int Count => m_reportDef.DataSetCount;

		public override DataSet this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				if (m_collection == null)
				{
					m_collection = new DataSet[Count];
				}
				if (m_collection[index] == null)
				{
					m_collection[index] = new DataSet(m_reportDef.MappingDataSetIndexToDataSet[index], m_rendringContext);
				}
				return m_collection[index];
			}
		}

		public DataSet this[string name]
		{
			get
			{
				if (m_reportDef.MappingNameToDataSet.ContainsKey(name))
				{
					return this[m_reportDef.MappingDataSetIndexToDataSet.IndexOf(m_reportDef.MappingNameToDataSet[name])];
				}
				return null;
			}
		}

		internal DataSetCollection(Microsoft.ReportingServices.ReportIntermediateFormat.Report reportDef, RenderingContext renderingContext)
		{
			m_reportDef = reportDef;
			m_rendringContext = renderingContext;
		}

		internal void SetNewContext()
		{
			if (m_collection != null)
			{
				for (int i = 0; i < m_collection.Length; i++)
				{
					m_collection[i]?.SetNewContext();
				}
			}
		}
	}
}
