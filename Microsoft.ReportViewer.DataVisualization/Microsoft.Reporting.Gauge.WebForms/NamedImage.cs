using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Gauge.WebForms
{
	[SRDescription("DescriptionAttributeNamedImage_NamedImage")]
	[DefaultProperty("Name")]
	[TypeConverter(typeof(NamedImageConverter))]
	internal class NamedImage : NamedElement
	{
		private Image image;

		[SRDescription("DescriptionAttributeNamedImage_Image")]
		public Image Image
		{
			get
			{
				return image;
			}
			set
			{
				image = value;
			}
		}

		public NamedImage()
		{
		}

		public NamedImage(string name, Image image)
			: base(name)
		{
			this.image = image;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
