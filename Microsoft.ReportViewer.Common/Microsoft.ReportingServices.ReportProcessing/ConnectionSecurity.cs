namespace Microsoft.ReportingServices.ReportProcessing
{
	internal enum ConnectionSecurity
	{
		UseIntegratedSecurity,
		ImpersonateWindowsUser,
		UseDataSourceCredentials,
		None,
		ImpersonateServiceAccount
	}
}
