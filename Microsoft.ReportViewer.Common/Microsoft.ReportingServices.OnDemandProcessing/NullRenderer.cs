using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class NullRenderer
	{
		private OnDemandProcessingContext m_odpContext;

		private DocumentMapWriter m_docMapWriter;

		private Stream m_docMapStream;

		private bool m_generateDocMap;

		private bool m_createSnapshot;

		private Microsoft.ReportingServices.OnDemandReportRendering.Report m_report;

		internal Stream DocumentMapStream => m_docMapStream;

		internal NullRenderer()
		{
		}

		internal void Process(Microsoft.ReportingServices.OnDemandReportRendering.Report report, OnDemandProcessingContext odpContext, bool generateDocumentMap, bool createSnapshot)
		{
			m_odpContext = odpContext;
			m_report = report;
			m_generateDocMap = (generateDocumentMap && m_report.HasDocumentMap);
			m_createSnapshot = createSnapshot;
			if (m_generateDocMap)
			{
				odpContext.HasRenderFormatDependencyInDocumentMap = false;
			}
			if (m_generateDocMap || m_createSnapshot)
			{
				foreach (Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection in report.ReportSections)
				{
					Visit(reportSection);
				}
			}
			if (m_generateDocMap && m_docMapWriter != null)
			{
				m_docMapWriter.WriteEndContainer();
				m_docMapWriter.Close();
				m_docMapWriter = null;
				if (odpContext.HasRenderFormatDependencyInDocumentMap)
				{
					odpContext.OdpMetadata.ReportSnapshot.SetRenderFormatDependencyInDocumentMap(odpContext);
				}
			}
		}

		private void Visit(Microsoft.ReportingServices.OnDemandReportRendering.ReportSection section)
		{
			Visit(section.Body.ReportItemCollection);
			VisitStyle(section.Body.Style);
			VisitStyle(section.Page.Style);
		}

		private void Visit(Microsoft.ReportingServices.OnDemandReportRendering.ReportItemCollection itemCollection)
		{
			for (int i = 0; i < itemCollection.Count; i++)
			{
				Visit(itemCollection[i]);
			}
		}

		private void Visit(Microsoft.ReportingServices.OnDemandReportRendering.ReportItem item)
		{
			if (item == null || item.Instance == null)
			{
				return;
			}
			bool generateDocMap = m_generateDocMap;
			if (!ProcessVisibilityAndContinue(item.Visibility, item.Instance.Visibility, null))
			{
				return;
			}
			if (item is Microsoft.ReportingServices.OnDemandReportRendering.Line || item is Microsoft.ReportingServices.OnDemandReportRendering.Chart || item is Microsoft.ReportingServices.OnDemandReportRendering.GaugePanel || item is Microsoft.ReportingServices.OnDemandReportRendering.Map)
			{
				GenerateSimpleReportItemDocumentMap(item);
			}
			else if (item is Microsoft.ReportingServices.OnDemandReportRendering.TextBox)
			{
				GenerateSimpleReportItemDocumentMap(item);
				VisitStyle(item.Style);
			}
			else if (item is Microsoft.ReportingServices.OnDemandReportRendering.Image)
			{
				GenerateSimpleReportItemDocumentMap(item);
				Microsoft.ReportingServices.OnDemandReportRendering.Image image = item as Microsoft.ReportingServices.OnDemandReportRendering.Image;
				Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType source = image.Source;
				if (m_createSnapshot && (source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.External || source == Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Database))
				{
					_ = (image.Instance as Microsoft.ReportingServices.OnDemandReportRendering.ImageInstance)?.ImageData;
				}
			}
			else if (item is Microsoft.ReportingServices.OnDemandReportRendering.Rectangle)
			{
				VisitRectangle(item as Microsoft.ReportingServices.OnDemandReportRendering.Rectangle);
				VisitStyle(item.Style);
			}
			else if (!(item is Microsoft.ReportingServices.OnDemandReportRendering.CustomReportItem))
			{
				bool flag = false;
				if (m_generateDocMap)
				{
					string documentMapLabel = item.Instance.DocumentMapLabel;
					if (documentMapLabel != null)
					{
						flag = true;
						WriteDocumentMapBeginContainer(documentMapLabel, item.Instance.UniqueName);
					}
				}
				if (item is Microsoft.ReportingServices.OnDemandReportRendering.Tablix)
				{
					VisitTablix(item as Microsoft.ReportingServices.OnDemandReportRendering.Tablix);
					VisitStyle(item.Style);
				}
				else if (item is Microsoft.ReportingServices.OnDemandReportRendering.SubReport)
				{
					VisitSubReport(item as Microsoft.ReportingServices.OnDemandReportRendering.SubReport);
				}
				else
				{
					Global.Tracer.Assert(condition: false);
				}
				if (flag)
				{
					WriteDocumentMapEndContainer();
				}
			}
			m_generateDocMap = generateDocMap;
		}

		private void GenerateSimpleReportItemDocumentMap(Microsoft.ReportingServices.OnDemandReportRendering.ReportItem item)
		{
			if (m_generateDocMap)
			{
				string documentMapLabel = item.Instance.DocumentMapLabel;
				if (documentMapLabel != null)
				{
					WriteDocumentMapNode(documentMapLabel, item.Instance.UniqueName);
				}
			}
		}

		private void VisitRectangle(Microsoft.ReportingServices.OnDemandReportRendering.Rectangle rectangleDef)
		{
			bool flag = false;
			if (m_generateDocMap)
			{
				string documentMapLabel = rectangleDef.Instance.DocumentMapLabel;
				if (documentMapLabel != null)
				{
					flag = true;
					string text = null;
					int linkToChild = rectangleDef.LinkToChild;
					text = ((linkToChild < 0) ? rectangleDef.Instance.UniqueName : rectangleDef.ReportItemCollection[linkToChild].Instance.UniqueName);
					WriteDocumentMapBeginContainer(documentMapLabel, text);
				}
			}
			Visit(rectangleDef.ReportItemCollection);
			if (flag)
			{
				WriteDocumentMapEndContainer();
			}
		}

		private void VisitSubReport(Microsoft.ReportingServices.OnDemandReportRendering.SubReport subreportDef)
		{
			if (subreportDef.Report == null || subreportDef.Instance == null || subreportDef.ProcessedWithError)
			{
				return;
			}
			Microsoft.ReportingServices.OnDemandReportRendering.Report report = subreportDef.Report;
			if (!report.HasDocumentMap && !m_createSnapshot)
			{
				return;
			}
			foreach (Microsoft.ReportingServices.OnDemandReportRendering.ReportSection reportSection in report.ReportSections)
			{
				Visit(reportSection.Body.ReportItemCollection);
				VisitStyle(reportSection.Body.Style);
			}
		}

		private void VisitTablix(Microsoft.ReportingServices.OnDemandReportRendering.Tablix tablixDef)
		{
			if (tablixDef.Corner != null)
			{
				TablixCornerRowCollection rowCollection = tablixDef.Corner.RowCollection;
				for (int i = 0; i < rowCollection.Count; i++)
				{
					TablixCornerRow tablixCornerRow = rowCollection[i];
					if (tablixCornerRow == null)
					{
						continue;
					}
					for (int j = 0; j < tablixCornerRow.Count; j++)
					{
						Microsoft.ReportingServices.OnDemandReportRendering.TablixCornerCell tablixCornerCell = tablixCornerRow[j];
						if (tablixCornerCell != null)
						{
							Visit(tablixCornerCell.CellContents.ReportItem);
						}
					}
				}
			}
			VisitTablixMemberCollection(tablixDef.ColumnHierarchy.MemberCollection, -1, isTopLevel: true);
			VisitTablixMemberCollection(tablixDef.RowHierarchy.MemberCollection, -1, isTopLevel: true);
		}

		private void VisitTablixMemberCollection(TablixMemberCollection memberCollection, int rowMemberIndex, bool isTopLevel)
		{
			if (memberCollection == null)
			{
				return;
			}
			for (int i = 0; i < memberCollection.Count; i++)
			{
				Microsoft.ReportingServices.OnDemandReportRendering.TablixMember tablixMember = memberCollection[i];
				if (tablixMember.IsStatic)
				{
					VisitTablixMember(tablixMember, rowMemberIndex, null);
					continue;
				}
				TablixDynamicMemberInstance tablixDynamicMemberInstance = (TablixDynamicMemberInstance)tablixMember.Instance;
				Stack<int> stack = new Stack<int>();
				if (isTopLevel)
				{
					tablixDynamicMemberInstance.ResetContext();
				}
				while (tablixDynamicMemberInstance.MoveNext())
				{
					VisitTablixMember(tablixMember, rowMemberIndex, stack);
				}
				for (int j = 0; j < stack.Count; j++)
				{
					WriteDocumentMapEndContainer();
				}
			}
		}

		private void VisitTablixMember(Microsoft.ReportingServices.OnDemandReportRendering.TablixMember memberDef, int rowMemberIndex, Stack<int> openRecursiveLevels)
		{
			if (memberDef.Instance == null)
			{
				return;
			}
			bool generateDocMap = m_generateDocMap;
			if (!ProcessVisibilityAndContinue(memberDef.Visibility, memberDef.Instance.Visibility, memberDef))
			{
				return;
			}
			if (!memberDef.IsStatic && rowMemberIndex == -1 && memberDef.Group != null && m_generateDocMap)
			{
				GroupInstance instance = memberDef.Group.Instance;
				string documentMapLabel = instance.DocumentMapLabel;
				int recursiveLevel = instance.RecursiveLevel;
				if (documentMapLabel != null)
				{
					while (openRecursiveLevels.Count > 0 && openRecursiveLevels.Peek() >= recursiveLevel)
					{
						WriteDocumentMapEndContainer();
						openRecursiveLevels.Pop();
					}
					WriteDocumentMapBeginContainer(documentMapLabel, memberDef.Group.Instance.UniqueName);
					openRecursiveLevels.Push(recursiveLevel);
				}
			}
			if (rowMemberIndex == -1 && memberDef.TablixHeader != null && memberDef.TablixHeader.CellContents != null)
			{
				Visit(memberDef.TablixHeader.CellContents.ReportItem);
			}
			if (memberDef.Children == null)
			{
				if (memberDef.IsColumn)
				{
					if (rowMemberIndex != -1)
					{
						Microsoft.ReportingServices.OnDemandReportRendering.TablixCell tablixCell = memberDef.OwnerTablix.Body.RowCollection[rowMemberIndex][memberDef.MemberCellIndex];
						if (tablixCell != null && tablixCell.CellContents != null)
						{
							Visit(tablixCell.CellContents.ReportItem);
						}
					}
				}
				else
				{
					VisitTablixMemberCollection(memberDef.OwnerTablix.ColumnHierarchy.MemberCollection, memberDef.MemberCellIndex, isTopLevel: true);
				}
			}
			else
			{
				VisitTablixMemberCollection(memberDef.Children, rowMemberIndex, isTopLevel: false);
			}
			m_generateDocMap = generateDocMap;
		}

		private void VisitStyle(Microsoft.ReportingServices.OnDemandReportRendering.Style style)
		{
			if (style != null && m_createSnapshot)
			{
				BackgroundImage backgroundImage = style.BackgroundImage;
				if (backgroundImage != null && backgroundImage.Source != Microsoft.ReportingServices.OnDemandReportRendering.Image.SourceType.Embedded && backgroundImage.Instance != null)
				{
					_ = backgroundImage.Instance.ImageData;
				}
			}
		}

		private bool ProcessVisibilityAndContinue(Microsoft.ReportingServices.OnDemandReportRendering.Visibility aVisibility, VisibilityInstance aVisibilityInstance, Microsoft.ReportingServices.OnDemandReportRendering.TablixMember memberDef)
		{
			if (aVisibility == null)
			{
				return true;
			}
			if (aVisibilityInstance != null && m_createSnapshot)
			{
				_ = aVisibilityInstance.StartHidden;
			}
			switch (aVisibility.HiddenState)
			{
			case SharedHiddenState.Always:
				if (m_createSnapshot)
				{
					m_generateDocMap = false;
					return true;
				}
				return false;
			case SharedHiddenState.Sometimes:
				if (aVisibilityInstance.CurrentlyHidden && aVisibility.ToggleItem == null)
				{
					if (m_createSnapshot)
					{
						m_generateDocMap = false;
						return true;
					}
					return false;
				}
				break;
			default:
				if (memberDef != null && memberDef.IsTotal)
				{
					if (m_createSnapshot)
					{
						m_generateDocMap = false;
						return true;
					}
					return false;
				}
				break;
			}
			return true;
		}

		private void InitWriter()
		{
			m_docMapStream = m_odpContext.ChunkFactory.CreateChunk("DocumentMap", Microsoft.ReportingServices.ReportProcessing.ReportProcessing.ReportChunkTypes.Interactivity, null);
			m_docMapWriter = new DocumentMapWriter(m_docMapStream, m_odpContext);
			m_docMapWriter.WriteBeginContainer(m_report.Name, m_report.Instance.UniqueName);
		}

		private void WriteDocumentMapNode(string aLabel, string aId)
		{
			if (m_docMapWriter == null)
			{
				InitWriter();
			}
			m_docMapWriter.WriteNode(aLabel, aId);
		}

		private void WriteDocumentMapBeginContainer(string aLabel, string aId)
		{
			if (m_docMapWriter == null)
			{
				InitWriter();
			}
			m_docMapWriter.WriteBeginContainer(aLabel, aId);
		}

		private void WriteDocumentMapEndContainer()
		{
			if (m_docMapWriter == null)
			{
				InitWriter();
			}
			m_docMapWriter.WriteEndContainer();
		}
	}
}
