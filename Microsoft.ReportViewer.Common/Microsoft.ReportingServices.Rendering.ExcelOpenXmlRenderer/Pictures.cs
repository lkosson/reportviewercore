using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using System.IO;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Pictures
	{
		private readonly IPictureShapesModel mModel;

		internal Pictures(IPictureShapesModel model)
		{
			mModel = model;
		}

		public Picture CreatePicture(string uniqueId, string extension, Stream pictureStream, Anchor startPosition, Anchor endPosition)
		{
			return mModel.CreatePicture(uniqueId, extension, pictureStream, startPosition.Model, endPosition.Model).Interface;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Pictures))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((Pictures)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
