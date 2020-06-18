using System;
using System.Threading;

namespace Microsoft.ReportingServices.Common
{
	internal class AsynchronousExceptionDetection
	{
		private AsynchronousExceptionDetection()
		{
		}

		internal static bool IsStoppingException(Exception e)
		{
			if (e is OutOfMemoryException || e is ExecutionEngineException || e is StackOverflowException || e is ThreadAbortException)
			{
				return true;
			}
			return false;
		}
	}
}
