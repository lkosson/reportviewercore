using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLineAnnotation_LineAnnotation")]
	internal class LineAnnotation : Annotation
	{
		private bool drawInfinitive;

		private LineAnchorCap startCap;

		private LineAnchorCap endCap;

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeDrawInfinitive")]
		public virtual bool DrawInfinitive
		{
			get
			{
				return drawInfinitive;
			}
			set
			{
				drawInfinitive = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LineAnchorCap.None)]
		[SRDescription("DescriptionAttributeStartCap3")]
		public virtual LineAnchorCap StartCap
		{
			get
			{
				return startCap;
			}
			set
			{
				startCap = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(LineAnchorCap.None)]
		[SRDescription("DescriptionAttributeStartCap3")]
		public virtual LineAnchorCap EndCap
		{
			get
			{
				return endCap;
			}
			set
			{
				endCap = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override ContentAlignment Alignment
		{
			get
			{
				return base.Alignment;
			}
			set
			{
				base.Alignment = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeTextColor3")]
		public override Color TextColor
		{
			get
			{
				return base.TextColor;
			}
			set
			{
				base.TextColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Font), "Microsoft Sans Serif, 8pt")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Font TextFont
		{
			get
			{
				return base.TextFont;
			}
			set
			{
				base.TextFont = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(TextStyle), "Default")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override TextStyle TextStyle
		{
			get
			{
				return base.TextStyle;
			}
			set
			{
				base.TextStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		[NotifyParentProperty(true)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(ChartHatchStyle.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override ChartHatchStyle BackHatchStyle
		{
			get
			{
				return base.BackHatchStyle;
			}
			set
			{
				base.BackHatchStyle = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(GradientType.None)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override GradientType BackGradientType
		{
			get
			{
				return base.BackGradientType;
			}
			set
			{
				base.BackGradientType = value;
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Browsable(false)]
		[DefaultValue(typeof(Color), "")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
		public override Color BackGradientEndColor
		{
			get
			{
				return base.BackGradientEndColor;
			}
			set
			{
				base.BackGradientEndColor = value;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeSizeAlwaysRelative3")]
		public override bool SizeAlwaysRelative
		{
			get
			{
				return base.SizeAlwaysRelative;
			}
			set
			{
				base.SizeAlwaysRelative = value;
			}
		}

		[SRCategory("CategoryAttributeAnchor")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(typeof(ContentAlignment), "TopLeft")]
		[SRDescription("DescriptionAttributeAnchorAlignment4")]
		public override ContentAlignment AnchorAlignment
		{
			get
			{
				return base.AnchorAlignment;
			}
			set
			{
				base.AnchorAlignment = value;
			}
		}

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Line";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.TwoPoints;

		public LineAnnotation()
		{
			anchorAlignment = ContentAlignment.TopLeft;
		}

		internal virtual void AdjustLineCoordinates(ref PointF point1, ref PointF point2, ref RectangleF selectionRect)
		{
			if (DrawInfinitive)
			{
				if (Math.Round(point1.X, 3) == Math.Round(point2.X, 3))
				{
					point1.Y = ((point1.Y < point2.Y) ? 0f : 100f);
					point2.Y = ((point1.Y < point2.Y) ? 100f : 0f);
					return;
				}
				if (Math.Round(point1.Y, 3) == Math.Round(point2.Y, 3))
				{
					point1.X = ((point1.X < point2.X) ? 0f : 100f);
					point2.X = ((point1.X < point2.X) ? 100f : 0f);
					return;
				}
				PointF empty = PointF.Empty;
				empty.Y = 0f;
				empty.X = (0f - point1.Y) * (point2.X - point1.X) / (point2.Y - point1.Y) + point1.X;
				PointF empty2 = PointF.Empty;
				empty2.Y = 100f;
				empty2.X = (100f - point1.Y) * (point2.X - point1.X) / (point2.Y - point1.Y) + point1.X;
				point1 = ((point1.Y < point2.Y) ? empty : empty2);
				point2 = ((point1.Y < point2.Y) ? empty2 : empty);
			}
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF point = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF selectionRect = new RectangleF(location, new SizeF(point.X - location.X, point.Y - location.Y));
			AdjustLineCoordinates(ref location, ref point, ref selectionRect);
			if (float.IsNaN(location.X) || float.IsNaN(location.Y) || float.IsNaN(point.X) || float.IsNaN(point.Y))
			{
				return;
			}
			bool flag = false;
			LineCap lineCap = LineCap.Flat;
			LineCap lineCap2 = LineCap.Flat;
			if (startCap != 0 || endCap != 0)
			{
				flag = true;
				lineCap = graphics.pen.StartCap;
				lineCap2 = graphics.pen.EndCap;
				if (startCap == LineAnchorCap.Arrow)
				{
					if (LineWidth < 4)
					{
						int num = 3 - LineWidth;
						graphics.pen.StartCap = LineCap.Custom;
						graphics.pen.CustomStartCap = new AdjustableArrowCap(LineWidth + num, LineWidth + num, isFilled: true);
					}
					else
					{
						graphics.pen.StartCap = LineCap.ArrowAnchor;
					}
				}
				else if (startCap == LineAnchorCap.Diamond)
				{
					graphics.pen.StartCap = LineCap.DiamondAnchor;
				}
				else if (startCap == LineAnchorCap.Round)
				{
					graphics.pen.StartCap = LineCap.RoundAnchor;
				}
				else if (startCap == LineAnchorCap.Square)
				{
					graphics.pen.StartCap = LineCap.SquareAnchor;
				}
				if (endCap == LineAnchorCap.Arrow)
				{
					if (LineWidth < 4)
					{
						int num2 = 3 - LineWidth;
						graphics.pen.EndCap = LineCap.Custom;
						graphics.pen.CustomEndCap = new AdjustableArrowCap(LineWidth + num2, LineWidth + num2, isFilled: true);
					}
					else
					{
						graphics.pen.EndCap = LineCap.ArrowAnchor;
					}
				}
				else if (endCap == LineAnchorCap.Diamond)
				{
					graphics.pen.EndCap = LineCap.DiamondAnchor;
				}
				else if (endCap == LineAnchorCap.Round)
				{
					graphics.pen.EndCap = LineCap.RoundAnchor;
				}
				else if (endCap == LineAnchorCap.Square)
				{
					graphics.pen.EndCap = LineCap.SquareAnchor;
				}
			}
			if (Chart.chartPicture.common.ProcessModePaint)
			{
				graphics.DrawLineRel(LineColor, LineWidth, LineStyle, location, point, ShadowColor, ShadowOffset);
			}
			if (Chart.chartPicture.common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddLine(graphics.GetAbsolutePoint(location), graphics.GetAbsolutePoint(point));
				using (Pen pen = (Pen)graphics.pen.Clone())
				{
					pen.DashStyle = DashStyle.Solid;
					pen.Width += 2f;
					ChartGraphics.Widen(graphicsPath, pen);
				}
				Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath, relativePath: false, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation);
			}
			if (flag)
			{
				graphics.pen.StartCap = lineCap;
				graphics.pen.EndCap = lineCap2;
			}
			PaintSelectionHandles(graphics, selectionRect, null);
		}
	}
}
