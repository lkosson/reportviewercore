using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_StretchInfoProperties : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_RelativeRect _fillRect;

		public CT_RelativeRect FillRect
		{
			get
			{
				return _fillRect;
			}
			set
			{
				_fillRect = value;
			}
		}

		public static string FillRectElementName => "fillRect";

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
		}

		public override void WriteElements(TextWriter s)
		{
			Write_fillRect(s);
		}

		public void Write_fillRect(TextWriter s)
		{
			if (_fillRect != null)
			{
				_fillRect.Write(s, "fillRect");
			}
		}
	}
}
