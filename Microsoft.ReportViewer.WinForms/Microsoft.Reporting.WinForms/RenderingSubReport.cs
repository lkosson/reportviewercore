using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class RenderingSubReport : RenderingItemContainer
	{
		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			RPLContainer rPLContainer = (RPLContainer)rplElement;
			if (rPLContainer.Children == null)
			{
				return;
			}
			if (rPLContainer.Children.Length == 1)
			{
				RenderingItem renderingItem = RenderingItem.CreateRenderingItem(context, rPLContainer.Children[0], bounds);
				if (renderingItem != null)
				{
					base.Children.Add(renderingItem);
				}
				rPLContainer.Children = null;
				return;
			}
			List<RPLItemMeasurement> list = new List<RPLItemMeasurement>(rPLContainer.Children.Length);
			for (int i = 0; i < rPLContainer.Children.Length; i++)
			{
				list.Add(rPLContainer.Children[i]);
			}
			rPLContainer.Children = null;
			RectangleF bounds2 = bounds;
			for (int j = 0; j < list.Count; j++)
			{
				RPLItemMeasurement rPLItemMeasurement = list[j];
				rPLItemMeasurement.Width = bounds.Width;
				bounds2.Height = rPLItemMeasurement.Height;
				RenderingItem renderingItem2 = RenderingItem.CreateRenderingItem(context, rPLItemMeasurement, bounds2);
				if (renderingItem2 != null)
				{
					base.Children.Add(renderingItem2);
				}
				bounds2.Y += rPLItemMeasurement.Height;
				list[j] = null;
			}
		}
	}
}
