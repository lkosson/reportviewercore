using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeLabelCollection : NamedCollection
	{
		private GaugeLabel this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new GaugeLabel());
				}
				return (GaugeLabel)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private GaugeLabel this[string name]
		{
			get
			{
				return (GaugeLabel)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public GaugeLabel this[object obj]
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
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
				throw new ArgumentException(Utils.SRGetStr("ExceptionInvalidIndexer_error"));
			}
		}

		internal GaugeLabelCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(GaugeLabel);
		}

		public GaugeLabel Add(string name)
		{
			GaugeLabel gaugeLabel = new GaugeLabel();
			gaugeLabel.Name = name;
			Add(gaugeLabel);
			return gaugeLabel;
		}

		public int Add(GaugeLabel value)
		{
			return base.List.Add(value);
		}

		public void Remove(GaugeLabel value)
		{
			base.List.Remove(value);
		}

		public bool Contains(GaugeLabel value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, GaugeLabel value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(GaugeLabel value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Label1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Label{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			GaugeLabel gaugeLabel = (GaugeLabel)value;
			if (gaugeLabel.Position.DefaultValues && index != 0)
			{
				GaugeLabel gaugeLabel2 = this[index - 1];
				gaugeLabel.Location.X = gaugeLabel2.Location.X + 3f;
				gaugeLabel.Location.Y = gaugeLabel2.Location.Y + 3f;
			}
			if (gaugeLabel.DefaultParent && gaugeLabel.Parent.Length == 0 && base.Common != null)
			{
				if (base.Common.GaugeContainer.CircularGauges.Count > 0)
				{
					gaugeLabel.Parent = "CircularGauges." + base.Common.GaugeContainer.CircularGauges[0].Name;
				}
				else if (base.Common.GaugeContainer.LinearGauges.Count > 0)
				{
					gaugeLabel.Parent = "LinearGauges." + base.Common.GaugeContainer.LinearGauges[0].Name;
				}
			}
		}
	}
}
