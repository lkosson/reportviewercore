using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.IO;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearScaleConverter))]
	internal sealed class LinearScale : ScaleBase, ISelectable
	{
		private float position = 50f;

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLabelStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearLabelStyle LabelStyle
		{
			get
			{
				return baseLabelStyle;
			}
			set
			{
				baseLabelStyle = value;
				baseLabelStyle.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_Position")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(50f)]
		[ValidateBound(5.0, 90.0)]
		public float Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_StartMargin")]
		[DefaultValue(8f)]
		[ValidateBound(0.0, 100.0)]
		public float StartMargin
		{
			get
			{
				return _startPosition;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				_startPosition = Math.Min(value, _endPosition);
				InvalidateSweepPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_EndMargin")]
		[DefaultValue(8f)]
		[ValidateBound(0.0, 100.0)]
		public float EndMargin
		{
			get
			{
				return 100f - _endPosition;
			}
			set
			{
				if (value > 100f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 100));
				}
				_endPosition = Math.Max(100f - value, _startPosition);
				InvalidateSweepPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeMajorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearMajorTickMark MajorTickMark
		{
			get
			{
				return (LinearMajorTickMark)base.MajorTickMarkInt;
			}
			set
			{
				base.MajorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeMinorTickMarkInt")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearMinorTickMark MinorTickMark
		{
			get
			{
				return (LinearMinorTickMark)base.MinorTickMarkInt;
			}
			set
			{
				base.MinorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearScale_Width")]
		[ValidateBound(0.0, 30.0)]
		[DefaultValue(5f)]
		public override float Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LinearSpecialPosition MinimumPin
		{
			get
			{
				return (LinearSpecialPosition)minimumPin;
			}
			set
			{
				minimumPin = value;
				minimumPin.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMaximumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LinearSpecialPosition MaximumPin
		{
			get
			{
				return (LinearSpecialPosition)maximumPin;
			}
			set
			{
				maximumPin = value;
				maximumPin.Parent = this;
				Invalidate();
			}
		}

		internal LinearGauge ParentGauge => GetGauge();

		public LinearScale()
		{
			_startPosition = 8f;
			_endPosition = 92f;
			coordSystemRatio = 1f;
			Width = 5f;
			InvalidateSweepPosition();
			base.MajorTickMarkInt = new LinearMajorTickMark(this);
			base.MinorTickMarkInt = new LinearMinorTickMark(this);
			baseLabelStyle = new LinearLabelStyle(this);
			maximumPin = new LinearSpecialPosition(this);
			minimumPin = new LinearSpecialPosition(this);
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside)
		{
			GaugeGraphics graph = Common.Graph;
			GraphicsPath graphicsPath = new GraphicsPath();
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = MajorTickMark.Width / 2f;
			}
			if (MinorTickMark.Visible)
			{
				num = Math.Max(num, MinorTickMark.Width / 2f);
			}
			RectangleF relative = new RectangleF(0f, 0f, 0f, 0f);
			if (ParentGauge.GetOrientation() == GaugeOrientation.Horizontal)
			{
				relative.X = base.StartPosition;
				relative.Width = base.EndPosition - base.StartPosition;
				relative.Y = Position - barOffsetInside;
				relative.Height = barOffsetInside + barOffsetOutside;
				relative = graph.GetAbsoluteRectangle(relative);
				relative.Inflate(graph.GetAbsoluteDimension(num), 0f);
			}
			else
			{
				relative.Y = base.StartPosition;
				relative.Height = base.EndPosition - base.StartPosition;
				relative.X = Position - barOffsetInside;
				relative.Width = barOffsetInside + barOffsetOutside;
				relative = graph.GetAbsoluteRectangle(relative);
				relative.Inflate(0f, graph.GetAbsoluteDimension(num));
			}
			if (relative.Width <= 0f)
			{
				relative.Width = 1E-06f;
			}
			if (relative.Height <= 0f)
			{
				relative.Height = 1E-06f;
			}
			graphicsPath.AddRectangle(relative);
			return graphicsPath;
		}

		private void SetScaleHitTestPath(GaugeGraphics g)
		{
			Gap gap = new Gap(Position);
			gap.SetOffset(Placement.Cross, Width);
			gap.SetBase();
			if (MajorTickMark.Visible)
			{
				gap.SetOffsetBase(MajorTickMark.Placement, MajorTickMark.Length);
				if (MajorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, MajorTickMark.DistanceFromScale + MajorTickMark.Length + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - MajorTickMark.DistanceFromScale);
				}
				else if (MajorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MajorTickMark.DistanceFromScale + MajorTickMark.Length / 2f);
					gap.Inside = Math.Max(gap.Inside, MajorTickMark.DistanceFromScale + MajorTickMark.Length / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MajorTickMark.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, MajorTickMark.DistanceFromScale + MajorTickMark.Length + Width / 2f);
				}
			}
			if (MinorTickMark.Visible)
			{
				gap.SetOffsetBase(MinorTickMark.Placement, MinorTickMark.Length);
				if (MinorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, MinorTickMark.DistanceFromScale + MinorTickMark.Length + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - MinorTickMark.DistanceFromScale);
				}
				else if (MinorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MinorTickMark.DistanceFromScale + MinorTickMark.Length / 2f);
					gap.Inside = Math.Max(gap.Inside, MinorTickMark.DistanceFromScale + MinorTickMark.Length / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MinorTickMark.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, MinorTickMark.DistanceFromScale + MinorTickMark.Length + Width / 2f);
				}
			}
			if (LabelStyle.Visible)
			{
				if (LabelStyle.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - LabelStyle.DistanceFromScale);
				}
				else if (LabelStyle.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height / 2f);
					gap.Inside = Math.Max(gap.Inside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - LabelStyle.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height + Width / 2f);
				}
			}
			GraphicsPath barPath = GetBarPath(gap.Inside, gap.Outside);
			Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, barPath);
		}

		internal GraphicsPath GetShadowPath()
		{
			if (base.Visible && base.ShadowOffset != 0f && Width > 0f)
			{
				GraphicsPath barPath = GetBarPath(Width / 2f, Width / 2f);
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					barPath.Transform(matrix);
					return barPath;
				}
			}
			return null;
		}

		private void RenderBar(GaugeGraphics g)
		{
			using (GraphicsPath path = GetBarPath(Width / 2f, Width / 2f))
			{
				g.DrawPathAbs(path, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, base.FillGradientType, base.FillGradientEndColor, base.BorderColor, base.BorderWidth, base.BorderStyle, PenAlignment.Outset);
			}
		}

		internal override void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = GetPositionFromValue(value);
			PointF absolutePoint = g.GetAbsolutePoint(GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				if (ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
				{
					matrix.RotateAt(90f, absolutePoint);
				}
				if (tickMark.Placement == Placement.Outside)
				{
					matrix.RotateAt(180f, absolutePoint);
				}
				DrawTickMark(g, tickMark, value, offset, matrix);
			}
		}

		internal override LinearLabelStyle GetLabelStyle()
		{
			return LabelStyle;
		}

		private void DrawLabel(Placement placement, string labelStr, double value, float labelPos, float rotateLabelAngle, Font font, Color color, FontUnit fontUnit)
		{
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Near;
			float num = GetPositionFromValue(value);
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round(num), value, placement);
			if (MarkerPosition.IsExistsInArray(labels, markerPosition))
			{
				return;
			}
			if (labelStr.Length > 0)
			{
				labels.Add(markerPosition);
			}
			GaugeGraphics graph = Common.Graph;
			using (Brush brush2 = new SolidBrush(color))
			{
				Font resizedFont = GetResizedFont(font, fontUnit);
				try
				{
					float num2 = 0f;
					if (ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
					{
						num2 = 90f;
					}
					SizeF size = graph.MeasureString(labelStr, resizedFont);
					float contactPointOffset = Utils.GetContactPointOffset(size, rotateLabelAngle - num2);
					PointF absolutePoint = graph.GetAbsolutePoint(GetPoint(num, labelPos));
					switch (placement)
					{
					case Placement.Inside:
						if (ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
						{
							absolutePoint.X -= contactPointOffset;
						}
						else
						{
							absolutePoint.Y -= contactPointOffset;
						}
						break;
					case Placement.Outside:
						if (ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
						{
							absolutePoint.X += contactPointOffset;
						}
						else
						{
							absolutePoint.Y += contactPointOffset;
						}
						break;
					}
					RectangleF rectangleF = new RectangleF(absolutePoint, new SizeF(0f, 0f));
					rectangleF.Inflate(size.Width / 2f, size.Height / 2f);
					Matrix transform = graph.Transform;
					Matrix matrix = graph.Transform.Clone();
					try
					{
						if (rotateLabelAngle == 0f)
						{
							if (base.ShadowOffset != 0f)
							{
								using (Brush brush = graph.GetShadowBrush())
								{
									RectangleF layoutRectangle = rectangleF;
									layoutRectangle.Offset(base.ShadowOffset, base.ShadowOffset);
									graph.DrawString(labelStr, resizedFont, brush, layoutRectangle, stringFormat);
								}
							}
							graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
							return;
						}
						TextRenderingHint textRenderingHint = graph.TextRenderingHint;
						try
						{
							if (textRenderingHint == TextRenderingHint.ClearTypeGridFit)
							{
								graph.TextRenderingHint = TextRenderingHint.AntiAlias;
							}
							if (base.ShadowOffset != 0f)
							{
								using (Brush brush3 = graph.GetShadowBrush())
								{
									using (Matrix matrix2 = matrix.Clone())
									{
										matrix2.Translate(base.ShadowOffset, base.ShadowOffset);
										matrix2.RotateAt(rotateLabelAngle, absolutePoint);
										graph.Transform = matrix2;
										graph.DrawString(labelStr, resizedFont, brush3, rectangleF, stringFormat);
									}
								}
							}
							matrix.RotateAt(rotateLabelAngle, absolutePoint);
							graph.Transform = matrix;
							graph.DrawString(labelStr, resizedFont, brush2, rectangleF, stringFormat);
						}
						finally
						{
							graph.TextRenderingHint = textRenderingHint;
						}
					}
					finally
					{
						matrix.Dispose();
						graph.Transform = transform;
					}
				}
				finally
				{
					if (resizedFont != font)
					{
						resizedFont.Dispose();
					}
				}
			}
		}

		private void RenderLabels(GaugeGraphics g)
		{
			if (!LabelStyle.Visible)
			{
				return;
			}
			double interval = GetInterval(IntervalTypes.Labels);
			float offsetLabelPos = GetOffsetLabelPos(LabelStyle.Placement, LabelStyle.DistanceFromScale, Position);
			double minimumLog = base.MinimumLog;
			double intervalOffset = GetIntervalOffset(IntervalTypes.Labels);
			Color textColor = LabelStyle.TextColor;
			CustomTickMark endLabelTickMark = GetEndLabelTickMark();
			if (LabelStyle.ShowEndLabels && intervalOffset > 0.0)
			{
				textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
				DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, value: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, fontUnit: LabelStyle.FontUnit);
				if (endLabelTickMark != null)
				{
					DrawTickMark(g, endLabelTickMark, minimumLog, GetTickMarkOffset(endLabelTickMark));
				}
			}
			minimumLog += intervalOffset;
			double num = 0.0;
			while (minimumLog <= base.Maximum)
			{
				bool flag = true;
				foreach (CustomLabel customLabel in base.CustomLabels)
				{
					if (Math.Abs(customLabel.Value - minimumLog) < 1E-07 && customLabel.Placement == LabelStyle.Placement && Math.Abs(customLabel.DistanceFromScale - LabelStyle.DistanceFromScale) < 1f)
					{
						flag = false;
					}
				}
				if (!LabelStyle.ShowEndLabels && (minimumLog == base.MinimumLog || minimumLog == base.Maximum))
				{
					flag = false;
				}
				if (flag)
				{
					textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
					DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, value: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, fontUnit: LabelStyle.FontUnit);
				}
				num = minimumLog;
				minimumLog = GetNextPosition(minimumLog, interval, forceLinear: false);
			}
			if (LabelStyle.ShowEndLabels && num < base.Maximum)
			{
				minimumLog = base.Maximum;
				textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
				DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, value: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, fontUnit: LabelStyle.FontUnit);
				if (endLabelTickMark != null)
				{
					DrawTickMark(g, endLabelTickMark, minimumLog, GetTickMarkOffset(endLabelTickMark));
				}
			}
		}

		internal override void DrawCustomLabel(CustomLabel label)
		{
			if (staticRendering)
			{
				float offsetLabelPos = GetOffsetLabelPos(label.Placement, label.DistanceFromScale, Position);
				string text = label.Text;
				DrawLabel(label.Placement, text, label.Value, offsetLabelPos, label.FontAngle, label.Font, label.TextColor, label.FontUnit);
			}
			if ((label.TickMarkStyle.Visible && !base.TickMarksOnTop) || !staticRendering)
			{
				DrawTickMark(Common.Graph, label.TickMarkStyle, label.Value, GetTickMarkOffset(label.TickMarkStyle));
			}
		}

		internal override void DrawSpecialPosition(GaugeGraphics g, SpecialPosition label, float angle)
		{
			if (label.Enable)
			{
				LinearPinLabel labelStyle = ((LinearSpecialPosition)label).LabelStyle;
				if (labelStyle.Text != string.Empty && staticRendering)
				{
					DrawLabel(labelStyle.Placement, labelStyle.Text, GetValueFromPosition(angle), GetOffsetLabelPos(labelStyle.Placement, labelStyle.DistanceFromScale, Position), labelStyle.FontAngle, labelStyle.Font, labelStyle.TextColor, labelStyle.FontUnit);
				}
				if ((label.Visible && !base.TickMarksOnTop) || !staticRendering)
				{
					float tickMarkOffset = GetTickMarkOffset(label);
					DrawTickMark(g, label, GetValueFromPosition(angle), tickMarkOffset);
				}
			}
		}

		internal void RenderStaticElements(GaugeGraphics g)
		{
			if (!base.Visible)
			{
				return;
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(Name));
			g.StartHotRegion(this);
			GraphicsState gstate = g.Save();
			try
			{
				staticRendering = true;
				if (!base.TickMarksOnTop)
				{
					markers.Clear();
				}
				labels.Clear();
				RenderBar(g);
				RenderCustomLabels(g);
				if (!base.TickMarksOnTop)
				{
					RenderGrid(g);
				}
				RenderLabels(g);
				RenderPins(g);
				SetScaleHitTestPath(g);
				if (!base.TickMarksOnTop)
				{
					markers.Sort();
				}
			}
			finally
			{
				g.Restore(gstate);
				g.EndHotRegion();
			}
			Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(Name));
		}

		internal void RenderDynamicElements(GaugeGraphics g)
		{
			if (base.Visible && base.TickMarksOnTop)
			{
				GraphicsState gstate = g.Save();
				try
				{
					staticRendering = false;
					markers.Clear();
					RenderCustomLabels(g);
					RenderGrid(g);
					RenderPins(g);
					markers.Sort();
				}
				finally
				{
					g.Restore(gstate);
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		public new LinearGauge GetGauge()
		{
			if (Collection != null)
			{
				return (LinearGauge)Collection.parent;
			}
			return null;
		}

		protected override bool IsReversed()
		{
			if (ParentElement != null && ParentGauge.GetOrientation() == GaugeOrientation.Vertical)
			{
				return !base.IsReversed();
			}
			return base.IsReversed();
		}

		protected override PointF GetPoint(float position, float offset)
		{
			PointF empty = PointF.Empty;
			if (ParentGauge.GetOrientation() == GaugeOrientation.Horizontal)
			{
				empty.X = position;
				empty.Y = Position + offset;
			}
			else
			{
				empty.Y = position;
				empty.X = Position + offset;
			}
			return empty;
		}

		internal override double GetValue(PointF c, PointF p)
		{
			if (Common != null)
			{
				HotRegionList hotRegionList = Common.GaugeCore.HotRegionList;
				int num = hotRegionList.LocateObject(GetGauge());
				if (num != -1)
				{
					RectangleF boundingRectangle = ((HotRegion)hotRegionList.List[num]).BoundingRectangle;
					float num2 = (ParentGauge.GetOrientation() != 0) ? ((p.Y - boundingRectangle.Y) / boundingRectangle.Height * 100f) : ((p.X - boundingRectangle.X) / boundingRectangle.Width * 100f);
					return GetValueFromPosition(num2);
				}
			}
			return double.NaN;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (GetGauge() == null)
			{
				return;
			}
			Stack stack = new Stack();
			for (NamedElement namedElement = GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)GetGauge()).GetBoundRect(g));
			Gap gap = new Gap(Position);
			gap.SetOffset(Placement.Cross, Width);
			gap.SetBase();
			if (MajorTickMark.Visible)
			{
				gap.SetOffsetBase(MajorTickMark.Placement, MajorTickMark.Length);
				if (MajorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, MajorTickMark.DistanceFromScale + MajorTickMark.Length + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - MajorTickMark.DistanceFromScale);
				}
				else if (MajorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MajorTickMark.DistanceFromScale + MajorTickMark.Length / 2f);
					gap.Inside = Math.Max(gap.Inside, MajorTickMark.DistanceFromScale + MajorTickMark.Length / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MajorTickMark.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, MajorTickMark.DistanceFromScale + MajorTickMark.Length + Width / 2f);
				}
			}
			if (MinorTickMark.Visible)
			{
				gap.SetOffsetBase(MinorTickMark.Placement, MinorTickMark.Length);
				if (MinorTickMark.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, MinorTickMark.DistanceFromScale + MinorTickMark.Length + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - MinorTickMark.DistanceFromScale);
				}
				else if (MinorTickMark.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MinorTickMark.DistanceFromScale + MinorTickMark.Length / 2f);
					gap.Inside = Math.Max(gap.Inside, MinorTickMark.DistanceFromScale + MinorTickMark.Length / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - MinorTickMark.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, MinorTickMark.DistanceFromScale + MinorTickMark.Length + Width / 2f);
				}
			}
			if (LabelStyle.Visible)
			{
				if (LabelStyle.Placement == Placement.Outside)
				{
					gap.Outside = Math.Max(gap.Outside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height + Width / 2f);
					gap.Inside = Math.Max(gap.Inside, 0f - LabelStyle.DistanceFromScale);
				}
				else if (LabelStyle.Placement == Placement.Cross)
				{
					gap.Outside = Math.Max(gap.Outside, 0f - LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height / 2f);
					gap.Inside = Math.Max(gap.Inside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height / 2f);
				}
				else
				{
					gap.Outside = Math.Max(gap.Outside, 0f - LabelStyle.DistanceFromScale);
					gap.Inside = Math.Max(gap.Inside, LabelStyle.DistanceFromScale + (float)LabelStyle.Font.Height + Width / 2f);
				}
			}
			using (GraphicsPath graphicsPath = GetBarPath(gap.Inside, gap.Outside))
			{
				if (graphicsPath != null)
				{
					RectangleF bounds = graphicsPath.GetBounds();
					g.DrawSelection(bounds, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
				}
			}
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			LinearScale linearScale = new LinearScale();
			binaryFormatSerializer.Deserialize(linearScale, stream);
			return linearScale;
		}
	}
}
