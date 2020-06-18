using Microsoft.ReportingServices.Diagnostics;
using Microsoft.ReportingServices.ReportProcessing;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportRendering
{
	internal abstract class ReportItem : IDocumentMapEntry
	{
		private string m_uniqueName;

		protected int m_intUniqueName;

		private Style m_style;

		private CustomPropertyCollection m_customProperties;

		protected bool m_canClick;

		protected bool m_canEdit;

		protected bool m_canDrag;

		protected bool m_dropTarget;

		private MemberBase m_members;

		public string Name
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.DefinitionName;
				}
				return ReportItemDef.Name;
			}
		}

		public string ID
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				if (ReportItemDef.RenderingModelID == null)
				{
					ReportItemDef.RenderingModelID = ReportItemDef.ID.ToString(CultureInfo.InvariantCulture);
				}
				return ReportItemDef.RenderingModelID;
			}
		}

		public bool InDocumentMap
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Label != null;
				}
				return Label != null;
			}
		}

		public bool IsFullSize
		{
			get
			{
				if (IsCustomControl)
				{
					return false;
				}
				return ReportItemDef.IsFullSize;
			}
		}

		public string Label
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Label;
				}
				string result = null;
				if (ReportItemDef.Label != null)
				{
					result = ((ReportItemDef.Label.Type == ExpressionInfo.Types.Constant) ? ReportItemDef.Label.Value : ((ReportItemInstance != null) ? InstanceInfo.Label : null));
				}
				return result;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Label = value;
			}
		}

		public virtual int LinkToChild => -1;

		public string Bookmark
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Bookmark;
				}
				string result = null;
				if (ReportItemDef.Bookmark != null)
				{
					result = ((ReportItemDef.Bookmark.Type == ExpressionInfo.Types.Constant) ? ReportItemDef.Bookmark.Value : ((ReportItemInstance != null) ? InstanceInfo.Bookmark : null));
				}
				return result;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Bookmark = value;
			}
		}

		public string UniqueName
		{
			get
			{
				if (IsCustomControl)
				{
					return m_uniqueName;
				}
				string text = m_uniqueName;
				if (m_uniqueName == null && m_intUniqueName != 0)
				{
					text = m_intUniqueName.ToString(CultureInfo.InvariantCulture);
					if (UseCache)
					{
						m_uniqueName = text;
					}
				}
				return text;
			}
		}

		public ReportSize Height
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Height;
				}
				if (ReportItemDef.HeightForRendering == null)
				{
					ReportItemDef.HeightForRendering = new ReportSize(ReportItemDef.Height, ReportItemDef.HeightValue);
				}
				return ReportItemDef.HeightForRendering;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Height = value;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Width;
				}
				if (ReportItemDef.WidthForRendering == null)
				{
					ReportItemDef.WidthForRendering = new ReportSize(ReportItemDef.Width, ReportItemDef.WidthValue);
				}
				return ReportItemDef.WidthForRendering;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Width = value;
			}
		}

		public ReportSize Top
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Top;
				}
				if (ReportItemDef.TopForRendering == null)
				{
					ReportItemDef.TopForRendering = new ReportSize(ReportItemDef.Top, ReportItemDef.TopValue);
				}
				return ReportItemDef.TopForRendering;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Top = value;
			}
		}

		public ReportSize Left
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Left;
				}
				if (ReportItemDef.LeftForRendering == null)
				{
					ReportItemDef.LeftForRendering = new ReportSize(ReportItemDef.Left, ReportItemDef.LeftValue);
				}
				return ReportItemDef.LeftForRendering;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Left = value;
			}
		}

		public int ZIndex
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.ZIndex;
				}
				return ReportItemDef.ZIndex;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.ZIndex = value;
			}
		}

		public Style Style
		{
			get
			{
				if (IsCustomControl)
				{
					return m_style;
				}
				Style style = m_style;
				if (m_style == null)
				{
					style = new Style(this, ReportItemDef, RenderingContext);
					if (UseCache)
					{
						m_style = style;
					}
				}
				return style;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_style = value;
			}
		}

		public string Custom
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				string text = ReportItemDef.Custom;
				if (text == null && CustomProperties != null)
				{
					CustomProperty customProperty = CustomProperties["Custom"];
					if (customProperty != null && customProperty.Value != null)
					{
						text = DataTypeUtility.ConvertToInvariantString(customProperty.Value);
					}
				}
				return text;
			}
		}

		public CustomPropertyCollection CustomProperties
		{
			get
			{
				if (IsCustomControl)
				{
					return m_customProperties;
				}
				CustomPropertyCollection customPropertyCollection = m_customProperties;
				if (m_customProperties == null && ReportItemDef.CustomProperties != null)
				{
					customPropertyCollection = ((ReportItemInstance == null) ? new CustomPropertyCollection(ReportItemDef.CustomProperties, null) : new CustomPropertyCollection(ReportItemDef.CustomProperties, InstanceInfo.CustomPropertyInstances));
					if (UseCache)
					{
						m_customProperties = customPropertyCollection;
					}
				}
				return customPropertyCollection;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				m_customProperties = value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Tooltip;
				}
				string result = null;
				if (ReportItemDef.ToolTip != null)
				{
					result = ((ExpressionInfo.Types.Constant == ReportItemDef.ToolTip.Type) ? ReportItemDef.ToolTip.Value : ((ReportItemInstance != null) ? InstanceInfo.ToolTip : null));
				}
				return result;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Tooltip = value;
			}
		}

		public virtual bool Hidden
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.Hidden;
				}
				if (ReportItemInstance == null)
				{
					return RenderingContext.GetDefinitionHidden(ReportItemDef.Visibility);
				}
				if (ReportItemDef.Visibility == null)
				{
					return false;
				}
				if (RenderingContext != null && ReportItemDef.Visibility.Toggle != null)
				{
					return RenderingContext.IsItemHidden(ReportItemInstance.UniqueName, potentialSender: false);
				}
				if (RenderingContext.GetDefinitionHidden(ReportItemDef.Visibility))
				{
					return true;
				}
				return InstanceInfo.StartHidden;
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.Hidden = value;
			}
		}

		public bool HasToggle
		{
			get
			{
				if (IsCustomControl)
				{
					return false;
				}
				return Visibility.HasToggle(ReportItemDef.Visibility);
			}
		}

		public string ToggleItem
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				if (ReportItemDef.Visibility == null)
				{
					return null;
				}
				return ReportItemDef.Visibility.Toggle;
			}
		}

		internal TextBox ToggleParent
		{
			get
			{
				if (!HasToggle)
				{
					return null;
				}
				if (ReportItemInstance == null)
				{
					return null;
				}
				if (RenderingContext == null)
				{
					return null;
				}
				return RenderingContext.GetToggleParent(ReportItemInstance.UniqueName);
			}
		}

		public SharedHiddenState SharedHidden
		{
			get
			{
				if (IsCustomControl)
				{
					return Processing.SharedHidden;
				}
				return Visibility.GetSharedHidden(ReportItemDef.Visibility);
			}
			set
			{
				if (!IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Processing.SharedHidden = value;
			}
		}

		public bool IsToggleChild
		{
			get
			{
				if (IsCustomControl)
				{
					return false;
				}
				if (ReportItemInstance == null)
				{
					return false;
				}
				return RenderingContext.IsToggleChild(ReportItemInstance.UniqueName);
			}
		}

		public bool RepeatedSibling
		{
			get
			{
				if (IsCustomControl)
				{
					return false;
				}
				return ReportItemDef.RepeatedSibling;
			}
		}

		public virtual object SharedRenderingInfo
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				Global.Tracer.Assert(RenderingContext != null);
				return RenderingContext.RenderingInfoManager.SharedRenderingInfo[ReportItemDef.ID];
			}
			set
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Global.Tracer.Assert(RenderingContext != null);
				RenderingContext.RenderingInfoManager.SharedRenderingInfo[ReportItemDef.ID] = value;
			}
		}

		public object RenderingInfo
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				Global.Tracer.Assert(RenderingContext != null);
				if (RenderingContext.InPageSection)
				{
					return RenderingContext.RenderingInfoManager.PageSectionRenderingInfo[m_uniqueName];
				}
				if (m_intUniqueName == 0)
				{
					return null;
				}
				Global.Tracer.Assert(m_intUniqueName != 0);
				return RenderingContext.RenderingInfoManager.RenderingInfo[m_intUniqueName];
			}
			set
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Global.Tracer.Assert(RenderingContext != null);
				if (RenderingContext.InPageSection)
				{
					RenderingContext.RenderingInfoManager.PageSectionRenderingInfo[m_uniqueName] = value;
					return;
				}
				if (m_intUniqueName == 0)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				Global.Tracer.Assert(m_intUniqueName != 0);
				RenderingContext.RenderingInfoManager.RenderingInfo[m_intUniqueName] = value;
			}
		}

		public string DataElementName
		{
			get
			{
				if (IsCustomControl)
				{
					return null;
				}
				return ReportItemDef.DataElementName;
			}
		}

		public DataElementOutputTypes DataElementOutput
		{
			get
			{
				if (IsCustomControl)
				{
					return DataElementOutputTypes.NoOutput;
				}
				return ReportItemDef.DataElementOutput;
			}
		}

		internal Microsoft.ReportingServices.ReportProcessing.ReportItem ReportItemDef => Rendering.m_reportItemDef;

		internal ReportItemInstance ReportItemInstance => Rendering.m_reportItemInstance;

		internal ReportItemInstanceInfo InstanceInfo
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (Rendering.m_reportItemInstance == null)
				{
					return null;
				}
				if (Rendering.m_reportItemInstanceInfo == null)
				{
					Rendering.m_reportItemInstanceInfo = Rendering.m_reportItemInstance.GetInstanceInfo(RenderingContext.ChunkManager, RenderingContext.InPageSection);
				}
				return Rendering.m_reportItemInstanceInfo;
			}
		}

		internal RenderingContext RenderingContext
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return Rendering.m_renderingContext;
			}
		}

		internal MatrixHeadingInstance HeadingInstance
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return Rendering.m_headingInstance;
			}
		}

		private ReportItemRendering Rendering
		{
			get
			{
				try
				{
					return (ReportItemRendering)m_members;
				}
				catch
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
		}

		internal ReportItemProcessing Processing
		{
			get
			{
				try
				{
					return (ReportItemProcessing)m_members;
				}
				catch
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
			}
		}

		internal bool UseCache
		{
			get
			{
				if (IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (Rendering.m_renderingContext != null)
				{
					return Rendering.m_renderingContext.CacheState;
				}
				return true;
			}
		}

		protected internal bool IsCustomControl => m_members.IsCustomControl;

		internal bool SkipSearch
		{
			get
			{
				if (SharedHidden == SharedHiddenState.Always)
				{
					return true;
				}
				if (SharedHidden == SharedHiddenState.Sometimes && Hidden)
				{
					return true;
				}
				return false;
			}
		}

		protected ReportItem(string definitionName, string instanceName)
		{
			m_members = new ReportItemProcessing();
			Processing.DefinitionName = definitionName;
			m_uniqueName = instanceName;
		}

		protected ReportItem()
		{
			m_members = new ReportItemProcessing();
		}

		internal ReportItem(Microsoft.ReportingServices.ReportProcessing.CustomReportItem criDef, CustomReportItemInstance criInstance, CustomReportItemInstanceInfo instanceInfo)
		{
			m_members = new ReportItemRendering();
			Rendering.m_reportItemDef = criDef;
			Rendering.m_reportItemInstance = criInstance;
			Rendering.m_reportItemInstanceInfo = instanceInfo;
			m_intUniqueName = criInstance.UniqueName;
		}

		internal ReportItem(string uniqueName, int intUniqueName, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext)
		{
			m_members = new ReportItemRendering();
			m_uniqueName = uniqueName;
			m_intUniqueName = intUniqueName;
			Rendering.m_renderingContext = renderingContext;
			Rendering.m_reportItemDef = reportItemDef;
			Rendering.m_reportItemInstance = reportItemInstance;
			Rendering.m_headingInstance = renderingContext.HeadingInstance;
		}

		protected void DeepClone(ReportItem clone)
		{
			if (clone == null || !IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (m_uniqueName != null)
			{
				clone.m_uniqueName = string.Copy(m_uniqueName);
			}
			clone.m_intUniqueName = m_intUniqueName;
			clone.m_canClick = m_canClick;
			clone.m_canEdit = m_canEdit;
			clone.m_canDrag = m_canDrag;
			clone.m_dropTarget = m_dropTarget;
			Global.Tracer.Assert(m_members is ReportItemProcessing);
			clone.m_members = ((ReportItemProcessing)m_members).DeepClone();
			if (m_style != null)
			{
				m_style.ExtractRenderStyles(out clone.Processing.SharedStyles, out clone.Processing.NonSharedStyles);
			}
			if (m_customProperties != null)
			{
				clone.m_customProperties = m_customProperties.DeepClone();
			}
		}

		internal virtual bool Search(SearchContext searchContext)
		{
			return false;
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

		internal static ReportItem CreateItem(int indexIntoParentCollection, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames nonComputedUniqueNames)
		{
			string uniqueName = null;
			if (renderingContext.InPageSection)
			{
				uniqueName = renderingContext.UniqueNamePrefix + "a" + indexIntoParentCollection.ToString(CultureInfo.InvariantCulture);
			}
			return CreateItem(uniqueName, reportItemDef, reportItemInstance, renderingContext, nonComputedUniqueNames);
		}

		internal static ReportItem CreateItem(string uniqueName, Microsoft.ReportingServices.ReportProcessing.ReportItem reportItemDef, ReportItemInstance reportItemInstance, RenderingContext renderingContext, NonComputedUniqueNames nonComputedUniqueNames)
		{
			if (reportItemDef == null)
			{
				return null;
			}
			Global.Tracer.Assert(renderingContext != null);
			ReportItem reportItem = null;
			int intUniqueName = 0;
			NonComputedUniqueNames[] childrenNonComputedUniqueNames = null;
			if (reportItemInstance != null)
			{
				intUniqueName = reportItemInstance.UniqueName;
			}
			else if (nonComputedUniqueNames != null)
			{
				intUniqueName = nonComputedUniqueNames.UniqueName;
				childrenNonComputedUniqueNames = nonComputedUniqueNames.ChildrenUniqueNames;
			}
			if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Line)
			{
				Microsoft.ReportingServices.ReportProcessing.Line reportItemDef2 = (Microsoft.ReportingServices.ReportProcessing.Line)reportItemDef;
				LineInstance reportItemInstance2 = (LineInstance)reportItemInstance;
				reportItem = new Line(uniqueName, intUniqueName, reportItemDef2, reportItemInstance2, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.CheckBox)
			{
				Microsoft.ReportingServices.ReportProcessing.CheckBox reportItemDef3 = (Microsoft.ReportingServices.ReportProcessing.CheckBox)reportItemDef;
				CheckBoxInstance reportItemInstance3 = (CheckBoxInstance)reportItemInstance;
				reportItem = new CheckBox(uniqueName, intUniqueName, reportItemDef3, reportItemInstance3, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Image)
			{
				Microsoft.ReportingServices.ReportProcessing.Image reportItemDef4 = (Microsoft.ReportingServices.ReportProcessing.Image)reportItemDef;
				ImageInstance reportItemInstance4 = (ImageInstance)reportItemInstance;
				reportItem = new Image(uniqueName, intUniqueName, reportItemDef4, reportItemInstance4, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.TextBox)
			{
				Microsoft.ReportingServices.ReportProcessing.TextBox reportItemDef5 = (Microsoft.ReportingServices.ReportProcessing.TextBox)reportItemDef;
				TextBoxInstance reportItemInstance5 = (TextBoxInstance)reportItemInstance;
				reportItem = new TextBox(uniqueName, intUniqueName, reportItemDef5, reportItemInstance5, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Rectangle)
			{
				Microsoft.ReportingServices.ReportProcessing.Rectangle reportItemDef6 = (Microsoft.ReportingServices.ReportProcessing.Rectangle)reportItemDef;
				RectangleInstance reportItemInstance6 = (RectangleInstance)reportItemInstance;
				reportItem = new Rectangle(uniqueName, intUniqueName, reportItemDef6, reportItemInstance6, renderingContext, childrenNonComputedUniqueNames);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.ActiveXControl)
			{
				Microsoft.ReportingServices.ReportProcessing.ActiveXControl reportItemDef7 = (Microsoft.ReportingServices.ReportProcessing.ActiveXControl)reportItemDef;
				ActiveXControlInstance reportItemInstance7 = (ActiveXControlInstance)reportItemInstance;
				reportItem = new ActiveXControl(uniqueName, intUniqueName, reportItemDef7, reportItemInstance7, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.SubReport)
			{
				Microsoft.ReportingServices.ReportProcessing.SubReport subReport = (Microsoft.ReportingServices.ReportProcessing.SubReport)reportItemDef;
				SubReportInstance subReportInstance = (SubReportInstance)reportItemInstance;
				bool processedWithError = false;
				Report innerReport;
				if (Microsoft.ReportingServices.ReportProcessing.SubReport.Status.Retrieved != subReport.RetrievalStatus)
				{
					innerReport = null;
					processedWithError = true;
				}
				else
				{
					if (subReport.ReportContext == null && renderingContext.CurrentReportContext != null)
					{
						subReport.ReportContext = renderingContext.CurrentReportContext.GetSubreportContext(subReport.ReportPath);
					}
					ICatalogItemContext reportContext = subReport.ReportContext;
					RenderingContext renderingContext2 = new RenderingContext(renderingContext, subReport.Uri, subReport.Report.EmbeddedImages, subReport.Report.ImageStreamNames, reportContext);
					if (subReportInstance == null)
					{
						innerReport = new Report(subReport.Report, null, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
					else if (subReportInstance.ReportInstance == null)
					{
						processedWithError = true;
						innerReport = new Report(subReport.Report, null, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
					else
					{
						innerReport = new Report(subReport.Report, subReportInstance.ReportInstance, renderingContext2, subReport.ReportName, subReport.Description, null);
					}
				}
				reportItem = new SubReport(intUniqueName, subReport, subReportInstance, renderingContext, innerReport, processedWithError);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.List)
			{
				Microsoft.ReportingServices.ReportProcessing.List reportItemDef8 = (Microsoft.ReportingServices.ReportProcessing.List)reportItemDef;
				ListInstance reportItemInstance8 = (ListInstance)reportItemInstance;
				reportItem = new List(intUniqueName, reportItemDef8, reportItemInstance8, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Matrix)
			{
				Microsoft.ReportingServices.ReportProcessing.Matrix reportItemDef9 = (Microsoft.ReportingServices.ReportProcessing.Matrix)reportItemDef;
				MatrixInstance reportItemInstance9 = (MatrixInstance)reportItemInstance;
				reportItem = new Matrix(intUniqueName, reportItemDef9, reportItemInstance9, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Table)
			{
				Microsoft.ReportingServices.ReportProcessing.Table reportItemDef10 = (Microsoft.ReportingServices.ReportProcessing.Table)reportItemDef;
				TableInstance reportItemInstance10 = (TableInstance)reportItemInstance;
				reportItem = new Table(intUniqueName, reportItemDef10, reportItemInstance10, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.OWCChart)
			{
				Microsoft.ReportingServices.ReportProcessing.OWCChart reportItemDef11 = (Microsoft.ReportingServices.ReportProcessing.OWCChart)reportItemDef;
				OWCChartInstance reportItemInstance11 = (OWCChartInstance)reportItemInstance;
				reportItem = new OWCChart(intUniqueName, reportItemDef11, reportItemInstance11, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.Chart)
			{
				Microsoft.ReportingServices.ReportProcessing.Chart reportItemDef12 = (Microsoft.ReportingServices.ReportProcessing.Chart)reportItemDef;
				ChartInstance reportItemInstance12 = (ChartInstance)reportItemInstance;
				reportItem = new Chart(intUniqueName, reportItemDef12, reportItemInstance12, renderingContext);
			}
			else if (reportItemDef is Microsoft.ReportingServices.ReportProcessing.CustomReportItem)
			{
				Microsoft.ReportingServices.ReportProcessing.CustomReportItem reportItemDef13 = (Microsoft.ReportingServices.ReportProcessing.CustomReportItem)reportItemDef;
				CustomReportItemInstance reportItemInstance13 = (CustomReportItemInstance)reportItemInstance;
				reportItem = new CustomReportItem(uniqueName, intUniqueName, reportItemDef13, reportItemInstance13, renderingContext, childrenNonComputedUniqueNames);
				if (!renderingContext.NativeAllCRITypes && (renderingContext.NativeCRITypes == null || !renderingContext.NativeCRITypes.ContainsKey(((CustomReportItem)reportItem).Type)))
				{
					reportItem = ((CustomReportItem)reportItem).AltReportItem;
				}
			}
			return reportItem;
		}
	}
}
