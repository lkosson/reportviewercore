using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Color : OoxmlComplexType
	{
		private OoxmlBool _auto_attr;

		private double _tint_attr;

		private uint _indexed_attr;

		private bool _indexed_attr_is_specified;

		private string _rgb_attr;

		private bool _rgb_attr_is_specified;

		private uint _theme_attr;

		private bool _theme_attr_is_specified;

		public OoxmlBool Auto_Attr
		{
			get
			{
				return _auto_attr;
			}
			set
			{
				_auto_attr = value;
			}
		}

		public double Tint_Attr
		{
			get
			{
				return _tint_attr;
			}
			set
			{
				_tint_attr = value;
			}
		}

		public uint Indexed_Attr
		{
			get
			{
				return _indexed_attr;
			}
			set
			{
				_indexed_attr = value;
				_indexed_attr_is_specified = true;
			}
		}

		public bool Indexed_Attr_Is_Specified
		{
			get
			{
				return _indexed_attr_is_specified;
			}
			set
			{
				_indexed_attr_is_specified = value;
			}
		}

		public uint Theme_Attr
		{
			get
			{
				return _theme_attr;
			}
			set
			{
				_theme_attr = value;
				_theme_attr_is_specified = true;
			}
		}

		public bool Theme_Attr_Is_Specified
		{
			get
			{
				return _theme_attr_is_specified;
			}
			set
			{
				_theme_attr_is_specified = value;
			}
		}

		public string Rgb_Attr
		{
			get
			{
				return _rgb_attr;
			}
			set
			{
				_rgb_attr = value;
				_rgb_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_auto_attr = OoxmlBool.OoxmlFalse;
			_tint_attr = Convert.ToDouble("0.0", CultureInfo.InvariantCulture);
			_indexed_attr_is_specified = false;
			_rgb_attr_is_specified = false;
			_theme_attr_is_specified = false;
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
			if ((bool)(_auto_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" auto=\"");
				OoxmlComplexType.WriteData(s, _auto_attr);
				s.Write("\"");
			}
			if (_tint_attr != Convert.ToDouble("0.0", CultureInfo.InvariantCulture))
			{
				s.Write(" tint=\"");
				OoxmlComplexType.WriteData(s, _tint_attr);
				s.Write("\"");
			}
			if (_indexed_attr_is_specified)
			{
				s.Write(" indexed=\"");
				OoxmlComplexType.WriteData(s, _indexed_attr);
				s.Write("\"");
			}
			if (_rgb_attr_is_specified)
			{
				s.Write(" rgb=\"");
				OoxmlComplexType.WriteData(s, _rgb_attr);
				s.Write("\"");
			}
			if (_theme_attr_is_specified)
			{
				s.Write(" theme=\"");
				OoxmlComplexType.WriteData(s, _theme_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
