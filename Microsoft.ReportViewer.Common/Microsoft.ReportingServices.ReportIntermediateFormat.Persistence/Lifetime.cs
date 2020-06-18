using Microsoft.ReportingServices.ReportProcessing;

namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal sealed class Lifetime
	{
		private readonly int m_addedVersion;

		private readonly int m_removedVersion;

		private static readonly Lifetime UnspecifiedInstance = new Lifetime(0, 0);

		public int AddedVersion => m_addedVersion;

		public bool HasAddedVersion => m_addedVersion != 0;

		public int RemovedVersion => m_removedVersion;

		public bool HasRemovedVersion => m_removedVersion != 0;

		public static Lifetime Unspecified => UnspecifiedInstance;

		private Lifetime(int addedVersion, int removedVersion)
		{
			m_addedVersion = addedVersion;
			m_removedVersion = removedVersion;
		}

		public bool IncludesVersion(int compatVersion)
		{
			if (compatVersion == 0)
			{
				return true;
			}
			bool num = m_addedVersion == 0 || m_addedVersion <= compatVersion;
			bool flag = m_removedVersion == 0 || m_removedVersion > compatVersion;
			return num && flag;
		}

		public static Lifetime AddedIn(int addedVersion)
		{
			Global.Tracer.Assert(addedVersion > 0, "Invalid addedVersion");
			return new Lifetime(addedVersion, 0);
		}

		public static Lifetime RemovedIn(int removedVersion)
		{
			Global.Tracer.Assert(removedVersion > 0, "Invalid addedVersion");
			return new Lifetime(0, removedVersion);
		}

		public static Lifetime Spanning(int addedVersion, int removedVersion)
		{
			Global.Tracer.Assert(addedVersion > 0, "Invalid addedVersion");
			Global.Tracer.Assert(removedVersion > 0, "Invalid removedVersion");
			Global.Tracer.Assert(removedVersion > addedVersion, "removedVersion must be later than addedVersion");
			return new Lifetime(addedVersion, removedVersion);
		}
	}
}
