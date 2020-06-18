using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class MatrixCell
	{
		private Matrix m_owner;

		private int m_rowIndex;

		private int m_columnIndex;

		private MatrixCellInstance m_matrixCellInstance;

		private ReportItem m_cellReportItem;

		private MatrixCellInstanceInfo m_matrixCellInstanceInfo;

		public ReportItem ReportItem
		{
			get
			{
				ReportItem reportItem = m_cellReportItem;
				if (m_cellReportItem == null)
				{
					Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
					ReportItemInstance reportItemInstance = null;
					NonComputedUniqueNames nonComputedUniqueNames = null;
					Microsoft.ReportingServices.ReportProcessing.ReportItem reportItem2 = null;
					if (m_owner.NoRows)
					{
						reportItem2 = matrix.GetCellReportItem(m_rowIndex, m_columnIndex);
					}
					else
					{
						reportItem2 = matrix.GetCellReportItem(InstanceInfo.RowIndex, InstanceInfo.ColumnIndex);
						reportItemInstance = m_matrixCellInstance.Content;
						nonComputedUniqueNames = InstanceInfo.ContentUniqueNames;
					}
					if (reportItem2 != null)
					{
						try
						{
							MatrixSubtotalCellInstance matrixSubtotalCellInstance = m_matrixCellInstance as MatrixSubtotalCellInstance;
							if (matrixSubtotalCellInstance != null)
							{
								Global.Tracer.Assert(matrixSubtotalCellInstance.SubtotalHeadingInstance.MatrixHeadingDef.Subtotal.StyleClass != null);
								m_owner.RenderingContext.HeadingInstance = matrixSubtotalCellInstance.SubtotalHeadingInstance;
							}
						}
						catch (Exception ex)
						{
							Global.Tracer.Trace(TraceLevel.Error, "Could not restore matrix subtotal heading instance from intermediate format: {0}", ex.StackTrace);
							m_owner.RenderingContext.HeadingInstance = null;
						}
						reportItem = ReportItem.CreateItem(0, reportItem2, reportItemInstance, m_owner.RenderingContext, nonComputedUniqueNames);
						m_owner.RenderingContext.HeadingInstance = null;
					}
					if (m_owner.RenderingContext.CacheState)
					{
						m_cellReportItem = reportItem;
					}
				}
				return reportItem;
			}
		}

		public string CellLabel
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
				if (matrix.OwcCellNames != null)
				{
					int index = IndexCellDefinition(matrix);
					return matrix.OwcCellNames[index];
				}
				return null;
			}
		}

		public string ID
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
				int num = IndexCellDefinition(matrix);
				if (matrix.CellIDsForRendering == null)
				{
					matrix.CellIDsForRendering = new string[matrix.CellIDs.Count];
				}
				if (matrix.CellIDsForRendering[num] == null)
				{
					matrix.CellIDsForRendering[num] = matrix.CellIDs[num].ToString(CultureInfo.InvariantCulture);
				}
				return matrix.CellIDsForRendering[num];
			}
		}

		public object SharedRenderingInfo
		{
			get
			{
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
				int index = IndexCellDefinition(matrix);
				int num = matrix.CellIDs[index];
				return m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num];
			}
			set
			{
				Microsoft.ReportingServices.ReportProcessing.Matrix matrix = (Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef;
				int index = IndexCellDefinition(matrix);
				int num = matrix.CellIDs[index];
				m_owner.RenderingContext.RenderingInfoManager.SharedRenderingInfo[num] = value;
			}
		}

		public DataElementOutputTypes DataElementOutput => ((Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef).CellDataElementOutput;

		public string DataElementName => ((Microsoft.ReportingServices.ReportProcessing.Matrix)m_owner.ReportItemDef).CellDataElementName;

		internal int ColumnIndex
		{
			get
			{
				if (m_matrixCellInstance == null)
				{
					return 0;
				}
				return InstanceInfo.ColumnIndex;
			}
		}

		internal int RowIndex
		{
			get
			{
				if (m_matrixCellInstance == null)
				{
					return 0;
				}
				return InstanceInfo.RowIndex;
			}
		}

		private MatrixCellInstanceInfo InstanceInfo
		{
			get
			{
				if (m_matrixCellInstance == null)
				{
					return null;
				}
				if (m_matrixCellInstanceInfo == null)
				{
					m_matrixCellInstanceInfo = m_matrixCellInstance.GetInstanceInfo(m_owner.RenderingContext.ChunkManager);
				}
				return m_matrixCellInstanceInfo;
			}
		}

		internal MatrixCell(Matrix owner, int rowIndex, int columnIndex)
		{
			m_owner = owner;
			m_rowIndex = rowIndex;
			m_columnIndex = columnIndex;
			if (!owner.NoRows)
			{
				MatrixCellInstancesList cells = ((MatrixInstance)owner.ReportItemInstance).Cells;
				m_matrixCellInstance = cells[rowIndex][columnIndex];
			}
		}

		private int IndexCellDefinition(Microsoft.ReportingServices.ReportProcessing.Matrix matrixDef)
		{
			int num = 0;
			if (m_owner.NoRows)
			{
				return m_rowIndex * matrixDef.MatrixColumns.Count + m_columnIndex;
			}
			return InstanceInfo.RowIndex * matrixDef.MatrixColumns.Count + InstanceInfo.ColumnIndex;
		}
	}
}
