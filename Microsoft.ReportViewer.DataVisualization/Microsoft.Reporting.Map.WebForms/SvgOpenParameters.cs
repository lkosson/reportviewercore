namespace Microsoft.Reporting.Map.WebForms
{
	internal struct SvgOpenParameters
	{
		public bool toolTipsEnabled;

		public bool resizable;

		public bool preserveAspectRatio;

		public SvgOpenParameters(bool toolTipsEnabled, bool resizable, bool preserveAspectRatio)
		{
			this.toolTipsEnabled = toolTipsEnabled;
			this.resizable = resizable;
			this.preserveAspectRatio = preserveAspectRatio;
		}
	}
}
