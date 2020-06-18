using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.ReportProcessing;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal sealed class Image : ReportItem, IImage, IDeepCloneable
	{
		public enum Sizings
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		internal enum SourceType
		{
			External,
			Embedded,
			Database
		}

		private ImageBase m_internalImage;

		private ActionInfo m_actionInfo;

		private ImageMapAreasCollection m_imageMap;

		public byte[] ImageData
		{
			get
			{
				if (base.IsCustomControl)
				{
					return Processing.m_imageData;
				}
				return Rendering.ImageData;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.m_imageData = value;
			}
		}

		public string MIMEType
		{
			get
			{
				if (base.IsCustomControl)
				{
					return Processing.m_mimeType;
				}
				return Rendering.MIMEType;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (value == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "MimeType");
				}
				if (!Validator.ValidateMimeType(value))
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidMimeType, value);
				}
				Processing.m_mimeType = value;
			}
		}

		public string StreamName
		{
			get
			{
				if (base.IsCustomControl)
				{
					return null;
				}
				return Rendering.StreamName;
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].HyperLinkURL;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughReport;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].DrillthroughParameters;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = ActionInfo;
				}
				return actionInfo?.Actions[0].BookmarkLink;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = m_actionInfo;
				if (!base.IsCustomControl && actionInfo == null)
				{
					Microsoft.ReportingServices.ReportProcessing.Action action = ((Microsoft.ReportingServices.ReportProcessing.Image)base.ReportItemDef).Action;
					if (action != null)
					{
						Microsoft.ReportingServices.ReportProcessing.ActionInstance actionInstance = null;
						string ownerUniqueName = base.UniqueName;
						if (base.ReportItemInstance != null)
						{
							actionInstance = ((ImageInstanceInfo)base.InstanceInfo).Action;
							if (base.RenderingContext.InPageSection)
							{
								ownerUniqueName = base.ReportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
						}
						else if (base.RenderingContext.InPageSection && m_intUniqueName != 0)
						{
							ownerUniqueName = m_intUniqueName.ToString(CultureInfo.InvariantCulture);
						}
						actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, base.RenderingContext);
						if (base.RenderingContext.CacheState)
						{
							m_actionInfo = actionInfo;
						}
					}
				}
				return actionInfo;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_actionInfo = value;
			}
		}

		public Sizings Sizing
		{
			get
			{
				if (base.IsCustomControl)
				{
					return Processing.m_sizing;
				}
				return (Sizings)((Microsoft.ReportingServices.ReportProcessing.Image)base.ReportItemDef).Sizing;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.m_sizing = value;
			}
		}

		public ImageMapAreasCollection ImageMap
		{
			get
			{
				if (base.IsCustomControl)
				{
					return m_imageMap;
				}
				ImageMapAreasCollection imageMapAreasCollection = m_imageMap;
				if (m_imageMap == null && Rendering.ImageMapAreaInstances != null)
				{
					imageMapAreasCollection = new ImageMapAreasCollection(Rendering.ImageMapAreaInstances, base.RenderingContext);
					if (base.RenderingContext.CacheState)
					{
						m_imageMap = imageMapAreasCollection;
					}
				}
				return imageMapAreasCollection;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_imageMap = value;
			}
		}

		private InternalImage Rendering
		{
			get
			{
				Global.Tracer.Assert(!base.IsCustomControl);
				InternalImage internalImage = m_internalImage as InternalImage;
				if (internalImage == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return internalImage;
			}
		}

		internal new ImageProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(base.IsCustomControl);
				ImageProcessing imageProcessing = m_internalImage as ImageProcessing;
				if (imageProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return imageProcessing;
			}
		}

		public Image(string definitionName, string instanceName)
			: base(definitionName, instanceName)
		{
			if (definitionName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "definitionName");
			}
			if (instanceName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "instanceName");
			}
			Global.Tracer.Assert(base.IsCustomControl && base.Processing != null);
			m_internalImage = new ImageProcessing();
		}

		internal Image(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.Image reportItemDef, Microsoft.ReportingServices.ReportProcessing.ImageInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			ImageInstanceInfo imageInstanceInfo = (ImageInstanceInfo)base.InstanceInfo;
			string mimeType = null;
			if (reportItemDef.Source == Microsoft.ReportingServices.ReportProcessing.Image.SourceType.Database && reportItemDef.MIMEType.Type == ExpressionInfo.Types.Constant)
			{
				mimeType = reportItemDef.MIMEType.Value;
			}
			m_internalImage = new InternalImage((SourceType)reportItemDef.Source, mimeType, (imageInstanceInfo != null) ? imageInstanceInfo.ValueObject : reportItemDef.Value.Value, renderingContext, imageInstanceInfo?.BrokenImage ?? false, imageInstanceInfo?.ImageMapAreas);
		}

		private Image()
		{
		}

		ReportItem IDeepCloneable.DeepClone()
		{
			if (!base.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Image image = new Image();
			DeepClone(image);
			Global.Tracer.Assert(m_internalImage != null && m_internalImage is ImageProcessing);
			image.m_internalImage = Processing.DeepClone();
			if (m_actionInfo != null)
			{
				image.m_actionInfo = m_actionInfo.DeepClone();
			}
			if (m_imageMap != null)
			{
				image.m_imageMap = m_imageMap.DeepClone();
			}
			return image;
		}
	}
}
