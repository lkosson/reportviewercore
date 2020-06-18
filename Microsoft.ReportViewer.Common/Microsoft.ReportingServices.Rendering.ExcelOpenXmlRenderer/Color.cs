using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using Microsoft.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Color : IColor
	{
		private readonly ColorModel mModel;

		public byte Blue => (byte)mModel.getBlue();

		public byte Green => (byte)mModel.getGreen();

		internal ColorModel Model => mModel;

		public byte Red => (byte)mModel.getRed();

		internal Color(ColorModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Color))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Color)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
