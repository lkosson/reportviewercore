using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.ReportingServices.RdlObjectModel
{
	internal class Textbox : ReportItem
	{
		internal new class Definition : DefinitionStore<Textbox, Definition.Properties>
		{
			internal enum Properties
			{
				Style,
				Name,
				ActionInfo,
				Top,
				Left,
				Height,
				Width,
				ZIndex,
				Visibility,
				ToolTip,
				ToolTipLocID,
				DocumentMapLabel,
				DocumentMapLabelLocID,
				Bookmark,
				RepeatWith,
				CustomProperties,
				DataElementName,
				DataElementOutput,
				CanGrow,
				CanShrink,
				HideDuplicates,
				ToggleImage,
				UserSort,
				DataElementStyle,
				KeepTogether,
				Paragraphs,
				PropertyCount
			}

			private Definition()
			{
			}
		}

		[DefaultValue(false)]
		public bool CanGrow
		{
			get
			{
				return base.PropertyStore.GetBoolean(18);
			}
			set
			{
				base.PropertyStore.SetBoolean(18, value);
			}
		}

		[DefaultValue(false)]
		public bool CanShrink
		{
			get
			{
				return base.PropertyStore.GetBoolean(19);
			}
			set
			{
				base.PropertyStore.SetBoolean(19, value);
			}
		}

		[DefaultValue("")]
		public string HideDuplicates
		{
			get
			{
				return (string)base.PropertyStore.GetObject(20);
			}
			set
			{
				base.PropertyStore.SetObject(20, value);
			}
		}

		public ToggleImage ToggleImage
		{
			get
			{
				return (ToggleImage)base.PropertyStore.GetObject(21);
			}
			set
			{
				base.PropertyStore.SetObject(21, value);
			}
		}

		public UserSort UserSort
		{
			get
			{
				return (UserSort)base.PropertyStore.GetObject(22);
			}
			set
			{
				base.PropertyStore.SetObject(22, value);
			}
		}

		[DefaultValue(DataElementStyles.Auto)]
		public DataElementStyles DataElementStyle
		{
			get
			{
				return (DataElementStyles)base.PropertyStore.GetInteger(23);
			}
			set
			{
				base.PropertyStore.SetInteger(23, (int)value);
			}
		}

		[DefaultValue(false)]
		public bool KeepTogether
		{
			get
			{
				return base.PropertyStore.GetBoolean(24);
			}
			set
			{
				base.PropertyStore.SetBoolean(24, value);
			}
		}

		[XmlElement(typeof(RdlCollection<Paragraph>))]
		public IList<Paragraph> Paragraphs
		{
			get
			{
				return (IList<Paragraph>)base.PropertyStore.GetObject(25);
			}
			set
			{
				base.PropertyStore.SetObject(25, value);
			}
		}

		public Textbox()
		{
		}

		internal Textbox(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			Paragraphs = new RdlCollection<Paragraph>();
			Paragraphs.Add(new Paragraph());
		}
	}
}
