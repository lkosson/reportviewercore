using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing
{
	internal class CT_NonVisualDrawingProps : OoxmlComplexType, IOoxmlComplexType
	{
		private uint _id_attr;

		private string _name_attr;

		public uint Id_Attr
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
			s.Write(" id=\"");
			OoxmlComplexType.WriteData(s, _id_attr);
			s.Write("\"");
			s.Write(" name=\"");
			OoxmlComplexType.WriteData(s, _name_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
