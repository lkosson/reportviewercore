using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class PathCollection : NamedCollection
	{
		private Path this[int index]
		{
			get
			{
				return (Path)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Path this[string name]
		{
			get
			{
				return (Path)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Path this[object obj]
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

		internal PathCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Path);
		}

		public Path Add(string name)
		{
			Path path = new Path();
			path.Name = name;
			Add(path);
			return path;
		}

		public int Add(Path value)
		{
			return base.List.Add(value);
		}

		public void Remove(Path value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common == null || base.Common.MapCore == null || base.Common.MapCore.PathFields == null)
			{
				return arrayList;
			}
			if (ignoreCase)
			{
				searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
			}
			FieldCollection pathFields = base.Common.MapCore.PathFields;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Path path = (Path)enumerator.Current;
					string text = ignoreCase ? path.Name.ToUpper(CultureInfo.CurrentCulture) : path.Name;
					if (exactSearch)
					{
						if (text == searchFor)
						{
							arrayList.Add(path);
							continue;
						}
					}
					else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
					{
						arrayList.Add(path);
						continue;
					}
					foreach (Field item in pathFields)
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
							object obj = path[item.Name];
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
										arrayList.Add(path);
										goto IL_01cb;
									}
								}
								else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
								{
									arrayList.Add(path);
									goto IL_01cb;
								}
							}
							else
							{
								object obj2 = item.Parse(searchFor);
								if (obj2 != null && obj2.Equals(obj))
								{
									arrayList.Add(path);
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
			return "Path1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Path{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			_ = (Path)value;
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
