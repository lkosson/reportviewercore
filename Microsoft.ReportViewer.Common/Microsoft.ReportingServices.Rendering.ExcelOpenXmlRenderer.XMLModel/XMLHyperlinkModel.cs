using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLHyperlinkModel : IOoxmlCtWrapperModel
	{
		private readonly CT_Hyperlink _link;

		private bool _isInternal;

		private readonly PartManager _manager;

		private readonly CT_Sheet _worksheetEntry;

		public OoxmlComplexType OoxmlTag => _link;

		public XMLHyperlinkModel(string area, string href, string text, PartManager manager, CT_Sheet entry)
		{
			_link = new CT_Hyperlink();
			_link._ref_Attr = area;
			_link.Location_Attr = href;
			_link.Display_Attr = text;
			_isInternal = !href.Contains("://");
			_manager = manager;
			_worksheetEntry = entry;
		}

		public void Cleanup()
		{
			if (!_isInternal)
			{
				Relationship relationship = _manager.AddExternalPartToTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", _link.Location_Attr, _manager.GetWorksheetXmlPart(_worksheetEntry), TargetMode.External);
				_link.Location_Attr = null;
				_link.Id_Attr = relationship.RelationshipId;
			}
		}
	}
}
