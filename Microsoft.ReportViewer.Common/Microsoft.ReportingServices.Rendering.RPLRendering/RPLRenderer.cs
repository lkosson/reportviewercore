using Microsoft.ReportingServices.Common;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.Rendering.SPBProcessing;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.RPLRendering
{
	internal sealed class RPLRenderer : IRenderingExtension, IExtension, ITotalPages
	{
		private const string PARAM_START_PAGE = "StartPage";

		private const string PARAM_END_PAGE = "EndPage";

		private const string PARAM_MEASURE_ITEMS = "MeasureItems";

		private const string PARAM_TOGGLE_ITEMS = "ToggleItems";

		private const string PARAM_SECONDARY_STREAMS = "SecondaryStreams";

		private const string PARAM_STREAM_NAMES = "StreamNames";

		private const string PARAM_RPL_VERSION = "RPLVersion";

		private const string PARAM_IMAGE_CONSOLIDATION = "ImageConsolidation";

		private const string PARAM_CONVERT_IMAGES = "ConvertImages";

		private const string MIME_EXTENSION = "rpl";

		private const string MIME_TYPE = "application/octet-stream";

		private string m_rplVersion;

		private SPBContext m_spbContext = new SPBContext();

		public string LocalizedName => RenderRes.RPLLocalizedName;

		static RPLRenderer()
		{
		}

		internal static SecondaryStreams ParseSecondaryStreamsParam(string secondaryStreamStr, SecondaryStreams defaultValue)
		{
			if (SecondaryStreams.Embedded.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Embedded;
			}
			if (SecondaryStreams.Server.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Server;
			}
			if (SecondaryStreams.Temporary.ToString().Equals(secondaryStreamStr, StringComparison.OrdinalIgnoreCase))
			{
				return SecondaryStreams.Temporary;
			}
			return defaultValue;
		}

		internal static bool ParseBool(string boolValue, bool defaultValue)
		{
			if (bool.TryParse(boolValue, out bool result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static int ParseInt(string intValue, int defaultValue)
		{
			if (int.TryParse(intValue, out int result))
			{
				return result;
			}
			return defaultValue;
		}

		internal static double ParseDouble(string doubleValue, double defaultValue)
		{
			if (double.TryParse(doubleValue, out double result))
			{
				return result;
			}
			return defaultValue;
		}

		private void ParseParameters(NameValueCollection deviceInfo)
		{
			m_spbContext.StartPage = ParseInt(deviceInfo["StartPage"], 1);
			m_spbContext.EndPage = ParseInt(deviceInfo["EndPage"], 1);
			m_spbContext.MeasureItems = ParseBool(deviceInfo["MeasureItems"], defaultValue: false);
			m_spbContext.AddToggledItems = ParseBool(deviceInfo["ToggleItems"], defaultValue: false);
			m_spbContext.SecondaryStreams = ParseSecondaryStreamsParam(deviceInfo["SecondaryStreams"], SecondaryStreams.Server);
			m_spbContext.AddSecondaryStreamNames = ParseBool(deviceInfo["StreamNames"], defaultValue: true);
			m_spbContext.UseImageConsolidation = ParseBool(deviceInfo["ImageConsolidation"], defaultValue: false);
			m_spbContext.ConvertImages = ParseBool(deviceInfo["ConvertImages"], defaultValue: false);
			m_rplVersion = deviceInfo["RPLVersion"];
		}

		public bool Render(Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				ParseParameters(deviceInfo);
				Stream outputStream = createAndRegisterStream(report.Name, "rpl", null, "application/octet-stream", willSeek: false, StreamOper.CreateAndRegister);
				Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing sPBProcessing = null;
				using (sPBProcessing = new Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing(report, createAndRegisterStream, registerEvents: true, m_rplVersion, ref renderProperties))
				{
					sPBProcessing.SetContext(m_spbContext);
					sPBProcessing.GetNextPage(outputStream);
					sPBProcessing.UpdateRenderProperties(ref renderProperties);
				}
				return false;
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		public bool RenderStream(string streamName, Microsoft.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection reportServerParameters, NameValueCollection deviceInfo, NameValueCollection clientCapabilities, ref Hashtable renderProperties, CreateAndRegisterStream createAndRegisterStream)
		{
			try
			{
				if (string.IsNullOrEmpty(streamName))
				{
					return false;
				}
				return Microsoft.ReportingServices.Rendering.SPBProcessing.SPBProcessing.RenderSecondaryStream(report, createAndRegisterStream, streamName);
			}
			catch (ReportRenderingException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				if (AsynchronousExceptionDetection.IsStoppingException(ex2))
				{
					throw;
				}
				throw new ReportRenderingException(ex2, unexpected: true);
			}
		}

		public void GetRenderingResource(CreateAndRegisterStream createAndRegisterStreamCallback, NameValueCollection deviceInfo)
		{
		}

		public void SetConfiguration(string configuration)
		{
		}
	}
}
