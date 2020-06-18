using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Reporting
{
	internal sealed class ProcessingStreamHandler : IDisposable
	{
		private bool m_allowOnlyTemporaryStreams;

		private CreateAndRegisterStream m_createStreamCallback;

		private List<Stream> m_streams = new List<Stream>();

		public ProcessingStreamHandler(CreateAndRegisterStream createStreamCallback)
		{
			m_allowOnlyTemporaryStreams = false;
			m_createStreamCallback = createStreamCallback;
		}

		public ProcessingStreamHandler()
		{
			m_allowOnlyTemporaryStreams = true;
		}

		public void Dispose()
		{
			foreach (Stream stream in m_streams)
			{
				stream.Close();
			}
			m_streams.Clear();
		}

		public Stream StreamCallback(string name, string extension, Encoding encoding, string mimeType, bool useChunking, StreamOper operation)
		{
			if (operation == StreamOper.RegisterOnly)
			{
				return null;
			}
			if (m_allowOnlyTemporaryStreams && operation != StreamOper.CreateOnly)
			{
				throw new InvalidOperationException("Only temporary streams are allowed by this StreamHandler");
			}
			if ((operation == StreamOper.CreateAndRegister || operation == StreamOper.CreateForPersistedStreams) && m_createStreamCallback != null)
			{
				return m_createStreamCallback(name, extension, encoding, mimeType, useChunking, operation);
			}
			MemoryStream memoryStream = new MemoryStream();
			m_streams.Add(memoryStream);
			return memoryStream;
		}
	}
}
