using System;
using System.Reflection;

namespace Microsoft.ReportingServices.Diagnostics
{
	internal sealed class AssemblyLocationResolver : MarshalByRefObject
	{
		private readonly bool m_fullLoad;

		public static AssemblyLocationResolver CreateResolver(AppDomain tempAppDomain)
		{
			if (tempAppDomain == null)
			{
				return new AssemblyLocationResolver(fullLoad: true);
			}
			Type typeFromHandle = typeof(AssemblyLocationResolver);
			return (AssemblyLocationResolver)tempAppDomain.CreateInstanceFromAndUnwrap(typeFromHandle.Assembly.Location, typeFromHandle.FullName);
		}

		public string LoadAssemblyAndResolveLocation(string name)
		{
			return Assembly.Load(name).Location;
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
