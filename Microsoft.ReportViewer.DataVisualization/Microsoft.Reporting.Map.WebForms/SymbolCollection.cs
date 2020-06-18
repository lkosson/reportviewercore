using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class SymbolCollection : NamedCollection
	{
		private Symbol this[int index]
		{
			get
			{
				return (Symbol)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Symbol this[string name]
		{
			get
			{
				return (Symbol)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Symbol this[object obj]
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

		internal SymbolCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Symbol);
		}

		public Symbol Add(string name)
		{
			Symbol symbol = new Symbol();
			symbol.Name = name;
			Add(symbol);
			return symbol;
		}

		public int Add(Symbol value)
		{
			return base.List.Add(value);
		}

		public void Remove(Symbol value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common == null || base.Common.MapCore == null || base.Common.MapCore.SymbolFields == null)
			{
				return arrayList;
			}
			if (ignoreCase)
			{
				searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
			}
			FieldCollection symbolFields = base.Common.MapCore.SymbolFields;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Symbol symbol = (Symbol)enumerator.Current;
					string text = ignoreCase ? symbol.Name.ToUpper(CultureInfo.CurrentCulture) : symbol.Name;
					if (exactSearch)
					{
						if (text == searchFor)
						{
							arrayList.Add(symbol);
							continue;
						}
					}
					else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
					{
						arrayList.Add(symbol);
						continue;
					}
					foreach (Field item in symbolFields)
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
							object obj = symbol[item.Name];
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
										arrayList.Add(symbol);
										goto IL_01cb;
									}
								}
								else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
								{
									arrayList.Add(symbol);
									goto IL_01cb;
								}
							}
							else
							{
								object obj2 = item.Parse(searchFor);
								if (obj2 != null && obj2.Equals(obj))
								{
									arrayList.Add(symbol);
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
			return "Symbol1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Symbol{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (Symbol)value;
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
