using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.SharedDataSets
{
	internal class DataSet : DataSetBase
	{
		internal new class Definition : DefinitionStore<DataSet, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				CaseSensitivity,
				Collation,
				AccentSensitivity,
				KanatypeSensitivity,
				WidthSensitivity,
				InterpretSubtotalsAsDetails,
				Query,
				Fields,
				Filters
			}
		}

		public Query Query
		{
			get
			{
				return (Query)base.PropertyStore.GetObject(7);
			}
			set
			{
				base.PropertyStore.SetObject(7, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Field>))]
		[XmlArrayItem("Field", typeof(Field), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
		public IList<Field> Fields
		{
			get
			{
				return (IList<Field>)base.PropertyStore.GetObject(8);
			}
			set
			{
				base.PropertyStore.SetObject(8, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Filter>))]
		[XmlArrayItem("Filter", typeof(Filter), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
		public IList<Filter> Filters
		{
			get
			{
				return (IList<Filter>)base.PropertyStore.GetObject(9);
			}
			set
			{
				base.PropertyStore.SetObject(9, value);
			}
		}

		public DataSet()
		{
		}

		internal DataSet(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public bool Equals(DataSet dataSet)
		{
			if (dataSet == null)
			{
				return false;
			}
			if ((Query == null && dataSet.Query != null) || (Query != null && dataSet.Query == null) || !Query.Equals(dataSet.Query))
			{
				return false;
			}
			if (!FieldsAreEqual(Fields, dataSet.Fields))
			{
				return false;
			}
			if (!FiltersAreEqual(Filters, dataSet.Filters))
			{
				return false;
			}
			if (!base.Equals(dataSet))
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as DataSet);
		}

		public override int GetHashCode()
		{
			if (Query != null)
			{
				return Query.GetHashCode();
			}
			return base.GetHashCode();
		}

		private bool FieldsAreEqual(IList<Field> FirstList, IList<Field> SecondList)
		{
			if (FirstList.Count != SecondList.Count)
			{
				return false;
			}
			for (int i = 0; i < FirstList.Count; i++)
			{
				if (!FirstList[i].Equals(SecondList[i]))
				{
					return false;
				}
			}
			return true;
		}

		private bool FiltersAreEqual(IList<Filter> FirstList, IList<Filter> SecondList)
		{
			if (FirstList.Count != SecondList.Count)
			{
				return false;
			}
			for (int i = 0; i < FirstList.Count; i++)
			{
				if (!FirstList[i].Equals(SecondList[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override QueryBase GetQuery()
		{
			return Query;
		}

		public override void Initialize()
		{
			base.Initialize();
			Query = new Query();
			Fields = new RdlCollection<Field>();
			Filters = new RdlCollection<Filter>();
		}
	}
}
