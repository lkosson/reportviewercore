using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IGaugeMapper : IDVMappingLayer, IDisposable
	{
		void RenderGaugePanel();

		void RenderDataGaugePanel();
	}
}
