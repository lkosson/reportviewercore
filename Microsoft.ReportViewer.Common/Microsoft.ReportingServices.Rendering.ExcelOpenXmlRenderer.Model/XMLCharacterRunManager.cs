using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal class XMLCharacterRunManager
	{
		private readonly CT_Rst _inlineString;

		private readonly List<XMLCharacterRunModel> _models = new List<XMLCharacterRunModel>();

		private readonly XMLPaletteModel _palette;

		public XMLCharacterRunManager(CT_Rst inlineString, XMLPaletteModel palette)
		{
			_inlineString = inlineString;
			_palette = palette;
		}

		public XMLCharacterRunModel CreateRun(int start, int length)
		{
			foreach (XMLCharacterRunModel model in _models)
			{
				if (model.StartIndex == start && model.Length == length)
				{
					return model;
				}
			}
			int num = start + length;
			List<CT_RElt> list = new List<CT_RElt>();
			int num2 = 0;
			for (int i = 0; i < _inlineString.R.Count; i++)
			{
				if (num2 < start && start < num2 + Span(i))
				{
					SplitRun(i, start - num2);
				}
				else if (num2 >= start && num2 + Span(i) <= num)
				{
					list.Add(_inlineString.R[i]);
				}
				else if (num2 < num && num < num2 + Span(i))
				{
					SplitRun(i, num - num2);
					list.Add(_inlineString.R[i]);
					break;
				}
				num2 += Span(i);
			}
			XMLCharacterRunModel xMLCharacterRunModel = new XMLCharacterRunModel(list, start, length, _palette);
			_models.Add(xMLCharacterRunModel);
			return xMLCharacterRunModel;
		}

		private int Span(int index)
		{
			return _inlineString.R[index].T.Length;
		}

		private void SplitRun(int runIndex, int splitIndex)
		{
			CT_RElt cT_RElt = _inlineString.R[runIndex];
			CT_RElt cT_RElt2 = new CT_RElt();
			string t = cT_RElt.T.Substring(0, splitIndex);
			string t2 = cT_RElt.T.Substring(splitIndex);
			cT_RElt.T = t;
			cT_RElt2.T = t2;
			if (cT_RElt.RPr != null)
			{
				throw new FatalException();
			}
			foreach (XMLCharacterRunModel model in _models)
			{
				model.Split(cT_RElt, cT_RElt2);
			}
			_inlineString.R.Insert(runIndex + 1, cT_RElt2);
		}
	}
}
