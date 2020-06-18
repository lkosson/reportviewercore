using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class StyleManager
	{
		private abstract class ElementBag<T>
		{
			protected readonly IDictionary<uint, T> _values = new Dictionary<uint, T>();

			protected readonly IDictionary<T, uint> _lookup = new Dictionary<T, uint>();

			protected uint _counter;

			public uint Count => (uint)_values.Count;

			public ElementBag(uint startIndex)
			{
				_counter = startIndex;
			}

			protected abstract T Freeze(T value);

			public void Add(T value, uint index)
			{
				if (_counter <= index)
				{
					_counter = index + 1;
				}
				_lookup[value] = index;
				_values[index] = value;
			}

			public uint Add(T value)
			{
				if (_lookup.TryGetValue(value, out uint value2))
				{
					return value2;
				}
				_lookup[value] = _counter;
				_values[_counter] = value;
				return _counter++;
			}

			public void ForceAdd(T value)
			{
				if (!_lookup.ContainsKey(value))
				{
					_lookup[value] = _counter;
				}
				_values[_counter] = value;
				_counter++;
			}

			public T Get(uint index)
			{
				if (_values.TryGetValue(index, out T value))
				{
					return Freeze(value);
				}
				throw new FatalException();
			}

			public IEnumerable<KeyValuePair<uint, T>> GetPairs()
			{
				foreach (KeyValuePair<uint, T> value in _values)
				{
					yield return value;
				}
			}
		}

		private class ElementBagOfStrings : ElementBag<string>
		{
			public ElementBagOfStrings(uint startIndex)
				: base(startIndex)
			{
			}

			protected override string Freeze(string value)
			{
				return value;
			}
		}

		private class ElementBagCloneable<T> : ElementBag<T> where T : IDeepCloneable<T>
		{
			public ElementBagCloneable(uint startIndex)
				: base(startIndex)
			{
			}

			protected override T Freeze(T value)
			{
				return value.DeepClone();
			}
		}

		private readonly StyleSheetPart _part;

		private readonly CT_Stylesheet _stylesheet;

		private readonly ElementBag<string> _numberFormats = new ElementBagOfStrings(82u);

		private readonly ElementBag<XMLFontModel> _fonts = new ElementBagCloneable<XMLFontModel>(0u);

		private readonly ElementBag<XMLFillModel> _fills = new ElementBagCloneable<XMLFillModel>(0u);

		private readonly ElementBag<XMLBorderModel> _borders = new ElementBagCloneable<XMLBorderModel>(0u);

		private readonly ElementBag<XMLStyleModel> _styles = new ElementBagCloneable<XMLStyleModel>(0u);

		private readonly XMLPaletteModel _palette = new XMLPaletteModel();

		private readonly Dictionary<string, uint> _namedlookup = new Dictionary<string, uint>();

		public XMLPaletteModel Palette => _palette;

		public StyleManager(StyleSheetPart part)
		{
			_part = part;
			_stylesheet = (CT_Stylesheet)_part.Root;
			Hydrate();
		}

		public static CT_Xf CreateDefaultXf()
		{
			return new CT_Xf
			{
				NumFmtId_Attr = 0u,
				FontId_Attr = 0u,
				FillId_Attr = 0u,
				BorderId_Attr = 0u
			};
		}

		public XMLStyleModel CreateStyle()
		{
			return new XMLStyleModel(this);
		}

		public XMLStyleModel CreateStyle(XMLStyleModel src)
		{
			return (XMLStyleModel)src.cloneStyle(cellStyle: true);
		}

		public XMLStyleModel CreateStyle(uint index)
		{
			if (index != 0)
			{
				return (XMLStyleModel)GetStyle(index).cloneStyle(cellStyle: true);
			}
			return new XMLStyleModel(CreateDefaultXf(), this);
		}

		public uint CommitStyle(XMLStyleModel style)
		{
			style.Index = _styles.Add(style);
			return style.Index;
		}

		public XMLFontModel GetFont(uint index)
		{
			return _fonts.Get(index);
		}

		public XMLFillModel GetFill(uint index)
		{
			return _fills.Get(index);
		}

		public XMLBorderModel GetBorder(uint index)
		{
			return _borders.Get(index);
		}

		public uint AddNumberFormat(string format)
		{
			return _numberFormats.Add(format);
		}

		public uint AddFont(XMLFontModel model)
		{
			return _fonts.Add(model);
		}

		public uint AddFill(XMLFillModel model)
		{
			return _fills.Add(model);
		}

		public uint AddBorder(XMLBorderModel model)
		{
			return _borders.Add(model);
		}

		public XMLStyleModel GetStyle(uint index)
		{
			return _styles.Get(index);
		}

		private void Hydrate()
		{
			if (_stylesheet.Borders == null)
			{
				_stylesheet.Borders = new CT_Borders();
			}
			if (_stylesheet.CellStyles == null)
			{
				_stylesheet.CellStyles = new CT_CellStyles();
			}
			if (_stylesheet.CellStyleXfs == null)
			{
				_stylesheet.CellStyleXfs = new CT_CellStyleXfs();
			}
			if (_stylesheet.CellXfs == null)
			{
				_stylesheet.CellXfs = new CT_CellXfs();
			}
			if (_stylesheet.Fills == null)
			{
				_stylesheet.Fills = new CT_Fills();
			}
			if (_stylesheet.Fonts == null)
			{
				_stylesheet.Fonts = new CT_Fonts();
			}
			if (_stylesheet.NumFmts == null)
			{
				_stylesheet.NumFmts = new CT_NumFmts();
			}
			if (_stylesheet.Borders.Border == null)
			{
				_stylesheet.Borders.Border = new List<CT_Border>();
			}
			if (_stylesheet.CellStyles.CellStyle == null)
			{
				_stylesheet.CellStyles.CellStyle = new List<CT_CellStyle>();
			}
			if (_stylesheet.CellStyleXfs.Xf == null)
			{
				_stylesheet.CellStyleXfs.Xf = new List<CT_Xf>();
			}
			if (_stylesheet.CellXfs.Xf == null)
			{
				_stylesheet.CellXfs.Xf = new List<CT_Xf>();
			}
			if (_stylesheet.Fills.Fill == null)
			{
				_stylesheet.Fills.Fill = new List<CT_Fill>();
			}
			if (_stylesheet.Fonts.Font == null)
			{
				_stylesheet.Fonts.Font = new List<CT_Font>();
			}
			if (_stylesheet.NumFmts.NumFmt == null)
			{
				_stylesheet.NumFmts.NumFmt = new List<CT_NumFmt>();
			}
			foreach (CT_NumFmt item in _stylesheet.NumFmts.NumFmt)
			{
				_numberFormats.Add(item.FormatCode_Attr, item.NumFmtId_Attr);
			}
			for (int i = 0; i < _stylesheet.Fonts.Font.Count; i++)
			{
				CT_Font font = _stylesheet.Fonts.Font[i];
				_fonts.Add(new XMLFontModel(font, Palette), (uint)i);
			}
			foreach (CT_Fill item2 in _stylesheet.Fills.Fill)
			{
				_fills.Add(new XMLFillModel(item2, Palette));
			}
			foreach (CT_Border item3 in _stylesheet.Borders.Border)
			{
				_borders.Add(new XMLBorderModel(item3, Palette));
			}
			foreach (CT_Xf item4 in _stylesheet.CellXfs.Xf)
			{
				_styles.ForceAdd(new XMLStyleModel(item4, this, setVerticalAlignment: false));
			}
			List<string> list = new List<string>();
			foreach (CT_CellStyle item5 in _stylesheet.CellStyles.CellStyle)
			{
				list.Add(item5.Name_Attr);
				_namedlookup.Add(item5.Name_Attr, item5.XfId_Attr);
			}
			_stylesheet.NumFmts.NumFmt.Clear();
			_stylesheet.Fonts.Font.Clear();
			_stylesheet.Fills.Fill.Clear();
			_stylesheet.Borders.Border.Clear();
			_stylesheet.CellXfs.Xf.Clear();
			_stylesheet.CellStyles.CellStyle.Clear();
		}

		public void Cleanup()
		{
			foreach (KeyValuePair<uint, XMLStyleModel> pair in _styles.GetPairs())
			{
				pair.Value.Cleanup();
				_stylesheet.CellXfs.Xf.Add(pair.Value.Data);
			}
			foreach (KeyValuePair<string, uint> item in _namedlookup)
			{
				CT_CellStyle cT_CellStyle = new CT_CellStyle();
				cT_CellStyle.Name_Attr = item.Key;
				cT_CellStyle.XfId_Attr = item.Value;
				_stylesheet.CellStyles.CellStyle.Add(cT_CellStyle);
			}
			foreach (KeyValuePair<uint, string> pair2 in _numberFormats.GetPairs())
			{
				CT_NumFmt cT_NumFmt = new CT_NumFmt();
				cT_NumFmt.NumFmtId_Attr = pair2.Key;
				cT_NumFmt.FormatCode_Attr = pair2.Value;
				_stylesheet.NumFmts.NumFmt.Add(cT_NumFmt);
			}
			foreach (KeyValuePair<uint, XMLFontModel> pair3 in _fonts.GetPairs())
			{
				_stylesheet.Fonts.Font.Add(pair3.Value.Data);
			}
			foreach (KeyValuePair<uint, XMLFillModel> pair4 in _fills.GetPairs())
			{
				pair4.Value.Cleanup();
				_stylesheet.Fills.Fill.Add(pair4.Value.Data);
			}
			foreach (KeyValuePair<uint, XMLBorderModel> pair5 in _borders.GetPairs())
			{
				_stylesheet.Borders.Border.Add(pair5.Value.Data);
			}
			if (_palette.LegacyPaletteModified)
			{
				_palette.WriteIndexedColors(_stylesheet);
			}
			_stylesheet.NumFmts.Count_Attr = _numberFormats.Count;
			_stylesheet.Fonts.Count_Attr = _fonts.Count;
			_stylesheet.Fills.Count_Attr = _fills.Count;
			_stylesheet.Borders.Count_Attr = _borders.Count;
			_stylesheet.CellXfs.Count_Attr = _styles.Count;
			_stylesheet.CellStyles.Count_Attr = (uint)_namedlookup.Count;
		}
	}
}
