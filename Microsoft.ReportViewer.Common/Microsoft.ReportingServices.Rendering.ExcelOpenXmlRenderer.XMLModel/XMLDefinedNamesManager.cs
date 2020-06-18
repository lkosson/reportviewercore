using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLDefinedNamesManager
	{
		private readonly CT_Workbook _workbook;

		public XMLDefinedNamesManager(CT_Workbook workbook)
		{
			_workbook = workbook;
		}

		public XMLDefinedName CreateDefinedName(string name)
		{
			if (_workbook.DefinedNames == null)
			{
				_workbook.DefinedNames = new CT_DefinedNames();
			}
			CT_DefinedName cT_DefinedName = new CT_DefinedName();
			XMLDefinedName result = new XMLDefinedName(cT_DefinedName)
			{
				Name = name
			};
			_workbook.DefinedNames.DefinedName.Add(cT_DefinedName);
			return result;
		}

		public void Cleanup()
		{
			if (_workbook.DefinedNames == null)
			{
				return;
			}
			for (int num = _workbook.DefinedNames.DefinedName.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrEmpty(_workbook.DefinedNames.DefinedName[num].Content))
				{
					_workbook.DefinedNames.DefinedName.RemoveAt(num);
				}
			}
		}
	}
}
