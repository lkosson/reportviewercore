using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_BlipFillProperties : OoxmlComplexType, IOoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			tile,
			stretch
		}

		private CT_Blip _blip;

		private CT_RelativeRect _srcRect;

		private CT_StretchInfoProperties _stretch;

		private ChoiceBucket_0 _choice_0;

		public CT_Blip Blip
		{
			get
			{
				return _blip;
			}
			set
			{
				_blip = value;
			}
		}

		public CT_RelativeRect SrcRect
		{
			get
			{
				return _srcRect;
			}
			set
			{
				_srcRect = value;
			}
		}

		public CT_StretchInfoProperties Stretch
		{
			get
			{
				return _stretch;
			}
			set
			{
				_stretch = value;
			}
		}

		public ChoiceBucket_0 Choice_0
		{
			get
			{
				return _choice_0;
			}
			set
			{
				_choice_0 = value;
			}
		}

		public static string BlipElementName => "blip";

		public static string SrcRectElementName => "srcRect";

		public static string StretchElementName => "stretch";

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
			WriteOpenTag(s, tagName, "pic", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</pic:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			Write_blip(s);
			Write_srcRect(s);
			Write_stretch(s);
		}

		public void Write_blip(TextWriter s)
		{
			if (_blip != null)
			{
				_blip.Write(s, "blip");
			}
		}

		public void Write_srcRect(TextWriter s)
		{
			if (_srcRect != null)
			{
				_srcRect.Write(s, "srcRect");
			}
		}

		public void Write_stretch(TextWriter s)
		{
			if (_choice_0 == ChoiceBucket_0.stretch && _stretch != null)
			{
				_stretch.Write(s, "stretch");
			}
		}
	}
}
