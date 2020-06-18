using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class DataValueCollection : ReportElementCollectionBase<DataValue>
	{
		private bool m_isChartValues;

		private DataValue[] m_cachedDataValues;

		private IList<Microsoft.ReportingServices.ReportIntermediateFormat.DataValue> m_dataValues;

		private IReportScope m_reportScope;

		private Cell m_cell;

		private RenderingContext m_renderingContext;

		private string m_objectName;

		public DataValue this[string name]
		{
			get
			{
				using (IEnumerator<DataValue> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						DataValue current = enumerator.Current;
						if (current != null && string.CompareOrdinal(name, current.Instance.Name) == 0)
						{
							return current;
						}
					}
				}
				return null;
			}
		}

		public override DataValue this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, Count);
				}
				DataValue dataValue = null;
				if (m_cachedDataValues == null)
				{
					m_cachedDataValues = new DataValue[m_dataValues.Count];
				}
				else
				{
					dataValue = m_cachedDataValues[index];
				}
				if (dataValue == null)
				{
					dataValue = new DataValue(m_reportScope, m_renderingContext, m_dataValues[index], m_isChartValues, m_cell, m_objectName);
					m_cachedDataValues[index] = dataValue;
				}
				return dataValue;
			}
		}

		public override int Count
		{
			get
			{
				if (m_cachedDataValues != null)
				{
					return m_cachedDataValues.Length;
				}
				if (m_dataValues != null)
				{
					return m_dataValues.Count;
				}
				return 0;
			}
		}

		internal DataValueCollection(RenderingContext renderingContext, Microsoft.ReportingServices.ReportRendering.ChartDataPoint dataPoint)
		{
			m_isChartValues = true;
			if (dataPoint.DataValues == null)
			{
				m_cachedDataValues = new DataValue[0];
				return;
			}
			int num = dataPoint.DataValues.Length;
			m_cachedDataValues = new DataValue[num];
			for (int i = 0; i < num; i++)
			{
				m_cachedDataValues[i] = new DataValue(renderingContext, dataPoint.DataValues[i]);
			}
		}

		internal DataValueCollection(RenderingContext renderingContext, Microsoft.ReportingServices.ReportRendering.DataCell dataCell)
		{
			m_isChartValues = false;
			if (dataCell.DataValues == null)
			{
				m_cachedDataValues = new DataValue[0];
				return;
			}
			int count = dataCell.DataValues.Count;
			m_cachedDataValues = new DataValue[count];
			for (int i = 0; i < count; i++)
			{
				m_cachedDataValues[i] = new DataValue(renderingContext, dataCell.DataValues[i]);
			}
		}

		internal DataValueCollection(Cell cell, IReportScope reportScope, RenderingContext renderingContext, IList<Microsoft.ReportingServices.ReportIntermediateFormat.DataValue> dataValues, string objectName, bool isChart)
		{
			m_isChartValues = isChart;
			m_cell = cell;
			m_reportScope = reportScope;
			m_dataValues = dataValues;
			m_renderingContext = renderingContext;
			m_objectName = objectName;
		}

		internal void UpdateChartDataValues(object[] datavalues)
		{
			if (m_isChartValues)
			{
				int num = m_cachedDataValues.Length;
				for (int i = 0; i < num; i++)
				{
					m_cachedDataValues[i].UpdateChartDataValue((datavalues == null) ? null : datavalues[i]);
				}
			}
		}

		internal void UpdateDataCellValues(Microsoft.ReportingServices.ReportRendering.DataCell dataCell)
		{
			if (!m_isChartValues)
			{
				int num = m_cachedDataValues.Length;
				for (int i = 0; i < num; i++)
				{
					m_cachedDataValues[i].UpdateDataCellValue((dataCell == null || dataCell.DataValues == null) ? null : dataCell.DataValues[i]);
				}
			}
		}

		internal void SetNewContext()
		{
			if (m_cachedDataValues == null)
			{
				return;
			}
			for (int i = 0; i < m_cachedDataValues.Length; i++)
			{
				if (m_cachedDataValues[i] != null)
				{
					m_cachedDataValues[i].SetNewContext();
				}
			}
		}
	}
}
