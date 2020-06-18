using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class Offset : MapObject, ICloneable
	{
		private MapPoint point = new MapPoint(0.0, 0.0);

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeOffset_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(0.0)]
		public double X
		{
			get
			{
				return point.X;
			}
			set
			{
				if (value > 100000000.0 || value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				point.X = value;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeOffset_Y")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(0.0)]
		public double Y
		{
			get
			{
				return point.Y;
			}
			set
			{
				if (value > 100000000.0 || value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				point.Y = value;
				ResetCachedPaths();
				InvalidateCachedBounds();
				InvalidateViewport();
			}
		}

		public Offset()
			: this(null)
		{
		}

		internal Offset(object parent)
			: base(parent)
		{
		}

		internal Offset(object parent, double x, double y)
			: this(parent)
		{
			point.X = x;
			point.Y = y;
		}

		public override string ToString()
		{
			return point.X.ToString(CultureInfo.CurrentCulture) + ", " + point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public override bool Equals(object obj)
		{
			if (obj is Offset)
			{
				Offset offset = (Offset)obj;
				if (offset.X == X && offset.Y == Y)
				{
					return true;
				}
				return false;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public object Clone()
		{
			return new Offset(Parent, X, Y);
		}

		internal void InvalidateCachedBounds()
		{
			if (Parent is Shape)
			{
				((Shape)Parent).InvalidateCachedBounds();
			}
			else if (Parent is Path)
			{
				((Path)Parent).InvalidateCachedBounds();
			}
			else if (Parent is Symbol)
			{
				((Symbol)Parent).InvalidateCachedBounds();
			}
		}

		internal void ResetCachedPaths()
		{
			if (Parent is Shape)
			{
				((Shape)Parent).ResetCachedPaths();
			}
			else if (Parent is Path)
			{
				((Path)Parent).ResetCachedPaths();
			}
			else if (Parent is Symbol)
			{
				((Symbol)Parent).ResetCachedPaths();
			}
		}
	}
}
