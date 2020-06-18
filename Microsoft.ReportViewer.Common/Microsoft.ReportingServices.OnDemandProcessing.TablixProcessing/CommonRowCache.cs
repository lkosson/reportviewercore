using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using System;

namespace Microsoft.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class CommonRowCache : IDisposable
	{
		private ScalableList<DataFieldRow> m_rows;

		internal const int UnInitializedRowIndex = -1;

		internal int Count => m_rows.Count;

		internal int LastRowIndex => Count - 1;

		internal CommonRowCache(IScalabilityCache scaleCache)
		{
			m_rows = new ScalableList<DataFieldRow>(0, scaleCache, 1000, 100);
		}

		internal int AddRow(DataFieldRow row)
		{
			int count = m_rows.Count;
			m_rows.Add(row);
			return count;
		}

		internal DataFieldRow GetRow(int index)
		{
			return m_rows[index];
		}

		internal void SetupRow(int index, OnDemandProcessingContext odpContext)
		{
			GetRow(index).SetFields(odpContext.ReportObjectModel.FieldsImpl);
		}

		public void Dispose()
		{
			m_rows.Dispose();
			m_rows = null;
		}
	}
}
