using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_RPrDefault : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_RPr _rPr;

		public CT_RPr RPr
		{
			get
			{
				return _rPr;
			}
			set
			{
				_rPr = value;
			}
		}

		public static string RPrElementName => "rPr";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
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
		}

		public override void WriteElements(TextWriter s)
		{
			Write_rPr(s);
		}

		public void Write_rPr(TextWriter s)
		{
			if (_rPr != null)
			{
				_rPr.Write(s, "rPr");
			}
		}
	}
}
