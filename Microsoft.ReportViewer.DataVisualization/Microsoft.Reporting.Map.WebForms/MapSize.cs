using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class MapSize : MapObject, ICloneable
	{
		private SizeF size = new SizeF(100f, 100f);

		private bool autoSize;

		private bool defaultValues;

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapSize_Width")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Width
		{
			get
			{
				return size.Width;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				float width = size.Width;
				size.Width = Math.Max(value, 0f);
				DefaultValues = false;
				Invalidate();
				if (size.Width != width)
				{
					(Parent as Panel)?.SizeChanged(this);
				}
			}
		}

		[SRCategory("CategoryAttribute_Values")]
		[SRDescription("DescriptionAttributeMapSize_Height")]
		[NotifyParentProperty(true)]
		[RefreshProperties(RefreshProperties.All)]
		public float Height
		{
			get
			{
				return size.Height;
			}
			set
			{
				if ((double)value > 100000000.0 || (double)value < -100000000.0)
				{
					throw new ArgumentOutOfRangeException(SR.out_of_range(-100000000.0, 100000000.0));
				}
				float height = size.Height;
				size.Height = Math.Max(value, 0f);
				DefaultValues = false;
				Invalidate();
				if (size.Height != height)
				{
					(Parent as Panel)?.SizeChanged(this);
				}
			}
		}

		internal bool AutoSize
		{
			get
			{
				return autoSize;
			}
			set
			{
				autoSize = value;
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

		private MapSize DefaultSize
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = Parent as IDefaultValueProvider;
				if (defaultValueProvider == null)
				{
					return new MapSize(null, 0f, 0f);
				}
				return (MapSize)defaultValueProvider.GetDefaultValue("Size", this);
			}
		}

		public MapSize()
			: this((object)null)
		{
		}

		internal MapSize(object parent)
			: base(parent)
		{
		}

		internal MapSize(object parent, float width, float height)
			: this(parent)
		{
			size.Width = Math.Max(width, 0f);
			size.Height = Math.Max(height, 0f);
		}

		internal MapSize(object parent, SizeF size)
			: this(parent)
		{
			this.size.Width = size.Width;
			this.size.Height = size.Height;
		}

		internal MapSize(MapSize size)
			: this(size.Parent, size.Width, size.Height)
		{
		}

		protected void ResetWidth()
		{
			Width = DefaultSize.Width;
		}

		protected bool ShouldSerializeWidth()
		{
			return true;
		}

		protected void ResetHeight()
		{
			Height = DefaultSize.Height;
		}

		protected bool ShouldSerializeHeight()
		{
			return true;
		}

		public override string ToString()
		{
			return size.Width.ToString(CultureInfo.CurrentCulture) + ", " + size.Height.ToString(CultureInfo.CurrentCulture);
		}

		public SizeF ToSize()
		{
			return new SizeF(size);
		}

		public static implicit operator SizeF(MapSize size)
		{
			return size.ToSize();
		}

		internal SizeF GetSizeF()
		{
			return size;
		}

		public object Clone()
		{
			return new MapSize(Parent, size);
		}
	}
}
