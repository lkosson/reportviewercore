using Microsoft.ReportingServices.ReportIntermediateFormat;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalBackgroundImageInstance : BackgroundImageInstance
	{
		private bool m_backgroundRepeatEvaluated;

		private BackgroundRepeatTypes m_backgroundRepeat = Style.DefaultEnumBackgroundRepeatType;

		private bool m_positionEvaluated;

		private Positions m_position;

		private bool m_transparentColorEvaluated;

		private ReportColor m_transparentColor;

		private readonly ImageDataHandler m_imageDataHandler;

		private readonly BackgroundImage m_backgroundImageDef;

		public override byte[] ImageData => m_imageDataHandler.ImageData;

		public override string StreamName => m_imageDataHandler.StreamName;

		public override string MIMEType => m_imageDataHandler.MIMEType;

		public override BackgroundRepeatTypes BackgroundRepeat
		{
			get
			{
				if (!m_backgroundRepeatEvaluated)
				{
					m_backgroundRepeatEvaluated = true;
					m_backgroundRepeat = (BackgroundRepeatTypes)m_backgroundImageDef.StyleDef.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundImageRepeat);
				}
				return m_backgroundRepeat;
			}
		}

		public override Positions Position
		{
			get
			{
				if (!m_positionEvaluated)
				{
					m_positionEvaluated = true;
					m_position = (Positions)m_backgroundImageDef.StyleDef.EvaluateInstanceStyleEnum(StyleAttributeNames.Position);
				}
				return m_position;
			}
		}

		public override ReportColor TransparentColor
		{
			get
			{
				if (!m_transparentColorEvaluated)
				{
					m_transparentColorEvaluated = true;
					m_transparentColor = m_backgroundImageDef.StyleDef.EvaluateInstanceReportColor(StyleAttributeNames.TransparentColor);
				}
				return m_transparentColor;
			}
		}

		internal InternalBackgroundImageInstance(BackgroundImage backgroundImageDef)
			: base(backgroundImageDef.StyleDef.ReportScope)
		{
			m_backgroundImageDef = backgroundImageDef;
			m_imageDataHandler = ImageDataHandlerFactory.Create(m_backgroundImageDef.StyleDef.ReportElement, backgroundImageDef);
		}

		protected override void ResetInstanceCache()
		{
			m_backgroundRepeatEvaluated = false;
			m_positionEvaluated = false;
			m_transparentColorEvaluated = false;
			m_transparentColor = null;
			m_imageDataHandler.ClearCache();
		}
	}
}
