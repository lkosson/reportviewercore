using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomColorCollection : NamedCollection
	{
		private CustomColor this[int index]
		{
			get
			{
				return (CustomColor)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CustomColor this[string name]
		{
			get
			{
				return (CustomColor)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public CustomColor this[object obj]
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

		internal CustomColorCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CustomColor);
		}

		public CustomColor Add(string name)
		{
			CustomColor customColor = new CustomColor();
			customColor.Name = name;
			Add(customColor);
			return customColor;
		}

		public int Add(CustomColor value)
		{
			return base.List.Add(value);
		}

		public void Remove(CustomColor value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "CustomColor1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "CustomColor{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (CustomColor)value;
		}
	}
}
