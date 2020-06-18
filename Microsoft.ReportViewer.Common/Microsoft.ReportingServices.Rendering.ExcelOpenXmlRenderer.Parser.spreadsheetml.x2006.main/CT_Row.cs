using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Row : OoxmlComplexType
	{
		private uint _s_attr;

		private OoxmlBool _customFormat_attr;

		private OoxmlBool _hidden_attr;

		private byte _outlineLevel_attr;

		private OoxmlBool _collapsed_attr;

		private OoxmlBool _thickTop_attr;

		private OoxmlBool _thickBot_attr;

		private OoxmlBool _ph_attr;

		private uint _r_attr;

		private bool _r_attr_is_specified;

		private List<string> _spans_attr;

		private bool _spans_attr_is_specified;

		private double _ht_attr;

		private bool _ht_attr_is_specified;

		private OoxmlBool _customHeight_attr;

		private bool _customHeight_attr_is_specified;

		private List<CT_Cell> _c;

		public uint S_Attr
		{
			get
			{
				return _s_attr;
			}
			set
			{
				_s_attr = value;
			}
		}

		public OoxmlBool CustomFormat_Attr
		{
			get
			{
				return _customFormat_attr;
			}
			set
			{
				_customFormat_attr = value;
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

		public OoxmlBool ThickTop_Attr
		{
			get
			{
				return _thickTop_attr;
			}
			set
			{
				_thickTop_attr = value;
			}
		}

		public OoxmlBool ThickBot_Attr
		{
			get
			{
				return _thickBot_attr;
			}
			set
			{
				_thickBot_attr = value;
			}
		}

		public OoxmlBool Ph_Attr
		{
			get
			{
				return _ph_attr;
			}
			set
			{
				_ph_attr = value;
			}
		}

		public uint R_Attr
		{
			get
			{
				return _r_attr;
			}
			set
			{
				_r_attr = value;
				_r_attr_is_specified = true;
			}
		}

		public bool R_Attr_Is_Specified
		{
			get
			{
				return _r_attr_is_specified;
			}
			set
			{
				_r_attr_is_specified = value;
			}
		}

		public double Ht_Attr
		{
			get
			{
				return _ht_attr;
			}
			set
			{
				_ht_attr = value;
				_ht_attr_is_specified = true;
			}
		}

		public bool Ht_Attr_Is_Specified
		{
			get
			{
				return _ht_attr_is_specified;
			}
			set
			{
				_ht_attr_is_specified = value;
			}
		}

		public OoxmlBool CustomHeight_Attr
		{
			get
			{
				return _customHeight_attr;
			}
			set
			{
				_customHeight_attr = value;
				_customHeight_attr_is_specified = true;
			}
		}

		public bool CustomHeight_Attr_Is_Specified
		{
			get
			{
				return _customHeight_attr_is_specified;
			}
			set
			{
				_customHeight_attr_is_specified = value;
			}
		}

		public List<string> Spans_Attr
		{
			get
			{
				return _spans_attr;
			}
			set
			{
				_spans_attr = value;
				_spans_attr_is_specified = (value != null);
			}
		}

		public List<CT_Cell> C
		{
			get
			{
				return _c;
			}
			set
			{
				_c = value;
			}
		}

		public static string CElementName => "c";

		protected override void InitAttributes()
		{
			_s_attr = Convert.ToUInt32("0", CultureInfo.InvariantCulture);
			_customFormat_attr = OoxmlBool.OoxmlFalse;
			_hidden_attr = OoxmlBool.OoxmlFalse;
			_outlineLevel_attr = Convert.ToByte("0", CultureInfo.InvariantCulture);
			_collapsed_attr = OoxmlBool.OoxmlFalse;
			_thickTop_attr = OoxmlBool.OoxmlFalse;
			_thickBot_attr = OoxmlBool.OoxmlFalse;
			_ph_attr = OoxmlBool.OoxmlFalse;
			_r_attr_is_specified = false;
			_spans_attr_is_specified = false;
			_ht_attr_is_specified = false;
			_customHeight_attr_is_specified = false;
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_c = new List<CT_Cell>();
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
			if (_s_attr != Convert.ToUInt32("0", CultureInfo.InvariantCulture))
			{
				s.Write(" s=\"");
				OoxmlComplexType.WriteData(s, _s_attr);
				s.Write("\"");
			}
			if ((bool)(_customFormat_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" customFormat=\"");
				OoxmlComplexType.WriteData(s, _customFormat_attr);
				s.Write("\"");
			}
			if ((bool)(_hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, _hidden_attr);
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
			if ((bool)(_thickTop_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickTop=\"");
				OoxmlComplexType.WriteData(s, _thickTop_attr);
				s.Write("\"");
			}
			if ((bool)(_thickBot_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" thickBot=\"");
				OoxmlComplexType.WriteData(s, _thickBot_attr);
				s.Write("\"");
			}
			if ((bool)(_ph_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" ph=\"");
				OoxmlComplexType.WriteData(s, _ph_attr);
				s.Write("\"");
			}
			if (_r_attr_is_specified)
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, _r_attr);
				s.Write("\"");
			}
			if (_ht_attr_is_specified)
			{
				s.Write(" ht=\"");
				OoxmlComplexType.WriteData(s, _ht_attr);
				s.Write("\"");
			}
			if (_customHeight_attr_is_specified)
			{
				s.Write(" customHeight=\"");
				OoxmlComplexType.WriteData(s, _customHeight_attr);
				s.Write("\"");
			}
			if (_spans_attr_is_specified)
			{
				s.Write(" spans=\"");
				for (int i = 0; i < _spans_attr.Count - 1; i++)
				{
					OoxmlComplexType.WriteData(s, _spans_attr[i]);
					s.Write(" ");
				}
				OoxmlComplexType.WriteData(s, _spans_attr[_spans_attr.Count - 1]);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_c(s, depth, namespaces);
		}

		public void Write_c(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_c == null)
			{
				return;
			}
			foreach (CT_Cell item in _c)
			{
				item?.Write(s, "c", depth + 1, namespaces);
			}
		}
	}
}
