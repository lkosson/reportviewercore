using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Styles : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_DocDefaults _docDefaults;

		private List<CT_Style> _style;

		public CT_DocDefaults DocDefaults
		{
			get
			{
				return _docDefaults;
			}
			set
			{
				_docDefaults = value;
			}
		}

		public List<CT_Style> Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		public static string DocDefaultsElementName => "docDefaults";

		public static string StyleElementName => "style";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
		}

		protected override void InitCollections()
		{
			_style = new List<CT_Style>();
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
			Write_docDefaults(s);
			Write_style(s);
		}

		public void Write_docDefaults(TextWriter s)
		{
			if (_docDefaults != null)
			{
				_docDefaults.Write(s, "docDefaults");
			}
		}

		public void Write_style(TextWriter s)
		{
			if (_style == null)
			{
				return;
			}
			foreach (CT_Style item in _style)
			{
				item?.Write(s, "style");
			}
		}
	}
}
