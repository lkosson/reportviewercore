namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class RdlCollection<T> : RdlCollectionBase<T>
	{
		public RdlCollection()
		{
		}

		public RdlCollection(IContainedObject parent)
			: base(parent)
		{
		}
	}
}
