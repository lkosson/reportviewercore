using System;
using System.Drawing.Printing;
using System.Timers;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class ReportInfo : IDisposable
	{
		public int CurrentPage;

		private ClientGDIRenderer m_gdiRenderer;

		private LocalReport m_localReport;

		private ServerReport m_serverReport;

		private FileManager m_fileManager = new FileManager();

		private Timer m_serverSessionTimer;

		private object m_timerLock = new object();

		private PageSettings m_pageSettings;

		private bool m_useTimer;

		public LocalReport LocalReport => m_localReport;

		public ServerReport ServerReport => m_serverReport;

		public ClientGDIRenderer GdiRenderer => m_gdiRenderer;

		public FileManager FileManager => m_fileManager;

		public PageSettings PageSettings
		{
			get
			{
				return m_pageSettings;
			}
			set
			{
				m_pageSettings = value;
			}
		}

		public ReportInfo(LocalReport localReport, ServerReport serverReport)
		{
			m_localReport = localReport;
			m_serverReport = serverReport;
		}

		public void Dispose()
		{
			StopTimer();
			m_fileManager.Clean();
			m_localReport.Dispose();
			ClearGdiPage();
		}

		public void SetNewGdiPage(byte[] pageBytes)
		{
			m_gdiRenderer = new ClientGDIRenderer(pageBytes);
		}

		public void ClearGdiPage()
		{
			if (m_gdiRenderer != null)
			{
				m_gdiRenderer.Dispose();
				m_gdiRenderer = null;
			}
		}

		public void StartTimer()
		{
			lock (m_timerLock)
			{
				if (!m_useTimer)
				{
					m_useTimer = true;
					m_serverReport.ExecutionIDChanged += OnServerExecutionIdChanged;
				}
			}
			OnServerExecutionIdChanged(m_serverReport, EventArgs.Empty);
		}

		public void StopTimer()
		{
			lock (m_timerLock)
			{
				DisposeTimer();
				if (m_useTimer)
				{
					m_serverReport.ExecutionIDChanged -= OnServerExecutionIdChanged;
					m_useTimer = false;
				}
			}
		}

		private void OnServerExecutionIdChanged(object sender, EventArgs e)
		{
			lock (m_timerLock)
			{
				DisposeTimer();
				if (m_serverReport.HasExecutionId)
				{
					m_serverSessionTimer = new Timer();
					int num = (int)(m_serverReport.GetExecutionSessionExpiration() - DateTime.Now.ToUniversalTime()).TotalSeconds;
					num -= 60;
					if (num >= 60)
					{
						m_serverSessionTimer.Interval = num * 1000;
						m_serverSessionTimer.Elapsed += TimerProc;
						m_serverSessionTimer.Start();
					}
				}
			}
		}

		private void TimerProc(object sender, ElapsedEventArgs e)
		{
			lock (m_timerLock)
			{
				try
				{
					m_serverReport.TouchSession();
				}
				catch
				{
					DisposeTimer();
				}
			}
		}

		private void DisposeTimer()
		{
			if (m_serverSessionTimer != null)
			{
				m_serverSessionTimer.Stop();
				m_serverSessionTimer.Dispose();
				m_serverSessionTimer = null;
			}
		}
	}
}
