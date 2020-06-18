namespace Microsoft.Reporting.WinForms
{
	public interface IReportViewerMessages2 : IReportViewerMessages
	{
		string ExportErrorTitle
		{
			get;
		}

		string AllFilesFilter
		{
			get;
		}

		string PromptAreaErrorTitle
		{
			get;
		}

		string StringToolTip
		{
			get;
		}

		string FloatToolTip
		{
			get;
		}

		string IntToolTip
		{
			get;
		}

		string DateToolTip
		{
			get;
		}

		string MessageBoxTitle
		{
			get;
		}

		string ProcessingStopped
		{
			get;
		}

		string HyperlinkErrorTitle
		{
			get;
		}

		string GetLocalizedNameForRenderingExtension(string format);

		string ParameterMissingSelectionError(string parameterPrompt);

		string ParameterMissingValueError(string parameterPrompt);

		string CredentialMissingUserNameError(string dataSourcePrompt);
	}
}
