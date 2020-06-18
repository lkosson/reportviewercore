using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_BookViews : OoxmlComplexType
	{
		private List<CT_BookView> _workbookView;

		public List<CT_BookView> WorkbookView
		{
			get
			{
				return _workbookView;
			}
			set
			{
				_workbookView = value;
			}
		}

		public static string WorkbookViewElementName => "workbookView";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_workbookView = new List<CT_BookView>();
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
			Write_workbookView(s, depth, namespaces);
		}

		public void Write_workbookView(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_workbookView == null)
			{
				return;
			}
			foreach (CT_BookView item in _workbookView)
			{
				item?.Write(s, "workbookView", depth + 1, namespaces);
			}
		}
	}
}
