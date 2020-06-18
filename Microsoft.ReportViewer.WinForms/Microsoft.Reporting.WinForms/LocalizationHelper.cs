using System;
using System.Globalization;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class LocalizationHelper : IReportViewerMessages5, IReportViewerMessages4, IReportViewerMessages3, IReportViewerMessages2, IReportViewerMessages
	{
		private static LocalizationHelper m_theInstance;

		private static object m_lockObject = new object();

		private IReportViewerMessages m_winformsViewerMessages;

		public static IReportViewerMessages5 Current
		{
			get
			{
				lock (m_lockObject)
				{
					if (m_theInstance == null)
					{
						m_theInstance = new LocalizationHelper();
					}
					return m_theInstance;
				}
			}
		}

		private IReportViewerMessages2 ReportViewerMessages2 => m_winformsViewerMessages as IReportViewerMessages2;

		private IReportViewerMessages3 ReportViewerMessages3 => m_winformsViewerMessages as IReportViewerMessages3;

		private IReportViewerMessages4 ReportViewerMessages4 => m_winformsViewerMessages as IReportViewerMessages4;

		private IReportViewerMessages5 ReportViewerMessages5 => m_winformsViewerMessages as IReportViewerMessages5;

		string IReportViewerMessages.DocumentMapButtonToolTip => GetLocalizedString(ReportPreviewStrings.DocMapToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.DocumentMapButtonToolTip : null);

		string IReportViewerMessages.ParameterAreaButtonToolTip => GetLocalizedString(ReportPreviewStrings.ShowParamsToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ParameterAreaButtonToolTip : null);

		string IReportViewerMessages.FirstPageButtonToolTip => GetLocalizedString(ReportPreviewStrings.FirstPageToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FirstPageButtonToolTip : null);

		string IReportViewerMessages.PreviousPageButtonToolTip => GetLocalizedString(ReportPreviewStrings.PreviousPageToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PreviousPageButtonToolTip : null);

		string IReportViewerMessages.CurrentPageTextBoxToolTip => GetLocalizedString(ReportPreviewStrings.CurrentPageToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.CurrentPageTextBoxToolTip : null);

		string IReportViewerMessages.PageOf => GetLocalizedString(ReportPreviewStrings.LabelOfText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PageOf : null);

		string IReportViewerMessages.NextPageButtonToolTip => GetLocalizedString(ReportPreviewStrings.NextPageToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.NextPageButtonToolTip : null);

		string IReportViewerMessages.LastPageButtonToolTip => GetLocalizedString(ReportPreviewStrings.LastPageToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.LastPageButtonToolTip : null);

		string IReportViewerMessages.BackButtonToolTip => GetLocalizedString(ReportPreviewStrings.BackToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.BackButtonToolTip : null);

		string IReportViewerMessages.RefreshButtonToolTip => GetLocalizedString(ReportPreviewStrings.RefreshToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.RefreshButtonToolTip : null);

		string IReportViewerMessages.PrintButtonToolTip => GetLocalizedString(ReportPreviewStrings.PrintToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PrintButtonToolTip : null);

		string IReportViewerMessages.ExportButtonToolTip => GetLocalizedString(ReportPreviewStrings.ExportToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ExportButtonToolTip : null);

		string IReportViewerMessages.ZoomControlToolTip => GetLocalizedString(ReportPreviewStrings.ZoomToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ZoomControlToolTip : null);

		string IReportViewerMessages.SearchTextBoxToolTip => GetLocalizedString(ReportPreviewStrings.SearchTextBoxToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.SearchTextBoxToolTip : null);

		string IReportViewerMessages.FindButtonToolTip => GetLocalizedString(ReportPreviewStrings.FindToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FindButtonToolTip : null);

		string IReportViewerMessages.FindNextButtonToolTip => GetLocalizedString(ReportPreviewStrings.FindNextToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FindNextButtonToolTip : null);

		string IReportViewerMessages.FoundResultText => string.Empty;

		string IReportViewerMessages.ZoomToPageWidth => GetLocalizedString(ReportPreviewStrings.PageWidth, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ZoomToPageWidth : null);

		string IReportViewerMessages.ZoomToWholePage => GetLocalizedString(ReportPreviewStrings.FullPage, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ZoomToWholePage : null);

		string IReportViewerMessages.FindButtonText => GetLocalizedString(ReportPreviewStrings.FindText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FindButtonText : null);

		string IReportViewerMessages.FindNextButtonText => GetLocalizedString(ReportPreviewStrings.FindNextText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FindNextButtonText : null);

		string IReportViewerMessages.ViewReportButtonText
		{
			get
			{
				if (m_winformsViewerMessages == null)
				{
					return null;
				}
				return m_winformsViewerMessages.ViewReportButtonText;
			}
		}

		string IReportViewerMessages.ProgressText => GetLocalizedString(CommonStrings.AsyncProgressText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ProgressText : null);

		string IReportViewerMessages.TextNotFound => GetLocalizedString(ReportPreviewStrings.NoMatches, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.TextNotFound : null);

		string IReportViewerMessages.NoMoreMatches => GetLocalizedString(ReportPreviewStrings.NoMoreMatches, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.NoMoreMatches : null);

		string IReportViewerMessages.ChangeCredentialsText
		{
			get
			{
				if (m_winformsViewerMessages == null)
				{
					return null;
				}
				return m_winformsViewerMessages.ChangeCredentialsText;
			}
		}

		string IReportViewerMessages.NullCheckBoxText => GetLocalizedString(ReportPreviewStrings.NullParameterLabel, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.NullCheckBoxText : null);

		string IReportViewerMessages.NullValueText => GetLocalizedString(ReportPreviewStrings.NullValue, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.NullValueText : null);

		string IReportViewerMessages.TrueValueText => GetLocalizedString(ReportPreviewStrings.TrueButtonLabel, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.TrueValueText : null);

		string IReportViewerMessages.FalseValueText => GetLocalizedString(ReportPreviewStrings.FalseButtonLabel, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.FalseValueText : null);

		string IReportViewerMessages.SelectAValue => GetLocalizedString(ReportPreviewStrings.SelectValidValue, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.SelectAValue : null);

		string IReportViewerMessages.UserNamePrompt => GetLocalizedString(ReportPreviewStrings.LogInNamePrompt, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.UserNamePrompt : null);

		string IReportViewerMessages.PasswordPrompt => GetLocalizedString(ReportPreviewStrings.PasswordPrompt, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PasswordPrompt : null);

		string IReportViewerMessages.SelectAll => GetLocalizedString(ReportPreviewStrings.SelectAll, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.SelectAll : null);

		string IReportViewerMessages.TotalPagesToolTip => GetLocalizedString(ReportPreviewStrings.TotalPagesToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.TotalPagesToolTip : null);

		string IReportViewerMessages.StopButtonToolTip => GetLocalizedString(ReportPreviewStrings.StopToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.StopButtonToolTip : null);

		string IReportViewerMessages.PrintLayoutButtonToolTip => GetLocalizedString(ReportPreviewStrings.PrintPreviewToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PrintLayoutButtonToolTip : null);

		string IReportViewerMessages.PageSetupButtonToolTip => GetLocalizedString(ReportPreviewStrings.PageSetupToolTipText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PageSetupButtonToolTip : null);

		string IReportViewerMessages.NullCheckBoxToolTip => GetLocalizedString(ReportPreviewStrings.NullTooltip, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.NullCheckBoxToolTip : null);

		string IReportViewerMessages.DocumentMapMenuItemText => GetLocalizedString(ReportPreviewStrings.DocumentMapMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.DocumentMapMenuItemText : null);

		string IReportViewerMessages.BackMenuItemText => GetLocalizedString(ReportPreviewStrings.BackMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.BackMenuItemText : null);

		string IReportViewerMessages.RefreshMenuItemText => GetLocalizedString(ReportPreviewStrings.RefreshMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.RefreshMenuItemText : null);

		string IReportViewerMessages.PrintMenuItemText => GetLocalizedString(ReportPreviewStrings.PrintMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PrintMenuItemText : null);

		string IReportViewerMessages.PrintLayoutMenuItemText => GetLocalizedString(ReportPreviewStrings.PrintLayoutMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PrintLayoutMenuItemText : null);

		string IReportViewerMessages.PageSetupMenuItemText => GetLocalizedString(ReportPreviewStrings.PageSetupMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.PageSetupMenuItemText : null);

		string IReportViewerMessages.ExportMenuItemText => GetLocalizedString(ReportPreviewStrings.ExportMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ExportMenuItemText : null);

		string IReportViewerMessages.StopMenuItemText => GetLocalizedString(ReportPreviewStrings.StopMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.StopMenuItemText : null);

		string IReportViewerMessages.ZoomMenuItemText => GetLocalizedString(ReportPreviewStrings.ZoomMenuItemText, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ZoomMenuItemText : null);

		string IReportViewerMessages.ViewReportButtonToolTip => GetLocalizedString(ReportPreviewStrings.ViewReportButton, (m_winformsViewerMessages != null) ? m_winformsViewerMessages.ViewReportButtonToolTip : null);

		public string ExportErrorTitle => GetLocalizedString(ReportPreviewStrings.ExportErrors, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ExportErrorTitle : null);

		public string AllFilesFilter => GetLocalizedString(ReportPreviewStrings.AllFiles, (ReportViewerMessages2 != null) ? ReportViewerMessages2.AllFilesFilter : null);

		public string PromptAreaErrorTitle => GetLocalizedString(ReportPreviewStrings.ParamsErrors, (ReportViewerMessages2 != null) ? ReportViewerMessages2.PromptAreaErrorTitle : null);

		public string StringToolTip => GetLocalizedString(ReportPreviewStrings.StringTooltip, (ReportViewerMessages2 != null) ? ReportViewerMessages2.StringToolTip : null);

		public string FloatToolTip => GetLocalizedString(ReportPreviewStrings.FloatTooltip, (ReportViewerMessages2 != null) ? ReportViewerMessages2.FloatToolTip : null);

		public string IntToolTip => GetLocalizedString(ReportPreviewStrings.IntTooltip, (ReportViewerMessages2 != null) ? ReportViewerMessages2.IntToolTip : null);

		public string DateToolTip => GetLocalizedString(ReportPreviewStrings.DateTimeTooltip, (ReportViewerMessages2 != null) ? ReportViewerMessages2.DateToolTip : null);

		public string MessageBoxTitle => GetLocalizedString(ReportPreviewStrings.GenericMessageBoxCaption, (ReportViewerMessages2 != null) ? ReportViewerMessages2.MessageBoxTitle : null);

		public string ProcessingStopped => GetLocalizedString(ReportPreviewStrings.StopProcessing, (ReportViewerMessages2 != null) ? ReportViewerMessages2.ProcessingStopped : null);

		public string HyperlinkErrorTitle => GetLocalizedString(ReportPreviewStrings.HyperlinkError, (ReportViewerMessages2 != null) ? ReportViewerMessages2.HyperlinkErrorTitle : null);

		public string CancelLinkText => GetLocalizedString(CommonStrings.CancelLinkText, (ReportViewerMessages3 != null) ? ReportViewerMessages3.CancelLinkText : null);

		public string ExportDialogTitle => GetLocalizedString(ReportPreviewStrings.ExportDialogTitle, (ReportViewerMessages3 != null) ? ReportViewerMessages3.ExportDialogTitle : null);

		public string ExportDialogCancelButton => GetLocalizedString(ReportPreviewStrings.ExportDialogCancelButton, (ReportViewerMessages3 != null) ? ReportViewerMessages3.ExportDialogCancelButton : null);

		public string ExportDialogStatusText => GetLocalizedString(ReportPreviewStrings.ExportDialogStatusText, (ReportViewerMessages3 != null) ? ReportViewerMessages3.ExportDialogStatusText : null);

		public string TrueBooleanToolTip => GetLocalizedString(ReportPreviewStrings.BoolTooltip, (ReportViewerMessages3 != null) ? ReportViewerMessages3.TrueBooleanToolTip : null);

		public string FalseBooleanToolTip => GetLocalizedString(ReportPreviewStrings.BoolTooltip, (ReportViewerMessages3 != null) ? ReportViewerMessages3.FalseBooleanToolTip : null);

		string IReportViewerMessages4.ShowDocumentMapButtonTooltip => GetLocalizedString(CommonStrings.ShowDocMapTooltip, (ReportViewerMessages4 != null) ? ReportViewerMessages4.ShowDocumentMapButtonTooltip : null);

		string IReportViewerMessages4.HideDocumentMapButtonTooltip => GetLocalizedString(CommonStrings.HideDocMapTooltip, (ReportViewerMessages4 != null) ? ReportViewerMessages4.HideDocumentMapButtonTooltip : null);

		string IReportViewerMessages4.ShowParameterAreaButtonToolTip => GetLocalizedString(CommonStrings.ShowParametersTooltip, (ReportViewerMessages4 != null) ? ReportViewerMessages4.ShowParameterAreaButtonToolTip : null);

		string IReportViewerMessages4.HideParameterAreaButtonToolTip => GetLocalizedString(CommonStrings.HideParametersTooltip, (ReportViewerMessages4 != null) ? ReportViewerMessages4.HideParameterAreaButtonToolTip : null);

		private LocalizationHelper()
		{
		}

		private static string GetLocalizedString(string builtinString, string customString)
		{
			if (customString == null)
			{
				return builtinString;
			}
			return customString;
		}

		internal void SetWinformsViewerMessages(IReportViewerMessages messages)
		{
			m_winformsViewerMessages = messages;
		}

		string IReportViewerMessages2.GetLocalizedNameForRenderingExtension(string format)
		{
			throw new NotImplementedException();
		}

		public string GetLocalizedNameForRenderingExtension(RenderingExtension ext)
		{
			return GetLocalizedString(ext.LocalizedName, (ReportViewerMessages2 != null) ? ReportViewerMessages2.GetLocalizedNameForRenderingExtension(ext.Name) : null);
		}

		public string ParameterMissingSelectionError(string parameterPrompt)
		{
			return GetLocalizedString(ReportPreviewStrings.MissingSelectionClientError(parameterPrompt), (ReportViewerMessages2 != null) ? ReportViewerMessages2.ParameterMissingSelectionError(parameterPrompt) : null);
		}

		public string ParameterMissingValueError(string parameterPrompt)
		{
			return GetLocalizedString(ReportPreviewStrings.MissingValueClientError(parameterPrompt), (ReportViewerMessages2 != null) ? ReportViewerMessages2.ParameterMissingValueError(parameterPrompt) : null);
		}

		public string CredentialMissingUserNameError(string dataSourcePrompt)
		{
			return GetLocalizedString(ReportPreviewStrings.MissingCredentials(dataSourcePrompt), (ReportViewerMessages2 != null) ? ReportViewerMessages2.CredentialMissingUserNameError(dataSourcePrompt) : null);
		}

		string IReportViewerMessages3.TotalPages(int pageCount, PageCountMode pageCountMode)
		{
			string builtinString = (pageCountMode != 0 && pageCount > 0) ? CommonStrings.EstimateTotalPages(pageCount) : pageCount.ToString(CultureInfo.CurrentCulture);
			return GetLocalizedString(builtinString, (ReportViewerMessages3 != null) ? ReportViewerMessages3.TotalPages(pageCount, pageCountMode) : null);
		}
	}
}
