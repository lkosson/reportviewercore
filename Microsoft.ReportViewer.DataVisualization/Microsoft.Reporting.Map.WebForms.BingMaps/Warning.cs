using System;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	[DataContract]
	public class Warning
	{
		[DataMember(Name = "origin", EmitDefaultValue = false)]
		public string Origin
		{
			get;
			set;
		}

		public Coordinate OriginCoordinate
		{
			get
			{
				if (string.IsNullOrEmpty(Origin))
				{
					return null;
				}
				string[] array = Origin.Split(new char[1]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2 && double.TryParse(array[0], out double result) && double.TryParse(array[1], out double result2))
				{
					return new Coordinate(result, result2);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					Origin = string.Empty;
				}
				else
				{
					Origin = $"{value.Latitude},{value.Longitude}";
				}
			}
		}

		[DataMember(Name = "severity", EmitDefaultValue = false)]
		public string Severity
		{
			get;
			set;
		}

		[DataMember(Name = "text", EmitDefaultValue = false)]
		public string Text
		{
			get;
			set;
		}

		[DataMember(Name = "to", EmitDefaultValue = false)]
		public string To
		{
			get;
			set;
		}

		public Coordinate ToCoordinate
		{
			get
			{
				if (string.IsNullOrEmpty(To))
				{
					return null;
				}
				string[] array = To.Split(new char[1]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 2 && double.TryParse(array[0], out double result) && double.TryParse(array[1], out double result2))
				{
					return new Coordinate(result, result2);
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					To = string.Empty;
				}
				else
				{
					To = $"{value.Latitude},{value.Longitude}";
				}
			}
		}

		[DataMember(Name = "warningType", EmitDefaultValue = false)]
		public string WarningType
		{
			get;
			set;
		}
	}
}
