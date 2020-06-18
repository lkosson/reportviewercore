using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_AnchorClientData : OoxmlComplexType
	{
		private OoxmlBool _fLocksWithSheet_attr;

		private OoxmlBool _fPrintsWithSheet_attr;

		public OoxmlBool FLocksWithSheet_Attr
		{
			get
			{
				return _fLocksWithSheet_attr;
			}
			set
			{
				_fLocksWithSheet_attr = value;
			}
		}

		public OoxmlBool FPrintsWithSheet_Attr
		{
			get
			{
				return _fPrintsWithSheet_attr;
			}
			set
			{
				_fPrintsWithSheet_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			_fLocksWithSheet_attr = OoxmlBool.OoxmlTrue;
			_fPrintsWithSheet_attr = OoxmlBool.OoxmlTrue;
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
			if ((bool)(_fLocksWithSheet_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" fLocksWithSheet=\"");
				OoxmlComplexType.WriteData(s, _fLocksWithSheet_attr);
				s.Write("\"");
			}
			if ((bool)(_fPrintsWithSheet_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" fPrintsWithSheet=\"");
				OoxmlComplexType.WriteData(s, _fPrintsWithSheet_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
