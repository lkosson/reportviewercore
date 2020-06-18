using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;

namespace Microsoft.Reporting.Map.WebForms.Design
{
	internal class DataWizardState
	{
		public bool FieldsChosen;

		public ShapeType DataType = ShapeType.Polygon;

		public DataTable Data;

		public bool MustCombine;

		public string NameField = "";

		public string SpatialField = "";

		public string ConnectionString = "";

		public string SafeConnectionString = "";

		public readonly StringCollection DataFields = new StringCollection();

		public readonly Hashtable CombineMode = new Hashtable();

		public readonly StringCollection AvailableLayers = new StringCollection();

		public string Layer = "";

		public string Category = "";

		public BasicMapElements MapElementsToImport;

		public static bool IsNumeric(Type type)
		{
			if (!type.IsPrimitive || !(type != typeof(char)))
			{
				return type == typeof(decimal);
			}
			return true;
		}

		public static bool HasUniqueValues(DataTable data, string columnName)
		{
			Hashtable hashtable = new Hashtable();
			foreach (DataRow row in data.Rows)
			{
				object obj = row[columnName];
				if (obj == null || Convert.IsDBNull(obj))
				{
					return false;
				}
				if (hashtable.ContainsKey(obj))
				{
					return false;
				}
				hashtable[obj] = new object();
			}
			return true;
		}
	}
}
