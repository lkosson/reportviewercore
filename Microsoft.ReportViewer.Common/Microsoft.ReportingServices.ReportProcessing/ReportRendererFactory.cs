using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using System;

namespace Microsoft.ReportingServices.ReportProcessing
{
	internal sealed class ReportRendererFactory
	{
		private ReportRendererFactory()
		{
		}

		internal static IRenderingExtension GetNewRenderer(string format, IExtensionFactory extFactory)
		{
			IRenderingExtension renderingExtension = null;
			try
			{
				return (IRenderingExtension)extFactory.GetNewRendererExtensionClass(format);
			}
			catch (Exception e)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(e))
				{
					throw;
				}
				throw new ReportProcessingException(ErrorCode.rsRenderingExtensionNotFound);
			}
		}
	}
}
