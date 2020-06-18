using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class StyleAttributeHashtable : Hashtable
	{
		internal AttributeInfo this[string index] => (AttributeInfo)base[index];

		internal StyleAttributeHashtable()
		{
		}

		internal StyleAttributeHashtable(int capacity)
			: base(capacity)
		{
		}

		private StyleAttributeHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
