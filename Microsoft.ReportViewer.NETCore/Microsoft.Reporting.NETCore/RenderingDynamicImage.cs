using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;
using System.IO;

namespace Microsoft.Reporting.NETCore
{
	internal sealed class RenderingDynamicImage : RenderingImage
	{
		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			ImagePositionPX = new RectangleF(0f, 0f, context.GdiWriter.ConvertToPixels(base.Position.Width), context.GdiWriter.ConvertToPixels(base.Position.Height));
			RPLDynamicImageProps rPLDynamicImageProps = (RPLDynamicImageProps)InstanceProperties;
			Stream stream = rPLDynamicImageProps.DynamicImageContent;
			if (stream != null)
			{
				stream.Position = 0L;
			}
			else if (rPLDynamicImageProps.DynamicImageContentOffset >= 0)
			{
				byte[] image = context.RplReport.GetImage(rPLDynamicImageProps.DynamicImageContentOffset);
				if (image == null)
				{
					return;
				}
				stream = new MemoryStream(image);
			}
			if (stream != null)
			{
				Image = Image.FromStream(stream);
				ImagePosition = new RectangleF(base.Position.Location, base.Position.Size);
				GdiContext.CalculateUsableReportItemRectangle(InstanceProperties, ref ImagePosition);
				if (!(ImagePosition.Width <= 0f) && !(ImagePosition.Height <= 0f))
				{
					ProcessImageMap(context);
				}
			}
		}
	}
}
