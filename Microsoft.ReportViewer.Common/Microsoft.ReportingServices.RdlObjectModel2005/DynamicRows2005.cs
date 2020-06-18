using Microsoft.ReportingServices.RdlObjectModel;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class DynamicRows2005 : ReportObject
	{
		internal class Definition : DefinitionStore<DynamicRows2005, Definition.Properties>
		{
			public enum Properties
			{
				Grouping,
				Sorting,
				Subtotal,
				ReportItems,
				Visibility
			}

			private Definition()
			{
			}
		}

		public Grouping2005 Grouping
		{
			get
			{
				return (Grouping2005)base.PropertyStore.GetObject(0);
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> Sorting
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public Subtotal2005 Subtotal
		{
			get
			{
				return (Subtotal2005)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[XmlElement(typeof(RdlCollection<ReportItem>))]
		public IList<ReportItem> ReportItems
		{
			get
			{
				return (IList<ReportItem>)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public Visibility Visibility
		{
			get
			{
				return (Visibility)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public DynamicRows2005()
		{
		}

		public DynamicRows2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Grouping = new Grouping2005();
			ReportItems = new RdlCollection<ReportItem>();
		}
	}
}
