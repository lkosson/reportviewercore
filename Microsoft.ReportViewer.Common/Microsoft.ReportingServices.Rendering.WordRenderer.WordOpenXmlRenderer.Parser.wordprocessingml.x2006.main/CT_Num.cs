using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Num : OoxmlComplexType, IOoxmlComplexType
	{
		private int _numId_attr;

		private CT_DecimalNumber _abstractNumId;

		public int NumId_Attr
		{
			get
			{
				return _numId_attr;
			}
			set
			{
				_numId_attr = value;
			}
		}

		public CT_DecimalNumber AbstractNumId
		{
			get
			{
				return _abstractNumId;
			}
			set
			{
				_abstractNumId = value;
			}
		}

		public static string AbstractNumIdElementName => "abstractNumId";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_abstractNumId = new CT_DecimalNumber();
		}

		protected override void InitCollections()
		{
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
			s.Write(" w:numId=\"");
			OoxmlComplexType.WriteData(s, _numId_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_abstractNumId(s);
		}

		public void Write_abstractNumId(TextWriter s)
		{
			if (_abstractNumId != null)
			{
				_abstractNumId.Write(s, "abstractNumId");
			}
		}
	}
}
