using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_PictureNonVisual : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_NonVisualDrawingProps _cNvPr;

		private CT_NonVisualPictureProperties _cNvPicPr;

		public CT_NonVisualDrawingProps CNvPr
		{
			get
			{
				return _cNvPr;
			}
			set
			{
				_cNvPr = value;
			}
		}

		public CT_NonVisualPictureProperties CNvPicPr
		{
			get
			{
				return _cNvPicPr;
			}
			set
			{
				_cNvPicPr = value;
			}
		}

		public static string CNvPrElementName => "cNvPr";

		public static string CNvPicPrElementName => "cNvPicPr";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_cNvPr = new CT_NonVisualDrawingProps();
			_cNvPicPr = new CT_NonVisualPictureProperties();
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
			Write_cNvPr(s);
			Write_cNvPicPr(s);
		}

		public void Write_cNvPr(TextWriter s)
		{
			if (_cNvPr != null)
			{
				_cNvPr.Write(s, "cNvPr");
			}
		}

		public void Write_cNvPicPr(TextWriter s)
		{
			if (_cNvPicPr != null)
			{
				_cNvPicPr.Write(s, "cNvPicPr");
			}
		}
	}
}
