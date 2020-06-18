using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class CustomData : DataRegionBody
	{
		internal class Definition : DefinitionStore<CustomData, Definition.Properties>
		{
			internal enum Properties
			{
				DataSetName,
				Filters,
				SortExpressions,
				DataColumnHierarchy,
				DataRowHierarchy,
				DataRows,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		public string DataSetName
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
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

		[XmlElement(typeof(RdlCollection<Filter>))]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[XmlElement(typeof(RdlCollection<SortExpression>))]
		public IList<SortExpression> SortExpressions
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

		public DataHierarchy DataColumnHierarchy
		{
			get
			{
				return (DataHierarchy)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		public DataHierarchy DataRowHierarchy
		{
			get
			{
				return (DataHierarchy)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		[XmlElement(typeof(RdlCollection<IList<IList<DataValue>>>))]
		[XmlArrayItem("DataRow", typeof(DataRow), NestingLevel = 0)]
		[XmlArrayItem("DataCell", typeof(DataCell), NestingLevel = 1)]
		public IList<IList<IList<DataValue>>> DataRows
		{
			get
			{
				return (IList<IList<IList<DataValue>>>)base.PropertyStore.GetObject(5);
			}
			set
			{
				base.PropertyStore.SetObject(5, value);
			}
		}

		public CustomData()
		{
		}

		internal CustomData(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataSetName = "";
			Filters = new RdlCollection<Filter>();
			SortExpressions = new RdlCollection<SortExpression>();
			DataRows = new RdlCollection<IList<IList<DataValue>>>();
		}
	}
}
