using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Xf : OoxmlComplexType
	{
		private OoxmlBool _quotePrefix_attr;

		private OoxmlBool _pivotButton_attr;

		private OoxmlBool _applyNumberFormat_attr;

		private OoxmlBool _applyFont_attr;

		private OoxmlBool _applyFill_attr;

		private OoxmlBool _applyBorder_attr;

		private OoxmlBool _applyAlignment_attr;

		private OoxmlBool _applyProtection_attr;

		private uint _numFmtId_attr;

		private bool _numFmtId_attr_is_specified;

		private uint _fontId_attr;

		private bool _fontId_attr_is_specified;

		private uint _fillId_attr;

		private bool _fillId_attr_is_specified;

		private uint _borderId_attr;

		private bool _borderId_attr_is_specified;

		private uint _xfId_attr;

		private bool _xfId_attr_is_specified;

		private CT_CellAlignment _alignment;

		public OoxmlBool QuotePrefix_Attr
		{
			get
			{
				return _quotePrefix_attr;
			}
			set
			{
				_quotePrefix_attr = value;
			}
		}

		public OoxmlBool PivotButton_Attr
		{
			get
			{
				return _pivotButton_attr;
			}
			set
			{
				_pivotButton_attr = value;
			}
		}

		public OoxmlBool ApplyNumberFormat_Attr
		{
			get
			{
				return _applyNumberFormat_attr;
			}
			set
			{
				_applyNumberFormat_attr = value;
			}
		}

		public OoxmlBool ApplyFont_Attr
		{
			get
			{
				return _applyFont_attr;
			}
			set
			{
				_applyFont_attr = value;
			}
		}

		public OoxmlBool ApplyFill_Attr
		{
			get
			{
				return _applyFill_attr;
			}
			set
			{
				_applyFill_attr = value;
			}
		}

		public OoxmlBool ApplyBorder_Attr
		{
			get
			{
				return _applyBorder_attr;
			}
			set
			{
				_applyBorder_attr = value;
			}
		}

		public OoxmlBool ApplyAlignment_Attr
		{
			get
			{
				return _applyAlignment_attr;
			}
			set
			{
				_applyAlignment_attr = value;
			}
		}

		public OoxmlBool ApplyProtection_Attr
		{
			get
			{
				return _applyProtection_attr;
			}
			set
			{
				_applyProtection_attr = value;
			}
		}

		public uint NumFmtId_Attr
		{
			get
			{
				return _numFmtId_attr;
			}
			set
			{
				_numFmtId_attr = value;
				_numFmtId_attr_is_specified = true;
			}
		}

		public bool NumFmtId_Attr_Is_Specified
		{
			get
			{
				return _numFmtId_attr_is_specified;
			}
			set
			{
				_numFmtId_attr_is_specified = value;
			}
		}

		public uint FontId_Attr
		{
			get
			{
				return _fontId_attr;
			}
			set
			{
				_fontId_attr = value;
				_fontId_attr_is_specified = true;
			}
		}

		public bool FontId_Attr_Is_Specified
		{
			get
			{
				return _fontId_attr_is_specified;
			}
			set
			{
				_fontId_attr_is_specified = value;
			}
		}

		public uint FillId_Attr
		{
			get
			{
				return _fillId_attr;
			}
			set
			{
				_fillId_attr = value;
				_fillId_attr_is_specified = true;
			}
		}

		public bool FillId_Attr_Is_Specified
		{
			get
			{
				return _fillId_attr_is_specified;
			}
			set
			{
				_fillId_attr_is_specified = value;
			}
		}

		public uint BorderId_Attr
		{
			get
			{
				return _borderId_attr;
			}
			set
			{
				_borderId_attr = value;
				_borderId_attr_is_specified = true;
			}
		}

		public bool BorderId_Attr_Is_Specified
		{
			get
			{
				return _borderId_attr_is_specified;
			}
			set
			{
				_borderId_attr_is_specified = value;
			}
		}

		public uint XfId_Attr
		{
			get
			{
				return _xfId_attr;
			}
			set
			{
				_xfId_attr = value;
				_xfId_attr_is_specified = true;
			}
		}

		public bool XfId_Attr_Is_Specified
		{
			get
			{
				return _xfId_attr_is_specified;
			}
			set
			{
				_xfId_attr_is_specified = value;
			}
		}

		public CT_CellAlignment Alignment
		{
			get
			{
				return _alignment;
			}
			set
			{
				_alignment = value;
			}
		}

		public static string AlignmentElementName => "alignment";

		protected override void InitAttributes()
		{
			_quotePrefix_attr = OoxmlBool.OoxmlFalse;
			_pivotButton_attr = OoxmlBool.OoxmlFalse;
			_applyNumberFormat_attr = OoxmlBool.OoxmlFalse;
			_applyFont_attr = OoxmlBool.OoxmlFalse;
			_applyFill_attr = OoxmlBool.OoxmlFalse;
			_applyBorder_attr = OoxmlBool.OoxmlFalse;
			_applyAlignment_attr = OoxmlBool.OoxmlFalse;
			_applyProtection_attr = OoxmlBool.OoxmlFalse;
			_numFmtId_attr_is_specified = false;
			_fontId_attr_is_specified = false;
			_fillId_attr_is_specified = false;
			_borderId_attr_is_specified = false;
			_xfId_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
		}

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if ((bool)(_quotePrefix_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" quotePrefix=\"");
				OoxmlComplexType.WriteData(s, _quotePrefix_attr);
				s.Write("\"");
			}
			if ((bool)(_pivotButton_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" pivotButton=\"");
				OoxmlComplexType.WriteData(s, _pivotButton_attr);
				s.Write("\"");
			}
			if ((bool)(_applyNumberFormat_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyNumberFormat=\"");
				OoxmlComplexType.WriteData(s, _applyNumberFormat_attr);
				s.Write("\"");
			}
			if ((bool)(_applyFont_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyFont=\"");
				OoxmlComplexType.WriteData(s, _applyFont_attr);
				s.Write("\"");
			}
			if ((bool)(_applyFill_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyFill=\"");
				OoxmlComplexType.WriteData(s, _applyFill_attr);
				s.Write("\"");
			}
			if ((bool)(_applyBorder_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyBorder=\"");
				OoxmlComplexType.WriteData(s, _applyBorder_attr);
				s.Write("\"");
			}
			if ((bool)(_applyAlignment_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyAlignment=\"");
				OoxmlComplexType.WriteData(s, _applyAlignment_attr);
				s.Write("\"");
			}
			if ((bool)(_applyProtection_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyProtection=\"");
				OoxmlComplexType.WriteData(s, _applyProtection_attr);
				s.Write("\"");
			}
			if (_numFmtId_attr_is_specified)
			{
				s.Write(" numFmtId=\"");
				OoxmlComplexType.WriteData(s, _numFmtId_attr);
				s.Write("\"");
			}
			if (_fontId_attr_is_specified)
			{
				s.Write(" fontId=\"");
				OoxmlComplexType.WriteData(s, _fontId_attr);
				s.Write("\"");
			}
			if (_fillId_attr_is_specified)
			{
				s.Write(" fillId=\"");
				OoxmlComplexType.WriteData(s, _fillId_attr);
				s.Write("\"");
			}
			if (_borderId_attr_is_specified)
			{
				s.Write(" borderId=\"");
				OoxmlComplexType.WriteData(s, _borderId_attr);
				s.Write("\"");
			}
			if (_xfId_attr_is_specified)
			{
				s.Write(" xfId=\"");
				OoxmlComplexType.WriteData(s, _xfId_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_alignment(s, depth, namespaces);
		}

		public void Write_alignment(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_alignment != null)
			{
				_alignment.Write(s, "alignment", depth + 1, namespaces);
			}
		}
	}
}
