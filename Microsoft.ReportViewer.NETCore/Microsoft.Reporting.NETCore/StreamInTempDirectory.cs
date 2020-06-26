using System.IO;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class StreamInTempDirectory : FileManagerStream
	{
		private string m_fileName;

		public StreamInTempDirectory()
		{
			m_fileName = Path.GetTempFileName();
			base.Stream = File.Open(m_fileName, FileMode.Truncate, FileAccess.ReadWrite, FileShare.None);
		}

		public override void Delete()
		{
			if (m_fileName != null)
			{
				base.Stream.Close();
				File.Delete(m_fileName);
				m_fileName = null;
				base.Stream = null;
			}
		}
	}
}
