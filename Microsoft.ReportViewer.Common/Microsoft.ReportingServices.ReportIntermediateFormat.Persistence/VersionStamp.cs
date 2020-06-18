namespace Microsoft.ReportingServices.ReportIntermediateFormat.Persistence
{
	internal sealed class VersionStamp
	{
		private static readonly byte[] Stamp = new byte[16]
		{
			146,
			87,
			240,
			123,
			205,
			241,
			175,
			78,
			136,
			213,
			28,
			14,
			76,
			128,
			111,
			25
		};

		internal static byte[] GetBytes()
		{
			return Stamp;
		}

		internal static bool Validate(byte[] stamp)
		{
			if (stamp == null)
			{
				return false;
			}
			if (Stamp.Length != stamp.Length)
			{
				return false;
			}
			for (int i = 0; i < Stamp.Length; i++)
			{
				if (Stamp[i] != stamp[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
