using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapLocation : MapObject, ICloneable
	{
		private PointF point = new PointF(0f, 0f);

		private bool docked;

		private bool defaultValues;

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapLocation_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float X
		{
			get
			{
				return point.X;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				float x = point.X;
				point.X = value;
				DefaultValues = false;
				Invalidate();
				if (point.X != x)
				{
					(Parent as Panel)?.LocationChanged(this);
				}
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapLocation_Y")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Y
		{
			get
			{
				return point.Y;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				float y = point.Y;
				point.Y = value;
				DefaultValues = false;
				Invalidate();
				if (point.Y != y)
				{
					(Parent as Panel)?.LocationChanged(this);
				}
			}
		}

		internal bool Docked
		{
			get
			{
				return docked;
			}
			set
			{
				docked = value;
			}
		}

		internal bool DefaultValues
		{
			get
			{
				return defaultValues;
			}
			set
			{
				defaultValues = value;
			}
		}

		private MapLocation DefaultLocation
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = Parent as IDefaultValueProvider;
				if (defaultValueProvider == null)
				{
					return new MapLocation(null, 0f, 0f);
				}
				return (MapLocation)defaultValueProvider.GetDefaultValue("Location", this);
			}
		}

		public MapLocation()
			: this((object)null)
		{
		}

		internal MapLocation(object parent)
			: base(parent)
		{
		}

		internal MapLocation(object parent, float x, float y)
			: this(parent)
		{
			point.X = x;
			point.Y = y;
		}

		internal MapLocation(MapLocation location)
			: this(location.Parent, location.X, location.Y)
		{
		}

		protected void ResetX()
		{
			X = DefaultLocation.X;
		}

		protected bool ShouldSerializeX()
		{
			return true;
		}

		protected void ResetY()
		{
			Y = DefaultLocation.Y;
		}

		protected bool ShouldSerializeY()
		{
			return true;
		}

		public override string ToString()
		{
			return point.X.ToString(CultureInfo.CurrentCulture) + ", " + point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public PointF ToPoint()
		{
			return new PointF(point.X, point.Y);
		}

		public static implicit operator PointF(MapLocation location)
		{
			return location.GetPointF();
		}

		public object Clone()
		{
			return new MapLocation(Parent, X, Y)
			{
				Docked = Docked
			};
		}

		internal PointF GetPointF()
		{
			return point;
		}
	}
}
