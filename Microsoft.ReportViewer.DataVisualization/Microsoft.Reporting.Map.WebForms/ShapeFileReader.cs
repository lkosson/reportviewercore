using System;
using System.Collections;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapeFileReader
	{
		private string fileName = string.Empty;

		private Stream shpStream;

		private Stream dbfStream;

		private int fileCode = 9994;

		private int fileLength;

		private int version = 1000;

		private ShapeType shapeType;

		private double xMin;

		private double xMax;

		private double yMin;

		private double yMax;

		private double zMin;

		private double zMax;

		private double mMin;

		private double mMax;

		private ArrayList points = new ArrayList();

		private ArrayList multiPoints = new ArrayList();

		private ArrayList polyLines = new ArrayList();

		private ArrayList polygons = new ArrayList();

		private DataTable table;

		public string FileName
		{
			get
			{
				return fileName;
			}
			set
			{
				fileName = value;
			}
		}

		public Stream ShpStream
		{
			get
			{
				return shpStream;
			}
			set
			{
				shpStream = value;
			}
		}

		public Stream DbfStream
		{
			get
			{
				return dbfStream;
			}
			set
			{
				dbfStream = value;
			}
		}

		public int FileCode
		{
			get
			{
				return fileCode;
			}
			set
			{
				fileCode = value;
			}
		}

		public int FileLength
		{
			get
			{
				return fileLength;
			}
			set
			{
				fileLength = value;
			}
		}

		public int Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public ShapeType ShapeType
		{
			get
			{
				return shapeType;
			}
			set
			{
				shapeType = value;
			}
		}

		public double XMin
		{
			get
			{
				return xMin;
			}
			set
			{
				xMin = value;
			}
		}

		public double XMax
		{
			get
			{
				return xMax;
			}
			set
			{
				xMax = value;
			}
		}

		public double YMin
		{
			get
			{
				return yMin;
			}
			set
			{
				yMin = value;
			}
		}

		public double YMax
		{
			get
			{
				return yMax;
			}
			set
			{
				yMax = value;
			}
		}

		public double ZMin
		{
			get
			{
				return zMin;
			}
			set
			{
				zMin = value;
			}
		}

		public double ZMax
		{
			get
			{
				return zMax;
			}
			set
			{
				zMax = value;
			}
		}

		public double MMin
		{
			get
			{
				return mMin;
			}
			set
			{
				mMin = value;
			}
		}

		public double MMax
		{
			get
			{
				return mMax;
			}
			set
			{
				mMax = value;
			}
		}

		public ArrayList Points => points;

		public ArrayList MultiPoints => multiPoints;

		public ArrayList PolyLines => polyLines;

		public ArrayList Polygons => polygons;

		public DataTable Table
		{
			get
			{
				if (table == null)
				{
					table = new DataTable();
					table.Locale = CultureInfo.CurrentCulture;
				}
				return table;
			}
			set
			{
				table = value;
			}
		}

		public static SqlBytes File2SqlBytes(string fileName)
		{
			FileStream fileStream = null;
			byte[] array = null;
			SqlBytes @null = SqlBytes.Null;
			if (!File.Exists(fileName))
			{
				return @null;
			}
			using (fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read))
			{
				if (fileStream.Length == 0L)
				{
					return @null;
				}
				if (fileStream.Length > int.MaxValue)
				{
					throw new InvalidDataException(SR.FileToLarge);
				}
				int num = (int)fileStream.Length;
				array = new byte[num];
				if (array == null)
				{
					throw new OutOfMemoryException(SR.UnableToAllocateMemoryForSqlBinary);
				}
				if (num != fileStream.Read(array, 0, num))
				{
					throw new IOException(SR.UnableToReadWholeFileToSqlBinary);
				}
			}
			return new SqlBytes(array);
		}

		public void LoadHeader()
		{
			bool flag = false;
			Stream stream = ShpStream;
			if (stream == null)
			{
				stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
				flag = true;
			}
			BinaryReader binaryReader = new BinaryReader(stream);
			ReadHeader(binaryReader);
			binaryReader.Close();
			if (flag)
			{
				stream.Close();
			}
		}

		public void Load()
		{
			if (ShpStream != null)
			{
				BinaryReader binaryReader = new BinaryReader(ShpStream);
				try
				{
					ReadHeader(binaryReader);
					ReadShapes(binaryReader);
				}
				finally
				{
					binaryReader.Close();
				}
				if (DbfStream != null)
				{
					DBF dBF = new DBF(new SqlBytes(dbfStream));
					Table = dBF.GetDataTable();
				}
				return;
			}
			using (FileStream input = new FileStream(FileName, FileMode.Open, FileAccess.Read))
			{
				BinaryReader binaryReader2 = new BinaryReader(input);
				try
				{
					ReadHeader(binaryReader2);
					ReadShapes(binaryReader2);
				}
				finally
				{
					binaryReader2.Close();
				}
			}
			string path = FileName.Substring(0, FileName.LastIndexOf('.')) + ".dbf";
			if (File.Exists(path))
			{
				DBF dBF2 = new DBF(File2SqlBytes(path));
				Table = dBF2.GetDataTable();
			}
		}

		internal void ReadHeader(BinaryReader reader)
		{
			FileCode = SwapBytes(reader.ReadInt32());
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			reader.ReadInt32();
			FileLength = SwapBytes(reader.ReadInt32());
			Version = reader.ReadInt32();
			ShapeType = (ShapeType)reader.ReadInt32();
			XMin = reader.ReadDouble();
			YMin = reader.ReadDouble();
			XMax = reader.ReadDouble();
			YMax = reader.ReadDouble();
			ZMin = reader.ReadDouble();
			ZMax = reader.ReadDouble();
			MMin = reader.ReadDouble();
			MMax = reader.ReadDouble();
		}

		internal void ReadShapes(BinaryReader reader)
		{
			while (reader.BaseStream.Length != reader.BaseStream.Position)
			{
				SwapBytes(reader.ReadInt32());
				SwapBytes(reader.ReadInt32());
				switch (reader.ReadInt32())
				{
				case 1:
				{
					ShapePoint shapePoint = new ShapePoint();
					shapePoint.Read(reader);
					Points.Add(shapePoint);
					break;
				}
				case 8:
				{
					MultiPoint multiPoint = new MultiPoint();
					multiPoint.Read(reader);
					MultiPoints.Add(multiPoint);
					break;
				}
				case 3:
				{
					PolyLine polyLine2 = new PolyLine();
					polyLine2.Read(reader);
					PolyLines.Add(polyLine2);
					break;
				}
				case 5:
				{
					PolyLine polyLine = new PolyLine();
					polyLine.Read(reader);
					Polygons.Add(polyLine);
					break;
				}
				}
			}
		}

		public static BasicMapElements? DetermineMapElementsFromShapeFile(string fileName, out ShapeType? unsupportedShapeType)
		{
			return new ShapeFileReader
			{
				FileName = fileName
			}.DetermineMapElements(out unsupportedShapeType);
		}

		public static BasicMapElements? DetermineMapElementsFromShapeFile(Stream shpStream, out ShapeType? unsupportedShapeType)
		{
			return new ShapeFileReader
			{
				ShpStream = shpStream
			}.DetermineMapElements(out unsupportedShapeType);
		}

		private BasicMapElements? DetermineMapElements(out ShapeType? unsupportedShapeType)
		{
			LoadHeader();
			unsupportedShapeType = null;
			if (ShapeType == ShapeType.Polygon)
			{
				return BasicMapElements.Shapes;
			}
			if (ShapeType == ShapeType.PolyLine)
			{
				return BasicMapElements.Paths;
			}
			if (ShapeType == ShapeType.Point || ShapeType == ShapeType.MultiPoint)
			{
				return BasicMapElements.Symbols;
			}
			unsupportedShapeType = ShapeType;
			return null;
		}

		public static string GetShortFileName(string fullPath, string fileName)
		{
			string result = "";
			try
			{
				Type type = Assembly.LoadWithPartialName("System.Web").GetType("System.Web.Util.FindFileData");
				object[] array = new object[2]
				{
					fullPath,
					null
				};
				type.InvokeMember("FindFile", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, null, array, CultureInfo.InvariantCulture);
				if (type.GetField("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField) != null)
				{
					result = (string)type.InvokeMember("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, array[1], null, CultureInfo.InvariantCulture);
					return result;
				}
				if (type.GetProperty("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty) != null)
				{
					result = (string)type.InvokeMember("FileNameShort", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetProperty, null, array[1], null, CultureInfo.InvariantCulture);
					return result;
				}
				result = fileName.Substring(0, 6) + "~1";
				return result;
			}
			catch
			{
				return result;
			}
		}

		public static DataTable ReadDBFThroughOLEDB(string fullPath)
		{
			int num = fullPath.LastIndexOf("\\", StringComparison.Ordinal);
			string str = fullPath.Substring(0, num + 1);
			string text = fullPath.Substring(num + 1);
			text = text.Substring(0, text.LastIndexOf('.'));
			if (text.Length > 8)
			{
				text = GetShortFileName(fullPath, text);
				text = text.Substring(0, text.LastIndexOf('.'));
			}
			string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + str + "; Extended Properties=dBase 5.0;";
			string sql = "SELECT * FROM [" + text + "#DBF]";
			return GetDataSet(connectionString, sql).Tables[0];
		}

		public static DataSet GetDataSet(string connectionString, string sql)
		{
			using (OleDbConnection oleDbConnection = new OleDbConnection(connectionString))
			{
				oleDbConnection.Open();
				using (OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter())
				{
					using (OleDbCommand oleDbCommand = new OleDbCommand(sql, oleDbConnection))
					{
						oleDbDataAdapter.SelectCommand = oleDbCommand;
						DataSet dataSet = new DataSet();
						dataSet.Locale = CultureInfo.CurrentCulture;
						oleDbDataAdapter.Fill(dataSet);
						oleDbCommand.Parameters.Clear();
						return dataSet;
					}
				}
			}
		}

		public static bool IsDataSchemaIdentical(DataTable table1, DataTable table2)
		{
			foreach (DataColumn column in table1.Columns)
			{
				if (!table2.Columns.Contains(column.ColumnName) || table2.Columns[column.ColumnName].DataType != column.DataType)
				{
					return false;
				}
			}
			foreach (DataColumn column2 in table2.Columns)
			{
				if (!table1.Columns.Contains(column2.ColumnName) || table1.Columns[column2.ColumnName].DataType != column2.DataType)
				{
					return false;
				}
			}
			return true;
		}

		internal int SwapBytes(int inputValue)
		{
			long num = (inputValue & 4278190080u) >> 24;
			long num2 = inputValue & 0xFF0000;
			num2 >>= 8;
			long num3 = inputValue & 0xFF00;
			num3 <<= 8;
			long num4 = inputValue & 0xFF;
			num4 <<= 24;
			return (int)(num | num2 | num3 | num4);
		}
	}
}
