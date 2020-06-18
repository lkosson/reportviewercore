using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class EmbeddedImage : ReportObject, INamedObject
	{
		internal class Definition : DefinitionStore<EmbeddedImage, Definition.Properties>
		{
			internal enum Properties
			{
				Name,
				MIMEType,
				ImageData
			}

			private Definition()
			{
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

		public string MIMEType
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

		public ImageData ImageData
		{
			get
			{
				return (ImageData)base.PropertyStore.GetObject(2);
			}
			set
			{
				base.PropertyStore.SetObject(2, value);
			}
		}

		public EmbeddedImage()
		{
		}

		internal EmbeddedImage(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}
	}
}
