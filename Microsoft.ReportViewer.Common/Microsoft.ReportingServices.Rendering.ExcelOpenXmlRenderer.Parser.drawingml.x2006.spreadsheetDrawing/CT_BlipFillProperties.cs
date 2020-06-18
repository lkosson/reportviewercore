using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_BlipFillProperties : OoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			tile,
			stretch
		}

		private uint _dpi_attr;

		private bool _dpi_attr_is_specified;

		private OoxmlBool _rotWithShape_attr;

		private bool _rotWithShape_attr_is_specified;

		private CT_Blip _blip;

		private CT_RelativeRect _srcRect;

		private CT_StretchInfoProperties _stretch;

		private ChoiceBucket_0 _choice_0;

		public uint Dpi_Attr
		{
			get
			{
				return _dpi_attr;
			}
			set
			{
				_dpi_attr = value;
				_dpi_attr_is_specified = true;
			}
		}

		public bool Dpi_Attr_Is_Specified
		{
			get
			{
				return _dpi_attr_is_specified;
			}
			set
			{
				_dpi_attr_is_specified = value;
			}
		}

		public OoxmlBool RotWithShape_Attr
		{
			get
			{
				return _rotWithShape_attr;
			}
			set
			{
				_rotWithShape_attr = value;
				_rotWithShape_attr_is_specified = true;
			}
		}

		public bool RotWithShape_Attr_Is_Specified
		{
			get
			{
				return _rotWithShape_attr_is_specified;
			}
			set
			{
				_rotWithShape_attr_is_specified = value;
			}
		}

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
			_dpi_attr_is_specified = false;
			_rotWithShape_attr_is_specified = false;
		}

		protected override void InitElements()
		{
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
			if (_dpi_attr_is_specified)
			{
				s.Write(" dpi=\"");
				OoxmlComplexType.WriteData(s, _dpi_attr);
				s.Write("\"");
			}
			if (_rotWithShape_attr_is_specified)
			{
				s.Write(" rotWithShape=\"");
				OoxmlComplexType.WriteData(s, _rotWithShape_attr);
				s.Write("\"");
			}
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_blip(s, depth, namespaces);
			Write_srcRect(s, depth, namespaces);
			Write_stretch(s, depth, namespaces);
		}

		public void Write_blip(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_blip != null)
			{
				_blip.Write(s, "blip", depth + 1, namespaces);
			}
		}

		public void Write_srcRect(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_srcRect != null)
			{
				_srcRect.Write(s, "srcRect", depth + 1, namespaces);
			}
		}

		public void Write_stretch(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.stretch && _stretch != null)
			{
				_stretch.Write(s, "stretch", depth + 1, namespaces);
			}
		}
	}
}
