using Microsoft.ReportingServices.ReportProcessing;
using System;

namespace Microsoft.Reporting
{
	[Serializable]
	internal class LocalExecutionInfo
	{
		public ParameterInfoCollection ReportParameters
		{
			get;
			set;
		}

		public PageProperties PageProperties
		{
			get;
			set;
		}

		public int AutoRefreshInterval
		{
			get;
			private set;
		}

		public int TotalPages
		{
			get;
			private set;
		}

		public PaginationMode PaginationMode
		{
			get;
			private set;
		}

		public bool HasDocMap
		{
			get;
			private set;
		}

		public bool HasSnapshot
		{
			get;
			set;
		}

		public bool IsCompiled
		{
			get;
			set;
		}

		public LocalExecutionInfo()
		{
			Reset();
		}

		public void Reset()
		{
			TotalPages = 0;
			HasDocMap = false;
			PaginationMode = PaginationMode.Progressive;
		}

		public void SaveProcessingResult(OnDemandProcessingResult result)
		{
			HasDocMap = result.HasDocumentMap;
			AutoRefreshInterval = result.AutoRefresh;
			if (result.NumberOfPages != 0)
			{
				TotalPages = result.NumberOfPages;
				PaginationMode = result.UpdatedPaginationMode;
			}
		}
	}
}
