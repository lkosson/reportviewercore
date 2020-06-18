using System;
using System.Collections;

namespace Microsoft.ReportingServices.Interfaces
{
	[Serializable]
	public sealed class AceCollection : CollectionBase
	{
		public AceStruct this[int index] => (AceStruct)base.InnerList[index];

		public int Add(AceStruct ace)
		{
			return base.InnerList.Add(ace);
		}
	}
}
