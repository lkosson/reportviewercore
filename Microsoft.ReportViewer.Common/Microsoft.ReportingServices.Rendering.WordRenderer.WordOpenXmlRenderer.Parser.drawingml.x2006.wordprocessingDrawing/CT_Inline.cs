using Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.wordprocessingDrawing
{
	internal class CT_Inline : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_PositiveSize2D _extent;

		private CT_NonVisualDrawingProps _docPr;

		private CT_GraphicalObject _graphic;

		public CT_PositiveSize2D Extent
		{
			get
			{
				return _extent;
			}
			set
			{
				_extent = value;
			}
		}

		public CT_NonVisualDrawingProps DocPr
		{
			get
			{
				return _docPr;
			}
			set
			{
				_docPr = value;
			}
		}

		public CT_GraphicalObject Graphic
		{
			get
			{
				return _graphic;
			}
			set
			{
				_graphic = value;
			}
		}

		public static string ExtentElementName => "extent";

		public static string DocPrElementName => "docPr";

		public static string GraphicElementName => "graphic";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_extent = new CT_PositiveSize2D();
			_docPr = new CT_NonVisualDrawingProps();
			_graphic = new CT_GraphicalObject();
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
			WriteOpenTag(s, tagName, "wp", namespaces);
		}

		public override void WriteCloseTag(TextWriter s, string tagName)
		{
			s.Write("</wp:");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
		}

		public override void WriteElements(TextWriter s)
		{
			Write_extent(s);
			Write_docPr(s);
			Write_graphic(s);
		}

		public void Write_extent(TextWriter s)
		{
			if (_extent != null)
			{
				_extent.Write(s, "extent");
			}
		}

		public void Write_docPr(TextWriter s)
		{
			if (_docPr != null)
			{
				_docPr.Write(s, "docPr");
			}
		}

		public void Write_graphic(TextWriter s)
		{
			if (_graphic != null)
			{
				_graphic.Write(s, "graphic");
			}
		}
	}
}
