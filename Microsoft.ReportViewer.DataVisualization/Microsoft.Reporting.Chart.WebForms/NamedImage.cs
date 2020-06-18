using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Chart.WebForms
{
	[SRDescription("DescriptionAttributeNamedImage_NamedImage")]
	[DefaultProperty("Name")]
	internal class NamedImage
	{
		private string name = string.Empty;

		private Image image;

		[Bindable(false)]
		[SRDescription("DescriptionAttributeNamedImage_Name")]
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		[Bindable(false)]
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
		{
			this.name = name;
			this.image = image;
		}
	}
}
