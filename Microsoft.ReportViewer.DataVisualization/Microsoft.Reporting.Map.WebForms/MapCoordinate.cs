using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(MapCoordinateConverter))]
	internal class MapCoordinate
	{
		private double value;

		public MapCoordinate()
		{
		}

		public MapCoordinate(double value)
		{
			this.value = value;
		}

		public MapCoordinate(string value)
		{
			this.value = Parse(value);
		}

		public static double Parse(string stringValue)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			MatchCollection matchCollection = new Regex("[-+]?[0-9]*\\.?[0-9]+").Matches(stringValue);
			if (matchCollection.Count == 0)
			{
				throw new ArgumentException(SR.ExceptionInvalidCoordinate(stringValue));
			}
			if (matchCollection.Count > 0)
			{
				num = double.Parse(matchCollection[0].Value, CultureInfo.InvariantCulture);
			}
			if (matchCollection.Count > 1)
			{
				num2 = double.Parse(matchCollection[1].Value, CultureInfo.InvariantCulture);
			}
			if (matchCollection.Count > 2)
			{
				num3 = double.Parse(matchCollection[2].Value, CultureInfo.InvariantCulture);
			}
			double num4 = 1.0;
			Match match = new Regex("[NnSsEeWw]\\s*\\z").Match(stringValue);
			if (match.Success)
			{
				string a = match.Value.ToUpper(CultureInfo.CurrentCulture);
				if (a == "S" || a == "W")
				{
					num4 = -1.0;
				}
			}
			return (num + num2 / 60.0 + num3 / 3600.0) * num4;
		}

		public static implicit operator double(MapCoordinate mapCoordinate)
		{
			return mapCoordinate.ToDouble();
		}

		public static implicit operator MapCoordinate(double value)
		{
			return new MapCoordinate(value);
		}

		public static implicit operator string(MapCoordinate mapCoordinate)
		{
			return mapCoordinate.ToString();
		}

		public static implicit operator MapCoordinate(string value)
		{
			return new MapCoordinate(value);
		}

		public double ToDouble()
		{
			return value;
		}

		public override string ToString()
		{
			return value.ToString(CultureInfo.CurrentCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is MapCoordinate)
			{
				return ((MapCoordinate)obj).ToDouble() == value;
			}
			if (obj is string)
			{
				return Parse((string)obj) == value;
			}
			if (obj is double)
			{
				return (double)obj == value;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
