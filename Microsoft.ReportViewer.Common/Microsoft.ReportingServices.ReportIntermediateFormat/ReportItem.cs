using Microsoft.ReportingServices.Diagnostics.Utilities;
using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandProcessing.Scalability;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal abstract class ReportItem : IDOwner, IStyleContainer, IComparable, IPersistable, ICustomPropertiesHolder, IVisibilityOwner, IReferenceable, IStaticReferenceable
	{
		internal enum DataElementStyles
		{
			Attribute,
			Element,
			Auto
		}

		private const string ZeroSize = "0mm";

		private const int OverlapDetectionRounding = 1;

		protected string m_name;

		protected Style m_styleClass;

		protected string m_top;

		protected double m_topValue;

		protected string m_left;

		protected double m_leftValue;

		protected string m_height;

		protected double m_heightValue;

		protected string m_width;

		protected double m_widthValue;

		protected int m_zIndex;

		protected ExpressionInfo m_toolTip;

		protected Visibility m_visibility;

		protected ExpressionInfo m_documentMapLabel;

		protected ExpressionInfo m_bookmark;

		protected bool m_repeatedSibling;

		protected bool m_isFullSize;

		private int m_exprHostID = -1;

		protected string m_dataElementName;

		protected DataElementOutputTypes m_dataElementOutput = DataElementOutputTypes.Auto;

		protected DataValueList m_customProperties;

		protected bool m_computed;

		protected string m_repeatWith;

		[Reference]
		private IVisibilityOwner m_containingDynamicVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicRowVisibility;

		[Reference]
		private IVisibilityOwner m_containingDynamicColumnVisibility;

		[NonSerialized]
		protected ReportItem m_parent;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		protected bool m_softPageBreak;

		[NonSerialized]
		protected bool m_shareMyLastPage = true;

		[NonSerialized]
		protected bool m_startHidden;

		[NonSerialized]
		protected double m_topInPage;

		[NonSerialized]
		protected double m_bottomInPage;

		[NonSerialized]
		private Microsoft.ReportingServices.ReportProcessing.ReportProcessing.PageTextboxes m_repeatedSiblingTextboxes;

		[NonSerialized]
		private int m_staticRefId = int.MinValue;

		[NonSerialized]
		private IReportScopeInstance m_romScopeInstance;

		[NonSerialized]
		private bool m_cachedHiddenValue;

		[NonSerialized]
		private bool m_cachedDeepHiddenValue;

		[NonSerialized]
		private bool m_cachedStartHiddenValue;

		[NonSerialized]
		private bool m_hasCachedHiddenValue;

		[NonSerialized]
		private bool m_hasCachedDeepHiddenValue;

		[NonSerialized]
		private bool m_hasCachedStartHiddenValue;

		[NonSerialized]
		private List<InstancePathItem> m_visibilityCacheLastInstancePath;

		[NonSerialized]
		protected StyleProperties m_sharedStyleProperties;

		[NonSerialized]
		protected bool m_noNonSharedStyleProps;

		[NonSerialized]
		protected ReportSize m_heightForRendering;

		[NonSerialized]
		protected ReportSize m_widthForRendering;

		[NonSerialized]
		protected ReportSize m_topForRendering;

		[NonSerialized]
		protected ReportSize m_leftForRendering;

		internal string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public Style StyleClass
		{
			get
			{
				return m_styleClass;
			}
			set
			{
				m_styleClass = value;
			}
		}

		IInstancePath IStyleContainer.InstancePath => this;

		Microsoft.ReportingServices.ReportProcessing.ObjectType IStyleContainer.ObjectType => ObjectType;

		string IStyleContainer.Name => Name;

		internal string Top
		{
			get
			{
				return m_top;
			}
			set
			{
				m_top = value;
			}
		}

		internal double TopValue
		{
			get
			{
				return m_topValue;
			}
			set
			{
				m_topValue = value;
			}
		}

		internal string Left
		{
			get
			{
				return m_left;
			}
			set
			{
				m_left = value;
			}
		}

		internal double LeftValue
		{
			get
			{
				return m_leftValue;
			}
			set
			{
				m_leftValue = value;
			}
		}

		internal string Height
		{
			get
			{
				return m_height;
			}
			set
			{
				m_height = value;
			}
		}

		internal double HeightValue
		{
			get
			{
				return m_heightValue;
			}
			set
			{
				m_heightValue = value;
			}
		}

		internal string Width
		{
			get
			{
				return m_width;
			}
			set
			{
				m_width = value;
			}
		}

		internal double WidthValue
		{
			get
			{
				return m_widthValue;
			}
			set
			{
				m_widthValue = value;
			}
		}

		internal double AbsoluteTopValue
		{
			get
			{
				if (m_heightValue < 0.0)
				{
					return RoundSize(m_topValue + m_heightValue);
				}
				return RoundSize(m_topValue);
			}
		}

		internal double AbsoluteLeftValue
		{
			get
			{
				if (m_widthValue < 0.0)
				{
					return RoundSize(m_leftValue + m_widthValue);
				}
				return RoundSize(m_leftValue);
			}
		}

		internal double AbsoluteBottomValue
		{
			get
			{
				if (m_heightValue < 0.0)
				{
					return RoundSize(m_topValue);
				}
				return RoundSize(m_topValue + m_heightValue);
			}
		}

		internal double AbsoluteRightValue
		{
			get
			{
				if (m_widthValue < 0.0)
				{
					return RoundSize(m_leftValue);
				}
				return RoundSize(m_leftValue + m_widthValue);
			}
		}

		internal int ZIndex
		{
			get
			{
				return m_zIndex;
			}
			set
			{
				m_zIndex = value;
			}
		}

		internal ExpressionInfo ToolTip
		{
			get
			{
				return m_toolTip;
			}
			set
			{
				m_toolTip = value;
			}
		}

		public Visibility Visibility
		{
			get
			{
				return m_visibility;
			}
			set
			{
				m_visibility = value;
			}
		}

		internal ExpressionInfo DocumentMapLabel
		{
			get
			{
				return m_documentMapLabel;
			}
			set
			{
				m_documentMapLabel = value;
			}
		}

		internal ExpressionInfo Bookmark
		{
			get
			{
				return m_bookmark;
			}
			set
			{
				m_bookmark = value;
			}
		}

		internal bool RepeatedSibling
		{
			get
			{
				return m_repeatedSibling;
			}
			set
			{
				m_repeatedSibling = value;
			}
		}

		internal int ExprHostID
		{
			get
			{
				return m_exprHostID;
			}
			set
			{
				m_exprHostID = value;
			}
		}

		internal string DataElementName
		{
			get
			{
				return m_dataElementName;
			}
			set
			{
				m_dataElementName = value;
			}
		}

		internal virtual string DataElementNameDefault => m_name;

		internal DataElementOutputTypes DataElementOutput
		{
			get
			{
				return m_dataElementOutput;
			}
			set
			{
				m_dataElementOutput = value;
			}
		}

		internal ReportItem Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				m_parent = value;
			}
		}

		internal bool Computed
		{
			get
			{
				return m_computed;
			}
			set
			{
				m_computed = value;
			}
		}

		internal virtual bool IsDataRegion => false;

		internal string RepeatWith
		{
			get
			{
				return m_repeatWith;
			}
			set
			{
				m_repeatWith = value;
			}
		}

		internal ReportItemExprHost ExprHost => m_exprHost;

		internal virtual bool SoftPageBreak
		{
			get
			{
				return m_softPageBreak;
			}
			set
			{
				m_softPageBreak = value;
			}
		}

		internal virtual bool ShareMyLastPage
		{
			get
			{
				return m_shareMyLastPage;
			}
			set
			{
				m_shareMyLastPage = value;
			}
		}

		internal bool StartHidden
		{
			get
			{
				return m_startHidden;
			}
			set
			{
				m_startHidden = value;
			}
		}

		internal StyleProperties SharedStyleProperties
		{
			get
			{
				return m_sharedStyleProperties;
			}
			set
			{
				m_sharedStyleProperties = value;
			}
		}

		internal bool NoNonSharedStyleProps
		{
			get
			{
				return m_noNonSharedStyleProps;
			}
			set
			{
				m_noNonSharedStyleProps = value;
			}
		}

		internal ReportSize HeightForRendering
		{
			get
			{
				return m_heightForRendering;
			}
			set
			{
				m_heightForRendering = value;
			}
		}

		internal ReportSize WidthForRendering
		{
			get
			{
				return m_widthForRendering;
			}
			set
			{
				m_widthForRendering = value;
			}
		}

		internal ReportSize TopForRendering
		{
			get
			{
				return m_topForRendering;
			}
			set
			{
				m_topForRendering = value;
			}
		}

		internal ReportSize LeftForRendering
		{
			get
			{
				return m_leftForRendering;
			}
			set
			{
				m_leftForRendering = value;
			}
		}

		internal virtual DataElementOutputTypes DataElementOutputDefault => DataElementOutputTypes.Output;

		internal double TopInStartPage
		{
			get
			{
				return m_topInPage;
			}
			set
			{
				m_topInPage = value;
			}
		}

		internal double BottomInEndPage
		{
			get
			{
				return m_bottomInPage;
			}
			set
			{
				m_bottomInPage = value;
			}
		}

		DataValueList ICustomPropertiesHolder.CustomProperties => m_customProperties;

		IInstancePath ICustomPropertiesHolder.InstancePath => this;

		internal DataValueList CustomProperties
		{
			get
			{
				return m_customProperties;
			}
			set
			{
				m_customProperties = value;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ReportProcessing.PageTextboxes RepeatedSiblingTextboxes
		{
			get
			{
				return m_repeatedSiblingTextboxes;
			}
			set
			{
				m_repeatedSiblingTextboxes = value;
			}
		}

		internal abstract Microsoft.ReportingServices.ReportProcessing.ObjectType ObjectType
		{
			get;
		}

		public IReportScopeInstance ROMScopeInstance
		{
			get
			{
				return m_romScopeInstance;
			}
			set
			{
				m_romScopeInstance = value;
				if (IsVisibilityCacheInstancePathInvalid())
				{
					ResetVisibilityComputationCache();
				}
			}
		}

		public IVisibilityOwner ContainingDynamicVisibility
		{
			get
			{
				return m_containingDynamicVisibility;
			}
			set
			{
				m_containingDynamicVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicColumnVisibility
		{
			get
			{
				return m_containingDynamicColumnVisibility;
			}
			set
			{
				m_containingDynamicColumnVisibility = value;
			}
		}

		public IVisibilityOwner ContainingDynamicRowVisibility
		{
			get
			{
				return m_containingDynamicRowVisibility;
			}
			set
			{
				m_containingDynamicRowVisibility = value;
			}
		}

		public string SenderUniqueName
		{
			get
			{
				if (m_visibility != null)
				{
					TextBox toggleSender = m_visibility.ToggleSender;
					if (toggleSender != null)
					{
						return toggleSender.UniqueName;
					}
				}
				return null;
			}
		}

		int IStaticReferenceable.ID => m_staticRefId;

		protected ReportItem(int id, ReportItem parent)
			: base(id)
		{
			m_parent = parent;
		}

		protected ReportItem(ReportItem parent)
		{
			m_parent = parent;
		}

		internal bool IsOrContainsDataRegionOrSubReport()
		{
			if (IsDataRegion)
			{
				return true;
			}
			if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Map)
			{
				return ((Map)this).ContainsMapDataRegion();
			}
			if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Subreport)
			{
				return true;
			}
			if (ObjectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Rectangle)
			{
				return ((Rectangle)this).ContainsDataRegionOrSubReport();
			}
			return false;
		}

		public bool ComputeHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			if (!CanUseCachedVisibilityData(m_hasCachedHiddenValue))
			{
				UpdateVisibilityDataCacheFlag(ref m_hasCachedHiddenValue);
				m_cachedHiddenValue = Visibility.ComputeHidden(this, renderingContext, direction, out bool valueIsDeep);
				if (valueIsDeep)
				{
					m_hasCachedDeepHiddenValue = true;
					m_cachedDeepHiddenValue = m_cachedHiddenValue;
				}
			}
			return m_cachedHiddenValue;
		}

		public bool ComputeDeepHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext, ToggleCascadeDirection direction)
		{
			if (!CanUseCachedVisibilityData(m_hasCachedDeepHiddenValue))
			{
				bool hidden = ComputeHidden(renderingContext, direction);
				if (!m_hasCachedDeepHiddenValue)
				{
					m_hasCachedDeepHiddenValue = true;
					m_cachedDeepHiddenValue = Visibility.ComputeDeepHidden(hidden, this, direction, renderingContext);
				}
			}
			return m_cachedDeepHiddenValue;
		}

		public bool ComputeStartHidden(Microsoft.ReportingServices.OnDemandReportRendering.RenderingContext renderingContext)
		{
			if (!CanUseCachedVisibilityData(m_hasCachedStartHiddenValue))
			{
				UpdateVisibilityDataCacheFlag(ref m_hasCachedStartHiddenValue);
				if (m_visibility == null || m_visibility.Hidden == null)
				{
					m_cachedStartHiddenValue = false;
				}
				else if (!m_visibility.Hidden.IsExpression)
				{
					m_cachedStartHiddenValue = m_visibility.Hidden.BoolValue;
				}
				else
				{
					m_cachedStartHiddenValue = EvaluateStartHidden(m_romScopeInstance, renderingContext.OdpContext);
				}
			}
			return m_cachedStartHiddenValue;
		}

		private bool CanUseCachedVisibilityData(bool cacheHasValue)
		{
			if (!cacheHasValue)
			{
				return false;
			}
			if ((m_romScopeInstance == null || m_romScopeInstance.IsNewContext) && IsVisibilityCacheInstancePathInvalid())
			{
				ResetVisibilityComputationCache();
				return false;
			}
			return true;
		}

		private bool IsVisibilityCacheInstancePathInvalid()
		{
			if (m_visibilityCacheLastInstancePath != null)
			{
				if (m_visibilityCacheLastInstancePath.Count > 0)
				{
					return !InstancePathItem.IsSamePath(InstancePath, m_visibilityCacheLastInstancePath);
				}
				return false;
			}
			return true;
		}

		private void UpdateVisibilityDataCacheFlag(ref bool cacheHasValue)
		{
			cacheHasValue = true;
			if (m_romScopeInstance == null || m_romScopeInstance.IsNewContext)
			{
				InstancePathItem.DeepCopyPath(InstancePath, ref m_visibilityCacheLastInstancePath);
			}
		}

		internal void ResetVisibilityComputationCache()
		{
			m_hasCachedHiddenValue = false;
			m_hasCachedDeepHiddenValue = false;
			m_hasCachedStartHiddenValue = false;
		}

		internal virtual bool Initialize(InitializationContext context)
		{
			if (m_top == null)
			{
				m_top = "0mm";
				m_topValue = 0.0;
			}
			else
			{
				m_topValue = context.ValidateSize(ref m_top, "Top");
			}
			if (m_left == null)
			{
				m_left = "0mm";
				m_leftValue = 0.0;
			}
			else
			{
				m_leftValue = context.ValidateSize(ref m_left, "Left");
			}
			ValidateItemSizeAndBoundaries(context);
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_documentMapLabel != null)
			{
				m_documentMapLabel.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(m_documentMapLabel);
			}
			if (m_bookmark != null)
			{
				m_bookmark.Initialize("Bookmark", context);
				context.ExprHostBuilder.ReportItemBookmark(m_bookmark);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.ReportItemToolTip(m_toolTip);
			}
			if (m_customProperties != null)
			{
				m_customProperties.Initialize(null, context);
			}
			DataRendererInitialize(context);
			return false;
		}

		internal virtual void TraverseScopes(IRIFScopeVisitor visitor)
		{
		}

		private void ValidateItemSizeAndBoundaries(InitializationContext context)
		{
			if (context.PublishingContext.PublishingContextKind == PublishingContextKind.DataShape)
			{
				return;
			}
			if (m_parent != null)
			{
				bool flag = true;
				if (m_width == null)
				{
					if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablix) == 0)
					{
						if (Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix == context.ObjectType)
						{
							m_width = "0mm";
							m_widthValue = 0.0;
							flag = false;
						}
						else if (Microsoft.ReportingServices.ReportProcessing.ObjectType.PageHeader == context.ObjectType || Microsoft.ReportingServices.ReportProcessing.ObjectType.PageFooter == context.ObjectType)
						{
							ReportSection reportSection = m_parent as ReportSection;
							m_widthValue = reportSection.PageSectionWidth;
							m_width = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(m_widthValue);
						}
						else
						{
							m_widthValue = Math.Round(m_parent.m_widthValue - m_leftValue, 10);
							m_width = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(m_widthValue);
						}
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					m_widthValue = context.ValidateSize(m_width, "Width");
				}
				flag = true;
				if (m_height == null)
				{
					if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablix) == 0)
					{
						if (Microsoft.ReportingServices.ReportProcessing.ObjectType.Tablix == context.ObjectType)
						{
							m_height = "0mm";
							m_heightValue = 0.0;
							flag = false;
						}
						else
						{
							m_heightValue = Math.Round(m_parent.m_heightValue - m_topValue, 10);
							m_height = Microsoft.ReportingServices.ReportPublishing.Converter.ConvertSize(m_heightValue);
						}
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					m_heightValue = context.ValidateSize(m_height, "Height");
				}
			}
			else
			{
				m_widthValue = context.ValidateSize(ref m_width, "Width");
				m_heightValue = context.ValidateSize(ref m_height, "Height");
			}
			if ((context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InTablix) == 0)
			{
				ValidateParentBoundaries(context, context.ObjectType, context.ObjectName);
			}
		}

		private void ValidateParentBoundaries(InitializationContext context, Microsoft.ReportingServices.ReportProcessing.ObjectType objectType, string objectName)
		{
			if (m_parent == null || m_parent is Report || m_parent is ReportSection)
			{
				return;
			}
			if (objectType == Microsoft.ReportingServices.ReportProcessing.ObjectType.Line)
			{
				if (AbsoluteTopValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Top".ToLowerInvariant());
				}
				if (AbsoluteLeftValue < 0.0)
				{
					context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Left".ToLowerInvariant());
				}
			}
			if (AbsoluteBottomValue > RoundSize(m_parent.HeightValue))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Bottom".ToLowerInvariant());
			}
			if (AbsoluteRightValue > RoundSize(m_parent.WidthValue))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Right".ToLowerInvariant());
			}
		}

		protected static double RoundSize(double size)
		{
			return Math.Round(Math.Round(size, 4), 1);
		}

		protected virtual void DataRendererInitialize(InitializationContext context)
		{
			Microsoft.ReportingServices.ReportPublishing.CLSNameValidator.ValidateDataElementName(ref m_dataElementName, DataElementNameDefault, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
			if (m_dataElementOutput == DataElementOutputTypes.Auto)
			{
				if ((m_visibility != null && m_visibility.Hidden != null && m_visibility.Hidden.Type == ExpressionInfo.Types.Constant && m_visibility.Hidden.BoolValue && m_visibility.Toggle == null) || (context.Location & Microsoft.ReportingServices.ReportPublishing.LocationFlags.InNonToggleableHiddenStaticTablixMember) != 0)
				{
					m_dataElementOutput = DataElementOutputTypes.NoOutput;
				}
				else
				{
					m_dataElementOutput = DataElementOutputDefault;
				}
			}
		}

		internal virtual void CalculateSizes(double width, double height, InitializationContext context, bool overwrite)
		{
			if (overwrite)
			{
				m_top = "0mm";
				m_topValue = 0.0;
				m_left = "0mm";
				m_leftValue = 0.0;
			}
			if (m_width == null || (overwrite && m_widthValue != width))
			{
				m_width = width.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_widthValue = context.ValidateSize(ref m_width, "Width");
			}
			if (m_height == null || (overwrite && m_heightValue != height))
			{
				m_height = height.ToString("f5", CultureInfo.InvariantCulture) + "mm";
				m_heightValue = context.ValidateSize(ref m_height, "Height");
			}
			ValidateParentBoundaries(context, ObjectType, Name);
		}

		internal void CalculateSizes(InitializationContext context, bool overwrite)
		{
			double width = m_widthValue;
			double height = m_heightValue;
			if (m_width == null)
			{
				width = Math.Round(Math.Max(0.0, m_parent.m_widthValue - m_leftValue), 10);
			}
			if (m_height == null)
			{
				height = Math.Round(Math.Max(0.0, m_parent.m_heightValue - m_topValue), 10);
			}
			CalculateSizes(width, height, context, overwrite);
		}

		internal virtual void InitializeRVDirectionDependentItems(InitializationContext context)
		{
		}

		internal virtual void DetermineGroupingExprValueCount(InitializationContext context, int groupingExprCount)
		{
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is ReportItem))
			{
				throw new ReportProcessingException(ErrorCode.rsInvalidOperation);
			}
			ReportItem reportItem = (ReportItem)obj;
			if (m_topValue < reportItem.m_topValue)
			{
				return -1;
			}
			if (m_topValue > reportItem.m_topValue)
			{
				return 1;
			}
			if (m_leftValue < reportItem.m_leftValue)
			{
				return -1;
			}
			if (m_leftValue > reportItem.m_leftValue)
			{
				return 1;
			}
			return 0;
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ReportItem reportItem = (ReportItem)base.PublishClone(context);
			reportItem.m_name = context.CreateUniqueReportItemName(m_name, m_isClone);
			if (m_styleClass != null)
			{
				reportItem.m_styleClass = (Style)m_styleClass.PublishClone(context);
			}
			if (m_top != null)
			{
				reportItem.m_top = (string)m_top.Clone();
			}
			if (m_left != null)
			{
				reportItem.m_left = (string)m_left.Clone();
			}
			if (m_height != null)
			{
				reportItem.m_height = (string)m_height.Clone();
			}
			if (m_width != null)
			{
				reportItem.m_width = (string)m_width.Clone();
			}
			if (m_toolTip != null)
			{
				reportItem.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_visibility != null)
			{
				reportItem.m_visibility = (Visibility)m_visibility.PublishClone(context, isSubtotalMember: false);
			}
			reportItem.m_documentMapLabel = null;
			reportItem.m_bookmark = null;
			if (m_dataElementName != null)
			{
				reportItem.m_dataElementName = (string)m_dataElementName.Clone();
			}
			if (m_repeatWith != null)
			{
				context.AddReportItemWithRepeatWithToUpdate(reportItem);
				reportItem.m_repeatWith = (string)m_repeatWith.Clone();
			}
			if (m_customProperties != null)
			{
				reportItem.m_customProperties = new DataValueList(m_customProperties.Count);
				{
					foreach (DataValue customProperty in m_customProperties)
					{
						reportItem.m_customProperties.Add((DataValue)customProperty.PublishClone(context));
					}
					return reportItem;
				}
			}
			return reportItem;
		}

		internal override void SetupCriRenderItemDef(ReportItem reportItem)
		{
			base.SetupCriRenderItemDef(reportItem);
			reportItem.Name = Name + "." + reportItem.Name;
			reportItem.DataElementName = reportItem.Name;
			reportItem.DataElementOutput = DataElementOutput;
			reportItem.RepeatWith = RepeatWith;
			reportItem.RepeatedSibling = RepeatedSibling;
			reportItem.Top = Top;
			reportItem.TopValue = TopValue;
			reportItem.Left = Left;
			reportItem.LeftValue = LeftValue;
			reportItem.Height = Height;
			reportItem.HeightValue = HeightValue;
			reportItem.Width = Width;
			reportItem.WidthValue = WidthValue;
			reportItem.ZIndex = ZIndex;
			reportItem.Visibility = Visibility;
			reportItem.Computed = true;
		}

		internal void UpdateRepeatWithReference(AutomaticSubtotalContext context)
		{
			if (m_repeatWith != null)
			{
				m_repeatWith = context.GetNewReportItemName(m_repeatWith);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Style));
			list.Add(new MemberInfo(MemberName.Top, Token.String));
			list.Add(new MemberInfo(MemberName.TopValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Left, Token.String));
			list.Add(new MemberInfo(MemberName.LeftValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Height, Token.String));
			list.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			list.Add(new MemberInfo(MemberName.Width, Token.String));
			list.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			list.Add(new MemberInfo(MemberName.ZIndex, Token.Int32));
			list.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Visibility));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Bookmark, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RepeatedSibling, Token.Boolean));
			list.Add(new MemberInfo(MemberName.IsFullSize, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			list.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			list.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.DataValue));
			list.Add(new MemberInfo(MemberName.Computed, Token.Boolean));
			list.Add(new MemberInfo(MemberName.ContainingDynamicVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicRowVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.ContainingDynamicColumnVisibility, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IVisibilityOwner, Token.Reference));
			list.Add(new MemberInfo(MemberName.RepeatWith, Token.String));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.IDOwner, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.StyleClass:
					writer.Write(m_styleClass);
					break;
				case MemberName.Top:
					writer.Write(m_top);
					break;
				case MemberName.TopValue:
					writer.Write(m_topValue);
					break;
				case MemberName.Left:
					writer.Write(m_left);
					break;
				case MemberName.LeftValue:
					writer.Write(m_leftValue);
					break;
				case MemberName.Height:
					writer.Write(m_height);
					break;
				case MemberName.HeightValue:
					writer.Write(m_heightValue);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.WidthValue:
					writer.Write(m_widthValue);
					break;
				case MemberName.ZIndex:
					writer.Write(m_zIndex);
					break;
				case MemberName.Visibility:
					writer.Write(m_visibility);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Label:
					writer.Write(m_documentMapLabel);
					break;
				case MemberName.Bookmark:
					writer.Write(m_bookmark);
					break;
				case MemberName.RepeatedSibling:
					writer.Write(m_repeatedSibling);
					break;
				case MemberName.IsFullSize:
					writer.Write(m_isFullSize);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.DataElementName:
					writer.Write(m_dataElementName);
					break;
				case MemberName.DataElementOutput:
					writer.WriteEnum((int)m_dataElementOutput);
					break;
				case MemberName.CustomProperties:
					writer.Write(m_customProperties);
					break;
				case MemberName.Computed:
					writer.Write(m_computed);
					break;
				case MemberName.ContainingDynamicVisibility:
					writer.WriteReference(m_containingDynamicVisibility);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					writer.WriteReference(m_containingDynamicRowVisibility);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					writer.WriteReference(m_containingDynamicColumnVisibility);
					break;
				case MemberName.RepeatWith:
					writer.Write(m_repeatWith);
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(m_Declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.StyleClass:
					m_styleClass = (Style)reader.ReadRIFObject();
					break;
				case MemberName.Top:
					m_top = reader.ReadString();
					break;
				case MemberName.TopValue:
					m_topValue = reader.ReadDouble();
					break;
				case MemberName.Left:
					m_left = reader.ReadString();
					break;
				case MemberName.LeftValue:
					m_leftValue = reader.ReadDouble();
					break;
				case MemberName.Height:
					m_height = reader.ReadString();
					break;
				case MemberName.HeightValue:
					m_heightValue = reader.ReadDouble();
					break;
				case MemberName.Width:
					m_width = reader.ReadString();
					break;
				case MemberName.WidthValue:
					m_widthValue = reader.ReadDouble();
					break;
				case MemberName.ZIndex:
					m_zIndex = reader.ReadInt32();
					break;
				case MemberName.Visibility:
					m_visibility = (Visibility)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Label:
					m_documentMapLabel = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Bookmark:
					m_bookmark = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RepeatedSibling:
					m_repeatedSibling = reader.ReadBoolean();
					break;
				case MemberName.IsFullSize:
					m_isFullSize = reader.ReadBoolean();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.DataElementName:
					m_dataElementName = reader.ReadString();
					break;
				case MemberName.DataElementOutput:
					m_dataElementOutput = (DataElementOutputTypes)reader.ReadEnum();
					break;
				case MemberName.CustomProperties:
					m_customProperties = reader.ReadListOfRIFObjects<DataValueList>();
					break;
				case MemberName.Computed:
					m_computed = reader.ReadBoolean();
					break;
				case MemberName.ContainingDynamicVisibility:
					m_containingDynamicVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicRowVisibility:
					m_containingDynamicRowVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.ContainingDynamicColumnVisibility:
					m_containingDynamicColumnVisibility = reader.ReadReference<IVisibilityOwner>(this);
					break;
				case MemberName.RepeatWith:
					m_repeatWith = reader.ReadString();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			if (!memberReferencesCollection.TryGetValue(m_Declaration.ObjectType, out List<MemberReference> value))
			{
				return;
			}
			foreach (MemberReference item in value)
			{
				switch (item.MemberName)
				{
				case MemberName.ContainingDynamicVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value3))
					{
						m_containingDynamicVisibility = (value3 as IVisibilityOwner);
					}
					break;
				}
				case MemberName.ContainingDynamicRowVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value4))
					{
						m_containingDynamicRowVisibility = (value4 as IVisibilityOwner);
					}
					break;
				}
				case MemberName.ContainingDynamicColumnVisibility:
				{
					if (referenceableItems.TryGetValue(item.RefID, out IReferenceable value2))
					{
						m_containingDynamicColumnVisibility = (value2 as IVisibilityOwner);
					}
					break;
				}
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ReportItem;
		}

		internal abstract void SetExprHost(ReportExprHost reportExprHost, ObjectModelImpl reportObjectModel);

		protected void ReportItemSetExprHost(ReportItemExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null);
			m_exprHost = exprHost;
			m_exprHost.SetReportObjectModel(reportObjectModel);
			if (m_styleClass != null)
			{
				m_styleClass.SetStyleExprHost(m_exprHost);
			}
			if (m_exprHost.CustomPropertyHostsRemotable != null)
			{
				Global.Tracer.Assert(m_customProperties != null, "(null != m_customProperties)");
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal bool EvaluateStartHidden(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateStartHiddenExpression(Visibility, m_exprHost, ObjectType, m_name);
		}

		internal string EvaluateBookmark(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemBookmarkExpression(this);
		}

		internal string EvaluateDocumentMapLabel(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemDocumentMapLabelExpression(this);
		}

		internal string EvaluateToolTip(IReportScopeInstance romInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(this, romInstance);
			return context.ReportRuntime.EvaluateReportItemToolTipExpression(this);
		}

		void IStaticReferenceable.SetID(int id)
		{
			m_staticRefId = id;
		}

		Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IStaticReferenceable.GetObjectType()
		{
			return GetObjectType();
		}
	}
}
