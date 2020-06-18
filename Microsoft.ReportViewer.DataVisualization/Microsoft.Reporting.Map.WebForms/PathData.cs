using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathData
	{
		private MapPoint[] points;

		private PathSegment[] segments;

		private MapPoint minimumExtent;

		private MapPoint maximumExtent;

		private int largestSegmentIndex;

		public MapPoint[] Points
		{
			get
			{
				return points;
			}
			set
			{
				points = value;
			}
		}

		public PathSegment[] Segments
		{
			get
			{
				return segments;
			}
			set
			{
				segments = value;
			}
		}

		public MapPoint MinimumExtent
		{
			get
			{
				return minimumExtent;
			}
			set
			{
				minimumExtent = value;
			}
		}

		public MapPoint MaximumExtent
		{
			get
			{
				return maximumExtent;
			}
			set
			{
				maximumExtent = value;
			}
		}

		internal int LargestSegmentIndex => largestSegmentIndex;

		internal bool IsEmpty
		{
			get
			{
				if (Segments != null && Segments.Length != 0 && Points != null)
				{
					return Points.Length == 0;
				}
				return true;
			}
		}

		internal void UpdateStoredParameters()
		{
			if (Segments == null)
			{
				return;
			}
			MapPoint mapPoint = new MapPoint(double.PositiveInfinity, double.PositiveInfinity);
			MapPoint mapPoint2 = new MapPoint(double.NegativeInfinity, double.NegativeInfinity);
			int num = 0;
			for (int i = 0; i < Segments.Length; i++)
			{
				MapPoint mapPoint3 = new MapPoint(double.PositiveInfinity, double.PositiveInfinity);
				MapPoint mapPoint4 = new MapPoint(double.NegativeInfinity, double.NegativeInfinity);
				double num2 = 0.0;
				for (int j = 0; j < Segments[i].Length; j++)
				{
					mapPoint3.X = Math.Min(mapPoint3.X, Points[num].X);
					mapPoint3.Y = Math.Min(mapPoint3.Y, Points[num].Y);
					mapPoint4.X = Math.Max(mapPoint4.X, Points[num].X);
					mapPoint4.Y = Math.Max(mapPoint4.Y, Points[num].Y);
					if (j > 0)
					{
						double num3 = Points[num].X - Points[num - 1].X;
						double num4 = Points[num].Y - Points[num - 1].Y;
						num2 += Math.Sqrt(num3 * num3 + num4 * num4);
					}
					num++;
				}
				Segments[i].SegmentLength = num2;
				Segments[i].MinimumExtent = mapPoint3;
				Segments[i].MaximumExtent = mapPoint4;
				mapPoint.X = Math.Min(mapPoint.X, mapPoint3.X);
				mapPoint.Y = Math.Min(mapPoint.Y, mapPoint3.Y);
				mapPoint2.X = Math.Max(mapPoint2.X, mapPoint4.X);
				mapPoint2.Y = Math.Max(mapPoint2.Y, mapPoint4.Y);
			}
			MinimumExtent = mapPoint;
			MaximumExtent = mapPoint2;
			UpdateLargestSegmentIndex();
		}

		private void UpdateLargestSegmentIndex()
		{
			double num = 0.0;
			largestSegmentIndex = 0;
			for (int i = 0; i < Segments.Length; i++)
			{
				double num2 = Math.Abs(Segments[i].SegmentLength);
				if (num < num2)
				{
					num = num2;
					largestSegmentIndex = i;
				}
			}
		}

		internal void LoadFromStream(Stream stream)
		{
			byte[] array = new byte[Marshal.SizeOf(typeof(byte))];
			byte[] array2 = new byte[Marshal.SizeOf(typeof(int))];
			byte[] array3 = new byte[Marshal.SizeOf(typeof(double))];
			stream.Read(array2, 0, array2.Length);
			int num = BitConverter.ToInt32(array2, 0);
			Segments = new PathSegment[num];
			for (int i = 0; i < num; i++)
			{
				stream.Read(array, 0, array.Length);
				Segments[i].Type = (SegmentType)array[0];
				stream.Read(array2, 0, array2.Length);
				Segments[i].Length = BitConverter.ToInt32(array2, 0);
			}
			stream.Read(array2, 0, array2.Length);
			int num2 = BitConverter.ToInt32(array2, 0);
			Points = new MapPoint[num2];
			for (int j = 0; j < num2; j++)
			{
				stream.Read(array3, 0, array3.Length);
				Points[j].X = BitConverter.ToDouble(array3, 0);
				stream.Read(array3, 0, array3.Length);
				Points[j].Y = BitConverter.ToDouble(array3, 0);
			}
		}

		internal void SaveToStream(Stream stream)
		{
			if (Segments != null && Points != null)
			{
				byte[] bytes = BitConverter.GetBytes(Segments.Length);
				stream.Write(bytes, 0, bytes.Length);
				PathSegment[] array = Segments;
				for (int i = 0; i < array.Length; i++)
				{
					PathSegment pathSegment = array[i];
					bytes = new byte[Marshal.SizeOf(typeof(byte))];
					bytes[0] = (byte)pathSegment.Type;
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(pathSegment.Length);
					stream.Write(bytes, 0, bytes.Length);
				}
				bytes = BitConverter.GetBytes(Points.Length);
				stream.Write(bytes, 0, bytes.Length);
				MapPoint[] array2 = Points;
				for (int i = 0; i < array2.Length; i++)
				{
					MapPoint mapPoint = array2[i];
					bytes = BitConverter.GetBytes(mapPoint.X);
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(mapPoint.Y);
					stream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		internal static string PathDataToString(PathData pathData)
		{
			MemoryStream memoryStream = new MemoryStream();
			pathData.SaveToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
		}

		internal static PathData PathDataFromString(string data)
		{
			byte[] array = new byte[1000];
			MemoryStream memoryStream = new MemoryStream();
			XmlTextReader xmlTextReader = new XmlTextReader(new StringReader("<base64>" + data + "</base64>"));
			xmlTextReader.Read();
			int num = 0;
			while ((num = xmlTextReader.ReadBase64(array, 0, 1000)) > 0)
			{
				memoryStream.Write(array, 0, num);
			}
			xmlTextReader.Read();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			PathData pathData = new PathData();
			pathData.LoadFromStream(memoryStream);
			xmlTextReader.Close();
			memoryStream.Close();
			return pathData;
		}
	}
}
