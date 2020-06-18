using Microsoft.ReportingServices.RdlObjectModel;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class TableGroup2005 : ReportObject
	{
		internal class Definition : DefinitionStore<TableGroup2005, Definition.Properties>
		{
			public enum Properties
			{
				Grouping,
				Sorting,
				Header,
				Footer,
				Visibility
			}

			private Definition()
			{
			}
		}

		public Group Grouping
		{
			get
			{
				return (Group)base.PropertyStore.GetObject(0);
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

		public Header2005 Header
		{
			get
			{
				return (Header2005)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Footer2005 Footer
		{
			get
			{
				return (Footer2005)base.PropertyStore.GetObject(3);
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

		public TableGroup2005()
		{
		}

		public TableGroup2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Grouping = new Group();
		}
	}
}
