using System;

namespace Microsoft.ReportingServices.Interfaces
{
	public abstract class Report
	{
		public abstract string Name
		{
			get;
		}

		public abstract string URL
		{
			get;
		}

		public abstract DateTime Date
		{
			get;
		}

		public abstract RenderedOutputFile[] Render(string renderFormat, string deviceInfo);
	}
}
