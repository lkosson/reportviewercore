using System;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IDVMappingLayer : IDisposable
	{
		float DpiX
		{
			set;
		}

		float DpiY
		{
			set;
		}

		double? WidthOverride
		{
			set;
		}

		double? HeightOverride
		{
			set;
		}

		Stream GetImage(DynamicImageInstance.ImageType type);

		ActionInfoWithDynamicImageMapCollection GetImageMaps();

		Stream GetCoreXml();
	}
}
