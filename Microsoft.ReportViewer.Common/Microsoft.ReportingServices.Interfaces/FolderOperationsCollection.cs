using System;
using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public sealed class FolderOperationsCollection : CollectionBase
	{
		public FolderOperation this[int index] => (FolderOperation)base.InnerList[index];

		public int Add(FolderOperation operation)
		{
			return base.InnerList.Add(operation);
		}
	}
}
