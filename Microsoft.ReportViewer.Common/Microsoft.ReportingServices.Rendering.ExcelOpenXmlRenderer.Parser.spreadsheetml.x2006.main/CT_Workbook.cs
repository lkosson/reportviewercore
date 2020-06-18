using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_Workbook : OoxmlComplexType
	{
		private CT_FileVersion _fileVersion;

		private CT_WorkbookPr _workbookPr;

		private CT_BookViews _bookViews;

		private CT_Sheets _sheets;

		private CT_DefinedNames _definedNames;

		private CT_CalcPr _calcPr;

		public CT_FileVersion FileVersion
		{
			get
			{
				return _fileVersion;
			}
			set
			{
				_fileVersion = value;
			}
		}

		public CT_WorkbookPr WorkbookPr
		{
			get
			{
				return _workbookPr;
			}
			set
			{
				_workbookPr = value;
			}
		}

		public CT_BookViews BookViews
		{
			get
			{
				return _bookViews;
			}
			set
			{
				_bookViews = value;
			}
		}

		public CT_Sheets Sheets
		{
			get
			{
				return _sheets;
			}
			set
			{
				_sheets = value;
			}
		}

		public CT_DefinedNames DefinedNames
		{
			get
			{
				return _definedNames;
			}
			set
			{
				_definedNames = value;
			}
		}

		public CT_CalcPr CalcPr
		{
			get
			{
				return _calcPr;
			}
			set
			{
				_calcPr = value;
			}
		}

		public static string FileVersionElementName => "fileVersion";

		public static string WorkbookPrElementName => "workbookPr";

		public static string BookViewsElementName => "bookViews";

		public static string SheetsElementName => "sheets";

		public static string DefinedNamesElementName => "definedNames";

		public static string CalcPrElementName => "calcPr";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_sheets = new CT_Sheets();
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
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_fileVersion(s, depth, namespaces);
			Write_workbookPr(s, depth, namespaces);
			Write_bookViews(s, depth, namespaces);
			Write_sheets(s, depth, namespaces);
			Write_definedNames(s, depth, namespaces);
			Write_calcPr(s, depth, namespaces);
		}

		public void Write_fileVersion(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_fileVersion != null)
			{
				_fileVersion.Write(s, "fileVersion", depth + 1, namespaces);
			}
		}

		public void Write_workbookPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_workbookPr != null)
			{
				_workbookPr.Write(s, "workbookPr", depth + 1, namespaces);
			}
		}

		public void Write_bookViews(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_bookViews != null)
			{
				_bookViews.Write(s, "bookViews", depth + 1, namespaces);
			}
		}

		public void Write_sheets(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_sheets != null)
			{
				_sheets.Write(s, "sheets", depth + 1, namespaces);
			}
		}

		public void Write_definedNames(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_definedNames != null)
			{
				_definedNames.Write(s, "definedNames", depth + 1, namespaces);
			}
		}

		public void Write_calcPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_calcPr != null)
			{
				_calcPr.Write(s, "calcPr", depth + 1, namespaces);
			}
		}
	}
}
