using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main
{
	internal class CT_OutlinePr : OoxmlComplexType
	{
		private OoxmlBool _applyStyles_attr;

		private OoxmlBool _summaryBelow_attr;

		private OoxmlBool _summaryRight_attr;

		private OoxmlBool _showOutlineSymbols_attr;

		public OoxmlBool ApplyStyles_Attr
		{
			get
			{
				return _applyStyles_attr;
			}
			set
			{
				_applyStyles_attr = value;
			}
		}

		public OoxmlBool SummaryBelow_Attr
		{
			get
			{
				return _summaryBelow_attr;
			}
			set
			{
				_summaryBelow_attr = value;
			}
		}

		public OoxmlBool SummaryRight_Attr
		{
			get
			{
				return _summaryRight_attr;
			}
			set
			{
				_summaryRight_attr = value;
			}
		}

		public OoxmlBool ShowOutlineSymbols_Attr
		{
			get
			{
				return _showOutlineSymbols_attr;
			}
			set
			{
				_showOutlineSymbols_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			_applyStyles_attr = OoxmlBool.OoxmlFalse;
			_summaryBelow_attr = OoxmlBool.OoxmlTrue;
			_summaryRight_attr = OoxmlBool.OoxmlTrue;
			_showOutlineSymbols_attr = OoxmlBool.OoxmlTrue;
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
			if ((bool)(_applyStyles_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" applyStyles=\"");
				OoxmlComplexType.WriteData(s, _applyStyles_attr);
				s.Write("\"");
			}
			if ((bool)(_summaryBelow_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" summaryBelow=\"");
				OoxmlComplexType.WriteData(s, _summaryBelow_attr);
				s.Write("\"");
			}
			if ((bool)(_summaryRight_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" summaryRight=\"");
				OoxmlComplexType.WriteData(s, _summaryRight_attr);
				s.Write("\"");
			}
			if ((bool)(_showOutlineSymbols_attr != OoxmlBool.OoxmlTrue))
			{
				s.Write(" showOutlineSymbols=\"");
				OoxmlComplexType.WriteData(s, _showOutlineSymbols_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
		}
	}
}
