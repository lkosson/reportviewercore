using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class DataShape
	{
		public string ID
		{
			get;
			set;
		}

		public long? TimeDataRetrieval
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeDataRetrievalSpecified => TimeDataRetrieval.HasValue;

		public long? TimeQueryTranslation
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeQueryTranslationSpecified => TimeQueryTranslation.HasValue;

		public long? TimeProcessing
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeProcessingSpecified => TimeProcessing.HasValue;

		public long? TimeRendering
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool TimeRenderingSpecified => TimeRendering.HasValue;

		public ScaleTimeCategory ScalabilityTime
		{
			get;
			set;
		}

		public EstimatedMemoryUsageKBCategory EstimatedMemoryUsageKB
		{
			get;
			set;
		}

		public List<Connection> Connections
		{
			get;
			set;
		}

		public string QueryPattern
		{
			get;
			set;
		}

		public string QueryPatternReasons
		{
			get;
			set;
		}

		internal DataShape()
		{
		}
	}
}
