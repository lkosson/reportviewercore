using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_TblGridBase : OoxmlComplexType, IOoxmlComplexType
	{
		private List<CT_TblGridCol> _gridCol;

		public List<CT_TblGridCol> GridCol
		{
			get
			{
				return _gridCol;
			}
			set
			{
				_gridCol = value;
			}
		}

		public static string GridColElementName => "gridCol";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_gridCol = new List<CT_TblGridCol>();
		}

		public override void Write(TextWriter s, string tagName)
		{
			WriteOpenTag(s, tagName, null);
			WriteElements(s);
			WriteCloseTag(s, tagName);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "w", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</w:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			Write_gridCol(s);
		}

		public void Write_gridCol(TextWriter s)
		{
			if (_gridCol == null)
			{
				return;
			}
			foreach (CT_TblGridCol item in _gridCol)
			{
				item?.Write(s, "gridCol");
			}
		}
	}
}
