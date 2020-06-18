using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace Microsoft.Reporting.Map.WebForms
{
	internal class DataBindingTargetResolver
	{
		private StringCollection fieldNames = new StringCollection();

		private Hashtable fieldsCache = new Hashtable();

		private FieldCollection fields;

		private NamedCollection items;

		private bool createNewItemForUnresoved;

		private Type newItemsType;

		public readonly BindingType BindingType;

		private DataBindingTargetResolver(FieldCollection fields)
		{
			this.fields = fields;
			foreach (Field field in this.fields)
			{
				if (field.UniqueIdentifier)
				{
					fieldNames.Add(field.Name);
					fieldsCache.Add(field.Name, null);
				}
			}
		}

		public DataBindingTargetResolver(FieldCollection fields, ShapeCollection shapes)
			: this(fields)
		{
			items = shapes;
			BindingType = BindingType.Shapes;
		}

		public DataBindingTargetResolver(FieldCollection fields, SymbolCollection symbols)
			: this(fields)
		{
			items = symbols;
			createNewItemForUnresoved = true;
			newItemsType = typeof(Symbol);
			BindingType = BindingType.Symbols;
		}

		public DataBindingTargetResolver(FieldCollection fields, GroupCollection groups)
			: this(fields)
		{
			items = groups;
			BindingType = BindingType.Groups;
		}

		public DataBindingTargetResolver(FieldCollection fields, PathCollection paths)
			: this(fields)
		{
			items = paths;
			BindingType = BindingType.Paths;
		}

		public NamedElement GetItemById(object itemID)
		{
			NamedElement namedElement = null;
			if (itemID is string)
			{
				namedElement = items.GetByName((string)itemID);
			}
			if (namedElement == null)
			{
				object key = Field.ConvertToSupportedValue(itemID);
				StringEnumerator enumerator = fieldNames.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						namedElement = (GetItemsByFiledName(current)[key] as NamedElement);
						if (namedElement != null)
						{
							break;
						}
					}
				}
				finally
				{
					(enumerator as IDisposable)?.Dispose();
				}
			}
			if (namedElement == null && createNewItemForUnresoved)
			{
				namedElement = (NamedElement)Activator.CreateInstance(newItemsType);
				if (namedElement != null)
				{
					if (itemID is string)
					{
						namedElement.Name = (string)itemID;
					}
					else
					{
						object obj = Field.ConvertToSupportedValue(itemID);
						StringEnumerator enumerator = fieldNames.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								string current2 = enumerator.Current;
								if (fields[current2].Type == obj.GetType())
								{
									SetFieldValue(namedElement, current2, obj);
									break;
								}
							}
						}
						finally
						{
							(enumerator as IDisposable)?.Dispose();
						}
					}
					AddItem(namedElement);
				}
			}
			return namedElement;
		}

		public NamedElement GetItemByIndex(int index)
		{
			return items.GetByIndex(index);
		}

		public bool ContainsField(string name)
		{
			return fields.GetIndex(name) >= 0;
		}

		public Field GetFieldByName(string fieldName)
		{
			return fields.GetByName(fieldName) as Field;
		}

		public void AddItem(NamedElement item)
		{
			((IList)items).Add((object)item);
			StringEnumerator enumerator = fieldNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					string current = enumerator.Current;
					Hashtable hashtable = fieldsCache[current] as Hashtable;
					if (hashtable == null)
					{
						continue;
					}
					PropertyInfo property = item.GetType().GetProperty("Item");
					if (property != null)
					{
						object value = property.GetValue(item, new object[1]
						{
							current
						});
						if (value != null)
						{
							hashtable[value] = item;
						}
					}
				}
			}
			finally
			{
				(enumerator as IDisposable)?.Dispose();
			}
		}

		public void AddField(Field field)
		{
			fields.Add(field);
		}

		public void SetFieldValue(NamedElement item, string fieldName, object value)
		{
			PropertyInfo property = item.GetType().GetProperty("Item");
			if (property != null)
			{
				property.SetValue(item, value, new object[1]
				{
					fieldName
				});
			}
		}

		private Hashtable GetItemsByFiledName(string fieldName)
		{
			if (!fieldNames.Contains(fieldName))
			{
				return null;
			}
			Hashtable hashtable = fieldsCache[fieldName] as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				foreach (NamedElement item in items)
				{
					object obj = null;
					if (item is Shape)
					{
						obj = ((Shape)item)[fieldName];
					}
					else if (item is Group)
					{
						obj = ((Group)item)[fieldName];
					}
					else if (item is Symbol)
					{
						obj = ((Symbol)item)[fieldName];
					}
					else if (item is Path)
					{
						obj = ((Path)item)[fieldName];
					}
					if (obj != null)
					{
						hashtable[obj] = item;
					}
				}
				fieldsCache[fieldName] = hashtable;
			}
			return hashtable;
		}
	}
}
