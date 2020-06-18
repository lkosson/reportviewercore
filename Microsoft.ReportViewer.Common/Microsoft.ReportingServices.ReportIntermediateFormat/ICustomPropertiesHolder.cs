namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	internal interface ICustomPropertiesHolder
	{
		IInstancePath InstancePath
		{
			get;
		}

		DataValueList CustomProperties
		{
			get;
		}
	}
}
