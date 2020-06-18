using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class GlobalStyle : Style
	{
		private readonly IStyleModel mModel;

		internal GlobalStyle(IStyleModel model)
			: base(model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is GlobalStyle))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((GlobalStyle)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
