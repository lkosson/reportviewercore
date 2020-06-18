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
	internal sealed class MapLegend : MapDockableSubItem, IPersistable
	{
		[NonSerialized]
		private static readonly Declaration m_Declaration = GetDeclaration();

		private ExpressionInfo m_layout;

		private MapLegendTitle m_mapLegendTitle;

		private ExpressionInfo m_autoFitTextDisabled;

		private ExpressionInfo m_minFontSize;

		private ExpressionInfo m_interlacedRows;

		private ExpressionInfo m_interlacedRowsColor;

		private ExpressionInfo m_equallySpacedItems;

		private ExpressionInfo m_textWrapThreshold;

		private string m_name;

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

		internal ExpressionInfo Layout
		{
			get
			{
				return m_layout;
			}
			set
			{
				m_layout = value;
			}
		}

		internal MapLegendTitle MapLegendTitle
		{
			get
			{
				return m_mapLegendTitle;
			}
			set
			{
				m_mapLegendTitle = value;
			}
		}

		internal ExpressionInfo AutoFitTextDisabled
		{
			get
			{
				return m_autoFitTextDisabled;
			}
			set
			{
				m_autoFitTextDisabled = value;
			}
		}

		internal ExpressionInfo MinFontSize
		{
			get
			{
				return m_minFontSize;
			}
			set
			{
				m_minFontSize = value;
			}
		}

		internal ExpressionInfo InterlacedRows
		{
			get
			{
				return m_interlacedRows;
			}
			set
			{
				m_interlacedRows = value;
			}
		}

		internal ExpressionInfo InterlacedRowsColor
		{
			get
			{
				return m_interlacedRowsColor;
			}
			set
			{
				m_interlacedRowsColor = value;
			}
		}

		internal ExpressionInfo EquallySpacedItems
		{
			get
			{
				return m_equallySpacedItems;
			}
			set
			{
				m_equallySpacedItems = value;
			}
		}

		internal ExpressionInfo TextWrapThreshold
		{
			get
			{
				return m_textWrapThreshold;
			}
			set
			{
				m_textWrapThreshold = value;
			}
		}

		internal new MapLegendExprHost ExprHost => (MapLegendExprHost)m_exprHost;

		internal MapLegend()
		{
		}

		internal MapLegend(Map map, int id)
			: base(map, id)
		{
		}

		internal override void Initialize(InitializationContext context)
		{
			context.ExprHostBuilder.MapLegendStart(m_name);
			base.Initialize(context);
			if (m_layout != null)
			{
				m_layout.Initialize("Layout", context);
				context.ExprHostBuilder.MapLegendLayout(m_layout);
			}
			if (m_mapLegendTitle != null)
			{
				m_mapLegendTitle.Initialize(context);
			}
			if (m_autoFitTextDisabled != null)
			{
				m_autoFitTextDisabled.Initialize("AutoFitTextDisabled", context);
				context.ExprHostBuilder.MapLegendAutoFitTextDisabled(m_autoFitTextDisabled);
			}
			if (m_minFontSize != null)
			{
				m_minFontSize.Initialize("MinFontSize", context);
				context.ExprHostBuilder.MapLegendMinFontSize(m_minFontSize);
			}
			if (m_interlacedRows != null)
			{
				m_interlacedRows.Initialize("InterlacedRows", context);
				context.ExprHostBuilder.MapLegendInterlacedRows(m_interlacedRows);
			}
			if (m_interlacedRowsColor != null)
			{
				m_interlacedRowsColor.Initialize("InterlacedRowsColor", context);
				context.ExprHostBuilder.MapLegendInterlacedRowsColor(m_interlacedRowsColor);
			}
			if (m_equallySpacedItems != null)
			{
				m_equallySpacedItems.Initialize("EquallySpacedItems", context);
				context.ExprHostBuilder.MapLegendEquallySpacedItems(m_equallySpacedItems);
			}
			if (m_textWrapThreshold != null)
			{
				m_textWrapThreshold.Initialize("TextWrapThreshold", context);
				context.ExprHostBuilder.MapLegendTextWrapThreshold(m_textWrapThreshold);
			}
			m_exprHostID = context.ExprHostBuilder.MapLegendEnd();
		}

		internal override object PublishClone(AutomaticSubtotalContext context)
		{
			MapLegend mapLegend = (MapLegend)base.PublishClone(context);
			if (m_layout != null)
			{
				mapLegend.m_layout = (ExpressionInfo)m_layout.PublishClone(context);
			}
			if (m_mapLegendTitle != null)
			{
				mapLegend.m_mapLegendTitle = (MapLegendTitle)m_mapLegendTitle.PublishClone(context);
			}
			if (m_autoFitTextDisabled != null)
			{
				mapLegend.m_autoFitTextDisabled = (ExpressionInfo)m_autoFitTextDisabled.PublishClone(context);
			}
			if (m_minFontSize != null)
			{
				mapLegend.m_minFontSize = (ExpressionInfo)m_minFontSize.PublishClone(context);
			}
			if (m_interlacedRows != null)
			{
				mapLegend.m_interlacedRows = (ExpressionInfo)m_interlacedRows.PublishClone(context);
			}
			if (m_interlacedRowsColor != null)
			{
				mapLegend.m_interlacedRowsColor = (ExpressionInfo)m_interlacedRowsColor.PublishClone(context);
			}
			if (m_equallySpacedItems != null)
			{
				mapLegend.m_equallySpacedItems = (ExpressionInfo)m_equallySpacedItems.PublishClone(context);
			}
			if (m_textWrapThreshold != null)
			{
				mapLegend.m_textWrapThreshold = (ExpressionInfo)m_textWrapThreshold.PublishClone(context);
			}
			return mapLegend;
		}

		internal void SetExprHost(MapLegendExprHost exprHost, ObjectModelImpl reportObjectModel)
		{
			Global.Tracer.Assert(exprHost != null && reportObjectModel != null, "(exprHost != null && reportObjectModel != null)");
			SetExprHost((MapDockableSubItemExprHost)exprHost, reportObjectModel);
			if (m_mapLegendTitle != null && ExprHost.MapLegendTitleHost != null)
			{
				m_mapLegendTitle.SetExprHost(ExprHost.MapLegendTitleHost, reportObjectModel);
			}
		}

		internal new static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.Name, Token.String));
			list.Add(new MemberInfo(MemberName.Layout, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MapLegendTitle, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegendTitle));
			list.Add(new MemberInfo(MemberName.AutoFitTextDisabled, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.MinFontSize, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRows, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.InterlacedRowsColor, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.EquallySpacedItems, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			list.Add(new MemberInfo(MemberName.TextWrapThreshold, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.ExpressionInfo));
			return new Declaration(Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend, Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapDockableSubItem, list);
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
				case MemberName.Layout:
					writer.Write(m_layout);
					break;
				case MemberName.MapLegendTitle:
					writer.Write(m_mapLegendTitle);
					break;
				case MemberName.AutoFitTextDisabled:
					writer.Write(m_autoFitTextDisabled);
					break;
				case MemberName.MinFontSize:
					writer.Write(m_minFontSize);
					break;
				case MemberName.InterlacedRows:
					writer.Write(m_interlacedRows);
					break;
				case MemberName.InterlacedRowsColor:
					writer.Write(m_interlacedRowsColor);
					break;
				case MemberName.EquallySpacedItems:
					writer.Write(m_equallySpacedItems);
					break;
				case MemberName.TextWrapThreshold:
					writer.Write(m_textWrapThreshold);
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
				case MemberName.Layout:
					m_layout = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MapLegendTitle:
					m_mapLegendTitle = (MapLegendTitle)reader.ReadRIFObject();
					break;
				case MemberName.AutoFitTextDisabled:
					m_autoFitTextDisabled = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.MinFontSize:
					m_minFontSize = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRows:
					m_interlacedRows = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.InterlacedRowsColor:
					m_interlacedRowsColor = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.EquallySpacedItems:
					m_equallySpacedItems = (ExpressionInfo)reader.ReadRIFObject();
					break;
				case MemberName.TextWrapThreshold:
					m_textWrapThreshold = (ExpressionInfo)reader.ReadRIFObject();
					break;
				default:
					Global.Tracer.Assert(condition: false);
					break;
				}
			}
		}

		public override Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType GetObjectType()
		{
			return Microsoft.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.MapLegend;
		}

		internal MapLegendLayout EvaluateLayout(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return EnumTranslator.TranslateMapLegendLayout(context.ReportRuntime.EvaluateMapLegendLayoutExpression(this, m_map.Name), context.ReportRuntime);
		}

		internal bool EvaluateAutoFitTextDisabled(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendAutoFitTextDisabledExpression(this, m_map.Name);
		}

		internal string EvaluateMinFontSize(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendMinFontSizeExpression(this, m_map.Name);
		}

		internal bool EvaluateInterlacedRows(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendInterlacedRowsExpression(this, m_map.Name);
		}

		internal string EvaluateInterlacedRowsColor(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendInterlacedRowsColorExpression(this, m_map.Name);
		}

		internal bool EvaluateEquallySpacedItems(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendEquallySpacedItemsExpression(this, m_map.Name);
		}

		internal int EvaluateTextWrapThreshold(IReportScopeInstance reportScopeInstance, OnDemandProcessingContext context)
		{
			context.SetupContext(m_map, reportScopeInstance);
			return context.ReportRuntime.EvaluateMapLegendTextWrapThresholdExpression(this, m_map.Name);
		}
	}
}
