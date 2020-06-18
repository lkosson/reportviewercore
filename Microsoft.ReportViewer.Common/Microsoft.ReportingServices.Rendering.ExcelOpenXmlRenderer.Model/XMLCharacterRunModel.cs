using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal class XMLCharacterRunModel : ICharacterRunModel
	{
		private readonly CharacterRun _interface;

		private readonly int _startIndex;

		private readonly int _length;

		private readonly List<CT_RElt> _runs;

		private readonly XMLFontRunModel _font;

		public CharacterRun Interface => _interface;

		public int StartIndex => _startIndex;

		public int Length => _length;

		public XMLCharacterRunModel(List<CT_RElt> runs, int startIndex, int length, XMLPaletteModel palette)
		{
			_runs = runs;
			_startIndex = startIndex;
			_length = length;
			_interface = new CharacterRun(this);
			_font = new XMLFontRunModel(palette);
			foreach (CT_RElt run in runs)
			{
				if (run.RPr == null)
				{
					run.RPr = new CT_RPrElt();
				}
				_font.Add(run.RPr);
			}
		}

		public void SetFont(Font font)
		{
			XMLFontModel xMLFontModel = font.Model as XMLFontModel;
			if (xMLFontModel != null)
			{
				_font.SetFont(xMLFontModel);
				return;
			}
			throw new FatalException();
		}

		public void Split(CT_RElt original, CT_RElt added)
		{
			if (_runs.Contains(original))
			{
				_runs.Add(added);
				_font.Add(added.RPr);
			}
		}
	}
}
