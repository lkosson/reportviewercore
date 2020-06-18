using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Picture
	{
		private readonly IShapeModel mModel;

		public string Hyperlink
		{
			set
			{
				mModel.Hyperlink = value;
			}
		}

		internal Picture(IShapeModel model)
		{
			mModel = model;
		}

		public void UpdateColumnOffset(double sizeInPoints, bool start)
		{
			mModel.UpdateColumnOffset(sizeInPoints, start);
		}

		public void UpdateRowOffset(double sizeInPoints, bool start)
		{
			mModel.UpdateRowOffset(sizeInPoints, start);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Picture))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Picture)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
