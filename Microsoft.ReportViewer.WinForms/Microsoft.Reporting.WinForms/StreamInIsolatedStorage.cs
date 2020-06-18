using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class StreamInIsolatedStorage : FileManagerStream
	{
		private string m_fileName;

		private IsolatedStorageFile m_isf;

		private IsolatedStorageFile IsolatedStorage
		{
			get
			{
				if (m_isf == null)
				{
					m_isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
				}
				return m_isf;
			}
		}

		public StreamInIsolatedStorage()
		{
			m_fileName = "ReportPageTempFile-" + Guid.NewGuid().ToString(null, CultureInfo.InvariantCulture);
			base.Stream = new IsolatedStorageFileStream(m_fileName, FileMode.Create, FileAccess.ReadWrite, IsolatedStorage);
		}

		public override void Delete()
		{
			if (m_fileName != null)
			{
				base.Stream.Close();
				IsolatedStorage.DeleteFile(m_fileName);
				m_fileName = null;
				base.Stream = null;
			}
		}
	}
}
