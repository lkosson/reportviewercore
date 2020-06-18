using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing
{
	internal class CT_PositiveSize2D : OoxmlComplexType, IOoxmlComplexType
	{
		private long _cx_attr;

		private long _cy_attr;

		public long Cx_Attr
		{
			get
			{
				return _cx_attr;
			}
			set
			{
				_cx_attr = value;
			}
		}

		public long Cy_Attr
		{
			get
			{
				return _cy_attr;
			}
			set
			{
				_cy_attr = value;
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
			WriteEmptyTag(s, tagName, "wp");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "wp", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</wp:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" cx=\"");
			OoxmlComplexType.WriteData(s, _cx_attr);
			s.Write("\"");
			s.Write(" cy=\"");
			OoxmlComplexType.WriteData(s, _cy_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
