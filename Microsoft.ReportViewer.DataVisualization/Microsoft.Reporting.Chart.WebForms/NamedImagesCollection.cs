using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class NamedImagesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal Chart chart;

		[SRDescription("DescriptionAttributeNamedImagesCollection_Item")]
		public NamedImage this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (NamedImage)array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (NamedImage item in array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionNamedImageNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int index = GetIndex(value.Name);
				if (parameter is int)
				{
					if (index != -1 && index != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(value.Name));
					}
					array[(int)parameter] = value;
					return;
				}
				if (parameter is string)
				{
					int num = 0;
					foreach (NamedImage item in array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(value.Name));
							}
							array[num] = value;
							break;
						}
						num++;
					}
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
		}

		object IList.this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public bool IsReadOnly => array.IsReadOnly;

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsSynchronized => array.IsSynchronized;

		public int Count => array.Count;

		public object SyncRoot => array.SyncRoot;

		internal NamedImagesCollection(Chart chart)
		{
			this.chart = chart;
		}

		private NamedImagesCollection()
		{
		}

		public int GetIndex(string name)
		{
			int result = -1;
			for (int i = 0; i < array.Count; i++)
			{
				if (string.Compare(this[i].Name, name, StringComparison.Ordinal) == 0)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public NamedImage Add(string name, Image image)
		{
			NamedImage namedImage = new NamedImage(name, image);
			if (UniqueName(name))
			{
				namedImage.Name = name;
				array.Add(namedImage);
				return namedImage;
			}
			throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(name));
		}

		public int Add(NamedImage value)
		{
			return array.Add(value);
		}

		public int Add(object value)
		{
			if (!(value is NamedImage))
			{
				throw new ArgumentException(SR.ExceptionNamedImageObjectRequired);
			}
			if (((NamedImage)value).Name == "")
			{
				string text = CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(text));
				}
				((NamedImage)value).Name = text;
			}
			if (GetIndex(((NamedImage)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionNamedImageAddedIsNotUnique(((NamedImage)value).Name));
			}
			return array.Add(value);
		}

		public bool Contains(NamedImage value)
		{
			return array.Contains(value);
		}

		public int IndexOf(NamedImage value)
		{
			return array.IndexOf(value);
		}

		public void Remove(NamedImage value)
		{
			array.Remove(value);
		}

		public void Insert(int index, NamedImage value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is NamedImage)
			{
				if (!(value is NamedImage))
				{
					throw new ArgumentException(SR.ExceptionNamedImageObjectRequired);
				}
				if (((NamedImage)value).Name == "")
				{
					string text = CreateName(null);
					if (text == null)
					{
						throw new ArgumentException(SR.ExceptionNamedImageInsertedIsNotUnique(text));
					}
					((NamedImage)value).Name = text;
				}
				array.Insert(index, value);
				return;
			}
			throw new ArgumentException(SR.ExceptionNamedImageInsertedHasWrongType);
		}

		private string CreateName(string Name)
		{
			if (Name != null && UniqueName(Name))
			{
				return Name;
			}
			int num = 1;
			while (num < int.MaxValue)
			{
				string text = "Named Image " + num.ToString(CultureInfo.InvariantCulture);
				num++;
				if (UniqueName(text))
				{
					return text;
				}
			}
			return null;
		}

		private bool UniqueName(string name)
		{
			foreach (NamedImage item in array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
		}

		public void Remove(object value)
		{
			array.Remove(value);
		}

		public int IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public bool Contains(object value)
		{
			return array.Contains(value);
		}

		public void Clear()
		{
			array.Clear();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
		}
	}
}
