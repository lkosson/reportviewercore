using Microsoft.ReportingServices.Interfaces;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IDataSegmentRenderer
	{
		void RenderSegment(Report report, Stream dataSegmentQuery, CreateAndRegisterStream createAndRegisterStream);

		void ExecuteQueries(Stream executeQueriesRequest, ExecuteQueriesContext executeQueriesContext);
	}
}
