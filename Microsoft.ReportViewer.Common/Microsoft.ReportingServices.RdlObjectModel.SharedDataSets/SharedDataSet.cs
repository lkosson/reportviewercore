using Microsoft.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel.SharedDataSets
{
	[XmlElementClass("SharedDataSet", typeof(SharedDataSet), Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2010/01/shareddatasetdefinition")]
	internal class SharedDataSet : ReportObject, IGlobalNamedObject, INamedObject
	{
		internal class SharedDataSetSerializerHost : ISerializerHost
		{
			public Type GetSubstituteType(Type type)
			{
				return type;
			}

			public void OnDeserialization(object value)
			{
			}

			public IEnumerable<ExtensionNamespace> GetExtensionNamespaces()
			{
				return new ExtensionNamespace[1]
				{
					new ExtensionNamespace("rd", "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")
				};
			}
		}

		internal class Definition : DefinitionStore<SharedDataSet, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				Description,
				DataSet
			}
		}

		private string m_reportServerUrl;

		[XmlIgnore]
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

		public string Description
		{
			get
			{
				return (string)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		public DataSet DataSet
		{
			get
			{
				return (DataSet)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		[XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		[DefaultValue("")]
		public string ReportServerUrl
		{
			get
			{
				return m_reportServerUrl;
			}
			set
			{
				m_reportServerUrl = value;
			}
		}

		public SharedDataSet()
		{
		}

		internal SharedDataSet(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		private static bool ContainsMappingName(List<MemberMapping> list, string name)
		{
			foreach (MemberMapping item in list)
			{
				if (item.Name.Equals(name))
				{
					return true;
				}
			}
			return false;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public static void Serialize(Stream stream, SharedDataSet sharedDataSet)
		{
			if (sharedDataSet != null)
			{
				CreateSerializer().Serialize(stream, sharedDataSet);
			}
		}

		public static SharedDataSet Deserialize(Stream stream)
		{
			return (SharedDataSet)CreateSerializer().Deserialize(stream, typeof(SharedDataSet));
		}

		internal static RdlSerializer CreateSerializer()
		{
			SharedDataSetSerializerHost host = new SharedDataSetSerializerHost();
			return new RdlSerializer(new RdlSerializerSettings
			{
				Host = host
			});
		}
	}
}
