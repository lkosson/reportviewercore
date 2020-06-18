using System;

namespace Microsoft.Reporting.Map.WebForms.BingMaps
{
	internal class ImageryMetadataRequest
	{
		internal enum ImageryType
		{
			Aerial,
			AerialWithLabels,
			Birdseye,
			BirdseyeWithLabels,
			CanvasDark,
			CanvasLight,
			CanvasGray,
			Road,
			RoadOnDemand,
			OrdnanceSurvey,
			CollinsBart
		}

		private double orientation;

		private int zoomLevel;

		public string BingMapsKey
		{
			get;
			set;
		}

		public string Culture
		{
			get;
			set;
		}

		public ImageryType ImagerySet
		{
			get;
			set;
		}

		public bool IncludeImageryProviders
		{
			get;
			set;
		}

		public double Orientation
		{
			get
			{
				return orientation;
			}
			set
			{
				if (value < 0.0)
				{
					orientation = value % 360.0 + 360.0;
				}
				else if (value > 360.0)
				{
					orientation = value % 360.0;
				}
				else
				{
					orientation = value;
				}
			}
		}

		public int ZoomLevel
		{
			get
			{
				return zoomLevel;
			}
			set
			{
				if (value < 1)
				{
					zoomLevel = 1;
				}
				else if (value > 21)
				{
					zoomLevel = 21;
				}
				else
				{
					zoomLevel = value;
				}
			}
		}

		public bool UseHTTPS
		{
			get;
			set;
		}

		public string GetRequestUrl()
		{
			string str = "https://dev.virtualearth.net/REST/v1/Imagery/Metadata/";
			str += Enum.GetName(typeof(ImageryType), ImagerySet);
			str += "?";
			if (orientation != 0.0)
			{
				str = str + "&dir=" + orientation;
			}
			if (IncludeImageryProviders)
			{
				str += "&incl=ImageryProviders";
			}
			if (UseHTTPS)
			{
				str += "&uriScheme=https";
			}
			if (!string.IsNullOrEmpty(Culture))
			{
				str = str + "&c=" + Culture;
			}
			return str + "&key=" + BingMapsKey + "&clientApi=SSRS";
		}
	}
}
