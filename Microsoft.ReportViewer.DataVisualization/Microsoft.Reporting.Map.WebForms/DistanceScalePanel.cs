using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DistanceScalePanel : DockablePanel, IToolTipProvider
	{
		private enum MeasurementUnit
		{
			km,
			m,
			cm,
			mi,
			ft,
			@in
		}

		private const int ScaleOutlineWidth = 3;

		private const int PanelPadding = 3;

		private float KilometersToMiles = 0.621f;

		private int HorizLabelMargin = 3;

		private int VertLabelMargin;

		private Font font = new Font("Microsoft Sans Serif", 8.25f);

		private Color scaleBorderColor = Color.DarkGray;

		private Color scaleForeColor = Color.White;

		private Color labelColor = Color.Black;

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_Font")]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8.25pt")]
		public Font Font
		{
			get
			{
				return font;
			}
			set
			{
				font = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_ScaleBorderColor")]
		[DefaultValue(typeof(Color), "DarkGray")]
		public Color ScaleBorderColor
		{
			get
			{
				return scaleBorderColor;
			}
			set
			{
				scaleBorderColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_ScaleForeColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color ScaleForeColor
		{
			get
			{
				return scaleForeColor;
			}
			set
			{
				scaleForeColor = value;
				Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeDistanceScalePanel_LabelColor")]
		[DefaultValue(typeof(Color), "Black")]
		public Color LabelColor
		{
			get
			{
				return labelColor;
			}
			set
			{
				labelColor = value;
				Invalidate();
			}
		}

		public DistanceScalePanel()
			: this(null)
		{
		}

		internal DistanceScalePanel(CommonElements common)
			: base(common)
		{
			Name = "DistanceScalePanel";
			SizeUnit = CoordinateUnit.Pixel;
		}

		internal override void Render(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			try
			{
				MapCore mapCore = GetMapCore();
				if (mapCore == null)
				{
					return;
				}
				g.AntiAliasing = AntiAliasing.None;
				base.Render(g);
				float num = 4.5f;
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				absoluteRectangle.Inflate(0f - num, 0f - num);
				if (absoluteRectangle.Width < 3f || absoluteRectangle.Height < 3f)
				{
					return;
				}
				float num2 = 0f;
				float num3 = 0f;
				string arg = "0";
				string arg2 = "0";
				MeasurementUnit measurementUnit = MeasurementUnit.km;
				MeasurementUnit measurementUnit2 = MeasurementUnit.mi;
				float num4 = absoluteRectangle.Width - 6f;
				float kilometers = (float)mapCore.PixelsToKilometers(num4);
				float miles = kilometers * KilometersToMiles;
				measurementUnit = AdjustMetricUnit(ref kilometers);
				float num5 = FloorDistance(kilometers);
				float num6 = num5 / kilometers;
				measurementUnit2 = AdjustImperialUnit(ref miles);
				float num7 = FloorDistance(miles);
				float num8 = num7 / miles;
				if (num5 >= 1f && num7 >= 1f)
				{
					num2 = num4 * num6;
					num3 = num4 * num8;
					if (GetMapCore().MapControl.FormatNumberHandler != null)
					{
						arg = GetMapCore().MapControl.FormatNumberHandler(GetMapCore().MapControl, num5, "G");
						arg2 = GetMapCore().MapControl.FormatNumberHandler(GetMapCore().MapControl, num7, "G");
					}
					else
					{
						arg = num5.ToString(CultureInfo.CurrentCulture);
						arg2 = num7.ToString(CultureInfo.CurrentCulture);
					}
				}
				else
				{
					num2 = num4;
					num3 = num4;
				}
				using (GraphicsPath path = CreateScalePath(absoluteRectangle, (int)num2, (int)num3))
				{
					using (Brush brush = new SolidBrush(ScaleForeColor))
					{
						g.FillPath(brush, path);
					}
					using (Pen pen = new Pen(ScaleBorderColor, 1f))
					{
						pen.Alignment = PenAlignment.Center;
						pen.MiterLimit = 0f;
						g.DrawPath(pen, path);
					}
					StringFormat stringFormat = (StringFormat)StringFormat.GenericTypographic.Clone();
					stringFormat.FormatFlags = StringFormatFlags.NoWrap;
					using (Brush brush2 = new SolidBrush(LabelColor))
					{
						RectangleF textBounds = new RectangleF(absoluteRectangle.Left + 3f, absoluteRectangle.Top, num2, absoluteRectangle.Height / 2f - 3f);
						string text = string.Format(CultureInfo.CurrentCulture, "{0} {1}", arg, measurementUnit.ToString(CultureInfo.CurrentCulture));
						SizeF textClipSize = g.MeasureString(text, Font, textBounds.Size, stringFormat);
						RectangleF layoutRectangle = CreateTextClip(textBounds, textClipSize);
						g.DrawString(text, Font, brush2, layoutRectangle, stringFormat);
						RectangleF textBounds2 = new RectangleF(absoluteRectangle.Left + 3f, absoluteRectangle.Top + absoluteRectangle.Height / 2f + 3f, num3, absoluteRectangle.Height / 2f - 3f);
						string text2 = string.Format(CultureInfo.CurrentCulture, "{0} {1}", arg2, measurementUnit2.ToString(CultureInfo.CurrentCulture));
						SizeF textClipSize2 = g.MeasureString(text2, Font, textBounds2.Size, stringFormat);
						RectangleF layoutRectangle2 = CreateTextClip(textBounds2, textClipSize2);
						g.DrawString(text2, Font, brush2, layoutRectangle2, stringFormat);
					}
				}
			}
			finally
			{
				g.AntiAliasing = antiAliasing;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 130f, 55f);
			case "SizeUnit":
				return CoordinateUnit.Pixel;
			case "Dock":
				return PanelDockStyle.Bottom;
			case "DockAlignment":
				return DockAlignment.Far;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		private int FloorDistance(double distance)
		{
			int num = (int)Math.Log10(distance);
			double num2 = Math.Pow(10.0, num) / 2.0;
			return (int)(Math.Floor(distance / num2) * num2);
		}

		private MeasurementUnit AdjustMetricUnit(ref float kilometers)
		{
			MeasurementUnit result = MeasurementUnit.km;
			if ((double)kilometers < 0.001)
			{
				kilometers *= 100000f;
				result = MeasurementUnit.cm;
			}
			else if (kilometers < 1f)
			{
				kilometers *= 1000f;
				result = MeasurementUnit.m;
			}
			return result;
		}

		private MeasurementUnit AdjustImperialUnit(ref float miles)
		{
			MeasurementUnit result = MeasurementUnit.mi;
			if (miles < 0.000189393933f)
			{
				miles *= 63360f;
				result = MeasurementUnit.@in;
			}
			else if (miles < 1f)
			{
				miles *= 5280f;
				result = MeasurementUnit.ft;
			}
			return result;
		}

		private GraphicsPath CreateScalePath(RectangleF drawingBounds, int metricScaleWidth, int imperialScaleWidth)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			Rectangle rectangle = Rectangle.Truncate(drawingBounds);
			rectangle.Inflate(0, -(rectangle.Height - 9) / 2);
			bool flag = metricScaleWidth > imperialScaleWidth;
			int num = rectangle.Y + rectangle.Height / 2;
			int num2 = (rectangle.Height % 2 == 0) ? 1 : 0;
			int num3 = num - 1;
			int num4 = num + 1 + 1;
			Point point = new Point(rectangle.X + 3 + metricScaleWidth, num3);
			Point point2 = new Point(rectangle.X + 6 + metricScaleWidth, flag ? num4 : num3);
			Point point3 = new Point(rectangle.X + 3 + imperialScaleWidth, num4);
			Point point4 = new Point(rectangle.X + 6 + imperialScaleWidth, flag ? num4 : num3);
			graphicsPath.AddPolygon(new Point[14]
			{
				new Point(rectangle.Left, rectangle.Top + num2),
				new Point(rectangle.Left + 3, rectangle.Top + num2),
				new Point(rectangle.Left + 3, num3),
				point,
				new Point(point.X, rectangle.Top + num2),
				new Point(point2.X, rectangle.Top + num2),
				point2,
				point4,
				new Point(point4.X, rectangle.Bottom),
				new Point(point3.X, rectangle.Bottom),
				point3,
				new Point(rectangle.Left + 3, num4),
				new Point(rectangle.Left + 3, rectangle.Bottom),
				new Point(rectangle.Left, rectangle.Bottom)
			});
			return graphicsPath;
		}

		private RectangleF CreateTextClip(RectangleF textBounds, SizeF textClipSize)
		{
			textBounds.Inflate(-HorizLabelMargin, -VertLabelMargin);
			textClipSize.Width = Math.Max(1f, Math.Min(textClipSize.Width, textBounds.Width));
			textClipSize.Height = Math.Max(1f, Math.Min(textClipSize.Height, textBounds.Height));
			return new RectangleF(new PointF(textBounds.X, textBounds.Y + (textBounds.Height - textClipSize.Height) / 2f), textClipSize);
		}
	}
}
