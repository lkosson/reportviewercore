using Microsoft.ReportingServices.DataProcessing;
using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.ReportingServices.OnDemandProcessing
{
	internal sealed class DataSetQueryRestartPosition
	{
		private List<RestartContext> m_restartPosition;

		private bool m_enableRowSkipping;

		public DataSetQueryRestartPosition(List<RestartContext> restartPosition)
		{
			m_restartPosition = restartPosition;
			m_enableRowSkipping = true;
		}

		[Conditional("DEBUG")]
		private void ValidateRestartPosition(List<RestartContext> restartPosition)
		{
			Global.Tracer.Assert(restartPosition != null && restartPosition.Count > 0, "Restart position should be non-null and non-empty");
			foreach (RestartContext item in restartPosition)
			{
				Global.Tracer.Assert(item.RestartMode != RestartMode.Rom, "DataSetQueryRestartPosition does not handle ROM restart values");
			}
		}

		public List<ScopeValueFieldName> GetQueryRestartPosition(Microsoft.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			List<ScopeValueFieldName> list = null;
			for (int i = 0; i < m_restartPosition.Count; i++)
			{
				RestartContext restartContext = m_restartPosition[i];
				if (restartContext.RestartMode == RestartMode.Query)
				{
					if (list == null)
					{
						list = new List<ScopeValueFieldName>();
					}
					list.AddRange(restartContext.GetScopeValueFieldNameCollection(dataSet));
				}
			}
			return list;
		}

		public bool ShouldSkip(OnDemandProcessingContext odpContext, Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row)
		{
			if (row != null && m_enableRowSkipping)
			{
				foreach (RestartContext item in m_restartPosition)
				{
					switch (item.DoesNotMatchRowRecordField(odpContext, row.RecordFields))
					{
					case RowSkippingControlFlag.Skip:
						if (item.RestartMode == RestartMode.Query)
						{
							item.TraceStartAtRecoveryMessage();
						}
						return true;
					case RowSkippingControlFlag.Stop:
						return false;
					}
				}
			}
			return false;
		}

		public void DisableRowSkipping(Microsoft.ReportingServices.ReportIntermediateFormat.RecordRow row)
		{
			m_enableRowSkipping = false;
		}
	}
}
