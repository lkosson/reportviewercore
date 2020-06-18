using System.Diagnostics;

namespace Microsoft.Reporting.WinForms
{
	public sealed class ReportViewerStatus
	{
		private bool m_canNavigatePages;

		private bool m_canNavigateBack;

		private bool m_inCancelableOperation;

		private bool m_canRefreshData;

		private bool m_canPrint;

		private bool m_canSearch;

		private bool m_canContinueSearch;

		private bool m_canChangeDisplayModes;

		private bool m_canExport;

		private bool m_canChangeZoom;

		private bool m_isPromptingSupported;

		private bool m_hasPromptsToDisplay;

		private bool m_canChangePromptableValues;

		private bool m_arePromptsVisible;

		private bool m_docMapCanBeVisible;

		private bool m_docMapIsVisible;

		private bool m_canInteractWithReportPage;

		private bool m_isInFailedState;

		internal bool IsInFailedState => m_isInFailedState;

		internal bool CanInteractWithReportPage
		{
			[DebuggerStepThrough]
			get
			{
				return m_canInteractWithReportPage;
			}
		}

		public bool CanNavigatePages
		{
			[DebuggerStepThrough]
			get
			{
				return m_canNavigatePages;
			}
		}

		public bool CanNavigateBack
		{
			[DebuggerStepThrough]
			get
			{
				return m_canNavigateBack;
			}
		}

		public bool InCancelableOperation
		{
			[DebuggerStepThrough]
			get
			{
				return m_inCancelableOperation;
			}
		}

		public bool CanRefreshData
		{
			[DebuggerStepThrough]
			get
			{
				return m_canRefreshData;
			}
		}

		public bool CanPrint
		{
			[DebuggerStepThrough]
			get
			{
				return m_canPrint;
			}
		}

		public bool CanSearch
		{
			[DebuggerStepThrough]
			get
			{
				return m_canSearch;
			}
		}

		public bool CanContinueSearch
		{
			[DebuggerStepThrough]
			get
			{
				return m_canContinueSearch;
			}
		}

		public bool CanChangeDisplayModes
		{
			[DebuggerStepThrough]
			get
			{
				return m_canChangeDisplayModes;
			}
		}

		public bool CanExport
		{
			[DebuggerStepThrough]
			get
			{
				return m_canExport;
			}
		}

		public bool CanChangeZoom
		{
			[DebuggerStepThrough]
			get
			{
				return m_canChangeZoom;
			}
		}

		public bool IsPromptingSupported
		{
			[DebuggerStepThrough]
			get
			{
				return m_isPromptingSupported;
			}
		}

		public bool HasPromptsToDisplay
		{
			[DebuggerStepThrough]
			get
			{
				return m_hasPromptsToDisplay;
			}
		}

		public bool CanSubmitPromptAreaValues
		{
			[DebuggerStepThrough]
			get
			{
				return m_canChangePromptableValues;
			}
		}

		public bool ArePromptsVisible
		{
			[DebuggerStepThrough]
			get
			{
				return m_arePromptsVisible;
			}
		}

		public bool HasDocumentMapToDisplay
		{
			[DebuggerStepThrough]
			get
			{
				return m_docMapCanBeVisible;
			}
		}

		public bool IsDocumentMapVisible
		{
			[DebuggerStepThrough]
			get
			{
				return m_docMapIsVisible;
			}
		}

		internal ReportViewerStatus(ReportViewer viewer, UIState state, bool canContinueSearch, bool isPromptingSupported, bool hasPromptsToDisplay, bool hasDocumentMap)
		{
			m_isInFailedState = (state == UIState.ProcessingFailure);
			m_canNavigatePages = (state == UIState.ProcessingPartial || state == UIState.ProcessingSuccess);
			m_canNavigateBack = (viewer.Report.IsDrillthroughReport && state != UIState.LongRunningAction);
			m_inCancelableOperation = (state == UIState.LongRunningAction || state == UIState.ProcessingPartial);
			m_canRefreshData = (state == UIState.ProcessingFailure || state == UIState.ProcessingSuccess);
			m_canPrint = DoesStateAllowPrinting(state);
			m_canSearch = DoesStateAllowSearch(viewer, state);
			m_canContinueSearch = (m_canSearch && canContinueSearch);
			m_canChangeDisplayModes = (state != UIState.LongRunningAction && state != UIState.NoReport);
			m_canExport = DoesStateAllowExport(state);
			m_canChangeZoom = (state == UIState.ProcessingSuccess || state == UIState.ProcessingPartial);
			m_isPromptingSupported = isPromptingSupported;
			m_hasPromptsToDisplay = hasPromptsToDisplay;
			m_canChangePromptableValues = (m_canRefreshData || state == UIState.NoReport);
			m_arePromptsVisible = ShouldPromptsBeVisible(hasPromptsToDisplay, viewer.PromptAreaCollapsed, isPromptingSupported);
			m_docMapCanBeVisible = (hasDocumentMap && viewer.DisplayMode == DisplayMode.Normal);
			m_docMapIsVisible = (m_docMapCanBeVisible && !viewer.DocumentMapCollapsed);
			m_canInteractWithReportPage = DoesStateAllowInteractWithReportPage(state);
		}

		internal static bool DoesStateAllowInteractWithReportPage(UIState state)
		{
			return state != UIState.LongRunningAction;
		}

		internal static bool DoesStateAllowPrinting(UIState state)
		{
			if (state != UIState.ProcessingSuccess)
			{
				return state == UIState.ProcessingPartial;
			}
			return true;
		}

		internal static bool DoesStateAllowSearch(ReportViewer viewer, UIState state)
		{
			if (viewer.DisplayMode == DisplayMode.Normal)
			{
				return state == UIState.ProcessingSuccess;
			}
			return false;
		}

		internal static bool DoesStateAllowExport(UIState state)
		{
			return state == UIState.ProcessingSuccess;
		}

		internal static bool ShouldPromptsBeVisible(bool havePromptsToDisplay, bool isPromptAreaCollapsed, bool isPromptingSupported)
		{
			return havePromptsToDisplay && !isPromptAreaCollapsed && isPromptingSupported;
		}
	}
}
