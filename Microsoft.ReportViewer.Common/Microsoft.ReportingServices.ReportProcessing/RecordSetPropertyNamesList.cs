using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class RecordSetPropertyNamesList : ArrayList
	{
		internal new RecordSetPropertyNames this[int index] => (RecordSetPropertyNames)base[index];

		internal RecordSetPropertyNamesList()
		{
		}

		internal RecordSetPropertyNamesList(int capacity)
			: base(capacity)
		{
		}

		internal StringList GetPropertyNames(int aliasIndex)
		{
			if (aliasIndex >= 0 && aliasIndex < Count)
			{
				return this[aliasIndex].PropertyNames;
			}
			return null;
		}

		internal string GetPropertyName(int aliasIndex, int propertyIndex)
		{
			StringList propertyNames = GetPropertyNames(aliasIndex);
			if (propertyNames != null && propertyIndex >= 0 && propertyIndex < propertyNames.Count)
			{
				return propertyNames[propertyIndex];
			}
			return null;
		}
	}
}
