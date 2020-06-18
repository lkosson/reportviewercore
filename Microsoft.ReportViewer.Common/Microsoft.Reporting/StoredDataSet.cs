using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.Reporting
{
	[Serializable]
	internal class StoredDataSet
	{
		public byte[] Definition
		{
			get;
			private set;
		}

		public DataSetPublishingResult PublishingResult
		{
			get;
			private set;
		}

		public StoredDataSet(byte[] dataSetDefinition, DataSetPublishingResult result)
		{
			Definition = dataSetDefinition;
			PublishingResult = result;
		}
	}
}
