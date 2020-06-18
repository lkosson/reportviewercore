namespace Microsoft.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLImage : RPLItem
	{
		internal RPLImage()
		{
			m_rplElementProps = new RPLImageProps();
			m_rplElementProps.Definition = new RPLImagePropsDef();
		}

		internal RPLImage(long startOffset, RPLContext context)
			: base(startOffset, context)
		{
		}
	}
}
