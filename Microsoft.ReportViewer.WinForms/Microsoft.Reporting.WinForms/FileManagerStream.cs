using System.IO;

namespace Microsoft.Reporting.WinForms
{
	internal abstract class FileManagerStream
	{
		private Stream m_stream;

		public Stream Stream
		{
			get
			{
				return m_stream;
			}
			protected set
			{
				m_stream = value;
			}
		}

		public abstract void Delete();
	}
}
