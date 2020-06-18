using System;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class FieldCollection : NamedCollection
	{
		private Field this[int index]
		{
			get
			{
				return (Field)base.List[index];
			}
			set
			{
				Insert(index, value);
			}
		}

		private Field this[string name]
		{
			get
			{
				return (Field)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Field this[object obj]
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

		internal FieldCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Field);
		}

		public Field Add(string name)
		{
			Field field = new Field();
			field.Name = name;
			Add(field);
			return field;
		}

		public int Add(Field value)
		{
			return base.List.Add(value);
		}

		public void Remove(Field value)
		{
			base.List.Remove(value);
		}

		public void Insert(int index, Field value)
		{
			base.List.Insert(index, value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Field1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Field{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
			}
			base.Invalidate();
		}

		internal void Purge()
		{
			for (int num = base.Count - 1; num >= 0; num--)
			{
				if (this[num].IsTemporary)
				{
					RemoveAt(num);
				}
			}
		}

		internal override void IsValidNameCheck(string name, NamedElement element)
		{
			base.IsValidNameCheck(name, element);
			if (name.IndexOf(' ') != -1)
			{
				throw new ArgumentException(SR.ExceptionCannotContainSpaces);
			}
		}
	}
}
