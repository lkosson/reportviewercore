using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_FtrRef : CT_Rel, IOoxmlComplexType, IEG_HdrFtrReferences
	{
		private ST_HdrFtr _type_attr;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_FtrRef;

		public ST_HdrFtr Type_Attr
		{
			get
			{
				return _type_attr;
			}
			set
			{
				_type_attr = value;
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
			base.WriteAttributes(s);
			s.Write(" w:type=\"");
			OoxmlComplexType.WriteData(s, _type_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			base.WriteElements(s);
		}
	}
}
