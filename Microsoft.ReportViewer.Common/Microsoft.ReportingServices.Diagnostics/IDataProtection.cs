namespace Microsoft.ReportingServices.Diagnostics
{
	internal interface IDataProtection
	{
		byte[] ProtectData(string unprotectedData, string tag);

		string UnprotectDataToString(byte[] protectedData, string tag);
	}
}
