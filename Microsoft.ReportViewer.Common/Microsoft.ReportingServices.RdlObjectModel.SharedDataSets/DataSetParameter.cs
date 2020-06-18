using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.SharedDataSets
{
	internal class DataSetParameter : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<DataSetParameter, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				DefaultValue,
				ReadOnly,
				Nullable,
				OmitFromQuery
			}
		}

		[XmlAttribute(typeof(string))]
		public string Name
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		public ReportExpression? DefaultValue
		{
			get
			{
				return base.PropertyStore.GetObject<ReportExpression?>(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public bool ReadOnly
		{
			get
			{
				return base.PropertyStore.GetBoolean(2);
			}
			set
			{
				base.PropertyStore.SetBoolean(2, value);
			}
		}

		public bool Nullable
		{
			get
			{
				return base.PropertyStore.GetBoolean(3);
			}
			set
			{
				base.PropertyStore.SetBoolean(3, value);
			}
		}

		public bool OmitFromQuery
		{
			get
			{
				return base.PropertyStore.GetBoolean(4);
			}
			set
			{
				base.PropertyStore.SetBoolean(4, value);
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			ReadOnly = false;
			Nullable = false;
			OmitFromQuery = false;
		}

		public DataSetParameter()
		{
		}

		internal DataSetParameter(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
