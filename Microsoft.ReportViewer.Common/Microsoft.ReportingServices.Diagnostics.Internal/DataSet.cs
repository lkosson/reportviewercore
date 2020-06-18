using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class DataSet
	{
		public string Name
		{
			get;
			set;
		}

		public string CommandText
		{
			get;
			set;
		}

		public List<QueryParameter> QueryParameters
		{
			get;
			set;
		}

		public long? RowsRead
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool RowsReadSpecified => RowsRead.HasValue;

		public long? TotalTimeDataRetrieval
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TotalTimeDataRetrievalSpecified => TotalTimeDataRetrieval.HasValue;

		public long? QueryPrepareAndExecutionTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool QueryPrepareAndExecutionTimeSpecified => QueryPrepareAndExecutionTime.HasValue;

		public long? ExecuteReaderTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ExecuteReaderTimeSpecified => ExecuteReaderTime.HasValue;

		public long? DataReaderMappingTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool DataReaderMappingTimeSpecified => DataReaderMappingTime.HasValue;

		public long? DisposeDataReaderTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool DisposeDataReaderTimeSpecified => DisposeDataReaderTime.HasValue;

		public string CancelCommandTime
		{
			get;
			set;
		}

		internal DataSet()
		{
		}
	}
}
