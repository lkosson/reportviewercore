using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_Transform2D : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_Point2D _off;

		private CT_PositiveSize2D _ext;

		public CT_Point2D Off
		{
			get
			{
				return _off;
			}
			set
			{
				_off = value;
			}
		}

		public CT_PositiveSize2D Ext
		{
			get
			{
				return _ext;
			}
			set
			{
				_ext = value;
			}
		}

		public static string OffElementName => "off";

		public static string ExtElementName => "ext";

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
			Write_off(s);
			Write_ext(s);
		}

		public void Write_off(TextWriter s)
		{
			if (_off != null)
			{
				_off.Write(s, "off");
			}
		}

		public void Write_ext(TextWriter s)
		{
			if (_ext != null)
			{
				_ext.Write(s, "ext");
			}
		}
	}
}
