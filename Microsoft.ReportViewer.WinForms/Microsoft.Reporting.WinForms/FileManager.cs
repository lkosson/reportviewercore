using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class FileManager
	{
		[Flags]
		private enum CreateFailureFlags
		{
			None = 0x0,
			Temp = 0x1,
			IsolatedStorage = 0x2,
			All = 0x3
		}

		private List<FileManagerStream> m_pages = new List<FileManagerStream>();

		private List<FileManagerStream> m_nonRegisteredStreams = new List<FileManagerStream>();

		private FileManagerStatus m_status;

		private ManualResetEvent m_waitingForNextPage = new ManualResetEvent(initialState: true);

		private object m_lockObject = new object();

		private CreateFailureFlags m_createFailures;

		public FileManagerStatus Status
		{
			get
			{
				lock (m_lockObject)
				{
					return m_status;
				}
			}
			set
			{
				lock (m_lockObject)
				{
					m_status = value;
					if (m_status != FileManagerStatus.InProgress)
					{
						m_waitingForNextPage.Set();
					}
				}
			}
		}

		public int Count
		{
			get
			{
				lock (m_lockObject)
				{
					return m_pages.Count;
				}
			}
		}

		public void Clean()
		{
			lock (m_lockObject)
			{
				foreach (FileManagerStream page in m_pages)
				{
					DeleteStream(page);
				}
				foreach (FileManagerStream nonRegisteredStream in m_nonRegisteredStreams)
				{
					DeleteStream(nonRegisteredStream);
				}
				m_pages.Clear();
				m_nonRegisteredStreams.Clear();
				m_waitingForNextPage.Set();
				m_status = FileManagerStatus.NotStarted;
			}
		}

		private void DeleteStream(FileManagerStream s)
		{
			try
			{
				s.Delete();
			}
			catch
			{
			}
		}

		public Stream CreatePage(bool register)
		{
			lock (m_lockObject)
			{
				if (m_status != 0 && m_status != FileManagerStatus.InProgress)
				{
					Clean();
				}
				FileManagerStream fileManagerStream = CreateStream();
				if (register)
				{
					m_pages.Add(fileManagerStream);
				}
				else
				{
					m_nonRegisteredStreams.Add(fileManagerStream);
				}
				m_status = FileManagerStatus.InProgress;
				m_waitingForNextPage.Set();
				return fileManagerStream.Stream;
			}
		}

		public Stream Get(int page)
		{
			while (true)
			{
				lock (m_lockObject)
				{
					if (m_pages.Count > page || m_status == FileManagerStatus.Aborted || m_status == FileManagerStatus.Complete)
					{
						break;
					}
					goto IL_003f;
				}
				IL_003f:
				WaitForNextPage();
			}
			lock (m_lockObject)
			{
				if (page > m_pages.Count)
				{
					return null;
				}
				return m_pages[page - 1].Stream;
			}
		}

		private FileManagerStream CreateStream()
		{
			if ((m_createFailures & CreateFailureFlags.IsolatedStorage) == 0)
			{
				try
				{
					return new StreamInIsolatedStorage();
				}
				catch
				{
					m_createFailures |= CreateFailureFlags.IsolatedStorage;
				}
			}
			if ((m_createFailures & CreateFailureFlags.Temp) == 0)
			{
				try
				{
					return new StreamInTempDirectory();
				}
				catch
				{
					m_createFailures |= CreateFailureFlags.Temp;
				}
			}
			return new StreamInMemory();
		}

		private void WaitForNextPage()
		{
			lock (m_lockObject)
			{
				if (m_status == FileManagerStatus.InProgress)
				{
					m_waitingForNextPage.Reset();
				}
			}
			m_waitingForNextPage.WaitOne();
		}
	}
}
