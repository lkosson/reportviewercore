using Microsoft.ReportingServices.OnDemandProcessing;
using Microsoft.ReportingServices.OnDemandReportRendering;
using Microsoft.ReportingServices.RdlExpressions.ExpressionHostObjectModel;
using Microsoft.ReportingServices.ReportIntermediateFormat.Persistence;
using Microsoft.ReportingServices.ReportProcessing;
using Microsoft.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using Microsoft.ReportingServices.ReportPublishing;
using System;
using System.Collections.Generic;

namespace Microsoft.ReportingServices.ReportIntermediateFormat
{
	[Serializable]
	internal class GaugeScale : GaugePanelStyleContainer, IPersistable, IActionOwner
	{
		private Action m_action;

		protected int m_exprHostID;

		[NonSerialized]
		private List<string> m_fieldsUsedInValueExpression;

		[NonSerialized]
		protected GaugeScaleExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		protected string m_name;

		private List<ScaleRange> m_scaleRanges;

		private List<CustomLabel> m_customLabels;

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_logarithmic;

		private ExpressionInfo m_logarithmicBase;

		private GaugeInputValue m_maximumValue;

		private GaugeInputValue m_minimumValue;

		private ExpressionInfo m_multiplier;

		private ExpressionInfo m_reversed;

		private GaugeTickMarks m_gaugeMajorTickMarks;

		private GaugeTickMarks m_gaugeMinorTickMarks;

		private ScalePin m_maximumPin;

		private ScalePin m_minimumPin;

		private ScaleLabels m_scaleLabels;

		private ExpressionInfo m_tickMarksOnTop;

		private ExpressionInfo m_toolTip;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_width;

		private int m_id;

		internal Action Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		Action IActionOwner.Action => m_action;

		List<string> IActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return m_fieldsUsedInValueExpression;
			}
			set
			{
				m_fieldsUsedInValueExpression = value;
			}
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

		internal int ID => m_id;

		internal List<ScaleRange> ScaleRanges
		{
			get
			{
				return m_scaleRanges;
			}
			set
			{
				m_scaleRanges = value;
			}
		}

		internal List<CustomLabel> CustomLabels
		{
			get
			{
				return m_customLabels;
			}
			set
			{
				m_customLabels = value;
			}
		}

		internal ExpressionInfo Interval
		{
			get
			{
				return m_interval;
			}
			set
			{
				m_interval = value;
			}
		}

		internal ExpressionInfo IntervalOffset
		{
			get
			{
				return m_intervalOffset;
			}
			set
			{
				m_intervalOffset = value;
			}
		}

		internal ExpressionInfo Logarithmic
		{
			get
			{
				return m_logarithmic;
			}
			set
			{
				m_logarithmic = value;
			}
		}

		internal ExpressionInfo LogarithmicBase
		{
			get
			{
				return m_logarithmicBase;
			}
			set
			{
				m_logarithmicBase = value;
			}
		}

		internal GaugeInputValue MaximumValue
		{
			get
			{
				return m_maximumValue;
			}
			set
			{
				m_maximumValue = value;
			}
		}

		internal GaugeInputValue MinimumValue
		{
			get
			{
				return m_minimumValue;
			}
			set
			{
				m_minimumValue = value;
			}
		}

		internal ExpressionInfo Multiplier
		{
			get
			{
				return m_multiplier;
			}
			set
			{
				m_multiplier = value;
			}
		}

		internal ExpressionInfo Reversed
		{
			get
			{
				return m_reversed;
			}
			set
			{
				m_reversed = value;
			}
		}

		internal GaugeTickMarks GaugeMajorTickMarks
		{
			get
			{
				return m_gaugeMajorTickMarks;
			}
			set
			{
				m_gaugeMajorTickMarks = value;
			}
		}

		internal GaugeTickMarks GaugeMinorTickMarks
		{
			get
			{
				return m_gaugeMinorTickMarks;
			}
			set
			{
				m_gaugeMinorTickMarks = value;
			}
		}

		internal ScalePin MaximumPin
		{
			get
			{
				return m_maximumPin;
			}
			set
			{
				m_maximumPin = value;
			}
		}

		internal ScalePin MinimumPin
		{
			get
			{
				return m_minimumPin;
			}
			set
			{
				m_minimumPin = value;
			}
		}

		internal ScaleLabels ScaleLabels
		{
			get
			{
				return m_scaleLabels;
			}
			set
			{
				m_scaleLabels = value;
			}
		}

		internal ExpressionInfo TickMarksOnTop
		{
			get
			{
				return m_tickMarksOnTop;
			}
			set
			{
				m_tickMarksOnTop = value;
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

		internal ExpressionInfo Hidden
		{
			get
			{
				return m_hidden;
			}
			set
			{
				m_hidden = value;
			}
		}

		internal ExpressionInfo Width
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

		internal string OwnerName => m_gaugePanel.Name;

		internal GaugeScaleExprHost ExprHost => m_exprHost;

		internal int ExpressionHostID => m_exprHostID;

		internal GaugeScale()
		{
		}

		internal GaugeScale(GaugePanel gaugePanel, int id)
			: base(gaugePanel)
		{
			m_id = id;
		}

		internal override void Initialize(InitializationContext context)
		{
			base.Initialize(context);
			if (m_action != null)
			{
				m_action.Initialize(context);
			}
			if (m_scaleRanges != null)
			{
				for (int i = 0; i < m_scaleRanges.Count; i++)
				{
					m_scaleRanges[i].Initialize(context);
				}
			}
			if (m_customLabels != null)
			{
				for (int j = 0; j < m_customLabels.Count; j++)
				{
					m_customLabels[j].Initialize(context);
				}
			}
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.GaugeScaleInterval(m_interval);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.GaugeScaleIntervalOffset(m_intervalOffset);
			}
			if (m_logarithmic != null)
			{
				m_logarithmic.Initialize("Logarithmic", context);
				context.ExprHostBuilder.GaugeScaleLogarithmic(m_logarithmic);
			}
			if (m_logarithmicBase != null)
			{
				m_logarithmicBase.Initialize("LogarithmicBase", context);
				context.ExprHostBuilder.GaugeScaleLogarithmicBase(m_logarithmicBase);
			}
			if (m_multiplier != null)
			{
				m_multiplier.Initialize("Multiplier", context);
				context.ExprHostBuilder.GaugeScaleMultiplier(m_multiplier);
			}
			if (m_reversed != null)
			{
				m_reversed.Initialize("Reversed", context);
				context.ExprHostBuilder.GaugeScaleReversed(m_reversed);
			}
			if (m_gaugeMajorTickMarks != null)
			{
				m_gaugeMajorTickMarks.Initialize(context, isMajor: true);
			}
			if (m_gaugeMinorTickMarks != null)
			{
				m_gaugeMinorTickMarks.Initialize(context, isMajor: false);
			}
			if (m_maximumPin != null)
			{
				m_maximumPin.Initialize(context, isMaximum: true);
			}
			if (m_minimumPin != null)
			{
				m_minimumPin.Initialize(context, isMaximum: false);
			}
			if (m_scaleLabels != null)
			{
				m_scaleLabels.Initialize(context);
			}
			if (m_tickMarksOnTop != null)
			{
				m_tickMarksOnTop.Initialize("TickMarksOnTop", context);
				context.ExprHostBuilder.GaugeScaleTickMarksOnTop(m_tickMarksOnTop);
			}
			if (m_toolTip != null)
			{
				m_toolTip.Initialize("ToolTip", context);
				context.ExprHostBuilder.GaugeScaleToolTip(m_toolTip);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.GaugeScaleHidden(m_hidden);
			}
			if (m_width != null)
			{
				m_width.Initialize("Width", context);
				context.ExprHostBuilder.GaugeScaleWidth(m_width);
			}
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			GaugeScale gaugeScale = (GaugeScale)base.PublishClone(context);
			if (m_action != null)
			{
				gaugeScale.m_action = (Action)m_action.PublishClone(context);
			}
			if (m_scaleRanges != null)
			{
				gaugeScale.m_scaleRanges = new List<ScaleRange>(m_scaleRanges.Count);
				foreach (ScaleRange scaleRange in m_scaleRanges)
				{
					gaugeScale.m_scaleRanges.Add((ScaleRange)scaleRange.PublishClone(context));
				}
			}
			if (m_customLabels != null)
			{
				gaugeScale.m_customLabels = new List<CustomLabel>(m_customLabels.Count);
				foreach (CustomLabel customLabel in m_customLabels)
				{
					gaugeScale.m_customLabels.Add((CustomLabel)customLabel.PublishClone(context));
				}
			}
			if (m_interval != null)
			{
				gaugeScale.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				gaugeScale.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_logarithmic != null)
			{
				gaugeScale.m_logarithmic = (ExpressionInfo)m_logarithmic.PublishClone(context);
			}
			if (m_logarithmicBase != null)
			{
				gaugeScale.m_logarithmicBase = (ExpressionInfo)m_logarithmicBase.PublishClone(context);
			}
			if (m_maximumValue != null)
			{
				gaugeScale.m_maximumValue = (GaugeInputValue)m_maximumValue.PublishClone(context);
			}
			if (m_minimumValue != null)
			{
				gaugeScale.m_minimumValue = (GaugeInputValue)m_minimumValue.PublishClone(context);
			}
			if (m_multiplier != null)
			{
				gaugeScale.m_multiplier = (ExpressionInfo)m_multiplier.PublishClone(context);
			}
			if (m_reversed != null)
			{
				gaugeScale.m_reversed = (ExpressionInfo)m_reversed.PublishClone(context);
			}
			if (m_gaugeMajorTickMarks != null)
			{
				gaugeScale.m_gaugeMajorTickMarks = (GaugeTickMarks)m_gaugeMajorTickMarks.PublishClone(context);
			}
			if (m_gaugeMinorTickMarks != null)
			{
				gaugeScale.m_gaugeMinorTickMarks = (GaugeTickMarks)m_gaugeMinorTickMarks.PublishClone(context);
			}
			if (m_maximumPin != null)
			{
				gaugeScale.m_maximumPin = (ScalePin)m_maximumPin.PublishClone(context);
			}
			if (m_minimumPin != null)
			{
				gaugeScale.m_minimumPin = (ScalePin)m_minimumPin.PublishClone(context);
			}
			if (m_scaleLabels != null)
			{
				gaugeScale.m_scaleLabels = (ScaleLabels)m_scaleLabels.PublishClone(context);
			}
			if (m_tickMarksOnTop != null)
			{
				gaugeScale.m_tickMarksOnTop = (ExpressionInfo)m_tickMarksOnTop.PublishClone(context);
			}
			if (m_toolTip != null)
			{
				gaugeScale.m_toolTip = (ExpressionInfo)m_toolTip.PublishClone(context);
			}
			if (m_hidden != null)
			{
				gaugeScale.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_width != null)
			{
				gaugeScale.m_width = (ExpressionInfo)m_width.PublishClone(context);
			}
			return gaugeScale;
		}

		internal void SetExprHost(GaugeScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
			IList<ScaleRangeExprHost> scaleRangesHostsRemotable = m_exprHost.ScaleRangesHostsRemotable;
			if (m_scaleRanges != null && scaleRangesHostsRemotable != null)
			{
				for (int i = 0; i < m_scaleRanges.Count; i++)
				{
					ScaleRange scaleRange = m_scaleRanges[i];
					if (scaleRange != null && scaleRange.ExpressionHostID > -1)
					{
						scaleRange.SetExprHost(scaleRangesHostsRemotable[scaleRange.ExpressionHostID], reportObjectModel);
					}
				}
			}
			IList<CustomLabelExprHost> customLabelsHostsRemotable = m_exprHost.CustomLabelsHostsRemotable;
			if (m_customLabels != null && customLabelsHostsRemotable != null)
			{
				for (int j = 0; j < m_customLabels.Count; j++)
				{
					CustomLabel customLabel = m_customLabels[j];
					if (customLabel != null && customLabel.ExpressionHostID > -1)
					{
						customLabel.SetExprHost(customLabelsHostsRemotable[customLabel.ExpressionHostID], reportObjectModel);
					}
				}
			}
			if (m_gaugeMajorTickMarks != null && m_exprHost.GaugeMajorTickMarksHost != null)
			{
				m_gaugeMajorTickMarks.SetExprHost(m_exprHost.GaugeMajorTickMarksHost, reportObjectModel);
			}
			if (m_gaugeMinorTickMarks != null && m_exprHost.GaugeMinorTickMarksHost != null)
			{
				m_gaugeMinorTickMarks.SetExprHost(m_exprHost.GaugeMinorTickMarksHost, reportObjectModel);
			}
			if (m_maximumPin != null && m_exprHost.MaximumPinHost != null)
			{
				m_maximumPin.SetExprHost(m_exprHost.MaximumPinHost, reportObjectModel);
			}
			if (m_minimumPin != null && m_exprHost.MinimumPinHost != null)
			{
				m_minimumPin.SetExprHost(m_exprHost.MinimumPinHost, reportObjectModel);
			}
			if (m_scaleLabels != null && m_exprHost.ScaleLabelsHost != null)
			{
				m_scaleLabels.SetExprHost(m_exprHost.ScaleLabelsHost, reportObjectModel);
			}
			if (m_action != null && exprHost.ActionInfoHost != null)
			{
				m_action.SetExprHost(exprHost.ActionInfoHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Action, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.Action));
			list.Add(new MemberInfo(MemberName.ScaleRanges, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleRange));
			list.Add(new MemberInfo(MemberName.CustomLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.CustomLabel));
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Logarithmic, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LogarithmicBase, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MaximumValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.MinimumValue, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeInputValue));
			list.Add(new MemberInfo(MemberName.Multiplier, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Reversed, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.GaugeMajorTickMarks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks));
			list.Add(new MemberInfo(MemberName.GaugeMinorTickMarks, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeTickMarks));
			list.Add(new MemberInfo(MemberName.MaximumPin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin));
			list.Add(new MemberInfo(MemberName.MinimumPin, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScalePin));
			list.Add(new MemberInfo(MemberName.ScaleLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels));
			list.Add(new MemberInfo(MemberName.TickMarksOnTop, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ToolTip, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Width, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ExprHostID, Token.Int32));
			list.Add(new MemberInfo(MemberName.ID, Token.Int32));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Action:
					writer.Write(m_action);
					break;
				case MemberName.Name:
					writer.Write(m_name);
					break;
				case MemberName.ScaleRanges:
					writer.Write(m_scaleRanges);
					break;
				case MemberName.CustomLabels:
					writer.Write(m_customLabels);
					break;
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(m_intervalOffset);
					break;
				case MemberName.Logarithmic:
					writer.Write(m_logarithmic);
					break;
				case MemberName.LogarithmicBase:
					writer.Write(m_logarithmicBase);
					break;
				case MemberName.MaximumValue:
					writer.Write(m_maximumValue);
					break;
				case MemberName.MinimumValue:
					writer.Write(m_minimumValue);
					break;
				case MemberName.Multiplier:
					writer.Write(m_multiplier);
					break;
				case MemberName.Reversed:
					writer.Write(m_reversed);
					break;
				case MemberName.GaugeMajorTickMarks:
					writer.Write(m_gaugeMajorTickMarks);
					break;
				case MemberName.GaugeMinorTickMarks:
					writer.Write(m_gaugeMinorTickMarks);
					break;
				case MemberName.MaximumPin:
					writer.Write(m_maximumPin);
					break;
				case MemberName.MinimumPin:
					writer.Write(m_minimumPin);
					break;
				case MemberName.ScaleLabels:
					writer.Write(m_scaleLabels);
					break;
				case MemberName.TickMarksOnTop:
					writer.Write(m_tickMarksOnTop);
					break;
				case MemberName.ToolTip:
					writer.Write(m_toolTip);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.Width:
					writer.Write(m_width);
					break;
				case MemberName.ExprHostID:
					writer.Write(m_exprHostID);
					break;
				case MemberName.ID:
					writer.Write(m_id);
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
				case MemberName.Action:
					m_action = (Action)reader.ReadRIFObject();
					break;
				case MemberName.Name:
					m_name = reader.ReadString();
					break;
				case MemberName.ScaleRanges:
					m_scaleRanges = reader.ReadGenericListOfRIFObjects<ScaleRange>();
					break;
				case MemberName.CustomLabels:
					m_customLabels = reader.ReadGenericListOfRIFObjects<CustomLabel>();
					break;
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Logarithmic:
					m_logarithmic = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LogarithmicBase:
					m_logarithmicBase = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MaximumValue:
					m_maximumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.MinimumValue:
					m_minimumValue = (GaugeInputValue)reader.ReadRIFObject();
					break;
				case MemberName.Multiplier:
					m_multiplier = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Reversed:
					m_reversed = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.GaugeMajorTickMarks:
					m_gaugeMajorTickMarks = (GaugeTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.GaugeMinorTickMarks:
					m_gaugeMinorTickMarks = (GaugeTickMarks)reader.ReadRIFObject();
					break;
				case MemberName.MaximumPin:
					m_maximumPin = (ScalePin)reader.ReadRIFObject();
					break;
				case MemberName.MinimumPin:
					m_minimumPin = (ScalePin)reader.ReadRIFObject();
					break;
				case MemberName.ScaleLabels:
					m_scaleLabels = (ScaleLabels)reader.ReadRIFObject();
					break;
				case MemberName.TickMarksOnTop:
					m_tickMarksOnTop = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ToolTip:
					m_toolTip = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Width:
					m_width = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ExprHostID:
					m_exprHostID = reader.ReadInt32();
					break;
				case MemberName.ID:
					m_id = reader.ReadInt32();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override void ResolveReferences(Dictionary<Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			base.ResolveReferences(memberReferencesCollection, referenceableItems);
			if (m_id == 0)
			{
				m_id = m_gaugePanel.GenerateActionOwnerID();
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugeScale;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleIntervalExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleIntervalOffsetExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateLogarithmic(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleLogarithmicExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateLogarithmicBase(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleLogarithmicBaseExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateMultiplier(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleMultiplierExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateReversed(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleReversedExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateTickMarksOnTop(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleTickMarksOnTopExpression(this, m_gaugePanel.Name);
		}

		internal string EvaluateToolTip(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleToolTipExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleHiddenExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateWidth(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateGaugeScaleWidthExpression(this, m_gaugePanel.Name);
		}
	}
}
