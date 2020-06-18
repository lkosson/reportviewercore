using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_PageSz : OoxmlComplexType, IOoxmlComplexType
	{
		private string _w_attr;

		private string _h_attr;

		private ST_Orientation _orient_attr;

		public string W_Attr
		{
			get
			{
				return _w_attr;
			}
			set
			{
				_w_attr = value;
			}
		}

		public string H_Attr
		{
			get
			{
				return _h_attr;
			}
			set
			{
				_h_attr = value;
			}
		}

		public ST_Orientation Orient_Attr
		{
			get
			{
				return _orient_attr;
			}
			set
			{
				_orient_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			_orient_attr = ST_Orientation._default;
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
			s.Write(" w:w=\"");
			OoxmlComplexType.WriteData(s, _w_attr);
			s.Write("\"");
			s.Write(" w:h=\"");
			OoxmlComplexType.WriteData(s, _h_attr);
			s.Write("\"");
			if (_orient_attr != ST_Orientation._default)
			{
				s.Write(" w:orient=\"");
				OoxmlComplexType.WriteData(s, _orient_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
