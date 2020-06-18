using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class CustomWidthCollection : NamedCollection
	{
		private CustomWidth this[int index]
		{
			get
			{
				return (CustomWidth)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private CustomWidth this[string name]
		{
			get
			{
				return (CustomWidth)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public CustomWidth this[object obj]
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

		internal CustomWidthCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(CustomWidth);
		}

		public CustomWidth Add(string name)
		{
			CustomWidth customWidth = new CustomWidth();
			customWidth.Name = name;
			Add(customWidth);
			return customWidth;
		}

		public int Add(CustomWidth value)
		{
			return base.List.Add(value);
		}

		public void Remove(CustomWidth value)
		{
			base.List.Remove(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "CustomWidth1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "CustomWidth{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (CustomWidth)value;
		}
	}
}
