namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface IReferenceable
	{
		int ID
		{
			get;
		}

		ObjectType GetObjectType();
	}
}
