using System.ComponentModel;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	[TypeConverter(typeof(PanelMarginsConverter))]
	internal class PanelMargins
	{
		private Panel owner;

		private int top;

		private int bottom;

		private int right;

		private int left;

		private bool all = true;

		[Browsable(false)]
		internal Panel Owner
		{
			get
			{
				return owner;
			}
			set
			{
				owner = value;
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Top")]
		[RefreshProperties(RefreshProperties.All)]
		public int Top
		{
			get
			{
				return top;
			}
			set
			{
				if (top != value)
				{
					top = value;
					SyncPropeties();
					NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Bottom")]
		[RefreshProperties(RefreshProperties.All)]
		public int Bottom
		{
			get
			{
				return bottom;
			}
			set
			{
				if (bottom != value)
				{
					bottom = value;
					SyncPropeties();
					NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Right")]
		[RefreshProperties(RefreshProperties.All)]
		public int Right
		{
			get
			{
				return right;
			}
			set
			{
				if (right != value)
				{
					right = value;
					SyncPropeties();
					NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_Left")]
		[RefreshProperties(RefreshProperties.All)]
		public int Left
		{
			get
			{
				return left;
			}
			set
			{
				if (left != value)
				{
					left = value;
					SyncPropeties();
					NotifyOwner();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributePanelMargins_All")]
		[RefreshProperties(RefreshProperties.All)]
		public int All
		{
			get
			{
				if (!all)
				{
					return -1;
				}
				return left;
			}
			set
			{
				if (value >= 0)
				{
					all = true;
					top = value;
					bottom = value;
					left = value;
					right = value;
					NotifyOwner();
				}
			}
		}

		private PanelMargins DefaultMargins
		{
			get
			{
				IDefaultValueProvider defaultValueProvider = Owner;
				if (defaultValueProvider == null)
				{
					return new PanelMargins(0, 0, 0, 0);
				}
				return (PanelMargins)defaultValueProvider.GetDefaultValue("Margins", this);
			}
		}

		internal PanelMargins(Panel owner)
			: this(owner, 0, 0, 0, 0)
		{
		}

		internal PanelMargins(int left, int top, int right, int bottom)
			: this(null, left, top, right, bottom)
		{
		}

		internal PanelMargins(Panel owner, int left, int top, int right, int bottom)
		{
			this.owner = owner;
			this.left = left;
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			SyncPropeties();
		}

		internal PanelMargins(PanelMargins margins)
			: this(margins.Owner, margins.Left, margins.Top, margins.Right, margins.Bottom)
		{
		}

		protected void ResetTop()
		{
			Top = DefaultMargins.Top;
		}

		protected bool ShouldSerializeTop()
		{
			if (!all && top != DefaultMargins.Top)
			{
				return true;
			}
			return false;
		}

		protected void ResetBottom()
		{
			Bottom = DefaultMargins.Bottom;
		}

		protected bool ShouldSerializeBottom()
		{
			if (!all && bottom != DefaultMargins.Bottom)
			{
				return true;
			}
			return false;
		}

		protected void ResetRight()
		{
			Right = DefaultMargins.Right;
		}

		protected bool ShouldSerializeRight()
		{
			if (!all && Right != DefaultMargins.Right)
			{
				return true;
			}
			return false;
		}

		protected void ResetLeft()
		{
			Left = DefaultMargins.Left;
		}

		protected bool ShouldSerializeLeft()
		{
			if (!all && left != DefaultMargins.Left)
			{
				return true;
			}
			return false;
		}

		protected void ResetAll()
		{
			All = DefaultMargins.All;
		}

		protected bool ShouldSerializeAll()
		{
			if (all && left != DefaultMargins.All)
			{
				return true;
			}
			return false;
		}

		private void SyncPropeties()
		{
			if (top == bottom && left == right && top == left)
			{
				all = true;
			}
			else
			{
				all = false;
			}
		}

		private void NotifyOwner()
		{
			if (owner != null)
			{
				owner.Invalidate();
				owner.SizeLocationChanged(SizeLocationChangeInfo.Margins);
			}
		}

		public RectangleF AdjustRectangle(RectangleF rect)
		{
			rect.X = Left;
			rect.Y = Top;
			rect.Width -= Left + Right;
			rect.Height -= Top + Bottom;
			return rect;
		}

		public Rectangle AdjustRectangle(Rectangle rect)
		{
			rect.X = Left;
			rect.Y = Top;
			rect.Width -= Left + Right;
			rect.Height -= Top + Bottom;
			return rect;
		}

		public override bool Equals(object obj)
		{
			if (obj is PanelMargins)
			{
				return (PanelMargins)obj == this;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "Left: {0}, Top: {0}, Right: {0}, Bottom: {0}", Left.ToString(CultureInfo.CurrentCulture), Top.ToString(CultureInfo.CurrentCulture), Right.ToString(CultureInfo.CurrentCulture), Bottom.ToString(CultureInfo.CurrentCulture));
		}

		internal bool IsEmpty()
		{
			return All == 0;
		}
	}
}
