using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Microsoft.Reporting.WinForms
{
	[TypeConverter(typeof(TypeNameHidingExpandableObjectConverter))]
	public abstract class Report
	{
		private string m_displayName = "";

		private int m_drillthroughDepth = 1;

		internal object m_syncObject = new object();

		[SRDescription("DisplayNameDesc")]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		public string DisplayName
		{
			get
			{
				return m_displayName;
			}
			set
			{
				m_displayName = value;
			}
		}

		internal abstract string DisplayNameForUse
		{
			get;
		}

		[Browsable(false)]
		public bool IsDrillthroughReport => DrillthroughDepth > 1;

		internal int DrillthroughDepth
		{
			get
			{
				return m_drillthroughDepth;
			}
			set
			{
				m_drillthroughDepth = value;
			}
		}

		internal abstract bool IsReadyForConnection
		{
			get;
		}

		[Browsable(false)]
		public bool IsReadyForRendering
		{
			get
			{
				try
				{
					return PrepareForRender();
				}
				catch
				{
					return false;
				}
			}
		}

		internal abstract bool IsPreparedReportReadyForRendering
		{
			get;
		}

		internal abstract bool HasDocMap
		{
			get;
		}

		internal abstract int AutoRefreshInterval
		{
			get;
		}

		internal abstract bool CanSelfCancel
		{
			get;
		}

		internal event EventHandler<ReportChangedEventArgs> Change;

		internal Report()
		{
		}

		public abstract ReportParameterInfoCollection GetParameters();

		internal abstract ParametersPaneLayout GetParametersPaneLayout();

		public abstract void SetParameters(IEnumerable<ReportParameter> parameters);

		public abstract int GetTotalPages(out PageCountMode pageCountMode);

		public abstract RenderingExtension[] ListRenderingExtensions();

		public abstract void LoadReportDefinition(TextReader report);

		public abstract void Refresh();

		public abstract byte[] Render(string format, string deviceInfo, PageCountMode pageCountMode, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings);

		internal abstract byte[] InternalRenderStream(string format, string streamID, string deviceInfo, out string mimeType, out string encoding);

		internal abstract void InternalDeliverReportItem(string format, string deviceInfo, ExtensionSettings settings, string description, string eventType, string matchData);

		internal abstract int PerformSearch(string searchText, int startPage, int endPage);

		internal abstract void PerformToggle(string toggleId);

		internal abstract int PerformBookmarkNavigation(string bookmarkId, out string uniqueName);

		internal abstract int PerformDocumentMapNavigation(string documentMapId);

		public abstract ReportPageSettings GetDefaultPageSettings();

		public int GetTotalPages()
		{
			PageCountMode pageCountMode;
			return GetTotalPages(out pageCountMode);
		}

		public byte[] Render(string format)
		{
			return Render(format, null);
		}

		public byte[] Render(string format, string deviceInfo)
		{
			string mimeType;
			string encoding;
			string fileNameExtension;
			string[] streams;
			Warning[] warnings;
			return Render(format, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
		}

		public byte[] Render(string format, string deviceInfo, out string mimeType, out string encoding, out string fileNameExtension, out string[] streams, out Warning[] warnings)
		{
			return Render(format, deviceInfo, PageCountMode.Estimate, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
		}

		public DocumentMapNode GetDocumentMap()
		{
			return GetDocumentMap(DisplayNameForUse);
		}

		internal abstract Report PerformDrillthrough(string drillthroughId, out string reportPath);

		internal abstract int PerformSort(string sortId, SortOrder sortDirection, bool clearSort, PageCountMode pageCountMode, out string uniqueName);

		internal bool PrepareForRender()
		{
			lock (m_syncObject)
			{
				if (IsReadyForConnection)
				{
					EnsureExecutionSession();
					return IsPreparedReportReadyForRendering;
				}
				return false;
			}
		}

		internal abstract void EnsureExecutionSession();

		internal abstract DocumentMapNode GetDocumentMap(string rootLabel);

		internal abstract void SetCancelState(bool shouldCancel);

		public void LoadReportDefinition(Stream report)
		{
			if (report == null)
			{
				throw new ArgumentNullException("report");
			}
			LoadReportDefinition(new StreamReader(report));
		}

		internal void OnChange(bool isRefreshOnly)
		{
			if (this.Change != null)
			{
				this.Change(this, new ReportChangedEventArgs(isRefreshOnly));
			}
		}

		internal void OnChange(object sender, EventArgs e)
		{
			OnChange(isRefreshOnly: false);
		}

		public void SetParameters(ReportParameter parameter)
		{
			SetParameters(new ReportParameter[1]
			{
				parameter
			});
		}
	}
}
