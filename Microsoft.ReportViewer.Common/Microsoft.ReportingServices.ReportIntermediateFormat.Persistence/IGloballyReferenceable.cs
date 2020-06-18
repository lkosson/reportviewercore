namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IGloballyReferenceable : IGlobalIDOwner
	{
		ObjectType GetObjectType();
	}
}
