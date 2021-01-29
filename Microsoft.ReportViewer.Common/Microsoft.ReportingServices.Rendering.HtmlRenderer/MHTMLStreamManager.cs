using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal sealed class MHTMLStreamManager
	{
		public class MHTMLStream
		{
			public string m_name = "";

			public string extension = "";

			public Encoding encoding = Encoding.UTF8;

			public string mimeType = "";

			public bool willSeek;

			public string tempFileName = "";

			public Stream m_stream;

			public MHTMLStream(Stream stream)
			{
				m_stream = stream;
			}
		}

		public ArrayList m_streams = new ArrayList();

		private readonly CreateAndRegisterStream m_createAndRegisterStream;

		public MHTMLStreamManager(CreateAndRegisterStream createAndRegisterStream)
		{
			RSTrace.RenderingTracer.Assert(createAndRegisterStream != null, "createAndRegisterStream");
			m_createAndRegisterStream = createAndRegisterStream;
		}

		public Stream CreateStream(string name, string extension, Encoding encoding, string mimeType, bool willSeek, StreamOper operation)
		{
			StreamOper streamOper = operation;
			if (streamOper == StreamOper.CreateAndRegister)
			{
				streamOper = StreamOper.CreateOnly;
			}
			Stream stream = m_createAndRegisterStream(name, extension, encoding, mimeType, willSeek, streamOper);
			if (stream == null)
			{
				return null;
			}
			MHTMLStream mHTMLStream = new MHTMLStream(stream);
			mHTMLStream.m_name = name;
			mHTMLStream.extension = extension;
			mHTMLStream.encoding = encoding;
			mHTMLStream.mimeType = mimeType;
			mHTMLStream.willSeek = willSeek;
			if (StreamOper.CreateOnly != operation)
			{
				m_streams.Add(mHTMLStream);
			}
			return mHTMLStream.m_stream;
		}

		public void CloseAllStreams()
		{
			if (m_streams.Count == 0)
			{
				return;
			}
			Exception ex = null;
			foreach (MHTMLStream stream in m_streams)
			{
				try
				{
					stream.m_stream.Close();
				}
				catch (Exception ex2)
				{
					RSTrace.RenderingTracer.Trace(TraceLevel.Error, "Exception thrown when closing result buffered stream: {0}", ex2);
					ex = ex2;
				}
			}
			if (ex == null)
			{
				return;
			}
			throw ex;
		}
	}
}
