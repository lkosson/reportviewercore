using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class AxisScrollBar
	{
		internal Axis axis;

		private bool enabled = true;

		private ScrollBarButtonStyles scrollBarButtonStyle = ScrollBarButtonStyles.All;

		private double scrollBarSize = 14.0;

		private int pressedButtonType = int.MaxValue;

		private Color buttonColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Color lineColor = Color.Empty;

		private Color buttonCurrentColor = Color.Empty;

		private Color backCurrentColor = Color.Empty;

		private Color lineCurrentColor = Color.Empty;

		private bool positionInside = true;

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAxisScrollBar_PositionInside")]
		public bool PositionInside
		{
			get
			{
				return positionInside;
			}
			set
			{
				if (positionInside != value)
				{
					positionInside = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeAxisScrollBar_Enabled")]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				if (enabled != value)
				{
					enabled = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[Browsable(false)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public ChartArea ChartArea => axis.chartArea;

		[Browsable(false)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public Axis Axis => axis;

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(ScrollBarButtonStyles.All)]
		[SRDescription("DescriptionAttributeAxisScrollBar_Buttons")]
		public ScrollBarButtonStyles Buttons
		{
			get
			{
				return scrollBarButtonStyle;
			}
			set
			{
				if (scrollBarButtonStyle != value)
				{
					scrollBarButtonStyle = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(14.0)]
		[SRDescription("DescriptionAttributeAxisScrollBar_Size")]
		public double Size
		{
			get
			{
				return scrollBarSize;
			}
			set
			{
				if (scrollBarSize != value)
				{
					if (value < 5.0 || value > 20.0)
					{
						throw new ArgumentOutOfRangeException("value", SR.ExceptionScrollBarSizeInvalid);
					}
					scrollBarSize = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeAxisScrollBar_ButtonColor")]
		public Color ButtonColor
		{
			get
			{
				return buttonColor;
			}
			set
			{
				if (buttonColor != value)
				{
					buttonColor = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeAxisScrollBar_LineColor")]
		public Color LineColor
		{
			get
			{
				return lineColor;
			}
			set
			{
				if (lineColor != value)
				{
					lineColor = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeAxisScrollBar_BackColor")]
		public Color BackColor
		{
			get
			{
				return backColor;
			}
			set
			{
				if (backColor != value)
				{
					backColor = value;
					if (axis != null)
					{
						axis.chartArea.Invalidate(invalidateAreaOnly: false);
					}
				}
			}
		}

		public AxisScrollBar()
		{
		}

		public AxisScrollBar(Axis axis)
		{
			this.axis = axis;
		}

		internal void Initialize()
		{
		}

		internal bool IsVisible()
		{
			return false;
		}

		internal void Paint(ChartGraphics graph)
		{
			int num = 1;
			if (!IsVisible())
			{
				return;
			}
			buttonCurrentColor = buttonColor;
			backCurrentColor = backColor;
			lineCurrentColor = lineColor;
			if (buttonCurrentColor == Color.Empty)
			{
				buttonCurrentColor = axis.chartArea.BackColor;
				if (buttonCurrentColor == Color.Empty)
				{
					buttonCurrentColor = Color.DarkGray;
				}
			}
			if (backCurrentColor == Color.Empty)
			{
				backCurrentColor = axis.chartArea.BackColor;
				if (backCurrentColor == Color.Empty)
				{
					backCurrentColor = Color.LightGray;
				}
			}
			if (lineCurrentColor == Color.Empty)
			{
				lineCurrentColor = axis.LineColor;
				if (lineCurrentColor == Color.Empty)
				{
					lineCurrentColor = Color.Black;
				}
			}
			RectangleF scrollBarRect = GetScrollBarRect();
			graph.FillRectangleRel(scrollBarRect, backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, lineCurrentColor, num, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
			PaintScrollBarConnectionRect(graph, scrollBarRect, num);
			SizeF size = new SizeF(num, num);
			size = graph.GetRelativeSize(size);
			RectangleF scrollBarClientRect = new RectangleF(scrollBarRect.Location, scrollBarRect.Size);
			scrollBarClientRect.Inflate(0f - size.Width, 0f - size.Height);
			foreach (ScrollBarButtonType value in Enum.GetValues(typeof(ScrollBarButtonType)))
			{
				RectangleF scrollBarButtonRect = GetScrollBarButtonRect(scrollBarClientRect, value);
				if (!scrollBarButtonRect.IsEmpty)
				{
					PaintScrollBar3DButton(graph, scrollBarButtonRect, pressedButtonType == (int)value, value);
				}
			}
		}

		private void PaintScrollBarConnectionRect(ChartGraphics graph, RectangleF scrollBarRect, int borderWidth)
		{
			if (this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right)
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			Axis[] axes = this.axis.chartArea.Axes;
			foreach (Axis axis in axes)
			{
				if (axis.AxisPosition == AxisPosition.Left && axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside == this.axis.ScrollBar.PositionInside)
				{
					num = (float)axis.ScrollBar.GetScrollBarRelativeSize();
				}
				if (axis.AxisPosition == AxisPosition.Right && axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside == this.axis.ScrollBar.PositionInside)
				{
					num2 = (float)axis.ScrollBar.GetScrollBarRelativeSize();
				}
			}
			RectangleF rectF = new RectangleF(scrollBarRect.Location, scrollBarRect.Size);
			if (num > 0f)
			{
				rectF.X = scrollBarRect.X - num;
				rectF.Width = num;
				graph.FillRectangleRel(rectF, backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, lineCurrentColor, borderWidth, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
			}
			if (num2 > 0f)
			{
				rectF.X = scrollBarRect.Right;
				rectF.Width = num2;
				graph.FillRectangleRel(rectF, backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, lineCurrentColor, borderWidth, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
			}
		}

		internal void PaintScrollBar3DButton(ChartGraphics graph, RectangleF buttonRect, bool pressedState, ScrollBarButtonType buttonType)
		{
			if (buttonType == ScrollBarButtonType.LargeIncrement || buttonType == ScrollBarButtonType.LargeDecrement)
			{
				return;
			}
			Color gradientColor = ChartGraphics.GetGradientColor(buttonCurrentColor, Color.Black, 0.5);
			Color gradientColor2 = ChartGraphics.GetGradientColor(buttonCurrentColor, Color.Black, 0.8);
			Color gradientColor3 = ChartGraphics.GetGradientColor(buttonCurrentColor, Color.White, 0.5);
			graph.FillRectangleRel(buttonRect, buttonCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, gradientColor, pressedState ? 1 : 0, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
			bool flag = Size <= 12.0;
			if (!pressedState)
			{
				SizeF size = new SizeF(1f, 1f);
				size = graph.GetRelativeSize(size);
				graph.DrawLineRel(flag ? gradientColor3 : buttonCurrentColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.X, buttonRect.Bottom), new PointF(buttonRect.X, buttonRect.Top));
				graph.DrawLineRel(flag ? gradientColor3 : buttonCurrentColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left, buttonRect.Y), new PointF(buttonRect.Right, buttonRect.Y));
				graph.DrawLineRel(flag ? gradientColor : gradientColor2, 1, ChartDashStyle.Solid, new PointF(buttonRect.Right, buttonRect.Bottom), new PointF(buttonRect.Right, buttonRect.Top));
				graph.DrawLineRel(flag ? gradientColor : gradientColor2, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left, buttonRect.Bottom), new PointF(buttonRect.Right, buttonRect.Bottom));
				if (!flag)
				{
					graph.DrawLineRel(gradientColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Right - size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Top + size.Height));
					graph.DrawLineRel(gradientColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left + size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Bottom - size.Height));
					graph.DrawLineRel(gradientColor3, 1, ChartDashStyle.Solid, new PointF(buttonRect.X + size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.X + size.Width, buttonRect.Top + size.Height));
					graph.DrawLineRel(gradientColor3, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left + size.Width, buttonRect.Y + size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Y + size.Height));
				}
			}
			bool flag2 = (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right) ? true : false;
			float num = flag ? 0.5f : 1f;
			if (pressedState)
			{
				graph.TranslateTransform(num, num);
			}
			RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(buttonRect);
			float num2 = flag ? 2 : 3;
			switch (buttonType)
			{
			case ScrollBarButtonType.SmallDecrement:
			{
				PointF[] array2 = new PointF[3];
				if (flag2)
				{
					array2[0].X = absoluteRectangle.X + num2;
					array2[0].Y = absoluteRectangle.Y + (num2 + 1f);
					array2[1].X = absoluteRectangle.X + absoluteRectangle.Width / 2f;
					array2[1].Y = absoluteRectangle.Bottom - num2;
					array2[2].X = absoluteRectangle.Right - num2;
					array2[2].Y = absoluteRectangle.Y + (num2 + 1f);
				}
				else
				{
					array2[0].X = absoluteRectangle.X + num2;
					array2[0].Y = absoluteRectangle.Y + absoluteRectangle.Height / 2f;
					array2[1].X = absoluteRectangle.Right - (num2 + 1f);
					array2[1].Y = absoluteRectangle.Y + num2;
					array2[2].X = absoluteRectangle.Right - (num2 + 1f);
					array2[2].Y = absoluteRectangle.Bottom - num2;
				}
				graph.FillPolygon(new SolidBrush(lineCurrentColor), array2);
				break;
			}
			case ScrollBarButtonType.SmallIncrement:
			{
				PointF[] array = new PointF[3];
				if (flag2)
				{
					array[0].X = absoluteRectangle.X + num2;
					array[0].Y = absoluteRectangle.Bottom - (num2 + 1f);
					array[1].X = absoluteRectangle.X + absoluteRectangle.Width / 2f;
					array[1].Y = absoluteRectangle.Y + num2;
					array[2].X = absoluteRectangle.Right - num2;
					array[2].Y = absoluteRectangle.Bottom - (num2 + 1f);
				}
				else
				{
					array[0].X = absoluteRectangle.Right - num2;
					array[0].Y = absoluteRectangle.Y + absoluteRectangle.Height / 2f;
					array[1].X = absoluteRectangle.X + (num2 + 1f);
					array[1].Y = absoluteRectangle.Y + num2;
					array[2].X = absoluteRectangle.X + (num2 + 1f);
					array[2].Y = absoluteRectangle.Bottom - num2;
				}
				graph.FillPolygon(new SolidBrush(lineCurrentColor), array);
				break;
			}
			case ScrollBarButtonType.ZoomReset:
			{
				Pen pen = new Pen(lineCurrentColor, 1f);
				graph.DrawEllipse(pen, absoluteRectangle.X + num2 - 0.5f, absoluteRectangle.Y + num2 - 0.5f, absoluteRectangle.Width - 2f * num2, absoluteRectangle.Height - 2f * num2);
				graph.DrawLine(pen, absoluteRectangle.X + num2 + 1.5f, absoluteRectangle.Y + absoluteRectangle.Height / 2f - 0.5f, absoluteRectangle.Right - num2 - 2.5f, absoluteRectangle.Y + absoluteRectangle.Height / 2f - 0.5f);
				break;
			}
			}
			if (pressedState)
			{
				graph.TranslateTransform(0f - num, 0f - num);
			}
		}

		internal RectangleF GetScrollBarButtonRect(RectangleF scrollBarClientRect, ScrollBarButtonType buttonType)
		{
			RectangleF result = new RectangleF(scrollBarClientRect.Location, scrollBarClientRect.Size);
			bool flag = (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right) ? true : false;
			SizeF size = new SizeF(1f, 1f);
			size = GetRelativeSize(size);
			SizeF relative = new SizeF(scrollBarClientRect.Width, scrollBarClientRect.Height);
			relative = GetAbsoluteSize(relative);
			if (flag)
			{
				relative.Height = relative.Width;
			}
			else
			{
				relative.Width = relative.Height;
			}
			relative = GetRelativeSize(relative);
			result.Width = relative.Width;
			result.Height = relative.Height;
			if (flag)
			{
				result.X = scrollBarClientRect.X;
			}
			else
			{
				result.Y = scrollBarClientRect.Y;
			}
			switch (buttonType)
			{
			case ScrollBarButtonType.ThumbTracker:
			case ScrollBarButtonType.LargeDecrement:
			case ScrollBarButtonType.LargeIncrement:
				if (flag)
				{
					double num = scrollBarClientRect.Height - (float)GetButtonsNumberAll() * relative.Height;
					result.Height = (float)(GetDataViewPercentage() * (num / 100.0));
					if (result.Height < size.Height * 6f)
					{
						result.Height = size.Height * 6f;
					}
					if (!axis.Reverse)
					{
						result.Y = scrollBarClientRect.Bottom - (float)GetButtonsNumberBottom() * relative.Height - result.Height;
						result.Y -= (float)(GetDataViewPositionPercentage() * (num / 100.0));
						if (result.Y < scrollBarClientRect.Y + (float)GetButtonsNumberTop() * relative.Height + ((GetButtonsNumberTop() == 0) ? 0f : size.Height))
						{
							result.Y = scrollBarClientRect.Y + (float)GetButtonsNumberTop() * relative.Height + ((GetButtonsNumberTop() == 0) ? 0f : size.Height);
						}
					}
					else
					{
						result.Y = scrollBarClientRect.Top + (float)GetButtonsNumberTop() * relative.Height;
						result.Y += (float)(GetDataViewPositionPercentage() * (num / 100.0));
						if (result.Y + result.Height > scrollBarClientRect.Bottom - (float)GetButtonsNumberBottom() * relative.Height - ((GetButtonsNumberBottom() == 0) ? 0f : size.Height))
						{
							result.Y = scrollBarClientRect.Bottom - (float)GetButtonsNumberBottom() * relative.Height - result.Height - ((GetButtonsNumberBottom() == 0) ? 0f : size.Height);
						}
					}
				}
				else
				{
					double num2 = scrollBarClientRect.Width - (float)GetButtonsNumberAll() * relative.Width;
					result.Width = (float)(GetDataViewPercentage() * (num2 / 100.0));
					if (result.Width < size.Width * 6f)
					{
						result.Width = size.Width * 6f;
					}
					if (!axis.Reverse)
					{
						result.X = scrollBarClientRect.X + (float)GetButtonsNumberTop() * relative.Width;
						result.X += (float)(GetDataViewPositionPercentage() * (num2 / 100.0));
						if (result.X + result.Width > scrollBarClientRect.Right - (float)GetButtonsNumberBottom() * relative.Width - ((GetButtonsNumberBottom() == 0) ? 0f : size.Width))
						{
							result.X = scrollBarClientRect.Right - result.Width - (float)GetButtonsNumberBottom() * relative.Width - ((GetButtonsNumberBottom() == 0) ? 0f : size.Width);
						}
					}
					else
					{
						result.X = scrollBarClientRect.Right - (float)GetButtonsNumberBottom() * relative.Width - ((GetButtonsNumberBottom() == 0) ? 0f : size.Width) - result.Width;
						result.X -= (float)(GetDataViewPositionPercentage() * (num2 / 100.0));
						if (result.X < scrollBarClientRect.X + (float)GetButtonsNumberTop() * relative.Width)
						{
							result.X = scrollBarClientRect.X + (float)GetButtonsNumberTop() * relative.Width;
						}
					}
				}
				switch (buttonType)
				{
				case ScrollBarButtonType.LargeDecrement:
					if (flag)
					{
						result.Y = result.Bottom + size.Height;
						result.Height = scrollBarClientRect.Bottom - (float)GetButtonsNumberBottom() * relative.Height - size.Height - result.Y;
					}
					else
					{
						float num4 = scrollBarClientRect.X + (float)GetButtonsNumberTop() * relative.Width + size.Width;
						result.Width = result.X - num4;
						result.X = num4;
					}
					break;
				case ScrollBarButtonType.LargeIncrement:
					if (flag)
					{
						float num3 = scrollBarClientRect.Y + (float)GetButtonsNumberTop() * relative.Height + size.Height;
						result.Height = result.Y - num3;
						result.Y = num3;
					}
					else
					{
						result.X = result.Right + size.Width;
						result.Width = scrollBarClientRect.Right - (float)GetButtonsNumberBottom() * relative.Width - size.Height - result.X;
					}
					break;
				}
				break;
			case ScrollBarButtonType.SmallDecrement:
				if (scrollBarButtonStyle == ScrollBarButtonStyles.All || scrollBarButtonStyle == ScrollBarButtonStyles.SmallScroll)
				{
					if (flag)
					{
						result.Y = scrollBarClientRect.Bottom - result.Height;
						break;
					}
					result.X = scrollBarClientRect.X + ((float)GetButtonsNumberTop() - 1f) * relative.Width;
					result.X += ((GetButtonsNumberTop() == 1) ? 0f : size.Width);
					break;
				}
				return RectangleF.Empty;
			case ScrollBarButtonType.SmallIncrement:
				if (scrollBarButtonStyle == ScrollBarButtonStyles.All || scrollBarButtonStyle == ScrollBarButtonStyles.SmallScroll)
				{
					if (flag)
					{
						result.Y = scrollBarClientRect.Y + ((float)GetButtonsNumberTop() - 1f) * relative.Height;
						result.Y += ((GetButtonsNumberTop() == 1) ? 0f : size.Height);
					}
					else
					{
						result.X = scrollBarClientRect.Right - result.Width;
					}
					break;
				}
				return RectangleF.Empty;
			case ScrollBarButtonType.ZoomReset:
				if (scrollBarButtonStyle == ScrollBarButtonStyles.All || scrollBarButtonStyle == ScrollBarButtonStyles.ResetZoom)
				{
					if (flag)
					{
						result.Y = scrollBarClientRect.Y;
					}
					else
					{
						result.X = scrollBarClientRect.X;
					}
					break;
				}
				return RectangleF.Empty;
			}
			return result;
		}

		internal RectangleF GetScrollBarRect()
		{
			float num = (float)GetScrollBarRelativeSize();
			RectangleF rectangleF = this.axis.PlotAreaPosition.ToRectangleF();
			if (!PositionInside)
			{
				rectangleF = this.axis.chartArea.Position.ToRectangleF();
				Axis[] axes = ChartArea.Axes;
				foreach (Axis axis in axes)
				{
					if (axis.ScrollBar.IsVisible() && !axis.ScrollBar.PositionInside)
					{
						float num2 = (float)axis.ScrollBar.GetScrollBarRelativeSize();
						switch (axis.AxisPosition)
						{
						case AxisPosition.Left:
							rectangleF.X += num2;
							rectangleF.Width -= num2;
							break;
						case AxisPosition.Right:
							rectangleF.Width -= num2;
							break;
						case AxisPosition.Bottom:
							rectangleF.Height -= num2;
							break;
						case AxisPosition.Top:
							rectangleF.Y += num2;
							rectangleF.Height -= num2;
							break;
						}
					}
				}
			}
			RectangleF empty = RectangleF.Empty;
			if (this.axis.PlotAreaPosition != null)
			{
				switch (this.axis.AxisPosition)
				{
				case AxisPosition.Left:
					empty.Y = rectangleF.Y;
					empty.Height = rectangleF.Height;
					empty.X = (PositionInside ? ((float)this.axis.GetAxisPosition(ignoreCrossing: true)) : rectangleF.X) - num;
					empty.Width = num;
					break;
				case AxisPosition.Right:
					empty.Y = rectangleF.Y;
					empty.Height = rectangleF.Height;
					empty.X = (PositionInside ? ((float)this.axis.GetAxisPosition(ignoreCrossing: true)) : rectangleF.Right);
					empty.Width = num;
					break;
				case AxisPosition.Bottom:
					empty.X = rectangleF.X;
					empty.Width = rectangleF.Width;
					empty.Y = (PositionInside ? ((float)this.axis.GetAxisPosition(ignoreCrossing: true)) : rectangleF.Bottom);
					empty.Height = num;
					break;
				case AxisPosition.Top:
					empty.X = rectangleF.X;
					empty.Width = rectangleF.Width;
					empty.Y = (PositionInside ? ((float)this.axis.GetAxisPosition(ignoreCrossing: true)) : rectangleF.Y) - num;
					empty.Height = num;
					break;
				}
			}
			return empty;
		}

		internal double GetScrollBarRelativeSize()
		{
			if (axis.chartArea.Area3DStyle.Enable3D || axis.chartArea.chartAreaIsCurcular)
			{
				return 0.0;
			}
			if (axis.AxisPosition == AxisPosition.Left || axis.AxisPosition == AxisPosition.Right)
			{
				return scrollBarSize * 100.0 / (double)(float)(axis.Common.Width - 1);
			}
			return scrollBarSize * 100.0 / (double)(float)(axis.Common.Height - 1);
		}

		private double GetDataViewPercentage()
		{
			double result = 100.0;
			if (axis != null && !double.IsNaN(axis.View.Position) && !double.IsNaN(axis.View.Size))
			{
				double intervalSize = axis.GetIntervalSize(axis.View.Position, axis.View.Size, axis.View.SizeType);
				double num = axis.minimum + axis.marginView;
				double num2 = axis.maximum - axis.marginView;
				if (axis.View.Position < num)
				{
					num = axis.View.Position;
				}
				if (axis.View.Position + intervalSize > num2)
				{
					num2 = axis.View.Position + intervalSize;
				}
				double num3 = Math.Abs(num - num2);
				if (intervalSize < num3)
				{
					result = intervalSize / (num3 / 100.0);
				}
			}
			return result;
		}

		private double GetDataViewPositionPercentage()
		{
			double result = 0.0;
			if (axis != null && !double.IsNaN(axis.View.Position) && !double.IsNaN(axis.View.Size))
			{
				double intervalSize = axis.GetIntervalSize(axis.View.Position, axis.View.Size, axis.View.SizeType);
				double num = axis.minimum + axis.marginView;
				double num2 = axis.maximum - axis.marginView;
				if (axis.View.Position < num)
				{
					num = axis.View.Position;
				}
				if (axis.View.Position + intervalSize > num2)
				{
					num2 = axis.View.Position + intervalSize;
				}
				double num3 = Math.Abs(num - num2);
				result = (axis.View.Position - num) / (num3 / 100.0);
			}
			return result;
		}

		private int GetButtonsNumberAll()
		{
			int num = 0;
			if ((scrollBarButtonStyle & ScrollBarButtonStyles.ResetZoom) == ScrollBarButtonStyles.ResetZoom)
			{
				num++;
			}
			if ((scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num += 2;
			}
			return num;
		}

		private int GetButtonsNumberTop()
		{
			int num = 0;
			if ((scrollBarButtonStyle & ScrollBarButtonStyles.ResetZoom) == ScrollBarButtonStyles.ResetZoom)
			{
				num++;
			}
			if ((scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num++;
			}
			return num;
		}

		private int GetButtonsNumberBottom()
		{
			int num = 0;
			if ((scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num++;
			}
			return num;
		}

		internal SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = relative.Width * (float)(axis.Common.Width - 1) / 100f;
			empty.Height = relative.Height * (float)(axis.Common.Height - 1) / 100f;
			return empty;
		}

		internal SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = size.Width * 100f / (float)(axis.Common.Width - 1);
			empty.Height = size.Height * 100f / (float)(axis.Common.Height - 1);
			return empty;
		}
	}
}
