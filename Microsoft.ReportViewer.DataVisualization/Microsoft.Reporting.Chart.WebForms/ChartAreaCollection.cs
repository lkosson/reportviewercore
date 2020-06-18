using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	internal class ChartAreaCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		private CommonElements common;

		private Chart chart;

		[SRDescription("DescriptionAttributeChartAreaCollection_Item")]
		public ChartArea this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (ChartArea)array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (ChartArea item in array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					if (chart == null && common != null)
					{
						chart = (Chart)common.container.GetService(typeof(Chart));
					}
					if ((string)parameter == "Default" && array.Count > 0 && chart != null && !chart.serializing)
					{
						return (ChartArea)array[0];
					}
					throw new ArgumentException(SR.ExceptionChartAreaAlreadyExistsInCollection((string)parameter));
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
						throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(value.Name));
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
					foreach (ChartArea item in array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(value.Name));
							}
							array[num] = value;
							break;
						}
						num++;
					}
				}
				Invalidate();
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

		internal ChartAreaCollection(CommonElements common)
		{
			this.common = common;
			common.chartAreaCollection = this;
		}

		private ChartAreaCollection()
		{
		}

		public int GetIndex(string name)
		{
			int result = -1;
			for (int i = 0; i < array.Count; i++)
			{
				if (this[i].Name == name)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public ChartArea Add(string name)
		{
			ChartArea chartArea = new ChartArea();
			if (UniqueName(name))
			{
				chartArea.Name = name;
				chartArea.SetCommon(common);
				array.Add(chartArea);
				Invalidate();
				return chartArea;
			}
			throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(name));
		}

		public int Add(object value)
		{
			if (!(value is ChartArea))
			{
				throw new ArgumentException(SR.ExceptionChartAreaObjectRequired);
			}
			if (((ChartArea)value).Name.Length == 0)
			{
				string text = CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(text));
				}
				((ChartArea)value).Name = text;
			}
			if (GetIndex(((ChartArea)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionChartAreaAddedIsNotUnique(((ChartArea)value).Name));
			}
			((ChartArea)value).SetCommon(common);
			Invalidate();
			return array.Add(value);
		}

		public void Insert(int index, ChartArea value)
		{
			Insert(index, (object)value);
		}

		public void Insert(int index, object value)
		{
			if (value is ChartArea)
			{
				if (!(value is ChartArea))
				{
					throw new ArgumentException(SR.ExceptionChartAreaObjectRequired);
				}
				if (((ChartArea)value).Name.Length == 0)
				{
					string text = CreateName(null);
					if (text == null)
					{
						throw new ArgumentException(SR.ExceptionChartAreaInsertedIsNotUnique(text));
					}
					((ChartArea)value).Name = text;
				}
				((ChartArea)value).SetCommon(common);
				array.Insert(index, value);
				Invalidate();
				return;
			}
			throw new ArgumentException(SR.ExceptionChartAreaInsertedHasWrongType);
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
				string text = "Chart Area " + num.ToString(CultureInfo.InvariantCulture);
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
			foreach (ChartArea item in array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
		}

		public bool Contains(ChartArea value)
		{
			return array.Contains(value);
		}

		public int IndexOf(ChartArea value)
		{
			return array.IndexOf(value);
		}

		public int IndexOf(string name)
		{
			int num = 0;
			foreach (ChartArea item in array)
			{
				if (item.Name == name)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void Remove(ChartArea value)
		{
			array.Remove(value);
		}

		private void Invalidate()
		{
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate();
		}

		public void Remove(object value)
		{
			array.Remove(value);
			Invalidate();
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
			Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate();
		}
	}
}
