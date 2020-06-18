using System;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IMapMapper : IDVMappingLayer, IDisposable
	{
		void RenderMap();
	}
}
