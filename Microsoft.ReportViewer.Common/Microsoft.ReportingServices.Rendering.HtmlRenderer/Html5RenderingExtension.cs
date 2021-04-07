using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using Microsoft.ReportingServices.ReportProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Web.UI;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[StrongNameIdentityPermission(SecurityAction.InheritanceDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	class Html5RenderingExtension : RenderingExtensionBase
	{
		public override string LocalizedName => RenderRes.HTML5LocalizedName;

		protected override bool InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			HtmlTextWriter htmlTextWriter = null;
			Html5ServerRenderer html5ServerRenderer = null;
			try
			{
				htmlTextWriter = HtmlWriterFactory.CreateWriter(report.Name, "text/html", createAndRegisterStream, StreamOper.CreateAndRegister);
				DeviceInfo deviceInfo2 = null;
				try
				{
					deviceInfo2 = new ServerDeviceInfo();
					deviceInfo2.ParseDeviceInfo(deviceInfo, clientCapabilities);
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new ReportRenderingException(RenderRes.rrInvalidDeviceInfo, innerException);
				}
				bool onlyVisibleStyles = deviceInfo2.OnlyVisibleStyles;
				int totalPages = 0;
				if (renderProperties != null)
				{
					object obj = renderProperties["ClientPaginationMode"];
					if (obj != null)
					{
						PaginationMode paginationMode = (PaginationMode)obj;
						if (paginationMode == PaginationMode.TotalPages)
						{
							object obj2 = renderProperties["PreviousTotalPages"];
							if (obj2 != null && obj2 is int)
							{
								totalPages = (int)obj2;
							}
						}
					}
				}
				if (deviceInfo2.BookmarkId != null)
				{
					string uniqueName = null;
					SPBInteractivityProcessing sPBInteractivityProcessing = new SPBInteractivityProcessing();
					int section = sPBInteractivityProcessing.ProcessBookmarkNavigationEvent(report, totalPages, deviceInfo2.BookmarkId, out uniqueName);
					if (uniqueName != null)
					{
						deviceInfo2.Section = section;
						deviceInfo2.NavigationId = uniqueName;
					}
				}
				try
				{
					html5ServerRenderer = CreateRenderer(report, reportServerParameters, deviceInfo2, deviceInfo, clientCapabilities, createAndRegisterStream, ref renderProperties, totalPages);
				}
				catch (InvalidSectionException innerException2)
				{
					throw new ReportRenderingException(innerException2);
				}
				html5ServerRenderer.Render(htmlTextWriter);
				html5ServerRenderer.UpdateRenderProperties(ref renderProperties);
				return false;
			}
			finally
			{
				html5ServerRenderer?.Dispose();
				htmlTextWriter?.Flush();
			}
		}

		internal virtual Html5ServerRenderer CreateRenderer(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, ref Hashtable renderProperties, int totalPages)
		{
			Dictionary<string, string> globalBookmarks = new Dictionary<string, string>();
			if (!deviceInfo.HasActionScript && (deviceInfo.Section == 0 || !deviceInfo.AllowScript))
			{
				globalBookmarks = Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing.CollectBookmarks(report, totalPages);
			}
			Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing = new Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing(report, createAndRegisterStreamCallback, registerEvents: true, ref renderProperties);
			SecondaryStreams secondaryStreams = SecondaryStreams.Embedded;
			Html5ServerRenderer html5ServerRenderer = new Html5ServerRenderer(new ROMReport(report), spbProcessing, reportServerParams, deviceInfo, rawDeviceInfo, browserCaps, createAndRegisterStreamCallback, secondaryStreams);
			html5ServerRenderer.InitializeReport();
			SetParameters(html5ServerRenderer, deviceInfo, report);
			html5ServerRenderer.GlobalBookmarks = globalBookmarks;
			return html5ServerRenderer;
		}

		internal void SetParameters(Html5ServerRenderer sr, DeviceInfo deviceInfo, Microsoft.ReportingServices.OnDemandReportRendering.Report report)
		{
			if (!deviceInfo.HTMLFragment && report.Parameters != null)
			{
				sr.Parameters = report.Parameters;
			}
		}

		protected override bool InternalRenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			if (streamName == null || streamName.Length <= 0)
			{
				return false;
			}
			if (Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing.RenderSecondaryStream(report, createAndRegisterStream, streamName))
			{
				return true;
			}
			char c = '_';
			char[] separator = new char[1]
			{
				c
			};
			string[] array = streamName.Split(separator);
			if (array.Length < 2)
			{
				return false;
			}
			string text = report.Name + c + "style";
			if (streamName.StartsWith(text, StringComparison.Ordinal))
			{
				DeviceInfo deviceInfo2 = null;
				try
				{
					deviceInfo2 = new ServerDeviceInfo();
					deviceInfo2.ParseDeviceInfo(deviceInfo, clientCapabilities);
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new ReportRenderingException(RenderRes.rrInvalidDeviceInfo, innerException);
				}
				if (streamName.Length > text.Length && deviceInfo2.Section == 0)
				{
					int result = 0;
					string s = streamName.Substring(text.Length + 1);
					if (int.TryParse(s, out result))
					{
						deviceInfo2.Section = result;
					}
				}
				if (!deviceInfo2.OnlyVisibleStyles || deviceInfo2.Section == 0)
				{
					Stream stream = createAndRegisterStream(streamName, "css", Encoding.UTF8, "text/css", willSeek: false, StreamOper.CreateAndRegister);
					HTMLStyleRenderer hTMLStyleRenderer = new HTMLStyleRenderer(report, createAndRegisterStream, deviceInfo2, null);
					hTMLStyleRenderer.Render(stream);
					stream.Flush();
				}
				else
				{
					Html5ServerRenderer html5ServerRenderer = null;
					try
					{
						html5ServerRenderer = CreateRenderer(report, reportServerParameters, deviceInfo2, deviceInfo, clientCapabilities, createAndRegisterStream, ref renderProperties, 0);
						html5ServerRenderer.RenderStylesOnly(streamName);
						html5ServerRenderer.UpdateRenderProperties(ref renderProperties);
					}
					catch (InvalidSectionException innerException2)
					{
						throw new ReportRenderingException(innerException2);
					}
					finally
					{
						html5ServerRenderer?.Dispose();
					}
				}
				return true;
			}
			return false;
		}
	}
}
