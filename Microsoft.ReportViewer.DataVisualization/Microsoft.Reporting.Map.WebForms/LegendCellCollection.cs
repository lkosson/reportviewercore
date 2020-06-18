using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Legend item cell collection.")]
	internal class LegendCellCollection : NamedCollection
	{
		private LegendItem legendItem;

		private LegendCell this[int index]
		{
			get
			{
				return (LegendCell)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LegendCell this[string name]
		{
			get
			{
				return (LegendCell)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		[SRDescription("DescriptionAttributeLegendCellCollection_Item")]
		public LegendCell this[object param]
		{
			get
			{
				if (param is string)
				{
					return this[(string)param];
				}
				if (param is int)
				{
					return this[(int)param];
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
			set
			{
				if (param is string)
				{
					this[(string)param] = value;
					return;
				}
				if (param is int)
				{
					this[(int)param] = value;
					return;
				}
				throw new ArgumentException(SR.ExceptionInvalidIndexerArgument);
			}
		}

		internal LegendCellCollection(LegendItem legendItem, NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LegendCell);
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

		public int Add(string name, LegendCellType cellType, string text, ContentAlignment alignment)
		{
			LegendCell legendCell = new LegendCell(cellType, text, alignment);
			legendCell.Name = name;
			return base.List.Add(legendCell);
		}

		public void Insert(int index, LegendCell cell)
		{
			base.List.Insert(index, cell);
		}

		public void Insert(int index, LegendCellType cellType, string text, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCell(cellType, text, alignment));
		}

		public int IndexOf(LegendCell cell)
		{
			return base.List.IndexOf(cell);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			if (legendItem != null)
			{
				((LegendCell)value).SetContainingLegend(legendItem.Legend, legendItem);
			}
			base.OnInsertComplete(index, value);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			if (legendItem != null)
			{
				((LegendCell)newValue).SetContainingLegend(legendItem.Legend, legendItem);
			}
			base.OnSetComplete(index, oldValue, newValue);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Cell1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Cell{0}";
		}

		internal override void Invalidate()
		{
			if (legendItem != null && legendItem.Legend != null)
			{
				legendItem.Legend.Invalidate();
			}
		}

		public LegendCell FindByName(string name)
		{
			return (LegendCell)GetByNameCheck(name);
		}
	}
}
