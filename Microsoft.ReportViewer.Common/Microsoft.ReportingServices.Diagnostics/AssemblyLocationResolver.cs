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
			return (AssemblyLocationResolver)tempAppDomain.CreateInstanceFromAndUnwrap(typeFromHandle.Assembly.CodeBase, typeFromHandle.FullName);
		}

		public string LoadAssemblyAndResolveLocation(string name)
		{
			if (m_fullLoad)
			{
				return Assembly.Load(name).Location;
			}
			return Assembly.ReflectionOnlyLoad(name).Location;
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
