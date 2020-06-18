using Microsoft.ReportingServices.Interfaces;
using System.IO;

namespace Microsoft.ReportingServices.OnDemandProcessing.Scalability
{
	internal sealed class CreateAndRegisterStreamHandler : IStreamHandler
	{
		private string m_streamName;

		private CreateAndRegisterStream m_createStreamCallback;

		internal CreateAndRegisterStreamHandler(string streamName, CreateAndRegisterStream createStreamCallback)
		{
			m_streamName = streamName;
			m_createStreamCallback = createStreamCallback;
		}

		public Stream OpenStream()
		{
			return m_createStreamCallback(m_streamName, null, null, null, willSeek: true, StreamOper.CreateOnly);
		}
	}
}
