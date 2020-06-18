using Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class ColumnProperties
	{
		private readonly IColumnModel mModel;

		public bool Hidden
		{
			set
			{
				mModel.Hidden = value;
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				mModel.OutlineCollapsed = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				mModel.OutlineLevel = value;
			}
		}

		public double Width
		{
			set
			{
				mModel.Width = value;
			}
		}

		internal ColumnProperties(IColumnModel model)
		{
			mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is ColumnProperties))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((ColumnProperties)obj).mModel.Equals(mModel);
		}

		public override int GetHashCode()
		{
			return mModel.GetHashCode();
		}
	}
}
