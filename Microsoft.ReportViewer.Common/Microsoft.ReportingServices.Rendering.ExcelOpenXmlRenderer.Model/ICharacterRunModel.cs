namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal interface ICharacterRunModel
	{
		CharacterRun Interface
		{
			get;
		}

		int StartIndex
		{
			get;
		}

		int Length
		{
			get;
		}

		void SetFont(Font font);
	}
}
