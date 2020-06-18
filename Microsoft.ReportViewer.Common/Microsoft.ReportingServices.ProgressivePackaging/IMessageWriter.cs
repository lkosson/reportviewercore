using System;
using System.IO;

namespace Microsoft.ReportingServices.ProgressivePackaging
{
	internal interface IMessageWriter : IDisposable
	{
		void WriteMessage(string name, object value);

		Stream CreateWritableStream(string name);
	}
}
