using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Drawing;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ServerDynamicImageRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		public ServerDynamicImageRenderer(HTML5Renderer renderer)
		{
			html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement dynamicImage, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLElementProps elementProps = dynamicImage.ElementProps;
			RPLElementPropsDef definition = elementProps.Definition;
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)elementProps;
			string tooltip = html5Renderer.GetTooltip(rPLDynamicImageProps);
			if (dynamicImage == null)
			{
				return;
			}
			bool flag = rPLDynamicImageProps.ActionImageMapAreas != null && rPLDynamicImageProps.ActionImageMapAreas.Length != 0;
			Rectangle rectangle = RenderDynamicImage(measurement, rPLDynamicImageProps);
			int xOffset = 0;
			int yOffset = 0;
			bool flag2 = !rectangle.IsEmpty;
			bool flag3 = !html5Renderer.m_deviceInfo.IsBrowserSafari || html5Renderer.m_deviceInfo.AllowScript || !styleContext.InTablix;
			if (flag3)
			{
				html5Renderer.WriteStream(HTMLElements.m_openDiv);
				if (flag2 && html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
				{
					WriteReportItemDataName();
				}
				html5Renderer.WriteAccesibilityTags(RenderRes.AccessibleChartLabel, rPLDynamicImageProps, treatAsTopLevel);
				if (treatAsTopLevel)
				{
					string accessibleAriaName = string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleChartLabel : RenderResWrapper.AccessibleChartNavigationGroupLabel(tooltip);
					html5Renderer.WriteAriaAccessibleTags(accessibleAriaName);
				}
			}
			bool flag4 = html5Renderer.m_deviceInfo.DataVisualizationFitSizing == DataVisualizationFitSizing.Exact && styleContext.InTablix;
			if (flag2)
			{
				RPLFormat.Sizings sizing = flag4 ? RPLFormat.Sizings.Fit : RPLFormat.Sizings.AutoSize;
				html5Renderer.WriteOuterConsolidation(rectangle, sizing, rPLDynamicImageProps.UniqueName);
				html5Renderer.RenderReportItemStyle(dynamicImage, null, ref borderContext);
				xOffset = rectangle.Left;
				yOffset = rectangle.Top;
			}
			else if (flag4 && html5Renderer.m_deviceInfo.AllowScript)
			{
				if (html5Renderer.m_imgFitDivIdsStream == null)
				{
					html5Renderer.CreateImgFitDivImageIdsStream();
				}
				html5Renderer.WriteIdToSecondaryStream(html5Renderer.m_imgFitDivIdsStream, rPLDynamicImageProps.UniqueName + "_ifd");
				html5Renderer.RenderReportItemId(rPLDynamicImageProps.UniqueName + "_ifd");
			}
			if (flag3)
			{
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			html5Renderer.WriteStream(HTMLElements.m_img);
			if (html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
			{
				if (!flag2)
				{
					WriteReportItemDataName();
				}
				RPLItemPropsDef rPLItemPropsDef = (RPLItemPropsDef)definition;
				html5Renderer.WriteAttrEncoded(HTMLElements.m_reportItemCustomAttr, html5Renderer.GetReportItemPath(rPLItemPropsDef.Name));
			}
			if (html5Renderer.m_browserIE)
			{
				html5Renderer.WriteStream(HTMLElements.m_imgOnError);
			}
			if (renderId)
			{
				html5Renderer.RenderReportItemId(rPLDynamicImageProps.UniqueName);
			}
			html5Renderer.WriteStream(HTMLElements.m_zeroBorder);
			bool flag5 = dynamicImage is RPLChart;
			if (flag)
			{
				html5Renderer.WriteAttrEncoded(HTMLElements.m_useMap, "#" + html5Renderer.m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + rPLDynamicImageProps.UniqueName);
				if (flag4)
				{
					html5Renderer.OpenStyle();
					if (html5Renderer.m_useInlineStyle && !flag2)
					{
						html5Renderer.WriteStream(HTMLElements.m_styleHeight);
						html5Renderer.WriteStream(HTMLElements.m_percent);
						html5Renderer.WriteStream(HTMLElements.m_semiColon);
						html5Renderer.WriteStream(HTMLElements.m_styleWidth);
						html5Renderer.WriteStream(HTMLElements.m_percent);
						html5Renderer.WriteStream(HTMLElements.m_semiColon);
						flag5 = false;
					}
					html5Renderer.WriteStream("border-style:none;");
				}
			}
			else if (flag4 && html5Renderer.m_useInlineStyle && !flag2)
			{
				html5Renderer.PercentSizes();
				flag5 = false;
			}
			StyleContext styleContext2 = new StyleContext();
			if (!flag4 && (html5Renderer.m_deviceInfo.IsBrowserIE7 || html5Renderer.m_deviceInfo.IsBrowserIE6))
			{
				styleContext2.RenderMeasurements = false;
				styleContext2.RenderMinMeasurements = false;
			}
			if (!flag2)
			{
				if (flag4)
				{
					html5Renderer.RenderReportItemStyle(dynamicImage, null, ref borderContext, styleContext2);
				}
				else if (flag5)
				{
					RPLElementProps elementProps2 = dynamicImage.ElementProps;
					StyleContext styleContext3 = new StyleContext();
					styleContext3.RenderMeasurements = false;
					html5Renderer.OpenStyle();
					html5Renderer.RenderMeasurementStyle(measurement.Height, measurement.Width);
					html5Renderer.RenderReportItemStyle(dynamicImage, elementProps2, definition, measurement, styleContext3, ref borderContext, definition.ID);
				}
				else
				{
					html5Renderer.RenderReportItemStyle(dynamicImage, measurement, ref borderContext, styleContext2);
				}
			}
			else
			{
				html5Renderer.WriteClippedDiv(rectangle);
			}
			tooltip = (string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleChartLabel : tooltip);
			html5Renderer.WriteToolTipAttribute(tooltip);
			html5Renderer.WriteStream(HTMLElements.m_src);
			html5Renderer.RenderDynamicImageSrc(rPLDynamicImageProps);
			html5Renderer.WriteStreamCR(HTMLElements.m_closeTag);
			if (flag)
			{
				html5Renderer.RenderImageMapAreas(rPLDynamicImageProps.ActionImageMapAreas, measurement.Width, measurement.Height, rPLDynamicImageProps.UniqueName, xOffset, yOffset);
			}
			if (flag3)
			{
				html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
		}

		private void WriteReportItemDataName()
		{
			html5Renderer.WriteStream(HTMLElements.m_space);
			html5Renderer.WriteStream(HTMLElements.m_reportItemDataName);
			html5Renderer.WriteStream(HTMLElements.m_equal);
			html5Renderer.WriteStream(HTMLElements.m_quoteString);
			html5Renderer.WriteStream(html5Renderer.m_elementExtender.ApplyToElement());
			html5Renderer.WriteStream(HTMLElements.m_quoteString);
		}

		private Rectangle RenderDynamicImage(RPLItemMeasurement measurement, RPLDynamicImageProps dynamicImageProps)
		{
			if (html5Renderer.m_createSecondaryStreams != 0)
			{
				return dynamicImageProps.ImageConsolidationOffsets;
			}
			Stream stream = null;
			stream = html5Renderer.CreateStream(dynamicImageProps.StreamName, "png", null, "image/png", willSeek: false, StreamOper.CreateAndRegister);
			if (dynamicImageProps.DynamicImageContentOffset >= 0)
			{
				html5Renderer.m_rplReport.GetImage(dynamicImageProps.DynamicImageContentOffset, stream);
			}
			else if (dynamicImageProps.DynamicImageContent != null)
			{
				byte[] array = new byte[4096];
				dynamicImageProps.DynamicImageContent.Position = 0L;
				int num = (int)dynamicImageProps.DynamicImageContent.Length;
				while (num > 0)
				{
					int num2 = dynamicImageProps.DynamicImageContent.Read(array, 0, Math.Min(array.Length, num));
					stream.Write(array, 0, num2);
					num -= num2;
				}
			}
			return Rectangle.Empty;
		}
	}
}
