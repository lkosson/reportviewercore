using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(Converter))]
	internal class ImageOrigin : ICloneable
	{
		internal class Converter : ExpandableObjectConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(string))
				{
					return true;
				}
				return base.CanConvertFrom(context, sourceType);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(ImageOrigin))
				{
					return true;
				}
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					return ((ImageOrigin)value).ToString();
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
					_ = (string)value;
					string[] array = ((string)value).Split(',');
					if (array.Length == 2)
					{
						return new ImageOrigin(notSet: false, int.Parse(array[0], CultureInfo.CurrentCulture), int.Parse(array[1], CultureInfo.CurrentCulture));
					}
					throw new ArgumentException(Utils.SRGetStr("ExceptionLocationFormat"));
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		private Point point = new Point(0, 0);

		private bool notSet = true;

		private bool defaultValues = true;

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeImageOrigin_X")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int X
		{
			get
			{
				return point.X;
			}
			set
			{
				point.X = value;
			}
		}

		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeY")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public int Y
		{
			get
			{
				return point.Y;
			}
			set
			{
				point.Y = value;
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryValues")]
		[SRDescription("DescriptionAttributeImageOrigin_NotSet")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public bool NotSet
		{
			get
			{
				return notSet;
			}
			set
			{
				notSet = value;
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

		public ImageOrigin()
		{
		}

		public ImageOrigin(bool notSet, int x, int y)
		{
			this.notSet = notSet;
			point.X = x;
			point.Y = y;
		}

		public override string ToString()
		{
			if (NotSet)
			{
				return "Not set";
			}
			return point.X.ToString(CultureInfo.CurrentCulture) + ", " + point.Y.ToString(CultureInfo.CurrentCulture);
		}

		public Point ToPoint()
		{
			return new Point(point.X, point.Y);
		}

		public object Clone()
		{
			return new ImageOrigin(notSet, point.X, point.Y);
		}
	}
}
