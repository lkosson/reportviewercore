using System;

namespace Microsoft.ReportingServices.ReportProcessing.ReportObjectModel
{
	internal abstract class RenderFormatImplBase : MarshalByRefObject
	{
		internal abstract string Name
		{
			get;
		}

		internal abstract bool IsInteractive
		{
			get;
		}

		internal abstract ReadOnlyNameValueCollection DeviceInfo
		{
			get;
		}
	}
}
