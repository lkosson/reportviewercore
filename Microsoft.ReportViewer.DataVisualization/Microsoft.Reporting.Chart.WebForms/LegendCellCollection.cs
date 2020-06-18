using System;
using System.Collections;
using System.Drawing;
using System.Globalization;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeLegendCellCollection_LegendCellCollection")]
	internal class LegendCellCollection : CollectionBase
	{
		private LegendItem legendItem;

		[SRDescription("DescriptionAttributeLegendCellCollection_Item")]
		public LegendCell this[object parameter]
		{
			get
			{
				if (parameter is int)
				{
					return (LegendCell)base.List[(int)parameter];
				}
				if (parameter is string)
				{
					foreach (LegendCell item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							return item;
						}
					}
					throw new ArgumentException(SR.ExceptionLegendCellNotFound((string)parameter));
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
						throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value.Name));
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
					foreach (LegendCell item in base.List)
					{
						if (item.Name == (string)parameter)
						{
							if (num != -1 && num != num2)
							{
								throw new ArgumentException(SR.ExceptionLegendCellNameAlreadyExistsInCollection(value.Name));
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

		public LegendCellCollection()
		{
		}

		internal LegendCellCollection(LegendItem legendItem)
		{
			this.legendItem = legendItem;
		}

		public void Remove(string name)
		{
			Remove(FindByName(name));
		}

		public void Remove(LegendCell cell)
		{
			if (cell != null)
			{
				base.List.Remove(cell);
			}
		}

		public int Add(LegendCell cell)
		{
			return base.List.Add(cell);
		}

		public int Add(LegendCellType cellType, string text, ContentAlignment alignment)
		{
			return base.List.Add(new LegendCell(cellType, text, alignment));
		}

		public void Insert(int index, LegendCell cell)
		{
			base.List.Insert(index, cell);
		}

		public void Insert(int index, LegendCellType cellType, string text, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCell(cellType, text, alignment));
		}

		public bool Contains(LegendCell value)
		{
			return base.List.Contains(value);
		}

		protected override void OnInsert(int index, object value)
		{
			if (((LegendCell)value).Name.Length == 0)
			{
				AssignUniqueName((LegendCell)value);
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if (legendItem != null)
			{
				((LegendCell)value).SetContainingLegend(legendItem.Legend, legendItem);
			}
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
			if (legendItem != null)
			{
				((LegendCell)newValue).SetContainingLegend(legendItem.Legend, legendItem);
			}
			Invalidate();
		}

		public int IndexOf(LegendCell cell)
		{
			return base.List.IndexOf(cell);
		}

		private void Invalidate()
		{
			if (legendItem != null && legendItem.Legend != null)
			{
				legendItem.Legend.Invalidate(invalidateLegendOnly: false);
			}
		}

		private void AssignUniqueName(LegendCell cell)
		{
			string empty = string.Empty;
			int num = 1;
			do
			{
				empty = "Cell" + num.ToString(CultureInfo.InvariantCulture);
				num++;
			}
			while (FindByName(empty) != null && num < 10000);
			cell.Name = empty;
		}

		public LegendCell FindByName(string name)
		{
			LegendCell result = null;
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
