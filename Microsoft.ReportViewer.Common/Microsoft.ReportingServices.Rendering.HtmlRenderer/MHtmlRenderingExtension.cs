using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	[StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey = "0024000004800000940000000602000000240000525341310004000001000100272736ad6e5f9586bac2d531eabc3acc666c2f8ec879fa94f8f7b0327d2ff2ed523448f83c3d5c5dd2dfc7bc99c5286b2c125117bf5cbe242b9d41750732b2bdffe649c6efb8e5526d526fdd130095ecdb7bf210809c6cdad8824faa9ac0310ac3cba2aa0523567b2dfa7fe250b30facbd62d4ec99b94ac47c7d3b28f1f6e4c8")]
	sealed class MHtmlRenderingExtension : Html40RenderingExtension
	{
		private enum MhtmlDeviceInfoTags
		{
			JavaScript,
			MhtmlFragment,
			StyleStream,
			Section,
			OutlookCompat,
			AccessibleTablix,
			DataVisualizationFitSizing
		}

		private const string _contentType = "multipart/related";

		private const string _mhtmlExtension = "mhtml";

		public override string LocalizedName => RenderRes.MHTMLLocalizedName;

		internal override ServerRenderer CreateRenderer(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParams, DeviceInfo deviceInfo, NameValueCollection rawDeviceInfo, NameValueCollection browserCaps, CreateAndRegisterStream createAndRegisterStreamCallback, ref Hashtable renderProperties, int totalPages)
		{
			Dictionary<string, string> globalBookmarks = Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing.CollectBookmarks(report, totalPages);
			Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing spbProcessing = new Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing(report, createAndRegisterStreamCallback, registerEvents: false, ref renderProperties);
			SecondaryStreams secondaryStreams = SecondaryStreams.Temporary;
			ServerRenderer serverRenderer = new MHTMLRenderer(new ROMReport(report), spbProcessing, reportServerParams, deviceInfo, rawDeviceInfo, browserCaps, createAndRegisterStreamCallback, secondaryStreams);
			serverRenderer.InitializeReport();
			SetParameters(serverRenderer, deviceInfo, report);
			serverRenderer.GlobalBookmarks = globalBookmarks;
			return serverRenderer;
		}

		protected override bool InternalRender(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			NameValueCollection deviceInfo2 = ResolveDeviceInfo(deviceInfo);
			clientCapabilities.Remove(BrowserDetectionUtility.JavaScriptKey);
			MHTMLStreamManager mHTMLStreamManager = new MHTMLStreamManager(createAndRegisterStream);
			try
			{
				bool flag = CheckForHtmlOnlyDeviceInfo(deviceInfo2);
				bool result = base.InternalRender(report, reportServerParameters, deviceInfo2, clientCapabilities, ref renderProperties, flag ? createAndRegisterStream : new CreateAndRegisterStream(mHTMLStreamManager.CreateStream));
				if (flag)
				{
					return result;
				}
				MHTMLStreamManager.MHTMLStream mHTMLStream = (MHTMLStreamManager.MHTMLStream)mHTMLStreamManager.m_streams[0];
				Stream stream = createAndRegisterStream(mHTMLStream.m_name, "mhtml", new ASCIIEncoding(), "multipart/related", willSeek: false, StreamOper.CreateAndRegister);
				MhtmlWriter mhtmlWriter = new MhtmlWriter();
				mhtmlWriter.StartMimeDocument(stream);
				foreach (MHTMLStreamManager.MHTMLStream stream2 in mHTMLStreamManager.m_streams)
				{
					mhtmlWriter.WriteMimeBodyPart(stream2.m_stream, stream2.m_name, stream2.mimeType, stream2.encoding);
				}
				mhtmlWriter.EndMimeDocument();
				stream.Flush();
				return result;
			}
			finally
			{
				mHTMLStreamManager.CloseAllStreams();
			}
		}

		private NameValueCollection ResolveDeviceInfo(NameValueCollection deviceInfo)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("ActiveXControls", "False");
			nameValueCollection.Add("Toolbar", "False");
			nameValueCollection.Add("ImageConsolidation", "False");
			string strB = MhtmlDeviceInfoTags.JavaScript.ToString();
			string strB2 = MhtmlDeviceInfoTags.Section.ToString();
			string strB3 = MhtmlDeviceInfoTags.MhtmlFragment.ToString();
			string strB4 = MhtmlDeviceInfoTags.StyleStream.ToString();
			string strB5 = MhtmlDeviceInfoTags.AccessibleTablix.ToString();
			string strB6 = MhtmlDeviceInfoTags.DataVisualizationFitSizing.ToString();
			string text = MhtmlDeviceInfoTags.OutlookCompat.ToString();
			bool result = true;
			for (int i = 0; i < deviceInfo.AllKeys.Length; i++)
			{
				string text2 = deviceInfo.AllKeys[i];
				if (string.Compare(text2, strB, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text2, strB3, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text2, strB4, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text2, strB5, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text2, strB2, StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(text2, strB6, StringComparison.OrdinalIgnoreCase) == 0)
				{
					nameValueCollection[text2] = deviceInfo[text2];
				}
				else if (string.Compare(text2, text, StringComparison.OrdinalIgnoreCase) == 0)
				{
					string value = deviceInfo[text2];
					bool.TryParse(value, out result);
				}
			}
			nameValueCollection[text] = result.ToString();
			return nameValueCollection;
		}

		private bool CheckForHtmlOnlyDeviceInfo(NameValueCollection deviceInfo)
		{
			bool result = false;
			string strB = MhtmlDeviceInfoTags.MhtmlFragment.ToString();
			for (int i = 0; i < deviceInfo.AllKeys.Length; i++)
			{
				string text = deviceInfo.AllKeys[i];
				if (string.Compare(text, strB, StringComparison.OrdinalIgnoreCase) == 0)
				{
					deviceInfo.Add("HTMLFragment", deviceInfo[text]);
					deviceInfo.Remove(text);
					result = true;
					break;
				}
			}
			return result;
		}

		protected override bool InternalRenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			return base.InternalRenderStream(streamName, report, reportServerParameters, deviceInfo, clientCapabilities, ref renderProperties, createAndRegisterStream);
		}
	}
}
