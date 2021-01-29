using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class LineRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		private bool hasSlantedLines;

		public LineRenderer(HTML5Renderer renderer)
		{
			html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderLine((RPLLine)reportItem, elementProps, (RPLLinePropsDef)definition, measurement, renderId, styleContext);
		}

		protected void RenderLine(RPLLine reportItem, RPLElementProps rplProps, RPLLinePropsDef rplPropsDef, RPLItemMeasurement measurement, bool renderId, StyleContext styleContext)
		{
			if (html5Renderer.IsLineSlanted(measurement))
			{
				if (renderId)
				{
					html5Renderer.RenderNavigationId(rplProps.UniqueName);
				}
				if (html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Quirks)
				{
					RenderVMLLine(reportItem, measurement, styleContext);
				}
				return;
			}
			bool flag = measurement.Height == 0f;
			html5Renderer.WriteStream(HTMLElements.m_openSpan);
			if (renderId)
			{
				html5Renderer.RenderReportItemId(rplProps.UniqueName);
			}
			int borderContext = 0;
			object obj = rplProps.Style[10];
			if (obj != null)
			{
				html5Renderer.OpenStyle();
				if (flag)
				{
					html5Renderer.WriteStream(HTMLElements.m_styleHeight);
				}
				else
				{
					html5Renderer.WriteStream(HTMLElements.m_styleWidth);
				}
				html5Renderer.WriteStream(obj);
				html5Renderer.WriteStream(HTMLElements.m_semiColon);
			}
			obj = rplProps.Style[0];
			if (obj != null)
			{
				html5Renderer.OpenStyle();
				html5Renderer.WriteStream(HTMLElements.m_backgroundColor);
				html5Renderer.WriteStream(obj);
			}
			html5Renderer.RenderReportItemStyle(reportItem, measurement, ref borderContext);
			html5Renderer.CloseStyle(renderQuote: true);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			html5Renderer.WriteStream(HTMLElements.m_closeSpan);
		}

		private void RenderVMLLine(RPLLine line, RPLItemMeasurement measurement, StyleContext styleContext)
		{
			if (!hasSlantedLines)
			{
				html5Renderer.WriteStream("<?XML:NAMESPACE PREFIX=v /><?IMPORT NAMESPACE=\"v\" IMPLEMENTATION=\"#default#VML\" />");
				hasSlantedLines = true;
			}
			html5Renderer.WriteStream(HTMLElements.m_openVGroup);
			html5Renderer.WriteStream(HTMLElements.m_openStyle);
			html5Renderer.WriteStream(HTMLElements.m_styleWidth);
			if (styleContext.InTablix)
			{
				html5Renderer.WriteStream(HTMLElements.m_percent);
				html5Renderer.WriteStream(HTMLElements.m_semiColon);
				html5Renderer.WriteStream(HTMLElements.m_styleHeight);
				html5Renderer.WriteStream(HTMLElements.m_percent);
			}
			else
			{
				html5Renderer.WriteRSStream(measurement.Width);
				html5Renderer.WriteStream(HTMLElements.m_semiColon);
				html5Renderer.WriteStream(HTMLElements.m_styleHeight);
				html5Renderer.WriteRSStream(measurement.Height);
			}
			html5Renderer.WriteStream(HTMLElements.m_closeQuote);
			html5Renderer.WriteStream(HTMLElements.m_openVLine);
			if (((RPLLinePropsDef)line.ElementProps.Definition).Slant)
			{
				html5Renderer.WriteStream(HTMLElements.m_rightSlant);
			}
			else
			{
				html5Renderer.WriteStream(HTMLElements.m_leftSlant);
			}
			IRPLStyle style = line.ElementProps.Style;
			string text = (string)style[0];
			string text2 = (string)style[10];
			if (text != null && text2 != null)
			{
				int value = new RPLReportColor(text).ToColor().ToArgb() & 0xFFFFFF;
				html5Renderer.WriteStream(HTMLElements.m_strokeColor);
				html5Renderer.WriteStream("#");
				html5Renderer.WriteStream(Convert.ToString(value, 16));
				html5Renderer.WriteStream(HTMLElements.m_quote);
				html5Renderer.WriteStream(HTMLElements.m_strokeWeight);
				html5Renderer.WriteStream(text2);
				html5Renderer.WriteStream(HTMLElements.m_closeQuote);
			}
			string theString = "solid";
			string text3 = null;
			object obj = style[5];
			if (obj != null)
			{
				string value2 = EnumStrings.GetValue((RPLFormat.BorderStyles)obj);
				if (string.CompareOrdinal(value2, "dashed") == 0)
				{
					theString = "dash";
				}
				else if (string.CompareOrdinal(value2, "dotted") == 0)
				{
					theString = "dot";
				}
				if (string.CompareOrdinal(value2, "double") == 0)
				{
					text3 = "thinthin";
				}
			}
			html5Renderer.WriteStream(HTMLElements.m_dashStyle);
			html5Renderer.WriteStream(theString);
			if (text3 != null)
			{
				html5Renderer.WriteStream(HTMLElements.m_quote);
				html5Renderer.WriteStream(HTMLElements.m_slineStyle);
				html5Renderer.WriteStream(text3);
			}
			html5Renderer.WriteStream(HTMLElements.m_quote);
			html5Renderer.WriteStream(HTMLElements.m_closeTag);
			html5Renderer.WriteStreamCR(HTMLElements.m_closeVGroup);
		}
	}
}
