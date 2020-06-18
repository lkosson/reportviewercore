namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IRIFObjectCreator
	{
		IPersistable CreateRIFObject(ObjectType objectType, ref IntermediateFormatReader context);
	}
}
