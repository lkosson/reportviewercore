namespace Microsoft.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model
{
	internal abstract class ColorModel
	{
		private Color mInterface;

		public Color Interface
		{
			get
			{
				if (mInterface == null)
				{
					mInterface = new Color(this);
				}
				return mInterface;
			}
		}

		public abstract int getRed();

		public abstract int getBlue();

		public abstract int getGreen();

		public override bool Equals(object aObject)
		{
			if (aObject is ColorModel)
			{
				ColorModel colorModel = (ColorModel)aObject;
				if (getRed() == colorModel.getRed() && getGreen() == colorModel.getGreen())
				{
					return getBlue() == colorModel.getBlue();
				}
				return false;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
