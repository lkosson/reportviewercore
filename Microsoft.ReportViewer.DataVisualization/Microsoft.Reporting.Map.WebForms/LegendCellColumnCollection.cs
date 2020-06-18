using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Legend column collection.")]
	internal class LegendCellColumnCollection : NamedCollection
	{
		private Legend legend;

		private LegendCellColumn this[int index]
		{
			get
			{
				return (LegendCellColumn)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LegendCellColumn this[string name]
		{
			get
			{
				return (LegendCellColumn)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		[SRDescription("DescriptionAttributeLegendCellColumnCollection_Item")]
		public LegendCellColumn this[object param]
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

		internal LegendCellColumnCollection(Legend legend, NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LegendCellColumn);
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

		public int Add(string headerText)
		{
			return base.List.Add(new LegendCellColumn(headerText));
		}

		public int Add(string name, string headerText)
		{
			return base.List.Add(new LegendCellColumn(name, headerText));
		}

		public int Add(string name, string headerText, ContentAlignment alignment)
		{
			return base.List.Add(new LegendCellColumn(name, headerText, alignment));
		}

		public void Insert(int index, LegendCellColumn column)
		{
			base.List.Insert(index, column);
		}

		public void Insert(int index, string headerText)
		{
			base.List.Insert(index, new LegendCellColumn(headerText));
		}

		public void Insert(int index, string name, string headerText)
		{
			base.List.Insert(index, new LegendCellColumn(name, headerText));
		}

		public void Insert(int index, string name, string headerText, ContentAlignment alignment)
		{
			base.List.Insert(index, new LegendCellColumn(name, headerText, alignment));
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((LegendCellColumn)value).SetContainingLegend(legend);
			base.OnInsertComplete(index, value);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((LegendCellColumn)newValue).SetContainingLegend(legend);
			base.OnSetComplete(index, oldValue, newValue);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Column1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Column{0}";
		}

		internal override void Invalidate()
		{
			if (legend != null)
			{
				legend.Invalidate();
			}
		}

		public LegendCellColumn FindByName(string name)
		{
			return (LegendCellColumn)GetByNameCheck(name);
		}
	}
}
