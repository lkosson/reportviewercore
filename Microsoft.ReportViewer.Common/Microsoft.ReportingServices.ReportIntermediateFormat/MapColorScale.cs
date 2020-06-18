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
	internal sealed class MapColorScale : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private MapColorScaleTitle m_mapColorScaleTitle;

		private ExpressionInfo m_tickMarkLength;

		private ExpressionInfo m_colorBarBorderColor;

		private ExpressionInfo m_labelInterval;

		private ExpressionInfo m_labelFormat;

		private ExpressionInfo m_labelPlacement;

		private ExpressionInfo m_labelBehavior;

		private ExpressionInfo m_hideEndLabels;

		private ExpressionInfo m_rangeGapColor;

		private ExpressionInfo m_noDataText;

		internal MapColorScaleTitle MapColorScaleTitle
		{
			get
			{
				return m_mapColorScaleTitle;
			}
			set
			{
				m_mapColorScaleTitle = value;
			}
		}

		internal ExpressionInfo TickMarkLength
		{
			get
			{
				return m_tickMarkLength;
			}
			set
			{
				m_tickMarkLength = value;
			}
		}

		internal ExpressionInfo ColorBarBorderColor
		{
			get
			{
				return m_colorBarBorderColor;
			}
			set
			{
				m_colorBarBorderColor = value;
			}
		}

		internal ExpressionInfo LabelInterval
		{
			get
			{
				return m_labelInterval;
			}
			set
			{
				m_labelInterval = value;
			}
		}

		internal ExpressionInfo LabelFormat
		{
			get
			{
				return m_labelFormat;
			}
			set
			{
				m_labelFormat = value;
			}
		}

		internal ExpressionInfo LabelPlacement
		{
			get
			{
				return m_labelPlacement;
			}
			set
			{
				m_labelPlacement = value;
			}
		}

		internal ExpressionInfo LabelBehavior
		{
			get
			{
				return m_labelBehavior;
			}
			set
			{
				m_labelBehavior = value;
			}
		}

		internal ExpressionInfo HideEndLabels
		{
			get
			{
				return m_hideEndLabels;
			}
			set
			{
				m_hideEndLabels = value;
			}
		}

		internal ExpressionInfo RangeGapColor
		{
			get
			{
				return m_rangeGapColor;
			}
			set
			{
				m_rangeGapColor = value;
			}
		}

		internal ExpressionInfo NoDataText
		{
			get
			{
				return m_noDataText;
			}
			set
			{
				m_noDataText = value;
			}
		}

		internal new MapColorScaleExprHost ExprHost => (MapColorScaleExprHost)m_exprHost;

		internal MapColorScale()
		{
		}

		internal MapColorScale(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapColorScaleStart();
			base.Initialize(context);
			if (m_mapColorScaleTitle != null)
			{
				m_mapColorScaleTitle.Initialize(context);
			}
			if (m_tickMarkLength != null)
			{
				m_tickMarkLength.Initialize("TickMarkLength", context);
				context.ExprHostBuilder.MapColorScaleTickMarkLength(m_tickMarkLength);
			}
			if (m_colorBarBorderColor != null)
			{
				m_colorBarBorderColor.Initialize("ColorBarBorderColor", context);
				context.ExprHostBuilder.MapColorScaleColorBarBorderColor(m_colorBarBorderColor);
			}
			if (m_labelInterval != null)
			{
				m_labelInterval.Initialize("LabelInterval", context);
				context.ExprHostBuilder.MapColorScaleLabelInterval(m_labelInterval);
			}
			if (m_labelFormat != null)
			{
				m_labelFormat.Initialize("LabelFormat", context);
				context.ExprHostBuilder.MapColorScaleLabelFormat(m_labelFormat);
			}
			if (m_labelPlacement != null)
			{
				m_labelPlacement.Initialize("LabelPlacement", context);
				context.ExprHostBuilder.MapColorScaleLabelPlacement(m_labelPlacement);
			}
			if (m_labelBehavior != null)
			{
				m_labelBehavior.Initialize("LabelBehavior", context);
				context.ExprHostBuilder.MapColorScaleLabelBehavior(m_labelBehavior);
			}
			if (m_hideEndLabels != null)
			{
				m_hideEndLabels.Initialize("HideEndLabels", context);
				context.ExprHostBuilder.MapColorScaleHideEndLabels(m_hideEndLabels);
			}
			if (m_rangeGapColor != null)
			{
				m_rangeGapColor.Initialize("RangeGapColor", context);
				context.ExprHostBuilder.MapColorScaleRangeGapColor(m_rangeGapColor);
			}
			if (m_noDataText != null)
			{
				m_noDataText.Initialize("NoDataText", context);
				context.ExprHostBuilder.MapColorScaleNoDataText(m_noDataText);
			}
			context.ExprHostBuilder.MapColorScaleEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapColorScale mapColorScale = (MapColorScale)base.PublishClone(context);
			if (m_mapColorScaleTitle != null)
			{
				mapColorScale.m_mapColorScaleTitle = (MapColorScaleTitle)m_mapColorScaleTitle.PublishClone(context);
			}
			if (m_tickMarkLength != null)
			{
				mapColorScale.m_tickMarkLength = (ExpressionInfo)m_tickMarkLength.PublishClone(context);
			}
			if (m_colorBarBorderColor != null)
			{
				mapColorScale.m_colorBarBorderColor = (ExpressionInfo)m_colorBarBorderColor.PublishClone(context);
			}
			if (m_labelInterval != null)
			{
				mapColorScale.m_labelInterval = (ExpressionInfo)m_labelInterval.PublishClone(context);
			}
			if (m_labelFormat != null)
			{
				mapColorScale.m_labelFormat = (ExpressionInfo)m_labelFormat.PublishClone(context);
			}
			if (m_labelPlacement != null)
			{
				mapColorScale.m_labelPlacement = (ExpressionInfo)m_labelPlacement.PublishClone(context);
			}
			if (m_labelBehavior != null)
			{
				mapColorScale.m_labelBehavior = (ExpressionInfo)m_labelBehavior.PublishClone(context);
			}
			if (m_hideEndLabels != null)
			{
				mapColorScale.m_hideEndLabels = (ExpressionInfo)m_hideEndLabels.PublishClone(context);
			}
			if (m_rangeGapColor != null)
			{
				mapColorScale.m_rangeGapColor = (ExpressionInfo)m_rangeGapColor.PublishClone(context);
			}
			if (m_noDataText != null)
			{
				mapColorScale.m_noDataText = (ExpressionInfo)m_noDataText.PublishClone(context);
			}
			return mapColorScale;
		}

		internal void SetExprHost(MapColorScaleExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapDockableSubItemExprHost)exprHost, reportObjectModel);
			if (m_mapColorScaleTitle != null && ExprHost.MapColorScaleTitleHost != null)
			{
				m_mapColorScaleTitle.SetExprHost(ExprHost.MapColorScaleTitleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.MapColorScaleTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScaleTitle));
			list.Add(new MemberInfo(MemberName.TickMarkLength, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.ColorBarBorderColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelInterval, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelFormat, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelPlacement, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.LabelBehavior, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.HideEndLabels, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.RangeGapColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.NoDataText, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(m_Declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.MapColorScaleTitle:
					writer.Write(m_mapColorScaleTitle);
					break;
				case MemberName.TickMarkLength:
					writer.Write(m_tickMarkLength);
					break;
				case MemberName.ColorBarBorderColor:
					writer.Write(m_colorBarBorderColor);
					break;
				case MemberName.LabelInterval:
					writer.Write(m_labelInterval);
					break;
				case MemberName.LabelFormat:
					writer.Write(m_labelFormat);
					break;
				case MemberName.LabelPlacement:
					writer.Write(m_labelPlacement);
					break;
				case MemberName.LabelBehavior:
					writer.Write(m_labelBehavior);
					break;
				case MemberName.HideEndLabels:
					writer.Write(m_hideEndLabels);
					break;
				case MemberName.RangeGapColor:
					writer.Write(m_rangeGapColor);
					break;
				case MemberName.NoDataText:
					writer.Write(m_noDataText);
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
				case MemberName.MapColorScaleTitle:
					m_mapColorScaleTitle = (MapColorScaleTitle)reader.ReadRIFObject();
					break;
				case MemberName.TickMarkLength:
					m_tickMarkLength = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.ColorBarBorderColor:
					m_colorBarBorderColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelInterval:
					m_labelInterval = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelFormat:
					m_labelFormat = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelPlacement:
					m_labelPlacement = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.LabelBehavior:
					m_labelBehavior = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.HideEndLabels:
					m_hideEndLabels = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.RangeGapColor:
					m_rangeGapColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.NoDataText:
					m_noDataText = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapColorScale;
		}

		internal string EvaluateTickMarkLength(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleTickMarkLengthExpression(this, m_map.Name);
		}

		internal string EvaluateColorBarBorderColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleColorBarBorderColorExpression(this, m_map.Name);
		}

		internal int EvaluateLabelInterval(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleLabelIntervalExpression(this, m_map.Name);
		}

		internal string EvaluateLabelFormat(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleLabelFormatExpression(this, m_map.Name);
		}

		internal MapLabelPlacement EvaluateLabelPlacement(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelPlacement(context.ReportRuntime.EvaluateMapColorScaleLabelPlacementExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal MapLabelBehavior EvaluateLabelBehavior(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateLabelBehavior(context.ReportRuntime.EvaluateMapColorScaleLabelBehaviorExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateHideEndLabels(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleHideEndLabelsExpression(this, m_map.Name);
		}

		internal string EvaluateRangeGapColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleRangeGapColorExpression(this, m_map.Name);
		}

		internal string EvaluateNoDataText(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapColorScaleNoDataTextExpression(this, m_map.Name);
		}
	}
}
