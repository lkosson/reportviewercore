using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Rst : OoxmlComplexType
	{
		private string _t;

		private List<CT_RElt> _r;

		public string T
		{
			get
			{
				return _t;
			}
			set
			{
				_t = value;
			}
		}

		public List<CT_RElt> R
		{
			get
			{
				return _r;
			}
			set
			{
				_r = value;
			}
		}

		public static string RElementName => "r";

		public static string TElementName => "t";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_r = new List<CT_RElt>();
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
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_t(s, depth, namespaces);
			Write_r(s, depth, namespaces);
		}

		public void Write_r(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_r == null)
			{
				return;
			}
			foreach (CT_RElt item in _r)
			{
				item?.Write(s, "r", depth + 1, namespaces);
			}
		}

		public void Write_t(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_t != null)
			{
				OoxmlComplexType.WriteRawTag(s, depth, namespaces, "t", preserveWhitespace: true, "http://schemas.openxmlformats.org/spreadsheetml/2006/main", _t);
			}
		}
	}
}
