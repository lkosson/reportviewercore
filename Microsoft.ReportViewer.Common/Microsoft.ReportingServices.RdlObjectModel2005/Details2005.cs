using Microsoft.ReportingServices.RdlObjectModel;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel2005
{
	internal class Details2005 : ReportObject
	{
		internal class Definition : DefinitionStore<Details2005, Definition.Properties>
		{
			public enum Properties
			{
				TableRows,
				Grouping,
				Sorting,
				Visibility
			}

			private Definition()
			{
			}
		}

		[XmlElement(typeof(RdlCollection<TableRow2005>))]
		[XmlArrayItem("TableRow", typeof(TableRow2005))]
		public IList<TableRow2005> TableRows
		{
			get
			{
				return (IList<TableRow2005>)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public Group Grouping
		{
			get
			{
				return (Group)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> Sorting
		{
			get
			{
				return (IList<SortExpression>)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public Visibility Visibility
		{
			get
			{
				return (Visibility)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public Details2005()
		{
		}

		public Details2005(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			TableRows = new RdlCollection<TableRow2005>();
		}
	}
}
