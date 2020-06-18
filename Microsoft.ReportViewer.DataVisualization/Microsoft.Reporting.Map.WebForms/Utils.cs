using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Utils
	{
		private static ResourceManager resMng;

		internal const float GoldenRatio = 1.618034f;

		internal static ResourceManager ResourceStr => resMng;

		static Utils()
		{
			resMng = new ResourceManager(typeof(MapControl).Namespace + ".Resources.Strings", Assembly.GetExecutingAssembly());
		}

		public static string GetStack()
		{
			string text = "";
			StackTrace stackTrace = new StackTrace(fNeedFileInfo: true);
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				text += string.Format(CultureInfo.CurrentCulture, "{0} at [{1}] {2} \n", frame.GetMethod(), frame.GetFileName(), frame.GetFileLineNumber());
			}
			return text;
		}

		public static void StartTrace()
		{
			if (Trace.Listeners.Count == 1)
			{
				Trace.Listeners.Add(new TextWriterTraceListener(File.AppendText("c:\\TestFile.txt")));
				Trace.AutoFlush = true;
			}
		}

		public static void StopTrace()
		{
			if (Trace.Listeners.Count > 2)
			{
				for (int i = 2; i < Trace.Listeners.Count; i++)
				{
					Trace.Listeners[i].Close();
				}
			}
		}

		internal static IEnumerable<PointF> GetRectangePoints(RectangleF rectangle)
		{
			PointF point = rectangle.Location;
			yield return point;
			point.X += rectangle.Width;
			yield return point;
			point.Y += rectangle.Height;
			yield return point;
			point.X -= rectangle.Width;
			yield return point;
			point.Y -= rectangle.Height;
			yield return point;
		}

		internal static IEnumerable<PointF> DensifyPoints(IEnumerable<PointF> points, double step)
		{
			PointF prevPoint = new PointF(float.MaxValue, float.MaxValue);
			foreach (PointF point in points)
			{
				if (prevPoint.X != float.MaxValue && !prevPoint.Equals(point))
				{
					float num = point.X - prevPoint.X;
					float num2 = point.Y - prevPoint.Y;
					int val = (int)Math.Round(Math.Abs((double)num / step));
					int val2 = (int)Math.Round(Math.Abs((double)num2 / step));
					int stepCount = Math.Max(val, val2);
					if (stepCount > 0)
					{
						float stepX = num / (float)stepCount;
						float stepY = num2 / (float)stepCount;
						for (int i = 0; i < stepCount - 1; i++)
						{
							prevPoint.X += stepX;
							prevPoint.Y += stepY;
							yield return prevPoint;
						}
					}
				}
				yield return point;
				prevPoint = point;
			}
		}

		internal static float GetDistanceSqr(PointF pointA, PointF pointB)
		{
			double num = pointA.X - pointB.X;
			double num2 = pointA.Y - pointB.Y;
			return (float)(num * num + num2 * num2);
		}

		internal static double GetDistanceSqr(MapPoint pointA, MapPoint pointB)
		{
			double num = pointA.X - pointB.X;
			double num2 = pointA.Y - pointB.Y;
			return num * num + num2 * num2;
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

		internal static string GetImageCustomProperty(Image image, CustomPropertyTag customPropertyTag)
		{
			try
			{
				PropertyItem[] propertyItems = image.PropertyItems;
				foreach (PropertyItem propertyItem in propertyItems)
				{
					if (propertyItem.Id == (int)customPropertyTag && propertyItem.Value != null)
					{
						return Encoding.Unicode.GetString(propertyItem.Value);
					}
				}
			}
			catch
			{
				return string.Empty;
			}
			return string.Empty;
		}

		internal static void SetImageCustomProperty(Image image, CustomPropertyTag customPropertyTag, string text)
		{
			PropertyItem propertyItem = (PropertyItem)typeof(PropertyItem).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(null);
			propertyItem.Id = (int)customPropertyTag;
			propertyItem.Type = 1;
			propertyItem.Value = Encoding.Unicode.GetBytes(text);
			propertyItem.Len = propertyItem.Value.Length;
			image.SetPropertyItem(propertyItem);
		}
	}
}
