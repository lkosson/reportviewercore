using Microsoft.Reporting.Chart.WebForms.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributePolylineAnnotation_PolylineAnnotation")]
	internal class PolylineAnnotation : Annotation
	{
		internal GraphicsPath path = new GraphicsPath();

		internal bool pathChanged;

		private AnnotationPathPointCollection pathPoints = new AnnotationPathPointCollection();

		internal bool isPolygon;

		internal bool freeDrawPlacement;

		private LineAnchorCap startCap;

		private LineAnchorCap endCap;

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
		[SRDescription("DescriptionAttributeTextAnnotation_LineWidth")]
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
		[SRDescription("DescriptionAttributeTextStyle5")]
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

		[SRCategory("CategoryAttributeMisc")]
		[Bindable(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeAnnotationType4")]
		public override string AnnotationType => "Polyline";

		[SRCategory("CategoryAttributeAppearance")]
		[DefaultValue(SelectionPointsStyle.Rectangle)]
		[ParenthesizePropertyName(true)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[SRDescription("DescriptionAttributeSelectionPointsStyle3")]
		internal override SelectionPointsStyle SelectionPointsStyle => SelectionPointsStyle.Rectangle;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeFreeDrawPlacement")]
		public virtual bool FreeDrawPlacement
		{
			get
			{
				return freeDrawPlacement;
			}
			set
			{
				freeDrawPlacement = value;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[DefaultValue(null)]
		[SRDescription("DescriptionAttributePath")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public virtual GraphicsPath Path
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
				pathChanged = true;
			}
		}

		[SRCategory("CategoryAttributePosition")]
		[SRDescription("DescriptionAttributePathPoints")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AnnotationPathPointCollection PathPoints
		{
			get
			{
				if (pathChanged || path.PointCount != pathPoints.Count)
				{
					pathPoints.annotation = null;
					pathPoints.Clear();
					PointF[] array = path.PathPoints;
					byte[] pathTypes = path.PathTypes;
					for (int i = 0; i < array.Length; i++)
					{
						pathPoints.Add(new AnnotationPathPoint(array[i].X, array[i].Y, pathTypes[i]));
					}
					pathPoints.annotation = this;
				}
				return pathPoints;
			}
		}

		public PolylineAnnotation()
		{
			pathPoints.annotation = this;
		}

		internal override void Paint(Chart chart, ChartGraphics graphics)
		{
			Chart = chart;
			if (path.PointCount == 0)
			{
				return;
			}
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			PointF pointF = new PointF(location.X + size.Width, location.Y + size.Height);
			RectangleF rectangleF = new RectangleF(location, new SizeF(pointF.X - location.X, pointF.Y - location.Y));
			RectangleF rectangleF2 = new RectangleF(rectangleF.Location, rectangleF.Size);
			if (rectangleF2.Width < 0f)
			{
				rectangleF2.X = rectangleF2.Right;
				rectangleF2.Width = 0f - rectangleF2.Width;
			}
			if (rectangleF2.Height < 0f)
			{
				rectangleF2.Y = rectangleF2.Bottom;
				rectangleF2.Height = 0f - rectangleF2.Height;
			}
			if (float.IsNaN(rectangleF2.X) || float.IsNaN(rectangleF2.Y) || float.IsNaN(rectangleF2.Right) || float.IsNaN(rectangleF2.Bottom))
			{
				return;
			}
			RectangleF absoluteRectangle = graphics.GetAbsoluteRectangle(rectangleF2);
			float num = absoluteRectangle.Width / 100f;
			float num2 = absoluteRectangle.Height / 100f;
			PointF[] array = path.PathPoints;
			byte[] pathTypes = path.PathTypes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].X = absoluteRectangle.X + array[i].X * num;
				array[i].Y = absoluteRectangle.Y + array[i].Y * num2;
			}
			GraphicsPath graphicsPath = new GraphicsPath(array, pathTypes);
			bool flag = false;
			LineCap lineCap = LineCap.Flat;
			LineCap lineCap2 = LineCap.Flat;
			if (!isPolygon && (startCap != 0 || endCap != 0))
			{
				flag = true;
				lineCap = graphics.pen.StartCap;
				lineCap2 = graphics.pen.EndCap;
				if (startCap == LineAnchorCap.Arrow)
				{
					if (LineWidth < 4)
					{
						int num3 = 3 - LineWidth;
						graphics.pen.StartCap = LineCap.Custom;
						graphics.pen.CustomStartCap = new AdjustableArrowCap(LineWidth + num3, LineWidth + num3, isFilled: true);
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
						int num4 = 3 - LineWidth;
						graphics.pen.EndCap = LineCap.Custom;
						graphics.pen.CustomEndCap = new AdjustableArrowCap(LineWidth + num4, LineWidth + num4, isFilled: true);
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
				if (isPolygon)
				{
					graphicsPath.CloseAllFigures();
					graphics.DrawPathAbs(graphicsPath, BackColor, BackHatchStyle, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, BackGradientType, BackGradientEndColor, LineColor, LineWidth, LineStyle, PenAlignment.Center, ShadowOffset, ShadowColor);
				}
				else
				{
					graphics.DrawPathAbs(graphicsPath, Color.Transparent, ChartHatchStyle.None, string.Empty, ChartImageWrapMode.Scaled, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, LineColor, LineWidth, LineStyle, PenAlignment.Center, ShadowOffset, ShadowColor);
				}
			}
			if (Chart.chartPicture.common.ProcessModeRegions)
			{
				GraphicsPath graphicsPath2 = null;
				if (isPolygon)
				{
					graphicsPath2 = graphicsPath;
				}
				else
				{
					graphicsPath2 = new GraphicsPath();
					graphicsPath2.AddPath(graphicsPath, connect: false);
					using (Pen pen = (Pen)graphics.pen.Clone())
					{
						pen.DashStyle = DashStyle.Solid;
						pen.Width += 2f;
						ChartGraphics.Widen(graphicsPath2, pen);
					}
				}
				Chart.chartPicture.common.HotRegionsList.AddHotRegion(graphics, graphicsPath2, relativePath: false, ReplaceKeywords(ToolTip), ReplaceKeywords(Href), ReplaceKeywords(MapAreaAttributes), this, ChartElementType.Annotation);
			}
			if (flag)
			{
				graphics.pen.StartCap = lineCap;
				graphics.pen.EndCap = lineCap2;
			}
			PaintSelectionHandles(graphics, rectangleF2, graphicsPath);
		}

		internal override void AdjustLocationSize(SizeF movingDistance, ResizingMode resizeMode, bool pixelCoord, bool userInput)
		{
			if (resizeMode != ResizingMode.MovingPathPoints)
			{
				base.AdjustLocationSize(movingDistance, resizeMode, pixelCoord, userInput);
				return;
			}
			PointF location = PointF.Empty;
			PointF anchorLocation = PointF.Empty;
			SizeF size = SizeF.Empty;
			GetRelativePosition(out location, out size, out anchorLocation);
			if (userInput)
			{
				_ = startMovePathRel;
			}
			if (pixelCoord)
			{
				movingDistance = GetGraphics().GetRelativeSize(movingDistance);
			}
			movingDistance.Width /= startMovePositionRel.Width / 100f;
			movingDistance.Height /= startMovePositionRel.Height / 100f;
			if (path.PointCount <= 0)
			{
				return;
			}
			GraphicsPath obj = userInput ? startMovePathRel : path;
			PointF[] array = obj.PathPoints;
			byte[] pathTypes = obj.PathTypes;
			_ = RectangleF.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				if (currentPathPointIndex == i || currentPathPointIndex < 0 || currentPathPointIndex >= array.Length)
				{
					array[i].X -= movingDistance.Width;
					array[i].Y -= movingDistance.Height;
				}
			}
			if (userInput && AllowResizing)
			{
				path.Dispose();
				path = new GraphicsPath(array, pathTypes);
				RectangleF bounds = path.GetBounds();
				bounds.X *= startMovePositionRel.Width / 100f;
				bounds.Y *= startMovePositionRel.Height / 100f;
				bounds.X += startMovePositionRel.X;
				bounds.Y += startMovePositionRel.Y;
				bounds.Width *= startMovePositionRel.Width / 100f;
				bounds.Height *= startMovePositionRel.Height / 100f;
				SetPositionRelative(bounds, anchorLocation);
				for (int j = 0; j < array.Length; j++)
				{
					array[j].X = startMovePositionRel.X + array[j].X * (startMovePositionRel.Width / 100f);
					array[j].Y = startMovePositionRel.Y + array[j].Y * (startMovePositionRel.Height / 100f);
					array[j].X = (array[j].X - bounds.X) / (bounds.Width / 100f);
					array[j].Y = (array[j].Y - bounds.Y) / (bounds.Height / 100f);
				}
			}
			positionChanged = true;
			path.Dispose();
			path = new GraphicsPath(array, pathTypes);
			pathChanged = true;
			Invalidate();
		}
	}
}
