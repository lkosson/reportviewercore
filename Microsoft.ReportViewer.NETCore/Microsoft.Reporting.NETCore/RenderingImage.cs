using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace Microsoft.Reporting.NETCore
{
	internal class RenderingImage : RenderingItem
	{
		internal RectangleF ImagePosition;

		internal RectangleF ImagePositionPX;

		internal Image Image;

		internal RPLFormat.Sizings Sizing;

		private bool m_processedImage;

		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			Image = RenderingItem.GetImage(context, ((RPLImageProps)InstanceProperties).Image);
			if (Image == null)
			{
				Image = GdiContext.ImageResources["InvalidImage"];
				Sizing = RPLFormat.Sizings.Clip;
			}
			else
			{
				Sizing = ((RPLImagePropsDef)DefinitionProperties).Sizing;
			}
			ImagePosition = new RectangleF(base.Position.Location, base.Position.Size);
			GdiContext.CalculateUsableReportItemRectangle(InstanceProperties, ref ImagePosition);
			if (!(ImagePosition.Width <= 0f))
			{
				_ = ImagePosition.Height;
				_ = 0f;
			}
		}

		internal override void DrawContent(GdiContext context)
		{
			if (Image == null)
			{
				return;
			}
			if (!m_processedImage)
			{
				if (Image is Bitmap)
				{
					((Bitmap)Image).SetResolution(96f, 96f);
				}
				SharedRenderer.CalculateImageRectangle(ImagePosition, Image.Width, Image.Height, Image.HorizontalResolution, Image.VerticalResolution, Sizing, out ImagePosition, out ImagePositionPX);
				if (InstanceProperties is RPLImageProps)
				{
					ProcessActions(context, InstanceProperties.UniqueName, ((RPLImageProps)InstanceProperties).ActionInfo, ImagePosition);
				}
				ProcessImageMap(context);
				m_processedImage = true;
			}
			if (!(ImagePosition.Width <= 0f) && !(ImagePosition.Height <= 0f))
			{
				SharedRenderer.DrawImage(context.Graphics, Image, ImagePosition, ImagePositionPX);
			}
		}

		internal void ProcessImageMap(GdiContext context)
		{
			bool flag = this is RenderingDynamicImage;
			RPLActionInfoWithImageMap[] array = (!flag) ? ((RPLImageProps)InstanceProperties).ActionImageMapAreas : ((RPLDynamicImageProps)InstanceProperties).ActionImageMapAreas;
			if (array == null)
			{
				return;
			}
			List<Action> list = new List<Action>();
			foreach (RPLActionInfoWithImageMap rPLActionInfoWithImageMap in array)
			{
				if (rPLActionInfoWithImageMap.ImageMaps == null)
				{
					continue;
				}
				for (int j = 0; j < rPLActionInfoWithImageMap.ImageMaps.Count; j++)
				{
					RPLImageMap rPLImageMap = rPLActionInfoWithImageMap.ImageMaps[j];
					if (rPLActionInfoWithImageMap.Actions != null && rPLActionInfoWithImageMap.Actions.Length != 0)
					{
						RPLAction rPLAction = rPLActionInfoWithImageMap.Actions[0];
						if (!string.IsNullOrEmpty(rPLAction.Hyperlink))
						{
							RegisterAction(context, new HyperLinkAction(InstanceProperties.UniqueName, rPLAction.Label, rPLImageMap.Shape, base.Position, rPLImageMap.Coordinates, rPLAction.Hyperlink));
						}
						else if (!string.IsNullOrEmpty(rPLAction.DrillthroughId))
						{
							RegisterAction(context, new DrillthroughAction(InstanceProperties.UniqueName, rPLAction.Label, rPLImageMap.Shape, base.Position, rPLImageMap.Coordinates, rPLAction.DrillthroughId));
						}
						else if (!string.IsNullOrEmpty(rPLAction.BookmarkLink))
						{
							RegisterAction(context, new BookmarkLinkAction(InstanceProperties.UniqueName, rPLAction.Label, rPLImageMap.Shape, base.Position, rPLImageMap.Coordinates, rPLAction.BookmarkLink));
						}
					}
					if (!string.IsNullOrEmpty(rPLImageMap.ToolTip))
					{
						list.Add(new ReportToolTip(InstanceProperties.UniqueName, rPLImageMap.Shape, base.Position, rPLImageMap.Coordinates, rPLImageMap.ToolTip));
					}
					if (!flag)
					{
						break;
					}
				}
			}
			if (context.FirstDraw && list.Count > 0)
			{
				context.RenderingReport.ToolTips.InsertRange(0, list);
			}
		}
	}
}
