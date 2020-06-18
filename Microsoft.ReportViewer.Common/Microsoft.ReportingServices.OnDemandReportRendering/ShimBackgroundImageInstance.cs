using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimBackgroundImageInstance : BackgroundImageInstance
	{
		private readonly Microsoft.ReportingServices.ReportRendering.BackgroundImage m_renderImage;

		private readonly BackgroundRepeatTypes m_backgroundRepeat;

		private readonly BackgroundImage m_backgroundImageDef;

		public override byte[] ImageData => m_renderImage.ImageData;

		public override string StreamName => m_renderImage.StreamName;

		public override string MIMEType => m_renderImage.MIMEType;

		public override BackgroundRepeatTypes BackgroundRepeat => m_backgroundRepeat;

		public override Positions Position => m_backgroundImageDef.Position.Value;

		public override ReportColor TransparentColor => m_backgroundImageDef.TransparentColor.Value;

		internal ShimBackgroundImageInstance(BackgroundImage backgroundImageDef, Microsoft.ReportingServices.ReportRendering.BackgroundImage renderImage, string backgroundRepeat)
			: base(null)
		{
			m_backgroundImageDef = backgroundImageDef;
			m_renderImage = renderImage;
			m_backgroundRepeat = StyleTranslator.TranslateBackgroundRepeat(backgroundRepeat, null, m_backgroundImageDef.StyleDef.IsDynamicImageStyle);
		}

		protected override void ResetInstanceCache()
		{
		}
	}
}
