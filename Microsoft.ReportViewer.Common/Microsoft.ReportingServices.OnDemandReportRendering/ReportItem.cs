using Microsoft.ReportingServices.ReportIntermediateFormat;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportRendering;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.ReportingServices.OnDemandReportRendering
{
	internal abstract class ReportItem : ReportElement
	{
		protected bool m_isListContentsRectangle;

		protected bool m_inSubtotal;

		protected string m_definitionPath;

		protected ReportStringProperty m_toolTip;

		protected ReportStringProperty m_bookmark;

		protected ReportStringProperty m_documentMapLabel;

		protected ReportItemInstance m_instance;

		private bool m_criGeneratedInstanceEvaluated;

		protected ReportBoolProperty m_startHidden;

		private Visibility m_visibility;

		private CustomPropertyCollection m_customProperties;

		private bool m_customPropertiesReady;

		private ReportSize m_cachedTop;

		private ReportSize m_cachedLeft;

		protected ReportSize m_cachedHeight;

		protected ReportSize m_cachedWidth;

		public override string DefinitionPath => m_definitionPath;

		public virtual string Name
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return m_renderReportItem.Name;
				}
				return base.ReportItemDef.Name;
			}
		}

		public override string ID
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_inSubtotal || m_isListContentsRectangle)
					{
						return DefinitionPath;
					}
					return m_renderReportItem.ID;
				}
				return base.ReportItemDef.RenderingModelID;
			}
		}

		public override Style Style
		{
			get
			{
				if (m_isOldSnapshot && m_isListContentsRectangle)
				{
					return new Style(this, m_renderingContext);
				}
				return base.Style;
			}
		}

		public virtual int LinkToChild => -1;

		public virtual ReportSize Height
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_cachedHeight == null)
					{
						m_cachedHeight = new ReportSize(RenderReportItem.Height);
					}
					return m_cachedHeight;
				}
				if (base.ReportItemDef.HeightForRendering == null)
				{
					base.ReportItemDef.HeightForRendering = new ReportSize(base.ReportItemDef.Height, base.ReportItemDef.HeightValue);
				}
				return base.ReportItemDef.HeightForRendering;
			}
		}

		public virtual ReportSize Width
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_cachedWidth == null)
					{
						m_cachedWidth = new ReportSize(RenderReportItem.Width);
					}
					return m_cachedWidth;
				}
				if (base.ReportItemDef.WidthForRendering == null)
				{
					base.ReportItemDef.WidthForRendering = new ReportSize(base.ReportItemDef.Width, base.ReportItemDef.WidthValue);
				}
				return base.ReportItemDef.WidthForRendering;
			}
		}

		public virtual ReportSize Top
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_cachedTop == null)
					{
						m_cachedTop = new ReportSize(RenderReportItem.Top);
					}
					return m_cachedTop;
				}
				if (base.ReportItemDef.TopForRendering == null)
				{
					base.ReportItemDef.TopForRendering = new ReportSize(base.ReportItemDef.Top, base.ReportItemDef.TopValue);
				}
				return base.ReportItemDef.TopForRendering;
			}
		}

		public virtual ReportSize Left
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_cachedLeft == null)
					{
						m_cachedLeft = new ReportSize(RenderReportItem.Left);
					}
					return m_cachedLeft;
				}
				if (base.ReportItemDef.LeftForRendering == null)
				{
					base.ReportItemDef.LeftForRendering = new ReportSize(base.ReportItemDef.Left, base.ReportItemDef.LeftValue);
				}
				return base.ReportItemDef.LeftForRendering;
			}
		}

		public int ZIndex
		{
			get
			{
				if (m_isOldSnapshot)
				{
					if (m_isListContentsRectangle)
					{
						return 0;
					}
					return RenderReportItem.ZIndex;
				}
				return base.ReportItemDef.ZIndex;
			}
		}

		public ReportStringProperty ToolTip
		{
			get
			{
				if (m_toolTip == null)
				{
					if (m_isOldSnapshot)
					{
						m_toolTip = new ReportStringProperty(RenderReportItem.ReportItemDef.ToolTip);
					}
					else
					{
						m_toolTip = new ReportStringProperty(m_reportItemDef.ToolTip);
					}
				}
				return m_toolTip;
			}
		}

		public ReportStringProperty Bookmark
		{
			get
			{
				if (m_bookmark == null)
				{
					if (m_isOldSnapshot)
					{
						m_bookmark = new ReportStringProperty(RenderReportItem.ReportItemDef.Bookmark);
					}
					else
					{
						m_bookmark = new ReportStringProperty(m_reportItemDef.Bookmark);
					}
				}
				return m_bookmark;
			}
		}

		public ReportStringProperty DocumentMapLabel
		{
			get
			{
				if (m_documentMapLabel == null)
				{
					if (m_isOldSnapshot)
					{
						m_documentMapLabel = new ReportStringProperty(RenderReportItem.ReportItemDef.Label);
					}
					else
					{
						m_documentMapLabel = new ReportStringProperty(m_reportItemDef.DocumentMapLabel);
					}
				}
				return m_documentMapLabel;
			}
		}

		public string RepeatWith
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderReportItem.ReportItemDef.RepeatWith;
				}
				return base.ReportItemDef.RepeatWith;
			}
		}

		public bool RepeatedSibling
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderReportItem.RepeatedSibling;
				}
				return base.ReportItemDef.RepeatedSibling;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				PrepareCustomProperties();
				return m_customProperties;
			}
		}

		public string DataElementName
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return RenderReportItem.DataElementName;
				}
				return base.ReportItemDef.DataElementName;
			}
		}

		public virtual DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (m_isOldSnapshot)
				{
					return (DataElementOutputTypes)RenderReportItem.DataElementOutput;
				}
				return base.ReportItemDef.DataElementOutput;
			}
		}

		public virtual Visibility Visibility
		{
			get
			{
				if (m_visibility == null)
				{
					if (m_isOldSnapshot && RenderReportItem.ReportItemDef.Visibility == null)
					{
						return null;
					}
					if (!m_isOldSnapshot && m_reportItemDef.Visibility == null)
					{
						return null;
					}
					m_visibility = new ReportItemVisibility(this);
				}
				return m_visibility;
			}
		}

		internal bool InSubtotal => m_inSubtotal;

		internal override ReportElementInstance ReportElementInstance => Instance;

		public new ReportItemInstance Instance
		{
			get
			{
				if (m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				ReportItemInstance orCreateInstance = GetOrCreateInstance();
				CriEvaluateInstance();
				return orCreateInstance;
			}
		}

		internal override string InstanceUniqueName
		{
			get
			{
				if (Instance != null)
				{
					return Instance.UniqueName;
				}
				return null;
			}
		}

		internal ReportItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, reportItemDef, renderingContext)
		{
			m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, indexIntoParentCollectionDef);
			m_reportItemDef.ROMScopeInstance = ReportScope.ReportScopeInstance;
		}

		internal ReportItem(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderReportItem, renderingContext)
		{
			m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, indexIntoParentCollectionDef);
			m_inSubtotal = inSubtotal;
		}

		internal ReportItem(IDefinitionPath parentDefinitionPath, bool inSubtotal, RenderingContext renderingContext)
			: base(parentDefinitionPath, renderingContext)
		{
			m_definitionPath = DefinitionPathConstants.GetCollectionDefinitionPath(parentDefinitionPath, 0);
			m_inSubtotal = inSubtotal;
			m_isListContentsRectangle = true;
		}

		internal abstract ReportItemInstance GetOrCreateInstance();

		internal void SetCachedWidth(double sizeDelta)
		{
			if (m_isOldSnapshot)
			{
				double sizeInMM = RenderReportItem.Width.ToMillimeters() + sizeDelta;
				string size = sizeInMM.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_cachedWidth = new ReportSize(size, sizeInMM);
			}
		}

		internal void SetCachedHeight(double sizeDelta)
		{
			if (m_isOldSnapshot)
			{
				double sizeInMM = RenderReportItem.Height.ToMillimeters() + sizeDelta;
				string size = sizeInMM.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_cachedHeight = new ReportSize(size, sizeInMM);
			}
		}

		internal override void SetNewContext()
		{
			if (m_instance != null)
			{
				m_instance.SetNewContext();
			}
			base.SetNewContext();
			m_criGeneratedInstanceEvaluated = false;
			if (m_reportItemDef != null)
			{
				m_reportItemDef.ResetVisibilityComputationCache();
			}
			m_customPropertiesReady = false;
		}

		internal override void SetNewContextChildren()
		{
		}

		internal void CriEvaluateInstance()
		{
			if (base.CriOwner != null && base.CriGenerationPhase == CriGenerationPhases.None && !m_criGeneratedInstanceEvaluated)
			{
				m_criGeneratedInstanceEvaluated = true;
				base.CriOwner.EvaluateGeneratedReportItemInstance();
			}
		}

		internal virtual void UpdateRenderReportItem(Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			if (!m_isOldSnapshot)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			SetNewContext();
			if (renderReportItem != null)
			{
				m_renderReportItem = renderReportItem;
				if (m_customProperties != null)
				{
					m_customProperties.UpdateCustomProperties(renderReportItem.CustomProperties);
				}
				if (m_style != null && !m_isListContentsRectangle)
				{
					m_style.UpdateStyleCache(renderReportItem);
				}
			}
		}

		public CustomProperty CreateCustomProperty()
		{
			if (base.CriGenerationPhase != CriGenerationPhases.Definition)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			PrepareCustomProperties();
			Microsoft.ReportingServices.ReportIntermediateFormat.DataValue dataValue = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValue();
			dataValue.Name = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			dataValue.Value = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			if (m_reportItemDef.CustomProperties == null)
			{
				m_reportItemDef.CustomProperties = new Microsoft.ReportingServices.ReportIntermediateFormat.DataValueList();
			}
			m_reportItemDef.CustomProperties.Add(dataValue);
			return CustomProperties.Add(base.RenderingContext, dataValue.Name, dataValue.Value);
		}

		internal virtual ReportItem ExposeAs(RenderingContext renderingContext)
		{
			return this;
		}

		internal virtual void ConstructReportItemDefinition()
		{
			Global.Tracer.Assert(condition: false, "ConstructReportElementDefinition is not implemented on this type of report item: " + m_reportItemDef.ObjectType);
		}

		internal virtual void CompleteCriGeneratedInstanceEvaluation()
		{
			Global.Tracer.Assert(condition: false, "CompleteCriGeneratedInstanceEvaluation is not implemented on this type of report item: " + m_reportItemDef.ObjectType);
		}

		internal void ConstructReportItemDefinitionImpl()
		{
			ConstructReportElementDefinitionImpl();
			ReportItemInstance instance = Instance;
			Global.Tracer.Assert(instance != null, "(instance != null)");
			if (instance.ToolTip != null)
			{
				base.ReportItemDef.ToolTip = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.ToolTip);
			}
			else
			{
				base.ReportItemDef.ToolTip = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_toolTip = null;
			if (instance.Bookmark != null)
			{
				base.ReportItemDef.Bookmark = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.Bookmark);
			}
			else
			{
				base.ReportItemDef.Bookmark = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_bookmark = null;
			if (instance.DocumentMapLabel != null)
			{
				base.ReportItemDef.DocumentMapLabel = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(instance.DocumentMapLabel);
			}
			else
			{
				base.ReportItemDef.DocumentMapLabel = Microsoft.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			m_documentMapLabel = null;
			if (m_customProperties != null)
			{
				if (m_customProperties.Count == 0)
				{
					m_reportItemDef.CustomProperties = null;
					m_customProperties = null;
				}
				else
				{
					m_customProperties.ConstructCustomPropertyDefinitions(m_reportItemDef.CustomProperties);
				}
			}
		}

		private void PrepareCustomProperties()
		{
			if (m_isOldSnapshot)
			{
				if (m_customProperties == null && RenderReportItem.CustomProperties != null)
				{
					m_customProperties = new CustomPropertyCollection(m_renderingContext, RenderReportItem.CustomProperties);
				}
				return;
			}
			if (m_customProperties == null)
			{
				m_customProperties = new CustomPropertyCollection(ReportScope.ReportScopeInstance, m_renderingContext, this, m_reportItemDef, m_reportItemDef.ObjectType, m_reportItemDef.Name);
			}
			else if (!m_customPropertiesReady)
			{
				m_customProperties.UpdateCustomProperties(ReportScope.ReportScopeInstance, m_reportItemDef, m_renderingContext.OdpContext, m_reportItemDef.ObjectType, m_reportItemDef.Name);
				CriEvaluateInstance();
			}
			m_customPropertiesReady = true;
		}

		internal static int StringToInt(string intAsString)
		{
			int result = -1;
			if (int.TryParse(intAsString, NumberStyles.None, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return -1;
		}

		internal static ReportItem CreateItem(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, Microsoft.ReportingServices.ReportIntermediateFormat.ReportItem reportItemDef, RenderingContext renderingContext)
		{
			ReportItem reportItem = null;
			switch (reportItemDef.ObjectType)
			{
			case ObjectType.Textbox:
				reportItem = new TextBox(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.TextBox)reportItemDef, renderingContext);
				break;
			case ObjectType.Rectangle:
				reportItem = new Rectangle(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Rectangle)reportItemDef, renderingContext);
				break;
			case ObjectType.Image:
				reportItem = new Image(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Image)reportItemDef, renderingContext);
				break;
			case ObjectType.Line:
				reportItem = new Line(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Line)reportItemDef, renderingContext);
				break;
			case ObjectType.Subreport:
				reportItem = new SubReport(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.SubReport)reportItemDef, renderingContext);
				break;
			case ObjectType.Tablix:
				reportItem = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Tablix)reportItemDef, renderingContext);
				break;
			case ObjectType.Chart:
				reportItem = new Chart(parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Chart)reportItemDef, renderingContext);
				break;
			case ObjectType.GaugePanel:
				reportItem = new GaugePanel(parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.GaugePanel)reportItemDef, renderingContext);
				break;
			case ObjectType.CustomReportItem:
			{
				Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem customReportItem = (Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem)reportItemDef;
				reportItem = new CustomReportItem(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, customReportItem, renderingContext);
				if (!((CustomReportItem)reportItem).Initialize(renderingContext))
				{
					reportItem = CreateItem(reportScope, parentDefinitionPath, customReportItem.AltReportItemIndexInParentCollectionDef, customReportItem.AltReportItem, renderingContext);
					reportItem.ReportItemDef.RepeatedSibling = customReportItem.RepeatedSibling;
					reportItem.ReportItemDef.RepeatWith = customReportItem.RepeatWith;
					ProcessAlternateCustomReportItem(customReportItem, reportItem, renderingContext);
				}
				break;
			}
			case ObjectType.Map:
				reportItem = new Map(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, (Microsoft.ReportingServices.ReportIntermediateFormat.Map)reportItemDef, renderingContext);
				break;
			}
			return reportItem;
		}

		internal static ReportItem CreateShim(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, Microsoft.ReportingServices.ReportRendering.ReportItem renderReportItem, RenderingContext renderingContext)
		{
			ReportItem result = null;
			if (renderReportItem is Microsoft.ReportingServices.ReportRendering.TextBox)
			{
				result = new TextBox(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.TextBox)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Rectangle)
			{
				result = new Rectangle(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Rectangle)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Image)
			{
				result = new Image(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Image)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.List)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.List)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Table)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Table)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Matrix)
			{
				result = new Tablix(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Matrix)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Chart)
			{
				result = new Chart(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Chart)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.CustomReportItem)
			{
				result = new CustomReportItem(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.CustomReportItem)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.SubReport)
			{
				result = new SubReport(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.SubReport)renderReportItem, renderingContext);
			}
			else if (renderReportItem is Microsoft.ReportingServices.ReportRendering.Line)
			{
				result = new Line(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, (Microsoft.ReportingServices.ReportRendering.Line)renderReportItem, renderingContext);
			}
			return result;
		}

		private static void ProcessAlternateCustomReportItem(Microsoft.ReportingServices.ReportIntermediateFormat.CustomReportItem criDef, ReportItem reportItem, RenderingContext renderingContext)
		{
			if (!criDef.ExplicitlyDefinedAltReportItem)
			{
				string text = null;
				Global.Tracer.Assert(renderingContext.OdpContext.ExtFactory != null, "ExtFactory != null.");
				if (!renderingContext.OdpContext.ExtFactory.IsRegisteredCustomReportItemExtension(criDef.Type))
				{
					renderingContext.OdpContext.TopLevelContext.ErrorContext.Register(ProcessingErrorCode.rsCRIControlNotInstalled, Severity.Warning, ObjectType.CustomReportItem, criDef.Name, criDef.Type);
					text = "The '{1}.{0}' extension is not present in the configuration file: The element '{2}' will render the AltReportItem, which is not defined. Therefore, it shows an empty space.";
				}
				else
				{
					renderingContext.ErrorContext.Register(ProcessingErrorCode.rsCRIControlFailedToLoad, Severity.Warning, ObjectType.CustomReportItem, criDef.Name, criDef.Type);
					text = "The '{1}.{0}' extension failed to load: The element '{2}' will render the AltReportItem, which is not defined. Therefore, it shows an empty space.";
				}
				Global.Tracer.Trace(TraceLevel.Verbose, text, criDef.Name, criDef.Type, reportItem.Name);
			}
		}
	}
}
