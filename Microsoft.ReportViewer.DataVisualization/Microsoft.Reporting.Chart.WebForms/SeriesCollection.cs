using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeSeriesCollection_SeriesCollection")]
	internal class SeriesCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		private IServiceContainer serviceContainer;

		[SRDescription("DescriptionAttributeSeriesCollection_Item")]
		public Series this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Series)array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Series item in array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionDataSeriesNameNotFound((string)parameter));
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
						throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(value.Name));
					}
					array[(int)parameter] = value;
				}
				else
				{
					if (!(parameter is string))
					{
						throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
					}
					int num = 0;
					foreach (Series item in array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(value.Name));
							}
							array[num] = value;
							break;
						}
						num++;
					}
				}
				Invalidate("");
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
				Invalidate("");
			}
		}

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		private SeriesCollection()
		{
		}

		public SeriesCollection(IServiceContainer container)
		{
			serviceContainer = container;
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

		public Series Add(string name)
		{
			Series series = new Series(name);
			Add(series);
			Invalidate(series.ChartArea);
			return series;
		}

		public Series Add(string name, int yValuesPerPoint)
		{
			Series series = new Series(name, yValuesPerPoint);
			Add(series);
			Invalidate(series.ChartArea);
			return series;
		}

		public void Clear()
		{
			array.Clear();
			Invalidate("");
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
			Invalidate("");
		}

		public bool Contains(Series value)
		{
			return array.Contains(value);
		}

		public int IndexOf(Series value)
		{
			return array.IndexOf(value);
		}

		public void Remove(Series value)
		{
			array.Remove(value);
			Invalidate("");
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate("");
		}

		public int Add(object value)
		{
			if (!(value is Series))
			{
				throw new ArgumentException(SR.ExceptionDataSeriesObjectRequired);
			}
			if (((Series)value).Name.Length == 0)
			{
				int num = array.Count + 1;
				((Series)value).Name = "Series" + num.ToString(CultureInfo.InvariantCulture);
				while (GetIndex(((Series)value).Name) != -1)
				{
					num++;
					((Series)value).Name = "Series" + num.ToString(CultureInfo.InvariantCulture);
				}
			}
			if (GetIndex(((Series)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(((Series)value).Name));
			}
			((Series)value).serviceContainer = serviceContainer;
			Invalidate(((Series)value).ChartArea);
			return array.Add(value);
		}

		public void Insert(int index, Series value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (!(value is Series))
			{
				throw new ArgumentException(SR.ExceptionDataSeriesObjectRequired);
			}
			if (((Series)value).Name.Length == 0)
			{
				((Series)value).Name = "Series" + (array.Count + 1).ToString(CultureInfo.InvariantCulture);
			}
			if (GetIndex(((Series)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionSeriesNameAddedIsNotUnique(((Series)value).Name));
			}
			((Series)value).serviceContainer = serviceContainer;
			array.Insert(index, value);
			Invalidate(((Series)value).ChartArea);
		}

		public void CopyTo(Array array, int index)
		{
			((Series[])this.array.ToArray(typeof(Series))).CopyTo(array, index);
			Invalidate("");
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		private void Invalidate(string chartArea)
		{
		}
	}
}
