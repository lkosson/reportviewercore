namespace Microsoft.Cloud.Platform.Utils
{
	internal sealed class ScrubbedString : IContainsPrivateInformation
	{
		private readonly string m_plainString;

		public ScrubbedString(string plainString)
		{
			m_plainString = plainString;
		}

		public string ToPrivateString()
		{
			return m_plainString;
		}

		public string ToInternalString()
		{
			return m_plainString;
		}

		public string ToOriginalString()
		{
			return m_plainString;
		}
	}
}
