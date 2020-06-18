using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Query : QueryBase
	{
		internal new class Definition : DefinitionStore<Query, Definition.Properties>
		{
			public enum Properties
			{
				CommandType,
				CommandText,
				Timeout,
				DataSourceName,
				QueryParameters
			}
		}

		public string DataSourceName
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

		[XmlIgnore]
		public DataSource DataSource
		{
			get
			{
				DataSource result = null;
				if (!string.IsNullOrEmpty(DataSourceName))
				{
					Report ancestor = GetAncestor<Report>();
					if (ancestor != null)
					{
						result = ancestor.GetDataSourceByName(DataSourceName);
					}
				}
				return result;
			}
		}

		[XmlElement(typeof(RdlCollection<QueryParameter>))]
		public IList<QueryParameter> QueryParameters
		{
			get
			{
				return (IList<QueryParameter>)base.PropertyStore.GetObject(4);
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
			DataSourceName = "";
			QueryParameters = new RdlCollection<QueryParameter>();
		}

		protected override void GetDependenciesCore(IList<ReportObject> dependencies)
		{
			base.GetDependenciesCore(dependencies);
			if (string.IsNullOrEmpty(DataSourceName))
			{
				return;
			}
			Report ancestor = GetAncestor<Report>();
			if (ancestor != null)
			{
				DataSource dataSourceByName = ancestor.GetDataSourceByName(DataSourceName);
				if (dataSourceByName != null && !dependencies.Contains(dataSourceByName))
				{
					dependencies.Add(dataSourceByName);
				}
			}
		}
	}
}
