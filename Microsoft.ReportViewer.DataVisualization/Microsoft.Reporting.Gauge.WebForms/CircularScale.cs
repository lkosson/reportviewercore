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
	[TypeConverter(typeof(CircularScaleConverter))]
	internal sealed class CircularScale : ScaleBase, ISelectable
	{
		private float radius = 37f;

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeLabelStyle")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularLabelStyle LabelStyle
		{
			get
			{
				return (CircularLabelStyle)baseLabelStyle;
			}
			set
			{
				baseLabelStyle = value;
				baseLabelStyle.Parent = this;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularScale_Radius")]
		[TypeConverter(typeof(FloatAutoValueConverter))]
		[NotifyParentProperty(true)]
		[DefaultValue(37f)]
		[ValidateBound(5.0, 90.0)]
		public float Radius
		{
			get
			{
				return radius;
			}
			set
			{
				if (value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange_min_open", 0));
				}
				radius = value;
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularScale_StartAngle")]
		[DefaultValue(20f)]
		[ValidateBound(0.0, 360.0)]
		public float StartAngle
		{
			get
			{
				return base.StartPosition;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				_startPosition = value;
				InvalidateEndPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularScale_SweepAngle")]
		[DefaultValue(320f)]
		[ValidateBound(0.0, 360.0)]
		public float SweepAngle
		{
			get
			{
				return base.SweepPosition;
			}
			set
			{
				if (value > 360f || value < 0f)
				{
					throw new ArgumentException(Utils.SRGetStr("ExceptionOutOfrange", 0, 360));
				}
				_sweepPosition = value;
				InvalidateEndPosition();
				Invalidate();
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeCircularScale_MajorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularMajorTickMark MajorTickMark
		{
			get
			{
				return (CircularMajorTickMark)base.MajorTickMarkInt;
			}
			set
			{
				base.MajorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLabelsAndTickMarks")]
		[SRDescription("DescriptionAttributeCircularScale_MinorTickMark")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularMinorTickMark MinorTickMark
		{
			get
			{
				return (CircularMinorTickMark)base.MinorTickMarkInt;
			}
			set
			{
				base.MinorTickMarkInt = value;
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularScale_GaugePivotPoint")]
		[TypeConverter(typeof(LocationConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DefaultValue(typeof(GaugeLocation), "50F, 50F")]
		[ValidateBound(100.0, 100.0)]
		public GaugeLocation GaugePivotPoint
		{
			get
			{
				return GetGauge()?.PivotPoint;
			}
			set
			{
				CircularGauge gauge = GetGauge();
				if (gauge != null)
				{
					gauge.PivotPoint = value;
				}
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeMinimumPin")]
		[TypeConverter(typeof(NoNameExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CircularSpecialPosition MinimumPin
		{
			get
			{
				return (CircularSpecialPosition)minimumPin;
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
		public new CircularSpecialPosition MaximumPin
		{
			get
			{
				return (CircularSpecialPosition)maximumPin;
			}
			set
			{
				maximumPin = value;
				maximumPin.Parent = this;
				Invalidate();
			}
		}

		public CircularScale()
		{
			_startPosition = 20f;
			_sweepPosition = 320f;
			InvalidateEndPosition();
			base.MajorTickMarkInt = new CircularMajorTickMark(this);
			base.MinorTickMarkInt = new CircularMinorTickMark(this);
			baseLabelStyle = new CircularLabelStyle(this);
			maximumPin = new CircularSpecialPosition(this);
			minimumPin = new CircularSpecialPosition(this);
		}

		internal float GetRadius()
		{
			return 100f;
		}

		internal PointF GetPivotPoint()
		{
			return new PointF(50f, 50f);
		}

		private GraphicsPath GetBarPath(float barOffsetInside, float barOffsetOutside, float angularMargin)
		{
			GaugeGraphics graph = Common.Graph;
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF location = new PointF(GetPivotPoint().X - GetRadius(), GetPivotPoint().Y - GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF(GetRadius() * 2f, GetRadius() * 2f));
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = (float)Math.Atan(MajorTickMark.Width / 2f / GetRadius());
			}
			if (MinorTickMark.Visible)
			{
				num = (float)Math.Max(num, Math.Atan(MinorTickMark.Width / 2f / GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			num += angularMargin;
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			float startAngle2 = Utils.ToGDIAngle(base.EndPosition + num);
			float num2 = base.EndPosition - base.StartPosition + num * 2f;
			graphicsPath.StartFigure();
			relative.Inflate(barOffsetOutside, barOffsetOutside);
			graphicsPath.AddArc(graph.GetAbsoluteRectangle(relative), startAngle, num2);
			relative.Inflate(0f - (barOffsetInside + barOffsetOutside), 0f - (barOffsetInside + barOffsetOutside));
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(relative);
			if (absoluteRectangle.Width > 0f && absoluteRectangle.Height > 0f)
			{
				graphicsPath.AddArc(absoluteRectangle, startAngle2, 0f - num2);
			}
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		private void SetScaleHitTestPath(GaugeGraphics g)
		{
			Gap gap = new Gap(GetRadius());
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
			GraphicsPath barPath = GetBarPath(gap.Inside, gap.Outside, 0f);
			Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(GetPivotPoint()), barPath);
		}

		internal GraphicsPath GetCompoundPath(GaugeGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF location = new PointF(GetPivotPoint().X - GetRadius(), GetPivotPoint().Y - GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF(GetRadius() * 2f, GetRadius() * 2f));
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = (float)Math.Atan(MajorTickMark.Width / 2f / GetRadius());
			}
			if (MinorTickMark.Visible)
			{
				num = (float)Math.Max(num, Math.Atan(MinorTickMark.Width / 2f / GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			float startAngle2 = Utils.ToGDIAngle(base.EndPosition + num);
			float num2 = base.EndPosition - base.StartPosition + num * 2f;
			graphicsPath.StartFigure();
			graphicsPath.AddArc(g.GetAbsoluteRectangle(relative), startAngle, num2);
			relative.Inflate((0f - relative.Width) / 2f, (0f - relative.Height) / 2f);
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(relative);
			absoluteRectangle.Inflate(15f, 15f);
			graphicsPath.AddArc(absoluteRectangle, startAngle2, 360f - num2);
			graphicsPath.CloseAllFigures();
			return graphicsPath;
		}

		internal GraphicsPath GetShadowPath(GaugeGraphics g)
		{
			if (base.Visible && base.ShadowOffset != 0f && Width > 0f)
			{
				SetDrawRegion(g);
				GraphicsPath graphicsPath = new GraphicsPath();
				using (GraphicsPath addingPath = GetBarPath(Width / 2f, Width / 2f, 0f))
				{
					graphicsPath.AddPath(addingPath, connect: false);
				}
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					graphicsPath.Transform(matrix);
				}
				PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
				g.RestoreDrawRegion();
				PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
					graphicsPath.Transform(matrix2);
					return graphicsPath;
				}
			}
			return null;
		}

		private void RenderBar(GaugeGraphics g)
		{
			if (Width > 0f)
			{
				using (GraphicsPath path = GetBarPath(Width / 2f, Width / 2f, 0f))
				{
					g.DrawPathAbs(path, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.Center, base.FillGradientType, base.FillGradientEndColor, base.BorderColor, base.BorderWidth, base.BorderStyle, PenAlignment.Outset);
				}
			}
		}

		internal override void DrawTickMark(GaugeGraphics g, CustomTickMark tickMark, double value, float offset)
		{
			float num = GetPositionFromValueNormalized(value);
			PointF absolutePoint = g.GetAbsolutePoint(GetPoint(num, offset));
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(num, absolutePoint);
				if (tickMark.Placement == Placement.Inside)
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

		private void DrawLabel(Placement placement, string labelStr, double position, float labelPos, float rotateLabelAngle, Font font, Color color, bool rotateLabels, bool allowUpsideDown, FontUnit fontUnit)
		{
			float num = GetPositionFromValueNormalized(position);
			if (rotateLabels)
			{
				rotateLabelAngle += num + 180f;
				rotateLabelAngle %= 360f;
				if (!allowUpsideDown && rotateLabelAngle > 90f && rotateLabelAngle < 270f)
				{
					rotateLabelAngle += 180f;
					rotateLabelAngle %= 360f;
				}
			}
			MarkerPosition markerPosition = new MarkerPosition((float)Math.Round(num), position, placement);
			if (MarkerPosition.IsExistsInArray(labels, markerPosition))
			{
				return;
			}
			if (labelStr.Length > 0)
			{
				labels.Add(markerPosition);
			}
			GaugeGraphics graph = Common.Graph;
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			using (Brush brush2 = new SolidBrush(color))
			{
				Font resizedFont = GetResizedFont(font, fontUnit);
				try
				{
					SizeF relativeSize = graph.GetRelativeSize(graph.MeasureString(labelStr, resizedFont));
					relativeSize.Height -= relativeSize.Height / 8f;
					float contactPointOffset = Utils.GetContactPointOffset(relativeSize, num - rotateLabelAngle);
					float offset = labelPos;
					switch (placement)
					{
					case Placement.Inside:
						offset = labelPos - Math.Max(0f, contactPointOffset);
						break;
					case Placement.Outside:
						offset = labelPos + Math.Max(0f, contactPointOffset);
						break;
					}
					PointF absolutePoint = graph.GetAbsolutePoint(GetPoint(num, offset));
					relativeSize = graph.GetAbsoluteSize(relativeSize);
					RectangleF layoutRectangle = new RectangleF(absolutePoint, new SizeF(0f, 0f));
					layoutRectangle.Inflate(relativeSize.Width / 2f, relativeSize.Height / 2f);
					Matrix transform = graph.Transform;
					Matrix matrix = graph.Transform.Clone();
					try
					{
						TextRenderingHint textRenderingHint = graph.TextRenderingHint;
						try
						{
							if (textRenderingHint == TextRenderingHint.ClearTypeGridFit)
							{
								graph.TextRenderingHint = TextRenderingHint.AntiAlias;
							}
							if (base.ShadowOffset != 0f)
							{
								using (Brush brush = graph.GetShadowBrush())
								{
									using (Matrix matrix2 = matrix.Clone())
									{
										matrix2.Translate(base.ShadowOffset, base.ShadowOffset);
										matrix2.RotateAt(rotateLabelAngle, absolutePoint);
										graph.Transform = matrix2;
										graph.DrawString(labelStr, resizedFont, brush, layoutRectangle, stringFormat);
									}
								}
							}
							matrix.RotateAt(rotateLabelAngle, absolutePoint);
							graph.Transform = matrix;
							graph.DrawString(labelStr, resizedFont, brush2, layoutRectangle, stringFormat);
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
			float offsetLabelPos = GetOffsetLabelPos(LabelStyle.Placement, LabelStyle.DistanceFromScale, GetRadius());
			double minimumLog = base.MinimumLog;
			double intervalOffset = GetIntervalOffset(IntervalTypes.Labels);
			Color textColor = LabelStyle.TextColor;
			CustomTickMark endLabelTickMark = GetEndLabelTickMark();
			if (LabelStyle.ShowEndLabels && intervalOffset > 0.0)
			{
				textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
				DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, position: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, rotateLabels: LabelStyle.RotateLabels, allowUpsideDown: LabelStyle.AllowUpsideDown, fontUnit: LabelStyle.FontUnit);
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
				if (base.SweepPosition > 359f && minimumLog == base.MinimumLog)
				{
					flag = false;
				}
				if (flag)
				{
					textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
					DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, position: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, rotateLabels: LabelStyle.RotateLabels, allowUpsideDown: LabelStyle.AllowUpsideDown, fontUnit: LabelStyle.FontUnit);
				}
				num = minimumLog;
				minimumLog = GetNextPosition(minimumLog, interval, forceLinear: false);
			}
			if (LabelStyle.ShowEndLabels && num < base.Maximum)
			{
				minimumLog = base.Maximum;
				textColor = GetRangeLabelsColor(minimumLog, LabelStyle.TextColor);
				DrawLabel(labelStr: (Common.GaugeContainer.FormatNumberHandler == null) ? string.Format(CultureInfo.CurrentCulture, LabelStyle.GetFormatStr(), minimumLog * base.Multiplier) : Common.GaugeContainer.FormatNumberHandler(Common.GaugeContainer, minimumLog * base.Multiplier, LabelStyle.FormatString), placement: LabelStyle.Placement, position: minimumLog, labelPos: offsetLabelPos, rotateLabelAngle: LabelStyle.FontAngle, font: LabelStyle.Font, color: textColor, rotateLabels: LabelStyle.RotateLabels, allowUpsideDown: LabelStyle.AllowUpsideDown, fontUnit: LabelStyle.FontUnit);
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
				float offsetLabelPos = GetOffsetLabelPos(label.Placement, label.DistanceFromScale, GetRadius());
				string text = label.Text;
				DrawLabel(label.Placement, text, label.Value, offsetLabelPos, label.FontAngle, label.Font, label.TextColor, label.RotateLabel, label.AllowUpsideDown, label.FontUnit);
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
				CircularPinLabel labelStyle = ((CircularSpecialPosition)label).LabelStyle;
				if (labelStyle.Text != string.Empty && staticRendering)
				{
					DrawLabel(labelStyle.Placement, labelStyle.Text, base.GetValueFromPosition(angle), GetOffsetLabelPos(labelStyle.Placement, labelStyle.DistanceFromScale, GetRadius()), labelStyle.FontAngle, labelStyle.Font, labelStyle.TextColor, labelStyle.RotateLabel, labelStyle.AllowUpsideDown, labelStyle.FontUnit);
				}
				if ((label.Visible && !base.TickMarksOnTop) || !staticRendering)
				{
					float tickMarkOffset = GetTickMarkOffset(label);
					DrawTickMark(g, label, base.GetValueFromPosition(angle), tickMarkOffset);
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
			SetDrawRegion(g);
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
				g.RestoreDrawRegion();
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
				SetDrawRegion(g);
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
					g.RestoreDrawRegion();
					g.Restore(gstate);
				}
			}
		}

		internal void SetDrawRegion(GaugeGraphics g)
		{
			RectangleF rect = new RectangleF(GetGauge().PivotPoint.ToPoint(), new SizeF(0f, 0f));
			rect.Inflate(radius / 2f, radius / 2f);
			g.CreateDrawRegion(rect);
		}

		public override string ToString()
		{
			return Name;
		}

		public new CircularGauge GetGauge()
		{
			if (Collection != null)
			{
				return (CircularGauge)Collection.parent;
			}
			return null;
		}

		private float GetPositionFromValueNormalized(double value)
		{
			return Utils.NormalizeAngle(base.GetPositionFromValue(value));
		}

		protected override PointF GetPoint(float position, float offset)
		{
			PointF pivotPoint = GetPivotPoint();
			float num = GetRadius() + offset;
			PointF[] array = new PointF[1]
			{
				new PointF(pivotPoint.X, pivotPoint.Y + num)
			};
			using (Matrix matrix = new Matrix())
			{
				matrix.RotateAt(position, pivotPoint);
				matrix.TransformPoints(array);
			}
			return array[0];
		}

		internal override double GetValue(PointF c, PointF p)
		{
			Math.Sqrt(Math.Pow(p.X - c.X, 2.0) + Math.Pow(p.Y - c.Y, 2.0));
			float num = (float)(Math.Atan2(c.X - p.X, c.Y - p.Y) * 180.0 / Math.PI);
			num += 180f;
			num = 360f - num;
			float num2 = StartAngle + SweepAngle - 360f + (360f - SweepAngle) / 2f;
			if (num2 > 0f && num < num2)
			{
				num += 360f;
			}
			return GetValueFromPosition(num);
		}

		internal float GetLargestRadius(GaugeGraphics g)
		{
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = Math.Max(num, GetTickMarkOffset(g, MajorTickMark));
			}
			if (MinorTickMark.Visible)
			{
				num = Math.Max(num, GetTickMarkOffset(g, MinorTickMark));
			}
			if (LabelStyle.Visible)
			{
				num = Math.Max(num, GetLabelsOffset(g));
			}
			if (MinimumPin.Enable)
			{
				num = Math.Max(num, GetTickMarkOffset(MinimumPin));
			}
			if (MaximumPin.Enable)
			{
				num = Math.Max(num, GetTickMarkOffset(MaximumPin));
			}
			if (LabelStyle.Visible && MajorTickMark.Visible)
			{
				num = Math.Max(num, GetLabelsOffset(g) + GetTickMarkOffset(g, MajorTickMark));
			}
			if (LabelStyle.Visible && MinorTickMark.Visible)
			{
				num = Math.Max(num, GetLabelsOffset(g) + GetTickMarkOffset(g, MinorTickMark));
			}
			foreach (CustomLabel customLabel in base.CustomLabels)
			{
				if (customLabel.Visible)
				{
					num = Math.Max(num, GetCustomLabelOffset(g, customLabel));
				}
			}
			return Radius + num * Radius / 100f;
		}

		internal float GetCustomLabelOffset(GaugeGraphics g, CustomLabel customLabel)
		{
			float num = (customLabel.FontUnit != 0) ? (g.GetRelativeSize(new SizeF(customLabel.Font.Size * 1.3f, 0f)).Width * (100f / Radius)) : customLabel.Font.Size;
			float num2 = 0f;
			num2 = ((customLabel.Placement == Placement.Inside) ? (0f - Width / 2f - customLabel.DistanceFromScale) : ((customLabel.Placement != Placement.Cross) ? (customLabel.DistanceFromScale + num) : (0f - customLabel.DistanceFromScale + num / 2f)));
			float val = (customLabel.TickMarkStyle.Placement == Placement.Inside) ? (0f - Width / 2f - customLabel.TickMarkStyle.DistanceFromScale) : ((customLabel.TickMarkStyle.Placement != Placement.Cross) ? (customLabel.TickMarkStyle.DistanceFromScale + customLabel.TickMarkStyle.Length) : (0f - customLabel.TickMarkStyle.DistanceFromScale + customLabel.TickMarkStyle.Length / 2f));
			return Math.Max(num2, val);
		}

		internal float GetTickMarkOffset(GaugeGraphics g, TickMark tickMark)
		{
			if (tickMark.Placement == Placement.Inside)
			{
				return 0f - Width / 2f - tickMark.DistanceFromScale;
			}
			if (tickMark.Placement == Placement.Cross)
			{
				return 0f - tickMark.DistanceFromScale + tickMark.Length / 2f;
			}
			return tickMark.DistanceFromScale + tickMark.Length;
		}

		internal float GetLabelsOffset(GaugeGraphics g)
		{
			float num = 0f;
			float num2 = (LabelStyle.FontUnit != 0) ? (g.GetRelativeSize(new SizeF(LabelStyle.Font.Size * 1.3f, 0f)).Width * (100f / Radius)) : LabelStyle.Font.Size;
			if (LabelStyle.Placement == Placement.Inside)
			{
				return 0f - Width / 2f - LabelStyle.DistanceFromScale;
			}
			if (LabelStyle.Placement == Placement.Cross)
			{
				return 0f - LabelStyle.DistanceFromScale + num2 / 2f;
			}
			return LabelStyle.DistanceFromScale + num2;
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
			SetDrawRegion(g);
			Gap gap = new Gap(GetRadius());
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
			float angularMargin = 4f;
			float num = 5f;
			using (GraphicsPath graphicsPath = GetBarPath(gap.Inside + num, gap.Outside + num, angularMargin))
			{
				if (graphicsPath != null)
				{
					PointF[] selectionMarkers = GetSelectionMarkers(g, gap.Inside + num, gap.Outside + num, angularMargin);
					g.DrawRadialSelection(g, graphicsPath, selectionMarkers, designTimeSelection, Common.GaugeCore.SelectionBorderColor, Common.GaugeCore.SelectionMarkerColor);
				}
			}
			g.RestoreDrawRegion();
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				_ = item2;
				g.RestoreDrawRegion();
			}
		}

		internal PointF[] GetSelectionMarkers(GaugeGraphics g, float barOffsetInside, float barOffsetOutside, float angularMargin)
		{
			PointF location = new PointF(GetPivotPoint().X - GetRadius(), GetPivotPoint().Y - GetRadius());
			RectangleF relative = new RectangleF(location, new SizeF(GetRadius() * 2f, GetRadius() * 2f));
			float num = 0f;
			if (MajorTickMark.Visible)
			{
				num = (float)Math.Atan(MajorTickMark.Width / 2f / GetRadius());
			}
			if (MinorTickMark.Visible)
			{
				num = (float)Math.Max(num, Math.Atan(MinorTickMark.Width / 2f / GetRadius()));
			}
			num = Utils.Rad2Deg(num);
			num += angularMargin;
			float startAngle = Utils.ToGDIAngle(base.StartPosition - num);
			Utils.ToGDIAngle(base.EndPosition + num);
			float sweepAngle = base.EndPosition - base.StartPosition + num * 2f;
			ArrayList arrayList = new ArrayList();
			relative.Inflate(barOffsetOutside, barOffsetOutside);
			float flatness = 0.1f;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddArc(g.GetAbsoluteRectangle(relative), startAngle, sweepAngle);
				graphicsPath.Flatten(null, flatness);
				PointF[] pathPoints = graphicsPath.PathPoints;
				GetBoundsFromPoints(pathPoints, out PointF minPoint, out PointF maxPoint);
				if (SweepAngle + num * 2f < 360f)
				{
					arrayList.Add(pathPoints[0]);
					arrayList.Add(pathPoints[pathPoints.Length - 1]);
				}
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = false;
				for (int i = 1; i < pathPoints.Length - 1; i++)
				{
					if (!flag && pathPoints[i].X == minPoint.X)
					{
						flag = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag2 && pathPoints[i].Y == minPoint.Y)
					{
						flag2 = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag3 && pathPoints[i].X == maxPoint.X)
					{
						flag3 = true;
						arrayList.Add(pathPoints[i]);
					}
					if (!flag4 && pathPoints[i].Y == maxPoint.Y)
					{
						flag3 = true;
						arrayList.Add(pathPoints[i]);
					}
				}
			}
			relative.Inflate(0f - (barOffsetInside + barOffsetOutside), 0f - (barOffsetInside + barOffsetOutside));
			RectangleF absoluteRectangle = g.GetAbsoluteRectangle(relative);
			if (absoluteRectangle.Width > 0f && absoluteRectangle.Height > 0f)
			{
				using (GraphicsPath graphicsPath2 = new GraphicsPath())
				{
					graphicsPath2.AddArc(absoluteRectangle, startAngle, sweepAngle);
					graphicsPath2.Flatten(null, flatness);
					PointF[] pathPoints2 = graphicsPath2.PathPoints;
					GetBoundsFromPoints(pathPoints2, out PointF minPoint2, out PointF maxPoint2);
					if (SweepAngle + num * 2f < 360f)
					{
						arrayList.Add(pathPoints2[0]);
						arrayList.Add(pathPoints2[pathPoints2.Length - 1]);
					}
					bool flag5 = false;
					bool flag6 = false;
					bool flag7 = false;
					bool flag8 = false;
					for (int j = 1; j < pathPoints2.Length - 1; j++)
					{
						if (!flag5 && pathPoints2[j].X == minPoint2.X)
						{
							flag5 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag6 && pathPoints2[j].Y == minPoint2.Y)
						{
							flag6 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag7 && pathPoints2[j].X == maxPoint2.X)
						{
							flag7 = true;
							arrayList.Add(pathPoints2[j]);
						}
						if (!flag8 && pathPoints2[j].Y == maxPoint2.Y)
						{
							flag7 = true;
							arrayList.Add(pathPoints2[j]);
						}
					}
				}
			}
			return (PointF[])arrayList.ToArray(typeof(PointF));
		}

		internal void GetBoundsFromPoints(PointF[] points, out PointF minPoint, out PointF maxPoint)
		{
			minPoint = new PointF(float.MaxValue, float.MaxValue);
			maxPoint = new PointF(float.MinValue, float.MinValue);
			for (int i = 0; i < points.Length; i++)
			{
				minPoint.X = Math.Min(minPoint.X, points[i].X);
				minPoint.Y = Math.Min(minPoint.Y, points[i].Y);
				maxPoint.X = Math.Max(maxPoint.X, points[i].X);
				maxPoint.Y = Math.Max(maxPoint.Y, points[i].Y);
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularScale circularScale = new CircularScale();
			binaryFormatSerializer.Deserialize(circularScale, stream);
			return circularScale;
		}
	}
}
