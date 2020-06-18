using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_PresetGeometry2D : OoxmlComplexType, IOoxmlComplexType
	{
		private string _prst_attr;

		private CT_GeomGuideList _avLst;

		public string Prst_Attr
		{
			get
			{
				return _prst_attr;
			}
			set
			{
				_prst_attr = value;
			}
		}

		public CT_GeomGuideList AvLst
		{
			get
			{
				return _avLst;
			}
			set
			{
				_avLst = value;
			}
		}

		public static string AvLstElementName => "avLst";

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
			s.Write(" prst=\"");
			OoxmlComplexType.WriteData(s, _prst_attr);
			s.Write("\"");
		}

		public override void WriteElements(TextWriter s)
		{
			Write_avLst(s);
		}

		public void Write_avLst(TextWriter s)
		{
			if (_avLst != null)
			{
				_avLst.Write(s, "avLst");
			}
		}
	}
}
