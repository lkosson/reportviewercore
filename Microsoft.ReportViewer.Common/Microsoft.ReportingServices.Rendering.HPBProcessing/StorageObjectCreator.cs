using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.Rendering.HPBProcessing
{
	internal class StorageObjectCreator : IScalabilityObjectCreator
	{
		private static StorageObjectCreator m_instance = null;

		private static List<Declaration> m_declarations = BuildDeclarations();

		internal static StorageObjectCreator Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new StorageObjectCreator();
				}
				return m_instance;
			}
		}

		private StorageObjectCreator()
		{
		}

		public bool TryCreateObject(ObjectType objectType, out IPersistable persistObj)
		{
			switch (objectType)
			{
			case ObjectType.ItemSizes:
				persistObj = new ItemSizes();
				break;
			case ObjectType.PageBreakProperties:
				persistObj = new PageBreakProperties();
				break;
			case ObjectType.HiddenPageItem:
				persistObj = new HiddenPageItem();
				break;
			case ObjectType.NoRowsItem:
				persistObj = new NoRowsItem();
				break;
			case ObjectType.SubReport:
				persistObj = new SubReport();
				break;
			case ObjectType.ReportBody:
				persistObj = new ReportBody();
				break;
			case ObjectType.Rectangle:
				persistObj = new Rectangle();
				break;
			case ObjectType.TextBox:
				persistObj = new TextBox();
				break;
			case ObjectType.Paragraph:
				persistObj = new Paragraph();
				break;
			case ObjectType.TextRun:
				persistObj = new TextRun();
				break;
			case ObjectType.TextBoxOffset:
				persistObj = new TextBox.TextBoxOffset();
				break;
			case ObjectType.Line:
				persistObj = new Line();
				break;
			case ObjectType.Chart:
				persistObj = new Chart();
				break;
			case ObjectType.GaugePanel:
				persistObj = new GaugePanel();
				break;
			case ObjectType.Map:
				persistObj = new Map();
				break;
			case ObjectType.Image:
				persistObj = new Image();
				break;
			case ObjectType.Tablix:
				persistObj = new Tablix();
				break;
			case ObjectType.RowInfo:
				persistObj = new Tablix.RowInfo();
				break;
			case ObjectType.SizeInfo:
				persistObj = new Tablix.SizeInfo();
				break;
			case ObjectType.ColumnInfo:
				persistObj = new Tablix.ColumnInfo();
				break;
			case ObjectType.PageDetailCell:
				persistObj = new Tablix.PageDetailCell();
				break;
			case ObjectType.PageCornerCell:
				persistObj = new Tablix.PageCornerCell();
				break;
			case ObjectType.PageMemberCell:
				persistObj = new Tablix.PageMemberCell();
				break;
			case ObjectType.PageStructStaticMemberCell:
				persistObj = new Tablix.PageStructStaticMemberCell();
				break;
			case ObjectType.PageStructDynamicMemberCell:
				persistObj = new Tablix.PageStructDynamicMemberCell();
				break;
			default:
				persistObj = null;
				return false;
			}
			return true;
		}

		public List<Declaration> GetDeclarations()
		{
			return m_declarations;
		}

		private static List<Declaration> BuildDeclarations()
		{
			return new List<Declaration>(30)
			{
				PageItem.GetDeclaration(),
				PageItemContainer.GetDeclaration(),
				ItemSizes.GetDeclaration(),
				HiddenPageItem.GetDeclaration(),
				NoRowsItem.GetDeclaration(),
				SubReport.GetDeclaration(),
				ReportBody.GetDeclaration(),
				Rectangle.GetDeclaration(),
				TextBox.GetDeclaration(),
				TextBox.TextBoxOffset.GetDeclaration(),
				Paragraph.GetDeclaration(),
				TextRun.GetDeclaration(),
				Line.GetDeclaration(),
				DynamicImage.GetDeclaration(),
				Chart.GetDeclaration(),
				GaugePanel.GetDeclaration(),
				Image.GetDeclaration(),
				Tablix.GetDeclaration(),
				Tablix.RowInfo.GetDeclaration(),
				Tablix.SizeInfo.GetDeclaration(),
				Tablix.ColumnInfo.GetDeclaration(),
				Tablix.PageTalixCell.GetDeclaration(),
				Tablix.PageDetailCell.GetDeclaration(),
				Tablix.PageCornerCell.GetDeclaration(),
				Tablix.PageMemberCell.GetDeclaration(),
				Tablix.PageStructMemberCell.GetDeclaration(),
				Tablix.PageStructStaticMemberCell.GetDeclaration(),
				Tablix.PageStructDynamicMemberCell.GetDeclaration(),
				Map.GetDeclaration(),
				PageBreakProperties.GetDeclaration()
			};
		}
	}
}
