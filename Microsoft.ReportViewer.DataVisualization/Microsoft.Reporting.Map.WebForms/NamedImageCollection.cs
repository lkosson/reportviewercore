using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class NamedImageCollection : NamedCollection
	{
		private NamedImage this[int index]
		{
			get
			{
				return (NamedImage)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private NamedImage this[string name]
		{
			get
			{
				return (NamedImage)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public NamedImage this[object obj]
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

		internal NamedImageCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(NamedImage);
		}

		public int Add(NamedImage value)
		{
			return base.List.Add(value);
		}

		public void Remove(NamedImage value)
		{
			base.List.Remove(value);
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "NamedImage{0}";
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "NamedImage1";
		}
	}
}
