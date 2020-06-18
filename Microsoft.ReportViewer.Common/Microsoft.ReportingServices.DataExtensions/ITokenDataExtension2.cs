using Microsoft.ReportingServices.DataProcessing;

namespace Microsoft.ReportingServices.DataExtensions
{
	internal interface ITokenDataExtension2 : ITokenDataExtension
	{
		bool UseTokenAuthentication
		{
			get;
		}
	}
}
