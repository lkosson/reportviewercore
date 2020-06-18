using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes
{
	internal class VectorPart : OoxmlPart
	{
		private CT_Vector _root;

		protected static readonly string _tag = "vector";

		private Dictionary<string, string> _namespaces;

		public override OoxmlComplexType Root => _root;

		public override string Tag => _tag;

		public override Dictionary<string, string> Namespaces => _namespaces;

		public VectorPart()
		{
			InitNamespaces();
			_root = new CT_Vector();
		}

		private void InitNamespaces()
		{
			_namespaces = new Dictionary<string, string>();
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"] = "";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/relationships"] = "r";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"] = "xdr";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/sharedTypes"] = "s";
			_namespaces["http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes"] = "vt";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/main"] = "a";
			_namespaces["http://schemas.openxmlformats.org/drawingml/2006/chartDrawing"] = "cdr";
		}
	}
}
