using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.docPropsVTypes;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.officeDocument.x2006.extended_properties;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Archive;
using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Rels.Relationships;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class PartManager
	{
		private WorkbookPart _workbook;

		private XMLWorkbookModel _workbookmodel;

		private StyleManager _stylesheet;

		private OPCRelationshipTree _relationshipTree;

		internal WorkbookPart Workbook => _workbook;

		internal StyleManager StyleSheet => _stylesheet;

		public PartManager(XMLWorkbookModel workbookModel)
		{
			_workbookmodel = workbookModel;
			_relationshipTree = new OPCRelationshipTree("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", ((IStreambookModel)_workbookmodel).ZipPackage);
			WorkbookPart workbookPart = new WorkbookPart();
			CT_Workbook obj = (CT_Workbook)workbookPart.Root;
			obj.FileVersion = new CT_FileVersion();
			obj.FileVersion.AppName_Attr = "xl";
			obj.FileVersion.LastEdited_Attr = "4";
			obj.FileVersion.LowestEdited_Attr = "4";
			obj.FileVersion.RupBuild_Attr = "4506";
			obj.WorkbookPr = new CT_WorkbookPr();
			obj.WorkbookPr.DefaultThemeVersion_Attr = 124226u;
			obj.BookViews = new CT_BookViews();
			CT_BookView item = new CT_BookView
			{
				XWindow_Attr = 240,
				YWindow_Attr = 120,
				WindowWidth_Attr = 18060u,
				WindowHeight_Attr = 7050u
			};
			obj.BookViews.WorkbookView.Add(item);
			obj.CalcPr = new CT_CalcPr();
			obj.CalcPr.CalcId_Attr = 125725u;
			Relationship relationship = _relationshipTree.AddRootPartToTree(workbookPart, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument", "xl/workbook.xml");
			_workbook = workbookPart;
			StyleSheetPart styleSheetPart = new StyleSheetPart();
			CT_Stylesheet obj2 = (CT_Stylesheet)styleSheetPart.Root;
			CT_Font cT_Font = new CT_Font();
			cT_Font.Sz = new CT_FontSize();
			cT_Font.Sz.Val_Attr = 11.0;
			cT_Font.Color = new CT_Color();
			cT_Font.Color.Rgb_Attr = "FF000000";
			cT_Font.Name = new CT_FontName();
			cT_Font.Name.Val_Attr = "Calibri";
			cT_Font.Family = new CT_IntProperty();
			cT_Font.Family.Val_Attr = 2;
			cT_Font.Scheme = new CT_FontScheme();
			cT_Font.Scheme.Val_Attr = ST_FontScheme.minor;
			obj2.Fonts = new CT_Fonts();
			obj2.Fonts.Font.Add(cT_Font);
			obj2.Fonts.Count_Attr = 1u;
			CT_Fill cT_Fill = new CT_Fill();
			cT_Fill.PatternFill = new CT_PatternFill();
			cT_Fill.PatternFill.PatternType_Attr = ST_PatternType.none;
			CT_Fill cT_Fill2 = new CT_Fill();
			cT_Fill2.PatternFill = new CT_PatternFill();
			cT_Fill2.PatternFill.PatternType_Attr = ST_PatternType.gray125;
			obj2.Fills = new CT_Fills();
			obj2.Fills.Fill.Add(cT_Fill);
			obj2.Fills.Fill.Add(cT_Fill2);
			obj2.Fills.Count_Attr = 2u;
			CT_Border item2 = new CT_Border
			{
				Left = new CT_BorderPr(),
				Right = new CT_BorderPr(),
				Top = new CT_BorderPr(),
				Bottom = new CT_BorderPr(),
				Diagonal = new CT_BorderPr()
			};
			obj2.Borders = new CT_Borders();
			obj2.Borders.Border.Add(item2);
			obj2.Borders.Count_Attr = 1u;
			CT_Xf item3 = new CT_Xf
			{
				NumFmtId_Attr = 0u,
				FontId_Attr = 0u,
				FillId_Attr = 0u,
				BorderId_Attr = 0u
			};
			obj2.CellStyleXfs = new CT_CellStyleXfs();
			obj2.CellStyleXfs.Xf.Add(item3);
			obj2.CellXfs = new CT_CellXfs();
			obj2.CellXfs.Xf.Add(StyleManager.CreateDefaultXf());
			CT_CellStyle item4 = new CT_CellStyle
			{
				Name_Attr = "Normal",
				XfId_Attr = 0u,
				BuiltinId_Attr = 0u
			};
			obj2.CellStyles = new CT_CellStyles();
			obj2.CellStyles.CellStyle.Add(item4);
			obj2.Dxfs = new CT_Dxfs();
			obj2.Dxfs.Count_Attr = 0u;
			obj2.TableStyles = new CT_TableStyles();
			obj2.TableStyles.Count_Attr = 0u;
			obj2.TableStyles.DefaultTableStyle_Attr = "TableStyleMedium9";
			obj2.TableStyles.DefaultPivotStyle_Attr = "PivotStyleLight16";
			_relationshipTree.AddPartToTree(styleSheetPart, "application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles", "xl/styles.xml", (XmlPart)_relationshipTree.GetPartByLocation(relationship.RelatedPart));
			_stylesheet = new StyleManager(styleSheetPart);
			OpcCorePropertiesPart part = new OpcCorePropertiesPart();
			_relationshipTree.AddRootPartToTree(part, "application/vnd.openxmlformats-package.core-properties+xml", "http://schemas.openxmlformats.org/package/2006/relationships/meatadata/core-properties", "docProps/core.xml");
			PropertiesPart propertiesPart = new PropertiesPart();
			CT_Properties obj3 = (CT_Properties)propertiesPart.Root;
			obj3.Application = "Microsoft Excel";
			obj3.DocSecurity = 0;
			obj3.ScaleCrop = false;
			obj3.HeadingPairs = new CT_VectorVariant();
			obj3.HeadingPairs.Vector = new CT_Vector();
			obj3.HeadingPairs.Vector.Size_Attr = 2u;
			obj3.HeadingPairs.Vector.BaseType_Attr = ST_VectorBaseType.variant;
			CT_Variant item5 = new CT_Variant
			{
				Choice_0 = CT_Variant.ChoiceBucket_0.lpstr,
				Lpstr = "Worksheets"
			};
			CT_Variant item6 = new CT_Variant
			{
				Choice_0 = CT_Variant.ChoiceBucket_0.i4,
				I4 = 1
			};
			obj3.HeadingPairs.Vector.Variant.Add(item5);
			obj3.HeadingPairs.Vector.Variant.Add(item6);
			obj3.TitlesOfParts = new CT_VectorLpstr();
			obj3.TitlesOfParts.Vector = new CT_Vector();
			obj3.TitlesOfParts.Vector.Size_Attr = 0u;
			obj3.TitlesOfParts.Vector.BaseType_Attr = ST_VectorBaseType.lpstr;
			obj3.LinksUpToDate = false;
			obj3.SharedDoc = false;
			obj3.HyperlinksChanged = false;
			obj3.AppVersion = "12.0000";
			_relationshipTree.AddRootPartToTree(propertiesPart, "application/vnd.openxmlformats-officedocument.extended-properties+xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties", "docProps/app.xml");
		}

		public void Write()
		{
			try
			{
				WriteCommon();
			}
			finally
			{
				Package zipPackage = ((IStreambookModel)_workbookmodel).ZipPackage;
				zipPackage.Flush();
				zipPackage.Close();
			}
		}

		private void WriteCommon()
		{
			_workbookmodel.Cleanup();
			if (_stylesheet != null)
			{
				_stylesheet.Cleanup();
			}
			_relationshipTree.WriteTree();
		}

		public Relationship AddPartToTree(OoxmlPart part, string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return _relationshipTree.AddPartToTree(part, contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddStreamingPartToTree(string contentType, string relationshipType, string locationHint, XmlPart parent)
		{
			return _relationshipTree.AddStreamingPartToTree(contentType, relationshipType, locationHint, parent);
		}

		public Relationship AddExternalPartToTree(string relationshipType, string externalTarget, XmlPart parent, TargetMode targetMode)
		{
			return _relationshipTree.AddExternalPartToTree(relationshipType, externalTarget, parent, targetMode);
		}

		public Relationship AddImageToTree(string uniqueId, Stream data, string extension, string relationshipType, string locationHint, string parentLocation, ContentTypeAction ctypeAction)
		{
			bool newBlob;
			Relationship relationship = _relationshipTree.AddImageToTree(uniqueId, extension, relationshipType, locationHint, parentLocation, ctypeAction, out newBlob);
			if (newBlob)
			{
				Stream stream = ((IStreambookModel)_workbookmodel).ZipPackage.GetPart(new Uri(Utils.CleanName(relationship.RelatedPart), UriKind.Relative)).GetStream();
				SupportClass.CopyStream(data, stream);
			}
			return relationship;
		}

		public RelPart GetPartByContentType(string contenttype)
		{
			return _relationshipTree.GetPartByContentType(contenttype);
		}

		public List<Relationship> GetRelationshipsForSheet(CT_Sheet sheetEntry, string relationshipType)
		{
			List<Relationship> list = new List<Relationship>();
			foreach (Relationship workbookRelationship in GetWorkbookRelationships())
			{
				if (!(workbookRelationship.RelationshipId == sheetEntry.Id_Attr))
				{
					continue;
				}
				foreach (Relationship item in _relationshipTree.GetRelationshipsByPath(workbookRelationship.RelatedPart))
				{
					if (item.RelationshipType == relationshipType)
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public XmlPart GetWorksheetXmlPart(CT_Sheet sheetentry)
		{
			foreach (Relationship workbookRelationship in GetWorkbookRelationships())
			{
				if (workbookRelationship.RelationshipId == sheetentry.Id_Attr)
				{
					return (XmlPart)_relationshipTree.GetPartByLocation(workbookRelationship.RelatedPart);
				}
			}
			throw new FatalException();
		}

		public XmlPart GetPartByLocation(string location)
		{
			return (XmlPart)_relationshipTree.GetPartByLocation(location);
		}

		private List<Relationship> GetWorkbookRelationships()
		{
			string[] array = new string[2]
			{
				"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml",
				"application/vnd.ms-excel.sheet.macroEnabled.main+xml"
			};
			foreach (string contenttype in array)
			{
				RelPart partByContentType = _relationshipTree.GetPartByContentType(contenttype);
				if (partByContentType != null)
				{
					return _relationshipTree.GetRelationshipsByPath(partByContentType.Location);
				}
			}
			throw new FatalException();
		}
	}
}
