using System.Globalization;

namespace Microsoft.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer
{
	internal struct Size
	{
		internal enum Strategy
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		internal int Width
		{
			get;
			set;
		}

		internal int Height
		{
			get;
			set;
		}

		public static bool operator ==(Size a, Size b)
		{
			if (a.Height == b.Height)
			{
				return a.Width == b.Width;
			}
			return false;
		}

		public static bool operator !=(Size a, Size b)
		{
			if (a.Height == b.Height)
			{
				return a.Width != b.Width;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			if (obj is Size)
			{
				return (Size)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Height.GetHashCode() ^ Width.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0} wide by {1} tall.", Width, Height);
		}
	}
}
