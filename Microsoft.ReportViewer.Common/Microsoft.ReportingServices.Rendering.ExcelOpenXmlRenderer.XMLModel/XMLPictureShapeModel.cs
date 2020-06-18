using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.drawingml.x2006.spreadsheetDrawing;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPictureShapeModel : XMLShapeModel, IPictureShapeModel, IShapeModel
	{
		private Picture _interface;

		private XmlPart _parent;

		public Picture Interface
		{
			get
			{
				if (_interface == null)
				{
					_interface = new Picture(this);
				}
				return _interface;
			}
		}

		public string RelId
		{
			set
			{
				BlipFill.Blip.Embed_Attr = value;
			}
		}

		public override string Hyperlink
		{
			set
			{
				Relationship relationship = Manager.AddExternalPartToTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink", value, _parent, value.Contains("://") ? TargetMode.External : TargetMode.Internal);
				NonVisualDrawingProps.HlinkClick = new CT_Hyperlink();
				NonVisualDrawingProps.HlinkClick.Id_Attr = relationship.RelationshipId;
				NonVisualDrawingProps.Descr_Attr = "Hyperlink";
			}
		}

		private CT_Picture Picture
		{
			get
			{
				if (TwoCellAnchor.Pic == null)
				{
					TwoCellAnchor.Pic = new CT_Picture();
				}
				return TwoCellAnchor.Pic;
			}
		}

		private CT_PictureNonVisual NonVisualProperties
		{
			get
			{
				if (Picture.NvPicPr == null)
				{
					Picture.NvPicPr = new CT_PictureNonVisual();
				}
				return Picture.NvPicPr;
			}
		}

		private CT_NonVisualDrawingProps NonVisualDrawingProps
		{
			get
			{
				if (NonVisualProperties.CNvPr == null)
				{
					NonVisualProperties.CNvPr = new CT_NonVisualDrawingProps();
				}
				return NonVisualProperties.CNvPr;
			}
		}

		private CT_BlipFillProperties BlipFill
		{
			get
			{
				if (Picture.BlipFill == null)
				{
					Picture.BlipFill = new CT_BlipFillProperties();
				}
				if (Picture.BlipFill.Blip == null)
				{
					Picture.BlipFill.Blip = new CT_Blip();
				}
				return Picture.BlipFill;
			}
		}

		public XMLPictureShapeModel(PartManager manager, WsDrPart part, XmlPart parent, AnchorModel startAnchor, AnchorModel endAnchor, uint uniqueId)
			: base(manager, part, startAnchor, endAnchor)
		{
			_parent = parent;
			TwoCellAnchor.Choice_0 = CT_TwoCellAnchor.ChoiceBucket_0.pic;
			TwoCellAnchor.ClientData = new CT_AnchorClientData();
			Picture.NvPicPr = new CT_PictureNonVisual();
			Picture.NvPicPr.CNvPicPr = new CT_NonVisualPictureProperties();
			Picture.NvPicPr.CNvPr = new CT_NonVisualDrawingProps();
			Picture.NvPicPr.CNvPr.Id_Attr = uniqueId;
			Picture.NvPicPr.CNvPr.Name_Attr = "Picture " + uniqueId;
			Picture.BlipFill = new CT_BlipFillProperties();
			Picture.BlipFill.Blip = new CT_Blip();
			Picture.BlipFill.Blip.Cstate_Attr = ST_BlipCompression.print;
			Picture.BlipFill.Choice_0 = CT_BlipFillProperties.ChoiceBucket_0.stretch;
			Picture.BlipFill.Stretch = new CT_StretchInfoProperties();
			Picture.BlipFill.Stretch.FillRect = new CT_RelativeRect();
			Picture.SpPr.Choice_0 = CT_ShapeProperties.ChoiceBucket_0.prstGeom;
			Picture.SpPr.PrstGeom = new CT_PresetGeometry2D();
			Picture.SpPr.PrstGeom.Prst_Attr = ST_ShapeType.rect;
			Picture.SpPr.PrstGeom.AvLst = new CT_GeomGuideList();
		}
	}
}
