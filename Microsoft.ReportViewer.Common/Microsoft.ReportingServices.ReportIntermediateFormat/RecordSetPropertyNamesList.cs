using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal sealed class RecordSetPropertyNamesList : ArrayList
	{
		internal new RecordSetPropertyNames this[int index] => (RecordSetPropertyNames)base[index];

		public RecordSetPropertyNamesList()
		{
		}

		internal RecordSetPropertyNamesList(int capacity)
			: base(capacity)
		{
		}

		internal List<string> GetPropertyNames(int aliasIndex)
		{
			if (aliasIndex >= 0 && aliasIndex < Count)
			{
				return this[aliasIndex].PropertyNames;
			}
			return null;
		}

		internal string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			List<string> propertyNames = GetPropertyNames(aliasIndex);
			if (propertyNames != null && propertyIndex >= 0 && propertyIndex < propertyNames.Count)
			{
				return propertyNames[propertyIndex];
			}
			return null;
		}
	}
}
