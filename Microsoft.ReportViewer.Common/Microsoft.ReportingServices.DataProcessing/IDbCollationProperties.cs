namespace Microsoft.ReportingServices.DataProcessing
{
	public interface IDbCollationProperties
	{
		bool GetCollationProperties(out string cultureName, out bool caseSensitive, out bool accentSensitive, out bool kanatypeSensitive, out bool widthSensitive);
	}
}
