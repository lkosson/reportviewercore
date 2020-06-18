using System;
using System.Collections;
using System.Runtime.Serialization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class EmbeddedImageHashtable : Hashtable
	{
		internal ImageInfo this[string index] => (ImageInfo)base[index];

		internal EmbeddedImageHashtable()
		{
		}

		internal EmbeddedImageHashtable(int capacity)
			: base(capacity)
		{
		}

		private EmbeddedImageHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
