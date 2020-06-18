using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.extended_properties
{
	internal class PropertiesPart : OoxmlPart
	{
		private CT_Properties _root;

		protected static readonly string _tag = "Properties";

		private Dictionary<string, string> _namespaces;

		public override OoxmlComplexType Root => _root;

		public override string Tag => _tag;

		public override Dictionary<string, string> Namespaces => _namespaces;

		public PropertiesPart()
		{
			InitNamespaces();
			_root = new CT_Properties();
		}

		private void InitNamespaces()
		{
			_namespaces = new Dictionary<string, string>();
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/extended-properties"] = "";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"] = "xdr";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes"] = "s";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"] = "vt";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/chartDrawing"] = "cdr";
		}
	}
}
