using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.Reporting.Map.WebForms
{
	[DataContract]
	public class Coordinate
	{
		private double _latitude;

		private double _longitude;

		[DataMember(Name = "lat", EmitDefaultValue = false)]
		public double Latitude
		{
			get
			{
				return _latitude;
			}
			set
			{
				if (!double.IsNaN(value) && value <= 90.0 && value >= -90.0)
				{
					_latitude = Math.Round(value, 5, MidpointRounding.AwayFromZero);
				}
			}
		}

		[DataMember(Name = "lon", EmitDefaultValue = false)]
		public double Longitude
		{
			get
			{
				return _longitude;
			}
			set
			{
				if (!double.IsNaN(value) && value <= 180.0 && value >= -180.0)
				{
					_longitude = Math.Round(value, 5, MidpointRounding.AwayFromZero);
				}
			}
		}

		public Coordinate()
		{
		}

		public Coordinate(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0:0.#####},{1:0.#####}", Latitude, Longitude);
		}
	}
}
