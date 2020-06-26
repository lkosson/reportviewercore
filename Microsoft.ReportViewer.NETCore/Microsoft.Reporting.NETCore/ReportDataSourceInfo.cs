using System;

namespace Microsoft.Reporting.NETCore
{
	[Serializable]
	public sealed class ReportDataSourceInfo
	{
		private string m_name;

		private string m_prompt;

		public string Name => m_name;

		public string Prompt => m_prompt;

		internal ReportDataSourceInfo(string name, string prompt)
		{
			m_name = name;
			m_prompt = prompt;
		}
	}
}
