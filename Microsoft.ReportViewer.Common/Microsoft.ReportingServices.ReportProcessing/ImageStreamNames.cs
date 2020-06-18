using System;
using System.Collections;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ImageStreamNames : Hashtable
	{
		internal ImageInfo this[string url]
		{
			get
			{
				return (ImageInfo)base[url];
			}
			set
			{
				base[url] = value;
			}
		}

		internal ImageStreamNames()
		{
		}

		internal ImageStreamNames(int capacity)
			: base(capacity)
		{
		}
	}
}
