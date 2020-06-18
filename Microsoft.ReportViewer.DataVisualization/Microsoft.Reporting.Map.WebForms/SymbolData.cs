using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolData
	{
		private MapPoint[] points = new MapPoint[1];

		private MapPoint minimumExtent;

		private MapPoint maximumExtent;

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

		internal bool IsEmpty
		{
			get
			{
				if (Points != null)
				{
					return Points.Length == 0;
				}
				return true;
			}
		}

		internal void UpdateStoredParameters()
		{
			MapPoint mapPoint = new MapPoint(double.PositiveInfinity, double.PositiveInfinity);
			MapPoint mapPoint2 = new MapPoint(double.NegativeInfinity, double.NegativeInfinity);
			MapPoint[] array = Points;
			for (int i = 0; i < array.Length; i++)
			{
				MapPoint mapPoint3 = array[i];
				mapPoint.X = Math.Min(mapPoint.X, mapPoint3.X);
				mapPoint.Y = Math.Min(mapPoint.Y, mapPoint3.Y);
				mapPoint2.X = Math.Max(mapPoint2.X, mapPoint3.X);
				mapPoint2.Y = Math.Max(mapPoint2.Y, mapPoint3.Y);
			}
			MinimumExtent = mapPoint;
			MaximumExtent = mapPoint2;
		}

		internal void LoadFromStream(Stream stream)
		{
			byte[] array = new byte[Marshal.SizeOf(typeof(int))];
			byte[] array2 = new byte[Marshal.SizeOf(typeof(double))];
			stream.Read(array, 0, array.Length);
			int num = BitConverter.ToInt32(array, 0);
			Points = new MapPoint[num];
			for (int i = 0; i < num; i++)
			{
				stream.Read(array2, 0, array2.Length);
				Points[i].X = BitConverter.ToDouble(array2, 0);
				stream.Read(array2, 0, array2.Length);
				Points[i].Y = BitConverter.ToDouble(array2, 0);
			}
		}

		internal void SaveToStream(Stream stream)
		{
			if (Points != null)
			{
				byte[] bytes = BitConverter.GetBytes(Points.Length);
				stream.Write(bytes, 0, bytes.Length);
				MapPoint[] array = Points;
				for (int i = 0; i < array.Length; i++)
				{
					MapPoint mapPoint = array[i];
					bytes = BitConverter.GetBytes(mapPoint.X);
					stream.Write(bytes, 0, bytes.Length);
					bytes = BitConverter.GetBytes(mapPoint.Y);
					stream.Write(bytes, 0, bytes.Length);
				}
			}
		}

		internal static string SymbolDataToString(SymbolData symbolData)
		{
			MemoryStream memoryStream = new MemoryStream();
			symbolData.SaveToStream(memoryStream);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			byte[] inArray = memoryStream.ToArray();
			memoryStream.Close();
			return Convert.ToBase64String(inArray);
		}

		internal static SymbolData SymbolDataFromString(string data)
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
			SymbolData symbolData = new SymbolData();
			symbolData.LoadFromStream(memoryStream);
			xmlTextReader.Close();
			memoryStream.Close();
			return symbolData;
		}
	}
}
