using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class ModelMetadata
	{
		public string VersionRequested
		{
			get;
			set;
		}

		public string PerspectiveName
		{
			get;
			set;
		}

		public long? TotalTimeDataRetrieval
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TotalTimeDataRetrievalSpecified => TotalTimeDataRetrieval.HasValue;

		public long? ByteCount
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ByteCountSpecified => ByteCount.HasValue;

		public long? TimeDataModelParsing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeDataModelParsingSpecified => TimeDataModelParsing.HasValue;

		internal ModelMetadata()
		{
		}
	}
}
