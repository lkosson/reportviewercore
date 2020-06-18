using System;
using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public sealed class ModelOperationsCollection : CollectionBase
	{
		public ModelOperation this[int index] => (ModelOperation)base.InnerList[index];

		public int Add(ModelOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
