using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser
{
	internal class OpcCorePropertiesPart : OoxmlPart
	{
		private OpcCoreProperties _root;

		private Dictionary<string, string> _namespaces;

		public override OoxmlComplexType Root => _root;

		public override string Tag => "coreProperties";

		public override Dictionary<string, string> Namespaces => _namespaces;

		public OpcCorePropertiesPart()
		{
			_root = new OpcCoreProperties();
			InitNamespaces();
		}

		private void InitNamespaces()
		{
			_namespaces = new Dictionary<string, string>();
			_namespaces["cp"] = "http://schemas.openxmlformats.org/package/2006/metadata/core-properties";
			_namespaces["dc"] = "http://purl.org/dc/elements/1.1/";
			_namespaces["dcterms"] = "http://purl.org/dc/terms/";
			_namespaces["dcmitype"] = "http://purl.org/dc/dcmitype/";
			_namespaces["xsi"] = "http://www.w3.org/2001/XMLSchema-instance";
		}
	}
}
