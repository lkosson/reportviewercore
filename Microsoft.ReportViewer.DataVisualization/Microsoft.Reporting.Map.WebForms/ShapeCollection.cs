using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class ShapeCollection : NamedCollection
	{
		private Shape this[int index]
		{
			get
			{
				return (Shape)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Shape this[string name]
		{
			get
			{
				return (Shape)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Shape this[object obj]
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

		internal ShapeCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Shape);
		}

		public Shape Add(string name)
		{
			Shape shape = new Shape();
			shape.Name = name;
			Add(shape);
			return shape;
		}

		public int Add(Shape value)
		{
			return base.List.Add(value);
		}

		public void Remove(Shape value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common == null || base.Common.MapCore == null || base.Common.MapCore.ShapeFields == null)
			{
				return arrayList;
			}
			if (ignoreCase)
			{
				searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
			}
			FieldCollection shapeFields = base.Common.MapCore.ShapeFields;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Shape shape = (Shape)enumerator.Current;
					string text = ignoreCase ? shape.Name.ToUpper(CultureInfo.CurrentCulture) : shape.Name;
					if (exactSearch)
					{
						if (text == searchFor)
						{
							arrayList.Add(shape);
							continue;
						}
					}
					else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
					{
						arrayList.Add(shape);
						continue;
					}
					foreach (Field item in shapeFields)
					{
						if (uniqueOnlyFields && !item.UniqueIdentifier)
						{
							continue;
						}
						try
						{
							if (base.Common.MapCore.IsDesignMode() && item.IsTemporary)
							{
								continue;
							}
							object obj = shape[item.Name];
							if (obj == null)
							{
								continue;
							}
							if (item.Type == typeof(string))
							{
								string text2 = ignoreCase ? ((string)obj).ToUpper(CultureInfo.CurrentCulture) : ((string)obj);
								if (exactSearch)
								{
									if (text2 == searchFor)
									{
										arrayList.Add(shape);
										goto IL_01cb;
									}
								}
								else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
								{
									arrayList.Add(shape);
									goto IL_01cb;
								}
							}
							else
							{
								object obj2 = item.Parse(searchFor);
								if (obj2 != null && obj2.Equals(obj))
								{
									arrayList.Add(shape);
									goto IL_01cb;
								}
							}
						}
						catch
						{
						}
					}
					IL_01cb:;
				}
				return arrayList;
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Shape1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Shape{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (Shape)value;
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
				base.Common.MapCore.InvalidateCachedBounds();
			}
			base.Invalidate();
		}
	}
}
