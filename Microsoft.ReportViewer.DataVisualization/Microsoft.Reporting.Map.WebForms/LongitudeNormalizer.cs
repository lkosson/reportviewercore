using Microsoft.SqlServer.Types;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class LongitudeNormalizer : IGeographySink
	{
		private const double MaxLongitude = 179.99999999;

		private SqlGeographyBuilder Builder;

		public SqlGeography Result => Builder.ConstructedGeography;

		private double GetNormalizedLatitude(double longitude)
		{
			double num = longitude;
			if (longitude < -180.0)
			{
				num = (longitude - 180.0) % 360.0 + 180.0;
			}
			else if (longitude > 180.0)
			{
				num = (longitude + 180.0) % 360.0 - 180.0;
			}
			if (num > 179.99999999)
			{
				num = 179.99999999;
			}
			else if (num < -179.99999999)
			{
				num = -179.99999999;
			}
			return num;
		}

		public LongitudeNormalizer()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			Builder = (SqlGeographyBuilder)(object)new SqlGeographyBuilder();
		}

		public void AddLine(double latitude, double longitude, double? z, double? m)
		{
			Builder.AddLine(latitude, GetNormalizedLatitude(longitude));
		}

		public void BeginFigure(double latitude, double longitude, double? z, double? m)
		{
			Builder.BeginFigure(latitude, GetNormalizedLatitude(longitude));
		}

		public void BeginGeography(OpenGisGeographyType type)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Builder.BeginGeography(type);
		}

		public void EndFigure()
		{
			Builder.EndFigure();
		}

		public void EndGeography()
		{
			Builder.EndGeography();
		}

		public void SetSrid(int srid)
		{
			Builder.SetSrid(srid);
		}
	}
}
