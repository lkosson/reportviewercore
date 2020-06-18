using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Point2D : OoxmlComplexType, IOoxmlComplexType
	{
		private long _x_attr;

		private long _y_attr;

		public long X_Attr
		{
			get
			{
				return _x_attr;
			}
			set
			{
				_x_attr = value;
			}
		}

		public long Y_Attr
		{
			get
			{
				return _y_attr;
			}
			set
			{
				_y_attr = value;
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
			WriteEmptyTag(s, tagName, "a");
		}

		public override void WriteOpenTag(TextWriter s, string tagName, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, "a", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</a:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			s.Write(" x=\"");
			OoxmlComplexType.WriteData(s, _x_attr);
			s.Write("\"");
			s.Write(" y=\"");
			OoxmlComplexType.WriteData(s, _y_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
