using System.ComponentModel;
using System.Drawing;

namespace Microsoft.Reporting.Map.WebForms
{
	[Description("Image with a unique name, saved as a resource.")]
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
		{
			Name = name;
			this.image = image;
		}

		public override string ToString()
		{
			return Name;
		}

		internal override void Invalidate()
		{
			if (Common != null)
			{
				Common.MapCore.InvalidateAndLayout();
			}
		}
	}
}
