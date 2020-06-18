using System;

namespace Microsoft.ReportingServices.Common
{
	internal sealed class CommonDataComparerException : Exception, IDataComparisonError
	{
		private string m_typeX;

		private string m_typeY;

		public string TypeX => m_typeX;

		public string TypeY => m_typeY;

		internal CommonDataComparerException(string typeX, string typeY)
		{
			m_typeX = typeX;
			m_typeY = typeY;
		}
	}
}
