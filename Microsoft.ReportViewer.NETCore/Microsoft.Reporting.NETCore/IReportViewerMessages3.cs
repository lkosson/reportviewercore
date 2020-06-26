namespace Microsoft.Reporting.NETCore
{
	public interface IReportViewerMessages3 : IReportViewerMessages2, IReportViewerMessages
	{
		string CancelLinkText
		{
			get;
		}

		string ExportDialogTitle
		{
			get;
		}

		string ExportDialogCancelButton
		{
			get;
		}

		string ExportDialogStatusText
		{
			get;
		}

		string FalseBooleanToolTip
		{
			get;
		}

		string TrueBooleanToolTip
		{
			get;
		}

		string TotalPages(int pageCount, PageCountMode pageCountMode);
	}
}
