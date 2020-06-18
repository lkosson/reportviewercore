using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Sheet : OoxmlComplexType
	{
		private string _name_attr;

		private uint _sheetId_attr;

		private ST_SheetState _state_attr;

		private string _id_attr;

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

		public uint SheetId_Attr
		{
			get
			{
				return _sheetId_attr;
			}
			set
			{
				_sheetId_attr = value;
			}
		}

		public ST_SheetState State_Attr
		{
			get
			{
				return _state_attr;
			}
			set
			{
				_state_attr = value;
			}
		}

		public string Id_Attr
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

		protected override void InitAttributes()
		{
			_state_attr = ST_SheetState.visible;
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
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, _name_attr);
			s.Write("\"");
			s.Write(" sheetId=\"");
			OoxmlComplexType.WriteData(s, _sheetId_attr);
			s.Write("\"");
			s.Write(" r:id=\"");
			OoxmlComplexType.WriteData(s, _id_attr);
			s.Write("\"");
			if (_state_attr != ST_SheetState.visible)
			{
				s.Write(" state=\"");
				OoxmlComplexType.WriteData(s, _state_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
