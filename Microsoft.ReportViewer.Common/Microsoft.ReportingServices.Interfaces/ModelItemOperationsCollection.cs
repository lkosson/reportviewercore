using System;
using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public sealed class ModelItemOperationsCollection : CollectionBase
	{
		public ModelItemOperation this[int index] => (ModelItemOperation)base.InnerList[index];

		public int Add(ModelItemOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
