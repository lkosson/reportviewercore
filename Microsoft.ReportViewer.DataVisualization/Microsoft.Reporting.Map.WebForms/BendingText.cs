using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class BendingText
	{
		protected char WHITESPACE_SUBSTITUTE = '-';

		public GraphicsPath CreatePath(Font font, PointF[] points, string text, int startIndex, int labelOffset)
		{
			GraphicsPath gBendedText;
			int num = BendText(out gBendedText, points, text, font, startIndex, backward: false, calculateOnly: true, labelOffset);
			if (num != -1)
			{
				if (points[num].X < points[startIndex].X)
				{
					BendText(out gBendedText, points, text, font, startIndex, backward: true, calculateOnly: false, labelOffset);
				}
				else
				{
					BendText(out gBendedText, points, text, font, startIndex, backward: false, calculateOnly: false, labelOffset);
				}
			}
			return gBendedText;
		}

		protected int BendText(out GraphicsPath gBendedText, PointF[] points, string text, Font font, int startIndex, bool backward, bool calculateOnly, int labelOffset)
		{
			gBendedText = null;
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				graphicsPath.AddString(text, font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), new StringFormat());
				double num = graphicsPath.GetBounds().Height + graphicsPath.GetBounds().Y;
				graphicsPath.Reset();
				double distanceFound = 0.0;
				double num2 = 0.0;
				double num3 = 0.0;
				double num4 = 0.0;
				int num5 = checked(startIndex + 1);
				bool flag = false;
				int num6 = backward ? (text.Length - 1) : 0;
				while (!flag)
				{
					char c = text[num6];
					bool flag2;
					if (char.IsWhiteSpace(c))
					{
						flag2 = true;
						c = WHITESPACE_SUBSTITUTE;
					}
					else
					{
						flag2 = false;
					}
					graphicsPath.AddString(c.ToString(CultureInfo.CurrentCulture), font.FontFamily, (int)font.Style, font.Size, new Point(0, 0), new StringFormat());
					double num7 = graphicsPath.GetBounds().Width + graphicsPath.GetBounds().X;
					double num8 = graphicsPath.GetBounds().X;
					double num9 = num / 2.0;
					if (backward)
					{
						Matrix matrix = new Matrix();
						matrix.RotateAt(180f, new PointF(graphicsPath.GetBounds().X + graphicsPath.GetBounds().Width / 2f, (float)(num / 2.0)));
						graphicsPath.Transform(matrix);
					}
					if (num5 < points.Length)
					{
						if (distanceFound != 0.0)
						{
							if (distanceFound < num7)
							{
								num5 = FindNearestPoint(points, new Point((int)num3, (int)num4), startIndex, num7, out distanceFound);
								num2 = Math.Asin(((double)points[num5].Y - num4) / distanceFound);
								if ((double)points[num5].X < num3)
								{
									num2 = Math.PI - num2;
								}
							}
						}
						else
						{
							num5 = FindNearestPoint(points, startIndex, num7, out distanceFound);
							num3 = points[startIndex].X;
							num4 = points[startIndex].Y;
							num2 = Math.Asin((double)(points[num5].Y - points[startIndex].Y) / distanceFound);
							if (points[num5].X < points[startIndex].X)
							{
								num2 = Math.PI - num2;
							}
						}
					}
					Matrix matrix2 = new Matrix();
					matrix2.RotateAt((float)(num2 * 180.0 / Math.PI), new PointF((float)num8, (float)num9));
					Matrix matrix3 = new Matrix();
					matrix3.Translate((float)(num3 - num8), (float)(num4 - num9));
					graphicsPath.Transform(matrix2);
					graphicsPath.Transform(matrix3);
					if (!calculateOnly && !flag2)
					{
						if (gBendedText == null)
						{
							gBendedText = new GraphicsPath();
						}
						gBendedText.AddPath(graphicsPath, connect: false);
					}
					graphicsPath.Reset();
					if (distanceFound < num7)
					{
						num5++;
					}
					startIndex = num5;
					num4 += num7 * Math.Sin(num2);
					num3 += num7 * Math.Cos(num2);
					distanceFound -= num7;
					if (backward)
					{
						if (num6 == 0)
						{
							flag = true;
						}
						else
						{
							num6--;
						}
					}
					else if (num6 == text.Length - 1)
					{
						flag = true;
					}
					else
					{
						num6++;
					}
				}
				if (num5 < points.Length && distanceFound >= 0.0)
				{
					return num5;
				}
				return -1;
			}
		}

		private int FindNearestPoint(PointF[] points, int startIndex, double distanceDesired, out double distanceFound)
		{
			return FindNearestPoint(points, points[startIndex], startIndex + 1, distanceDesired, out distanceFound);
		}

		private int FindNearestPoint(PointF[] points, PointF point, int startIndex, double distanceDesired, out double distanceFound)
		{
			int i = startIndex;
			int result = startIndex;
			double num = distanceDesired * distanceDesired;
			double num2 = 0.0;
			for (; i < points.Length; i++)
			{
				double num3 = (point.X - points[i].X) * (point.X - points[i].X) + (point.Y - points[i].Y) * (point.Y - points[i].Y);
				if (num3 > num && (Math.Abs(num3 - num) < Math.Abs(num2 - num) || num2 == 0.0))
				{
					num2 = num3;
					result = i;
				}
				if (num3 > num2 && num3 > num)
				{
					distanceFound = Math.Sqrt(num2);
					return result;
				}
			}
			distanceFound = Math.Sqrt(num2);
			return result;
		}
	}
}
