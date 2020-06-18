using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.Diagnostics.Internal
{
	public sealed class Connection
	{
		public long? ConnectionOpenTime
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ConnectionOpenTimeSpecified => ConnectionOpenTime.HasValue;

		public bool? ConnectionFromPool
		{
			get;
			set;
		}

		[XmlIgnore]
		public bool ConnectionFromPoolSpecified => ConnectionFromPool.HasValue;

		public ModelMetadata ModelMetadata
		{
			get;
			set;
		}

		public DataSource DataSource
		{
			get;
			set;
		}

		public List<DataSet> DataSets
		{
			get;
			set;
		}

		internal Connection()
		{
		}
	}
}
