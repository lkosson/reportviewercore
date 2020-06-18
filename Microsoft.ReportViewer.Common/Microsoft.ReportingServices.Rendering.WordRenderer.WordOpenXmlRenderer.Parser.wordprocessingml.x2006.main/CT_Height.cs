using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Height : OoxmlComplexType, IOoxmlComplexType
	{
		private string _val_attr;

		private ST_HeightRule _hRule_attr;

		public string Val_Attr
		{
			get
			{
				return _val_attr;
			}
			set
			{
				_val_attr = value;
			}
		}

		public ST_HeightRule HRule_Attr
		{
			get
			{
				return _hRule_attr;
			}
			set
			{
				_hRule_attr = value;
			}
		}

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
			WriteEmptyTag(s, tagName, "w");
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
			s.Write(" w:val=\"");
			OoxmlComplexType.WriteData(s, _val_attr);
			s.Write("\"");
			s.Write(" w:hRule=\"");
			OoxmlComplexType.WriteData(s, _hRule_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
