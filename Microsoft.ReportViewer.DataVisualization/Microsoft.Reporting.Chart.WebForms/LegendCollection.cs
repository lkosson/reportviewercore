using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCollection_LegendCollection")]
	internal class LegendCollection : IList, ICollection, IEnumerable
	{
		private ArrayList array = new ArrayList();

		internal CommonElements common;

		[SRDescription("DescriptionAttributeLegendCollection_Item")]
		public Legend this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (Legend)array[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (Legend item in array)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					if ((string)parameter == "Default" && array.Count > 0)
					{
						return (Legend)array[0];
					}
					throw new ArgumentException(SR.ExceptionLegendNotFound((string)parameter));
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
						throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(value.Name));
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
					foreach (Legend item in array)
					{
						if (item.Name == (string)parameter)
						{
							if (index != -1 && index != num)
							{
								throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(value.Name));
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

		public bool IsFixedSize => array.IsFixedSize;

		public bool IsReadOnly => array.IsReadOnly;

		public int Count => array.Count;

		public bool IsSynchronized => array.IsSynchronized;

		public object SyncRoot => array.SyncRoot;

		public LegendCollection()
		{
		}

		public LegendCollection(CommonElements common)
		{
			this.common = common;
		}

		public int Add(string name)
		{
			Legend value = new Legend(common, name);
			int result = Add(value);
			Invalidate();
			return result;
		}

		public void Insert(int index, string name)
		{
			Legend value = new Legend(common, name);
			Insert(index, value);
			Invalidate();
		}

		public void Clear()
		{
			array.Clear();
			Invalidate();
		}

		bool IList.Contains(object value)
		{
			return array.Contains(value);
		}

		public bool Contains(Legend value)
		{
			return array.Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return array.IndexOf(value);
		}

		public int IndexOf(Legend value)
		{
			return array.IndexOf(value);
		}

		void IList.Remove(object value)
		{
			array.Remove(value);
			Invalidate();
		}

		public void Remove(Legend value)
		{
			((IList)this).Remove((object)value);
		}

		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
			Invalidate();
		}

		public int Add(object value)
		{
			if (!(value is Legend))
			{
				throw new ArgumentException(SR.ExceptionLegendAddedHasWrongType);
			}
			if (((Legend)value).Name.Length == 0)
			{
				string text = CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(text));
				}
				((Legend)value).Name = text;
			}
			bool flag = true;
			int result = 0;
			if (((Legend)value).Name == "Default")
			{
				int index = GetIndex("Default");
				if (index >= 0)
				{
					array[index] = value;
					flag = false;
					result = index;
				}
			}
			if (flag && GetIndex(((Legend)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(((Legend)value).Name));
			}
			((Legend)value).Common = common;
			Invalidate();
			if (flag)
			{
				result = array.Add(value);
			}
			return result;
		}

		public void Insert(int index, Legend value)
		{
			value.Common = common;
			Insert(index, (object)value);
			Invalidate();
		}

		public void Insert(int index, object value)
		{
			if (!(value is Legend))
			{
				throw new ArgumentException(SR.ExceptionLegendInsertedHasWrongType);
			}
			if (((Legend)value).Name.Length == 0)
			{
				string text = CreateName(null);
				if (text == null)
				{
					throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(text));
				}
				((Legend)value).Name = text;
			}
			bool flag = true;
			if (((Legend)value).Name == "Default")
			{
				int index2 = GetIndex("Default");
				if (index2 >= 0)
				{
					array[index2] = value;
					flag = false;
				}
			}
			if (flag && GetIndex(((Legend)value).Name) != -1)
			{
				throw new ArgumentException(SR.ExceptionLegendAddedIsNotUnique(((Legend)value).Name));
			}
			((Legend)value).Common = common;
			if (flag)
			{
				array.Insert(index, value);
			}
			Invalidate();
		}

		public void CopyTo(Array array, int index)
		{
			this.array.CopyTo(array, index);
			Invalidate();
		}

		public IEnumerator GetEnumerator()
		{
			return array.GetEnumerator();
		}

		private bool UniqueName(string name)
		{
			foreach (Legend item in array)
			{
				if (item.Name == name)
				{
					return false;
				}
			}
			return true;
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
				string text = "Legend" + num.ToString(CultureInfo.InvariantCulture);
				num++;
				if (UniqueName(text))
				{
					return text;
				}
			}
			return null;
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

		private void Invalidate()
		{
		}

		internal void CalcLegendPosition(ChartGraphics chartGraph, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			foreach (Legend item in array)
			{
				if (item.IsEnabled() && item.DockToChartArea == "NotSet" && item.Position.Auto)
				{
					item.CalcLegendPosition(chartGraph, ref chartAreasRectangle, maxLegendSize, elementSpacing);
				}
			}
		}

		internal void CalcOutsideLegendPosition(ChartGraphics chartGraph, ChartArea area, ref RectangleF chartAreasRectangle, float maxLegendSize, float elementSpacing)
		{
			if (common == null || common.ChartPicture == null)
			{
				return;
			}
			float num = Math.Min(chartAreasRectangle.Height / 100f * elementSpacing, chartAreasRectangle.Width / 100f * elementSpacing);
			foreach (Legend item in array)
			{
				if (item.DockToChartArea != "NotSet")
				{
					try
					{
						_ = common.ChartPicture.ChartAreas[item.DockToChartArea];
					}
					catch
					{
						throw new ArgumentException(SR.ExceptionLegendDockedChartAreaIsMissing(item.DockToChartArea));
					}
				}
				if (!item.IsEnabled() || item.DockInsideChartArea || !(item.DockToChartArea == area.Name) || !item.Position.Auto)
				{
					continue;
				}
				item.CalcLegendPosition(chartGraph, ref chartAreasRectangle, maxLegendSize, num);
				RectangleF rectangleF = item.Position.ToRectangleF();
				if (item.Docking == LegendDocking.Top)
				{
					rectangleF.Y -= num;
					if (!area.Position.Auto)
					{
						rectangleF.Y -= rectangleF.Height;
					}
				}
				else if (item.Docking == LegendDocking.Bottom)
				{
					rectangleF.Y += num;
					if (!area.Position.Auto)
					{
						rectangleF.Y = area.Position.Bottom() + num;
					}
				}
				if (item.Docking == LegendDocking.Left)
				{
					rectangleF.X -= num;
					if (!area.Position.Auto)
					{
						rectangleF.X -= rectangleF.Width;
					}
				}
				if (item.Docking == LegendDocking.Right)
				{
					rectangleF.X += num;
					if (!area.Position.Auto)
					{
						rectangleF.X = area.Position.Right() + num;
					}
				}
				item.Position.SetPositionNoAuto(rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
			}
		}

		internal void CalcInsideLegendPosition(ChartGraphics chartGraph, float maxLegendSize, float elementSpacing)
		{
			if (common == null || common.ChartPicture == null)
			{
				return;
			}
			foreach (Legend item in array)
			{
				if (item.DockToChartArea != "NotSet")
				{
					try
					{
						_ = common.ChartPicture.ChartAreas[item.DockToChartArea];
					}
					catch
					{
						throw new ArgumentException(SR.ExceptionLegendDockedChartAreaIsMissing(item.DockToChartArea));
					}
				}
			}
			foreach (ChartArea chartArea in common.ChartPicture.ChartAreas)
			{
				if (!chartArea.Visible)
				{
					continue;
				}
				RectangleF chartAreasRectangle = chartArea.PlotAreaPosition.ToRectangleF();
				float elementSpacing2 = Math.Min(chartAreasRectangle.Height / 100f * elementSpacing, chartAreasRectangle.Width / 100f * elementSpacing);
				foreach (Legend item2 in array)
				{
					if (item2.IsEnabled() && item2.DockInsideChartArea && item2.DockToChartArea == chartArea.Name && item2.Position.Auto)
					{
						item2.CalcLegendPosition(chartGraph, ref chartAreasRectangle, maxLegendSize, elementSpacing2);
					}
				}
			}
		}
	}
}
