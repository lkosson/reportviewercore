using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_RelativeRect : OoxmlComplexType, IOoxmlComplexType
	{
		private string _r_attr;

		private string _b_attr;

		public string R_Attr
		{
			get
			{
				return _r_attr;
			}
			set
			{
				_r_attr = value;
			}
		}

		public string B_Attr
		{
			get
			{
				return _b_attr;
			}
			set
			{
				_b_attr = value;
			}
		}

		protected override void InitAttributes()
		{
			_r_attr = "0%";
			_b_attr = "0%";
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
			if (_r_attr != "0%")
			{
				s.Write(" r=\"");
				OoxmlComplexType.WriteData(s, _r_attr);
				s.Write("\"");
			}
			if (_b_attr != "0%")
			{
				s.Write(" b=\"");
				OoxmlComplexType.WriteData(s, _b_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s)
		{
		}
	}
}
