using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.Drawing;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Palette
	{
		public const int MaxColorIndex = 55;

		public const int MaxRGBValue = 255;

		public const int MinColorIndex = 0;

		public const int MinRGBValue = 0;

		private readonly IPaletteModel mModel;

		internal Palette(IPaletteModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Palette))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Palette)obj).mModel.Equals(mModel);
		}

		public Color GetColor(System.Drawing.Color color)
		{
			return mModel.getColor(color.R, color.G, color.B).Interface;
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}

		public void SetColorAt(int index, int red, int green, int blue)
		{
			mModel.setColorAt(index, red, green, blue);
		}

		public int GetColorIndex(int red, int green, int blue)
		{
			return mModel.GetColorIndex(red, green, blue);
		}

		public Color GetColorAt(int index)
		{
			return mModel.GetColorAt(index).Interface;
		}
	}
}
