using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.drawingml.x2006.main
{
	internal class CT_GraphicalObject : OoxmlComplexType, IOoxmlComplexType
	{
		private CT_GraphicalObjectData _graphicData;

		public CT_GraphicalObjectData GraphicData
		{
			get
			{
				return _graphicData;
			}
			set
			{
				_graphicData = value;
			}
		}

		public static string GraphicDataElementName => "graphicData";

		protected override void InitAttributes()
		{
		}

		protected override void InitElements()
		{
			_graphicData = new CT_GraphicalObjectData();
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
			Write_graphicData(s);
		}

		public void Write_graphicData(TextWriter s)
		{
			if (_graphicData != null)
			{
				_graphicData.Write(s, "graphicData");
			}
		}
	}
}
