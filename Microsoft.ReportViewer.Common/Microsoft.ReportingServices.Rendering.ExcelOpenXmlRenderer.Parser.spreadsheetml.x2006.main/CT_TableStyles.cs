using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_TableStyles : OoxmlComplexType
	{
		private uint _count_attr;

		private bool _count_attr_is_specified;

		private string _defaultTableStyle_attr;

		private bool _defaultTableStyle_attr_is_specified;

		private string _defaultPivotStyle_attr;

		private bool _defaultPivotStyle_attr_is_specified;

		public uint Count_Attr
		{
			get
			{
				return _count_attr;
			}
			set
			{
				_count_attr = value;
				_count_attr_is_specified = true;
			}
		}

		public bool Count_Attr_Is_Specified
		{
			get
			{
				return _count_attr_is_specified;
			}
			set
			{
				_count_attr_is_specified = value;
			}
		}

		public string DefaultTableStyle_Attr
		{
			get
			{
				return _defaultTableStyle_attr;
			}
			set
			{
				_defaultTableStyle_attr = value;
				_defaultTableStyle_attr_is_specified = (value != null);
			}
		}

		public string DefaultPivotStyle_Attr
		{
			get
			{
				return _defaultPivotStyle_attr;
			}
			set
			{
				_defaultPivotStyle_attr = value;
				_defaultPivotStyle_attr_is_specified = (value != null);
			}
		}

		protected override void InitAttributes()
		{
			_count_attr_is_specified = false;
			_defaultTableStyle_attr_is_specified = false;
			_defaultPivotStyle_attr_is_specified = false;
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
			if (_count_attr_is_specified)
			{
				s.Write(" count=\"");
				OoxmlComplexType.WriteData(s, _count_attr);
				s.Write("\"");
			}
			if (_defaultTableStyle_attr_is_specified)
			{
				s.Write(" defaultTableStyle=\"");
				OoxmlComplexType.WriteData(s, _defaultTableStyle_attr);
				s.Write("\"");
			}
			if (_defaultPivotStyle_attr_is_specified)
			{
				s.Write(" defaultPivotStyle=\"");
				OoxmlComplexType.WriteData(s, _defaultPivotStyle_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
