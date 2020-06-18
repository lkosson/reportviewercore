using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLDefinedName
	{
		private readonly CT_DefinedName _definedname;

		public string Name
		{
			set
			{
				TryValidName(value);
				_definedname.Name_Attr = value;
			}
		}

		public string Content
		{
			set
			{
				_definedname.Content = value;
			}
		}

		public int SheetIndex
		{
			set
			{
				_definedname.LocalSheetId_Attr = (uint)value;
			}
		}

		public XMLDefinedName(CT_DefinedName definedName)
		{
			_definedname = definedName;
		}

		private static void TryValidName(string name)
		{
			if (name == null)
			{
				throw new FatalException();
			}
			if (name.Length < 1 || name.Length > 255)
			{
				throw new FatalException();
			}
			if (name.Equals("true", StringComparison.OrdinalIgnoreCase) || name.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
				throw new FatalException();
			}
			if (!XMLConstants.DefinedNames.DefinedNameValidationRe.IsMatch(name))
			{
				throw new FatalException();
			}
		}
	}
}
