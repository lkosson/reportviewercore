using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.IO;
using System.Text;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class RectangleRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		private const string GrowRectanglesSuffix = "_gr";

		public RectangleRenderer(HTML5Renderer renderer)
		{
			html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = reportItem.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RenderRectangle((RPLContainer)reportItem, elementProps, definition, measurement, ref borderContext, renderId, styleContext, treatAsTopLevel);
		}

		protected void RenderRectangle(RPLContainer rectangle, RPLElementProps props, RPLElementPropsDef def, RPLItemMeasurement measurement, ref int borderContext, bool renderId, StyleContext styleContext, bool treatAsTopLevel)
		{
			RPLItemMeasurement[] children = rectangle.Children;
			RPLRectanglePropsDef rPLRectanglePropsDef = def as RPLRectanglePropsDef;
			if (rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null)
			{
				html5Renderer.m_linkToChildStack.Push(rPLRectanglePropsDef.LinkToChildId);
			}
			bool expandItem = html5Renderer.m_expandItem;
			bool flag = renderId;
			string text = props.UniqueName;
			bool flag2 = children == null || children.Length == 0;
			if (flag2 && styleContext.InTablix)
			{
				return;
			}
			bool flag3 = html5Renderer.m_deviceInfo.OutlookCompat || !html5Renderer.m_browserIE || (flag2 && html5Renderer.m_usePercentWidth);
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					html5Renderer.WriteStream(HTMLElements.m_openTable);
					html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
				}
				else
				{
					html5Renderer.WriteStream(HTMLElements.m_openDiv);
					if (html5Renderer.m_deviceInfo.IsBrowserIE && html5Renderer.m_deviceInfo.AllowScript)
					{
						if (!html5Renderer.m_needsGrowRectangleScript)
						{
							CreateGrowRectIdsStream();
						}
						flag = true;
						if (!renderId)
						{
							text = props.UniqueName + "_gr";
						}
						html5Renderer.WriteIdToSecondaryStream(html5Renderer.m_growRectangleIdsStream, text);
					}
				}
				if (flag)
				{
					html5Renderer.RenderReportItemId(text);
				}
				if (html5Renderer.m_isBody)
				{
					html5Renderer.m_isBody = false;
					styleContext.RenderMeasurements = false;
					if (flag2)
					{
						html5Renderer.OpenStyle();
						if (html5Renderer.m_usePercentWidth)
						{
							html5Renderer.RenderMeasurementHeight(measurement.Height);
							html5Renderer.WriteStream(HTMLElements.m_styleWidth);
							html5Renderer.WriteStream(HTMLElements.m_percent);
							html5Renderer.WriteStream(HTMLElements.m_semiColon);
						}
						else
						{
							RenderRectangleMeasurements(measurement, props.Style);
						}
					}
					else if (flag3 && html5Renderer.m_usePercentWidth)
					{
						html5Renderer.OpenStyle();
						html5Renderer.WriteStream(HTMLElements.m_styleWidth);
						html5Renderer.WriteStream(HTMLElements.m_percent);
						html5Renderer.WriteStream(HTMLElements.m_semiColon);
					}
					html5Renderer.m_usePercentWidth = false;
				}
				if (!styleContext.InTablix)
				{
					if (styleContext.RenderMeasurements)
					{
						html5Renderer.OpenStyle();
						RenderRectangleMeasurements(measurement, props.Style);
					}
					html5Renderer.RenderReportItemStyle(rectangle, props, def, measurement, styleContext, ref borderContext, def.ID);
				}
				html5Renderer.CloseStyle(renderQuote: true);
				html5Renderer.WriteToolTip(props);
				html5Renderer.WriteStreamCR(HTMLElements.m_closeBracket);
				if (flag3)
				{
					html5Renderer.WriteStream(HTMLElements.m_firstTD);
					html5Renderer.OpenStyle();
					if (flag2)
					{
						html5Renderer.RenderMeasurementStyle(measurement.Height, measurement.Width);
						html5Renderer.WriteStream(HTMLElements.m_fontSize);
						html5Renderer.WriteStream("1pt");
					}
					else
					{
						html5Renderer.WriteStream(HTMLElements.m_verticalAlign);
						html5Renderer.WriteStream(HTMLElements.m_topValue);
					}
					html5Renderer.CloseStyle(renderQuote: true);
					html5Renderer.WriteStream(HTMLElements.m_closeBracket);
				}
			}
			if (flag2)
			{
				html5Renderer.WriteStream(HTMLElements.m_nbsp);
			}
			else
			{
				bool inTablix = styleContext.InTablix;
				styleContext.InTablix = false;
				flag2 = html5Renderer.GenerateHTMLTable(children, measurement.Top, measurement.Left, measurement.Width, measurement.Height, borderContext, expandItem, SharedListLayoutState.None, null, props.Style, treatAsTopLevel);
				if (inTablix)
				{
					styleContext.InTablix = true;
				}
			}
			if (!styleContext.InTablix || renderId)
			{
				if (flag3)
				{
					html5Renderer.WriteStream(HTMLElements.m_lastTD);
					html5Renderer.WriteStream(HTMLElements.m_closeTable);
				}
				else
				{
					html5Renderer.WriteStreamCR(HTMLElements.m_closeDiv);
				}
			}
			if (html5Renderer.m_linkToChildStack.Count > 0 && rPLRectanglePropsDef != null && rPLRectanglePropsDef.LinkToChildId != null && rPLRectanglePropsDef.LinkToChildId.Equals(html5Renderer.m_linkToChildStack.Peek()))
			{
				html5Renderer.m_linkToChildStack.Pop();
			}
		}

		private void CreateGrowRectIdsStream()
		{
			string streamName = HTML5Renderer.GetStreamName(html5Renderer.m_rplReport.ReportName, html5Renderer.m_pageNum, "_gr");
			Stream stream = html5Renderer.CreateStream(streamName, "txt", Encoding.UTF8, "text/plain", willSeek: true, StreamOper.CreateOnly);
			html5Renderer.m_growRectangleIdsStream = new BufferedStream(stream);
			html5Renderer.m_needsGrowRectangleScript = true;
		}

		private void RenderRectangleMeasurements(RPLItemMeasurement measurement, IRPLStyle style)
		{
			float width = measurement.Width;
			float height = measurement.Height;
			html5Renderer.RenderMeasurementWidth(width, renderMinWidth: true);
			if (html5Renderer.m_deviceInfo.IsBrowserIE && html5Renderer.m_deviceInfo.BrowserMode == BrowserMode.Standards && !html5Renderer.m_deviceInfo.IsBrowserIE6)
			{
				html5Renderer.RenderMeasurementMinHeight(height);
			}
			else
			{
				html5Renderer.RenderMeasurementHeight(height);
			}
		}
	}
}
