using Microsoft.ReportingServices.ReportProcessing.ExprHostObjectModel;
using Microsoft.ReportingServices.ReportProcessing.Persistence;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.ReportingServices.ReportRendering;
using System;
using System.Globalization;

namespace Microsoft.ReportingServices.ReportProcessing
{
	[Serializable]
	internal abstract class ReportItem : IDOwner, ISearchByUniqueName, IComparable
	{
		internal enum DataElementOutputTypesRDL
		{
			Output,
			NoOutput,
			ContentsOnly,
			Auto
		}

		internal enum DataElementStylesRDL
		{
			AttributeNormal,
			ElementNormal,
			Auto
		}

		private const string ZeroSize = "0mm";

		internal const int OverlapDetectionRounding = 1;

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

		protected ExpressionInfo m_label;

		protected ExpressionInfo m_bookmark;

		protected string m_custom;

		protected bool m_repeatedSibling;

		protected bool m_isFullSize;

		private int m_exprHostID = -1;

		protected string m_dataElementName;

		protected DataElementOutputTypes m_dataElementOutput;

		protected int m_distanceFromReportTop = -1;

		protected int m_distanceBeforeTop;

		protected IntList m_siblingAboveMe;

		protected DataValueList m_customProperties;

		[NonSerialized]
		protected ReportItem m_parent;

		[NonSerialized]
		protected bool m_computed;

		[NonSerialized]
		protected string m_repeatWith;

		[NonSerialized]
		protected DataElementOutputTypesRDL m_dataElementOutputRDL = DataElementOutputTypesRDL.Auto;

		[NonSerialized]
		private ReportItemExprHost m_exprHost;

		[NonSerialized]
		protected int m_startPage = -1;

		[NonSerialized]
		protected int m_endPage = -1;

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
		private ReportProcessing.PageTextboxes m_repeatedSiblingTextboxes;

		[NonSerialized]
		protected string m_renderingModelID;

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

		internal abstract ObjectType ObjectType
		{
			get;
		}

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

		internal Style StyleClass
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
					return Math.Round(m_topValue + m_heightValue, 1);
				}
				return Math.Round(m_topValue, 1);
			}
		}

		internal double AbsoluteLeftValue
		{
			get
			{
				if (m_widthValue < 0.0)
				{
					return Math.Round(m_leftValue + m_widthValue, 1);
				}
				return Math.Round(m_leftValue, 1);
			}
		}

		internal double AbsoluteBottomValue
		{
			get
			{
				if (m_heightValue < 0.0)
				{
					return Math.Round(m_topValue, 1);
				}
				return Math.Round(m_topValue + m_heightValue, 1);
			}
		}

		internal double AbsoluteRightValue
		{
			get
			{
				if (m_widthValue < 0.0)
				{
					return Math.Round(m_leftValue, 1);
				}
				return Math.Round(m_leftValue + m_widthValue, 1);
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

		internal Visibility Visibility
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

		internal ExpressionInfo Label
		{
			get
			{
				return m_label;
			}
			set
			{
				m_label = value;
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

		internal string Custom
		{
			get
			{
				return m_custom;
			}
			set
			{
				m_custom = value;
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

		internal bool IsFullSize
		{
			get
			{
				return m_isFullSize;
			}
			set
			{
				m_isFullSize = value;
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

		internal virtual int DistanceFromReportTop
		{
			get
			{
				return m_distanceFromReportTop;
			}
			set
			{
				m_distanceFromReportTop = value;
			}
		}

		internal int DistanceBeforeTop
		{
			get
			{
				return m_distanceBeforeTop;
			}
			set
			{
				m_distanceBeforeTop = value;
			}
		}

		internal IntList SiblingAboveMe
		{
			get
			{
				return m_siblingAboveMe;
			}
			set
			{
				m_siblingAboveMe = value;
			}
		}

		internal ReportItem Parent => m_parent;

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

		internal DataElementOutputTypesRDL DataElementOutputRDL
		{
			get
			{
				return m_dataElementOutputRDL;
			}
			set
			{
				m_dataElementOutputRDL = value;
			}
		}

		internal ReportItemExprHost ExprHost => m_exprHost;

		internal virtual int StartPage
		{
			get
			{
				return m_startPage;
			}
			set
			{
				m_startPage = value;
			}
		}

		internal virtual int EndPage
		{
			get
			{
				return m_endPage;
			}
			set
			{
				m_endPage = value;
			}
		}

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

		internal string RenderingModelID
		{
			get
			{
				return m_renderingModelID;
			}
			set
			{
				m_renderingModelID = value;
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

		internal ReportProcessing.PageTextboxes RepeatedSiblingTextboxes
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

		protected ReportItem(int id, ReportItem parent)
			: base(id)
		{
			m_parent = parent;
		}

		protected ReportItem(ReportItem parent)
		{
			m_parent = parent;
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
			if (m_parent != null)
			{
				bool flag = true;
				if (m_width == null)
				{
					if ((context.Location & LocationFlags.InMatrixOrTable) == 0)
					{
						if (ObjectType.Table == context.ObjectType || ObjectType.Matrix == context.ObjectType)
						{
							m_width = "0mm";
							m_widthValue = 0.0;
							flag = false;
						}
						else if (ObjectType.PageHeader == context.ObjectType || ObjectType.PageFooter == context.ObjectType)
						{
							Report report = m_parent as Report;
							m_widthValue = report.PageSectionWidth;
							m_width = Converter.ConvertSize(m_widthValue);
						}
						else
						{
							m_widthValue = Math.Round(m_parent.m_widthValue - m_leftValue, Validator.DecimalPrecision);
							m_width = Converter.ConvertSize(m_widthValue);
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
					if ((context.Location & LocationFlags.InMatrixOrTable) == 0)
					{
						if (ObjectType.Table == context.ObjectType || ObjectType.Matrix == context.ObjectType)
						{
							m_height = "0mm";
							m_heightValue = 0.0;
							flag = false;
						}
						else
						{
							m_heightValue = Math.Round(m_parent.m_heightValue - m_topValue, Validator.DecimalPrecision);
							m_height = Converter.ConvertSize(m_heightValue);
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
			if ((context.Location & LocationFlags.InMatrixOrTable) == 0)
			{
				ValidateParentBoundaries(context, context.ObjectType, context.ObjectName);
			}
			if (m_styleClass != null)
			{
				m_styleClass.Initialize(context);
			}
			if (m_label != null)
			{
				m_label.Initialize("Label", context);
				context.ExprHostBuilder.GenericLabel(m_label);
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
				m_customProperties.Initialize(null, isCustomProperty: true, context);
			}
			DataRendererInitialize(context);
			return false;
		}

		private void ValidateParentBoundaries(InitializationContext context, ObjectType objectType, string objectName)
		{
			if (m_parent == null || m_parent is Report)
			{
				return;
			}
			if (objectType == ObjectType.Line)
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
			if (AbsoluteBottomValue > Math.Round(m_parent.HeightValue, 1))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Bottom".ToLowerInvariant());
			}
			if (AbsoluteRightValue > Math.Round(m_parent.WidthValue, 1))
			{
				context.ErrorContext.Register(ProcessingErrorCode.rsReportItemOutsideContainer, Severity.Warning, objectType, objectName, "Right".ToLowerInvariant());
			}
		}

		protected virtual void DataRendererInitialize(InitializationContext context)
		{
			CLSNameValidator.ValidateDataElementName(ref m_dataElementName, DataElementNameDefault, context.ObjectType, context.ObjectName, "DataElementName", context.ErrorContext);
			switch (m_dataElementOutputRDL)
			{
			case DataElementOutputTypesRDL.Output:
				m_dataElementOutput = DataElementOutputTypes.Output;
				break;
			case DataElementOutputTypesRDL.NoOutput:
				m_dataElementOutput = DataElementOutputTypes.NoOutput;
				break;
			case DataElementOutputTypesRDL.ContentsOnly:
				m_dataElementOutput = DataElementOutputTypes.ContentsOnly;
				break;
			case DataElementOutputTypesRDL.Auto:
				if (context.TableColumnVisible && (m_visibility == null || m_visibility.Hidden == null || m_visibility.Toggle != null || (ExpressionInfo.Types.Constant == m_visibility.Hidden.Type && !m_visibility.Hidden.BoolValue)))
				{
					m_dataElementOutput = DataElementOutputDefault;
				}
				else
				{
					m_dataElementOutput = DataElementOutputTypes.NoOutput;
				}
				break;
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
				width = Math.Round(m_parent.m_widthValue - m_leftValue, Validator.DecimalPrecision);
			}
			if (m_height == null)
			{
				height = Math.Round(m_parent.m_heightValue - m_topValue, Validator.DecimalPrecision);
			}
			CalculateSizes(width, height, context, overwrite);
		}

		internal virtual void RegisterReceiver(InitializationContext context)
		{
			if (m_visibility != null)
			{
				m_visibility.RegisterReceiver(context, isContainer: false);
			}
		}

		int IComparable.CompareTo(object obj)
		{
			if (!(obj is ReportItem))
			{
				throw new ArgumentException("Argument was not a ReportItem.  Can only compare two ReportItems");
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
				Global.Tracer.Assert(m_customProperties != null);
				m_customProperties.SetExprHost(m_exprHost.CustomPropertyHostsRemotable, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.Name, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.StyleClass, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Style));
			memberInfoList.Add(new MemberInfo(MemberName.Top, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.TopValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Left, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.LeftValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Height, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.HeightValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.Width, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.WidthValue, Token.Double));
			memberInfoList.Add(new MemberInfo(MemberName.ZIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Visibility, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.Visibility));
			memberInfoList.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Label, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Bookmark, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.ExpressionInfo));
			memberInfoList.Add(new MemberInfo(MemberName.Custom, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.RepeatedSibling, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.IsFullSize, Token.Boolean));
			memberInfoList.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.DataElementOutput, Token.Enum));
			memberInfoList.Add(new MemberInfo(MemberName.DistanceFromReportTop, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DistanceBeforeTop, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.SiblingAboveMe, Token.Array, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IntList));
			memberInfoList.Add(new MemberInfo(MemberName.CustomProperties, Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueList));
			return new Declaration(Microsoft.ReportingServices.ReportProcessing.Persistence.ObjectType.IDOwner, memberInfoList);
		}

		object ISearchByUniqueName.Find(int targetUniqueName, ref NonComputedUniqueNames nonCompNames, ChunkManager.RenderingChunkManager chunkManager)
		{
			if (nonCompNames == null)
			{
				return null;
			}
			if (targetUniqueName == nonCompNames.UniqueName)
			{
				return this;
			}
			return (this as Rectangle)?.SearchChildren(targetUniqueName, ref nonCompNames, chunkManager);
		}

		internal virtual void ProcessDrillthroughAction(ReportProcessing.ProcessingContext processingContext, NonComputedUniqueNames nonCompNames)
		{
		}

		internal void ProcessNavigationAction(ReportProcessing.NavigationInfo navigationInfo, NonComputedUniqueNames nonCompNames, int startPage)
		{
			if (nonCompNames == null)
			{
				return;
			}
			if (m_bookmark != null && m_bookmark.Value != null)
			{
				navigationInfo.ProcessBookmark(m_bookmark.Value, startPage, nonCompNames.UniqueName);
			}
			Rectangle rectangle = this as Rectangle;
			if (m_label != null && m_label.Value != null)
			{
				int num = -1;
				if (rectangle != null)
				{
					navigationInfo.EnterDocumentMapChildren();
					num = rectangle.ProcessNavigationChildren(navigationInfo, nonCompNames, startPage);
				}
				if (num < 0)
				{
					num = nonCompNames.UniqueName;
				}
				navigationInfo.AddToDocumentMap(num, rectangle != null, startPage, m_label.Value);
			}
			else
			{
				rectangle?.ProcessNavigationChildren(navigationInfo, nonCompNames, startPage);
			}
		}
	}
}
