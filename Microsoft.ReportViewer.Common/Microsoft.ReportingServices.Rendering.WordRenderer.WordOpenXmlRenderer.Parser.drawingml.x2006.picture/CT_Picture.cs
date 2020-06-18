using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.picture
{
	internal class CT_Picture : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_PictureNonVisual _nvPicPr;

		private CT_BlipFillProperties _blipFill;

		private CT_ShapeProperties _spPr;

		public CT_PictureNonVisual NvPicPr
		{
			get
			{
				return _nvPicPr;
			}
			set
			{
				_nvPicPr = value;
			}
		}

		public CT_BlipFillProperties BlipFill
		{
			get
			{
				return _blipFill;
			}
			set
			{
				_blipFill = value;
			}
		}

		public CT_ShapeProperties SpPr
		{
			get
			{
				return _spPr;
			}
			set
			{
				_spPr = value;
			}
		}

		public static string NvPicPrElementName => "nvPicPr";

		public static string BlipFillElementName => "blipFill";

		public static string SpPrElementName => "spPr";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_nvPicPr = new CT_PictureNonVisual();
			_blipFill = new CT_BlipFillProperties();
			_spPr = new CT_ShapeProperties();
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
			Write_nvPicPr(s);
			Write_blipFill(s);
			Write_spPr(s);
		}

		public void Write_nvPicPr(TextWriter s)
		{
			if (_nvPicPr != null)
			{
				_nvPicPr.Write(s, "nvPicPr");
			}
		}

		public void Write_blipFill(TextWriter s)
		{
			if (_blipFill != null)
			{
				_blipFill.Write(s, "blipFill");
			}
		}

		public void Write_spPr(TextWriter s)
		{
			if (_spPr != null)
			{
				_spPr.Write(s, "spPr");
			}
		}
	}
}
