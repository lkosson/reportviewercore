using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_Picture : OoxmlComplexType
	{
		private string _macro_attr;

		private OoxmlBool _fPublished_attr;

		private CT_PictureNonVisual _nvPicPr;

		private CT_BlipFillProperties _blipFill;

		private CT_ShapeProperties _spPr;

		public string Macro_Attr
		{
			get
			{
				return _macro_attr;
			}
			set
			{
				_macro_attr = value;
			}
		}

		public OoxmlBool FPublished_Attr
		{
			get
			{
				return _fPublished_attr;
			}
			set
			{
				_fPublished_attr = value;
			}
		}

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
			_macro_attr = Convert.ToString("", CultureInfo.InvariantCulture);
			_fPublished_attr = OoxmlBool.OoxmlFalse;
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

		public override void WriteAsRoot(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: true);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void Write(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			WriteOpenTag(s, tagName, depth, namespaces, root: false);
			WriteElements(s, depth, namespaces);
			WriteCloseTag(s, tagName, depth, namespaces);
		}

		public override void WriteOpenTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces, bool root)
		{
			s.Write("<");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			WriteAttributes(s);
			if (root)
			{
				foreach (string key in namespaces.Keys)
				{
					s.Write(" xmlns");
					if (namespaces[key] != "")
					{
						s.Write(":");
						s.Write(namespaces[key]);
					}
					s.Write("=\"");
					s.Write(key);
					s.Write("\"");
				}
			}
			s.Write(">");
		}

		public override void WriteCloseTag(TextWriter s, string tagName, int depth, Dictionary<string, string> namespaces)
		{
			s.Write("</");
			OoxmlComplexType.WriteXmlPrefix(s, namespaces, "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
			s.Write(tagName);
			s.Write(">");
		}

		public override void WriteAttributes(TextWriter s)
		{
			if (_macro_attr != Convert.ToString("", CultureInfo.InvariantCulture))
			{
				s.Write(" macro=\"");
				OoxmlComplexType.WriteData(s, _macro_attr);
				s.Write("\"");
			}
			if ((bool)(_fPublished_attr != OoxmlBool.OoxmlFalse))
			{
				s.Write(" fPublished=\"");
				OoxmlComplexType.WriteData(s, _fPublished_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_nvPicPr(s, depth, namespaces);
			Write_blipFill(s, depth, namespaces);
			Write_spPr(s, depth, namespaces);
		}

		public void Write_nvPicPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_nvPicPr != null)
			{
				_nvPicPr.Write(s, "nvPicPr", depth + 1, namespaces);
			}
		}

		public void Write_blipFill(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_blipFill != null)
			{
				_blipFill.Write(s, "blipFill", depth + 1, namespaces);
			}
		}

		public void Write_spPr(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_spPr != null)
			{
				_spPr.Write(s, "spPr", depth + 1, namespaces);
			}
		}
	}
}
