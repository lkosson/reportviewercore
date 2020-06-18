using System;
using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Legend items collection.")]
	internal class LegendItemsCollection : NamedCollection
	{
		internal Legend Legend;

		private LegendItem this[int index]
		{
			get
			{
				return (LegendItem)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private LegendItem this[string name]
		{
			get
			{
				return (LegendItem)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public LegendItem this[object obj]
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

		internal LegendItemsCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(LegendItem);
		}

		public void Insert(int index, Color color, string text)
		{
			LegendItem value = new LegendItem(text, color, "");
			base.List.Insert(index, value);
		}

		public void Insert(int index, string image, string text)
		{
			LegendItem value = new LegendItem(text, Color.Empty, image);
			base.List.Insert(index, value);
		}

		public LegendItem Add(string name)
		{
			LegendItem legendItem = new LegendItem();
			legendItem.Name = name;
			Add(legendItem);
			return legendItem;
		}

		public int Add(LegendItem item)
		{
			return base.List.Add(item);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "LegendItem1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "LegendItem{0}";
		}

		internal override void Invalidate()
		{
			if (Legend != null)
			{
				Legend.Invalidate();
			}
		}

		protected override void OnInsertComplete(int index, object value)
		{
			((LegendItem)value).Legend = Legend;
			base.OnInsertComplete(index, value);
			Invalidate();
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			Invalidate();
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			((LegendItem)newValue).Legend = Legend;
			base.OnSetComplete(index, oldValue, newValue);
			Invalidate();
		}
	}
}
