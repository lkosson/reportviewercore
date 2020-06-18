namespace Microsoft.Reporting.Chart.WebForms.Utilities
{
	internal class KeywordInfo
	{
		public string Name = string.Empty;

		public string Keyword = string.Empty;

		public string KeywordAliases = string.Empty;

		public string Description = string.Empty;

		public string AppliesToTypes = string.Empty;

		public string AppliesToProperties = string.Empty;

		public bool SupportsFormatting;

		public bool SupportsValueIndex;

		public KeywordInfo(string name, string keyword, string keywordAliases, string description, string appliesToTypes, string appliesToProperties, bool supportsFormatting, bool supportsValueIndex)
		{
			Name = name;
			Keyword = keyword;
			KeywordAliases = keywordAliases;
			Description = description;
			AppliesToTypes = appliesToTypes;
			AppliesToProperties = appliesToProperties;
			SupportsFormatting = supportsFormatting;
			SupportsValueIndex = supportsValueIndex;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
