using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.SharedDataSets
{
	internal class Query : QueryBase
	{
		internal new class Definition : DefinitionStore<Query, Definition.Properties>
		{
			internal enum Properties
			{
				CommandType,
				CommandText,
				Timeout,
				DataSourceReference,
				DataSetParameters
			}
		}

		public string DataSourceReference
		{
			get
			{
				return (string)base.PropertyStore.GetObject(3);
			}
			set
			{
				base.PropertyStore.SetObject(3, value);
			}
		}

		[XmlElement(typeof(RdlCollection<DataSetParameter>))]
		[XmlArrayItem("DataSetParameter", typeof(DataSetParameter), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
		public IList<DataSetParameter> DataSetParameters
		{
			get
			{
				return (IList<DataSetParameter>)base.PropertyStore.GetObject(4);
			}
			set
			{
				base.PropertyStore.SetObject(4, value);
			}
		}

		public Query()
		{
		}

		internal Query(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			DataSourceReference = "";
			DataSetParameters = new RdlCollection<DataSetParameter>();
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			string.IsNullOrEmpty(DataSourceReference);
		}

		public bool Equals(Query query)
		{
			if (query == null)
			{
				return false;
			}
			if (DataSetParametersAreEqual(DataSetParameters, query.DataSetParameters) && DataSourceReference == query.DataSourceReference)
			{
				return Equals((QueryBase)query);
			}
			return false;
		}

		private bool DataSetParametersAreEqual(IList<DataSetParameter> FirstList, IList<DataSetParameter> SecondList)
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

		public override bool Equals(object obj)
		{
			return Equals(obj as Query);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
