using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

#if NETSTANDARD2_0_OR_GREATER
namespace System.Runtime.Loader
{
	class AssemblyLoadContext
	{
		public static AssemblyLoadContext Default => new AssemblyLoadContext();

		internal Assembly LoadFromStream(MemoryStream stream)
		{
			return Assembly.Load(stream.ToArray());
		}
	}
}
#endif