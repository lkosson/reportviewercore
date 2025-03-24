using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class AssemblyLocationResolver : MarshalByRefObject
	{
		private readonly bool m_fullLoad;

		public static AssemblyLocationResolver CreateResolver(AppDomain tempAppDomain)
		{
#if NETSTANDARD2_0_OR_GREATER
			return new AssemblyLocationResolver(fullLoad: true);
#else
			if (tempAppDomain == null)
			{
				return new AssemblyLocationResolver(fullLoad: true);
			}
			Type typeFromHandle = typeof(AssemblyLocationResolver);
			return (AssemblyLocationResolver)tempAppDomain.CreateInstanceFromAndUnwrap(typeFromHandle.Assembly.Location, typeFromHandle.FullName);
#endif
		}

		public string[] LoadAssemblyAndResolveLocation(string name)
		{
			var locations = new List<string>();
			LoadAssemblyAndResolveLocation(new AssemblyName(name), locations);
			return locations.ToArray();
		}

		private void LoadAssemblyAndResolveLocation(AssemblyName name, List<string> locations)
		{
			var assembly = Assembly.Load(name);
			if (locations.Contains(assembly.Location)) return;
			locations.Add(assembly.Location);
			foreach (var referencedAssembly in assembly.GetReferencedAssemblies())
			{
				LoadAssemblyAndResolveLocation(referencedAssembly, locations);
			}
		}

		public AssemblyLocationResolver()
			: this(fullLoad: false)
		{
		}

		private AssemblyLocationResolver(bool fullLoad)
		{
			m_fullLoad = fullLoad;
		}
	}
}
