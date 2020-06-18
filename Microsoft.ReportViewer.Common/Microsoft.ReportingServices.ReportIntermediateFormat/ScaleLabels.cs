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
	internal sealed class ScaleLabels : GaugePanelStyleContainer, IPersistable
	{
		[NonSerialized]
		private ScaleLabelsExprHost m_exprHost;

		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_interval;

		private ExpressionInfo m_intervalOffset;

		private ExpressionInfo m_allowUpsideDown;

		private ExpressionInfo m_distanceFromScale;

		private ExpressionInfo m_fontAngle;

		private ExpressionInfo m_placement;

		private ExpressionInfo m_rotateLabels;

		private ExpressionInfo m_showEndLabels;

		private ExpressionInfo m_hidden;

		private ExpressionInfo m_useFontPercent;

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

		internal ExpressionInfo AllowUpsideDown
		{
			get
			{
				return m_allowUpsideDown;
			}
			set
			{
				m_allowUpsideDown = value;
			}
		}

		internal ExpressionInfo DistanceFromScale
		{
			get
			{
				return m_distanceFromScale;
			}
			set
			{
				m_distanceFromScale = value;
			}
		}

		internal ExpressionInfo FontAngle
		{
			get
			{
				return m_fontAngle;
			}
			set
			{
				m_fontAngle = value;
			}
		}

		internal ExpressionInfo Placement
		{
			get
			{
				return m_placement;
			}
			set
			{
				m_placement = value;
			}
		}

		internal ExpressionInfo RotateLabels
		{
			get
			{
				return m_rotateLabels;
			}
			set
			{
				m_rotateLabels = value;
			}
		}

		internal ExpressionInfo ShowEndLabels
		{
			get
			{
				return m_showEndLabels;
			}
			set
			{
				m_showEndLabels = value;
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

		internal ExpressionInfo UseFontPercent
		{
			get
			{
				return m_useFontPercent;
			}
			set
			{
				m_useFontPercent = value;
			}
		}

		internal string OwnerName => m_gaugePanel.Name;

		internal ScaleLabelsExprHost ExprHost => m_exprHost;

		internal ScaleLabels()
		{
		}

		internal ScaleLabels(GaugePanel gaugePanel)
			: base(gaugePanel)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.ScaleLabelsStart();
			base.Initialize(context);
			if (m_interval != null)
			{
				m_interval.Initialize("Interval", context);
				context.ExprHostBuilder.ScaleLabelsInterval(m_interval);
			}
			if (m_intervalOffset != null)
			{
				m_intervalOffset.Initialize("IntervalOffset", context);
				context.ExprHostBuilder.ScaleLabelsIntervalOffset(m_intervalOffset);
			}
			if (m_allowUpsideDown != null)
			{
				m_allowUpsideDown.Initialize("AllowUpsideDown", context);
				context.ExprHostBuilder.ScaleLabelsAllowUpsideDown(m_allowUpsideDown);
			}
			if (m_distanceFromScale != null)
			{
				m_distanceFromScale.Initialize("DistanceFromScale", context);
				context.ExprHostBuilder.ScaleLabelsDistanceFromScale(m_distanceFromScale);
			}
			if (m_fontAngle != null)
			{
				m_fontAngle.Initialize("FontAngle", context);
				context.ExprHostBuilder.ScaleLabelsFontAngle(m_fontAngle);
			}
			if (m_placement != null)
			{
				m_placement.Initialize("Placement", context);
				context.ExprHostBuilder.ScaleLabelsPlacement(m_placement);
			}
			if (m_rotateLabels != null)
			{
				m_rotateLabels.Initialize("RotateLabels", context);
				context.ExprHostBuilder.ScaleLabelsRotateLabels(m_rotateLabels);
			}
			if (m_showEndLabels != null)
			{
				m_showEndLabels.Initialize("ShowEndLabels", context);
				context.ExprHostBuilder.ScaleLabelsShowEndLabels(m_showEndLabels);
			}
			if (m_hidden != null)
			{
				m_hidden.Initialize("Hidden", context);
				context.ExprHostBuilder.ScaleLabelsHidden(m_hidden);
			}
			if (m_useFontPercent != null)
			{
				m_useFontPercent.Initialize("UseFontPercent", context);
				context.ExprHostBuilder.ScaleLabelsUseFontPercent(m_useFontPercent);
			}
			context.ExprHostBuilder.ScaleLabelsEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			ScaleLabels scaleLabels = (ScaleLabels)base.PublishClone(context);
			if (m_interval != null)
			{
				scaleLabels.m_interval = (ExpressionInfo)m_interval.PublishClone(context);
			}
			if (m_intervalOffset != null)
			{
				scaleLabels.m_intervalOffset = (ExpressionInfo)m_intervalOffset.PublishClone(context);
			}
			if (m_allowUpsideDown != null)
			{
				scaleLabels.m_allowUpsideDown = (ExpressionInfo)m_allowUpsideDown.PublishClone(context);
			}
			if (m_distanceFromScale != null)
			{
				scaleLabels.m_distanceFromScale = (ExpressionInfo)m_distanceFromScale.PublishClone(context);
			}
			if (m_fontAngle != null)
			{
				scaleLabels.m_fontAngle = (ExpressionInfo)m_fontAngle.PublishClone(context);
			}
			if (m_placement != null)
			{
				scaleLabels.m_placement = (ExpressionInfo)m_placement.PublishClone(context);
			}
			if (m_rotateLabels != null)
			{
				scaleLabels.m_rotateLabels = (ExpressionInfo)m_rotateLabels.PublishClone(context);
			}
			if (m_showEndLabels != null)
			{
				scaleLabels.m_showEndLabels = (ExpressionInfo)m_showEndLabels.PublishClone(context);
			}
			if (m_hidden != null)
			{
				scaleLabels.m_hidden = (ExpressionInfo)m_hidden.PublishClone(context);
			}
			if (m_useFontPercent != null)
			{
				scaleLabels.m_useFontPercent = (ExpressionInfo)m_useFontPercent.PublishClone(context);
			}
			return scaleLabels;
		}

		internal void SetExprHost(ScaleLabelsExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null);
			base.SetExprHost(exprHost, reportObjectModel);
			m_exprHost = exprHost;
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Interval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.IntervalOffset, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.AllowUpsideDown, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.DistanceFromScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.FontAngle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Placement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RotateLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ShowEndLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.Hidden, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.UseFontPercent, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.GaugePanelStyleContainer, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.Interval:
					writer.Write(m_interval);
					break;
				case MemberName.IntervalOffset:
					writer.Write(m_intervalOffset);
					break;
				case MemberName.AllowUpsideDown:
					writer.Write(m_allowUpsideDown);
					break;
				case MemberName.DistanceFromScale:
					writer.Write(m_distanceFromScale);
					break;
				case MemberName.FontAngle:
					writer.Write(m_fontAngle);
					break;
				case MemberName.Placement:
					writer.Write(m_placement);
					break;
				case MemberName.RotateLabels:
					writer.Write(m_rotateLabels);
					break;
				case MemberName.ShowEndLabels:
					writer.Write(m_showEndLabels);
					break;
				case MemberName.Hidden:
					writer.Write(m_hidden);
					break;
				case MemberName.UseFontPercent:
					writer.Write(m_useFontPercent);
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
				case MemberName.Interval:
					m_interval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.IntervalOffset:
					m_intervalOffset = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.AllowUpsideDown:
					m_allowUpsideDown = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.DistanceFromScale:
					m_distanceFromScale = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.FontAngle:
					m_fontAngle = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Placement:
					m_placement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RotateLabels:
					m_rotateLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ShowEndLabels:
					m_showEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.Hidden:
					m_hidden = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.UseFontPercent:
					m_useFontPercent = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ScaleLabels;
		}

		internal double EvaluateInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsIntervalExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateIntervalOffset(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsIntervalOffsetExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateAllowUpsideDown(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsAllowUpsideDownExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateDistanceFromScale(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsDistanceFromScaleExpression(this, m_gaugePanel.Name);
		}

		internal double EvaluateFontAngle(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsFontAngleExpression(this, m_gaugePanel.Name);
		}

		internal GaugeLabelPlacements EvaluatePlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return EnumTranslator.TranslateGaugeLabelPlacements(context.ReportRuntime.EvaluateScaleLabelsPlacementExpression(this, m_gaugePanel.Name), context.ReportRuntime);
		}

		internal bool EvaluateRotateLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsRotateLabelsExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateShowEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsShowEndLabelsExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateHidden(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsHiddenExpression(this, m_gaugePanel.Name);
		}

		internal bool EvaluateUseFontPercent(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_gaugePanel, reportScopeInstance);
			return context.ReportRuntime.EvaluateScaleLabelsUseFontPercentExpression(this, m_gaugePanel.Name);
		}
	}
}
