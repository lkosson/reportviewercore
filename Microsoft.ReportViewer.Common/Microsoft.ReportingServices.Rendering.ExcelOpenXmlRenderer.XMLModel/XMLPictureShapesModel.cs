using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPictureShapesModel : IPictureShapesModel
	{
		private Pictures _interface;

		private readonly PartManager _manager;

		private readonly XmlPart _parent;

		private readonly WsDrPart _drawing;

		private readonly Relationship _drawingrel;

		private readonly List<XMLPictureShapeModel> _pictures;

		private uint _nextid;

		public Pictures Interface
		{
			get
			{
				if (_interface == null)
				{
					_interface = new Pictures(this);
				}
				return _interface;
			}
		}

		private uint NextId => ++_nextid;

		public XMLPictureShapesModel(PartManager manager, CT_Sheet sheetEntry, string drawingId)
		{
			_manager = manager;
			_parent = _manager.GetWorksheetXmlPart(sheetEntry);
			_pictures = new List<XMLPictureShapeModel>();
			foreach (Relationship item in _manager.GetRelationshipsForSheet(sheetEntry, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing"))
			{
				if (item.RelationshipId == drawingId)
				{
					_drawingrel = item;
					_drawing = (WsDrPart)_manager.GetPartByLocation(item.RelatedPart).HydratedPart;
					_ = (Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing.CT_Drawing)_drawing.Root;
					break;
				}
			}
		}

		public IPictureShapeModel CreatePicture(string uniqueId, string extension, Stream pictureStream, AnchorModel startPosition, AnchorModel endPosition)
		{
			XMLPictureShapeModel xMLPictureShapeModel = new XMLPictureShapeModel(_manager, _drawing, _manager.GetPartByLocation(_drawingrel.RelatedPart), startPosition, endPosition, NextId);
			_pictures.Add(xMLPictureShapeModel);
			Relationship relationship = _manager.AddImageToTree(uniqueId, pictureStream, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "xl/media/image{0}." + extension, _drawingrel.RelatedPart, ContentTypeAction.Default);
			xMLPictureShapeModel.RelId = relationship.RelationshipId;
			return xMLPictureShapeModel;
		}
	}
}
