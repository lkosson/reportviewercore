using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing
{
	internal class CT_ShapeProperties : OoxmlComplexType
	{
		public enum ChoiceBucket_0
		{
			custGeom,
			prstGeom
		}

		public enum ChoiceBucket_1
		{
			noFill,
			solidFill,
			gradFill,
			blipFill,
			pattFill,
			grpFill
		}

		public enum ChoiceBucket_2
		{
			effectLst,
			effectDag
		}

		private CT_PresetGeometry2D _prstGeom;

		private ChoiceBucket_0 _choice_0;

		private ChoiceBucket_1 _choice_1;

		private ChoiceBucket_2 _choice_2;

		public CT_PresetGeometry2D PrstGeom
		{
			get
			{
				return _prstGeom;
			}
			set
			{
				_prstGeom = value;
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

		public ChoiceBucket_1 Choice_1
		{
			get
			{
				return _choice_1;
			}
			set
			{
				_choice_1 = value;
			}
		}

		public ChoiceBucket_2 Choice_2
		{
			get
			{
				return _choice_2;
			}
			set
			{
				_choice_2 = value;
			}
		}

		public static string PrstGeomElementName => "prstGeom";

		protected override void InitAttributes()
		{
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
		}

		public override void WriteElements(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			Write_prstGeom(s, depth, namespaces);
		}

		public void Write_prstGeom(TextWriter s, int depth, Dictionary<string, string> namespaces)
		{
			if (_choice_0 == ChoiceBucket_0.prstGeom && _prstGeom != null)
			{
				_prstGeom.Write(s, "prstGeom", depth + 1, namespaces);
			}
		}
	}
}
