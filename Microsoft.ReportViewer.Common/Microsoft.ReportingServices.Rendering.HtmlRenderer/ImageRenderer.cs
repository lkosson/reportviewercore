using Microsoft.ReportingServices.HtmlRendering;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Security.AntiXss;

namespace Microsoft.ReportingServices.Rendering.HtmlRenderer
{
	internal class ImageRenderer : IReportItemRenderer
	{
		private HTML5Renderer html5Renderer;

		public ImageRenderer(HTML5Renderer renderer)
		{
			html5Renderer = renderer;
		}

		public void RenderReportItem(RPLElement reportItem, RPLItemMeasurement measurement, StyleContext styleContext, ref int borderContext, bool renderId, bool treatAsTopLevel)
		{
			RPLImageProps rPLImageProps = (RPLImageProps)reportItem.ElementProps;
			RPLImagePropsDef rPLImagePropsDef = (RPLImagePropsDef)rPLImageProps.Definition;
			RPLFormat.Sizings sizing = rPLImagePropsDef.Sizing;
			RPLImageData image = rPLImageProps.Image;
			float innerContainerHeightSubtractBorders = html5Renderer.GetInnerContainerHeightSubtractBorders(measurement, rPLImageProps.Style);
			float innerContainerWidthSubtractBorders = html5Renderer.GetInnerContainerWidthSubtractBorders(measurement, rPLImageProps.Style);
			string text = html5Renderer.GetImageUrl(useSessionId: true, image);
			string ariaLabel = null;
			string role = null;
			string tooltip = html5Renderer.GetTooltip(rPLImageProps);
			if (treatAsTopLevel)
			{
				ariaLabel = (string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleImageLabel : RenderResWrapper.AccessibleImageNavigationGroupLabel(tooltip));
				role = HTMLElements.m_navigationRole;
			}
			string input = string.IsNullOrEmpty(tooltip) ? RenderRes.AccessibleImageLabel : tooltip;
			input = AntiXssEncoder.XmlAttributeEncode(input);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (html5Renderer.m_elementExtender.ShouldApplyToElement(treatAsTopLevel))
			{
				dictionary.Add(HTMLElements.m_reportItemDataName, html5Renderer.m_elementExtender.ApplyToElement());
				dictionary.Add(HTMLElements.m_reportItemCustomAttrStr, html5Renderer.GetReportItemPath(rPLImagePropsDef.Name));
			}
			bool flag = rPLImageProps.ActionImageMapAreas != null && rPLImageProps.ActionImageMapAreas.Length != 0;
			if (flag)
			{
				string s = HTMLElements.m_hashTag + html5Renderer.m_deviceInfo.HtmlPrefixId + HTMLElements.m_mapPrefixString + rPLImageProps.UniqueName;
				dictionary.Add(HTMLElements.m_useMapName, HttpUtility.HtmlAttributeEncode(s));
			}
			if (html5Renderer.HasAction(rPLImageProps.ActionInfo))
			{
				RenderElementHyperlink(rPLImageProps.Style, rPLImageProps.ActionInfo.Actions[0]);
			}
			if (!styleContext.InTablix)
			{
				if (sizing == RPLFormat.Sizings.AutoSize)
				{
					styleContext.RenderMeasurements = false;
				}
				html5Renderer.WriteStream(HTMLElements.m_openDiv);
				html5Renderer.RenderReportItemStyle(reportItem, rPLImageProps, rPLImagePropsDef, measurement, styleContext, ref borderContext, rPLImagePropsDef.ID);
				html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = "data:image/gif;base64," + Convert.ToBase64String(HTMLRendererResources.GetBytes("Blank.gif"));
			}
			HtmlElement htmlElement;
			switch (sizing)
			{
			case RPLFormat.Sizings.FitProportional:
				htmlElement = new FitProportionalImageElement(text, innerContainerWidthSubtractBorders, innerContainerHeightSubtractBorders, role, input, ariaLabel, dictionary);
				break;
			case RPLFormat.Sizings.Fit:
				htmlElement = new FitImageElement(text, innerContainerWidthSubtractBorders, innerContainerHeightSubtractBorders, role, input, ariaLabel, dictionary);
				break;
			case RPLFormat.Sizings.Clip:
				htmlElement = new ClipImageElement(text, role, input, ariaLabel, dictionary);
				break;
			default:
				htmlElement = new OriginalSizeImageElement(text, role, input, ariaLabel, dictionary);
				break;
			}
			htmlElement.Render(new Html5OutputStream(html5Renderer));
			if (!styleContext.InTablix)
			{
				html5Renderer.WriteStream(HTMLElements.m_closeDiv);
			}
			if (html5Renderer.HasAction(rPLImageProps.ActionInfo))
			{
				html5Renderer.WriteStream(HTMLElements.m_closeA);
			}
			if (flag)
			{
				html5Renderer.RenderImageMapAreas(rPLImageProps.ActionImageMapAreas, measurement.Width, measurement.Height, rPLImageProps.UniqueName, 0, 0);
			}
		}

		private bool RenderHyperlink(RPLAction action, RPLFormat.TextDecorations textDec, string color)
		{
			html5Renderer.WriteStream(HTMLElements.m_openA);
			html5Renderer.RenderTabIndex();
			html5Renderer.RenderActionHref(action, textDec, color);
			html5Renderer.WriteStream(HTMLElements.m_closeBracket);
			return true;
		}

		private bool RenderElementHyperlink(IRPLStyle style, RPLAction action)
		{
			object obj = style[24];
			obj = ((obj != null) ? obj : ((object)RPLFormat.TextDecorations.None));
			string color = (string)style[27];
			return RenderHyperlink(action, (RPLFormat.TextDecorations)obj, color);
		}
	}
}
