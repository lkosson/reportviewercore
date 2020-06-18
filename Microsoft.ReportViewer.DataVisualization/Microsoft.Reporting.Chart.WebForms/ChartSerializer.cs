using Microsoft.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeChartSerializer_ChartSerializer")]
	[DefaultProperty("Format")]
	internal class ChartSerializer : IServiceProvider
	{
		private IServiceContainer serviceContainer;

		private Chart chart;

		private SerializerBase serializer = new XmlFormatSerializer();

		private SerializationFormat format;

		private SerializationContents content = SerializationContents.Default;

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(typeof(SerializationContents), "Default")]
		[SRDescription("DescriptionAttributeChartSerializer_Content")]
		public SerializationContents Content
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(typeof(SerializationFormat), "Xml")]
		[SRDescription("DescriptionAttributeChartSerializer_Format")]
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChartSerializer_ResetWhenLoading")]
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartSerializer_IgnoreUnknownXmlAttributes")]
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue(false)]
		[SRDescription("DescriptionAttributeChartSerializer_TemplateMode")]
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeChartSerializer_SerializableContent")]
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

		[SRCategory("CategoryAttributeMisc")]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeChartSerializer_NonSerializableContent")]
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

		private ChartSerializer()
		{
		}

		public ChartSerializer(IServiceContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException(SR.ExceptionInvalidServiceContainer);
			}
			serviceContainer = container;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ChartSerializer))
			{
				return this;
			}
			throw new ArgumentException(SR.ExceptionChartSerializerUnsupportedType(serviceType.ToString()));
		}

		public void Reset()
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Resetting;
			}
			serializer.ResetObjectProperties(GetChartObject());
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(string fileName)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			GetChartObject().ResetAutoValues();
			serializer.Serialize(GetChartObject(), fileName);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(Stream stream)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			GetChartObject().ResetAutoValues();
			serializer.Serialize(GetChartObject(), stream);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(XmlWriter writer)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			GetChartObject().ResetAutoValues();
			serializer.Serialize(GetChartObject(), writer);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Save(TextWriter writer)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Saving;
			}
			GetChartObject().ResetAutoValues();
			serializer.Serialize(GetChartObject(), writer);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(string fileName)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			serializer.Deserialize(GetChartObject(), fileName);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(Stream stream)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			serializer.Deserialize(GetChartObject(), stream);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(XmlReader reader)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			serializer.Deserialize(GetChartObject(), reader);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		public void Load(TextReader reader)
		{
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = true;
				GetChartObject().serializationStatus = SerializationStatus.Loading;
			}
			serializer.Deserialize(GetChartObject(), reader);
			if (GetChartObject() != null)
			{
				GetChartObject().serializing = false;
				GetChartObject().serializationStatus = SerializationStatus.None;
			}
		}

		protected void SetSerializableContentFromFlags()
		{
			SerializableContent = "";
			NonSerializableContent = "";
			foreach (object value in Enum.GetValues(typeof(SerializationContents)))
			{
				if (!(value is SerializationContents))
				{
					continue;
				}
				SerializationContents serializationContents = (SerializationContents)value;
				if ((Content & serializationContents) == serializationContents && serializationContents != SerializationContents.All && Content != SerializationContents.All)
				{
					if (NonSerializableContent.Length != 0)
					{
						NonSerializableContent += ", ";
					}
					NonSerializableContent += GetFlagContentString(serializationContents, serializable: false);
					NonSerializableContent = NonSerializableContent.TrimStart(',');
					if (SerializableContent.Length != 0)
					{
						SerializableContent += ", ";
					}
					SerializableContent += GetFlagContentString(serializationContents, serializable: true);
					SerializableContent = SerializableContent.TrimStart(',');
				}
			}
		}

		protected string GetFlagContentString(SerializationContents flag, bool serializable)
		{
			switch (flag)
			{
			case SerializationContents.All:
				return "";
			case SerializationContents.Default:
				return "";
			case SerializationContents.Data:
				if (serializable)
				{
					return "Chart.BuildNumber, Chart.Series, Series.Points, Series.Name, DataPoint.XValue, DataPoint.YValues,DataPoint.Label,DataPoint.AxisLabel,DataPoint.LabelFormat,DataPoint.Empty, Series.YValuesPerPoint, Series.XValueIndexed, Series.XValueType, Series.YValueType";
				}
				return "";
			case SerializationContents.Appearance:
				if (serializable)
				{
					return "Chart.BuildNumber, *.Name, *.Back*, *.Border*, *.Line*, *.Frame*, *.PageColor*, *.SkinStyle*, *.Palette, *.PaletteCustomColors, *.Font*, *.*Font, *.Color, *.Shadow*, *.MarkerColor, *.MarkerStyle, *.MarkerSize, *.MarkerBorderColor, *.MarkerImage, *.MarkerImageTransparentColor, *.LabelBackColor, *.LabelBorder*, *.Enable3D, *.RightAngleAxes, *.Clustered, *.Light, *.Perspective, *.XAngle, *.YAngle, *.PointDepth, *.PointGapDepth, *.WallWidth";
				}
				return "";
			default:
				throw new InvalidOperationException(SR.ExceptionChartSerializerContentFlagUnsupported);
			}
		}

		internal Chart GetChartObject()
		{
			if (chart == null)
			{
				chart = (Chart)serviceContainer.GetService(typeof(Chart));
			}
			return chart;
		}
	}
}
