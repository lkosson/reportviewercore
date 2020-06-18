using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_NonVisualDrawingProps : OoxmlComplexType
	{
		private uint _id_attr;

		private string _name_attr;

		private string _descr_attr;

		private OoxmlBool _hidden_attr;

		private string _title_attr;

		private CT_Hyperlink _hlinkClick;

		private CT_Hyperlink _hlinkHover;

		public uint Id_Attr
		{
			get
			{
				return _id_attr;
			}
			set
			{
				_id_attr = value;
			}
		}

		public string Name_Attr
		{
			get
			{
				return _name_attr;
			}
			set
			{
				_name_attr = value;
			}
		}

		public string Descr_Attr
		{
			get
			{
				return _descr_attr;
			}
			set
			{
				_descr_attr = value;
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

		public string Title_Attr
		{
			get
			{
				return _title_attr;
			}
			set
			{
				_title_attr = value;
			}
		}

		public CT_Hyperlink HlinkClick
		{
			get
			{
				return _hlinkClick;
			}
			set
			{
				_hlinkClick = value;
			}
		}

		public CT_Hyperlink HlinkHover
		{
			get
			{
				return _hlinkHover;
			}
			set
			{
				_hlinkHover = value;
			}
		}

		public static string HlinkClickElementName => "hlinkClick";

		public static string HlinkHoverElementName => "hlinkHover";

		protected override void InitAttributes()
		{
			_descr_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_hidden_attr = OoxmlBool.OoxmlFalse;
			_title_attr = Convert.ToString("", CultureInfo.InvariantCulture);
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
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
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" id=\"");
			OoxmlComplexType.WriteData(s, _id_attr);
			s.Write("\"");
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, _name_attr);
			s.Write("\"");
			if (_descr_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" descr=\"");
				OoxmlComplexType.WriteData(s, _descr_attr);
				s.Write("\"");
			}
			if ((bool)(_hidden_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" hidden=\"");
				OoxmlComplexType.WriteData(s, _hidden_attr);
				s.Write("\"");
			}
			if (_title_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" title=\"");
				OoxmlComplexType.WriteData(s, _title_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_hlinkClick(s, depth, namespaces);
			Write_hlinkHover(s, depth, namespaces);
		}

		public void Write_hlinkClick(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_hlinkClick != null)
			{
				_hlinkClick.Write(s, "hlinkClick", depth + 1, namespaces);
			}
		}

		public void Write_hlinkHover(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_hlinkHover != null)
			{
				_hlinkHover.Write(s, "hlinkHover", depth + 1, namespaces);
			}
		}
	}
}
