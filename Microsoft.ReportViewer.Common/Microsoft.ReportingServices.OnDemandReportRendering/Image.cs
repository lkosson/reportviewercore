using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal sealed class Image : ReportItem, IImage, IROMActionOwner, IBaseImage
	{
		public enum Sizings
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		public enum SourceType
		{
			External,
			Embedded,
			Database
		}

		internal enum EmbeddingModes
		{
			Inline,
			Package
		}

		private Microsoft.ReportingServices.ReportRendering.Image m_renderImage;

		private ReportStringProperty m_value;

		private ReportStringProperty m_mimeType;

		private TagCollection m_tags;

		private ActionInfo m_actionInfo;

		public SourceType Source
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (SourceType)((Microsoft.ReportingServices.ReportProcessing.Image)m_renderReportItem.ReportItemDef).Source;
				}
				return ImageDef.Source;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (m_value == null)
				{
					if (m_isOldSnapshot)
					{
						m_value = new ReportStringProperty(((Microsoft.ReportingServices.ReportProcessing.Image)m_renderReportItem.ReportItemDef).Value);
					}
					else
					{
						m_value = new ReportStringProperty(ImageDef.Value);
					}
				}
				return m_value;
			}
		}

		public ReportStringProperty MIMEType
		{
			get
			{
				if (m_mimeType == null)
				{
					if (m_isOldSnapshot)
					{
						m_mimeType = new ReportStringProperty(((Microsoft.ReportingServices.ReportProcessing.Image)m_renderReportItem.ReportItemDef).MIMEType);
					}
					else
					{
						m_mimeType = new ReportStringProperty(ImageDef.MIMEType);
					}
				}
				return m_mimeType;
			}
		}

		public ReportVariantProperty Tag
		{
			get
			{
				TagCollection tags = Tags;
				if (tags == null)
				{
					return new ReportVariantProperty(isExpression: false);
				}
				return tags[0].Value;
			}
		}

		internal TagCollection Tags
		{
			get
			{
				if (m_tags == null && !m_isOldSnapshot && ImageDef.Tags != null)
				{
					m_tags = new TagCollection(this);
				}
				return m_tags;
			}
		}

		internal bool IsNullImage
		{
			get
			{
				if (Value.IsExpression)
				{
					return false;
				}
				return string.IsNullOrEmpty(Value.Value);
			}
		}

		public Sizings Sizing
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (Sizings)((Microsoft.ReportingServices.ReportRendering.Image)m_renderReportItem).Sizing;
				}
				return ImageDef.Sizing;
			}
			set
			{
				if (base.CriGenerationPhase != CriGenerationPhases.Definition)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				ImageDef.Sizing = value;
			}
		}

		string IROMActionOwner.UniqueName => m_reportItemDef.UniqueName;

		public ActionInfo ActionInfo
		{
			get
			{
				if (m_actionInfo == null)
				{
					if (m_isOldSnapshot)
					{
						if (((Microsoft.ReportingServices.ReportRendering.Image)m_renderReportItem).ActionInfo != null)
						{
							m_actionInfo = new ActionInfo(base.RenderingContext, ((Microsoft.ReportingServices.ReportRendering.Image)m_renderReportItem).ActionInfo);
						}
					}
					else
					{
						Microsoft.ReportingServices.ReportIntermediateFormat.Action action = ImageDef.Action;
						if (action != null)
						{
							m_actionInfo = new ActionInfo(base.RenderingContext, ReportScope, action, m_reportItemDef, this, m_reportItemDef.ObjectType, m_reportItemDef.Name, this);
						}
					}
				}
				return m_actionInfo;
			}
		}

		public ImageInstance ImageInstance => (ImageInstance)base.Instance;

		internal Microsoft.ReportingServices.ReportIntermediateFormat.Image ImageDef => (Microsoft.ReportingServices.ReportIntermediateFormat.Image)m_reportItemDef;

		List<string> IROMActionOwner.FieldsUsedInValueExpression => ((ImageInstance)GetOrCreateInstance()).GetFieldsUsedInValueExpression();

		ObjectType IBaseImage.ObjectType => ObjectType.Image;

		string IBaseImage.ObjectName => Name;

		ReportProperty IBaseImage.Value => Value;

		string IBaseImage.ImageDataPropertyName => "ImageData";

		string IBaseImage.ImageValuePropertyName => "Value";

		string IBaseImage.MIMETypePropertyName => "MIMEType";

		EmbeddingModes IBaseImage.EmbeddingMode
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return EmbeddingModes.Inline;
				}
				return ImageDef.EmbeddingMode;
			}
		}

		internal Image(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.Image reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Image(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.Image renderImage, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderImage, renderingContext)
		{
			m_renderImage = renderImage;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (m_instance == null)
			{
				if (base.CriOwner != null)
				{
					m_instance = new CriImageInstance(this);
				}
				else if (base.IsOldSnapshot)
				{
					m_instance = new ShimImageInstance(this);
				}
				else
				{
					m_instance = new InternalImageInstance(this);
				}
			}
			return m_instance;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Image obj = (Microsoft.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef;
			bool flag = obj.ShouldTrackFieldsUsedInValue();
			fieldsUsedInValue = null;
			if (flag)
			{
				base.RenderingContext.OdpContext.ReportObjectModel.ResetFieldsUsedInExpression();
			}
			byte[] result = obj.EvaluateBinaryValueExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext, out errorOccurred);
			if (errorOccurred)
			{
				return null;
			}
			if (flag)
			{
				fieldsUsedInValue = new List<string>();
				base.RenderingContext.OdpContext.ReportObjectModel.AddFieldsUsedInExpression(fieldsUsedInValue);
			}
			return result;
		}

		string IBaseImage.GetMIMETypeValue()
		{
			return ((Microsoft.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef).EvaluateMimeTypeExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			Microsoft.ReportingServices.ReportIntermediateFormat.Image obj = (Microsoft.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef;
			bool flag = obj.ShouldTrackFieldsUsedInValue();
			fieldsUsedInValue = null;
			if (flag)
			{
				base.RenderingContext.OdpContext.ReportObjectModel.ResetFieldsUsedInExpression();
			}
			string result = obj.EvaluateStringValueExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext, out errOccurred);
			if (errOccurred)
			{
				return null;
			}
			if (flag)
			{
				fieldsUsedInValue = new List<string>();
				base.RenderingContext.OdpContext.ReportObjectModel.AddFieldsUsedInExpression(fieldsUsedInValue);
			}
			return result;
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			InternalImageInstance internalImageInstance = ImageInstance as InternalImageInstance;
			Global.Tracer.Assert(internalImageInstance != null, "GetTransparentImageProperties may only be called from the ODP engine.");
			return internalImageInstance.LoadAndCacheTransparentImage(out mimeType, out imageData);
		}

		internal override void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (m_actionInfo != null)
			{
				m_actionInfo.Update(((Microsoft.ReportingServices.ReportRendering.Image)m_renderReportItem).ActionInfo);
			}
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (m_actionInfo != null)
			{
				m_actionInfo.SetNewContext();
			}
			if (m_tags != null)
			{
				m_tags.SetNewContext();
			}
		}

		internal override void ConstructReportItemDefinition()
		{
			ConstructReportItemDefinitionImpl();
			ImageInstance imageInstance = ImageInstance;
			Global.Tracer.Assert(imageInstance != null, "(instance != null)");
			if (imageInstance.MIMEType != null)
			{
				ImageDef.MIMEType = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(imageInstance.MIMEType);
			}
			else
			{
				ImageDef.MIMEType = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_mimeType = null;
			if (imageInstance.ImageData != null || imageInstance.StreamName != null)
			{
				Global.Tracer.Assert(condition: false, "Runtime construction of images with constant Image.Value is not supported.");
			}
			else
			{
				ImageDef.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_value = null;
			if (!ActionInfo.ConstructActionDefinition())
			{
				ImageDef.Action = null;
				m_actionInfo = null;
			}
		}

		internal override void CompleteCriGeneratedInstanceEvaluation()
		{
			Global.Tracer.Assert(base.CriGenerationPhase == CriGenerationPhases.Instance, "(CriGenerationPhase == CriGenerationPhases.Instance)");
			ImageInstance imageInstance = ImageInstance;
			Global.Tracer.Assert(imageInstance != null, "(instance != null)");
			if (imageInstance.ActionInfoWithDynamicImageMapAreas != null)
			{
				base.CriGenerationPhase = CriGenerationPhases.Definition;
				imageInstance.ActionInfoWithDynamicImageMapAreas.ConstructDefinitions();
				base.CriGenerationPhase = CriGenerationPhases.Instance;
			}
		}
	}
}
