using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Col : OoxmlComplexType
	{
		private uint _min_attr;

		private uint _max_attr;

		private uint _style_attr;

		private OoxmlBool _hidden_attr;

		private OoxmlBool _bestFit_attr;

		private OoxmlBool _customWidth_attr;

		private OoxmlBool _phonetic_attr;

		private byte _outlineLevel_attr;

		private OoxmlBool _collapsed_attr;

		private double _width_attr;

		private bool _width_attr_is_specified;

		public uint Min_Attr
		{
			get
			{
				return _min_attr;
			}
			set
			{
				_min_attr = value;
			}
		}

		public uint Max_Attr
		{
			get
			{
				return _max_attr;
			}
			set
			{
				_max_attr = value;
			}
		}

		public uint Style_Attr
		{
			get
			{
				return _style_attr;
			}
			set
			{
				_style_attr = value;
			}
		}

		public OoxmlBool Hidden_Attr
		{
			get
			{
				return _hidden_attr;
			}
			set
			{
				_hidden_attr = value;
			}
		}

		public OoxmlBool BestFit_Attr
		{
			get
			{
				return _bestFit_attr;
			}
			set
			{
				_bestFit_attr = value;
			}
		}

		public OoxmlBool CustomWidth_Attr
		{
			get
			{
				return _customWidth_attr;
			}
			set
			{
				_customWidth_attr = value;
			}
		}

		public OoxmlBool Phonetic_Attr
		{
			get
			{
				return _phonetic_attr;
			}
			set
			{
				_phonetic_attr = value;
			}
		}

		public byte OutlineLevel_Attr
		{
			get
			{
				return _outlineLevel_attr;
			}
			set
			{
				_outlineLevel_attr = value;
			}
		}

		public OoxmlBool Collapsed_Attr
		{
			get
			{
				return _collapsed_attr;
			}
			set
			{
				_collapsed_attr = value;
			}
		}

		public double Width_Attr
		{
			get
			{
				return _width_attr;
			}
			set
			{
				_width_attr = value;
				_width_attr_is_specified = true;
			}
		}

		public bool Width_Attr_Is_Specified
		{
			get
			{
				return _width_attr_is_specified;
			}
			set
			{
				_width_attr_is_specified = value;
			}
		}

		protected override void InitAttributes()
		{
			_style_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_hidden_attr = OoxmlBool.OoxmlFalse;
			_bestFit_attr = OoxmlBool.OoxmlFalse;
			_customWidth_attr = OoxmlBool.OoxmlFalse;
			_phonetic_attr = OoxmlBool.OoxmlFalse;
			_outlineLevel_attr = Convert.ToByte("0", CultureInfo.InvariantCulture);
			_collapsed_attr = OoxmlBool.OoxmlFalse;
			_width_attr_is_specified = false;
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
			s.Write(" min=\"");
			OoxmlComplexType.WriteData(s, _min_attr);
			s.Write("\"");
			s.Write(" max=\"");
			OoxmlComplexType.WriteData(s, _max_attr);
			s.Write("\"");
			if (_style_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" style=\"");
				OoxmlComplexType.WriteData(s, _style_attr);
				s.Write("\"");
			}
			if ((bool)(_hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, _hidden_attr);
				s.Write("\"");
			}
			if ((bool)(_bestFit_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" bestFit=\"");
				OoxmlComplexType.WriteData(s, _bestFit_attr);
				s.Write("\"");
			}
			if ((bool)(_customWidth_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" customWidth=\"");
				OoxmlComplexType.WriteData(s, _customWidth_attr);
				s.Write("\"");
			}
			if ((bool)(_phonetic_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" phonetic=\"");
				OoxmlComplexType.WriteData(s, _phonetic_attr);
				s.Write("\"");
			}
			if (_outlineLevel_attr != Convert.ToByte("0", CultureInfo.InvariantCulture))
			{
				s.Write(" outlineLevel=\"");
				OoxmlComplexType.WriteData(s, _outlineLevel_attr);
				s.Write("\"");
			}
			if ((bool)(_collapsed_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" collapsed=\"");
				OoxmlComplexType.WriteData(s, _collapsed_attr);
				s.Write("\"");
			}
			if (_width_attr_is_specified)
			{
				s.Write(" width=\"");
				OoxmlComplexType.WriteData(s, _width_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
