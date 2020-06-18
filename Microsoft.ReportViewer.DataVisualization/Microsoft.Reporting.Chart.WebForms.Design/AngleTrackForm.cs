using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace Microsoft.Reporting.Chart.WebForms.Design
{
	internal class AngleTrackForm : Control
	{
		private Container components;

		private int angleValue;

		private bool dragging;

		private EventHandler onValueChanged;

		private Region lastUpdatedRegion = new Region();

		private bool showText = true;

		private bool showLine = true;

		private bool showMarks = true;

		private int markStep = 15;

		private bool showOnly180Degrees = true;

		private int forceRoundingToDegees = 5;

		private int prevAngleValue;

		private const int size = 170;

		private const int offset = 22;

		[SRCategory("CategoryAttributeData")]
		[DefaultValue(0)]
		public int Angle
		{
			get
			{
				if (showOnly180Degrees && angleValue >= 270)
				{
					angleValue -= 360;
				}
				return angleValue;
			}
			set
			{
				if (value != angleValue)
				{
					OnValueChanged(EventArgs.Empty);
					angleValue = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAngleTrackFormEvent_ValueChanged")]
		public event EventHandler ValueChanged
		{
			add
			{
				onValueChanged = (EventHandler)Delegate.Combine(onValueChanged, value);
			}
			remove
			{
				onValueChanged = (EventHandler)Delegate.Remove(onValueChanged, value);
			}
		}

		public AngleTrackForm()
		{
			InitializeComponent();
			SetStyle(ControlStyles.Opaque, value: true);
			SetStyle(ControlStyles.ResizeRedraw, value: true);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			components.Dispose();
			if (lastUpdatedRegion != null)
			{
				lastUpdatedRegion.Dispose();
				lastUpdatedRegion = null;
			}
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			base.Size = new System.Drawing.Size(170 / ((!showOnly180Degrees) ? 1 : 2), 170);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			dragging = true;
			OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			dragging = false;
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			OnValueChanged(EventArgs.Empty);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!dragging)
			{
				return;
			}
			PointF pointF = new PointF(e.X - (showOnly180Degrees ? 22 : 85), e.Y - 85);
			if (pointF.X >= 0f)
			{
				double num = Math.Atan(pointF.Y / pointF.X);
				angleValue = (int)(num * 180.0 / Math.PI);
			}
			else if (!showOnly180Degrees)
			{
				double num = Math.Atan(pointF.Y / pointF.X);
				angleValue = (int)(num * 180.0 / Math.PI);
				angleValue = 180 + angleValue;
			}
			if (angleValue < 0)
			{
				angleValue = 360 + angleValue;
			}
			if (showOnly180Degrees)
			{
				if (angleValue > 90 && angleValue <= 180)
				{
					angleValue = 90;
				}
				else if (angleValue > 180 && angleValue < 270)
				{
					angleValue = 270;
				}
			}
			if (forceRoundingToDegees > 1)
			{
				angleValue = forceRoundingToDegees * (int)Math.Round((double)angleValue / (double)forceRoundingToDegees);
			}
			if (angleValue != prevAngleValue)
			{
				Invalidate(lastUpdatedRegion);
				if (showOnly180Degrees)
				{
					Invalidate(new Rectangle(0, 22, 22, 126));
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Brush brush = new SolidBrush(Color.White);
			Brush brush2 = new SolidBrush(Color.Black);
			Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
			e.Graphics.FillRectangle(brush, base.ClientRectangle);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			int num = showOnly180Degrees ? 22 : 85;
			PointF point = new PointF(num, 85f);
			if (showOnly180Degrees)
			{
				e.Graphics.DrawLine(Pens.Black, num, 22, num, 148);
			}
			e.Graphics.DrawArc(Pens.Black, num - 63, 22, 126, 126, -90, showOnly180Degrees ? 180 : 360);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(num - 63 + 1, 23, 124, 124, -90f, showOnly180Degrees ? 180 : 360);
			lastUpdatedRegion = new Region(graphicsPath);
			SizeF sizeF = e.Graphics.MeasureString("X", font);
			StringFormat stringFormat = new StringFormat();
			if (showOnly180Degrees)
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString("-90", font, brush2, 11f, 3f);
				e.Graphics.DrawString("90", font, brush2, 11f, 151f);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("0", font, brush2, new RectangleF(88f, 85f - sizeF.Height / 2f, 170f, 85f + sizeF.Height / 2f), stringFormat);
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString("-90", font, brush2, new RectangleF(0f, 3f, 170f, 22f), stringFormat);
				e.Graphics.DrawString("90", font, brush2, new RectangleF(0f, 151f, 170f, 170f), stringFormat);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("0", font, brush2, new RectangleF(151f, 85f - sizeF.Height / 2f, 170f, 85f + sizeF.Height / 2f), stringFormat);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("180", font, brush2, new RectangleF(0f, 85f - sizeF.Height / 2f, 170f, 85f + sizeF.Height / 2f), stringFormat);
			}
			Matrix matrix = new Matrix();
			if (showMarks)
			{
				if (showOnly180Degrees)
				{
					for (int i = 0; i <= 90; i += markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt(i, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
					for (int j = 270; j < 360; j += markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt(j, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
				}
				else
				{
					for (int k = 0; k < 360; k += markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt(k, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
				}
			}
			matrix.Dispose();
			matrix = new Matrix();
			matrix.RotateAt(angleValue, point);
			e.Graphics.Transform = matrix;
			prevAngleValue = angleValue;
			if (showLine)
			{
				e.Graphics.DrawLine(Pens.Black, num, 85, num + 63, 85);
			}
			if (showOnly180Degrees && angleValue >= 270)
			{
				angleValue -= 360;
			}
			if (showText)
			{
				e.Graphics.DrawString(angleValue.ToString(CultureInfo.InvariantCulture) + " degrees", font, brush2, point);
			}
			e.Graphics.Flush();
			brush.Dispose();
			brush2.Dispose();
			font.Dispose();
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (onValueChanged != null)
			{
				onValueChanged(this, e);
			}
		}
	}
}
