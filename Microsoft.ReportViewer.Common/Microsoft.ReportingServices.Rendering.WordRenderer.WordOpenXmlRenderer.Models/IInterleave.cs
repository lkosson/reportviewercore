using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal interface IInterleave : IStorable, IPersistable
	{
		int Index
		{
			get;
		}

		long Location
		{
			get;
		}

		void Write(TextWriter output);
	}
}
