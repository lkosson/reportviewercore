namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLActionInfoWithImageMap : RPLActionInfo
	{
		private RPLImageMapCollection m_imageMaps;

		public RPLImageMapCollection ImageMaps
		{
			get
			{
				return m_imageMaps;
			}
			set
			{
				m_imageMaps = value;
			}
		}

		internal RPLActionInfoWithImageMap()
		{
		}

		internal RPLActionInfoWithImageMap(int actionCount)
			: base(actionCount)
		{
		}
	}
}
