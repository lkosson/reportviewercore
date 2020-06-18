using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_RelativeRect : OoxmlComplexType
	{
		private string _l_attr;

		private string _t_attr;

		private string _r_attr;

		private string _b_attr;

		public string L_Attr
		{
			get
			{
				return _l_attr;
			}
			set
			{
				_l_attr = value;
			}
		}

		public string T_Attr
		{
			get
			{
				return _t_attr;
			}
			set
			{
				_t_attr = value;
			}
		}

		public string R_Attr
		{
			get
			{
				return _r_attr;
			}
			set
			{
				_r_attr = value;
			}
		}

		public string B_Attr
		{
			get
			{
				return _b_attr;
			}
			set
			{
				_b_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			_l_attr = Convert.ToString("0%", CultureInfo.InvariantCulture);
			_t_attr = Convert.ToString("0%", CultureInfo.InvariantCulture);
			_r_attr = Convert.ToString("0%", CultureInfo.InvariantCulture);
			_b_attr = Convert.ToString("0%", CultureInfo.InvariantCulture);
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/main");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_l_attr != Convert.ToString("0%", CultureInfo.InvariantCulture))
			{
				s.Write(" l=\"");
				OoxmlComplexType.WriteData(s, _l_attr);
				s.Write("\"");
			}
			if (_t_attr != Convert.ToString("0%", CultureInfo.InvariantCulture))
			{
				s.Write(" t=\"");
				OoxmlComplexType.WriteData(s, _t_attr);
				s.Write("\"");
			}
			if (_r_attr != Convert.ToString("0%", CultureInfo.InvariantCulture))
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, _r_attr);
				s.Write("\"");
			}
			if (_b_attr != Convert.ToString("0%", CultureInfo.InvariantCulture))
			{
				s.Write(" b=\"");
				OoxmlComplexType.WriteData(s, _b_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
