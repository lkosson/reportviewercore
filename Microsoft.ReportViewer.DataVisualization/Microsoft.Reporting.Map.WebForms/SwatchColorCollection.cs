using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SwatchColorCollection : NamedCollection
	{
		private SwatchColor this[int index]
		{
			get
			{
				return (SwatchColor)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private SwatchColor this[string name]
		{
			get
			{
				return (SwatchColor)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public SwatchColor this[object obj]
		{
			get
			{
				if (obj is string)
				{
					return this[(string)obj];
				}
				if (obj is int)
				{
					return this[(int)obj];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (obj is string)
				{
					this[(string)obj] = value;
					return;
				}
				if (obj is int)
				{
					this[(int)obj] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal SwatchColorCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(SwatchColor);
		}

		public SwatchColor Add(string name)
		{
			SwatchColor swatchColor = new SwatchColor();
			swatchColor.Name = name;
			Add(swatchColor);
			return swatchColor;
		}

		public int Add(SwatchColor value)
		{
			return base.List.Add(value);
		}

		public void Remove(SwatchColor value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Color1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Color{0}";
		}

		internal override void Invalidate()
		{
			NamedElement parentElement = base.ParentElement;
			ColorSwatchPanel colorSwatchPanel = null;
			do
			{
				colorSwatchPanel = (parentElement as ColorSwatchPanel);
			}
			while (parentElement.ParentElement != null && colorSwatchPanel == null);
			if (colorSwatchPanel != null && colorSwatchPanel.AutoSize && base.Common != null)
			{
				base.Common.MapCore.InvalidateAndLayout();
			}
			else
			{
				base.Invalidate();
			}
		}

		protected override void OnValidate(object value)
		{
			if (!(value is SwatchColor))
			{
				throw new InvalidCastException(SR.invalid_cast(value.GetType().Name, "SwatchColor"));
			}
			base.OnValidate(value);
		}
	}
}
