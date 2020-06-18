namespace Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel
{
	internal interface IRichTextInfo
	{
		bool CheckForRotatedFarEastChars
		{
			set;
		}

		IFont AppendTextRun(string value);

		IFont AppendTextRun(string value, bool replaceInvalidWhitespace);

		void AppendText(string value);

		void AppendText(string value, bool replaceInvalidWhiteSpace);

		void AppendText(char value);
	}
}
