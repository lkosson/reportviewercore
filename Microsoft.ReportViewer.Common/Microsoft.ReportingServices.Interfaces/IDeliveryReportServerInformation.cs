namespace Microsoft.ReportingServices.Interfaces
{
	public interface IDeliveryReportServerInformation
	{
		Extension[] RenderingExtension
		{
			get;
		}

		Setting[] ServerSettings
		{
			get;
		}
	}
}
