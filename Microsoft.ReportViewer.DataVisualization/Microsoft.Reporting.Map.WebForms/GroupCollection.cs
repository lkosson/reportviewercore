using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class GroupCollection : NamedCollection
	{
		private Group this[int index]
		{
			get
			{
				return (Group)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private Group this[string name]
		{
			get
			{
				return (Group)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public Group this[object obj]
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

		internal GroupCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(Group);
		}

		public Group Add(string name)
		{
			Group group = new Group();
			group.Name = name;
			Add(group);
			return group;
		}

		public int Add(Group value)
		{
			return base.List.Add(value);
		}

		public void Remove(Group value)
		{
			base.List.Remove(value);
		}

		public ArrayList Find(string searchFor, bool ignoreCase, bool exactSearch, bool uniqueOnlyFields)
		{
			ArrayList arrayList = new ArrayList();
			if (base.Common == null || base.Common.MapCore == null || base.Common.MapCore.GroupFields == null)
			{
				return arrayList;
			}
			if (ignoreCase)
			{
				searchFor = searchFor.ToUpper(CultureInfo.CurrentCulture);
			}
			FieldCollection groupFields = base.Common.MapCore.GroupFields;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group group = (Group)enumerator.Current;
					string text = ignoreCase ? group.Name.ToUpper(CultureInfo.CurrentCulture) : group.Name;
					if (exactSearch)
					{
						if (text == searchFor)
						{
							arrayList.Add(group);
							continue;
						}
					}
					else if (text.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
					{
						arrayList.Add(group);
						continue;
					}
					foreach (Field item in groupFields)
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
							object obj = group[item.Name];
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
										arrayList.Add(group);
										goto IL_01cb;
									}
								}
								else if (text2.IndexOf(searchFor, StringComparison.Ordinal) >= 0)
								{
									arrayList.Add(group);
									goto IL_01cb;
								}
							}
							else
							{
								object obj2 = item.Parse(searchFor);
								if (obj2 != null && obj2.Equals(obj))
								{
									arrayList.Add(group);
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
			return "Group1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Group{0}";
		}

		internal override void Invalidate()
		{
			if (base.Common != null)
			{
				base.Common.MapCore.InvalidateDataBinding();
			}
			base.Invalidate();
		}
	}
}
