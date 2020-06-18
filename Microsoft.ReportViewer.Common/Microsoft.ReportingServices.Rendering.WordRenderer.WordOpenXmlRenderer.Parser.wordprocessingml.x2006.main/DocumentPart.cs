using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main
{
	internal class DocumentPart : OoxmlPart
	{
		private CT_Document _root;

		protected static readonly string _tag = "document";

		private Dictionary<string, string> _namespaces;

		public override OoxmlComplexType Root => _root;

		public override string Tag => _tag;

		public override Dictionary<string, string> Namespaces => _namespaces;

		public DocumentPart()
		{
			InitNamespaces();
			_root = new CT_Document();
		}

		private void InitNamespaces()
		{
			_namespaces = new Dictionary<string, string>();
			_namespaces["http://schemas.openxmlformats.org/markup-compatibility/2006"] = "ve";
			_namespaces["urn:schemas-microsoft-com:office:office"] = "o";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/math"] = "m";
			_namespaces["urn:schemas-microsoft-com:vml"] = "v";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"] = "wp";
			_namespaces["urn:schemas-microsoft-com:office:word"] = "w10";
			_namespaces["http://schemas.openxmlformats.org/wordprocessingml/2006/main"] = "w";
			_namespaces["http://schemas.microsoft.com/office/word/2006/wordml"] = "wne";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			_namespaces["http://schemas.openxmlformats.org/package/2006/metadata/core-properties"] = "cp";
			_namespaces["http://purl.org/dc/elements/1.1/"] = "dc";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/picture"] = "pic";
			_namespaces["http://www.w3.org/2001/XMLSchema"] = "";
		}
	}
}
