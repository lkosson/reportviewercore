using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Anchor
	{
		private readonly AnchorModel mModel;

		internal AnchorModel Model => mModel;

		internal Anchor(AnchorModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Anchor))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Anchor)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
