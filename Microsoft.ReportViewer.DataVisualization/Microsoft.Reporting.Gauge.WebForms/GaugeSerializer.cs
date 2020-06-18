using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[SRDescription("DescriptionAttributeGaugeSerializer_GaugeSerializer")]
	[DefaultProperty("Format")]
	internal class GaugeSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private GaugeCore gaugeObject;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContent content = SerializationContent.All;

		[SRCategory("CategoryMisc")]
		[DefaultValue(typeof(SerializationContent), "All")]
		[SRDescription("DescriptionAttributeGaugeSerializer_Content")]
		public SerializationContent Content
		{
			get
			{
				return content;
			}
			set
			{
				content = value;
				SetSerializableContentFromFlags();
			}
		}

		[SRCategory("CategoryMisc")]
		[DefaultValue(typeof(SerializationFormat), "Xml")]
		[SRDescription("DescriptionAttributeGaugeSerializer_Format")]
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

		[SRCategory("CategoryMisc")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeGaugeSerializer_ResetWhenLoading")]
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

		[SRCategory("CategoryMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeGaugeSerializer_IgnoreUnknownXmlAttributes")]
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

		[SRCategory("CategoryMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeGaugeSerializer_TemplateMode")]
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

		[SRCategory("CategoryMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeSerializer_SerializableContent")]
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

		[SRCategory("CategoryMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeGaugeSerializer_NonSerializableContent")]
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

		private GaugeSerializer()
		{
		}

		public GaugeSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container", Utils.SRGetStr("ExceptionInvalidServiceContainer"));
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(GaugeSerializer))
			{
				return this;
			}
			throw new ArgumentException(Utils.SRGetStr("ExceptionSerializerMissingSerivice", serviceType.ToString()), "serviceType");
		}

		public void Reset()
		{
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			serializer.ResetObjectProperties(GetGaugeObject());
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
		}

		public void Save(string fileName)
		{
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			GetGaugeObject().ResetAutoValues();
			serializer.Serialize(GetGaugeObject(), fileName);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
		}

		public void Save(Stream stream)
		{
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			GetGaugeObject().ResetAutoValues();
			serializer.Serialize(GetGaugeObject(), stream);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
		}

		public void Save(XmlWriter writer)
		{
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			GetGaugeObject().ResetAutoValues();
			serializer.Serialize(GetGaugeObject(), writer);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
		}

		public void Save(TextWriter writer)
		{
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			GetGaugeObject().ResetAutoValues();
			serializer.Serialize(GetGaugeObject(), writer);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
		}

		public void Load(string fileName)
		{
			GetGaugeObject().BeginInit();
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			serializer.Deserialize(GetGaugeObject(), fileName);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
			GetGaugeObject().EndInit();
		}

		public void Load(Stream stream)
		{
			GetGaugeObject().BeginInit();
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			serializer.Deserialize(GetGaugeObject(), stream);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
			if (GetGaugeObject().Values.GetByName("DataValue") == null)
			{
				GetGaugeObject().Values.Add("DataValue");
			}
			GetGaugeObject().EndInit();
		}

		public void Load(XmlReader reader)
		{
			GetGaugeObject().BeginInit();
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			serializer.Deserialize(GetGaugeObject(), reader);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
			GetGaugeObject().EndInit();
		}

		public void Load(TextReader reader)
		{
			GetGaugeObject().BeginInit();
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = true;
			}
			serializer.Deserialize(GetGaugeObject(), reader);
			if (GetGaugeObject() != null)
			{
				GetGaugeObject().Serializing = false;
			}
			GetGaugeObject().EndInit();
		}

		protected void SetSerializableContentFromFlags()
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
					if (NonSerializableContent.Length != 0)
					{
						NonSerializableContent += ", ";
					}
					NonSerializableContent += GetFlagContentString(serializationContent, serializable: false);
					NonSerializableContent = NonSerializableContent.TrimStart(',');
					if (SerializableContent.Length != 0)
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
					return "GaugeCore.BuildNumber, *.Name, *.LedDimColor, *.NeedleStyle, *.Cap*, CircularPointer.Type, LinearPointer.Type, *.Shape, *.FrameShape, *.Fill*, *.BackFrame*, *.Frame*, *.Back*, *.Border*, *.SeparatorColor, *.DecimalColor, *.DigitColor, *.TextColor, *.Color, *.Shadow*, *.AntiAliasing, *.GlassEffect, *.FontColor";
				}
				return "";
			default:
				throw new InvalidOperationException(Utils.SRGetStr("ExceptionSerializerInvalidFlag"));
			}
		}

		internal GaugeCore GetGaugeObject()
		{
			if (gaugeObject == null)
			{
				gaugeObject = (GaugeCore)serviceContainer.GetService(typeof(GaugeCore));
			}
			return gaugeObject;
		}
	}
}
