using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class HatchStyleEditor : UITypeEditor
	{
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			if (!(e.Value is MapHatchStyle))
			{
				return;
			}
			Color color = Color.Black;
			Color color2 = Color.White;
			if (e.Context != null && e.Context.PropertyDescriptor != null && e.Context.Instance != null)
			{
				if (e.Context.PropertyDescriptor.Name == "FillHatchStyle")
				{
					PropertyInfo property = e.Context.Instance.GetType().GetProperty("FillColor");
					if (property != null)
					{
						color = (Color)property.GetValue(e.Context.Instance, null);
					}
					property = e.Context.Instance.GetType().GetProperty("FillSecondaryColor");
					if (property != null)
					{
						color2 = (Color)property.GetValue(e.Context.Instance, null);
					}
				}
				else if (e.Context.PropertyDescriptor.Name == "BackHatchStyle")
				{
					PropertyInfo property2 = e.Context.Instance.GetType().GetProperty("BackColor");
					if (property2 != null)
					{
						color = (Color)property2.GetValue(e.Context.Instance, null);
					}
					property2 = e.Context.Instance.GetType().GetProperty("BackSecondaryColor");
					if (property2 != null)
					{
						color2 = (Color)property2.GetValue(e.Context.Instance, null);
					}
				}
				else if (e.Context.PropertyDescriptor.Name == "FrameHatchStyle")
				{
					PropertyInfo property3 = e.Context.Instance.GetType().GetProperty("FrameColor");
					if (property3 != null)
					{
						color = (Color)property3.GetValue(e.Context.Instance, null);
					}
					property3 = e.Context.Instance.GetType().GetProperty("FrameSecondaryColor");
					if (property3 != null)
					{
						color2 = (Color)property3.GetValue(e.Context.Instance, null);
					}
				}
				else if (e.Context.PropertyDescriptor.Name == "NeedleCapFillHatchStyle")
				{
					PropertyInfo property4 = e.Context.Instance.GetType().GetProperty("NeedleCapFillColor");
					if (property4 != null)
					{
						color = (Color)property4.GetValue(e.Context.Instance, null);
					}
					property4 = e.Context.Instance.GetType().GetProperty("NeedleCapFillSecondaryColor");
					if (property4 != null)
					{
						color2 = (Color)property4.GetValue(e.Context.Instance, null);
					}
				}
				else if (e.Context.PropertyDescriptor.Name == "ThermometerBackHatchStyle")
				{
					PropertyInfo property5 = e.Context.Instance.GetType().GetProperty("ThermometerBackColor");
					if (property5 != null)
					{
						color = (Color)property5.GetValue(e.Context.Instance, null);
					}
					property5 = e.Context.Instance.GetType().GetProperty("ThermometerBackSecondaryColor");
					if (property5 != null)
					{
						color2 = (Color)property5.GetValue(e.Context.Instance, null);
					}
				}
			}
			if (color == Color.Empty)
			{
				color = Color.Black;
			}
			if (color2 == Color.Empty)
			{
				color2 = Color.White;
			}
			if (color == color2)
			{
				color2 = Color.FromArgb(color.B, color.R, color.G);
			}
			if ((MapHatchStyle)e.Value != 0)
			{
				Brush hatchBrush = MapGraphics.GetHatchBrush((MapHatchStyle)e.Value, color, color2);
				e.Graphics.FillRectangle(hatchBrush, e.Bounds);
				hatchBrush.Dispose();
			}
		}
	}
}
