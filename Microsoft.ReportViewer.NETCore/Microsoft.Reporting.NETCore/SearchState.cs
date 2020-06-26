using System;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class SearchState
	{
		private string m_text;

		private int m_startPage;

		public string Text => m_text;

		public int StartPage => m_startPage;

		internal SearchState(string text, int startPage)
		{
			m_text = text;
			m_startPage = startPage;
		}
	}
}
