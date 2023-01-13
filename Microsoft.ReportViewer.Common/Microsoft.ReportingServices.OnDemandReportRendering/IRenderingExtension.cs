using Microsoft.ReportingServices.Interfaces;
using System.Collections;
using System.Collections.Specialized;
using System.Security.Permissions;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal interface IRenderingExtension : IExtension
	{
		bool Render(Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);

		void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo);

		bool RenderStream(string streamName, Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream);
	}
}
