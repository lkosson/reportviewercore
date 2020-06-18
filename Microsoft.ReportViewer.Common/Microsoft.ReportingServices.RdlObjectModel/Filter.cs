using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Filter : ReportObject
	{
		internal class Definition
		{
			internal enum Properties
			{
				FilterExpression,
				Operator,
				FilterValues
			}
		}

		public ReportExpression FilterExpression
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression>(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public Operators Operator
		{
			get
			{
				return (Operators)base.PropertyStore.GetInteger(1);
			}
			set
			{
				base.PropertyStore.SetInteger(1, (int)value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportExpression>))]
		[XmlArrayItem("FilterValue", typeof(ReportExpression))]
		public IList<ReportExpression> FilterValues
		{
			get
			{
				return (IList<ReportExpression>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Filter()
		{
		}

		internal Filter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			FilterValues = new RdlCollection<ReportExpression>();
		}

		public bool Equals(Filter filter)
		{
			if (filter == null)
			{
				return false;
			}
			if (FilterExpression == filter.FilterExpression && FilterValues == filter.FilterValues && Operator == filter.Operator)
			{
				return base.Equals(filter);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Filter);
		}

		public override int GetHashCode()
		{
			return FilterExpression.GetHashCode();
		}
	}
}
