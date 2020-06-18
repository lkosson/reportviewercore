using System;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal static class Utils
	{
		internal const float GoldenRatio = 1.618034f;

		internal static string SRGetStr(string key, params object[] p)
		{
			return string.Format(CultureInfo.CurrentCulture, SR.Keys.GetString(key), p);
		}

		internal static double Round(double value, int precision)
		{
			if (precision >= 0)
			{
				return Math.Round(value, precision);
			}
			precision = -precision;
			double num = Math.Pow(10.0, precision);
			return Math.Round(value / num, 0) * num;
		}

		internal static float Deg2Rad(float angleInDegree)
		{
			return (float)((double)Math.Abs(angleInDegree) * Math.PI / 180.0);
		}

		internal static float Rad2Deg(float angleInRadians)
		{
			return (float)((double)Math.Abs(angleInRadians) / Math.PI * 180.0);
		}

		internal static float NormalizeAngle(float angle)
		{
			if (angle < 0f)
			{
				return 360f - angle;
			}
			if (angle > 360f)
			{
				return angle - 360f;
			}
			return angle;
		}

		internal static float GetContactPointOffset(SizeF size, float angle)
		{
			angle = NormalizeAngle(Math.Abs(angle));
			if (angle >= 180f)
			{
				angle %= 180f;
			}
			if (angle % 180f > 90f)
			{
				angle = 180f - angle % 180f;
			}
			float num = Rad2Deg((float)Math.Atan(size.Width / size.Height));
			float num2 = 0f;
			if (angle >= num)
			{
				return (float)((double)(size.Width / 2f) / Math.Sin(Deg2Rad(angle)));
			}
			return (float)((double)(size.Height / 2f) / Math.Cos(Deg2Rad(angle)));
		}

		internal static float ToGDIAngle(float angle)
		{
			angle += 90f;
			if (!(angle > 360f))
			{
				return angle;
			}
			return angle - 360f;
		}

		internal static RectangleF NormalizeRectangle(RectangleF boundRect, SizeF insetSize, bool resizeResult)
		{
			RectangleF result = boundRect;
			if (resizeResult)
			{
				float num = insetSize.Width / insetSize.Height;
				if (boundRect.Size.Width / boundRect.Size.Height > num)
				{
					result.Height = boundRect.Size.Height;
					result.Width = result.Height * num;
					result.X += (boundRect.Size.Width - result.Width) / 2f;
				}
				else
				{
					result.Width = boundRect.Size.Width;
					result.Height = result.Width / num;
					result.Y += (boundRect.Size.Height - result.Height) / 2f;
				}
			}
			else
			{
				result.Width = insetSize.Width;
				result.Height = insetSize.Height;
				result.X += (boundRect.Size.Width - result.Width) / 2f;
				result.Y += (boundRect.Size.Height - result.Height) / 2f;
			}
			return result;
		}
	}
}
