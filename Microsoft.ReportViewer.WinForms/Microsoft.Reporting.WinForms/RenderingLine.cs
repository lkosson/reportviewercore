using Microsoft.ReportingServices.Rendering.ImageRenderer;
using Microsoft.ReportingServices.Rendering.RPLProcessing;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.WinForms
{
	internal sealed class RenderingLine : RenderingItem
	{
		internal Color Color;

		internal float Width;

		internal DashStyle Style;

		internal override void ProcessRenderingElementContent(RPLElement rplElement, GdiContext context, RectangleF bounds)
		{
			Color = GdiContext.GetStylePropertyValueColor(InstanceProperties, 0);
			Width = GdiContext.GetStylePropertyValueSizeMM(InstanceProperties, 10);
			switch ((RPLFormat.BorderStyles)SharedRenderer.GetStylePropertyValueObject(InstanceProperties, 5))
			{
			case RPLFormat.BorderStyles.Dotted:
				Style = DashStyle.Dot;
				break;
			case RPLFormat.BorderStyles.Dashed:
				Style = DashStyle.Dash;
				break;
			default:
				Style = DashStyle.Solid;
				break;
			}
		}

		internal override void DrawContent(GdiContext context)
		{
			Pen pen = new Pen(Color, Width);
			pen.DashStyle = Style;
			if (!((RPLLinePropsDef)DefinitionProperties).Slant)
			{
				context.Graphics.DrawLine(pen, base.Position.Left, base.Position.Top, base.Position.Right, base.Position.Bottom);
			}
			else
			{
				context.Graphics.DrawLine(pen, base.Position.Left, base.Position.Bottom, base.Position.Right, base.Position.Top);
			}
		}
	}
}
