using System.IO;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IDynamicImageInstance
	{
		void SetDpi(int xDpi, int yDpi);

		void SetSize(double width, double height);

		Stream GetImage(DynamicImageInstance.ImageType type, out ActionInfoWithDynamicImageMapCollection actionImageMaps);
	}
}
