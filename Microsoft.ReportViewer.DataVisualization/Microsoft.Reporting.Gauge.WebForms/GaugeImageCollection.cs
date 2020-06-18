using System;

namespace Microsoft.Reporting.Gauge.WebForms
{
	internal class GaugeImageCollection : NamedCollection
	{
		private GaugeImage this[int index]
		{
			get
			{
				if (index == 0 && base.List.Count == 0)
				{
					Add(new GaugeImage());
				}
				return (GaugeImage)base.List[index];
			}
			set
			{
				base.List.Insert(index, value);
			}
		}

		private GaugeImage this[string name]
		{
			get
			{
				return (GaugeImage)GetByNameCheck(name);
			}
			set
			{
				SetByNameCheck(name, value);
			}
		}

		public GaugeImage this[object obj]
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

		internal GaugeImageCollection(NamedElement parent, CommonElements common)
			: base(parent, common)
		{
			elementType = typeof(GaugeImage);
		}

		public GaugeImage Add(string name)
		{
			GaugeImage gaugeImage = new GaugeImage();
			gaugeImage.Name = name;
			Add(gaugeImage);
			return gaugeImage;
		}

		public int Add(GaugeImage value)
		{
			return base.List.Add(value);
		}

		public void Remove(GaugeImage value)
		{
			base.List.Remove(value);
		}

		public bool Contains(GaugeImage value)
		{
			return base.List.Contains(value);
		}

		public void Insert(int index, GaugeImage value)
		{
			base.List.Insert(index, value);
		}

		public int IndexOf(GaugeImage value)
		{
			return base.List.IndexOf(value);
		}

		internal override string GetDefaultElementName(NamedElement el)
		{
			return "Image1";
		}

		internal override string GetElementNameFormat(NamedElement el)
		{
			return "Image{0}";
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete(index, value);
			GaugeImage gaugeImage = (GaugeImage)value;
			if (gaugeImage.Position.DefaultValues && index != 0)
			{
				GaugeImage gaugeImage2 = this[index - 1];
				gaugeImage.Location.X = gaugeImage2.Location.X + 3f;
				gaugeImage.Location.Y = gaugeImage2.Location.Y + 3f;
			}
			if (gaugeImage.DefaultParent && gaugeImage.Parent.Length == 0 && base.Common != null)
			{
				if (base.Common.GaugeContainer.CircularGauges.Count > 0)
				{
					gaugeImage.Parent = "CircularGauges." + base.Common.GaugeContainer.CircularGauges[0].Name;
				}
				else if (base.Common.GaugeContainer.LinearGauges.Count > 0)
				{
					gaugeImage.Parent = "LinearGauges." + base.Common.GaugeContainer.LinearGauges[0].Name;
				}
			}
		}
	}
}
