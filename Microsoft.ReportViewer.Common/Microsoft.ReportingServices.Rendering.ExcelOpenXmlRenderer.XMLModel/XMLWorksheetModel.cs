using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal abstract class XMLWorksheetModel : IWorksheetModel, ICloneable
	{
		protected XMLWorkbookModel _workbookModel;

		protected XMLWorksheetsModel _worksheetsModel;

		protected PartManager _manager;

		protected XMLPictureShapesModel PicturesModel;

		protected CT_Sheet _sheetentry;

		public abstract Streamsheet Interface
		{
			get;
		}

		public IWorkbookModel Workbook => _workbookModel;

		public abstract IPictureShapesModel Pictures
		{
			get;
		}

		public abstract IPageSetupModel PageSetup
		{
			get;
		}

		public string Name
		{
			get
			{
				return _sheetentry.Name_Attr;
			}
			set
			{
				_sheetentry.Name_Attr = value;
			}
		}

		public int Position => _worksheetsModel.getSheetPosition(Name);

		public abstract bool ShowGridlines
		{
			set;
		}

		public XMLDefinedNamesManager NameManager => _workbookModel.NameManager;

		public abstract IColumnModel getColumn(int index);

		public AnchorModel createAnchor(int row, int column, double offsetX, double offsetY)
		{
			return new AnchorModel(row, column, offsetX, offsetY);
		}

		public abstract void SetBackgroundPicture(string uniqueId, string extension, Stream pictureStream);

		protected string InsertBackgroundPicture(string uniqueId, string extension, Stream pictureStream)
		{
			return _manager.AddImageToTree(uniqueId, pictureStream, extension, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image", "xl/media/image{0}." + extension, _manager.GetWorksheetXmlPart(_sheetentry).Location, ContentTypeAction.Default).RelationshipId;
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public abstract void Cleanup();
	}
}
