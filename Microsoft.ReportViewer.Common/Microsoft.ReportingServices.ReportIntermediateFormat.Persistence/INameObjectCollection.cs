namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal interface INameObjectCollection
	{
		int Count
		{
			get;
		}

		void Add(string key, object value);

		string GetKey(int index);

		object GetValue(int index);
	}
}
