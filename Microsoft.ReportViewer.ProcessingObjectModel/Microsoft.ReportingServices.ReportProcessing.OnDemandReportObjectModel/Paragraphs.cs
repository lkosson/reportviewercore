using System;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel
{
	public abstract class Paragraphs : MarshalByRefObject
	{
		public abstract Paragraph this[int index]
		{
			get;
		}
	}
}
