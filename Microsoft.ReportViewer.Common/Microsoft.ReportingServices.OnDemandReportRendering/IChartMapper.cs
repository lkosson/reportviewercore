using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IChartMapper : IDVMappingLayer, IDisposable
	{
		void RenderChart();
	}
}
