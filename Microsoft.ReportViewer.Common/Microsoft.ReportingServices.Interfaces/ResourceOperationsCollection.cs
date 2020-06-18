using System;
using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public sealed class ResourceOperationsCollection : CollectionBase
	{
		public ResourceOperation this[int index] => (ResourceOperation)base.InnerList[index];

		public int Add(ResourceOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
