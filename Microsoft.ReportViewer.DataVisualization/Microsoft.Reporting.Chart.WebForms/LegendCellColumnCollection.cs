using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellColumnCollection_LegendCellColumnCollection")]
	internal class LegendCellColumnCollection : CollectionBase
	{
		private Legend legend;

		[SRDescription("DescriptionAttributeLegendCellColumnCollection_Item")]
		public LegendCellColumn this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (LegendCellColumn)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (LegendCellColumn item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionLegendCellColumnNotFound((string)parameter));
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
			}
			set
			{
				int num = -1;
				if (value.Name.Length != 0)
				{
					num = base.List.IndexOf(value);
				}
				else
				{
					AssignUniqueName(value);
				}
				if (parameter is int)
				{
					if (num != -1 && num != (int)parameter)
					{
						throw new ArgumentException(SR.ExceptionLegendCellColumnAlreadyExistsInCollection(value.Name));
					}
					base.List[(int)parameter] = value;
				}
				else
				{
					if (!(parameter is string))
					{
						throw new ArgumentException(SR.ExceptionInvalidIndexerArgumentType);
					}
					int num2 = 0;
					foreach (LegendCellColumn item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionLegendCellColumnAlreadyExistsInCollection(value.Name));
							}
							base.List[num2] = value;
							break;
						}
						num2++;
					}
				}
				Invalidate();
			}
		}

		public LegendCellColumnCollection()
		{
		}

		internal LegendCellColumnCollection(Legend legend)
		{
			this.legend = legend;
		}

		public void Remove(string name)
		{
			Remove(FindByName(name));
		}

		public void Remove(LegendCellColumn column)
		{
			if (column != null)
			{
				base.List.Remove(column);
			}
		}

		public int Add(LegendCellColumn column)
		{
			return base.List.Add(column);
		}

		public int Add(string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			return base.List.Add(new LegendCellColumn(headerText, columnType, text, alignment));
		}

		public void Insert(int index, LegendCellColumn column)
		{
			base.List.Insert(index, column);
		}

		public void Insert(int index, string headerText, LegendCellColumnType columnType, string text, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCellColumn(headerText, columnType, text, alignment));
		}

		public bool Contains(LegendCellColumn value)
		{
			return base.List.Contains(value);
		}

		public int IndexOf(LegendCellColumn value)
		{
			return base.List.IndexOf(value);
		}

		protected override void OnInsert(int index, object value)
		{
			if (((LegendCellColumn)value).Name.Length == 0)
			{
				AssignUniqueName((LegendCellColumn)value);
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((LegendCellColumn)value).SetContainingLegend(legend);
			Invalidate();
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			Invalidate();
		}

		protected override void OnClearComplete()
		{
			Invalidate();
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((LegendCellColumn)newValue).SetContainingLegend(legend);
			Invalidate();
		}

		private void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate(invalidateLegendOnly: false);
			}
		}

		private void AssignUniqueName(LegendCellColumn column)
		{
			if (column.Name.Length == 0)
			{
				string empty = string.Empty;
				int num = 1;
				do
				{
					empty = "Column" + num.ToString(CultureInfo.InvariantCulture);
					num++;
				}
				while (FindByName(empty) != null && num < 10000);
				column.Name = empty;
			}
		}

		public LegendCellColumn FindByName(string name)
		{
			LegendCellColumn result = null;
			for (int i = 0; i < base.List.Count; i++)
			{
				if (string.Compare(this[i].Name, name, ignoreCase: false, CultureInfo.CurrentCulture) == 0)
				{
					result = this[i];
					break;
				}
			}
			return result;
		}
	}
}
