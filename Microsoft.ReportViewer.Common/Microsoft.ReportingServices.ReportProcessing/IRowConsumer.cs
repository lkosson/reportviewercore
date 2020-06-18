using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal interface IRowConsumer
	{
		string ReportDataSetName
		{
			get;
		}

		void SetProcessingDataReader(IProcessingDataReader dataReader);

		void NextRow(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row);
	}
}
