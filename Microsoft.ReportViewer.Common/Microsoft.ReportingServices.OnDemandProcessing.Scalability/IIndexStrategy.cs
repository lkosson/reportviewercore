namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal interface IIndexStrategy
	{
		ReferenceID MaxId
		{
			get;
		}

		ReferenceID GenerateId(ReferenceID tempId);

		ReferenceID GenerateTempId();

		long Retrieve(ReferenceID id);

		void Update(ReferenceID id, long value);

		void Close();
	}
}
