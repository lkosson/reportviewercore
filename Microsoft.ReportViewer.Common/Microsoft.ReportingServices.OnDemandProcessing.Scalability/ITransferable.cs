namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface ITransferable
	{
		void TransferTo(IScalabilityCache scaleCache);
	}
}
