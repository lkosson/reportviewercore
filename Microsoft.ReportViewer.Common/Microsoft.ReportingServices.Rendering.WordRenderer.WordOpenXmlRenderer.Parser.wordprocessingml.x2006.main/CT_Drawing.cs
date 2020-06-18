using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class CT_Drawing : OoxmlComplexType, IOoxmlComplexType, IEG_RunInnerContent
	{
		public enum ChoiceBucket_0
		{
			anchor,
			inline
		}

		private CT_Inline _inline;

		private ChoiceBucket_0 _choice_0;

		public override GeneratedType GroupInterfaceType => GeneratedType.CT_Drawing;

		public CT_Inline Inline
		{
			get
			{
				return _inline;
			}
			set
			{
				_inline = value;
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

		public static string InlineElementName => "inline";

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
			Write_inline(s);
		}

		public void Write_inline(TextWriter s)
		{
			if (_choice_0 == ChoiceBucket_0.inline && _inline != null)
			{
				_inline.Write(s, "inline");
			}
		}
	}
}
