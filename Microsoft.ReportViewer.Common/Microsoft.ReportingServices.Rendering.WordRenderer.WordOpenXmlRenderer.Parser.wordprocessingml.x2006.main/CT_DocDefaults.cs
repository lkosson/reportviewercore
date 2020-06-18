using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_DocDefaults : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_RPrDefault _rPrDefault;

		public CT_RPrDefault RPrDefault
		{
			get
			{
				return _rPrDefault;
			}
			set
			{
				_rPrDefault = value;
			}
		}

		public static string RPrDefaultElementName => "rPrDefault";

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
			Write_rPrDefault(s);
		}

		public void Write_rPrDefault(TextWriter s)
		{
			if (_rPrDefault != null)
			{
				_rPrDefault.Write(s, "rPrDefault");
			}
		}
	}
}
