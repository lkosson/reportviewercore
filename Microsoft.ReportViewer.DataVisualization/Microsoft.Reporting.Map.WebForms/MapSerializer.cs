using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Map serializer class.")]
	[DefaultProperty("Format")]
	internal class MapSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private MapCore mapObject;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContent content = SerializationContent.All;

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(typeof(SerializationContent), "All")]
		[SRDescription("DescriptionAttributeMapSerializer_Content")]
		public SerializationContent Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
				SetSerializableConentFromFlags();
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(typeof(SerializationFormat), "Xml")]
		[SRDescription("DescriptionAttributeMapSerializer_Format")]
		public SerializationFormat Format
		{
			get
			{
				return format;
			}
			set
			{
				if (format != value)
				{
					format = value;
					SerializerBase serializerBase = null;
					serializerBase = ((format != SerializationFormat.Binary) ? ((SerializerBase)new XmlFormatSerializer()) : ((SerializerBase)new BinaryFormatSerializer()));
					serializerBase.IgnoreUnknownAttributes = serializer.IgnoreUnknownAttributes;
					serializerBase.NonSerializableContent = serializer.NonSerializableContent;
					serializerBase.ResetWhenLoading = serializer.ResetWhenLoading;
					serializerBase.SerializableContent = serializer.SerializableContent;
					serializer = serializerBase;
				}
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeMapSerializer_ResetWhenLoading")]
		public bool ResetWhenLoading
		{
			get
			{
				return serializer.ResetWhenLoading;
			}
			set
			{
				serializer.ResetWhenLoading = value;
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMapSerializer_IgnoreUnknownXmlAttributes")]
		public bool IgnoreUnknownXmlAttributes
		{
			get
			{
				return serializer.IgnoreUnknownAttributes;
			}
			set
			{
				serializer.IgnoreUnknownAttributes = value;
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeMapSerializer_TemplateMode")]
		public bool TemplateMode
		{
			get
			{
				return serializer.TemplateMode;
			}
			set
			{
				serializer.TemplateMode = value;
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapSerializer_SerializableContent")]
		public string SerializableContent
		{
			get
			{
				return serializer.SerializableContent;
			}
			set
			{
				serializer.SerializableContent = value;
			}
		}

		[SRCategory("CategoryAttribute_Misc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeMapSerializer_NonSerializableContent")]
		public string NonSerializableContent
		{
			get
			{
				return serializer.NonSerializableContent;
			}
			set
			{
				serializer.NonSerializableContent = value;
			}
		}

		private MapSerializer()
		{
		}

		public MapSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("Valid Service Container object must be provided");
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(MapSerializer))
			{
				return this;
			}
			throw new ArgumentException("The map serializer does not provide service of type: " + serviceType.ToString());
		}

		public void Reset()
		{
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.ResetObjectProperties(GetMapObject());
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
		}

		public void Save(string fileName)
		{
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Serialize(GetMapObject(), fileName);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
		}

		public void Save(Stream stream)
		{
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Serialize(GetMapObject(), stream);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
		}

		public void Save(XmlDocument document)
		{
			if (serializer is XmlFormatSerializer)
			{
				if (GetMapObject() != null)
				{
					GetMapObject().Serializing = true;
				}
				(serializer as XmlFormatSerializer).Serialize(GetMapObject(), document);
				if (GetMapObject() != null)
				{
					GetMapObject().Serializing = false;
				}
			}
		}

		public void Save(XmlWriter writer)
		{
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Serialize(GetMapObject(), writer);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
		}

		public void Save(TextWriter writer)
		{
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Serialize(GetMapObject(), writer);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
		}

		public void Load(string fileName)
		{
			GetMapObject().BeginInit();
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Deserialize(GetMapObject(), fileName);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
			GetMapObject().EndInit();
		}

		public void Load(Stream stream)
		{
			GetMapObject().BeginInit();
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Deserialize(GetMapObject(), stream);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
			GetMapObject().EndInit();
		}

		public void Load(XmlReader reader)
		{
			GetMapObject().BeginInit();
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Deserialize(GetMapObject(), reader);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
			GetMapObject().EndInit();
		}

		public void Load(TextReader reader)
		{
			GetMapObject().BeginInit();
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = true;
			}
			serializer.Deserialize(GetMapObject(), reader);
			if (GetMapObject() != null)
			{
				GetMapObject().Serializing = false;
			}
			GetMapObject().EndInit();
		}

		protected void SetSerializableConentFromFlags()
		{
			SerializableContent = "";
			NonSerializableContent = "";
			foreach (object value in Enum.GetValues(typeof(SerializationContent)))
			{
				if (!(value is SerializationContent))
				{
					continue;
				}
				SerializationContent serializationContent = (SerializationContent)value;
				if ((Content & serializationContent) == serializationContent && serializationContent != SerializationContent.All && Content != SerializationContent.All)
				{
					if (!string.IsNullOrEmpty(NonSerializableContent))
					{
						NonSerializableContent += ", ";
					}
					NonSerializableContent += GetFlagContentString(serializationContent, serializable: false);
					NonSerializableContent = NonSerializableContent.TrimStart(',');
					if (!string.IsNullOrEmpty(SerializableContent))
					{
						SerializableContent += ", ";
					}
					SerializableContent += GetFlagContentString(serializationContent, serializable: true);
					SerializableContent = SerializableContent.TrimStart(',');
				}
			}
		}

		internal string GetFlagContentString(SerializationContent flag, bool serializable)
		{
			switch (flag)
			{
			case SerializationContent.All:
				return "";
			case SerializationContent.Appearance:
				if (serializable)
				{
					return "MapCore.BuildNumber, *.Name, *.LedDimColor, *.NeedleStyle, *.Cap*, CircularPointer.Type, LinearPointer.Type, *.Shape, *.Fill*, *.Frame*, *.Back*, *.Border*, *.SeparatorColor, *.DecimalColor, *.DigitColor, *.TextColor, *.Color, *.Shadow*, *.AntiAliasing, *.GlassEffect, *.FontColor";
				}
				return "";
			default:
				throw new InvalidOperationException("Serializer - Unsupported serialization content flag.");
			}
		}

		internal MapCore GetMapObject()
		{
			if (mapObject == null)
			{
				mapObject = (MapCore)serviceContainer.GetService(typeof(MapCore));
			}
			return mapObject;
		}
	}
}
